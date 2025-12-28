using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;

namespace SistemaGestionCGI
{
    public partial class CentrosInvestigacion : System.Web.UI.Page
    {
        private readonly ManejadorCentroInvestigacion _manejador = new ManejadorCentroInvestigacion();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            // Validación de sesión
            if (Session["UsuarioLogueado"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }
            CargarDatos();
            CargarCombos();

            if (Session["TempMsg"] != null)
            {
                Msg(Session["TempMsg"].ToString(), Session["TempTipo"].ToString());
                Session["TempMsg"] = null;
                Session["TempTipo"] = null;
            }

            VerificarModalPendiente();
        }

        private void CargarDatos()
        {
            try
            {
                rptCentros.DataSource = _manejador.ObtenerTodos();
                rptCentros.DataBind();
            }
            catch (Exception ex) { Msg("Error al cargar tabla: " + ex.Message, "ee"); }
        }

        private void CargarCombos()
        {
            try
            {
                var directores = _manejador.ObtenerCandidatosDirector();
                ddlDirector.DataSource = directores;
                ddlDirector.DataTextField = "NombreCompleto";
                ddlDirector.DataValueField = "strId_int";
                ddlDirector.DataBind();
                ddlDirector.Items.Insert(0, new ListItem("-- Seleccione Director --", ""));
            }
            catch (Exception ex) { Msg("Error al cargar directores: " + ex.Message, "ee"); }
        }

        // ========================
        // GESTIÓN DE PANELES
        // ========================
        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            lblTituloFormulario.Text = "Registrar Nuevo Centro"; 

            pnlGrilla.Visible = false;
            pnlFormulario.Visible = true;
            btnNuevo.Visible = false;
            btnRegresar.Visible = true;
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            pnlGrilla.Visible = true;
            pnlFormulario.Visible = false;
            btnNuevo.Visible = true;
            btnRegresar.Visible = false;
        }

        // ========================
        // ACCIONES CRUD (CREATE / UPDATE / DELETE)
        // ========================
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    Msg("El nombre del centro es obligatorio.", "ww");
                    return;
                }

                var centro = new InvgccCentroInvestigacion
                {
                    strNombre_cen = txtNombre.Text.Trim(),
                    strFacultad_cen = ddlFacultad.SelectedValue,
                    strArea_cen = txtArea.Text.Trim(),
                    strUbicacion_cen = txtUbicacion.Text.Trim(),
                    strLineaInv_cen = txtLineas.Text.Trim(),
                    strMision_cen = txtMision.Text.Trim(),
                    strVision_cen = txtVision.Text.Trim(),
                    fkId_director = ddlDirector.SelectedValue,
                    dtFechaAprobacion_cen = !string.IsNullOrEmpty(txtFechaAprobacion.Text)
                                            ? DateTime.Parse(txtFechaAprobacion.Text)
                                            : DateTime.Now
                };

                if (string.IsNullOrEmpty(hfIdCentro.Value))
                {
                    _manejador.Guardar(centro);
                    Redireccionar("Centro registrado correctamente.", "ss");
                }
                else
                {
                    centro.strId_cen = hfIdCentro.Value;
                    _manejador.Actualizar(centro);
                    Redireccionar("Centro actualizado correctamente.", "ss");
                }
            }
            catch (Exception ex) { Msg("Error al guardar: " + ex.Message, "ee"); }
        }

        protected void rptCentros_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                string id = e.CommandArgument.ToString();

                if (e.CommandName == "eliminar")
                {
                    _manejador.Eliminar(id);
                    Redireccionar("Centro eliminado correctamente.", "ss");
                }
                else if (e.CommandName == "editar")
                {
                    CargarEdicion(id);
                }
                else if (e.CommandName == "verIntegrantes")
                {
                    Session["ModalCentroId"] = id;
                    Response.Redirect("CentrosInvestigacion.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
            }
            catch (Exception ex) { Msg("Error en operación: " + ex.Message, "ee"); }
        }

        // ========================
        // LÓGICA DE CARGA Y MODAL
        // ========================
        private void CargarEdicion(string id)
        {
            var centro = _manejador.ObtenerPorId(id);
            if (centro == null) return;

            lblTituloFormulario.Text = $"Editar Centro: {centro.strId_cen}";

            hfIdCentro.Value = centro.strId_cen;
            txtNombre.Text = centro.strNombre_cen;
            txtArea.Text = centro.strArea_cen;
            txtUbicacion.Text = centro.strUbicacion_cen;
            txtLineas.Text = centro.strLineaInv_cen;
            txtMision.Text = centro.strMision_cen;
            txtVision.Text = centro.strVision_cen;
            txtFechaAprobacion.Text = centro.dtFechaAprobacion_cen.ToString("yyyy-MM-dd");

            if (ddlFacultad.Items.FindByValue(centro.strFacultad_cen) != null)
                ddlFacultad.SelectedValue = centro.strFacultad_cen;

            if (ddlDirector.Items.FindByValue(centro.fkId_director) != null)
                ddlDirector.SelectedValue = centro.fkId_director;

            pnlGrilla.Visible = false;
            pnlFormulario.Visible = true;
            btnNuevo.Visible = false;
            btnRegresar.Visible = true;
        }

        private void MostrarModalIntegrantes(string idCentro)
        {
            var centro = _manejador.ObtenerPorId(idCentro);
            if (centro != null) lblCentroModal.Text = centro.strNombre_cen;

            var integrantes = _manejador.ObtenerIntegrantesPorCentro(idCentro);
            rptIntegrantesModal.DataSource = integrantes;
            rptIntegrantesModal.DataBind();

            ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalInt", "AbrirModalIntegrantes();", true);
        }

        private void LimpiarFormulario()
        {
            hfIdCentro.Value = "";
            txtNombre.Text = "";
            txtArea.Text = "";
            txtUbicacion.Text = "";
            txtLineas.Text = "";
            txtMision.Text = "";
            txtVision.Text = "";
            txtFechaAprobacion.Text = DateTime.Now.ToString("yyyy-MM-dd");
            ddlFacultad.SelectedIndex = 0;
            ddlDirector.SelectedIndex = 0;
        }

        private void VerificarModalPendiente()
        {
            if (Session["ModalCentroId"] != null)
            {
                string id = Session["ModalCentroId"].ToString();
                MostrarModalIntegrantes(id);
                Session["ModalCentroId"] = null;
            }
        }

        // ========================
        // REDIRECCIÓN Y MENSAJES
        // ========================
        private void Redireccionar(string msg, string type)
        {
            Session["TempMsg"] = msg;
            Session["TempTipo"] = type;

            Response.Redirect("CentrosInvestigacion.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void Msg(string msg, string type)
        {
            if (string.IsNullOrEmpty(msg)) return;
            string cleanMsg = msg.Replace("'", "").Replace("\r\n", " ").Replace("\n", " ");
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"$(function() {{ toastify('{type}', '{cleanMsg}', 'Sistema'); }});", true);
        }
    }
}