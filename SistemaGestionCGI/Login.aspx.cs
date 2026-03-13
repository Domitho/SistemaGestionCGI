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
                    if (string.IsNullOrEmpty(usuarioLogueado.strNombre_usu))
                    {
                        Msg("Error técnico: El usuario existe pero los datos no se mapearon correctamente.", "ee");
                        return;
                    }

                    Session["UsuarioLogueado"] = usuarioLogueado.strNombre_usu;

                    string rol = (usuarioLogueado.strRol_usu ?? "").Trim().ToUpper();
                    Session["RolUsuario"] = rol;

                    Session["IdUsuario"] = usuarioLogueado.intId_usu;

                    Session["CedulaUsuario"] = usuarioLogueado.strCedula_ref ?? "";

                    if (rol == "ADMINISTRADOR" || rol == "COORDINADOR")
                    {
                        Session["TempMsg"] = "Bienvenido";
                        Session["TempTipo"] = "welcome";
                    }

                    if (rol == "COORDINADOR")
                    {
                        Response.Redirect("ProyectosAprobadosCoordinadores.aspx", false);
                    }
                    else
                    {
                        Response.Redirect("Dashboard.aspx", false);
                    }

                    Context.ApplicationInstance.CompleteRequest();
                }
                else
                {
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
            string cleanMsg = msg.Replace("'", "\\'");
            string titulo = "Notificación";
            string script = $"$(function() {{ toastify('{tipo}', '{cleanMsg}', '{titulo}'); }});";
            ScriptManager.RegisterStartupScript(this, GetType(), "ToastrNotification", script, true);
        }
    }
}