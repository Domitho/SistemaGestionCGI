using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;

namespace SistemaGestionCGI
{
    public partial class Dashboard : System.Web.UI.Page
    {
        private readonly ManejadorDashboard _bll = new ManejadorDashboard();

        // JSON para gráficos
        public string JsonProyectos { get; set; } = "[]";
        public string JsonDocentes { get; set; } = "[]";

        // JSON para detalle completo de docentes por categoría (para modal)
        public string JsonDocentesDetalle { get; set; } = "[]";

        // JSON para detalle completo de proyectos (para modal)
        public string JsonProyectosDetalle { get; set; } = "[]";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Validar sesión
                if (Session["UsuarioLogueado"] == null)
                {
                    Response.Redirect("Login.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                CargarDashboard();
            }
        }

        private void CargarDashboard()
        {
            try
            {
                // 1. KPI Cards
                var kpis = _bll.ObtenerContadoresGenerales();

                lblCentros.Text = kpis.TotalCentros.ToString();
                lblIntegrantesCentros.Text = $"{kpis.TotalIntegrantesCentros} Integrantes";

                lblConvocatorias.Text = kpis.TotalConvocatorias.ToString();

                lblGrupos.Text = kpis.TotalGrupos.ToString();
                lblIntegrantesGrupos.Text = $"{kpis.TotalIntegrantesGrupos} Integrantes";

                lblTotalDocentes.Text = kpis.TotalDocentes.ToString();

                // 2. Gráficos
                var proyectos = _bll.ObtenerProyectosPorEstado();
                var docentes = _bll.ObtenerDocentesPorCategoria();

                JsonProyectos = JsonConvert.SerializeObject(proyectos);
                JsonDocentes = JsonConvert.SerializeObject(docentes);

                // 3. Detalle de docentes para modal
                var docentesDetalle = _bll.ObtenerDocentesPorCategoriaDetalleTodos();
                JsonDocentesDetalle = JsonConvert.SerializeObject(docentesDetalle);

                // 4. Detalle de proyectos para modal
                var proyectosDetalle = _bll.ObtenerProyectosDetalleTodos();
                JsonProyectosDetalle = JsonConvert.SerializeObject(proyectosDetalle);
            }
            catch (Exception ex)
            {
                // Logging mínimo; se puede mejorar con NLog, Serilog, etc.
                Console.WriteLine("Error Dashboard: " + ex.Message);
            }
        }
    }
}