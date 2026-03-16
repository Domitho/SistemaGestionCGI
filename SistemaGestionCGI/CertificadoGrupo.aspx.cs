using System;
using System.Collections.Generic;
using System.Text;
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;

namespace SistemaGestionCGI
{
    public partial class CertificadoGrupo : System.Web.UI.Page
    {
        private readonly ManejadorCertificados _manejador = new ManejadorCertificados();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            string idIntegrante = Request.QueryString["id"];

            if (string.IsNullOrWhiteSpace(idIntegrante))
            {
                litDatos.Text = "<div class='vacio'>No se recibió el identificador del integrante.</div>";
                litTimeline.Text = string.Empty;
                return;
            }

            CargarReporte(idIntegrante);
        }

        private void CargarReporte(string idIntegrante)
        {
            var integrante = _manejador.ObtenerCertificadoGrupoPorIdIntegrante(idIntegrante);

            if (integrante == null)
            {
                litDatos.Text = "<div class='vacio'>No se encontró información del integrante.</div>";
                litTimeline.Text = string.Empty;
                return;
            }

            var historial = _manejador.ObtenerHistorialPorIdIntegrante(idIntegrante);

            CargarDatos(integrante);
            CargarTimeline(historial);
        }

        private void CargarDatos(CertificadoGrupoDTO integrante)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<div class='info-grid'>");

            sb.Append("<div class='info-card'>");
            sb.Append("<span class='label'>Cédula</span>");
            sb.Append("<span class='valor'>" + Safe(integrante.Cedula) + "</span>");
            sb.Append("</div>");

            sb.Append("<div class='info-card'>");
            sb.Append("<span class='label'>Nombres</span>");
            sb.Append("<span class='valor'>" + Safe(integrante.Nombres) + "</span>");
            sb.Append("</div>");

            sb.Append("<div class='info-card'>");
            sb.Append("<span class='label'>Apellidos</span>");
            sb.Append("<span class='valor'>" + Safe(integrante.Apellidos) + "</span>");
            sb.Append("</div>");

            sb.Append("<div class='info-card'>");
            sb.Append("<span class='label'>Grupo de investigación</span>");
            sb.Append("<span class='valor'>" + Safe(integrante.NombreGrupo) + "</span>");
            sb.Append("</div>");

            sb.Append("<div class='info-card'>");
            sb.Append("<span class='label'>Código de grupo</span>");
            sb.Append("<span class='valor'>" + Safe(integrante.IdGrupo) + "</span>");
            sb.Append("</div>");

            sb.Append("<div class='info-card'>");
            sb.Append("<span class='label'>Módulo</span>");
            sb.Append("<span class='valor'>" + Safe(integrante.Modulo) + "</span>");
            sb.Append("</div>");

            sb.Append("<div class='info-card'>");
            sb.Append("<span class='label'>Función</span>");
            sb.Append("<span class='valor'>" + Safe(integrante.Funcion) + "</span>");
            sb.Append("</div>");

            sb.Append("<div class='info-card'>");
            sb.Append("<span class='label'>Estado</span>");
            sb.Append("<span class='valor'>" + Safe(integrante.Estado) + "</span>");
            sb.Append("</div>");

            sb.Append("<div class='info-card'>");
            sb.Append("<span class='label'>Fecha de inicio</span>");
            sb.Append("<span class='valor'>" + Safe(integrante.FechaInicio) + "</span>");
            sb.Append("</div>");

            sb.Append("</div>");

            litDatos.Text = sb.ToString();
        }

        private void CargarTimeline(List<HistorialIntegranteDTO> historial)
        {
            if (historial == null || historial.Count == 0)
            {
                litTimeline.Text = "<div class='vacio'>No hay historial registrado para este integrante.</div>";
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("<div class='timeline'>");

            foreach (var h in historial)
            {
                sb.Append("<div class='timeline-item'>");
                sb.Append("<div class='timeline-fecha'>" + Safe(h.Fecha) + "</div>");
                sb.Append("<div class='timeline-accion'>" + Safe(h.Accion) + "</div>");

                if (!string.IsNullOrWhiteSpace(h.Motivo))
                    sb.Append("<div class='timeline-detalle'><strong>Motivo:</strong> " + Safe(h.Motivo) + "</div>");

                if (!string.IsNullOrWhiteSpace(h.Usuario))
                    sb.Append("<div class='timeline-detalle'><strong>Usuario responsable:</strong> " + Safe(h.Usuario) + "</div>");

                sb.Append("</div>");
            }

            sb.Append("</div>");
            litTimeline.Text = sb.ToString();
        }

        private string Safe(string texto)
        {
            return Server.HtmlEncode(texto ?? string.Empty);
        }
    }
}