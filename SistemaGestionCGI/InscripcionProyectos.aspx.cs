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
        private readonly ManejadorInscripcionProyectos _manejador = new ManejadorInscripcionProyectos();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UsuarioLogueado"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                CargarCombos();
                CargarGrilla();

                if (Session["TempMsg"] != null)
                {
                    Msg(Session["TempMsg"].ToString(), Session["TempTipo"].ToString());
                    Session["TempMsg"] = null;
                    Session["TempTipo"] = null;
                }
            }
        }

        private void CargarGrilla()
        {
            try
            {
                List<InvgccInscripcionProyectos> lista = _manejador.ObtenerTodos();
                rptProyectos.DataSource = lista;
                rptProyectos.DataBind();
            }
            catch (Exception ex)
            {
                Msg("Error al cargar la grilla: " + ex.Message, "ee");
            }
        }

        private void CargarCombos()
        {
            try
            {
                // 1. Cargar Grupos desde BLL
                var grupos = _manejador.ObtenerGruposCombo();

                LlenarListControl(ddlGrupo, grupos, "strNombre_gru", "strId_gru");
                LlenarListControl(ddlGrupoEdit, grupos, "strNombre_gru", "strId_gru");

                // 2. Cargar Convocatorias desde BLL
                var convocatorias = _manejador.ObtenerConvocatoriasCombo();

                LlenarListControl(ddlConv, convocatorias, "strNombre_conv", "strId_conv");
                LlenarListControl(ddlConvEdit, convocatorias, "strNombre_conv", "strId_conv");
            }
            catch (Exception ex)
            {
                Msg("Error al cargar combos: " + ex.Message, "ee");
            }
        }

        // Método genérico limpio para llenar DropDowns
        private void LlenarListControl(ListControl ddl, object dataSource, string textField, string valueField)
        {
            ddl.DataSource = dataSource;
            ddl.DataTextField = textField;
            ddl.DataValueField = valueField;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("-- Seleccione --", ""));
        }

        protected void ddlGrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string idGrupo = ddlGrupo.SelectedValue;

            // 1. Lógica existente: Cargar coordinadores
            CargarCoordinadores(ddlCoordinador, idGrupo);

            // 2. NUEVA LÓGICA: Mostrar InfoCard del Grupo
            if (!string.IsNullOrEmpty(idGrupo))
            {
                var infoGrupo = _manejador.ObtenerInfoGrupo(idGrupo); // Debes crear este método en BLL
                if (infoGrupo != null)
                {
                    lblNombreGrupoInfo.Text = infoGrupo.strNombre_gru;
                    // Asegúrate que tu modelo Grupo tenga strLineasinv_gru, si no, usa otra propiedad
                    lblLineasInfo.Text = !string.IsNullOrEmpty(infoGrupo.strLineasinv_gru) ? infoGrupo.strLineasinv_gru : "General";
                    pnlInfoGrupo.Visible = true;
                }
            }
            else
            {
                pnlInfoGrupo.Visible = false;
            }
        }

        protected void btnGuardarIntegrante_Click(object sender, EventArgs e)
        {
            try
            {
                string idGrupo = ddlGrupo.SelectedValue;
                if (string.IsNullOrEmpty(idGrupo)) { Msg("Error: No hay grupo seleccionado.", "ww"); return; }

                // 1. Validaciones básicas
                if (string.IsNullOrWhiteSpace(txtNombresInt.Text) ||
                    string.IsNullOrWhiteSpace(txtApellidosInt.Text) ||
                    string.IsNullOrWhiteSpace(txtCedulaInt.Text))
                {
                    Msg("Complete los campos obligatorios (Cédula, Nombres, Apellidos).", "ww");
                    // Mantener modal abierto en caso de error (opcional)
                    ScriptManager.RegisterStartupScript(this, GetType(), "reopen", "AbrirModalNuevoIntegrante();", true);
                    return;
                }

                // 2. Llenar Objeto
                InvgccGrupoIntegrantes nuevo = new InvgccGrupoIntegrantes();
                nuevo.fkId_gru = idGrupo;
                nuevo.strCedula_int = txtCedulaInt.Text.Trim();
                nuevo.strNombres_int = txtNombresInt.Text.Trim();
                nuevo.strApellidos_int = txtApellidosInt.Text.Trim();
                nuevo.strCorreo_int = txtCorreoInt.Text.Trim(); 
                nuevo.strFuncion_int = txtFuncionInt.Text.Trim();
                nuevo.strObservacion_int = txtObservacionInt.Text.Trim();
                nuevo.strTipo_int = ddlTipoInt.SelectedValue;

                if (nuevo.strTipo_int == "Externo")
                {
                    // Si es Externo: Guardamos Entidad, Limpiamos datos académicos UTC
                    nuevo.strEntidad_int = txtEntidadInt.Text.Trim();
                    nuevo.strCarrera_int = null;
                    nuevo.strFacultad_int = null;

                    if (string.IsNullOrEmpty(nuevo.strEntidad_int))
                    {
                        Msg("Debe especificar la Institución de Origen para externos.", "ww");
                        ScriptManager.RegisterStartupScript(this, GetType(), "reopen", "AbrirModalNuevoIntegrante(); ToggleTipoIntegrante();", true);
                        return;
                    }
                }
                else
                {
                    nuevo.strEntidad_int = null;
                    nuevo.strCarrera_int = txtCarreraInt.Text.Trim();
                    nuevo.strFacultad_int = ddlFacultadInt.SelectedValue;
                }

                // 3. Guardar
                _manejador.GuardarIntegranteExpress(nuevo);

                // 4. Recargar Combo Coordinadores
                CargarCoordinadores(ddlCoordinador, idGrupo);

                // 5. Autoseleccionar al nuevo (Buscando por Nombre Completo)
                string nombreCompleto = nuevo.strApellidos_int + " " + nuevo.strNombres_int;
                ListItem item = ddlCoordinador.Items.FindByText(nombreCompleto);
                if (item != null) item.Selected = true;

                // 6. Limpiar campos
                txtCedulaInt.Text = ""; txtNombresInt.Text = ""; txtApellidosInt.Text = "";
                txtCorreoInt.Text = ""; txtCarreraInt.Text = ""; txtFuncionInt.Text = ""; txtObservacionInt.Text = "";
                ddlFacultadInt.SelectedIndex = 0;

                Msg("Integrante registrado y seleccionado.", "ss");
            }
            catch (Exception ex)
            {
                Msg("Error al guardar integrante: " + ex.Message, "ee");
            }
        }

        protected void ddlGrupoEdit_SelectedIndexChanged(object sender, EventArgs e)
        {
            string idGrupo = ddlGrupoEdit.SelectedValue;
            CargarCoordinadores(ddlCoordinadorEdit, idGrupo);
        }

        private void CargarCoordinadores(DropDownList ddl, string idGrupo)
        {
            ddl.Items.Clear();
            if (!string.IsNullOrEmpty(idGrupo))
            {
                var integrantes = _manejador.ObtenerIntegrantesPorGrupo(idGrupo);

                // Value = Nombre Completo (Porque en la tabla PROYECTO guardas el nombre, no el ID)
                // Text = Nombre Completo
                // NOTA: Si quisieras guardar el ID del integrante, cambia DataValueField a "strId_int"

                ddl.DataSource = integrantes;
                ddl.DataTextField = "NombreCompleto";
                ddl.DataValueField = "NombreCompleto"; // Guardaremos el NOMBRE en la tabla Proyecto
                ddl.DataBind();
            }
            ddl.Items.Insert(0, new ListItem("-- Seleccione Coordinador --", ""));
        }

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
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            Response.Redirect("InscripcionProyectos.aspx");
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validamos que haya seleccionado coordinador
                if (string.IsNullOrWhiteSpace(txtTema.Text) || ddlCoordinador.SelectedIndex <= 0)
                {
                    Msg("Complete los campos obligatorios (Tema, Facultad, Coordinador).", "ww");
                    return;
                }

                InvgccInscripcionProyectos nuevo = new InvgccInscripcionProyectos();
                nuevo.strTema_pro = txtTema.Text.Trim();

                // AHORA TOMAMOS EL VALOR DEL DROPDOWNLIST
                nuevo.strCoordinador_pro = ddlCoordinador.SelectedValue;

                nuevo.strDuracion_pro = txtDuracion.Text.Trim();
                nuevo.dtFehains_pro = DateTime.Parse(txtFecha.Text);
                nuevo.fkId_gru = ddlGrupo.SelectedValue;
                nuevo.fkId_conv = ddlConv.SelectedValue;

                // (Lógica de archivo se mantiene igual...)
                if (flpArchivo.HasFile)
                {
                    string ext = Path.GetExtension(flpArchivo.FileName).ToLower();
                    if (ext != ".pdf" && ext != ".xls" && ext != ".xlsx") { Msg("Formato no permitido.", "ww"); return; }
                    string nombre = "PROY_" + DateTime.Now.Ticks + ext;
                    nuevo.strArchivo_pro = GuardarArchivo(flpArchivo, nombre);
                }

                // LÓGICA PUNTAJE OPCIONAL
                if (!string.IsNullOrEmpty(txtPuntaje.Text))
                {
                    if (int.TryParse(txtPuntaje.Text, out int puntos))
                        nuevo.intPuntaje_pro = puntos;
                    
                }
                else
                {
                    nuevo.intPuntaje_pro = null;
                }

                _manejador.Guardar(nuevo);
                SetFlashMessage("Proyecto registrado correctamente.", "ss");
                Response.Redirect("InscripcionProyectos.aspx", false);
            }
            catch (Exception ex) { Msg("Error: " + ex.Message, "ee"); }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("InscripcionProyectos.aspx");
        }

        protected void rptProyectos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();

            if (e.CommandName == "ver")
            {
                string archivo = id; 
                if (!string.IsNullOrEmpty(archivo))
                {
                    string rutaFisica = "";

                    if (Path.IsPathRooted(archivo) && !archivo.StartsWith("~"))
                    {
                        rutaFisica = archivo;
                    }
                    else
                    {
                        rutaFisica = Server.MapPath(archivo);
                    }

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
                        Msg("El archivo no existe en la ruta física: " + rutaFisica, "ww");
                    }
                }
            }
            else if (e.CommandName == "eliminar")
            {
                try
                {
                    _manejador.Eliminar(id);
                    SetFlashMessage("Proyecto eliminado.", "ss");
                    Response.Redirect("InscripcionProyectos.aspx", false);
                }
                catch (Exception ex)
                {
                    Msg("Error al eliminar: " + ex.Message, "ee");
                }
            }
            else if (e.CommandName == "estado")
            {
                InvgccInscripcionProyectos pro = _manejador.ObtenerPorId(id);
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
            else if (e.CommandName == "editar")
            {
                // CORRECCIÓN: Ya no declaramos 'string id' aquí, usamos la de arriba.
                var pro = _manejador.ObtenerPorId(id);
                if (pro != null)
                {
                    hfIdEdit.Value = pro.strId_pro;
                    txtTemaEdit.Text = pro.strTema_pro;

                    if (pro.intPuntaje_pro.HasValue)
                        txtPuntajeEdit.Text = pro.intPuntaje_pro.Value.ToString();
                    else
                        txtPuntajeEdit.Text = "";

                    txtDuracionEdit.Text = pro.strDuracion_pro;
                    txtFechaEdit.Text = pro.dtFehains_pro.ToString("yyyy-MM-dd");

                    if (ddlConvEdit.Items.FindByValue(pro.fkId_conv) != null)
                        ddlConvEdit.SelectedValue = pro.fkId_conv;

                    hfArchivoActual.Value = pro.strArchivo_pro;
                    lblArchivoActual.Text = string.IsNullOrEmpty(pro.strArchivo_pro) ? "Sin archivo" : Path.GetFileName(pro.strArchivo_pro);

                    // LOGICA DE COMBOS EN CASCADA
                    if (ddlGrupoEdit.Items.FindByValue(pro.fkId_gru) != null)
                    {
                        ddlGrupoEdit.SelectedValue = pro.fkId_gru;

                        // Cargar integrantes del grupo seleccionado
                        CargarCoordinadores(ddlCoordinadorEdit, pro.fkId_gru);

                        if (ddlCoordinadorEdit.Items.FindByValue(pro.strCoordinador_pro) != null)
                        {
                            ddlCoordinadorEdit.SelectedValue = pro.strCoordinador_pro;
                        }
                    }

                    pnlGrilla.Visible = false;
                    pnlFormulario.Visible = false;
                    pnlEdicion.Visible = true;
                    btnNuevo.Visible = false;
                    btnRegresar.Visible = true;
                }
            }
        }

        protected void btnConfirmarEstadoPro_Click(object sender, EventArgs e)
        {
            try
            {
                string id = hfIdProyectoEstado.Value;
                _manejador.AlternarEstado(id);
                SetFlashMessage("Estado actualizado correctamente.", "ss");
                Response.Redirect("InscripcionProyectos.aspx", false);
            }
            catch (Exception ex)
            {
                Msg("Error al cambiar estado: " + ex.Message, "ee");
            }
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            try
            {
                InvgccInscripcionProyectos edit = new InvgccInscripcionProyectos();
                edit.strId_pro = hfIdEdit.Value;
                edit.strTema_pro = txtTemaEdit.Text.Trim();

                // AHORA TOMAMOS EL COORDINADOR DEL COMBO EDIT
                edit.strCoordinador_pro = ddlCoordinadorEdit.SelectedValue;

                edit.strDuracion_pro = txtDuracionEdit.Text.Trim();
                edit.dtFehains_pro = DateTime.Parse(txtFechaEdit.Text);
                edit.fkId_gru = ddlGrupoEdit.SelectedValue;
                edit.fkId_conv = ddlConvEdit.SelectedValue;
                edit.strArchivo_pro = hfArchivoActual.Value;

                if (flpArchivoEdit.HasFile)
                {
                    string nombre = "PROY_" + DateTime.Now.Ticks + Path.GetExtension(flpArchivoEdit.FileName);
                    edit.strArchivo_pro = GuardarArchivo(flpArchivoEdit, nombre);
                }

                if (!string.IsNullOrEmpty(txtPuntajeEdit.Text))
                {
                    if (int.TryParse(txtPuntajeEdit.Text, out int puntos))
                        edit.intPuntaje_pro = puntos;
                    

                }
                else
                {
                    edit.intPuntaje_pro = null;
                }

                _manejador.Actualizar(edit);
                SetFlashMessage("Proyecto actualizado.", "ss");
                Response.Redirect("InscripcionProyectos.aspx", false);
            }
            catch (Exception ex)
            {
                Msg("Error al actualizar: " + ex.Message, "ee");
            }
        }

        protected void btnCancelarEdit_Click(object sender, EventArgs e)
        {
            Response.Redirect("InscripcionProyectos.aspx");
        }

        // ================= HELPER METHODS =================

        private void SetFlashMessage(string msg, string type)
        {
            Session["TempMsg"] = msg;
            Session["TempTipo"] = type;
        }

        private void Msg(string msg, string type)
        {
            if (string.IsNullOrEmpty(msg)) return;

            // 1. PRIMERO reemplaza las barras invertidas (para no dañar los escapes posteriores)
            string cleanMsg = msg.Replace("\\", "\\\\");

            // 2. LUEGO reemplaza las comillas simples
            cleanMsg = cleanMsg.Replace("'", "\\'");

            // 3. FINALMENTE elimina todos los tipos de saltos de línea
            cleanMsg = cleanMsg.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");

            string script = $"$(function() {{ toastify('{type}', '{cleanMsg}', 'Sistema'); }});";
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", script, true);
        }

        private string GuardarArchivo(FileUpload ctl, string nombre)
        {
            string carpeta = @"C:\UTC\PROYECTOS\";
            if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);
            string ruta = Path.Combine(carpeta, nombre);
            ctl.SaveAs(ruta);
            return ruta;
        }
    }
}