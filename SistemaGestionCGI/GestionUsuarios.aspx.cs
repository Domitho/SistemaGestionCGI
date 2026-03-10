using System;
using System.Linq;
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
                if (Session["UsuarioLogueado"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                // Seguridad: Si es Coordinador, lo mandamos a su panel, no puede estar aquí.
                if (Session["RolUsuario"]?.ToString() == "COORDINADOR")
                {
                    Response.Redirect("EjecucionProAprobados.aspx");
                    return;
                }

                CargarUsuarios();
            }
        }

        private void CargarUsuarios(string filtro = "")
        {
            var lista = _manejador.ObtenerUsuarios();

            if (!string.IsNullOrEmpty(filtro))
            {
                lista = lista.Where(u => u.strNombre_usu.ToLower().Contains(filtro.ToLower())).ToList();
            }

            rptUsuarios.DataSource = lista;
            rptUsuarios.DataBind();

            pnlNoData.Visible = lista.Count == 0;
            rptUsuarios.Visible = lista.Count > 0;
        }

        protected void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            CargarUsuarios(txtBuscar.Text.Trim());
        }

        // --- NAVEGACIÓN ---

        private void MostrarFormulario(bool mostrar)
        {
            pnlGrilla.Visible = !mostrar;
            pnlFormulario.Visible = mostrar;
            btnNuevoUsuario.Visible = !mostrar;
        }

        protected void btnNuevoUsuario_Click(object sender, EventArgs e)
        {
            hfIdUsuario.Value = "";
            txtUsername.Text = "";
            txtPassword.Text = "";
            ddlRol.SelectedIndex = 0;
            chkActivo.Checked = true;

            pnlSeleccionCoordinador.Visible = false;
            txtUsername.ReadOnly = false;
            ViewState["CedulaVinculada"] = null;

            lblTituloFormulario.Text = "Crear Nuevo Usuario";
            lblInfoPass.Visible = false;

            MostrarFormulario(true);
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            MostrarFormulario(false);
        }

        // =========================================================
        // LÓGICA DE VINCULACIÓN (SOLO PARA COORDINADORES)
        // =========================================================

        protected void ddlRol_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlRol.SelectedValue == "COORDINADOR")
            {
                CargarCandidatosCoordinadores();
            }
            else
            {
                pnlSeleccionCoordinador.Visible = false;
                txtUsername.ReadOnly = false;
                txtUsername.Text = "";
                ViewState["CedulaVinculada"] = null;
            }
        }

        private void CargarCandidatosCoordinadores()
        {
            try
            {
                var lista = _manejador.ObtenerCoordinadoresPendientes();

                if (lista != null && lista.Count > 0)
                {
                    ddlCandidatos.DataSource = lista;
                    ddlCandidatos.DataTextField = "strNombre_usu";
                    ddlCandidatos.DataValueField = "strCedula_ref";
                    ddlCandidatos.DataBind();

                    ddlCandidatos.Items.Insert(0, new ListItem("-- Seleccione Coordinador --", ""));
                    pnlSeleccionCoordinador.Visible = true;
                }
                else
                {
                    pnlSeleccionCoordinador.Visible = false;
                    Msg("No hay coordinadores pendientes de usuario.", "ww");
                }
            }
            catch (Exception ex)
            {
                Msg("Error cargando candidatos: " + ex.Message, "ee");
            }
        }

        protected void ddlCandidatos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCandidatos.SelectedIndex > 0)
            {
                string cedula = ddlCandidatos.SelectedValue;
                ViewState["CedulaVinculada"] = cedula;

                // Auto-rellenar nombre
                string textoCombo = ddlCandidatos.SelectedItem.Text;
                string nombreCompleto = textoCombo.Split('(')[0].Trim();
                txtUsername.Text = nombreCompleto.Replace(" ", ".");
            }
            else
            {
                ViewState["CedulaVinculada"] = null;
                txtUsername.Text = "";
            }
        }

        // =========================================================
        // CRUD LOGIC
        // =========================================================

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
                    txtPassword.Text = "";

                    if (ddlRol.Items.FindByValue(u.strRol_usu) != null)
                        ddlRol.SelectedValue = u.strRol_usu;

                    chkActivo.Checked = u.bActivo_usu;

                    // Al editar, recuperamos la cédula existente en memoria
                    ViewState["CedulaVinculada"] = u.strCedula_ref;

                    // No permitimos cambiar la vinculación al editar para evitar inconsistencias
                    pnlSeleccionCoordinador.Visible = false;

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

                string cedulaFinal = ViewState["CedulaVinculada"] as string;

                if (ddlRol.SelectedValue != "COORDINADOR")
                {
                    cedulaFinal = null;
                }

                var u = new InvgccUsuario
                {
                    strNombre_usu = txtUsername.Text.Trim(),
                    strRol_usu = ddlRol.SelectedValue,
                    bActivo_usu = chkActivo.Checked,
                    strClave_usu = txtPassword.Text.Trim(),
                    strCedula_ref = cedulaFinal
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

        public string ObtenerIniciales(string nombre)
        {
            if (string.IsNullOrEmpty(nombre)) return "U";
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