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
            }
        }

        private void ValidarSesion()
        {
            // Si la sesión no existe, redirigir al Login inmediatamente.
            // Esto protege todas las páginas que usen esta MasterPage.
            if (Session["UsuarioLogueado"] == null)
            {
                Response.Redirect("Login.aspx");
            }
        }

        private void CargarDatosUsuario()
        {
            // Recuperar el nombre del usuario desde la variable de sesión
            if (Session["UserName"] != null)
            {
                // Muestra el nombre en mayúsculas para mejor diseño
                lblNombre.Text = Session["UserName"].ToString().ToUpper();
            }
            else
            {
                lblNombre.Text = "USUARIO";
            }

            // Actualizar la fecha en el footer automáticamente
            if (lblFecha != null)
            {
                lblFecha.Text = DateTime.Now.Year.ToString();
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