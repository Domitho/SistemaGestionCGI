using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;
using System.IO.Compression;

namespace SistemaGestionCGI
{
    public partial class CalificacionGruInvestigacion : System.Web.UI.Page
    {
        // Instancias 
        private readonly ManejadorCalificacionGrupo _manejador = new ManejadorCalificacionGrupo();
        private const string RUTA_VIRTUAL = "~/Archivos/Calificaciones/";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            if (Session["UsuarioLogueado"] == null)
            {
                Response.Redirect("Login.aspx", true);
                return;
            }

            if (Session["RolUsuario"]?.ToString() == "COORDINADOR")
            {
                Response.Redirect("EjecucionProAprobados.aspx");
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

        private void CargarGrilla()
        {
            try
            {
                int anio = (ddlFiltroAnio.SelectedIndex > 0 && int.TryParse(ddlFiltroAnio.SelectedValue, out int a)) ? a : 0;
                rptCalificaciones.DataSource = _manejador.ObtenerCalificaciones(anio);
                rptCalificaciones.DataBind();
            }
            catch (Exception ex) { Msg("Error al cargar calificaciones: " + ex.Message, "ee"); }
        }

        private void CargarCombos()
        {
            try
            {
                ddlFiltroAnio.Items.Clear();
                ddlFiltroAnio.Items.Add(new ListItem("Todos los Años", "0"));
                foreach (int y in _manejador.ObtenerAniosDisponibles())
                {
                    ddlFiltroAnio.Items.Add(new ListItem(y.ToString(), y.ToString()));
                }

                var aniosEnBd = _manejador.ObtenerAniosConMetricasConfiguradas();

                int anioInicio = 2015;
                int anioFin = DateTime.Now.Year + 5;

                var listaAnios = new List<int>();
                for (int i = anioInicio; i <= anioFin; i++)
                {
                    listaAnios.Add(i);
                }

                var aniosConsolidados = aniosEnBd
                                        .Union(listaAnios)         
                                        .Distinct()               
                                        .OrderByDescending(x => x) 
                                        .ToList();

                ddlAnioMetricas.Items.Clear();
                ddlAnioMetricas.DataSource = aniosConsolidados;
                ddlAnioMetricas.DataBind();

                string currentYear = DateTime.Now.Year.ToString();
                if (ddlAnioMetricas.Items.FindByValue(currentYear) != null)
                    ddlAnioMetricas.SelectedValue = currentYear;
            }
            catch (Exception ex) { Msg("Error cargando listas: " + ex.Message, "ee"); }
        }

        private void CargarGruposPendientes(string idGrupoIncluir = "")
        {
            try
            {
                int anioSeleccionado = int.TryParse(ddlAnioMetricaSeleccion.SelectedValue, out int a) ? a : DateTime.Now.Year;

                var grupos = _manejador.ObtenerGruposParaCombo(anioSeleccionado, idGrupoIncluir);

                ddlGrupoAdd.DataSource = grupos;
                ddlGrupoAdd.DataTextField = "strNombre_gru";
                ddlGrupoAdd.DataValueField = "strId_gru";
                ddlGrupoAdd.DataBind();

                if (grupos.Count > 0)
                    ddlGrupoAdd.Items.Insert(0, new ListItem("-- Seleccione Grupo --", ""));
                else
                    ddlGrupoAdd.Items.Insert(0, new ListItem("-- Todos calificados --", ""));
            }
            catch (Exception ex) { Msg("Error al cargar grupos: " + ex.Message, "ee"); }
        }

        protected void ddlFiltroAnio_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarGrilla();
        }

        protected void ddlAnioMetricaSeleccion_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarGruposPendientes();
            ActualizarMetricaVisual();
        }

        protected void btnNuevaCalif_Click(object sender, EventArgs e)
        {
            CambiarVista(Vista.Formulario);

            pnlInfoEvaluacion.Visible = false;
            pnlUploadEvaluacion.Style["display"] = "block";

            contenedorResolucionTotal.Visible = true;
            contenedorResolucionTotal.Style["display"] = "none";

            txtFechaAdd.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtPuntajeAdd.Text = "";
            txtReconocimientoAdd.Text = "";

            ddlAnioMetricaSeleccion.DataSource = _manejador.ObtenerAniosConMetricasConfiguradas();
            ddlAnioMetricaSeleccion.DataBind();

            string anioActual = DateTime.Now.Year.ToString();
            if (ddlAnioMetricaSeleccion.Items.FindByValue(anioActual) != null)
                ddlAnioMetricaSeleccion.SelectedValue = anioActual;

            CargarGruposPendientes();
            LimpiarFormulario();
            ActualizarMetricaVisual();
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            Response.Redirect("CalificacionGruInvestigacion.aspx");
        }

        private void ActualizarMetricaVisual()
        {
            if (int.TryParse(ddlAnioMetricaSeleccion.SelectedValue, out int anio))
            {
                var metricas = _manejador.ObtenerConfiguracionMetricas(anio);

                lblReglaMetrica.Text = $@"
                    <div class='row g-3 text-center'>
                        <div class='col-md-4'>
                            <div class='p-2 border border-success bg-success bg-opacity-10 rounded-3'>
                                <h5 class='fw-bold text-success mb-0'>{metricas.minConsolidado} - 100</h5>
                                <small class='text-uppercase fw-bold text-success' style='font-size:0.75rem'>
                                    <i class='fa-solid fa-circle-check me-1'></i>Consolidado
                                </small>
                            </div>
                        </div>

                        <div class='col-md-4'>
                            <div class='p-2 border border-warning bg-warning bg-opacity-10 rounded-3'>
                                <h5 class='fw-bold text-warning text-dark mb-0'>{metricas.minEmergente} - {metricas.minConsolidado - 1}</h5>
                                <small class='text-uppercase fw-bold text-dark' style='font-size:0.75rem'>
                                    <i class='fa-solid fa-circle-exclamation me-1'></i>Emergente
                                </small>
                            </div>
                        </div>

                        <div class='col-md-4'>
                            <div class='p-2 border border-danger bg-danger bg-opacity-10 rounded-3'>
                                <h5 class='fw-bold text-danger mb-0'>0 - {metricas.minEmergente - 1}</h5>
                                <small class='text-uppercase fw-bold text-danger' style='font-size:0.75rem'>
                                    <i class='fa-solid fa-circle-xmark me-1'></i>Disuelto
                                </small>
                            </div>
                        </div>
                    </div>
                    <div class='text-end mt-2'>
                        <small class='text-muted fst-italic'>* Normativa vigente del año <b>{anio}</b></small>
                    </div>";
            }
            else
            {
                lblReglaMetrica.Text = @"
            <div class='text-center text-muted py-3'>
                <i class='fa-solid fa-arrow-up mb-2'></i><br/>
                Seleccione un <b>Año de Métrica</b> para visualizar las reglas.
            </div>";
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlGrupoAdd.SelectedIndex <= 0) { Msg("Seleccione un grupo.", "ww"); return; }
                if (string.IsNullOrWhiteSpace(txtFechaAdd.Text)) { Msg("Ingrese la fecha.", "ww"); return; }

                bool esEdicion = !string.IsNullOrEmpty(IdCalificacionEnEdicion);

                int? puntaje = null;
                if (!string.IsNullOrWhiteSpace(txtPuntajeAdd.Text))
                {
                    if (int.TryParse(txtPuntajeAdd.Text, out int p))
                    {
                        if (p < 0 || p > 100) { Msg("Puntaje inválido (0-100).", "ww"); return; }
                        puntaje = p;
                    }
                    else { Msg("El puntaje debe ser numérico.", "ww"); return; }
                }

                bool tieneResolucionFisica = flpResolucion.HasFile;
                InvgccCalificacionGrupo registroAnterior = esEdicion ? _manejador.ObtenerPorId(IdCalificacionEnEdicion) : null;

                bool tieneResolucionAnterior = (registroAnterior != null && !string.IsNullOrEmpty(registroAnterior.strResolucion_valo));
                bool existeResolucionFinal = tieneResolucionFisica || tieneResolucionAnterior;

                if (puntaje.HasValue && !existeResolucionFinal)
                {
                    Msg("Para registrar un Puntaje, es OBLIGATORIO subir la Resolución.", "ww");
                    return;
                }
                if (existeResolucionFinal && !puntaje.HasValue)
                {
                    Msg("Si sube una Resolución, debe asignar el Puntaje obtenido.", "ww");
                    return;
                }

                bool tieneEvaluacionNueva = flpArchivoAdd.HasFile;
                bool tieneEvaluacionAnterior = (registroAnterior != null && !string.IsNullOrEmpty(registroAnterior.strInforme_valo));

                if (!tieneEvaluacionNueva && !tieneEvaluacionAnterior)
                {
                    Msg("El Informe de Evaluación es obligatorio.", "ww");
                    return;
                }

                string rutaInforme = (registroAnterior != null) ? registroAnterior.strInforme_valo : "";
                string rutaResolucion = (registroAnterior != null) ? registroAnterior.strResolucion_valo : "";

                if (flpArchivoAdd.HasFile)
                    rutaInforme = GuardarArchivoFisico(flpArchivoAdd, $"VAL_{DateTime.Now.Ticks}.pdf");

                if (flpResolucion.HasFile)
                    rutaResolucion = GuardarArchivoFisico(flpResolucion, $"RES_{DateTime.Now.Ticks}.pdf");

                string categoria = "PENDIENTE";

                if (puntaje.HasValue)
                {
                    if (!int.TryParse(ddlAnioMetricaSeleccion.SelectedValue, out int anioM))
                    {
                        Msg("Error: Año de métrica inválido.", "ww");
                        return;
                    }

                    var reglas = _manejador.ObtenerConfiguracionMetricas(anioM);

                    if (puntaje.Value >= reglas.minConsolidado)
                    {
                        categoria = "CONSOLIDADO";
                    }
                    else if (puntaje.Value >= reglas.minEmergente)
                    {
                        categoria = "EMERGENTE";
                    }
                    else
                    {
                        categoria = "DISUELTO";
                    }
                }

                var obj = new InvgccCalificacionGrupo
                {
                    strId_valo = esEdicion ? IdCalificacionEnEdicion : null,
                    fkId_gru = ddlGrupoAdd.SelectedValue,
                    dtFecha_valo = DateTime.Parse(txtFechaAdd.Text),
                    intPuntaje_valo = puntaje,
                    strReconocimiento_valo = txtReconocimientoAdd.Text.Trim(),
                    intAnioMetrica = int.Parse(ddlAnioMetricaSeleccion.SelectedValue),
                    strCategoria_valo = categoria,
                    strInforme_valo = rutaInforme,
                    strResolucion_valo = rutaResolucion
                };

                string usuarioSesion = Session["UsuarioLogueado"]?.ToString() ?? "SIN_USUARIO";
                int idUsuarioSesion = Session["IdUsuario"] != null ? (int)Session["IdUsuario"] : 0;

                if (esEdicion)
                {
                    _manejador.ActualizarCalificacion(obj, usuarioSesion, idUsuarioSesion);
                    IdCalificacionEnEdicion = null;
                    Redireccionar($"Actualización exitosa. Estado resultante: <b>{categoria}</b>", "ss");
                }
                else
                {
                    _manejador.GuardarCalificacion(obj, usuarioSesion, idUsuarioSesion);
                    Redireccionar($"Registro creado. Estado resultante: <b>{categoria}</b>", "ss");
                }
            }
            catch (Exception ex) { Msg("Error: " + ex.Message, "ee"); }
        }

        protected void rptCalificaciones_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();

            if (e.CommandName == "Editar")
            {
                CargarDatosParaEditar(id);
            }
            else if (e.CommandName == "Eliminar")
            {
                try
                {
                    string usuarioSesion = Session["UsuarioLogueado"]?.ToString() ?? "SIN_USUARIO";
                    int idUsuarioSesion = Session["IdUsuario"] != null ? (int)Session["IdUsuario"] : 0;
                    _manejador.EliminarCalificacion(id, usuarioSesion, idUsuarioSesion);
                    Redireccionar("Calificación eliminada correctamente.", "ss");
                }
                catch (Exception ex) { Msg("Error al eliminar: " + ex.Message, "ee"); }
            }
            else if (e.CommandName == "Ver")
            {
                var calificacion = _manejador.ObtenerPorId(id);

                if (calificacion != null)
                {
                    ConfigurarVisualizacionArchivo(
                        calificacion.strInforme_valo,
                        lnkDescargarInforme,
                        iconInforme,
                        divEstadoInforme
                    );

                    ConfigurarVisualizacionArchivo(
                        calificacion.strResolucion_valo,
                        lnkDescargarResolucion,
                        iconResolucion,
                        divEstadoResolucion
                    );

                    ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalArchivos", "AbrirModalArchivos();", true);
                }
                else
                {
                    Msg("No se encontró la calificación.", "ww");
                }
            }
        }

        // MODAL DOCUMENTOS
        private void ConfigurarVisualizacionArchivo(string ruta, HyperLink lnk, System.Web.UI.HtmlControls.HtmlGenericControl divIcon, System.Web.UI.HtmlControls.HtmlGenericControl divEstado)
        {
            divIcon.InnerHtml = "";
            divEstado.InnerHtml = "";

            if (!string.IsNullOrEmpty(ruta))
            {
                string ext = System.IO.Path.GetExtension(ruta).ToLower();
                string nombreArchivo = System.IO.Path.GetFileName(ruta);

                string nombreCorto = nombreArchivo.Length > 25 ? nombreArchivo.Substring(0, 22) + "..." : nombreArchivo;

                lnk.Visible = true;
                lnk.NavigateUrl = ruta;

                if (ext == ".zip" || ext == ".rar" || ext == ".7z")
                {
                    lnk.Text = "<i class='fa-solid fa-cloud-arrow-down me-2'></i> DESCARGAR ZIP";
                    lnk.CssClass = "btn btn-success btn-pill fw-bold shadow-sm";

                    divIcon.InnerHtml = "<i class='fa-solid fa-file-zipper fa-3x text-warning'></i>";

                    divEstado.InnerHtml = $"<span class='badge bg-warning text-dark'><i class='fa-solid fa-box-archive me-1'></i> {nombreCorto}</span>";
                }
                else
                {
                    lnk.Text = "<i class='fa-solid fa-eye me-2'></i> VISUALIZAR";
                    lnk.CssClass = "btn btn-primary btn-pill fw-bold shadow-sm"; 

                    if (ext == ".pdf")
                        divIcon.InnerHtml = "<i class='fa-solid fa-file-pdf fa-3x text-danger'></i>";
                    else if (ext.Contains("doc"))
                        divIcon.InnerHtml = "<i class='fa-solid fa-file-word fa-3x text-primary'></i>";
                    else
                        divIcon.InnerHtml = "<i class='fa-solid fa-file-lines fa-3x text-secondary'></i>";

                    divEstado.InnerHtml = $"<span class='badge bg-light text-secondary border'><i class='fa-solid fa-check-circle text-success me-1'></i> {nombreCorto}</span>";
                }
            }
            else
            {
                lnk.Visible = false;

                divIcon.InnerHtml = "<i class='fa-solid fa-folder-open fa-3x text-muted opacity-25'></i>";

                divEstado.InnerHtml = "<span class='badge bg-danger bg-opacity-10 text-danger'><i class='fa-solid fa-circle-xmark me-1'></i> No cargado</span>";

                lnk.Visible = true;
                lnk.NavigateUrl = "#";
                lnk.CssClass = "btn btn-outline-secondary btn-pill disabled";
                lnk.Text = "No Disponible";
            }
        }

        // =============================================
        // GESTIÓN DE MÉTRICAS (CONFIGURACIÓN)
        // =============================================

        protected void btnGuardarMetricas_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMinConsolidado.Text) || string.IsNullOrEmpty(txtMinEmergente.Text))
                {
                    Msg("Debe ingresar ambos puntajes mínimos (Consolidado y Emergente).", "ww");
                    return;
                }

                int minCons = int.Parse(txtMinConsolidado.Text);
                int minEmer = int.Parse(txtMinEmergente.Text);

                if (minEmer >= minCons)
                {
                    Msg("Error Lógico: El mínimo para EMERGENTE debe ser menor que el de CONSOLIDADO.", "ww");
                    return;
                }

                var m = new InvgccMetricas
                {
                    anio = int.Parse(ddlAnioMetricas.SelectedValue),
                    minConsolidado = minCons,
                    minEmergente = minEmer
                };

                _manejador.GuardarMetrica(m);

                if (ddlAnioMetricaSeleccion.SelectedValue == m.anio.ToString())
                {
                    ActualizarMetricaVisual();
                }

                Redireccionar($"Métricas {m.anio} actualizadas correctamente.", "ss");
            }
            catch (Exception ex) { Msg("Error al guardar métrica: " + ex.Message, "ee"); }
        }

        // =============================================
        // UTILIDADES Y AYUDAS
        // =============================================

        private void VisualizarArchivo(string rutaVirtual)
        {
            try
            {
                string rutaFisica = Server.MapPath(rutaVirtual);

                if (File.Exists(rutaFisica))
                {
                    string nombreArchivo = Path.GetFileName(rutaFisica);

                    Response.Clear();
                    Response.Buffer = true;
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("Content-Disposition", "inline; filename=" + nombreArchivo);
                    Response.TransmitFile(rutaFisica);
                    Response.Flush();
                    Response.End();
                }
                else
                {
                    Msg("El archivo físico no existe en el servidor.", "ee");
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                Msg("Error al visualizar: " + ex.Message, "ee");
            }
        }

        private enum Vista { Lista, Formulario }

        private void CambiarVista(Vista vista)
        {
            pnlGrilla.Visible = vista == Vista.Lista;
            pnlFiltros.Visible = vista == Vista.Lista;
            headerCalificacion.Visible = vista == Vista.Lista;
            pnlFormulario.Visible = vista == Vista.Formulario;
        }

        private string GuardarArchivoFisico(FileUpload control, string nombreBase)
        {
            string rutaFisicaCarpeta = Server.MapPath(RUTA_VIRTUAL);

            if (!Directory.Exists(rutaFisicaCarpeta))
            {
                Directory.CreateDirectory(rutaFisicaCarpeta);
            }

            if (control.PostedFiles.Count > 1)
            {
                string nombreCarpetaTemp = "TEMP_" + DateTime.Now.Ticks;
                string rutaTemp = Path.Combine(rutaFisicaCarpeta, nombreCarpetaTemp);
                Directory.CreateDirectory(rutaTemp);

                try
                {
                    foreach (var archivo in control.PostedFiles)
                    {
                        string nombreArchivo = Path.GetFileName(archivo.FileName);
                        archivo.SaveAs(Path.Combine(rutaTemp, nombreArchivo));
                    }

                    string nombreZip = nombreBase.Replace(".pdf", ".zip").Replace(".docx", ".zip"); 
                    if (!nombreZip.EndsWith(".zip")) nombreZip += ".zip";

                    string rutaZipFinal = Path.Combine(rutaFisicaCarpeta, nombreZip);

                    if (File.Exists(rutaZipFinal)) File.Delete(rutaZipFinal);

                    ZipFile.CreateFromDirectory(rutaTemp, rutaZipFinal);

                    return Path.Combine(RUTA_VIRTUAL, nombreZip).Replace("\\", "/");
                }
                finally
                {
                    if (Directory.Exists(rutaTemp)) Directory.Delete(rutaTemp, true);
                }
            }
            else if (control.HasFile)
            {
                string nombreFinal = nombreBase;

                string ext = Path.GetExtension(control.FileName).ToLower();
                if (ext == ".zip" || ext == ".rar")
                {
                    nombreFinal = Path.ChangeExtension(nombreBase, ext);
                }

                string rutaFisicaCompleta = Path.Combine(rutaFisicaCarpeta, nombreFinal);
                control.SaveAs(rutaFisicaCompleta);
                return Path.Combine(RUTA_VIRTUAL, nombreFinal).Replace("\\", "/");
            }

            return "";
        }

        private void Redireccionar(string msg, string type)
        {
            Session["TempMsg"] = msg;
            Session["TempTipo"] = type;
            Response.Redirect("CalificacionGruInvestigacion.aspx", false);
        }

        private void Msg(string msg, string type)
        {
            if (string.IsNullOrEmpty(msg)) return;

            string cleanMsg = msg
                .Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace("\"", "\\\"")
                .Replace("\r\n", " ")
                .Replace("\n", " ")
                .Replace("\r", " ");

            string script = $"$(function() {{ toastify('{type}', '{cleanMsg}', 'Sistema'); }});";

            ScriptManager.RegisterStartupScript(this, GetType(), "toast", script, true);
        }

        //

        public string IdCalificacionEnEdicion
        {
            get { return ViewState["IdEdit"] as string; }
            set { ViewState["IdEdit"] = value; }
        }

        private void CargarDatosParaEditar(string id)
        {
            var obj = _manejador.ObtenerPorId(id);
            if (obj != null)
            {
                IdCalificacionEnEdicion = obj.strId_valo;

                ddlAnioMetricaSeleccion.DataSource = _manejador.ObtenerAniosConMetricasConfiguradas();
                ddlAnioMetricaSeleccion.DataBind();
                string anioStr = obj.intAnioMetrica.ToString();
                if (ddlAnioMetricaSeleccion.Items.FindByValue(anioStr) == null)
                    ddlAnioMetricaSeleccion.Items.Add(new ListItem(anioStr, anioStr));
                ddlAnioMetricaSeleccion.SelectedValue = anioStr;

                CargarGruposPendientes(obj.fkId_gru);
                ddlGrupoAdd.SelectedValue = obj.fkId_gru;

                txtFechaAdd.Text = obj.dtFecha_valo.ToString("yyyy-MM-dd");
                txtPuntajeAdd.Text = obj.intPuntaje_valo.HasValue ? obj.intPuntaje_valo.ToString() : "";
                txtReconocimientoAdd.Text = obj.strReconocimiento_valo;

                if (!string.IsNullOrEmpty(obj.strInforme_valo))
                {
                    pnlInfoEvaluacion.Visible = true;
                    lnkVerEvaluacionActual.NavigateUrl = obj.strInforme_valo; 
                    pnlUploadEvaluacion.Style["display"] = "none";
                }
                else
                {
                    pnlInfoEvaluacion.Visible = false;
                    pnlUploadEvaluacion.Style["display"] = "block";
                }

                contenedorResolucionTotal.Visible = true; 

                if (!string.IsNullOrEmpty(obj.strResolucion_valo))
                {
                    pnlInfoResolucion.Visible = true;
                    lnkVerResolucionActual.NavigateUrl = obj.strResolucion_valo; 
                    pnlUploadResolucion.Style["display"] = "none";
                }
                else
                {
                    pnlInfoResolucion.Visible = false;
                    pnlUploadResolucion.Style["display"] = "block";
                }

                CambiarVista(Vista.Formulario);
            }
        }

        private void LimpiarFormulario()
        {
            ddlGrupoAdd.SelectedIndex = -1;
            txtFechaAdd.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtPuntajeAdd.Text = "";
            txtReconocimientoAdd.Text = "";

            string anioActual = DateTime.Now.Year.ToString();
            if (ddlAnioMetricaSeleccion.Items.FindByValue(anioActual) != null)
                ddlAnioMetricaSeleccion.SelectedValue = anioActual;

            ActualizarMetricaVisual();

            pnlInfoEvaluacion.Visible = false;     
            pnlUploadEvaluacion.Style["display"] = "block";

            contenedorResolucionTotal.Visible = true; 
            contenedorResolucionTotal.Style["display"] = "none";

            pnlInfoResolucion.Visible = false;
            pnlUploadResolucion.Style["display"] = "block";

            IdCalificacionEnEdicion = null;
        }


        //
        protected void btnHistorialGlobal_Click(object sender, EventArgs e)
        {
            try
            {
                var historial = _manejador.ObtenerHistorialGlobal();

                rptHistorial.DataSource = historial;
                rptHistorial.DataBind();

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "OpenHistorial",
                    "AbrirModalHistorial();",
                    true
                );
            }
            catch (Exception ex)
            {
                Msg("Error al cargar historial: " + ex.Message, "ee");
            }
        }

    }
}