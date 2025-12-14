using System;
using System.Web;
using System.IO;
using SistemaGestionCGI.BLL;

namespace SistemaGestionCGI
{
    public class VerArchivo : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            // 1. RECIBIR PARÁMETROS
            string idStr = context.Request.QueryString["id"];
            string tipo = context.Request.QueryString["tipo"]; // GRUPO, CALIFICACION, EJECUCION, INFORME

            // Validación básica
            if (string.IsNullOrEmpty(idStr) || string.IsNullOrEmpty(tipo))
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("Error: Faltan parámetros (id o tipo).");
                return;
            }

            string rutaFisica = "";

            try
            {
                // 2. SWITCH PARA ELEGIR EL MÓDULO CORRECTO
                switch (tipo.ToUpper())
                {
                    // === CASO 1: Calificaciones (ID String) ===
                    case "CALIFICACION":
                        ManejadorCalificacionGrupo mCalif = new ManejadorCalificacionGrupo();
                        var calif = mCalif.ObtenerPorId(idStr); // ID es string (VAL-1)
                        if (calif != null) rutaFisica = calif.strInforme_valo;
                        break;

                    // === CASO 2: Grupos (ID String) ===
                    case "GRUPO":
                        ManejadorGruposInvestigacion mGrupo = new ManejadorGruposInvestigacion();
                        var grupo = mGrupo.ObtenerGrupoPorId(idStr); // ID es string (G-1)
                        if (grupo != null) rutaFisica = grupo.strArchivo_gru;
                        break;

                    // === CASO 3: Ejecución - Plan Inicial (ID Int) ===
                    case "EJECUCION":
                        if (int.TryParse(idStr, out int idEjec))
                        {
                            ManejadorEjecucionProyectos mEjec = new ManejadorEjecucionProyectos();
                            var ejec = mEjec.ObtenerEjecucionPorId(idEjec);
                            if (ejec != null) rutaFisica = ejec.strInforme_ejec;
                        }
                        else
                        {
                            context.Response.Write("Error: ID de Ejecución inválido (debe ser numérico).");
                            return;
                        }
                        break;

                    // === CASO 4: Informes de Avance (ID Int) ===
                    case "INFORME":
                        if (int.TryParse(idStr, out int idInf))
                        {
                            ManejadorEjecucionProyectos mInf = new ManejadorEjecucionProyectos();
                            var informe = mInf.ObtenerInformePorId(idInf);
                            if (informe != null) rutaFisica = informe.strArchivo_path;
                        }
                        else
                        {
                            context.Response.Write("Error: ID de Informe inválido (debe ser numérico).");
                            return;
                        }
                        break;

                    default:
                        context.Response.Write($"Error: Tipo de archivo '{tipo}' no reconocido.");
                        return;
                }

                // 3. SERVIR EL ARCHIVO
                if (!string.IsNullOrEmpty(rutaFisica) && File.Exists(rutaFisica))
                {
                    string nombreArchivo = Path.GetFileName(rutaFisica);
                    string extension = Path.GetExtension(rutaFisica).ToLower();

                    // Content Type
                    if (extension == ".pdf") context.Response.ContentType = "application/pdf";
                    else if (extension == ".jpg" || extension == ".jpeg") context.Response.ContentType = "image/jpeg";
                    else if (extension == ".png") context.Response.ContentType = "image/png";
                    else if (extension == ".xlsx") context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    else context.Response.ContentType = "application/octet-stream";

                    // inline = ver en navegador
                    context.Response.AddHeader("Content-Disposition", "inline; filename=" + nombreArchivo);

                    context.Response.WriteFile(rutaFisica);
                }
                else
                {
                    // Error visual si no existe en disco
                    context.Response.ContentType = "text/plain";
                    context.Response.Write("ERROR: El archivo físico no existe en el servidor.\n");
                    context.Response.Write($"Ruta buscada: {rutaFisica}");
                }
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("Error interno del servidor: " + ex.Message);
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}