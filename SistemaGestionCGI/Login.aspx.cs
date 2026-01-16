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
                    Session["UsuarioLogueado"] = usuarioLogueado.strNombre_usu;
                    Session["RolUsuario"] = usuarioLogueado.strRol_usu;
                    Session["UserId"] = usuarioLogueado.intId_usu;

                    // Normalizamos el rol a mayúsculas para comparaciones seguras
                    string rol = usuarioLogueado.strRol_usu.Trim().ToUpper();

                    // 1. LÓGICA DE BIENVENIDA
                    if (rol == "ADMINISTRADOR" || rol == "COORDINADOR")
                    {
                        // Activamos la alerta de bienvenida para el Master Page
                        Session["TempMsg"] = "Bienvenido";
                        Session["TempTipo"] = "welcome";
                    }

                    // 2. REDIRECCIÓN SEGÚN ROL (Usamos la variable 'rol' normalizada)
                    if (rol == "COORDINADOR")
                    {
                        Response.Redirect("EjecucionProAprobados.aspx", false);
                    }
                    else
                    {
                        Response.Redirect("Dashboard.aspx", false);
                    }
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