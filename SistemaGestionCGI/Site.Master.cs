using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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
                lblNombre.Text = Session["UserName"].ToString().ToUpper();
            else
                lblNombre.Text = "USUARIO";

            // Validación de null para evitar errores si el footer no carga
            if (lblFecha != null)
                lblFecha.Text = DateTime.Now.Year.ToString();
        }

        private void AplicarPermisosMenu()
        {
            string rol = Session["RolUsuario"]?.ToString() ?? "";

            bool esAdmin = (rol == "ADMINISTRADOR");
            bool esCoord = (rol == "COORDINADOR");

            // 1. REINICIO: Ocultamos los Items individuales
            OcultarTodo();

            // 2. APLICAR LÓGICA SEGÚN ROL
            if (esAdmin)
            {
                // El Admin ve TODO
                MostrarTodo();
            }
            else if (esCoord)
            {
                // El coordinador SOLO ve Ejecución
                if (lnkMenuEjecucion != null) lnkMenuEjecucion.Visible = true;

                // IMPORTANTE: Aseguramos que el Dropdown PADRE de proyectos sea visible
                if (liDropdownProyectos != null) liDropdownProyectos.Visible = true;

                // Ocultamos el Dropdown PADRE de grupos porque no tiene acceso a nada dentro
                if (liDropdownGrupos != null) liDropdownGrupos.Visible = false;
            }
            else
            {
                // Rol desconocido: Se queda todo oculto (seguridad)
                if (liDropdownGrupos != null) liDropdownGrupos.Visible = false;
                if (liDropdownProyectos != null) liDropdownProyectos.Visible = false;
            }
        }

        private void OcultarTodo()
        {
            // Accesos directos
            if (lnkMenuDashboard != null) lnkMenuDashboard.Visible = false;
            if (lnkMenuUsuarios != null) lnkMenuUsuarios.Visible = false;

            // Items de Grupos
            if (lnkMenuConvocatorias != null) lnkMenuConvocatorias.Visible = false;
            if (lnkMenuCentros != null) lnkMenuCentros.Visible = false;
            if (lnkMenuGrupos != null) lnkMenuGrupos.Visible = false;
            if (lnkMenuCalificacion != null) lnkMenuCalificacion.Visible = false;
            if (lnkMenuCategorizacion != null) lnkMenuCategorizacion.Visible = false;

            // Items de Proyectos
            if (lnkMenuInscripcion != null) lnkMenuInscripcion.Visible = false;
            if (lnkMenuEjecucion != null) lnkMenuEjecucion.Visible = false;
        }

        private void MostrarTodo()
        {
            // Accesos directos
            if (lnkMenuDashboard != null) lnkMenuDashboard.Visible = true;
            if (lnkMenuUsuarios != null) lnkMenuUsuarios.Visible = true;

            // Parents (Dropdowns headers)
            if (liDropdownGrupos != null) liDropdownGrupos.Visible = true;
            if (liDropdownProyectos != null) liDropdownProyectos.Visible = true;

            // Items de Grupos
            if (lnkMenuConvocatorias != null) lnkMenuConvocatorias.Visible = true;
            if (lnkMenuCentros != null) lnkMenuCentros.Visible = true;
            if (lnkMenuGrupos != null) lnkMenuGrupos.Visible = true;
            if (lnkMenuCalificacion != null) lnkMenuCalificacion.Visible = true;
            if (lnkMenuCategorizacion != null) lnkMenuCategorizacion.Visible = true;

            // Items de Proyectos
            if (lnkMenuInscripcion != null) lnkMenuInscripcion.Visible = true;
            if (lnkMenuEjecucion != null) lnkMenuEjecucion.Visible = true;
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            System.Web.Security.FormsAuthentication.SignOut();
            Response.Redirect("Login.aspx");
        }
    }
}