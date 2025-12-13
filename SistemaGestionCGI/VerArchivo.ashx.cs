using System;
using System.Web;
using System.IO;
using SistemaGestionCGI.BLL; // Asegúrate de importar tu BLL

namespace SistemaGestionCGI
{
    public class VerArchivo : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            string id = context.Request.QueryString["id"];

            if (string.IsNullOrEmpty(id))
            {
                context.Response.Write("ERROR: ID no proporcionado en la URL.");
                return;
            }

            ManejadorCalificacionGrupo manejador = new ManejadorCalificacionGrupo();
            var calificacion = manejador.ObtenerPorId(id);

            if (calificacion == null)
            {
                // DIAGNÓSTICO: Mostramos qué ID intentó buscar
                context.Response.ContentType = "text/plain";
                context.Response.Write($"ERROR: No se encontró el registro en BD.\n");
                context.Response.Write($"ID Buscado: [{id}]\n");
                context.Response.Write("Verifique que el ID exista en la tabla INVGCCCALIFICACION_GRUPO y que el método BLL.ObtenerPorId tenga comillas simples en el SQL.");
                return;
            }

            if (string.IsNullOrEmpty(calificacion.strInforme_valo))
            {
                context.Response.Write($"ERROR: El registro [{id}] existe, pero el campo 'strInforme_valo' está vacío en la BD.");
                return;
            }

            string rutaFisica = calificacion.strInforme_valo;

            if (File.Exists(rutaFisica))
            {
                context.Response.ContentType = "application/pdf";
                context.Response.AddHeader("Content-Disposition", "inline; filename=Informe.pdf");
                context.Response.WriteFile(rutaFisica);
            }
            else
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write($"ERROR: Archivo físico no encontrado.\n");
                context.Response.Write($"Ruta buscada: {rutaFisica}");
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}