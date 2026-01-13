using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// ⚠️ IMPORTANTE: Namespace actualizado al nuevo proyecto
namespace SistemaGestionCGI
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ValidarSesion();
                CargarDatosUsuario();

                AplicarPermisosMenu();
            }
        }

        private void ValidarSesion()
        {
            if (Session["UsuarioLogueado"] == null)
            {
                Response.Redirect("Login.aspx");
            }
        }

        private void CargarDatosUsuario()
        {
            if (Session["UserName"] != null)
            {
                lblNombre.Text = Session["UserName"].ToString().ToUpper();
            }
            else
            {
                lblNombre.Text = "USUARIO";
            }

            if (lblFecha != null)
            {
                lblFecha.Text = DateTime.Now.Year.ToString();
            }
        }

        private void AplicarPermisosMenu()
        {
            string rol = Session["RolUsuario"]?.ToString() ?? "";

            // 1. PRIMERO: Ocultamos TODO por defecto (Estrategia de Seguridad "Deny All")
            // Esto evita que si agregas un botón nuevo, se te olvide ocultarlo.
            bool esAdmin = (rol == "ADMINISTRADOR");
            bool esCoord = (rol == "COORDINADOR");

            // Lógica para COORDINADOR
            if (esCoord)
            {
                // Ocultar todo lo que NO es suyo
                if (lnkMenuDashboard != null) lnkMenuDashboard.Visible = false;
                if (lnkMenuUsuarios != null) lnkMenuUsuarios.Visible = false;
                if (lnkMenuConvocatorias != null) lnkMenuConvocatorias.Visible = false;
                if (lnkMenuGrupos != null) lnkMenuGrupos.Visible = false;
                if (lnkMenuCentros != null) lnkMenuCentros.Visible = false;
                if (lnkMenuCategorizacion != null) lnkMenuCategorizacion.Visible = false;
                if (lnkMenuCalificacion != null) lnkMenuCalificacion.Visible = false;

                // CORRECCIÓN: El ID en el HTML es 'lnkMenuInscripcion', no 'lnkMenuProyectos'
                if (lnkMenuInscripcion != null) lnkMenuInscripcion.Visible = false;

                // Mostrar SU módulo
                if (lnkMenuEjecucion != null) lnkMenuEjecucion.Visible = true;
            }
            // Lógica para ADMINISTRADOR
            else if (esAdmin)
            {
                // El Admin ve TODO
                if (lnkMenuDashboard != null) lnkMenuDashboard.Visible = true;
                if (lnkMenuUsuarios != null) lnkMenuUsuarios.Visible = true;
                if (lnkMenuConvocatorias != null) lnkMenuConvocatorias.Visible = true;
                if (lnkMenuInscripcion != null) lnkMenuInscripcion.Visible = true; // ID Corregido
                if (lnkMenuGrupos != null) lnkMenuGrupos.Visible = true;
                if (lnkMenuCentros != null) lnkMenuCentros.Visible = true;
                if (lnkMenuCategorizacion != null) lnkMenuCategorizacion.Visible = true;
                if (lnkMenuCalificacion != null) lnkMenuCalificacion.Visible = true;
                if (lnkMenuEjecucion != null) lnkMenuEjecucion.Visible = true;
            }
            // Lógica para DESCONOCIDOS / SIN ROL (Seguridad)
            else
            {
                // Ocultar todo
                if (lnkMenuDashboard != null) lnkMenuDashboard.Visible = false;
                if (lnkMenuUsuarios != null) lnkMenuUsuarios.Visible = false;
                if (lnkMenuConvocatorias != null) lnkMenuConvocatorias.Visible = false;
                if (lnkMenuInscripcion != null) lnkMenuInscripcion.Visible = false;
                if (lnkMenuGrupos != null) lnkMenuGrupos.Visible = false;
                if (lnkMenuCentros != null) lnkMenuCentros.Visible = false;
                if (lnkMenuCategorizacion != null) lnkMenuCategorizacion.Visible = false;
                if (lnkMenuCalificacion != null) lnkMenuCalificacion.Visible = false;
                if (lnkMenuEjecucion != null) lnkMenuEjecucion.Visible = false;
            }
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            // 1. Limpiar variables de sesión
            Session.Clear();

            // 2. Abandonar la sesión actual
            Session.Abandon();

            // 3. Limpiar cookie de autenticación (si usas FormsAuthentication)
            System.Web.Security.FormsAuthentication.SignOut();

            // 4. Redirigir al Login evitando que el usuario use el botón "Atrás"
            Response.Redirect("Login.aspx");
        }
    }
}