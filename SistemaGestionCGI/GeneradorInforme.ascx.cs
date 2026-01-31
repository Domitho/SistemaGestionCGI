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
                // 1. OBTENER DATOS
                if (!int.TryParse(hfIdEjecucionInterno.Value, out int idEjecucion)) return;
                string tipoFormato = ((LinkButton)sender).CommandArgument; // WORD o PDF
                var proyecto = _manejador.ObtenerEjecucionPorId(idEjecucion);

                // 2. CONSTRUCCIÓN DEL HTML (DISEÑO FIEL AL WORD)
                StringBuilder sb = new StringBuilder();

                // --- CABECERAS Y ESTILOS CSS (BORDES TIPO WORD) ---
                sb.Append("<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:w='urn:schemas-microsoft-com:office:word'>");
                sb.Append("<head><meta charset='utf-8'><style>");

                // Configuración A4 Horizontal
                sb.Append("@page Section1 { size: 841.9pt 595.3pt; mso-page-orientation: landscape; margin: 1.5cm; }");
                sb.Append("div.Section1 { page: Section1; }");

                // Fuente Base
                sb.Append("body { font-family: 'Times New Roman', serif; font-size: 10pt; line-height: 1.1; }");

                // === ESTILOS DE CUADRÍCULA (BORDES NEGROS FINOS) ===
                // border-collapse: collapse es VITAL para que se vea como rejilla
                sb.Append("table { width: 100%; border-collapse: collapse; margin-bottom: 15px; table-layout: fixed; }");
                sb.Append("th, td { border: 1px solid black; padding: 4px; vertical-align: middle; word-wrap: break-word; }");

                // Cabeceras de Tabla (Gris Word)
                sb.Append("th { background-color: #D9D9D9; font-weight: bold; text-align: center; font-size: 8pt; }");

                // Celdas de Datos
                sb.Append("td { text-align: left; font-size: 9pt; }");

                // Clases Utilitarias
                sb.Append(".no-border { border: none !important; }"); // Para los datos informativos (sin borde)
                sb.Append(".text-center { text-align: center; }");
                sb.Append(".text-bold { font-weight: bold; }");
                sb.Append(".page-break { page-break-before: always; }");
                sb.Append(".section-title { font-weight: bold; margin-bottom: 5px; margin-top: 15px; text-transform: uppercase; font-size: 10pt; }");

                sb.Append("</style></head><body>");
                sb.Append("<div class='Section1'>"); // Inicio Landscape

                // ==========================================================================================
                // CABECERA INSTITUCIONAL
                // ==========================================================================================
                sb.Append("<div class='text-center text-bold' style='font-size:12pt; margin-bottom:15px;'>");
                sb.Append("FORMATO PARA LOS INFORME DE AVANCES POR PERIÓDOS ACADÉMICOS<br>");
                sb.Append("DEL PROYECTO ACADÉMICO CIENTÍFICO");
                sb.Append("</div>");

                // Datos Informativos (Tabla SIN BORDES, como texto plano alineado)
                sb.Append("<table style='border:none; margin-bottom:20px;'>");
                sb.Append($"<tr><td class='no-border' style='width:300px;'><b>Nombre del proyecto:</b></td><td class='no-border'>{proyecto.TituloProyecto}</td></tr>");
                sb.Append($"<tr><td class='no-border'><b>Grupo de Investigación al que pertenece el proyecto:</b></td><td class='no-border'>{txtGenGrupoInv.Text}</td></tr>");
                sb.Append($"<tr><td class='no-border'><b>Período de seguimiento:</b></td><td class='no-border'>{txtGenPeriodo.Text}</td></tr>");
                sb.Append("</table>");


                // ==========================================================================================
                // 1. ACTIVIDADES (MATRIZ PRINCIPAL)
                // ==========================================================================================
                sb.Append("<div class='section-title'>ACTIVIDADES PLANIFICADAS - ACTIVIDADES EJECUTADAS Y RESULTADOS</div>");

                sb.Append("<table><thead><tr>");
                sb.Append("<th style='width:15%'>COMPONENTE/<br>OBJETIVOS</th>");
                sb.Append("<th style='width:18%'>ACTIVIDADES<br>PLANIFICADAS</th>");
                sb.Append("<th style='width:18%'>ACTIVIDADES<br>EJECUTADAS</th>");
                sb.Append("<th style='width:15%'>DOCENTES/INVESTIGADORES<br>PARTICIPANTES/COLABORADORES</th>");
                sb.Append("<th style='width:12%'>INSTITUCIÓN/CARRERA<br>A LA QUE PERTENECE</th>"); // Ajustado al doc
                sb.Append("<th style='width:7%'>% DE<br>CUMPLIMIENTO</th>");
                sb.Append("<th style='width:15%'>RESULTADOS<br>ALCANZADOS</th>");
                sb.Append("</tr></thead><tbody><tr>");

                sb.Append($"<td>{txtGenObjetivos.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td>{txtGenPlanificadas.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td>{txtGenEjecutadas.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td>{txtGenParticipantes.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td class='text-center'>{txtGenInstitucion.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td class='text-center'>{txtGenAvance.Text}%</td>");
                sb.Append($"<td>{txtGenResultados.Text.Replace("\n", "<br>")}</td>");
                sb.Append("</tr></tbody></table>");


                // ==========================================================================================
                // 2. INVESTIGADORES (SALTO DE PÁGINA)
                // ==========================================================================================
                sb.Append("<br><div class='section-title'>INVESTIGADORES PARTICIPANTES DEL PROYECTO</div>");

                // 2.1 DOCENTES
                sb.Append("<div class='text-bold' style='font-size:9pt;'>2.1. DOCENTES PARTICIPANTES</div>");
                sb.Append("<table><thead><tr>");
                sb.Append("<th style='width:20%'>NOMBRES Y APELLIDOS</th>");
                sb.Append("<th style='width:10%'>CÉDULA</th>");
                sb.Append("<th style='width:10%'>CARRERA</th>");
                sb.Append("<th style='width:15%'>FACULTAD Y/O EXTENSIÓN/O EXTERNO A LA UTC</th>");
                sb.Append("<th style='width:10%'>PERÍODO DE PARTICIPACIÓN<br>(CICLO ACADÉMICO)</th>");
                sb.Append("<th style='width:10%'>TOTAL HORAS (DISTRIBUTIVO)</th>");
                sb.Append("<th style='width:10%'>TOTAL HORAS (SIN ASIGNACIÓN)</th>");
                sb.Append("<th style='width:15%'>OBSERVACIONES</th>");
                sb.Append("</tr></thead><tbody><tr>");

                sb.Append($"<td>{txtDocNombres.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td class='text-center'>{txtDocCedula.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td>{txtDocCarrera.Text.Replace("\n", "<br>")}</td>"); // Asumiendo carrera en este campo
                sb.Append($"<td>UTC</td>"); // O usar un campo facultad
                sb.Append($"<td>{txtGenPeriodo.Text}</td>");
                sb.Append($"<td class='text-center'>{txtDocHoras.Text}</td>");
                sb.Append("<td class='text-center'>0</td>"); // Campo nuevo del doc
                sb.Append($"<td>{txtDocObs.Text.Replace("\n", "<br>")}</td>");
                sb.Append("</tr></tbody></table>");

                sb.Append("<div style='font-size:8pt; font-style:italic; margin-bottom:10px;'>Nota: Se deben reportar solo los investigadores registrados...</div>");

                // 2.2 ESTUDIANTES
                sb.Append("<div class='text-bold' style='font-size:9pt;'>2.2. ESTUDIANTES PARTICIPANTES</div>");
                sb.Append("<table><thead><tr>");
                sb.Append("<th style='width:25%'>NOMBRES Y APELLIDOS</th>");
                sb.Append("<th style='width:10%'>CÉDULA</th>");
                sb.Append("<th style='width:15%'>CARRERA</th>");
                sb.Append("<th style='width:15%'>FACULTAD Y/O EXTENSIÓN/O EXTERNOS AL UTC</th>");
                sb.Append("<th style='width:10%'>PERÍODO DE PARTICIPACIÓN</th>");
                sb.Append("<th style='width:10%'>ACTIVIDAD REALIZADA</th>");
                sb.Append("<th style='width:15%'>OBSERVACIONES</th>");
                sb.Append("</tr></thead><tbody><tr>");

                sb.Append($"<td>{txtEstNombres.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td class='text-center'>{txtEstCedula.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td>{txtEstCarrera.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td>UTC</td>");
                sb.Append($"<td>{txtGenPeriodo.Text}</td>");
                sb.Append($"<td>{txtEstActividad.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td>{txtEstObs.Text.Replace("\n", "<br>")}</td>");
                sb.Append("</tr></tbody></table>");


                // ==========================================================================================
                // 3. TITULACIONES (SALTO)
                // ==========================================================================================
                if (tipoFormato == "WORD") sb.Append("<br clear=all style='page-break-before:always'>"); else sb.Append("<div class='page-break'></div>");

                sb.Append("<div class='section-title'>TITULACIONES DE TERCER O CUARTO NIVEL DERIVADAS DEL PROYECTO</div>");
                sb.Append("<div class='text-bold' style='font-size:9pt;'>TITULACIÓN DE ESTUDIANTES PARTICIPANTES</div>");

                sb.Append("<table><thead><tr>");
                sb.Append("<th>NOMBRES Y APELLIDOS</th><th>CÉDULA</th><th>CARRERA/PROGRAMA</th><th>FACULTAD Y/O EXTENSIÓN</th>");
                sb.Append("<th>TÍTULO</th><th>PERÍODO TITULACIÓN</th><th>OBSERVACIONES</th>");
                sb.Append("</tr></thead><tbody><tr>");
                sb.Append($"<td colspan='7'>{txtTitDetalle.Text.Replace("\n", "<br>")}</td>"); // Fila única con datos
                sb.Append("</tr></tbody></table>");


                // ==========================================================================================
                // 4. INTEGRACIÓN CURRICULAR / VINCULACIÓN
                // ==========================================================================================
                sb.Append("<div class='section-title'>COMPONENTE DE INTEGRACIÓN CURRICULAR, PRÁCTICA Y/O VINCULACIÓN REALIZADA (SI APLICA)</div>");
                sb.Append("<div class='text-bold' style='font-size:9pt;'>INTEGRACIÓN CURRICULAR, PRÁCTICA Y/O VINCULACIÓN REALIZADA (SI APLICA)</div>");

                sb.Append("<table><thead><tr>");
                sb.Append("<th>ACTIVIDAD</th><th>SECTOR</th><th>TOTAL BENEFICIARIOS</th>");
                sb.Append("<th>NOMBRES PARTICIPANTES</th><th>HORAS</th><th>FACULTAD/CARRERA</th>");
                sb.Append("<th>RESPONSABLE</th><th>RESULTADOS ALCANZADOS</th>");
                sb.Append("</tr></thead><tbody><tr>");
                sb.Append($"<td colspan='8'>{txtVinculacion.Text.Replace("\n", "<br>")}</td>");
                sb.Append("</tr></tbody></table>");


                // ==========================================================================================
                // 5. INNOVACIÓN / SENADI
                // ==========================================================================================
                sb.Append("<div class='section-title'>COMPONENTE DE INNOVACIÓN Y/O REGISTRO DE PROPIEDAD INTELECTUAL (SEGÚN SENADI) (SI APLICA)</div>");
                sb.Append("<div class='text-bold' style='font-size:9pt;'>INNOVACIÓN Y/O REGISTRO DE PROPIEDAD INTELECTUAL (SI APLICA)</div>");

                sb.Append("<table><thead><tr>");
                sb.Append("<th>ACTIVIDAD Y/O COMPONENTE</th><th>CANTIDAD PARTICIPANTES</th><th>NOMBRES Y APELLIDOS</th>");
                sb.Append("<th>CÉDULA</th><th>FACULTAD / EXTENSIÓN / CARRERA</th>");
                sb.Append("</tr></thead><tbody><tr>");
                sb.Append($"<td colspan='5'>{txtInnovacion.Text.Replace("\n", "<br>")}</td>");
                sb.Append("</tr></tbody></table>");


                // ==========================================================================================
                // 6. CONVENIOS
                // ==========================================================================================
                sb.Append("<div class='section-title'>CONVENIOS INTERINSTITUCIONALES ESCRITOS EN EL PERÍODO DE SEGUIMIENTO (SI APLICA)</div>");
                sb.Append("<div class='text-bold' style='font-size:9pt;'>CONVENIOS INTERINSTITUCIONALES ESCRITOS EN EL PERÍODO DE SEGUIMIENTO (SI APLICA)</div>");

                sb.Append("<table><thead><tr>");
                sb.Append("<th>ENTIDAD</th><th>PERÍODO VIGENCIA</th><th>FECHA SUSCRIPCIÓN</th>");
                sb.Append("<th>RESPONSABLE SEGUIMIENTO</th><th>CARRERA/PROGRAMA</th><th>FACULTAD Y/O EXTENSIÓN</th>");
                sb.Append("</tr></thead><tbody><tr>");
                sb.Append($"<td colspan='6'>{txtConvenios.Text.Replace("\n", "<br>")}</td>");
                sb.Append("</tr></tbody></table>");


                // ==========================================================================================
                // 7. PRODUCCIÓN CIENTÍFICA (SALTO)
                // ==========================================================================================
                if (tipoFormato == "WORD") sb.Append("<br clear=all style='page-break-before:always'>"); else sb.Append("<div class='page-break'></div>");

                sb.Append("<div class='section-title'>PRODUCCIÓN CIENTÍFICA DERIVADA DEL PROYECTO*</div>");

                // 7.1 ARTÍCULOS
                sb.Append("<div class='text-bold' style='font-size:9pt; margin-top:5px;'>7.1. ARTÍCULOS CIENTÍFICOS</div>");
                sb.Append("<table><thead><tr>");
                sb.Append("<th>AÑO</th><th>BASE DE DATOS</th><th>TÍTULO PUBLICACIÓN</th><th>AUTORES</th><th>ISSN REVISTA</th>");
                sb.Append("<th>NOMBRE REVISTA</th><th>VOLUMEN/NÚMERO</th><th>FILIACIÓN INSTITUCIONAL</th><th>URL/DOI</th>");
                sb.Append("</tr></thead><tbody><tr>");
                sb.Append($"<td colspan='9'>{txtProdArticulos.Text.Replace("\n", "<br>")}</td>");
                sb.Append("</tr></tbody></table>");

                // 7.2 LIBROS
                sb.Append("<div class='text-bold' style='font-size:9pt;'>7.2. LIBROS</div>");
                sb.Append("<table><thead><tr>");
                sb.Append("<th>AÑO</th><th>CÓDIGO ISBN</th><th>TÍTULO OBRA</th><th>ESTADO</th><th>FILIACIÓN INSTITUCIONAL</th><th>AUTORES</th>");
                sb.Append("</tr></thead><tbody><tr>");
                sb.Append($"<td colspan='6'>{txtProdLibros.Text.Replace("\n", "<br>")}</td>");
                sb.Append("</tr></tbody></table>");

                // 7.3 CAPÍTULOS
                sb.Append("<div class='text-bold' style='font-size:9pt;'>7.3. CAPÍTULO DE LIBROS</div>");
                sb.Append("<table><thead><tr>");
                sb.Append("<th>AÑO PUBLICACIÓN</th><th>ISBN</th><th>NOMBRE LIBRO</th><th>NOMBRE CAPÍTULO</th><th>ESTADO</th><th>FILIACIÓN INSTITUCIONAL</th>");
                sb.Append("</tr></thead><tbody><tr>");
                sb.Append($"<td colspan='6'>{txtProdCapitulos.Text.Replace("\n", "<br>")}</td>");
                sb.Append("</tr></tbody></table>");

                // 7.4 PONENCIAS
                sb.Append("<div class='text-bold' style='font-size:9pt;'>7.4. PONENCIAS PRESENTADAS A EVENTOS</div>");
                sb.Append("<table><thead><tr>");
                sb.Append("<th>EVENTO</th><th>LUGAR EVENTO</th><th>FECHA PARTICIPACIÓN</th><th>TÍTULO PONENCIA</th><th>ISBN/ISSN</th><th>ESTADO</th>");
                sb.Append("</tr></thead><tbody><tr>");
                sb.Append($"<td colspan='6'>{txtProdPonencias.Text.Replace("\n", "<br>")}</td>");
                sb.Append("</tr></tbody></table>");

                sb.Append("<div style='font-size:8pt; font-style:italic;'>Nota: Toda la producción científica se debe subir a la plataforma de Ecuciencia</div>");


                // ==========================================================================================
                // 8. PRESUPUESTO (SALTO)
                // ==========================================================================================
                if (tipoFormato == "WORD") sb.Append("<br clear=all style='page-break-before:always'>"); else sb.Append("<div class='page-break'></div>");

                sb.Append("<div class='section-title'>AVANCE EN LA GESTIÓN DE COMPRAS PÚBLICAS DEL COMPONENTE PLANIFICADO EN LA ETAPA</div>");
                sb.Append("<div class='text-bold' style='font-size:9pt;'>EJECUCIÓN DE PRESUPUESTO</div>");

                sb.Append("<table><thead><tr>");
                sb.Append("<th style='width:40%'>RUBRO</th>");
                sb.Append("<th style='width:15%'>VALOR ASIGNADO</th>");
                sb.Append("<th style='width:15%'>VALOR EJECUTADO</th>");
                sb.Append("<th style='width:30%'>OBSERVACIONES*</th>");
                sb.Append("</tr></thead><tbody><tr>");
                sb.Append($"<td>{txtPresupRubro.Text.Replace("\n", "<br>")}</td>");
                sb.Append($"<td class='text-center'>{txtPresupAsignado.Text}</td>");
                sb.Append($"<td class='text-center'>{txtPresupEjecutado.Text}</td>");
                sb.Append($"<td>{txtPresupObservacion.Text.Replace("\n", "<br>")}</td>");
                sb.Append("</tr></tbody></table>");
                sb.Append("<div style='font-size:8pt;'>*Describir las novedades referidas a la gestión en compras públicas</div>");


                // ==========================================================================================
                // 9. CONCLUSIONES Y RECOMENDACIONES
                // ==========================================================================================
                sb.Append("<br><div class='section-title'>CONCLUSIONES/RECOMENDACIONES</div>");

                // 9.1
                sb.Append("<table><thead><tr><th style='text-align:left;'>9.1. CONCLUSIONES (Referente a los objetivos del proyecto)</th></tr></thead>");
                sb.Append($"<tbody><tr><td style='height:80px;'>{txtConclusiones.Text.Replace("\n", "<br>")}</td></tr></tbody></table>");

                // 9.2
                sb.Append("<table><thead><tr><th style='text-align:left;'>9.2. RECOMENDACIONES (Referente a perpectivas y lecciones aprendidas)</th></tr></thead>");
                sb.Append($"<tbody><tr><td style='height:80px;'>{txtRecomendaciones.Text.Replace("\n", "<br>")}</td></tr></tbody></table>");

                sb.Append("<div style='margin-top:10px; font-size:9pt;'>Anexos: Se deben adjuntar la documentación de evidencias físicas o digitales...</div>");


                // ==========================================================================================
                // FIRMAS DE RESPONSABILIDAD
                // ==========================================================================================
                sb.Append("<br><br><br>");

                // Tabla de Firmas (SIN BORDES DE CELDA, para maquetar)
                sb.Append("<table style='border:none; margin-top:30px;'>");
                sb.Append("<tr>");

                // Columna 1
                sb.Append("<td class='no-border text-center' style='padding:10px; width:33%;'>");
                sb.Append("ELABORADO POR<br>(Coordinador del proyecto):<br><br><br>");
                sb.Append("__________________________<br>");
                sb.Append("Nombre:.......................<br>");
                sb.Append("C.I.:.......................<br>");
                sb.Append("Fecha:.......................");
                sb.Append("</td>");

                // Columna 2
                sb.Append("<td class='no-border text-center' style='padding:10px; width:33%;'>");
                sb.Append("REVISADO POR<br>(Director de Inv. Facultad/Extensión):<br><br><br>");
                sb.Append("__________________________<br>");
                sb.Append("Nombre:.......................<br>");
                sb.Append("C.I.:.......................<br>");
                sb.Append("Fecha:.......................");
                sb.Append("</td>");

                // Columna 3
                sb.Append("<td class='no-border text-center' style='padding:10px; width:33%;'>");
                sb.Append("AUTORIZADO POR<br>(DGI):<br><br><br>");
                sb.Append("__________________________<br>");
                sb.Append("Nombre: Carlos Javier Torres Miño<br>");
                sb.Append("C.I.: 0502329238<br>");
                sb.Append("Fecha:.......................");
                sb.Append("</td>");

                sb.Append("</tr></table>");

                sb.Append("</div></body></html>"); // Fin Section1

                // ==========================================================================================
                // 3. GUARDADO DEL ARCHIVO FÍSICO
                // ==========================================================================================
                string nombreBase = $"INFORME_AVANCE_{proyecto.strId_ejec}_{DateTime.Now:yyyyMMdd_HHmm}";
                string carpetaDestino = Server.MapPath("~/RepositorioUTC/EjecucionInformes/");
                if (!Directory.Exists(carpetaDestino)) Directory.CreateDirectory(carpetaDestino);

                string nombreFinal = "";

                if (tipoFormato == "WORD")
                {
                    nombreFinal = nombreBase + ".doc";
                    string rutaDoc = Path.Combine(carpetaDestino, nombreFinal);
                    File.WriteAllText(rutaDoc, sb.ToString(), Encoding.UTF8);
                }
                else // PDF
                {
                    nombreFinal = nombreBase + ".pdf";
                    string rutaPdf = Path.Combine(carpetaDestino, nombreFinal);

                    // Asegúrate de que tu método GenerarPdfConExe usa "-O Landscape"
                    if (!GenerarPdfConExe(sb.ToString(), rutaPdf))
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "Alert", "alert('Error al generar PDF. Revise wkhtmltopdf.');", true);
                        return;
                    }
                }

                // ==========================================================================================
                // 4. REGISTRO EN BASE DE DATOS
                // ==========================================================================================

                var nuevoInforme = new InvgccEjecucionInformes
                {
                    fkId_ejec = idEjecucion,
                    strNombrePeriodo = txtGenPeriodo.Text,
                    strArchivo_path = "~/RepositorioUTC/EjecucionInformes/" + nombreFinal,
                };

                var datosProyecto = _manejador.ObtenerEjecucionPorId(nuevoInforme.fkId_ejec);
                string cicloActual = datosProyecto.strPeriodo_ejec;

                _manejador.GuardarInforme(nuevoInforme, cicloActual);

                // 5. CERRAR Y LIMPIAR
                string scriptFin = $"limpiarBorrador(); bootstrap.Modal.getInstance(document.getElementById('modalGeneradorInforme')).hide(); resetWizard({idEjecucion});";
                ScriptManager.RegisterStartupScript(this, GetType(), "CloseGen", scriptFin, true);

                InformeGuardado?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Err", $"alert('Error crítico: {ex.Message}');", true);
            }
        }

        protected void txtDocCedula_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string cedula = txtDocCedula.Text.Trim();
                if (string.IsNullOrEmpty(cedula)) return;

                // 1. Buscamos al docente en la base de datos
                // Asegúrate de que este método exista en tu manejador, o ajusta la llamada según tu lógica
                var docente = _manejador.BuscarDocentePorCedula(cedula);

                if (docente != null)
                {
                    // ¡ENCONTRADO! Llenamos los campos automáticamente
                    // Ajusta los nombres de las propiedades (strNombres_doc, etc.) según tu modelo real
                    txtDocNombres.Text = $"{docente.strApellidos_doc} {docente.strNombres_doc}";
                    txtDocCarrera.Text = $"{docente.strFacultad_doc} - {docente.strCarrera_doc}";

                    // Opcional: Feedback visual en JavaScript (si usas Toastify o alert)
                    ScriptManager.RegisterStartupScript(this, GetType(), "Found", "console.log('Docente encontrado');", true);
                }
                else
                {
                    // NO ENCONTRADO: Limpiamos para permitir escritura manual
                    // No borramos la cédula, solo los nombres para que el usuario los escriba
                    txtDocNombres.Text = "";
                    txtDocCarrera.Text = "";

                    // ScriptManager.RegisterStartupScript(this, GetType(), "NotFound", "alert('Cédula no encontrada. Ingrese datos manualmente.');", true);
                }

                // Mantener el foco en el campo de nombres para agilizar
                txtDocNombres.Focus();
            }
            catch (Exception ex)
            {
                // Manejo de error silencioso para no romper el flujo
                System.Diagnostics.Debug.WriteLine("Error al buscar docente: " + ex.Message);
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