using System;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using SistemaGestionCGI.BLL;    // Asegura tus namespaces
using SistemaGestionCGI.Models;

namespace SistemaGestionCGI
{
    public partial class GeneradorInforme : System.Web.UI.UserControl
    {
        // Instancia del manejador (propia del control)
        private readonly ManejadorEjecucionProyectos _manejador = new ManejadorEjecucionProyectos();

        // EVENTO: Para avisar al padre que refresque la grilla
        public event EventHandler InformeGuardado;

        protected void Page_Load(object sender, EventArgs e) { }

        // --- MÉTODO PÚBLICO PARA ABRIR EL MODAL ---
        public void Mostrar(int idEjecucion)
        {
            LimpiarCampos();
            hfIdEjecucionInterno.Value = idEjecucion.ToString();

            // --- AUTO-COMPLETADO INTELIGENTE ---
            // Buscamos los miembros en la BD para no escribirlos de nuevo
            try
            {
                var miembros = _manejador.ObtenerMiembros(idEjecucion);
                if (miembros != null && miembros.Count > 0)
                {
                    StringBuilder sbDocNombres = new StringBuilder();
                    StringBuilder sbDocCedulas = new StringBuilder();
                    StringBuilder sbDocCarreras = new StringBuilder();

                    StringBuilder sbEstNombres = new StringBuilder();
                    StringBuilder sbEstCedulas = new StringBuilder();
                    StringBuilder sbEstCarreras = new StringBuilder();

                    foreach (var m in miembros)
                    {
                        // Filtramos por Rol (Ajusta el string según tu BD: "Docente", "Estudiante", "Investigador")
                        string rol = m.strRol_miembro.ToLower();

                        if (rol.Contains("docente") || rol.Contains("investigador") || rol.Contains("director"))
                        {
                            sbDocNombres.AppendLine($"{m.strApellidos_miembro} {m.strNombres_miembro}");
                            sbDocCedulas.AppendLine(m.strCedula_miembro);
                            sbDocCarreras.AppendLine(m.strFacultad_miembro); // O carrera si tienes
                        }
                        else
                        {
                            // Asumimos Estudiante u otro
                            sbEstNombres.AppendLine($"{m.strApellidos_miembro} {m.strNombres_miembro}");
                            sbEstCedulas.AppendLine(m.strCedula_miembro);
                            sbEstCarreras.AppendLine(m.strFacultad_miembro);
                        }
                    }

                    // Llenamos los TextBoxes
                    txtDocNombres.Text = sbDocNombres.ToString();
                    txtDocCedula.Text = sbDocCedulas.ToString();
                    txtDocCarrera.Text = sbDocCarreras.ToString();

                    txtEstNombres.Text = sbEstNombres.ToString();
                    txtEstCedula.Text = sbEstCedulas.ToString();
                    txtEstCarrera.Text = sbEstCarreras.ToString();
                }
            }
            catch { /* Si falla, simplemente salen vacíos */ }

            string script = $"resetWizard({idEjecucion}); var m = new bootstrap.Modal(document.getElementById('modalGeneradorInforme')); m.show();";

            ScriptManager.RegisterStartupScript(this, GetType(), "OpenWiz", script, true);
        }

        private void LimpiarCampos()
        {
            txtGenGrupoInv.Text = ""; txtGenPeriodo.Text = ""; txtGenAvance.Text = "";
            txtGenObjetivos.Text = ""; txtGenPlanificadas.Text = ""; txtGenEjecutadas.Text = "";
            txtGenParticipantes.Text = ""; txtGenInstitucion.Text = ""; txtGenResultados.Text = "";
            txtPresupRubro.Text = ""; txtPresupAsignado.Text = ""; txtPresupEjecutado.Text = "";
            txtPresupPorcentaje.Text = ""; txtPresupObservacion.Text = "";
        }

        protected void btnGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                // =========================================================================
                // 1. VALIDACIÓN Y DATOS INICIALES
                // =========================================================================
                if (!int.TryParse(hfIdEjecucionInterno.Value, out int idEjecucion)) return;

                string tipoFormato = ((LinkButton)sender).CommandArgument; // "WORD" o "PDF"
                var proyecto = _manejador.ObtenerEjecucionPorId(idEjecucion); // Datos base del proyecto

                // =========================================================================
                // 2. CONSTRUCCIÓN DEL HTML (ESTRUCTURA PROFESIONAL)
                // =========================================================================
                StringBuilder sb = new StringBuilder();

                // Cabeceras XML para Word (Vital para márgenes y orientación)
                sb.Append("<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:w='urn:schemas-microsoft-com:office:word' xmlns='http://www.w3.org/TR/REC-html40'>");
                sb.Append("<head><meta charset='utf-8'>");
                sb.Append("<style>");

                // --- CONFIGURACIÓN DE PÁGINA HORIZONTAL (LANDSCAPE) ---
                sb.Append("@page Section1 { size: 841.9pt 595.3pt; mso-page-orientation: landscape; margin: 2cm; }");
                sb.Append("div.Section1 { page: Section1; }");

                // --- ESTILOS GENERALES ---
                sb.Append("body { font-family: 'Times New Roman', serif; font-size: 11pt; margin: 0; line-height: 1.1; }");

                // Clase para salto de página forzado
                sb.Append(".page-break { page-break-before: always; }");

                // --- ESTILOS DE TABLAS (PROFESIONAL) ---
                sb.Append("table { width: 100%; border-collapse: collapse; margin-bottom: 20px; table-layout: fixed; }");

                // TH: Gris Word (#D9D9D9), Centrado, Negrita
                sb.Append("th { background-color: #D9D9D9; border: 1px solid #000; padding: 5px; font-weight: bold; text-align: center; vertical-align: middle; font-size: 9pt; }");

                // TD: Ajuste de texto, alineación vertical media
                sb.Append("td { border: 1px solid #000; padding: 5px; text-align: left; vertical-align: middle; font-size: 10pt; word-wrap: break-word; }");

                // Utilidades
                sb.Append(".text-center { text-align: center; } .text-bold { font-weight: bold; } .uppercase { text-transform: uppercase; }");
                sb.Append(".info-table td { border: none; vertical-align: top; } .center-data { text-align: center; }");

                sb.Append("</style></head><body>");

                // APERTURA DEL CONTENEDOR "LANDSCAPE"
                sb.Append("<div class='Section1'>");


                // =========================================================================
                // PÁGINA 1: DATOS GENERALES Y MATRIZ DE ACTIVIDADES
                // =========================================================================

                // 1. Encabezado
                sb.Append("<div class='text-center text-bold uppercase' style='font-size:14pt; margin-bottom:20px;'>FORMATO PARA LOS INFORME DE AVANCES POR PERIÓDOS ACADÉMICOS<br>DEL PROYECTO ACADÉMICO CIENTÍFICO</div>");

                // 2. Datos Informativos (Tabla sin bordes)
                sb.Append("<table class='info-table'>");
                sb.Append($"<tr><td style='width:25%'><b>Nombre del proyecto:</b></td><td>{proyecto.TituloProyecto.ToUpper()}</td></tr>");
                sb.Append($"<tr><td><b>Grupo de Investigación:</b></td><td>{txtGenGrupoInv.Text.ToUpper()}</td></tr>");
                sb.Append($"<tr><td><b>Período de seguimiento:</b></td><td>{txtGenPeriodo.Text.ToUpper()}</td></tr>");
                sb.Append("</table>");

                // 3. Matriz Punto 1
                sb.Append("<div class='text-bold uppercase' style='margin-bottom:5px; font-size:11pt;'>ACTIVIDADES PLANIFICADAS - ACTIVIDADES EJECUTADAS Y RESULTADOS</div>");

                sb.Append("<table><thead><tr>");
                sb.Append("<th style='width:15%'>COMPONENTE /<br>OBJETIVOS</th>");
                sb.Append("<th style='width:20%'>ACTIVIDADES<br>PLANIFICADAS</th>");
                sb.Append("<th style='width:20%'>ACTIVIDADES<br>EJECUTADAS</th>");
                sb.Append("<th style='width:15%'>DOCENTES /<br>INVESTIGADORES</th>");
                sb.Append("<th style='width:10%'>INSTITUCIÓN /<br>CARRERA</th>");
                sb.Append("<th style='width:6%'>% DE<br>CUMP.</th>");
                sb.Append("<th style='width:14%'>RESULTADOS<br>ALCANZADOS</th>");
                sb.Append("</tr></thead><tbody><tr>");

                sb.Append($"<td>{txtGenObjetivos.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td>{txtGenPlanificadas.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td>{txtGenEjecutadas.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td>{txtGenParticipantes.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td class='center-data'>{txtGenInstitucion.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td class='center-data'>{txtGenAvance.Text}%</td>");
                sb.Append($"<td>{txtGenResultados.Text.Replace("\n", "<br>")}</td>");

                sb.Append("</tr></tbody></table>");


                // =========================================================================
                // PÁGINA 2: INVESTIGADORES (SALTO DE PÁGINA)
                // =========================================================================

                // Salto de página compatible con Word y HTML
                if (tipoFormato == "WORD")
                    sb.Append("<br clear=all style='mso-special-character:line-break;page-break-before:always'>");
                else
                    sb.Append("<div class='page-break'></div>");

                sb.Append("<div class='text-bold uppercase' style='font-size:12pt; margin-bottom:15px; margin-top:20px;'>2. INVESTIGADORES PARTICIPANTES DEL PROYECTO</div>");

                // --- TABLA 2.1 DOCENTES ---
                sb.Append("<div class='text-bold' style='margin-bottom:5px;'>2.1. DOCENTES PARTICIPANTES</div>");
                sb.Append("<table><thead><tr>");
                sb.Append("<th style='width:20%'>NOMBRES Y APELLIDOS</th>");
                sb.Append("<th style='width:10%'>CÉDULA</th>");
                sb.Append("<th style='width:15%'>CARRERA</th>");
                sb.Append("<th style='width:20%'>FACULTAD / EXTENSIÓN</th>");
                sb.Append("<th style='width:10%'>HORAS</th>");
                sb.Append("<th style='width:25%'>OBSERVACIONES</th>");
                sb.Append("</tr></thead><tbody><tr>");

                // Datos del Paso 2 (Docentes)
                sb.Append($"<td>{txtDocNombres.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td class='center-data'>{txtDocCedula.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td>{txtDocCarrera.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td>UTC</td>"); // Por defecto UTC, o usa otro campo
                sb.Append($"<td class='center-data'>{txtDocHoras.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td>{txtDocObs.Text.Replace("\n", "<br>")}</td>");

                sb.Append("</tr></tbody></table>");

                // --- TABLA 2.2 ESTUDIANTES ---
                sb.Append("<div class='text-bold' style='margin-bottom:5px; margin-top:10px;'>2.2. ESTUDIANTES PARTICIPANTES</div>");
                sb.Append("<table><thead><tr>");
                sb.Append("<th style='width:25%'>NOMBRES Y APELLIDOS</th>");
                sb.Append("<th style='width:10%'>CÉDULA</th>");
                sb.Append("<th style='width:20%'>CARRERA</th>");
                sb.Append("<th style='width:20%'>ACTIVIDAD REALIZADA</th>");
                sb.Append("<th style='width:25%'>OBSERVACIONES</th>");
                sb.Append("</tr></thead><tbody><tr>");

                // Datos del Paso 2 (Estudiantes)
                sb.Append($"<td>{txtEstNombres.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td class='center-data'>{txtEstCedula.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td>{txtEstCarrera.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td>{txtEstActividad.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td>{txtEstObs.Text.Replace("\n", "<br>")}</td>");

                sb.Append("</tr></tbody></table>");


                // =========================================================================
                // PÁGINA 3 (O CONTINUACIÓN): PRESUPUESTO
                // =========================================================================

                sb.Append("<div class='text-bold uppercase' style='font-size:11pt; margin-bottom:5px; margin-top:25px;'>3. AVANCE EN LA GESTIÓN DE COMPRAS PÚBLICAS - EJECUCIÓN DE PRESUPUESTO</div>");

                sb.Append("<table><thead><tr>");
                sb.Append("<th style='width:35%'>RUBRO / DESCRIPCIÓN</th>");
                sb.Append("<th style='width:15%'>ASIGNADO ($)</th>");
                sb.Append("<th style='width:15%'>EJECUTADO ($)</th>");
                sb.Append("<th style='width:10%'>% EJEC.</th>");
                sb.Append("<th style='width:25%'>OBSERVACIONES</th>");
                sb.Append("</tr></thead><tbody><tr>");

                // Datos del Paso 3
                sb.Append($"<td>{txtPresupRubro.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td class='center-data'>{txtPresupAsignado.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td class='center-data'>{txtPresupEjecutado.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td class='center-data'>{txtPresupPorcentaje.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td>{txtPresupObservacion.Text.Replace("\n", "<br>")}</td>");

                sb.Append("</tr></tbody></table>");

                sb.Append("<div style='font-size:9pt; font-style:italic;'>Nota: Los valores deben coincidir con la planificación financiera.</div>");

                // CIERRE
                sb.Append("</div></body></html>");


                // =========================================================================
                // 3. GUARDADO DEL ARCHIVO FÍSICO
                // =========================================================================
                string nombreBase = $"INFORME_{proyecto.strId_ejec}_{DateTime.Now:yyyyMMdd_HHmm}";
                string carpetaDestino = Server.MapPath("~/RepositorioUTC/EjecucionInformes/");

                if (!Directory.Exists(carpetaDestino)) Directory.CreateDirectory(carpetaDestino);

                string nombreFinal = "";

                if (tipoFormato == "WORD")
                {
                    // Generación WORD
                    nombreFinal = nombreBase + ".doc";
                    string rutaDoc = Path.Combine(carpetaDestino, nombreFinal);
                    File.WriteAllText(rutaDoc, sb.ToString(), Encoding.UTF8);
                }
                else
                {
                    // Generación PDF (Requiere wkhtmltopdf en carpeta Binarios)
                    nombreFinal = nombreBase + ".pdf";
                    string rutaPdf = Path.Combine(carpetaDestino, nombreFinal);

                    // Llamamos al método auxiliar. IMPORTANTE: "-O Landscape"
                    bool exito = GenerarPdfConExe(sb.ToString(), rutaPdf);

                    if (!exito)
                    {
                        // Si falla el PDF, lanzamos alerta en JS
                        ScriptManager.RegisterStartupScript(this, GetType(), "AlertErr", "alert('Error: No se pudo generar el PDF. Verifique wkhtmltopdf.exe');", true);
                        return;
                    }
                }

                // =========================================================================
                // 4. REGISTRO EN BDD Y FINALIZACIÓN
                // =========================================================================
                var nuevoInforme = new InvgccEjecucionInformes();
                nuevoInforme.fkId_ejec = idEjecucion;
                nuevoInforme.strNombrePeriodo = string.IsNullOrEmpty(txtGenPeriodo.Text) ? "Informe Generado" : txtGenPeriodo.Text;
                nuevoInforme.strArchivo_path = "~/RepositorioUTC/EjecucionInformes/" + nombreFinal;

                _manejador.GuardarInforme(nuevoInforme);

                string scriptFinal = @"
                    limpiarBorrador(); 
                    bootstrap.Modal.getInstance(document.getElementById('modalGeneradorInforme')).hide();
                ";

                // Cerrar modal
                string scriptCierre = "limpiarBorrador(); bootstrap.Modal.getInstance(document.getElementById('modalGeneradorInforme')).hide();";

                ScriptManager.RegisterStartupScript(this, GetType(), "CloseWiz", scriptCierre, true);

                InformeGuardado?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ErrorCritico", $"alert('Error crítico: {ex.Message}');", true);
            }
        }

        // Pega aquí también el método private bool GenerarPdfConExe(...)
        private bool GenerarPdfConExe(string htmlContent, string rutaPdfDestino)
        {
            try
            {
                string rutaHtmlTemp = Path.Combine(Server.MapPath("~/Binarios/"), $"temp_{Guid.NewGuid()}.html");
                File.WriteAllText(rutaHtmlTemp, htmlContent, Encoding.UTF8);

                string rutaExe = Server.MapPath("~/Binarios/wkhtmltopdf.exe");
                if (!File.Exists(rutaExe)) return false;

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = rutaExe;
                psi.Arguments = $"-q -O Landscape --encoding utf-8 \"{rutaHtmlTemp}\" \"{rutaPdfDestino}\"";
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.RedirectStandardError = true;

                using (Process proc = Process.Start(psi))
                {
                    proc.WaitForExit();
                }

                if (File.Exists(rutaHtmlTemp)) File.Delete(rutaHtmlTemp);

                return File.Exists(rutaPdfDestino);
            }
            catch
            {
                return false;
            }
        }

        private void Msg(string msg, string type)
        {
            string cleanMsg = msg.Replace("'", "\\'").Replace("\r\n", " ").Replace("\n", " ").Replace("\\", "\\\\");
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"$(function() {{ toastify('{type}', '{cleanMsg}', 'Sistema'); }});", true);
        }

    }
}