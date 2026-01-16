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

                if (Session["TempMsg"] != null)
                {
                    string msg = Session["TempMsg"].ToString();
                    string tipo = Session["TempTipo"]?.ToString() ?? "info";

                    MostrarNotificacion(msg, tipo);

                    Session["TempMsg"] = null;
                    Session["TempTipo"] = null;
                }
            }
        }

        private void MostrarNotificacion(string msg, string tipo)
        {
            string cleanMsg = msg.Replace("'", "").Replace("\r\n", "");
            string script = "";

            if (tipo == "welcome")
            {
                string nombre = Session["UsuarioLogueado"]?.ToString() ?? "Usuario";

                string rolRaw = Session["RolUsuario"]?.ToString() ?? "Sistema";
                string rol = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(rolRaw.ToLower());

                script = $"$(function() {{ mostrarBienvenida('{nombre}', '{rol}'); }});";
            }
            else
            {
                string toastType = "info";
                if (tipo == "ss") toastType = "success";
                if (tipo == "ee") toastType = "error";
                if (tipo == "ww") toastType = "warning";

                script = $"$(function() {{ toastify('{toastType}', '{cleanMsg}', 'Sistema UTC'); }});";
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "NotificacionGlobal", script, true);
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

            if (lblFecha != null)
                lblFecha.Text = DateTime.Now.Year.ToString();
        }

        private void AplicarPermisosMenu()
        {
            string rol = Session["RolUsuario"]?.ToString() ?? "";

            bool esAdmin = (rol == "ADMINISTRADOR");
            bool esCoord = (rol == "COORDINADOR");

            OcultarTodo();

            if (esAdmin)
            {
                MostrarTodo();
            }
            else if (esCoord)
            {
                if (lnkMenuEjecucion != null) lnkMenuEjecucion.Visible = true;
                if (liDropdownProyectos != null) liDropdownProyectos.Visible = true;
                if (liDropdownGrupos != null) liDropdownGrupos.Visible = false;
            }
            else
            {
                if (liDropdownGrupos != null) liDropdownGrupos.Visible = false;
                if (liDropdownProyectos != null) liDropdownProyectos.Visible = false;
            }
        }

        private void OcultarTodo()
        {
            if (lnkMenuDashboard != null) lnkMenuDashboard.Visible = false;
            if (lnkMenuUsuarios != null) lnkMenuUsuarios.Visible = false;

            if (lnkMenuConvocatorias != null) lnkMenuConvocatorias.Visible = false;
            if (lnkMenuCentros != null) lnkMenuCentros.Visible = false;
            if (lnkMenuGrupos != null) lnkMenuGrupos.Visible = false;
            if (lnkMenuCalificacion != null) lnkMenuCalificacion.Visible = false;
            if (lnkMenuCategorizacion != null) lnkMenuCategorizacion.Visible = false;

            if (lnkMenuInscripcion != null) lnkMenuInscripcion.Visible = false;
            if (lnkMenuEjecucion != null) lnkMenuEjecucion.Visible = false;
        }

        private void MostrarTodo()
        {
            if (lnkMenuDashboard != null) lnkMenuDashboard.Visible = true;
            if (lnkMenuUsuarios != null) lnkMenuUsuarios.Visible = true;

            if (liDropdownGrupos != null) liDropdownGrupos.Visible = true;
            if (liDropdownProyectos != null) liDropdownProyectos.Visible = true;

            if (lnkMenuConvocatorias != null) lnkMenuConvocatorias.Visible = true;
            if (lnkMenuCentros != null) lnkMenuCentros.Visible = true;
            if (lnkMenuGrupos != null) lnkMenuGrupos.Visible = true;
            if (lnkMenuCalificacion != null) lnkMenuCalificacion.Visible = true;
            if (lnkMenuCategorizacion != null) lnkMenuCategorizacion.Visible = true;

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