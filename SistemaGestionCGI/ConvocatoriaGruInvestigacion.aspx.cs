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
        private readonly ManejadorConvocatorias _manejador = new ManejadorConvocatorias();
        private const string RUTA_VIRTUAL_CONVOCATORIAS = "~/Archivos/Convocatorias/";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            if (Session["UsuarioLogueado"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (Session["RolUsuario"]?.ToString() == "COORDINADOR")
            {
                Response.Redirect("EjecucionProAprobados.aspx");
                return;
            }

            CargarGrilla();

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

                // CAMBIO 2: Uso del nuevo método de guardado virtual
                string rutaRelativa = GuardarArchivoVirtual(flpArchivoAdd, $"CONV_{DateTime.Now.Ticks}{Path.GetExtension(flpArchivoAdd.FileName)}");

                var conv = new InvgccConvocatoriaGruInvestigacion
                {
                    strNombre_conv = txtNombreAdd.Text.Trim(),
                    dtFechaini_conv = DateTime.Parse(txtFechaIniAdd.Text),
                    strDescripcion_conv = HttpUtility.HtmlEncode(txtDescAdd.Text),
                    strArchivo_conv = rutaRelativa // Se guarda "~/Archivos/..."
                };

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
                    strArchivo_conv = hfArchivoActual.Value, // Mantiene la ruta actual por defecto
                    dtFechafin_conv = new DateTime(1900, 1, 1)
                };

                if (flpArchivoEdit.HasFile)
                {
                    if (!ValidarArchivo(flpArchivoEdit.FileName)) return;

                    // CAMBIO 3: Guardado virtual en edición
                    conv.strArchivo_conv = GuardarArchivoVirtual(flpArchivoEdit, $"CONV_{DateTime.Now.Ticks}{Path.GetExtension(flpArchivoEdit.FileName)}");
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
                        // CAMBIO 4: Resolución de ruta para eliminar
                        if (obj != null && !string.IsNullOrEmpty(obj.strArchivo_conv))
                        {
                            string rutaFisica = ObtenerRutaFisica(obj.strArchivo_conv);
                            if (File.Exists(rutaFisica))
                            {
                                try { File.Delete(rutaFisica); } catch { }
                            }
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
        // UTILIDADES Y MANEJO DE ARCHIVOS
        // =============================================

        private enum Vista { Lista, Agregar, Editar }

        private void CambiarVista(Vista vista)
        {
            pnlGrilla.Visible = vista == Vista.Lista;
            pnlAgregar.Visible = vista == Vista.Agregar;
            pnlEditar.Visible = vista == Vista.Editar;
            lbtNuevaConv.Visible = (vista == Vista.Lista);
            btnRegresar.Visible = (vista != Vista.Lista);
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

        // CAMBIO 5: Método principal para guardar usando Server.MapPath
        private string GuardarArchivoVirtual(FileUpload ctl, string nombreArchivo)
        {
            // 1. Obtener la ruta física del servidor basada en la ruta virtual
            string directorioFisico = Server.MapPath(RUTA_VIRTUAL_CONVOCATORIAS);

            // 2. Crear directorio si no existe (dentro de la carpeta del proyecto)
            if (!Directory.Exists(directorioFisico))
            {
                Directory.CreateDirectory(directorioFisico);
            }

            // 3. Guardar el archivo físicamente
            string rutaGuardado = Path.Combine(directorioFisico, nombreArchivo);
            ctl.SaveAs(rutaGuardado);

            // 4. RETORNAR la ruta VIRTUAL para guardar en BD (Portabilidad)
            // Retorna algo como: "~/Archivos/Convocatorias/CONV_123456.pdf"
            return Path.Combine(RUTA_VIRTUAL_CONVOCATORIAS, nombreArchivo).Replace("\\", "/");
        }

        // CAMBIO 6: Helper para resolver rutas antiguas (C:\) y nuevas (~/)
        private string ObtenerRutaFisica(string rutaBd)
        {
            if (string.IsNullOrEmpty(rutaBd)) return "";

            // Si empieza con ~ o /, es ruta virtual nueva
            if (rutaBd.StartsWith("~") || rutaBd.StartsWith("/"))
            {
                return Server.MapPath(rutaBd);
            }

            // Si tiene dos puntos (C:), asumimos que es ruta física legada
            if (rutaBd.Contains(":"))
            {
                return rutaBd;
            }

            // Fallback: intentar mapear asumiendo que es solo nombre de archivo
            return Server.MapPath(Path.Combine(RUTA_VIRTUAL_CONVOCATORIAS, rutaBd));
        }

        private void DescargarArchivo(string rutaBd)
        {
            // Resolver la ruta real en el disco
            string rutaFisica = ObtenerRutaFisica(rutaBd);

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
                Msg("El archivo no se encuentra en el servidor.", "ww");
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

            string cleanMsg = msg
                .Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace("\"", "\\\"")
                .Replace("\r\n", " ")
                .Replace("\n", " ");

            string titulo = type == "ss" ? "Éxito" : (type == "ee" ? "Error" : "Atención");
            string script = $"$(function() {{ toastify('{type}', '{cleanMsg}', '{titulo}'); }});";

            ScriptManager.RegisterStartupScript(this, GetType(), "alert", script, true);
        }
    }
}