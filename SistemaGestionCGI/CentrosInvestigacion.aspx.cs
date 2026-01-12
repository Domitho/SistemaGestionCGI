using System;
using System.Collections.Generic;
using System.Text;
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

            try
            {
                CargarCentros();
            }
            catch (Exception ex) { Msg("Error al cargar módulo: " + ex.Message, "ee"); }

            if (Session["TempMsg"] != null)
            {
                Msg(Session["TempMsg"].ToString(), Session["TempTipo"].ToString());
                Session["TempMsg"] = null;
                Session["TempTipo"] = null;
            }
        }

        // ==========================
        // 1. GESTIÓN DE CENTROS
        // ==========================
        private void CargarCentros()
        {
            rptCentros.DataSource = _manejador.ObtenerTodos();
            rptCentros.DataBind();
        }

        private void CargarCombosDirector(string idCentro)
        {
            ddlDirector.Items.Clear();
            ddlDirector.Items.Add(new ListItem("-- Sin Director Asignado --", ""));

            if (!string.IsNullOrEmpty(idCentro))
            {
                var integrantes = _manejador.ObtenerIntegrantesPorCentro(idCentro);

                foreach (var item in integrantes)
                {
                    ListItem li = new ListItem(item.NombreCompleto, item.strId_cin); 
                    ddlDirector.Items.Add(li);
                }

                var directorActual = _manejador.BuscarDirectorDelCentro(idCentro);
                if (directorActual != null && ddlDirector.Items.FindByValue(directorActual.strId_cin) != null)
                {
                    ddlDirector.SelectedValue = directorActual.strId_cin;
                }
            }
        }

        protected void btnGuardarDirectorModal_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCedulaDirModal.Text) || string.IsNullOrWhiteSpace(txtNombresDirModal.Text))
                {
                    Msg("Cédula y Nombres obligatorios.", "ww");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ReOpen", "new bootstrap.Modal(document.getElementById('modalNuevoDirector')).show();", true);
                    return;
                }

                var nuevoDir = new InvgccCentroIntegrantes
                {
                    fkId_cen = hfIdCentro.Value,
                    strCedula_cin = txtCedulaDirModal.Text.Trim(),
                    strNombres_cin = txtNombresDirModal.Text.Trim(),
                    strApellidos_cin = txtApellidosDirModal.Text.Trim(),
                    strCorreo_cin = txtCorreoDirModal.Text.Trim(),
                    strTipo_cin = ddlTipoDirModal.SelectedValue,
                    strFuncion_cin = "Director",
                    strCarrera_cin = txtCarreraDirModal.Text.Trim(),
                    strFacultad_cin = ddlFacultadDirModal.SelectedValue,
                    strEntidad_cin = (ddlTipoDirModal.SelectedValue == "Externo") ? txtEntidadDirModal.Text : ""
                };

                if (string.IsNullOrEmpty(hfIdCentro.Value))
                {
                    ViewState["DirectorPendiente"] = nuevoDir;

                    ddlDirector.Items.Clear();
                    string nombreCompleto = $"{nuevoDir.strApellidos_cin} {nuevoDir.strNombres_cin}";
                    ddlDirector.Items.Add(new ListItem(nombreCompleto + " (Por Guardar)", "-1"));
                    ddlDirector.SelectedValue = "-1";

                    Msg("Director asignado. Se guardará al crear el Centro.", "ii");
                }
                else
                {
                    string usuario = Session["UsuarioLogueado"]?.ToString() ?? "SISTEMA";
                    _manejador.GuardarIntegrante(nuevoDir, usuario);

                    CargarCombosDirector(hfIdCentro.Value);

                    var dirGuardado = _manejador.ObtenerIntegrantesPorCentro(hfIdCentro.Value)
                                                .Find(x => x.strCedula_cin == nuevoDir.strCedula_cin);
                    if (dirGuardado != null) ddlDirector.SelectedValue = dirGuardado.strId_cin;

                    Msg("Director registrado exitosamente.", "ss");
                }

                txtCedulaDirModal.Text = ""; txtNombresDirModal.Text = ""; txtApellidosDirModal.Text = "";
                txtCorreoDirModal.Text = ""; txtCarreraDirModal.Text = ""; txtEntidadDirModal.Text = "";
            }
            catch (Exception ex) { Msg("Error modal: " + ex.Message, "ee"); }
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
                if (string.IsNullOrWhiteSpace(txtNombre.Text)) { Msg("Nombre del centro obligatorio.", "ww"); return; }

                bool hayDirectorSeleccionado = ddlDirector.SelectedValue != "" && ddlDirector.SelectedValue != "0";
                bool hayDirectorPendiente = ViewState["DirectorPendiente"] != null;

                if (!hayDirectorSeleccionado && !hayDirectorPendiente)
                {
                    Msg("El Director es obligatorio. Seleccione uno o cree uno nuevo.", "ww");
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
                    dtFechaAprobacion_cen = string.IsNullOrEmpty(txtFechaAprobacion.Text) ? DateTime.Now : DateTime.Parse(txtFechaAprobacion.Text)
                };

                if (string.IsNullOrEmpty(hfIdCentro.Value))
                {
                    _manejador.Guardar(centro);

                    if (hayDirectorPendiente)
                    {
                        var dirPendiente = (InvgccCentroIntegrantes)ViewState["DirectorPendiente"];
                        dirPendiente.fkId_cen = centro.strId_cen; 

                        string usuario = Session["UsuarioLogueado"]?.ToString() ?? "SISTEMA";
                        _manejador.GuardarIntegrante(dirPendiente, usuario);

                        ViewState["DirectorPendiente"] = null;
                    }
                    Redireccionar("Centro y Director registrados correctamente.", "ss");
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
            string id = e.CommandArgument.ToString();

            switch (e.CommandName)
            {
                case "Integrantes":
                    hfIdCentro.Value = id;
                    CargarIntegrantes(id);
                    CambiarVista(Vista.ListaIntegrantes);
                    break;
                case "Editar":
                    CargarEdicion(id);
                    break;
                case "Eliminar":
                    _manejador.Eliminar(id);
                    Redireccionar("Centro eliminado.", "ss");
                    break;
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

            CargarCombosDirector(c.strId_cen);

            CambiarVista(Vista.FormularioCentro);
        }

        // ==========================
        // 2. GESTIÓN DE INTEGRANTES
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

                string usuario = Session["UsuarioLogueado"]?.ToString() ?? "SISTEMA";

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
                    _manejador.GuardarIntegrante(i, usuario);
                    Msg("Integrante agregado.", "ss");
                }
                else
                {
                    i.strId_cin = hfIdIntegrante.Value;
                    var original = _manejador.ObtenerIntegrantePorId(i.strId_cin);
                    if (original != null) i.bitActivo_cin = original.bitActivo_cin; 

                    _manejador.ActualizarIntegrante(i);
                    _manejador.GuardarHistorial(i.strId_cin, "EDICIÓN", "Actualización de datos generales", usuario);
                    Msg("Datos actualizados.", "ss");
                }

                CargarIntegrantes(hfIdCentro.Value);
                CambiarVista(Vista.ListaIntegrantes);
            }
            catch (Exception ex) { Msg("Error int: " + ex.Message, "ee"); }
        }

        protected void rptIntegrantes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();

            switch (e.CommandName)
            {
                case "Eliminar": 
                    CargarModalEstado(id);
                    break;
                case "Editar":
                    CargarEdicionIntegrante(id);
                    break;
                case "Historial":
                    CargarModalHistorial(id);
                    break;
            }
        }

        private void CargarEdicionIntegrante(string id)
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
                if (ddlFuncionInt.Items.FindByValue(i.strFuncion_cin) != null) ddlFuncionInt.SelectedValue = i.strFuncion_cin;
                if (ddlTipoInt.Items.FindByValue(i.strTipo_cin) != null) ddlTipoInt.SelectedValue = i.strTipo_cin;

                CambiarVista(Vista.FormularioIntegrante);
            }
        }

        // ==========================
        // 3. MODALS (ESTADO, HISTORIAL, REPORTE)
        // ==========================

        private void CargarModalEstado(string idInt)
        {
            hfIdIntegranteEstado.Value = idInt;
            txtMotivoEstado.Text = "";
            ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalEst",
                "new bootstrap.Modal(document.getElementById('modalEstadoInt')).show();", true);
        }

        protected void btnConfirmarCambioEstado_Click(object sender, EventArgs e)
        {
            try
            {
                string id = hfIdIntegranteEstado.Value;
                string motivo = txtMotivoEstado.Text.Trim();
                string usuario = Session["UsuarioLogueado"]?.ToString() ?? "SISTEMA";

                if (string.IsNullOrEmpty(motivo)) { Msg("Debe ingresar un motivo.", "ww"); return; }

                _manejador.CambiarEstadoIntegrante(id, motivo, usuario);

                CargarIntegrantes(hfIdCentro.Value);
                Msg("Estado actualizado correctamente.", "ss");
            }
            catch (Exception ex) { Msg("Error cambio estado: " + ex.Message, "ee"); }
        }

        private void CargarModalHistorial(string idInt)
        {
            var i = _manejador.ObtenerIntegrantePorId(idInt);
            if (i != null)
            {
                lblNombreHistorial.Text = $"{i.strNombres_cin} {i.strApellidos_cin}";
                hfIdIntegranteHistorial.Value = idInt;
                rptHistorial.DataSource = _manejador.ObtenerHistorial(idInt);
                rptHistorial.DataBind();

                ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalHist",
                    "new bootstrap.Modal(document.getElementById('modalHistorial')).show();", true);
            }
        }

        protected void btnGenerarReporte_Click(object sender, EventArgs e)
        {
            try
            {
                string idInt = hfIdIntegranteHistorial.Value;
                if (string.IsNullOrEmpty(idInt)) return;

                var integrante = _manejador.ObtenerIntegrantePorId(idInt);
                var historial = _manejador.ObtenerHistorial(idInt);

                if (integrante != null)
                {
                    lblRefId.Text = integrante.strId_cin;
                    lblReporteNombre.Text = $"{integrante.strApellidos_cin} {integrante.strNombres_cin}";
                    lblReporteCedula.Text = integrante.strCedula_cin;
                    lblReporteFuncion.Text = integrante.strFuncion_cin;

                    lblReporteEstado.Text = integrante.bitActivo_cin ? "ACTIVO" : "INACTIVO";
                    lblReporteEstado.ForeColor = integrante.bitActivo_cin
                        ? System.Drawing.ColorTranslator.FromHtml("#1b9e4b")
                        : System.Drawing.ColorTranslator.FromHtml("#d9534f");

                    rptReporteHistorial.DataSource = historial;
                    rptReporteHistorial.DataBind();

                    string script = "var m = new bootstrap.Modal(document.getElementById('modalVistaPrevia')); m.show();";
                    ScriptManager.RegisterStartupScript(this, GetType(), "OpenPreview", script, true);
                }
            }
            catch (Exception ex) { Msg("Error al generar reporte: " + ex.Message, "ee"); }
        }

        private string ConstruirHtmlReporte(InvgccCentroIntegrantes integrante, List<InvgccCentroIntegrantesHistorial> historial)
        {
            StringBuilder sb = new StringBuilder();

            // --- ENCABEZADO ---
            sb.Append("<div class='text-center mb-4'>");
            sb.Append("<h4 class='text-uppercase fw-bold'>Reporte de Movimientos</h4>");
            sb.Append($"<p class='text-muted small'>Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}</p>");
            sb.Append("</div>");

            // --- DATOS ---
            sb.Append("<div class='card mb-4 border-0 shadow-sm'><div class='card-body bg-light rounded'><div class='row'>");
            sb.Append($"<div class='col-6'><strong>Cédula:</strong> {integrante.strCedula_cin}</div>");
            sb.Append($"<div class='col-6'><strong>Nombre:</strong> {integrante.strApellidos_cin} {integrante.strNombres_cin}</div>");
            sb.Append($"<div class='col-6'><strong>Función:</strong> {integrante.strFuncion_cin}</div>");
            sb.Append($"<div class='col-6'><strong>Estado Actual:</strong> {(integrante.bitActivo_cin ? "ACTIVO" : "INACTIVO")}</div>");
            sb.Append("</div></div></div>");

            // --- TABLA HISTORIAL ---
            sb.Append("<table class='table table-bordered table-striped text-center small'>");
            sb.Append("<thead class='table-dark text-white'><tr><th>FECHA</th><th>ACCIÓN</th><th>MOTIVO</th><th>USUARIO</th></tr></thead><tbody>");

            if (historial != null && historial.Count > 0)
            {
                foreach (var item in historial)
                {
                    string colorBadge = item.strAccion == "BAJA" ? "bg-danger" : "bg-success";
                    sb.Append("<tr>");
                    sb.Append($"<td>{item.dtFecha:dd/MM/yyyy HH:mm}</td>");
                    sb.Append($"<td><span class='badge {colorBadge}'>{item.strAccion}</span></td>");
                    sb.Append($"<td class='text-start'>{item.strMotivo}</td>");
                    sb.Append($"<td>{item.strUsuario}</td>");
                    sb.Append("</tr>");
                }
            }
            else
            {
                sb.Append("<tr><td colspan='4' class='text-muted py-3'>No existen movimientos registrados.</td></tr>");
            }
            sb.Append("</tbody></table>");

            return sb.ToString();
        }

        // ==========================
        // 4. UTILIDADES
        // ==========================
        private enum Vista { ListaCentros, FormularioCentro, ListaIntegrantes, FormularioIntegrante }

        private void CambiarVista(Vista v)
        {
            headerCentros.Visible = (v == Vista.ListaCentros);
            pnlGrilla.Visible = v == Vista.ListaCentros;
            pnlFormulario.Visible = v == Vista.FormularioCentro;
            pnlIntegrantes.Visible = v == Vista.ListaIntegrantes;
            pnlFormularioInt.Visible = v == Vista.FormularioIntegrante;

            if (v == Vista.ListaIntegrantes && !string.IsNullOrEmpty(hfIdCentro.Value))
            {
                var centro = _manejador.ObtenerPorId(hfIdCentro.Value);
                if (centro != null) lblNombreCentroSeleccionado.Text = centro.strNombre_cen;
            }
        }

        protected void btnVolverCentro_Click(object sender, EventArgs e) { CargarCentros(); CambiarVista(Vista.ListaCentros); }
        protected void btnCancelarInt_Click(object sender, EventArgs e) { CambiarVista(Vista.ListaIntegrantes); }
        protected void btnRegresar_Click(object sender, EventArgs e) { Response.Redirect("CentrosInvestigacion.aspx"); }

        private void LimpiarFormulario()
        {
            hfIdCentro.Value = "";
            txtNombre.Text = ""; txtArea.Text = ""; txtUbicacion.Text = "";
            txtLineas.Text = ""; txtMision.Text = ""; txtVision.Text = "";
            txtFechaAprobacion.Text = DateTime.Now.ToString("yyyy-MM-dd");

            ddlDirector.Items.Clear();
            ddlDirector.Items.Add(new ListItem("-- Sin Director Asignado --", ""));
            ViewState["DirectorPendiente"] = null;
        }

        private void LimpiarFormInt() { 
            hfIdIntegrante.Value = ""; 
            txtCedulaInt.Text = ""; 
            txtNombresInt.Text = ""; 
            txtApellidosInt.Text = ""; 
            txtCorreoInt.Text = ""; 
            txtEntidadInt.Text = ""; 
            ddlFuncionInt.SelectedIndex = 0; 
        }

        private void Redireccionar(string msg, string type) { 
            Session["TempMsg"] = msg; 
            Session["TempTipo"] = type; 
            Response.Redirect("CentrosInvestigacion.aspx", false); 
        }
        private void Msg(string msg, string type) { string script = $"$(function() {{ toastify('{type}', '{msg.Replace("'", "").Replace("\r\n", "")}', 'Sistema'); }});"; ScriptManager.RegisterStartupScript(this, GetType(), "toast", script, true); }
    }
}