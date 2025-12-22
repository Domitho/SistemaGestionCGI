using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;

namespace SistemaGestionCGI
{
    public partial class ConvocatoriaGruInvestigacion : System.Web.UI.Page
    {
        // 1. Instancias
        private readonly ManejadorConvocatorias _manejador = new ManejadorConvocatorias();
        private const string RUTA_CONVOCATORIAS = @"C:\UTC\CONVOCATORIAS\";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            // Seguridad
            if (Session["UsuarioLogueado"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // Carga Inicial
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
        // LECTURA DE DATOS
        // =============================================

        private void CargarGrilla()
        {
            try
            {
                rptConvocatorias.DataSource = _manejador.ObtenerConvocatorias();
                rptConvocatorias.DataBind();
            }
            catch (Exception ex) { Msg("Error al cargar convocatorias: " + ex.Message, "ee"); }
        }

        // =============================================
        // CRUD CONVOCATORIAS
        // =============================================

        protected void lbtNuevaConv_Click(object sender, EventArgs e)
        {
            CambiarVista(Vista.Agregar);
            txtNombreAdd.Text = "";
            txtDescAdd.Text = "";
            txtFechaIniAdd.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        protected void lbtGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!flpArchivoAdd.HasFile)
                {
                    Msg("Debe adjuntar el archivo de bases.", "ww");
                    return;
                }

                if (!ValidarArchivo(flpArchivoAdd.FileName)) return;

                var conv = new InvgccConvocatoriaGruInvestigacion
                {
                    strNombre_conv = txtNombreAdd.Text.Trim(),
                    dtFechaini_conv = DateTime.Parse(txtFechaIniAdd.Text),
                    strDescripcion_conv = HttpUtility.HtmlEncode(txtDescAdd.Text), // XSS Protection
                    strArchivo_conv = GuardarArchivoFisico(flpArchivoAdd, $"CONV_{DateTime.Now.Ticks}{Path.GetExtension(flpArchivoAdd.FileName)}")
                };

                // Asignamos una fecha "vacía" para cumplir con el modelo si es requerido
                conv.dtFechafin_conv = new DateTime(1900, 1, 1);

                _manejador.GuardarConvocatoria(conv);
                Redireccionar("Convocatoria creada exitosamente.", "ss");
            }
            catch (Exception ex) { Msg("Error al guardar: " + ex.Message, "ee"); }
        }

        protected void lbtActualizar_Click(object sender, EventArgs e)
        {
            try
            {
                var conv = new InvgccConvocatoriaGruInvestigacion
                {
                    strId_conv = hfIdConvEdit.Value,
                    strNombre_conv = txtNombreEdit.Text.Trim(),
                    dtFechaini_conv = DateTime.Parse(txtFechaIniEdit.Text),
                    strDescripcion_conv = HttpUtility.HtmlEncode(txtDescEdit.Text),
                    strArchivo_conv = hfArchivoActual.Value,
                    dtFechafin_conv = new DateTime(1900, 1, 1) // Fecha dummy segura
                };

                if (flpArchivoEdit.HasFile)
                {
                    if (!ValidarArchivo(flpArchivoEdit.FileName)) return;
                    conv.strArchivo_conv = GuardarArchivoFisico(flpArchivoEdit, $"CONV_{DateTime.Now.Ticks}{Path.GetExtension(flpArchivoEdit.FileName)}");
                }

                _manejador.ActualizarConvocatoria(conv);
                Redireccionar("Convocatoria actualizada.", "ss");
            }
            catch (Exception ex) { Msg("Error al actualizar: " + ex.Message, "ee"); }
        }

        protected void rptConvocatorias_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();

            switch (e.CommandName)
            {
                case "Editar":
                    CargarEdicion(id);
                    break;

                case "Eliminar":
                    try
                    {
                        var obj = _manejador.ObtenerConvocatoriaPorId(id);
                        if (obj != null && File.Exists(obj.strArchivo_conv))
                        {
                            try { File.Delete(obj.strArchivo_conv); } catch { }
                        }

                        _manejador.EliminarConvocatoria(id);
                        Redireccionar("Convocatoria eliminada.", "ss");
                    }
                    catch (Exception ex) { Msg("Error al eliminar: " + ex.Message, "ee"); }
                    break;

                case "VerArchivo":
                    var conv = _manejador.ObtenerConvocatoriaPorId(id);
                    if (conv != null && !string.IsNullOrEmpty(conv.strArchivo_conv))
                    {
                        DescargarArchivo(conv.strArchivo_conv);
                    }
                    else
                    {
                        Msg("No hay archivo adjunto.", "ww");
                    }
                    break;
            }
        }

        private void CargarEdicion(string id)
        {
            var conv = _manejador.ObtenerConvocatoriaPorId(id);
            if (conv != null)
            {
                hfIdConvEdit.Value = conv.strId_conv;
                txtNombreEdit.Text = conv.strNombre_conv;
                txtFechaIniEdit.Text = conv.dtFechaini_conv.ToString("yyyy-MM-dd");
                txtDescEdit.Text = HttpUtility.HtmlDecode(conv.strDescripcion_conv);
                hfArchivoActual.Value = conv.strArchivo_conv;

                CambiarVista(Vista.Editar);
            }
        }

        // =============================================
        // NAVEGACIÓN Y CANCELAR
        // =============================================

        protected void btnRegresar_Click(object sender, EventArgs e) => Response.Redirect("ConvocatoriaGruInvestigacion.aspx");
        protected void lbtCancelar_Click(object sender, EventArgs e) => Response.Redirect("ConvocatoriaGruInvestigacion.aspx");
        protected void lbtCancelarEdit_Click(object sender, EventArgs e) => Response.Redirect("ConvocatoriaGruInvestigacion.aspx");

        // =============================================
        // UTILIDADES
        // =============================================

        private enum Vista { Lista, Agregar, Editar }

        private void CambiarVista(Vista vista)
        {
            pnlGrilla.Visible = vista == Vista.Lista;
            headerConvocatoria.Visible = vista == Vista.Lista;

            pnlAgregar.Visible = vista == Vista.Agregar;
            pnlEditar.Visible = vista == Vista.Editar;
        }

        private bool ValidarArchivo(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLower();
            if (ext != ".pdf" && ext != ".xls" && ext != ".xlsx" && ext != ".doc" && ext != ".docx")
            {
                Msg("Formato no permitido (Solo PDF, Excel, Word).", "ww");
                return false;
            }
            return true;
        }

        private string GuardarArchivoFisico(FileUpload ctl, string nombre)
        {
            if (!Directory.Exists(RUTA_CONVOCATORIAS)) Directory.CreateDirectory(RUTA_CONVOCATORIAS);
            string ruta = Path.Combine(RUTA_CONVOCATORIAS, nombre);
            ctl.SaveAs(ruta);
            return ruta;
        }

        private void DescargarArchivo(string rutaFisica)
        {
            if (File.Exists(rutaFisica))
            {
                string nombre = Path.GetFileName(rutaFisica);
                string ext = Path.GetExtension(rutaFisica).ToLower();
                Response.Clear();

                switch (ext)
                {
                    case ".pdf": Response.ContentType = "application/pdf"; break;
                    case ".xls": Response.ContentType = "application/vnd.ms-excel"; break;
                    case ".xlsx": Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; break;
                    case ".doc": Response.ContentType = "application/msword"; break;
                    case ".docx": Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"; break;
                    default: Response.ContentType = "application/octet-stream"; break;
                }

                Response.AppendHeader("Content-Disposition", "inline; filename=" + nombre);
                Response.TransmitFile(rutaFisica);
                Response.End();
            }
            else
            {
                Msg("El archivo físico no existe.", "ww");
            }
        }

        private void Redireccionar(string msg, string type)
        {
            Session["TempMsg"] = msg;
            Session["TempTipo"] = type;
            Response.Redirect("ConvocatoriaGruInvestigacion.aspx", false);
        }

        private void Msg(string msg, string type)
        {
            if (string.IsNullOrEmpty(msg)) return;
            string cleanMsg = msg.Replace("'", "\\'").Replace("\r\n", " ").Replace("\n", " ");
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"$(function() {{ toastify('{type}', '{cleanMsg}', 'Sistema'); }});", true);
        }
    }
}