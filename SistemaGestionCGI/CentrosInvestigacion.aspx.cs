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

            if (Session["RolUsuario"]?.ToString() == "COORDINADOR")
            {
                Response.Redirect("EjecucionProAprobados.aspx");
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

        private void CargarDirectorActual(string idCentro)
        {
            txtDirector.Text = "--- SIN DIRECTOR ASIGNADO ---";
            txtDirector.CssClass = "form-control border-start-0 text-muted"; 

            ViewState["DirectorPendiente"] = null;

            if (!string.IsNullOrEmpty(idCentro))
            {
                var directorActual = _manejador.BuscarDirectorDelCentro(idCentro);

                if (directorActual != null)
                {
                    txtDirector.Text = $"{directorActual.strNombres_cin} {directorActual.strApellidos_cin}";
                    txtDirector.CssClass = "form-control border-start-0 text-dark fw-bold"; 
                }
            }
        }

        private void HabilitarBotonNuevoDirector(bool habilitar)
        {
            if (habilitar)
            {
                btnNuevoDirectorInput.Disabled = false;
                btnNuevoDirectorInput.Attributes["class"] = "btn btn-outline-primary";
                btnNuevoDirectorInput.Attributes.Remove("title");
            }
            else
            {
                btnNuevoDirectorInput.Disabled = true;
                btnNuevoDirectorInput.Attributes["class"] = "btn btn-outline-secondary"; 
                btnNuevoDirectorInput.Attributes["title"] = "Ya existe un Director activo. Para agregar uno nuevo, primero debe dar de baja al actual en la sección 'Integrantes'.";
            }
        }

        protected void btnGuardarDirectorModal_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCedulaDirModal.Text))
                {
                    Msg("La cédula es obligatoria.", "ww");
                    MantenerModalAbierto(); 
                    return;
                }

                if (!EsCedulaValida(txtCedulaDirModal.Text.Trim()))
                {
                    Msg("No se puede asignar: La cédula es inválida.", "ee");
                    txtCedulaDirModal.CssClass = "form-control is-invalid";
                    MantenerModalAbierto(); 
                    return;
                }

                var nuevoDir = new InvgccCentroIntegrantes
                {
                    strCedula_cin = txtCedulaDirModal.Text.Trim(),
                    strNombres_cin = txtNombresDirModal.Text.ToUpper().Trim(),
                    strApellidos_cin = txtApellidosDirModal.Text.ToUpper().Trim(),
                    strCorreo_cin = txtCorreoDirModal.Text.ToLower().Trim(),
                    strTipo_cin = ddlTipoDirModal.SelectedValue,
                    strFuncion_cin = "Director",
                    strCarrera_cin = (ddlTipoDirModal.SelectedValue == "Interno") ? ddlCarreraDirModal.SelectedValue : "",
                    strFacultad_cin = (ddlTipoDirModal.SelectedValue == "Interno") ? ddlFacultadDirModal.SelectedValue : "",
                    strEntidad_cin = (ddlTipoDirModal.SelectedValue == "Externo") ? txtEntidadDirModal.Text.ToUpper() : "UTC"
                };

                ViewState["DirectorPendiente"] = nuevoDir;

                txtDirector.Text = $"{nuevoDir.strNombres_cin} {nuevoDir.strApellidos_cin}";
                txtDirector.CssClass = "form-control border-start-0 text-primary fw-bold";

                Msg("Director asignado temporalmente. (Recuerde Guardar el Centro)", "ss");
            }
            catch (Exception ex)
            {
                Msg("Error al asignar: " + ex.Message, "ee");
                MantenerModalAbierto(); 
            }
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            CambiarVista(Vista.FormularioCentro);

            ScriptManager.RegisterStartupScript(this, GetType(), "InitNewFiles",
                $"if(typeof initMyFileInput === 'function') {{ initMyFileInput('wrapperResolucion', '{flpResolucion.ClientID}'); initMyFileInput('wrapperAceptacion', '{flpAceptacion.ClientID}'); }}", true);
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNombre.Text)) { Msg("Nombre obligatorio.", "ww"); return; }

                var centro = new InvgccCentroInvestigacion
                {
                    strId_cen = hfIdCentro.Value,
                    strNombre_cen = txtNombre.Text,
                    strFacultad_cen = ddlFacultad.SelectedValue,
                    strArea_cen = txtArea.Text,
                    strUbicacion_cen = txtUbicacion.Text,
                    strLineaInv_cen = ddlLineas.SelectedValue,
                    strMision_cen = txtMision.Text,
                    strVision_cen = txtVision.Text,
                    dtFechaAprobacion_cen = string.IsNullOrEmpty(txtFechaAprobacion.Text) ? DateTime.Now : DateTime.Parse(txtFechaAprobacion.Text),
                    strResolucion_cen = hfResolucionActual.Value,
                    strAceptacion_cen = hfAceptacionActual.Value
                };

                if (flpResolucion.HasFile) centro.strResolucion_cen = GuardarArchivo(flpResolucion, "RES");
                if (flpAceptacion.HasFile) centro.strAceptacion_cen = GuardarArchivo(flpAceptacion, "ACEP");

                string idCentroFinal = _manejador.GuardarCentroCompleto(centro);

                string usuario = Session["UsuarioLogueado"]?.ToString() ?? "SISTEMA";

                if (ViewState["DirectorPendiente"] != null)
                {
                    var directorPendiente = (InvgccCentroIntegrantes)ViewState["DirectorPendiente"];

                    var directorAntiguo = _manejador.BuscarDirectorDelCentro(idCentroFinal);
                    if (directorAntiguo != null)
                    {
                        directorAntiguo.strFuncion_cin = "Miembro";
                        _manejador.ActualizarIntegrante(directorAntiguo, usuario);
                    }

                    directorPendiente.fkId_cen = idCentroFinal;
                    _manejador.GuardarIntegrante(directorPendiente, usuario);

                    ViewState["DirectorPendiente"] = null;
                }

                Redireccionar("Centro guardado exitosamente.", "ss");
            }
            catch (Exception ex) { Msg("Error al guardar: " + ex.Message, "ee"); }
        }

        protected void rptCentros_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();

            switch (e.CommandName)
            {
                case "Archivos": 
                    CargarModalDocumentos(id);
                    break;
                case "Integrantes":
                    hfIdCentro.Value = id;

                    hfCentroIdActual.Value = id;
                    ViewState["IdCentroActual"] = id; 

                    var centro = _manejador.ObtenerPorId(id);
                    if (centro != null) lblNombreCentroSeleccionado.Text = centro.strNombre_cen;

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

            if (ddlLineas.Items.FindByValue(c.strLineaInv_cen) != null)
                ddlLineas.SelectedValue = c.strLineaInv_cen;
            else
                ddlLineas.SelectedIndex = 0;

            txtMision.Text = c.strMision_cen;
            txtVision.Text = c.strVision_cen;

            txtFechaAprobacion.Text = c.dtFechaAprobacion_cen != DateTime.MinValue ? c.dtFechaAprobacion_cen.ToString("yyyy-MM-dd") : "";

            if (ddlFacultad.Items.FindByValue(c.strFacultad_cen) != null) ddlFacultad.SelectedValue = c.strFacultad_cen;

            hfResolucionActual.Value = c.strResolucion_cen;
            hfAceptacionActual.Value = c.strAceptacion_cen;

            CargarDirectorActual(c.strId_cen);

            CambiarVista(Vista.FormularioCentro);

            string scriptFiles = $"if(typeof initMyFileInput === 'function') {{ initMyFileInput('wrapperResolucion', '{flpResolucion.ClientID}'); initMyFileInput('wrapperAceptacion', '{flpAceptacion.ClientID}'); }}";

            string scriptEstado = $"cargarEstadoEdicion('wrapperResolucion', '{hfResolucionActual.ClientID}'); cargarEstadoEdicion('wrapperAceptacion', '{hfAceptacionActual.ClientID}');";

            ScriptManager.RegisterStartupScript(this, GetType(), "InitFilesEdit", scriptFiles + scriptEstado, true);
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
                    Msg("Cédula y Nombres son obligatorios.", "ww");
                    MantenerEstadoVisualIntegrantes();
                    return;
                }

                string cedulaIngresada = txtCedulaInt.Text.Trim();

                if (!EsCedulaValida(cedulaIngresada))
                {
                    Msg("La cédula ingresada NO es válida (Formato incorrecto).", "ee");
                    return;
                }

                var integranteExistente = _manejador.BuscarIntegranteActivoPorCedula(cedulaIngresada);

                if (integranteExistente != null)
                {
                    if (string.IsNullOrEmpty(hfIdIntegrante.Value))
                    {
                        Msg($"La cédula {cedulaIngresada} ya está registrada en el sistema (Centro ID: {integranteExistente.fkId_cen}).", "ww");
                        return;
                    }
                    else if (integranteExistente.strId_cin != hfIdIntegrante.Value)
                    {
                        Msg($"La cédula {cedulaIngresada} pertenece a otro integrante.", "ww");
                        return;
                    }
                }

                string funcionAsignar = "Miembro"; 

                if (!string.IsNullOrEmpty(hfIdIntegrante.Value))
                {
                    var integranteActual = _manejador.ObtenerIntegrantePorId(hfIdIntegrante.Value);

                    if (integranteActual != null && integranteActual.strFuncion_cin == "Director")
                    {
                        funcionAsignar = "Director";
                    }
                }

                var i = new InvgccCentroIntegrantes
                {
                    fkId_cen = hfCentroIdActual.Value,
                    strCedula_cin = txtCedulaInt.Text.Trim(),
                    strNombres_cin = txtNombresInt.Text.Trim().ToUpper(),
                    strApellidos_cin = txtApellidosInt.Text.Trim().ToUpper(),
                    strCorreo_cin = txtCorreoInt.Text.Trim().ToLower(),

                    strFuncion_cin = funcionAsignar, 

                    strTipo_cin = ddlTipoInt.SelectedValue
                };

                if (i.strTipo_cin == "Interno")
                {
                    i.strFacultad_cin = ddlFacultadInt.SelectedValue;
                    i.strCarrera_cin = ddlCarreraInt.SelectedValue;
                    i.strEntidad_cin = "UTC";
                }
                else
                {
                    i.strEntidad_cin = txtEntidadExternoInt.Text.Trim().ToUpper();
                    i.strFacultad_cin = "";
                    i.strCarrera_cin = "";
                }

                string usuario = Session["UsuarioLogueado"]?.ToString() ?? "SISTEMA";

                if (string.IsNullOrEmpty(hfIdIntegrante.Value))
                {
                    _manejador.GuardarIntegrante(i, usuario);
                    Msg("Integrante registrado correctamente.", "ss");
                }
                else
                {
                    i.strId_cin = hfIdIntegrante.Value;

                    var original = _manejador.ObtenerIntegrantePorId(i.strId_cin);
                    if (original != null) i.bitActivo_cin = original.bitActivo_cin;

                    _manejador.ActualizarIntegrante(i, usuario);

                    Msg("Integrante actualizado correctamente.", "ss");
                }

                CargarIntegrantes(hfCentroIdActual.Value);

                if (funcionAsignar == "Director") CargarCentros();

                CambiarVista(Vista.ListaIntegrantes);
            }
            catch (Exception ex) { Msg("Error: " + ex.Message, "ee"); }
        }

        private void MantenerEstadoVisualIntegrantes()
        {
            string script = $"ToggleTipoIntegrante(document.getElementById('{ddlTipoInt.ClientID}'));";
            ScriptManager.RegisterStartupScript(this, GetType(), "RestaurarUIInt", script, true);
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

                if (ddlFuncionInt.Items.FindByValue(i.strFuncion_cin) != null)
                    ddlFuncionInt.SelectedValue = i.strFuncion_cin;

                if (ddlTipoInt.Items.FindByValue(i.strTipo_cin) != null)
                    ddlTipoInt.SelectedValue = i.strTipo_cin;

                if (i.strTipo_cin == "Interno")
                {
                    if (ddlFacultadInt.Items.FindByValue(i.strFacultad_cin) != null)
                        ddlFacultadInt.SelectedValue = i.strFacultad_cin;

                    CargarCarreras(ddlCarreraInt, i.strFacultad_cin);

                    if (ddlCarreraInt.Items.FindByValue(i.strCarrera_cin) != null)
                        ddlCarreraInt.SelectedValue = i.strCarrera_cin;

                    txtEntidadExternoInt.Text = "";

                    pnlIntInterno.Visible = true;
                    pnlIntExterno.Visible = false;
                }
                else
                {
                    txtEntidadExternoInt.Text = i.strEntidad_cin;

                    ddlFacultadInt.SelectedIndex = 0;
                    ddlCarreraInt.Items.Clear();
                    ddlCarreraInt.Items.Add(new ListItem("-- Seleccione Facultad --", ""));

                    pnlIntInterno.Visible = false;
                    pnlIntExterno.Visible = true;
                }

                CambiarVista(Vista.FormularioIntegrante);
            }
        }

        // ==========================
        // 3. MODALS (ESTADO, HISTORIAL, REPORTE)
        // ==========================

        private void CargarModalDocumentos(string id)
        {
            var centro = _manejador.ObtenerPorId(id);
            if (centro == null) return;

            lblCentroDocNombre.Text = centro.strNombre_cen;
            hfIdCentroDocModal.Value = centro.strId_cen;

            hfResModalActual.Value = centro.strResolucion_cen;
            if (!string.IsNullOrEmpty(centro.strResolucion_cen))
            {
                lnkDescargarResModal.NavigateUrl = ResolveUrl(centro.strResolucion_cen);
                lnkDescargarResModal.Visible = true;
            }
            else
            {
                lnkDescargarResModal.Visible = false;
            }

            hfAceModalActual.Value = centro.strAceptacion_cen;
            if (!string.IsNullOrEmpty(centro.strAceptacion_cen))
            {
                lnkDescargarAceModal.NavigateUrl = ResolveUrl(centro.strAceptacion_cen);
                lnkDescargarAceModal.Visible = true;
            }
            else
            {
                lnkDescargarAceModal.Visible = false;
            }

            string script = @"
                var m = new bootstrap.Modal(document.getElementById('modalDocumentos')); 
                m.show();
        
                // Inicializar FileInput del Modal
                if(typeof initMyFileInput === 'function') {
                    initMyFileInput('wrapperResModal', '" + flpResModal.ClientID + @"');
                    initMyFileInput('wrapperAceModal', '" + flpAceModal.ClientID + @"');
                }

                // Cargar vista previa si ya existen
                cargarEstadoEdicion('wrapperResModal', '" + hfResModalActual.ClientID + @"');
                cargarEstadoEdicion('wrapperAceModal', '" + hfAceModalActual.ClientID + @"');
            ";

            ScriptManager.RegisterStartupScript(this, GetType(), "OpenDocsEditable", script, true);
        }

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
            ddlLineas.SelectedIndex = 0; txtMision.Text = ""; txtVision.Text = "";
            txtFechaAprobacion.Text = DateTime.Now.ToString("yyyy-MM-dd");
            hfResolucionActual.Value = "";
            hfAceptacionActual.Value = "";

            txtDirector.Text = "";
            ViewState["DirectorPendiente"] = null;

            ViewState["DirectorPendiente"] = null;
        }

        private void LimpiarFormInt()
        {
            hfIdIntegrante.Value = "";
            txtCedulaInt.Text = "";
            txtNombresInt.Text = "";
            txtApellidosInt.Text = "";
            txtCorreoInt.Text = "";
            ddlTipoInt.SelectedIndex = 0;
            ddlFacultadInt.SelectedIndex = 0;
            ddlCarreraInt.Items.Clear();
            ddlCarreraInt.Items.Add(new ListItem("-- Seleccione Facultad Primero --", ""));

            string script = $"ToggleTipoIntegrante(document.getElementById('{ddlTipoInt.ClientID}'));";
            ScriptManager.RegisterStartupScript(this, GetType(), "ResetDesign", script, true);
        }

        private void Redireccionar(string msg, string type) { 
            Session["TempMsg"] = msg; 
            Session["TempTipo"] = type; 
            Response.Redirect("CentrosInvestigacion.aspx", false); 
        }
        private void Msg(string msg, string type) { string script = $"$(function() {{ toastify('{type}', '{msg.Replace("'", "").Replace("\r\n", "")}', 'Sistema'); }});"; ScriptManager.RegisterStartupScript(this, GetType(), "toast", script, true); }

        //

        private void CargarCarreras(DropDownList ddl, string facultad)
        {
            ddl.Items.Clear();
            ddl.Items.Add(new ListItem("-- Seleccione --", ""));

            if (string.IsNullOrEmpty(facultad)) return;

            switch (facultad)
            {
                case "CIYA":
                    ddl.Items.Add(new ListItem("SISTEMAS DE INFORMACIÓN", "SISTEMAS DE INFORMACIÓN"));
                    ddl.Items.Add(new ListItem("INDUSTRIAL", "INDUSTRIAL"));
                    ddl.Items.Add(new ListItem("ELECTROMECÁNICA", "ELECTROMECANICA")); 
                    ddl.Items.Add(new ListItem("ELECTRICIDAD", "ELECTRICIDAD"));
                    ddl.Items.Add(new ListItem("HIDRAULICA", "HIDRAULICA"));
                    ddl.Items.Add(new ListItem("SOFTWARE", "SOFTWARE"));
                    break;

                case "CAREN":
                    ddl.Items.Add(new ListItem("AGRONOMÍA", "AGRONOMIA")); 
                    ddl.Items.Add(new ListItem("VETERINARIA", "VETERINARIA"));
                    ddl.Items.Add(new ListItem("TURISMO", "TURISMO"));
                    ddl.Items.Add(new ListItem("AMBIENTE", "AMBIENTE"));
                    ddl.Items.Add(new ListItem("AGROPECUARIAS", "AGROPECUARIAS"));
                    ddl.Items.Add(new ListItem("BIOTECNOLOGIA", "BIOTECNOLOGIA"));
                    break;

                case "CAYE":
                    ddl.Items.Add(new ListItem("ADMINISTRACIÓN DE EMPRESAS", "ADMINISTRACIÓN DE EMPRESAS"));
                    ddl.Items.Add(new ListItem("CONTABILIDAD", "CONTABILIDAD"));
                    ddl.Items.Add(new ListItem("MERCADOTÉCNIA", "MERCADOTÉCNIA"));
                    ddl.Items.Add(new ListItem("ECONOMIA", "ECONOMIA"));
                    ddl.Items.Add(new ListItem("FINANZAS", "FINANZAS"));
                    ddl.Items.Add(new ListItem("GESTIÓN DEL TALENTO HUMANO", "GESTIÓN DEL TALENTO HUMANO"));
                    break;

                case "CSAYE":
                    ddl.Items.Add(new ListItem("DISEÑO GRAFICO", "DISEÑO GRAFICO"));
                    ddl.Items.Add(new ListItem("DISEÑO GRAFICO INTERACTIVO", "DISEÑO GRAFICO INTERACTIVO"));
                    ddl.Items.Add(new ListItem("COMUNICACIÓN", "COMUNICACIÓN"));
                    ddl.Items.Add(new ListItem("TRABAJO SOCIAL", "TRABAJO SOCIAL"));
                    ddl.Items.Add(new ListItem("ANIMACIÓN DIGITAL", "ANIMACIÓN DIGITAL"));
                    ddl.Items.Add(new ListItem("PSICOLOGÍA SOCIAL", "PSICOLOGÍA SOCIAL"));
                    break;

                case "SALUD":
                    ddl.Items.Add(new ListItem("ENFERMERIA", "ENFERMERIA"));
                    break;

                case "PUJILI":
                    ddl.Items.Add(new ListItem("EDUCACIÓN INICIAL", "EDUCACIÓN INICIAL"));
                    ddl.Items.Add(new ListItem("EDUCACIÓN BASICA", "EDUCACIÓN BASICA"));
                    ddl.Items.Add(new ListItem("PEDAGOGÍA DEL IDIOMA INGLÉS", "PEDAGOGÍA DEL IDIOMA INGLÉS"));
                    ddl.Items.Add(new ListItem("PEDAGOGÍA DE LA LENGUA Y LITERATURA", "PEDAGOGÍA DE LA LENGUA Y LITERATURA"));
                    ddl.Items.Add(new ListItem("PEDAGOGÍA DE LAS MATEMÁTICAS Y LA FÍSICA", "PEDAGOGÍA DE LAS MATEMÁTICAS Y LA FÍSICA"));
                    break;

                case "LAMANA":
                    ddl.Items.Add(new ListItem("CONTABILIDAD_LM", "CONTABILIDAD_LM"));
                    ddl.Items.Add(new ListItem("ADMINISTRACIÓN_LM", "ADMINISTRACIÓN_LM"));
                    ddl.Items.Add(new ListItem("ELECTROMECÁNICA_LM", "ELECTROMECÁNICA_LM"));
                    ddl.Items.Add(new ListItem("SISTEMAS DE INFORMACIÓN_LM", "SISTEMAS DE INFORMACIÓN_LM"));
                    ddl.Items.Add(new ListItem("TURISMO_LM", "TURISMO_LM"));
                    ddl.Items.Add(new ListItem("AGRONOMÍA_LM", "AGRONOMÍA_LM"));
                    ddl.Items.Add(new ListItem("AGROINDUSTRIAS_LM", "AGROINDUSTRIAS_LM"));
                    break;
            }
        }

        // Método para guardar archivos físicos en el servidor
        private string GuardarArchivo(FileUpload control, string tipo)
        {
            if (!control.HasFile) return "";

            try
            {
                string nombre = $"{tipo}_{DateTime.Now.Ticks}{System.IO.Path.GetExtension(control.FileName)}";
                string carpetaVirtual = "~/RepositorioUTC/Centros/";
                string carpetaFisica = Server.MapPath(carpetaVirtual);

                if (!System.IO.Directory.Exists(carpetaFisica))
                    System.IO.Directory.CreateDirectory(carpetaFisica);

                control.SaveAs(System.IO.Path.Combine(carpetaFisica, nombre));
                return $"{carpetaVirtual}{nombre}";
            }
            catch (Exception ex) { throw new Exception("Error al subir archivo: " + ex.Message); }
        }

        protected void ddlFacultadInt_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarCarreras(ddlCarreraInt, ddlFacultadInt.SelectedValue);

            ScriptManager.RegisterStartupScript(this, GetType(), "ReOpenCascada", "abrirModalInt(); toggleFormInt();", true);
        }

        protected void ddlFacultadDirModal_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarCarreras(ddlCarreraDirModal, ddlFacultadDirModal.SelectedValue);

            string script = "new bootstrap.Modal(document.getElementById('modalNuevoDirector')).show(); ToggleTipoDirector(document.getElementById('ddlTipoDirModal'));";
            ScriptManager.RegisterStartupScript(this, GetType(), "ReOpenDirCascada", script, true);
        }

        // ==========================================
        // 5. GESTIÓN DE PAPELERA (FALTABA ESTO)
        // ==========================================

        protected void btnVerPapelera_Click(object sender, EventArgs e)
        {
            try
            {
                string idCentro = hfCentroIdActual.Value;

                if (string.IsNullOrEmpty(idCentro))
                {
                    Msg("No se ha seleccionado el centro.", "ww");
                    return;
                }

                var listaEliminados = _manejador.ObtenerIntegrantesPapelera(idCentro);

                rptPapelera.DataSource = listaEliminados;
                rptPapelera.DataBind();

                ScriptManager.RegisterStartupScript(this, GetType(), "OpenPapelera", "abrirModalPapelera();", true);
            }
            catch (Exception ex) { Msg("Error al cargar papelera: " + ex.Message, "ee"); }
        }

        protected void rptPapelera_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Restaurar")
            {
                string idIntegrante = e.CommandArgument.ToString();
                string usuario = Session["UsuarioLogueado"]?.ToString() ?? "SISTEMA";

                bool restaurado = _manejador.RestaurarIntegrante(idIntegrante, usuario);

                if (restaurado)
                {
                    Msg("Integrante restaurado exitosamente.", "ss");
                    CargarIntegrantes(hfCentroIdActual.Value);

                    btnVerPapelera_Click(null, null);
                }
                else
                {
                    Msg("No se puede restaurar: Ya existe un DIRECTOR activo en el centro.", "ww");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ReOpenPapelera", "abrirModalPapelera();", true);
                }
            }
        }

        private bool EsCedulaValida(string cedula)
        {
            if (string.IsNullOrEmpty(cedula) || cedula.Length != 10) return false;
            if (!long.TryParse(cedula, out _)) return false;

            try
            {
                int provincia = int.Parse(cedula.Substring(0, 2));
                if (!((provincia >= 1 && provincia <= 24) || provincia == 30)) return false;

                int tercerDigito = int.Parse(cedula.Substring(2, 1));
                if (tercerDigito >= 6) return false;

                int[] coeficientes = { 2, 1, 2, 1, 2, 1, 2, 1, 2 };
                int suma = 0;
                int verificador = int.Parse(cedula.Substring(9, 1));

                for (int i = 0; i < 9; i++)
                {
                    int digito = int.Parse(cedula.Substring(i, 1));
                    int producto = digito * coeficientes[i];

                    if (producto >= 10) producto -= 9;
                    suma += producto;
                }

                int residuo = suma % 10;
                int resultado = (residuo == 0) ? 0 : (10 - residuo);

                return resultado == verificador;
            }
            catch
            {
                return false;
            }
        }

        protected void btnValidarCedula_Click(object sender, EventArgs e)
        {
            try
            {
                string cedula = txtCedulaDirModal.Text.Trim();

                txtCedulaDirModal.CssClass = "form-control";

                if (string.IsNullOrEmpty(cedula))
                {
                    txtCedulaDirModal.CssClass = "form-control is-invalid";
                    Msg("Ingrese un número de cédula.", "ww");
                    return;
                }

                if (!EsCedulaValida(cedula))
                {
                    txtCedulaDirModal.CssClass = "form-control is-invalid";
                    Msg("Cédula inválida/incorrecta.", "ee");
                    return;
                }

                var existe = _manejador.BuscarIntegranteActivoPorCedula(cedula);
                if (existe != null)
                {
                    txtCedulaDirModal.CssClass = "form-control is-warning"; 
                    Msg($"Esta persona ya existe en el centro {existe.fkId_cen}.", "ww");
                }
                else
                {
                    txtCedulaDirModal.CssClass = "form-control is-valid"; 
                    Msg("Cédula correcta y disponible.", "ss");
                    txtNombresDirModal.Focus();
                }
            }
            catch (Exception ex)
            {
                Msg("Error: " + ex.Message, "ee");
            }
            finally
            {
                MantenerModalAbierto();
            }
        }

        private void MantenerModalAbierto()
        {
            string scriptAbrir = "new bootstrap.Modal(document.getElementById('modalNuevoDirector')).show();";
            string scriptRestaurarUI = $"ToggleTipoDirector(document.getElementById('{ddlTipoDirModal.ClientID}'));";
            ScriptManager.RegisterStartupScript(this, GetType(), "ReOpenAndFixUI", scriptAbrir + scriptRestaurarUI, true);
        }

        protected void btnValidarCedulaInt_Click(object sender, EventArgs e)
        {
            try
            {
                string cedula = txtCedulaInt.Text.Trim();

                if (string.IsNullOrEmpty(cedula))
                {
                    txtCedulaInt.CssClass = "form-control is-invalid"; 
                    Msg("Ingrese un número de cédula.", "ww");
                    return;
                }

                if (!EsCedulaValida(cedula))
                {
                    txtCedulaInt.CssClass = "form-control is-invalid"; 
                    Msg("Cédula INCORRECTA (Formato inválido).", "ee");
                    return;
                }

                var existente = _manejador.BuscarIntegranteActivoPorCedula(cedula);

                if (existente != null)
                {
                    if (string.IsNullOrEmpty(hfIdIntegrante.Value))
                    {
                        txtCedulaInt.CssClass = "form-control is-invalid";
                        Msg($"Esta persona YA EXISTE en el centro: {existente.fkId_cen}.", "ww");
                    }
                    else if (existente.strId_cin != hfIdIntegrante.Value)
                    {
                        txtCedulaInt.CssClass = "form-control is-invalid";
                        Msg("La cédula pertenece a otro integrante registrado.", "ww");
                    }
                    else
                    {
                        txtCedulaInt.CssClass = "form-control is-valid"; 
                        Msg("Cédula VÁLIDA (Es la actual del usuario).", "ss");
                    }
                }
                else
                {
                    txtCedulaInt.CssClass = "form-control is-valid"; 
                    Msg("Cédula VÁLIDA y DISPONIBLE.", "ss");
                    txtNombresInt.Focus();
                }
            }
            catch (Exception ex) { Msg("Error validación: " + ex.Message, "ee"); }
        }

        // 
        protected void ddlTipoDirModal_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtCedulaDirModal.Text = string.Empty;
            txtNombresDirModal.Text = string.Empty;
            txtApellidosDirModal.Text = string.Empty;
            txtCorreoDirModal.Text = string.Empty;
            txtEntidadDirModal.Text = string.Empty;

            txtCedulaDirModal.CssClass = "form-control";

            string tipo = ddlTipoDirModal.SelectedValue;

            if (tipo == "Interno")
            {
                pnlDirInterno.Visible = true;
                pnlDirExterno.Visible = false;

                ddlFacultadDirModal.SelectedIndex = 0;
                ddlCarreraDirModal.Items.Clear();
                ddlCarreraDirModal.Items.Add(new ListItem("-- Seleccione Facultad Primero --", ""));
            }
            else
            {
                pnlDirInterno.Visible = false;
                pnlDirExterno.Visible = true;
            }

            string scriptReabrir = @"
                var myModal = new bootstrap.Modal(document.getElementById('modalNuevoDirector'));
                myModal.show();
            ";

            ScriptManager.RegisterStartupScript(this, GetType(), "ReOpenModalDir", scriptReabrir, true);
        }

        protected void ddlTipoInt_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtCedulaInt.Text = string.Empty;
            txtNombresInt.Text = string.Empty;
            txtApellidosInt.Text = string.Empty;
            txtCorreoInt.Text = string.Empty;

            txtEntidadExternoInt.Text = string.Empty;

            string tipo = ddlTipoInt.SelectedValue;

            txtCedulaInt.CssClass = "form-control";

            if (tipo == "Interno")
            {
                pnlIntInterno.Visible = true;
                pnlIntExterno.Visible = false;

                ddlFacultadInt.SelectedIndex = 0;
                ddlCarreraInt.Items.Clear();
                ddlCarreraInt.Items.Add(new ListItem("-- Seleccione Facultad Primero --", ""));
            }
            else
            {
                pnlIntInterno.Visible = false;
                pnlIntExterno.Visible = true;
            }

        }

        //

        protected void btnActualizarDocs_Click(object sender, EventArgs e)
        {
            try
            {
                string idCentro = hfIdCentroDocModal.Value;
                string rutaRes = "";
                string rutaAce = "";
                bool huboCambios = false;

                if (flpResModal.HasFile)
                {
                    rutaRes = GuardarArchivo(flpResModal, "RES"); 
                    huboCambios = true;
                }

                if (flpAceModal.HasFile)
                {
                    rutaAce = GuardarArchivo(flpAceModal, "ACEP");
                    huboCambios = true;
                }

                if (huboCambios)
                {
                    _manejador.ActualizarArchivosCentro(idCentro, rutaRes, rutaAce);

                    Msg("Documentación actualizada correctamente.", "ss");
                }
                else
                {
                    Msg("No se detectaron nuevos archivos para actualizar.", "ii");
                }

                string scriptCerrar = "var m = bootstrap.Modal.getInstance(document.getElementById('modalDocumentos')); if(m) m.hide();";
                ScriptManager.RegisterStartupScript(this, GetType(), "CloseDocs", scriptCerrar, true);
            }
            catch (Exception ex)
            {
                Msg("Error al actualizar documentos: " + ex.Message, "ee");
                ScriptManager.RegisterStartupScript(this, GetType(), "ReOpenDocs", "new bootstrap.Modal(document.getElementById('modalDocumentos')).show();", true);
            }
        }

    }
}