using System;
using System.Collections.Generic;
using System.Web.UI;
using Newtonsoft.Json; // Necesario para convertir listas C# a JSON para JavaScript
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;

namespace SistemaGestionCGI
{
    public partial class Dashboard : System.Web.UI.Page
    {
        // Instancia de la Capa de Negocio
        private readonly ManejadorDashboard _bll = new ManejadorDashboard();

        public string JsonCategorias { get; set; } = "[]";
        public string JsonEstados { get; set; } = "[]";
        public string JsonPublicaciones { get; set; } = "[]";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 1. VALIDACIÓN DE SESIÓN ROBUSTA
                // Verifica si existe la sesión "legacy" o la "nueva". Si ambas son nulas, redirige.
                if (Session["Username"] == null && Session["UsuarioLogueado"] == null)
                {
                    Response.Redirect("Login.aspx", true);
                    return;
                }

                try
                {
                    // 2. CARGAR FECHA Y DATOS
                    lblFechaActual.Text = DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy");

                    CargarKPIs();
                    CargarDatosGraficos();
                }
                catch (Exception ex)
                {
                    // En producción, aquí deberías loguear el error.
                    // Para el dashboard, evitamos que un error rompa toda la página.
                    Console.WriteLine("Error en carga de Dashboard: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Carga los contadores numéricos de las tarjetas superiores
        /// </summary>
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
                // Dejamos los labels en "0" (valor por defecto en el aspx)
            }
        }

        private void CargarDatosGraficos()
        {
            try
            {
                // 1. Obtener datos crudos (Listas de Objetos C#)
                var listaCategorias = _bll.ObtenerDocentesPorCategoria();
                var listaEstados = _bll.ObtenerProyectosPorEstado();
                var listaPublicaciones = _bll.ObtenerPublicacionesPorTipo();

                // 2. Serializar a JSON (Texto) para que JS lo entienda
                // Si la lista es nula, asignamos un array vacío "[]"
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
                // Si falla, las variables quedan con "[]" por defecto para no romper el JS
            }
        }
    }
}