using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;

namespace SistemaGestionCGI
{
    public partial class ConvocatoriaGruInvestigacion : System.Web.UI.Page
    {
        // Instancia de la Capa de Negocio
        private readonly ManejadorConvocatorias _manejador = new ManejadorConvocatorias();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 1. Validar Sesión
                if (Session["UsuarioLogueado"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                // 2. Cargar Grilla Inicial
                CargarGrilla();

                // 3. Mostrar Mensajes Flash (si viene de una redirección)
                if (Session["TempMsg"] != null)
                {
                    Msg(Session["TempMsg"].ToString(), Session["TempTipo"].ToString());
                    Session["TempMsg"] = null;
                    Session["TempTipo"] = null;
                }
            }
        }

        // =======================================================
        // 1. GESTIÓN DE PANELES Y NAVEGACIÓN
        // =======================================================

        private void CargarGrilla()
        {
            try
            {
                List<InvgccConvocatoriaGruInvestigacion> lista = _manejador.ObtenerConvocatorias();
                rptConvocatorias.DataSource = lista;
                rptConvocatorias.DataBind();
            }
            catch (Exception ex)
            {
                Msg("Error al cargar datos: " + ex.Message, "ee");
            }
        }

        protected void lbtNuevaConv_Click(object sender, EventArgs e)
        {
            // Ocultar Principal -> Mostrar Agregar
            pnlGrilla.Visible = false;
            pnlAgregar.Visible = true;
            pnlEditar.Visible = false;

            // Ajustar Header
            lbtNuevaConv.Visible = false;
            btnRegresar.Visible = true;

            // Limpiar Campos
            LimpiarFormularioAdd();
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            // Volver al inicio
            pnlGrilla.Visible = true;
            pnlAgregar.Visible = false;
            pnlEditar.Visible = false;

            lbtNuevaConv.Visible = true;
            btnRegresar.Visible = false;

            CargarGrilla(); // Refrescar por si acaso
        }

        protected void lbtCancelar_Click(object sender, EventArgs e)
        {
            btnRegresar_Click(null, null);
        }

        protected void lbtCancelarEdit_Click(object sender, EventArgs e)
        {
            btnRegresar_Click(null, null);
        }

        // =======================================================
        // 2. CREAR (INSERT)
        // =======================================================

        private void LimpiarFormularioAdd()
        {
            txtNombreAdd.Text = "";
            txtDescAdd.Text = "";
            txtFechaIniAdd.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        protected void lbtGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Obtener Fecha de Publicación (Solo Inicio)
                DateTime inicio = DateTime.Parse(txtFechaIniAdd.Text);

                // 2. Validar Archivo Obligatorio (Al crear es mandatorio)
                if (!flpArchivoAdd.HasFile)
                {
                    Msg("Debe adjuntar el archivo de bases (PDF/Excel).", "ww");
                    return;
                }

                // 3. Guardar Archivo Físico
                string rutaArchivo = GuardarArchivoFisico(flpArchivoAdd);
                if (string.IsNullOrEmpty(rutaArchivo)) return; // Si falla la subida, detenemos

                // 4. Construir Objeto
                InvgccConvocatoriaGruInvestigacion obj = new InvgccConvocatoriaGruInvestigacion();

                // NO asignamos ID aquí, la BLL lo genera automáticamente (igual que en Grupos)
                // obj.strId_conv = ... (Se hace en el Manejador)

                obj.strNombre_conv = txtNombreAdd.Text.Trim();
                obj.strDescripcion_conv = txtDescAdd.Text;
                obj.dtFechaini_conv = inicio;

                // TRUCO SQL: Usamos 01/01/1900 para que sea una fecha válida en SQL Server
                // pero que visualmente entendemos como "Sin Fecha Fin"
                obj.dtFechafin_conv = new DateTime(1900, 1, 1);

                obj.strArchivo_conv = rutaArchivo;

                // 5. Llamar a la BLL (Método void)
                _manejador.GuardarConvocatoria(obj);

                // 6. Feedback y Redirección
                SetFlashMessage("Convocatoria creada exitosamente.", "ss");
                Response.Redirect("ConvocatoriaGruInvestigacion.aspx");
            }
            catch (Exception ex)
            {
                Msg("Error al guardar: " + ex.Message, "ee");
            }
        }

        // =======================================================
        // 3. EDITAR Y ELIMINAR (CRUD)
        // =======================================================

        protected void rptConvocatorias_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string idConv = e.CommandArgument.ToString();

            if (e.CommandName == "Editar")
            {
                CargarDatosEdicion(idConv);
            }
            else if (e.CommandName == "Eliminar")
            {
                EliminarRegistro(idConv);
            }
            else if (e.CommandName == "VerArchivo")
            {
                VerArchivoAdjunto(idConv);
            }
        }

        private void CargarDatosEdicion(string id)
        {
            try
            {
                var obj = _manejador.ObtenerConvocatoriaPorId(id);
                if (obj != null)
                {
                    hfIdConvEdit.Value = obj.strId_conv;
                    txtNombreEdit.Text = obj.strNombre_conv;
                    txtDescEdit.Text = obj.strDescripcion_conv;
                    txtFechaIniEdit.Text = obj.dtFechaini_conv.ToString("yyyy-MM-dd");

                    hfArchivoActual.Value = obj.strArchivo_conv;

                    // Cambiar Vista
                    pnlGrilla.Visible = false;
                    pnlAgregar.Visible = false;
                    pnlEditar.Visible = true;

                    lbtNuevaConv.Visible = false;
                    btnRegresar.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Msg("Error al cargar para editar: " + ex.Message, "ee");
            }
        }

        protected void lbtActualizar_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Obtener Fecha
                DateTime inicio = DateTime.Parse(txtFechaIniEdit.Text);

                // 2. Construir Objeto con el ID oculto
                InvgccConvocatoriaGruInvestigacion obj = new InvgccConvocatoriaGruInvestigacion();
                obj.strId_conv = hfIdConvEdit.Value; // ID que cargamos al dar click en Editar
                obj.strNombre_conv = txtNombreEdit.Text.Trim();
                obj.strDescripcion_conv = txtDescEdit.Text;

                obj.dtFechaini_conv = inicio;
                obj.dtFechafin_conv = new DateTime(1900, 1, 1); // Fecha "dummy" segura

                // 3. Lógica de Archivo (Reemplazo opcional)
                if (flpArchivoEdit.HasFile)
                {
                    // A) Si sube uno nuevo -> Borramos el viejo y guardamos el nuevo
                    if (File.Exists(hfArchivoActual.Value))
                    {
                        try { File.Delete(hfArchivoActual.Value); } catch { /* Ignorar si está bloqueado */ }
                    }
                    obj.strArchivo_conv = GuardarArchivoFisico(flpArchivoEdit);
                }
                else
                {
                    // B) Si no sube nada -> Mantenemos el que ya tenía
                    obj.strArchivo_conv = hfArchivoActual.Value;
                }

                // 4. Llamar a la BLL (Método void)
                _manejador.ActualizarConvocatoria(obj);

                // 5. Feedback y Redirección
                SetFlashMessage("Registro actualizado correctamente.", "ss");
                Response.Redirect("ConvocatoriaGruInvestigacion.aspx");
            }
            catch (Exception ex)
            {
                Msg("Error al actualizar: " + ex.Message, "ee");
            }
        }

        private void EliminarRegistro(string id)
        {
            try
            {
                // 1. Obtener datos para borrar archivo físico primero (limpieza)
                var obj = _manejador.ObtenerConvocatoriaPorId(id);

                if (obj != null && !string.IsNullOrEmpty(obj.strArchivo_conv) && File.Exists(obj.strArchivo_conv))
                {
                    try
                    {
                        File.SetAttributes(obj.strArchivo_conv, FileAttributes.Normal);
                        File.Delete(obj.strArchivo_conv);
                    }
                    catch { /* Ignoramos si el archivo está en uso, no es crítico */ }
                }

                // 2. Llamada a la BLL (Método VOID)
                // Ya no va dentro de un 'if', porque si falla lanzará una excepción
                _manejador.EliminarConvocatoria(id);

                // 3. Si llegamos aquí, es éxito
                Msg("Convocatoria eliminada correctamente.", "ss");
                CargarGrilla();
            }
            catch (Exception ex)
            {
                // 4. Si hubo error SQL (ej: llave foránea), cae aquí
                Msg("No se pudo eliminar. Es posible que tenga registros asociados.", "ee");
            }
        }

        // =======================================================
        // 4. UTILIDADES (Archivos y Mensajes)
        // =======================================================

        private string GuardarArchivoFisico(FileUpload control)
        {
            try
            {
                string extension = Path.GetExtension(control.FileName).ToLower();

                // Validación Servidor
                if (extension != ".pdf" && extension != ".xls" && extension != ".xlsx" && extension != ".doc" && extension != ".docx")
                {
                    Msg("Formato de archivo no permitido. Solo PDF, Excel o Word.", "ww");
                    return "";
                }

                // Definir Ruta
                string carpetaBase = @"C:\UTC\CONVOCATORIAS\";
                if (!Directory.Exists(carpetaBase)) Directory.CreateDirectory(carpetaBase);

                // Nombre único: Convocatoria-FECHA-HORA.ext
                string nombreArchivo = $"Convocatoria-{DateTime.Now:yyyyMMdd-HHmmss}{extension}";
                string rutaCompleta = Path.Combine(carpetaBase, nombreArchivo);

                control.SaveAs(rutaCompleta);
                return rutaCompleta;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al subir archivo físico: " + ex.Message);
            }
        }

        private void VerArchivoAdjunto(string id)
        {
            try
            {
                var obj = _manejador.ObtenerConvocatoriaPorId(id);
                if (obj != null && File.Exists(obj.strArchivo_conv))
                {
                    string url = $"VerArchivo.ashx?id={id}&tipo=CONVOCATORIA";

                    // Llamamos a la función JS que abre el Modal
                    string script = $"VerPDF('{url}');";
                    ClientScript.RegisterStartupScript(this.GetType(), "OpenVisor", script, true);
                }
                else
                {
                    Msg("El archivo físico no existe en el servidor.", "ww");
                }
            }
            catch (Exception ex)
            {
                Msg("Error al intentar visualizar: " + ex.Message, "ee");
            }
        }

        // Helpers para Toastify
        private void Msg(string msg, string type)
        {
            // Limpia caracteres que rompen JS
            string cleanMsg = msg.Replace("'", "").Replace("\n", " ").Replace("\r", "");
            string script = $"<script>toastify('{type}', '{cleanMsg}', 'Sistema');</script>";

            // Inyecta el script al final de la carga de la página
            ClientScript.RegisterStartupScript(this.GetType(), "ToastMessage", script);
        }

        private void SetFlashMessage(string msg, string type)
        {
            Session["TempMsg"] = msg;
            Session["TempTipo"] = type;
        }
    }
}