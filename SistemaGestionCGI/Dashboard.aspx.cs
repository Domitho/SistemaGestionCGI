using Newtonsoft.Json;
using System;
using SistemaGestionCGI.BLL;

namespace SistemaGestionCGI
{
    public partial class Dashboard : System.Web.UI.Page
    {
        private readonly ManejadorDashboard _bll = new ManejadorDashboard();

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
                var kpis = _bll.ObtenerContadoresGenerales();

                lblCentros.Text = $"{kpis.TotalCentros}";
                lblIntegrantesCentros.Text = $"{kpis.TotalIntegrantesCentros} Integrantes";

                lblConvocatorias.Text = kpis.TotalConvocatorias.ToString();

                lblGrupos.Text = $"{kpis.TotalGrupos}";
                lblIntegrantesGrupos.Text = $"{kpis.TotalIntegrantesGrupos} Integrantes"; 

                lblTotalDocentes.Text = kpis.TotalDocentes.ToString();

                var proyectos = _bll.ObtenerProyectosPorEstado();
                var docentes = _bll.ObtenerDocentesPorCategoria();

                JsonProyectos = JsonConvert.SerializeObject(proyectos);
                JsonDocentes = JsonConvert.SerializeObject(docentes);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Dashboard: " + ex.Message);
            }
        }
    }
}