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

        public string JsonProyectos { get; set; } = "[]";
        public string JsonDocentes { get; set; } = "[]";

        public string JsonDocentesDetalle { get; set; } = "[]";

        public string JsonProyectosDetalle { get; set; } = "[]";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UsuarioLogueado"] == null)
                {
                    Response.Redirect("Login.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                lblFechaActual.Text = DateTime.Now.ToString("dddd, dd 'de' MMMM 'de' yyyy");
                CargarDashboard();
            }
        }

        private void CargarDashboard()
        {
            try
            {
                var kpis = _bll.ObtenerContadoresGenerales();

                lblCentros.Text = kpis.TotalCentros.ToString();
                lblIntegrantesCentros.Text = $"{kpis.TotalIntegrantesCentros} Integrantes";

                lblConvocatorias.Text = kpis.TotalConvocatorias.ToString();

                lblGrupos.Text = kpis.TotalGrupos.ToString();
                lblIntegrantesGrupos.Text = $"{kpis.TotalIntegrantesGrupos} Integrantes";

                lblTotalDocentes.Text = kpis.TotalDocentes.ToString();

                var proyectos = _bll.ObtenerProyectosPorEstado();
                var docentes = _bll.ObtenerDocentesPorCategoria();

                JsonProyectos = JsonConvert.SerializeObject(proyectos);
                JsonDocentes = JsonConvert.SerializeObject(docentes);

                var docentesDetalle = _bll.ObtenerDocentesPorCategoriaDetalleTodos();
                JsonDocentesDetalle = JsonConvert.SerializeObject(docentesDetalle);

                var proyectosDetalle = _bll.ObtenerProyectosDetalleTodos();
                JsonProyectosDetalle = JsonConvert.SerializeObject(proyectosDetalle);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Dashboard: " + ex.Message);
            }
        }
    }
}