using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;

namespace SistemaGestionCGI
{
    public partial class InscripcionProyectos : System.Web.UI.Page
    {
        // 1. Instancias y Constantes
        private readonly ManejadorInscripcionProyectos _manejador = new ManejadorInscripcionProyectos();
        private const string RUTA_PROYECTOS = @"C:\UTC\PROYECTOS\";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            // Validar Sesión
            if (Session["UsuarioLogueado"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // Carga Inicial
            CargarCombos();
            CargarGrilla();

            // Mensajes Flash
            if (Session["TempMsg"] != null)
            {
                Msg(Session["TempMsg"].ToString(), Session["TempTipo"].ToString());
                Session["TempMsg"] = null;
                Session["TempTipo"] = null;
            }
        }

        // =============================================
        // CARGA DE DATOS
        // =============================================

        private void CargarGrilla()
        {
            try
            {
                rptProyectos.DataSource = _manejador.ObtenerTodos();
                rptProyectos.DataBind();
            }
            catch (Exception ex) { Msg("Error al cargar la grilla: " + ex.Message, "ee"); }
        }

        private void CargarCombos()
        {
            try
            {
                var grupos = _manejador.ObtenerGruposCombo();
                LlenarListControl(ddlGrupo, grupos, "strNombre_gru", "strId_gru");
                LlenarListControl(ddlGrupoEdit, grupos, "strNombre_gru", "strId_gru");

                var convocatorias = _manejador.ObtenerConvocatoriasCombo();
                LlenarListControl(ddlConv, convocatorias, "strNombre_conv", "strId_conv");
                LlenarListControl(ddlConvEdit, convocatorias, "strNombre_conv", "strId_conv");
            }
            catch (Exception ex) { Msg("Error al cargar combos: " + ex.Message, "ee"); }
        }

        private void LlenarListControl(ListControl ddl, object dataSource, string textField, string valueField)
        {
            ddl.DataSource = dataSource;
            ddl.DataTextField = textField;
            ddl.DataValueField = valueField;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("-- Seleccione --", ""));
        }

        private void CargarCoordinadores(DropDownList ddl, string idGrupo)
        {
            ddl.Items.Clear();
            if (!string.IsNullOrEmpty(idGrupo))
            {
                ddl.DataSource = _manejador.ObtenerIntegrantesPorGrupo(idGrupo);
                ddl.DataTextField = "NombreCompleto";
                ddl.DataValueField = "NombreCompleto";
                ddl.DataBind();
            }
            ddl.Items.Insert(0, new ListItem("-- Seleccione Coordinador --", ""));
        }

        // =============================================
        // EVENTOS DE INTERFAZ (DROPDOWNS)
        // =============================================

        protected void ddlGrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string idGrupo = ddlGrupo.SelectedValue;
            CargarCoordinadores(ddlCoordinador, idGrupo);

            // Mostrar Info Card
            if (!string.IsNullOrEmpty(idGrupo))
            {
                var info = _manejador.ObtenerInfoGrupo(idGrupo);
                if (info != null)
                {
                    lblNombreGrupoInfo.Text = info.strNombre_gru;
                    lblLineasInfo.Text = !string.IsNullOrEmpty(info.strLineasinv_gru) ? info.strLineasinv_gru : "General";
                    pnlInfoGrupo.Visible = true;
                    return;
                }
            }
            pnlInfoGrupo.Visible = false;
        }

        protected void ddlGrupoEdit_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarCoordinadores(ddlCoordinadorEdit, ddlGrupoEdit.SelectedValue);
        }

        // =============================================
        // GESTIÓN DE INTEGRANTES (MODAL)
        // =============================================

        protected void btnGuardarIntegrante_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ddlGrupo.SelectedValue))
                {
                    Msg("Error: No hay grupo seleccionado.", "ww");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtNombresInt.Text) || string.IsNullOrWhiteSpace(txtCedulaInt.Text))
                {
                    Msg("Complete los campos obligatorios.", "ww");
                    ScriptManager.RegisterStartupScript(this, GetType(), "reopen", "AbrirModalNuevoIntegrante();", true);
                    return;
                }

                // Construcción limpia del objeto
                var nuevo = new InvgccGrupoIntegrantes
                {
                    fkId_gru = ddlGrupo.SelectedValue,
                    strCedula_int = txtCedulaInt.Text.Trim(),
                    strNombres_int = txtNombresInt.Text.Trim(),
                    strApellidos_int = txtApellidosInt.Text.Trim(),
                    strCorreo_int = txtCorreoInt.Text.Trim(),
                    strFuncion_int = txtFuncionInt.Text.Trim(),
                    strObservacion_int = txtObservacionInt.Text.Trim(),
                    strTipo_int = ddlTipoInt.SelectedValue
                };

                if (nuevo.strTipo_int == "Externo")
                {
                    nuevo.strEntidad_int = txtEntidadInt.Text.Trim();
                    if (string.IsNullOrEmpty(nuevo.strEntidad_int))
                    {
                        Msg("Especifique la Institución para externos.", "ww");
                        ScriptManager.RegisterStartupScript(this, GetType(), "reopen", "AbrirModalNuevoIntegrante(); ToggleTipoIntegrante(document.getElementById('ddlTipoInt'));", true);
                        return;
                    }
                }
                else
                {
                    nuevo.strCarrera_int = txtCarreraInt.Text.Trim();
                    nuevo.strFacultad_int = ddlFacultadInt.SelectedValue;
                }

                _manejador.GuardarIntegranteExpress(nuevo);

                // Refrescar y Seleccionar automáticamente
                CargarCoordinadores(ddlCoordinador, ddlGrupo.SelectedValue);
                string nombreCompleto = $"{nuevo.strApellidos_int} {nuevo.strNombres_int}";
                var item = ddlCoordinador.Items.FindByText(nombreCompleto);
                if (item != null) item.Selected = true;

                // Limpiar Modal
                txtCedulaInt.Text = ""; txtNombresInt.Text = ""; txtApellidosInt.Text = "";
                txtCorreoInt.Text = ""; txtCarreraInt.Text = ""; txtFuncionInt.Text = ""; txtObservacionInt.Text = "";
                ddlFacultadInt.SelectedIndex = 0;

                Msg("Integrante registrado y seleccionado.", "ss");
            }
            catch (Exception ex) { Msg("Error al guardar integrante: " + ex.Message, "ee"); }
        }

        // =============================================
        // CRUD PRINCIPAL: GUARDAR Y ACTUALIZAR
        // =============================================

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            pnlGrilla.Visible = false;
            pnlFormulario.Visible = true;
            pnlEdicion.Visible = false;
            btnNuevo.Visible = false;
            btnRegresar.Visible = true;

            txtFecha.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtTema.Text = "";
            txtDuracion.Text = "";
            ddlGrupo.SelectedIndex = 0;
            ddlConv.SelectedIndex = 0;
            ddlCoordinador.Items.Clear();
            ddlCoordinador.Items.Add(new ListItem("-- Seleccione Grupo Primero --", ""));
            pnlInfoGrupo.Visible = false;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTema.Text) || ddlCoordinador.SelectedIndex <= 0)
                {
                    Msg("Complete los campos obligatorios.", "ww");
                    return;
                }

                var nuevo = new InvgccInscripcionProyectos
                {
                    strTema_pro = txtTema.Text.Trim(),
                    strCoordinador_pro = ddlCoordinador.SelectedValue,
                    strDuracion_pro = txtDuracion.Text.Trim(),
                    dtFehains_pro = DateTime.Parse(txtFecha.Text),
                    fkId_gru = ddlGrupo.SelectedValue,
                    fkId_conv = ddlConv.SelectedValue,
                    intPuntaje_pro = int.TryParse(txtPuntaje.Text, out int pt) ? (int?)pt : null
                };

                if (flpArchivo.HasFile)
                {
                    if (!ValidarExtension(flpArchivo.FileName)) return;
                    nuevo.strArchivo_pro = GuardarArchivoFisico(flpArchivo, $"PROY_{DateTime.Now.Ticks}{Path.GetExtension(flpArchivo.FileName)}");
                }

                _manejador.Guardar(nuevo);
                Redireccionar("Proyecto registrado correctamente.", "ss");
            }
            catch (Exception ex) { Msg("Error: " + ex.Message, "ee"); }
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            try
            {
                var edit = new InvgccInscripcionProyectos
                {
                    strId_pro = hfIdEdit.Value,
                    strTema_pro = txtTemaEdit.Text.Trim(),
                    strCoordinador_pro = ddlCoordinadorEdit.SelectedValue,
                    strDuracion_pro = txtDuracionEdit.Text.Trim(),
                    dtFehains_pro = DateTime.Parse(txtFechaEdit.Text),
                    fkId_gru = ddlGrupoEdit.SelectedValue,
                    fkId_conv = ddlConvEdit.SelectedValue,
                    strArchivo_pro = hfArchivoActual.Value,
                    intPuntaje_pro = int.TryParse(txtPuntajeEdit.Text, out int pt) ? (int?)pt : null
                };

                if (flpArchivoEdit.HasFile)
                {
                    if (!ValidarExtension(flpArchivoEdit.FileName)) return;
                    edit.strArchivo_pro = GuardarArchivoFisico(flpArchivoEdit, $"PROY_{DateTime.Now.Ticks}{Path.GetExtension(flpArchivoEdit.FileName)}");
                }

                _manejador.Actualizar(edit);
                Redireccionar("Proyecto actualizado.", "ss");
            }
            catch (Exception ex) { Msg("Error al actualizar: " + ex.Message, "ee"); }
        }

        // =============================================
        // ACCIONES DE GRILLA (COMANDOS)
        // =============================================

        protected void rptProyectos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();

            switch (e.CommandName)
            {
                case "ver":
                    DescargarArchivo(id);
                    break;

                case "eliminar":
                    try
                    {
                        _manejador.Eliminar(id);
                        Redireccionar("Proyecto eliminado.", "ss");
                    }
                    catch (Exception ex) { Msg("Error al eliminar: " + ex.Message, "ee"); }
                    break;

                case "estado":
                    CargarModalEstado(id);
                    break;

                case "editar":
                    CargarEdicion(id);
                    break;
            }
        }

        private void CargarEdicion(string id)
        {
            var pro = _manejador.ObtenerPorId(id);
            if (pro == null) return;

            hfIdEdit.Value = pro.strId_pro;
            txtTemaEdit.Text = pro.strTema_pro;
            txtPuntajeEdit.Text = pro.intPuntaje_pro?.ToString() ?? "";
            txtDuracionEdit.Text = pro.strDuracion_pro;
            txtFechaEdit.Text = pro.dtFehains_pro.ToString("yyyy-MM-dd");
            hfArchivoActual.Value = pro.strArchivo_pro;
            lblArchivoActual.Text = string.IsNullOrEmpty(pro.strArchivo_pro) ? "Sin archivo" : Path.GetFileName(pro.strArchivo_pro);

            if (ddlConvEdit.Items.FindByValue(pro.fkId_conv) != null)
                ddlConvEdit.SelectedValue = pro.fkId_conv;

            if (ddlGrupoEdit.Items.FindByValue(pro.fkId_gru) != null)
            {
                ddlGrupoEdit.SelectedValue = pro.fkId_gru;
                CargarCoordinadores(ddlCoordinadorEdit, pro.fkId_gru);
                if (ddlCoordinadorEdit.Items.FindByValue(pro.strCoordinador_pro) != null)
                    ddlCoordinadorEdit.SelectedValue = pro.strCoordinador_pro;
            }

            pnlGrilla.Visible = false;
            pnlFormulario.Visible = false;
            pnlEdicion.Visible = true;
            btnNuevo.Visible = false;
            btnRegresar.Visible = true;
        }

        private void CargarModalEstado(string id)
        {
            var pro = _manejador.ObtenerPorId(id);
            if (pro != null)
            {
                hfIdProyectoEstado.Value = pro.strId_pro;
                string script = $@"
                    document.getElementById('infoIdPro').innerText = '{pro.strId_pro}';
                    document.getElementById('infoTemaPro').innerText = '{pro.strTema_pro}';
                    document.getElementById('infoEstadoPro').innerText = '{pro.strEstado_pro}';
                    AbrirModalEstadoPro();";
                ScriptManager.RegisterStartupScript(this, GetType(), "modalEstado", script, true);
            }
        }

        protected void btnConfirmarEstadoPro_Click(object sender, EventArgs e)
        {
            try
            {
                _manejador.AlternarEstado(hfIdProyectoEstado.Value);
                Redireccionar("Estado actualizado correctamente.", "ss");
            }
            catch (Exception ex) { Msg("Error al cambiar estado: " + ex.Message, "ee"); }
        }

        // =============================================
        // NAVEGACIÓN Y CANCELACIÓN
        // =============================================

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            Response.Redirect("InscripcionProyectos.aspx");
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            btnRegresar_Click(sender, e);
        }

        protected void btnCancelarEdit_Click(object sender, EventArgs e)
        {
            btnRegresar_Click(sender, e);
        }

        // =============================================
        // UTILIDADES Y ARCHIVOS
        // =============================================

        private bool ValidarExtension(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLower();
            if (ext != ".pdf" && ext != ".xls" && ext != ".xlsx")
            {
                Msg("Formato no permitido (Solo PDF, XLS, XLSX).", "ww");
                return false;
            }
            return true;
        }

        private string GuardarArchivoFisico(FileUpload ctl, string nombre)
        {
            if (!Directory.Exists(RUTA_PROYECTOS)) Directory.CreateDirectory(RUTA_PROYECTOS);
            string ruta = Path.Combine(RUTA_PROYECTOS, nombre);
            ctl.SaveAs(ruta);
            return ruta;
        }

        private void DescargarArchivo(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            string rutaFisica = (!path.StartsWith("~") && Path.IsPathRooted(path)) ? path : Server.MapPath(path);

            if (File.Exists(rutaFisica))
            {
                string ext = Path.GetExtension(rutaFisica).ToLower();
                Response.Clear();
                switch (ext)
                {
                    case ".pdf": Response.ContentType = "application/pdf"; break;
                    case ".xls": Response.ContentType = "application/vnd.ms-excel"; break;
                    case ".xlsx": Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; break;
                    default: Response.ContentType = "application/octet-stream"; break;
                }
                Response.AddHeader("Content-Disposition", "inline; filename=" + Path.GetFileName(rutaFisica));
                Response.WriteFile(rutaFisica);
                Response.End();
            }
            else
            {
                Msg("El archivo no existe en la ruta física.", "ww");
            }
        }

        private void Redireccionar(string msg, string type)
        {
            Session["TempMsg"] = msg;
            Session["TempTipo"] = type;
            Response.Redirect("InscripcionProyectos.aspx", false);
        }

        private void Msg(string msg, string type)
        {
            if (string.IsNullOrEmpty(msg)) return;
            string cleanMsg = msg.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\r\n", " ").Replace("\n", " ");
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"$(function() {{ toastify('{type}', '{cleanMsg}', 'Sistema'); }});", true);
        }
    }
}