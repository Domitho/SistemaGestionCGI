using Newtonsoft.Json;
using System;
using SistemaGestionCGI.BLL;

namespace SistemaGestionCGI
{
    public partial class Dashboard : System.Web.UI.Page
    {
        private readonly ManejadorDashboard _bll = new ManejadorDashboard();

        // Variables públicas para JS
        public string JsonProyectos { get; set; } = "[]";
        public string JsonDocentes { get; set; } = "[]";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UsuarioLogueado"] == null) Response.Redirect("Login.aspx");
                CargarDashboard();
            }
        }

        private void CargarDashboard()
        {
            try
            {
                // 1. Cargar KPIs (Tarjetas)
                var kpis = _bll.ObtenerContadoresGenerales();

                // Formato combinado: "5 Centros (12 Integrantes)"
                lblCentros.Text = $"{kpis.TotalCentros}";
                lblIntegrantesCentros.Text = $"{kpis.TotalIntegrantesCentros} Integrantes"; // Nuevo Label

                lblConvocatorias.Text = kpis.TotalConvocatorias.ToString();

                lblGrupos.Text = $"{kpis.TotalGrupos}";
                lblIntegrantesGrupos.Text = $"{kpis.TotalIntegrantesGrupos} Integrantes"; // Nuevo Label

                // Tarjeta Extra: Total Docentes
                lblTotalDocentes.Text = kpis.TotalDocentes.ToString();

                // 2. Cargar Gráficos (JSON)
                var proyectos = _bll.ObtenerProyectosPorEstado();
                var docentes = _bll.ObtenerDocentesPorCategoria();

                JsonProyectos = JsonConvert.SerializeObject(proyectos);
                JsonDocentes = JsonConvert.SerializeObject(docentes);
            }
            catch (Exception ex)
            {
                // Loguear error
                Console.WriteLine("Error Dashboard: " + ex.Message);
            }
        }
    }
}