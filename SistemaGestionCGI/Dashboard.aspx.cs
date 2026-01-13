using System;
using System.Collections.Generic;
using System.Web.UI;
using Newtonsoft.Json; 
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;

namespace SistemaGestionCGI
{
    public partial class Dashboard : System.Web.UI.Page
    {
        // Instancia
        private readonly ManejadorDashboard _bll = new ManejadorDashboard();

        public string JsonCategorias { get; set; } = "[]";
        public string JsonEstados { get; set; } = "[]";
        public string JsonPublicaciones { get; set; } = "[]";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["Username"] == null && Session["UsuarioLogueado"] == null)
                {
                    Response.Redirect("Login.aspx", true);
                    return;
                }

                try
                {
                    lblFechaActual.Text = DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy");

                    CargarKPIs();
                    CargarDatosGraficos();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error en carga de Dashboard: " + ex.Message);
                }
            }
        }

        private void CargarKPIs()
        {
            try
            {
                var kpis = _bll.ObtenerKPIs();

                lblCentros.Text = kpis.Centros.ToString();
                lblConvocatorias.Text = kpis.Convocatorias.ToString();
                lblGruInv.Text = kpis.Grupos.ToString();
                lblIntegrantes.Text = kpis.Integrantes.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error cargando KPIs: " + ex.Message);
            }
        }

        private void CargarDatosGraficos()
        {
            try
            {
                var listaCategorias = _bll.ObtenerDocentesPorCategoria();
                var listaEstados = _bll.ObtenerProyectosPorEstado();
                var listaPublicaciones = _bll.ObtenerPublicacionesPorTipo();

                JsonCategorias = listaCategorias != null
                    ? JsonConvert.SerializeObject(listaCategorias)
                    : "[]";

                JsonEstados = listaEstados != null
                    ? JsonConvert.SerializeObject(listaEstados)
                    : "[]";

                JsonPublicaciones = listaPublicaciones != null
                    ? JsonConvert.SerializeObject(listaPublicaciones)
                    : "[]";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error generando JSON para gráficos: " + ex.Message);
            }
        }
    }
}