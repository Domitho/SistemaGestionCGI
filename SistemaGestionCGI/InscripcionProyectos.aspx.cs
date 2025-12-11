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
        private readonly ManejadorProyectos _manejador = new ManejadorProyectos();

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
                List<InvgccInsPro> lista = _manejador.ObtenerTodos();
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

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            pnlGrilla.Visible = false;
            pnlFormulario.Visible = true;
            pnlEdicion.Visible = false;
            btnNuevo.Visible = false;
            btnRegresar.Visible = true;

            txtFecha.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtTema.Text = "";
            txtCoordinador.Text = "";
            txtDuracion.Text = "";
            ddlFacultad.SelectedIndex = 0;
            ddlGrupo.SelectedIndex = 0;
            ddlConv.SelectedIndex = 0;
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            Response.Redirect("InscripcionProyectos.aspx");
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTema.Text) || ddlFacultad.SelectedIndex == 0)
                {
                    Msg("Complete los campos obligatorios.", "ww");
                    return;
                }

                InvgccInsPro nuevo = new InvgccInsPro();
                nuevo.strTema_pro = txtTema.Text.Trim();
                nuevo.strCoordinador_pro = txtCoordinador.Text.Trim();
                nuevo.strFacultad_pro = ddlFacultad.SelectedValue;
                nuevo.strDuracion_pro = txtDuracion.Text.Trim();
                nuevo.dtFehains_pro = DateTime.Parse(txtFecha.Text);
                nuevo.fkId_gru = ddlGrupo.SelectedValue;
                nuevo.fkId_conv = ddlConv.SelectedValue;

                if (flpArchivo.HasFile)
                {
                    string ext = Path.GetExtension(flpArchivo.FileName).ToLower();
                    if (ext != ".pdf" && ext != ".xls" && ext != ".xlsx")
                    {
                        Msg("Formato de archivo no permitido.", "ww");
                        return;
                    }

                    string carpetaFisica = @"C:\UTC\PROYECTOS\";
                    if (!Directory.Exists(carpetaFisica)) Directory.CreateDirectory(carpetaFisica);
                    string nombre = "PROY_" + DateTime.Now.Ticks + ext;
                    string rutaCompleta = Path.Combine(carpetaFisica, nombre);
                    flpArchivo.SaveAs(rutaCompleta);
                    nuevo.strArchivo_pro = rutaCompleta;
                }

                _manejador.Guardar(nuevo);

                SetFlashMessage("Proyecto registrado correctamente.", "ss");
                Response.Redirect("InscripcionProyectos.aspx", false);
            }
            catch (Exception ex)
            {
                Msg("Error al guardar: " + ex.Message, "ee");
            }
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
                InvgccInsPro pro = _manejador.ObtenerPorId(id);
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
                InvgccInsPro pro = _manejador.ObtenerPorId(id);
                if (pro != null)
                {
                    pnlGrilla.Visible = false;
                    pnlFormulario.Visible = false;
                    pnlEdicion.Visible = true;
                    btnNuevo.Visible = false;
                    btnRegresar.Visible = true;

                    hfIdEdit.Value = pro.strId_pro;
                    txtTemaEdit.Text = pro.strTema_pro;
                    txtCoordinadorEdit.Text = pro.strCoordinador_pro;

                    if (ddlFacultadEdit.Items.FindByValue(pro.strFacultad_pro) != null)
                        ddlFacultadEdit.SelectedValue = pro.strFacultad_pro;

                    txtDuracionEdit.Text = pro.strDuracion_pro;
                    txtFechaEdit.Text = pro.dtFehains_pro.ToString("yyyy-MM-dd");

                    if (ddlGrupoEdit.Items.FindByValue(pro.fkId_gru) != null)
                        ddlGrupoEdit.SelectedValue = pro.fkId_gru;

                    if (ddlConvEdit.Items.FindByValue(pro.fkId_conv) != null)
                        ddlConvEdit.SelectedValue = pro.fkId_conv;

                    hfArchivoActual.Value = pro.strArchivo_pro;
                    lblArchivoActual.Text = string.IsNullOrEmpty(pro.strArchivo_pro) ? "Sin archivo" : pro.strArchivo_pro;
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
                InvgccInsPro edit = new InvgccInsPro();
                edit.strId_pro = hfIdEdit.Value;
                edit.strTema_pro = txtTemaEdit.Text.Trim();
                edit.strCoordinador_pro = txtCoordinadorEdit.Text.Trim();
                edit.strFacultad_pro = ddlFacultadEdit.SelectedValue;
                edit.strDuracion_pro = txtDuracionEdit.Text.Trim();
                edit.dtFehains_pro = DateTime.Parse(txtFechaEdit.Text);
                edit.fkId_gru = ddlGrupoEdit.SelectedValue;
                edit.fkId_conv = ddlConvEdit.SelectedValue;
                edit.strArchivo_pro = hfArchivoActual.Value;

                if (flpArchivoEdit.HasFile)
                {
                    string ext = Path.GetExtension(flpArchivoEdit.FileName).ToLower();
                    if (ext != ".pdf" && ext != ".xls" && ext != ".xlsx")
                    {
                        Msg("Formato inválido.", "ww");
                        return;
                    }

                    string carpetaFisica = @"C:\UTC\PROYECTOS\";
                    if (!Directory.Exists(carpetaFisica)) Directory.CreateDirectory(carpetaFisica);
                    string nombre = "PROY_" + DateTime.Now.Ticks + ext;
                    string rutaCompleta = Path.Combine(carpetaFisica, nombre);
                    flpArchivoEdit.SaveAs(rutaCompleta);
                    edit.strArchivo_pro = rutaCompleta;
                    string archivoAnterior = hfArchivoActual.Value;
                    if (!string.IsNullOrEmpty(archivoAnterior) && File.Exists(archivoAnterior) && Path.IsPathRooted(archivoAnterior))
                    {
                        try { File.Delete(archivoAnterior); } catch { /* Ignorar si falla borrado */ }
                    }
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
            string cleanMsg = msg.Replace("'", "\\'");
            string script = $"$(function() {{ toastify('{type}', '{cleanMsg}', 'Sistema'); }});";
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", script, true);
        }
    }
}