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
                    // VALIDACIÓN DE SEGURIDAD:
                    // Si el mapeo JsonProperty falló, el objeto existe pero las propiedades están vacías.
                    if (string.IsNullOrEmpty(usuarioLogueado.strNombre_usu))
                    {
                        Msg("Error técnico: El usuario existe pero los datos no se mapearon correctamente.", "ee");
                        return;
                    }

                    // 1. LLENAR SESIÓN
                    Session["UsuarioLogueado"] = usuarioLogueado.strNombre_usu;

                    // Manejo seguro del Rol (evita NullReference)
                    string rol = (usuarioLogueado.strRol_usu ?? "").Trim().ToUpper();
                    Session["RolUsuario"] = rol;

                    Session["UserId"] = usuarioLogueado.intId_usu;

                    // ===========================================
                    // LA LÍNEA NUEVA QUE NECESITAMOS
                    // ===========================================
                    Session["CedulaUsuario"] = usuarioLogueado.strCedula_ref ?? "";
                    // ===========================================

                    // 2. MENSAJES
                    if (rol == "ADMINISTRADOR" || rol == "COORDINADOR")
                    {
                        Session["TempMsg"] = "Bienvenido";
                        Session["TempTipo"] = "welcome";
                    }

                    // 3. REDIRECCIÓN
                    if (rol == "COORDINADOR")
                    {
                        Response.Redirect("EjecucionProAprobados.aspx", false);
                    }
                    else
                    {
                        Response.Redirect("Dashboard.aspx", false);
                    }

                    // Asegura que termine la petición
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