using System;
using System.Web.UI;
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;

namespace SistemaGestionCGI
{
    public partial class Login : System.Web.UI.Page
    {
        private readonly ManejadorUsuarios _manejador = new ManejadorUsuarios();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UsuarioLogueado"] != null)
                {
                    Response.Redirect("Dashboard.aspx");
                }

                if (Request.QueryString["error"] == "1")
                {
                    Msg("Usuario o contraseña incorrectos.", "ee");
                }
            }
        }

        protected void LoginButton_Click2(object sender, EventArgs e)
        {
            try
            {
                string user = UserName.Text.Trim();
                string pass = Password.Text.Trim();

                InvgccUsuario usuarioLogueado = _manejador.Autenticar(user, pass);

                if (usuarioLogueado != null)
                {
                    // === LOGIN EXITOSO ===
                    Session["UsuarioLogueado"] = usuarioLogueado;

                    Session["UserId"] = usuarioLogueado.intId_usu;
                    Session["UserName"] = usuarioLogueado.strNombre_usu;
                    Session["UserRole"] = usuarioLogueado.strRol_usu;

                    Response.Redirect("Dashboard.aspx", false);
                }
                else
                {
                    // === LOGIN FALLIDO ===
                    Msg("Usuario incorrecto, contraseña inválida o cuenta inactiva.", "ee");
                }
            }
            catch (Exception ex)
            {
                Msg("Error de conexión: " + ex.Message, "ee");
            }
        }

        private void Msg(string msg, string tipo)
        {
            // Limpiamos el mensaje de comillas simples para evitar romper el JS
            string cleanMsg = msg.Replace("'", "\\'");
            string titulo = "Notificación";

            // Envolvemos en $(function(){ ... }) para asegurar que JQuery esté cargado
            string script = $"$(function() {{ toastify('{tipo}', '{cleanMsg}', '{titulo}'); }});";

            ScriptManager.RegisterStartupScript(this, GetType(), "ToastrNotification", script, true);

            // Respaldo visual
            // LblMsg.Text = msg;
        }
    }
}