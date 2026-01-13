using System;
using System.Linq; // Necesario para el buscador
using System.Web.UI;
using System.Web.UI.WebControls;
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;

namespace SistemaGestionCGI
{
    public partial class GestionUsuarios : System.Web.UI.Page
    {
        private readonly ManejadorUsuarios _manejador = new ManejadorUsuarios();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarUsuarios();
            }
        }

        private void CargarUsuarios(string filtro = "")
        {
            var lista = _manejador.ObtenerUsuarios();

            // Lógica simple de búsqueda
            if (!string.IsNullOrEmpty(filtro))
            {
                lista = lista.Where(u => u.strNombre_usu.ToLower().Contains(filtro.ToLower())).ToList();
            }

            rptUsuarios.DataSource = lista;
            rptUsuarios.DataBind();

            // Mostrar panel "No Data" si está vacío
            pnlNoData.Visible = lista.Count == 0;
            rptUsuarios.Visible = lista.Count > 0;
        }

        protected void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            CargarUsuarios(txtBuscar.Text.Trim());
        }

        // --- NAVEGACIÓN SIMPLE (VISIBLE TRUE/FALSE) ---

        private void MostrarFormulario(bool mostrar)
        {
            pnlGrilla.Visible = !mostrar;
            pnlFormulario.Visible = mostrar;

            // Botón nuevo arriba a la derecha
            btnNuevoUsuario.Visible = !mostrar;
        }

        protected void btnNuevoUsuario_Click(object sender, EventArgs e)
        {
            hfIdUsuario.Value = "";
            txtUsername.Text = "";
            txtPassword.Text = "";
            ddlRol.SelectedIndex = 0;
            chkActivo.Checked = true;

            lblTituloFormulario.Text = "Crear Nuevo Usuario";
            lblInfoPass.Visible = false;

            MostrarFormulario(true);
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            MostrarFormulario(false);
        }

        // --- CRUD LOGIC ---

        protected void rptUsuarios_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "Editar")
            {
                var u = _manejador.ObtenerUsuarioPorId(id);
                if (u != null)
                {
                    hfIdUsuario.Value = u.intId_usu.ToString();
                    txtUsername.Text = u.strNombre_usu;
                    txtPassword.Text = ""; // Seguridad

                    if (ddlRol.Items.FindByValue(u.strRol_usu) != null)
                        ddlRol.SelectedValue = u.strRol_usu;

                    chkActivo.Checked = u.bActivo_usu;

                    lblTituloFormulario.Text = "Editar: " + u.strNombre_usu;
                    lblInfoPass.Visible = true;

                    MostrarFormulario(true);
                }
            }
            else if (e.CommandName == "Eliminar")
            {
                _manejador.EliminarUsuario(id);
                Msg("Usuario desactivado.", "ss");
                CargarUsuarios(txtBuscar.Text);
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrEmpty(ddlRol.SelectedValue))
                {
                    Msg("Complete todos los campos obligatorios.", "ww");
                    return;
                }

                var u = new InvgccUsuario
                {
                    strNombre_usu = txtUsername.Text.Trim(),
                    strRol_usu = ddlRol.SelectedValue,
                    bActivo_usu = chkActivo.Checked,
                    strClave_usu = txtPassword.Text.Trim()
                };

                if (string.IsNullOrEmpty(hfIdUsuario.Value))
                {
                    if (string.IsNullOrEmpty(u.strClave_usu)) { Msg("Ingrese contraseña.", "ww"); return; }

                    _manejador.GuardarUsuario(u);
                    Msg("Usuario creado exitosamente.", "ss");
                }
                else
                {
                    u.intId_usu = int.Parse(hfIdUsuario.Value);
                    if (string.IsNullOrEmpty(u.strClave_usu))
                    {
                        var old = _manejador.ObtenerUsuarioPorId(u.intId_usu);
                        u.strClave_usu = old.strClave_usu;
                    }

                    _manejador.ActualizarUsuario(u);
                    Msg("Usuario actualizado.", "ss");
                }

                MostrarFormulario(false);
                CargarUsuarios(txtBuscar.Text);
            }
            catch (Exception ex) { Msg("Error: " + ex.Message, "ee"); }
        }

        // --- HELPER PARA AVATAR ---
        public string ObtenerIniciales(string nombre)
        {
            if (string.IsNullOrEmpty(nombre)) return "U";
            // Toma la primera letra en mayúscula
            return nombre.Substring(0, 1).ToUpper();
        }

        private void Msg(string msg, string type)
        {
            string cleanMsg = msg.Replace("'", "\\'").Replace("\r\n", " ");
            ScriptManager.RegisterStartupScript(this, GetType(), "alert",
                $"$(function() {{ toastify('{type}', '{cleanMsg}', 'Sistema'); }});", true);
        }
    }
}