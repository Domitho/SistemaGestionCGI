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
            if (Session["UsuarioLogueado"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            CargarCentros();

            // Mensajes Flash (Toastify)
            if (Session["TempMsg"] != null)
            {
                Msg(Session["TempMsg"].ToString(), Session["TempTipo"].ToString());
                Session["TempMsg"] = null;
                Session["TempTipo"] = null;
            }
        }

        // ==========================
        // VISTA 1 & 2: GESTIÓN DE CENTROS
        // ==========================
        private void CargarCentros()
        {
            try
            {
                rptCentros.DataSource = _manejador.ObtenerTodos();
                rptCentros.DataBind();
            }
            catch (Exception ex) { Msg("Error cargando centros: " + ex.Message, "ee"); }
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            CambiarVista(Vista.FormularioCentro);
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNombre.Text)) { Msg("Nombre obligatorio.", "ww"); return; }

                var centro = new InvgccCentroInvestigacion
                {
                    strNombre_cen = txtNombre.Text.Trim(),
                    strFacultad_cen = ddlFacultad.SelectedValue,
                    strArea_cen = txtArea.Text.Trim(),
                    strUbicacion_cen = txtUbicacion.Text.Trim(),
                    strLineaInv_cen = txtLineas.Text.Trim(),
                    strMision_cen = txtMision.Text.Trim(),
                    strVision_cen = txtVision.Text.Trim(),
                    dtFechaAprobacion_cen = string.IsNullOrEmpty(txtFechaAprobacion.Text) ? DateTime.Now : DateTime.Parse(txtFechaAprobacion.Text)
                };

                if (string.IsNullOrEmpty(hfIdCentro.Value))
                {
                    _manejador.Guardar(centro);
                    Redireccionar("Centro creado.", "ss");
                }
                else
                {
                    centro.strId_cen = hfIdCentro.Value;
                    _manejador.Actualizar(centro);
                    Redireccionar("Centro actualizado.", "ss");
                }
            }
            catch (Exception ex) { Msg("Error: " + ex.Message, "ee"); }
        }

        protected void rptCentros_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();

            if (e.CommandName == "Integrantes") // LÓGICA MOVIDA AQUI
            {
                hfIdCentro.Value = id;
                CargarIntegrantes(id);
                CambiarVista(Vista.ListaIntegrantes);
            }
            else if (e.CommandName == "Editar")
            {
                CargarEdicion(id);
            }
            else if (e.CommandName == "Eliminar")
            {
                _manejador.Eliminar(id);
                Redireccionar("Centro eliminado.", "ss");
            }
        }

        private void CargarEdicion(string id)
        {
            var c = _manejador.ObtenerPorId(id);
            if (c == null) return;

            hfIdCentro.Value = c.strId_cen;
            txtNombre.Text = c.strNombre_cen;
            txtArea.Text = c.strArea_cen;
            txtUbicacion.Text = c.strUbicacion_cen;
            txtLineas.Text = c.strLineaInv_cen;
            txtMision.Text = c.strMision_cen;
            txtVision.Text = c.strVision_cen;
            txtFechaAprobacion.Text = c.dtFechaAprobacion_cen.ToString("yyyy-MM-dd");
            if (ddlFacultad.Items.FindByValue(c.strFacultad_cen) != null) ddlFacultad.SelectedValue = c.strFacultad_cen;

            // Buscamos al director actual
            var director = _manejador.BuscarDirectorDelCentro(c.strId_cen);
            txtDirectorActual.Text = director != null ? director.NombreCompleto : "--- SIN ASIGNAR ---";

            // CORRECCIÓN: Se eliminaron las líneas que causaban error (btnGestionarIntegrantes.Visible...)

            CambiarVista(Vista.FormularioCentro);
        }

        // ==========================
        // VISTA 3 & 4: GESTIÓN DE INTEGRANTES
        // ==========================

        private void CargarIntegrantes(string idCentro)
        {
            rptIntegrantes.DataSource = _manejador.ObtenerIntegrantesPorCentro(idCentro);
            rptIntegrantes.DataBind();
        }

        protected void btnNuevoIntegrante_Click(object sender, EventArgs e)
        {
            LimpiarFormInt();
            CambiarVista(Vista.FormularioIntegrante);
        }

        protected void btnGuardarInt_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCedulaInt.Text) || string.IsNullOrWhiteSpace(txtNombresInt.Text))
                {
                    Msg("Cédula y Nombres obligatorios", "ww"); return;
                }

                var i = new InvgccCentroIntegrantes
                {
                    fkId_cen = hfIdCentro.Value,
                    strCedula_cin = txtCedulaInt.Text.Trim(),
                    strNombres_cin = txtNombresInt.Text.Trim(),
                    strApellidos_cin = txtApellidosInt.Text.Trim(),
                    strCorreo_cin = txtCorreoInt.Text.Trim(),
                    strFuncion_cin = ddlFuncionInt.SelectedValue,
                    strTipo_cin = ddlTipoInt.SelectedValue,
                    strCarrera_cin = txtEntidadInt.Text.Trim(),
                    strFacultad_cin = "",
                    strEntidad_cin = (ddlTipoInt.SelectedValue == "Externo") ? txtEntidadInt.Text : ""
                };

                if (string.IsNullOrEmpty(hfIdIntegrante.Value))
                {
                    _manejador.GuardarIntegrante(i);
                    Msg("Integrante agregado.", "ss");
                }
                else
                {
                    i.strId_cin = hfIdIntegrante.Value;
                    _manejador.ActualizarIntegrante(i);
                    Msg("Integrante actualizado.", "ss");
                }

                CargarIntegrantes(hfIdCentro.Value);
                CambiarVista(Vista.ListaIntegrantes);
            }
            catch (Exception ex) { Msg("Error int: " + ex.Message, "ee"); }
        }

        protected void rptIntegrantes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();
            if (e.CommandName == "Eliminar")
            {
                _manejador.EliminarIntegrante(id);
                CargarIntegrantes(hfIdCentro.Value);
                Msg("Integrante eliminado", "ss");
            }
            else if (e.CommandName == "Editar")
            {
                var i = _manejador.ObtenerIntegrantePorId(id);
                if (i != null)
                {
                    hfIdIntegrante.Value = i.strId_cin;
                    txtCedulaInt.Text = i.strCedula_cin;
                    txtNombresInt.Text = i.strNombres_cin;
                    txtApellidosInt.Text = i.strApellidos_cin;
                    txtCorreoInt.Text = i.strCorreo_cin;
                    txtEntidadInt.Text = i.strCarrera_cin;
                    ddlFuncionInt.SelectedValue = i.strFuncion_cin;
                    ddlTipoInt.SelectedValue = i.strTipo_cin;
                    CambiarVista(Vista.FormularioIntegrante);
                }
            }
        }

        protected void btnVolverCentro_Click(object sender, EventArgs e)
        {
            // Regresa a la PANTALLA PRINCIPAL (Centros)
            CargarCentros();
            CambiarVista(Vista.ListaCentros);
        }

        protected void btnCancelarInt_Click(object sender, EventArgs e)
        {
            // Regresa a la LISTA DE INTEGRANTES del mismo centro
            CambiarVista(Vista.ListaIntegrantes);
        }

        // ==========================
        // UTILIDADES Y NAVEGACIÓN
        // ==========================
        protected void btnRegresar_Click(object sender, EventArgs e) { Response.Redirect("CentrosInvestigacion.aspx"); }

        private enum Vista { ListaCentros, FormularioCentro, ListaIntegrantes, FormularioIntegrante }

        private void CambiarVista(Vista v)
        {
            // 1. HEADER PRINCIPAL (CENTROS): Solo visible en la lista principal
            headerCentros.Visible = (v == Vista.ListaCentros);

            // 2. VISIBILIDAD DE PANELES (Al mostrar el panel, se muestra su header interno)
            pnlGrilla.Visible = v == Vista.ListaCentros;
            pnlFormulario.Visible = v == Vista.FormularioCentro;
            pnlIntegrantes.Visible = v == Vista.ListaIntegrantes;
            pnlFormularioInt.Visible = v == Vista.FormularioIntegrante;

            // 3. CONTEXTO DE INTEGRANTES
            if (v == Vista.ListaIntegrantes && !string.IsNullOrEmpty(hfIdCentro.Value))
            {
                var centro = _manejador.ObtenerPorId(hfIdCentro.Value);
                if (centro != null) lblNombreCentroSeleccionado.Text = centro.strNombre_cen;
            }
        }

        private void LimpiarFormulario()
        {
            hfIdCentro.Value = "";
            txtNombre.Text = ""; txtArea.Text = ""; txtUbicacion.Text = "";
            txtDirectorActual.Text = "";
            // CORRECCIÓN: Se eliminaron las líneas que causaban error (msgDirector.Visible...)
        }

        private void LimpiarFormInt()
        {
            hfIdIntegrante.Value = "";
            txtCedulaInt.Text = ""; txtNombresInt.Text = ""; txtApellidosInt.Text = "";
            txtCorreoInt.Text = ""; txtEntidadInt.Text = "";
            ddlFuncionInt.SelectedIndex = 0;
        }

        private void Redireccionar(string msg, string type)
        {
            Session["TempMsg"] = msg;
            Session["TempTipo"] = type;
            Response.Redirect("CentrosInvestigacion.aspx", false);
        }

        private void Msg(string msg, string type)
        {
            string cleanMsg = msg.Replace("'", "").Replace("\r\n", "");
            string script = $"$(function() {{ toastify('{type}', '{cleanMsg}', 'Sistema'); }});";
            ScriptManager.RegisterStartupScript(this, GetType(), "toast", script, true);
        }
    }
}