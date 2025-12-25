using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;

namespace SistemaGestionCGI
{
    public partial class GruposInvestigacion : System.Web.UI.Page
    {
        // 1. Instancias y Constantes
        private readonly ManejadorGruposInvestigacion _manejador = new ManejadorGruposInvestigacion();
        private const string RUTA_BASE_GRUPOS = @"C:\UTC\GRUPOS\";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            // Validar Sesión
            if (Session["UsuarioLogueado"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // Carga Inicial
            CargarGrillaGrupos();
            
            // Verificar Redirección (Al volver de detalle integrantes)
            string idGrupoRedirect = Request.QueryString["idGrupo"];
            if (!string.IsNullOrEmpty(idGrupoRedirect))
            {
                CargarIntegrantesPanel(idGrupoRedirect);
            }

            // Mensajes Flash
            if (Session["TempMsg"] != null)
            {
                Msg(Session["TempMsg"].ToString(), Session["TempTipo"].ToString());
                Session["TempMsg"] = null;
                Session["TempTipo"] = null;
            }
        }

        // =============================================
        // GESTIÓN DE GRUPOS (CRUD)
        // =============================================

        private void CargarGrillaGrupos()
        {
            try
            {
                rptGrupoInv.DataSource = _manejador.ObtenerGruposConConteo();
                rptGrupoInv.DataBind();
            }
            catch (Exception ex) { Msg("Error al cargar grupos: " + ex.Message, "ee"); }
        }

        protected void lbtNuevoGruInv_Click(object sender, EventArgs e)
        {
            CambiarVista(Vista.NuevoGrupo);
            LimpiarFormularioGrupo();
        }

        protected void btnRegresarGruInv_Click(object sender, EventArgs e)
        {
            Response.Redirect("GruposInvestigacion.aspx");
        }

        protected void lbtCancelarGruInv_Click(object sender, EventArgs e)
        {
            Response.Redirect("GruposInvestigacion.aspx");
        }

        protected void lbnCancellEditGruInv_Click(object sender, EventArgs e)
        {
            Response.Redirect("GruposInvestigacion.aspx");
        }

        protected void lbtADDGruInv_Click(object sender, EventArgs e)
        {
            try
            {
                var g = new InvgccGrupoInvestigacion
                {
                    strNombre_gru = strNombreGru.Text.Trim(),
                    strCoordinador_gru = strNombreCoorGru.Text.Trim(),
                    strFacultad_gru = ddlFacultadGru.SelectedValue,
                    dtFechacrea_gru = DateTime.Parse(dtFechaCreaGru.Text),
                    strCategoria_gru = ddlCatGruInv.SelectedValue,
                    strLineasinv_gru = strLineaInvGru1.SelectedValue,
                    strSublineasinv_gru = strSLineaInvGru1.SelectedValue
                };

                if (fuFotoGrupoAdd.HasFile)
                {
                    string nombre = $"FOTO_{DateTime.Now.Ticks}{Path.GetExtension(fuFotoGrupoAdd.FileName)}";
                    g.strFoto_gru = GuardarArchivoFisico(fuFotoGrupoAdd, "FOTOS", nombre);
                }

                if (flpArchivoAdd.HasFile)
                {
                    string nombre = $"DOC_{DateTime.Now.Ticks}{Path.GetExtension(flpArchivoAdd.FileName)}";
                    g.strArchivo_gru = GuardarArchivoFisico(flpArchivoAdd, "DOCUMENTOS", nombre);
                }

                _manejador.GuardarGrupo(g);
                Redireccionar("Grupo creado correctamente.", "ss");
            }
            catch (Exception ex) { Msg("Error al guardar: " + ex.Message, "ee"); }
        }

        protected void lbnEditGruInv_Click(object sender, EventArgs e)
        {
            try
            {
                var g = new InvgccGrupoInvestigacion
                {
                    strId_gru = hfIdGrupoEdit.Value,
                    strNombre_gru = txtGrupoInvEdit.Text.Trim(),
                    strCoordinador_gru = txtNombreCoorGruInvEdit.Text.Trim(),
                    strFacultad_gru = ddlFacultadGruEdit.SelectedValue,
                    dtFechacrea_gru = DateTime.Parse(dtEditFechaCreaEdit.Text),
                    strCategoria_gru = ddlEditCategoria.SelectedValue,
                    strLineasinv_gru = txtEditLineaIGru1.SelectedValue,
                    strSublineasinv_gru = txtEditSLineaIGru1.SelectedValue,
                    strFoto_gru = hfFotoActual.Value,
                    strArchivo_gru = hfArchivoActual.Value
                };

                if (fuFotoGrupoEdit.HasFile)
                {
                    string nombre = $"FOTO_{DateTime.Now.Ticks}{Path.GetExtension(fuFotoGrupoEdit.FileName)}";
                    g.strFoto_gru = GuardarArchivoFisico(fuFotoGrupoEdit, "FOTOS", nombre);
                }

                if (flpArchivoEdit.HasFile)
                {
                    string nombre = $"DOC_{DateTime.Now.Ticks}{Path.GetExtension(flpArchivoEdit.FileName)}";
                    g.strArchivo_gru = GuardarArchivoFisico(flpArchivoEdit, "DOCUMENTOS", nombre);
                }

                _manejador.ActualizarGrupo(g);
                Redireccionar("Grupo actualizado.", "ss");
            }
            catch (Exception ex) { Msg("Error al actualizar: " + ex.Message, "ee"); }
        }

        protected void rptGrupoInv_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();

            switch (e.CommandName)
            {
                case "VerProyectos":
                    CargarProyectosDeGrupo(id);
                    break;

                case "Eliminar":
                    try
                    {
                        _manejador.EliminarGrupo(id);
                        Redireccionar("Grupo eliminado.", "ss");
                    }
                    catch (Exception ex) { Msg("Error: " + ex.Message, "ee"); }
                    break;

                case "Editar":
                    CargarEdicionGrupo(id);
                    break;

                case "VerIntegrantes":
                    Response.Redirect($"GruposInvestigacion.aspx?idGrupo={id}", false);
                    break;

                case "Archivo":
                    var grupo = _manejador.ObtenerGrupoPorId(id);
                    if (grupo != null && !string.IsNullOrEmpty(grupo.strArchivo_gru))
                    {
                        DescargarArchivo(grupo.strArchivo_gru);
                    }
                    else
                    {
                        Msg("No hay archivo adjunto.", "ww");
                    }
                    break;
            }
        }

        // MÉTODO NUEVO PARA EL MODAL DE DETALLE
        private void CargarProyectosDeGrupo(string idGrupo)
        {
            try
            {
                // 1. Buscar los proyectos de ese grupo
                var listaProyectos = _manejador.ObtenerProyectosDeGrupo(idGrupo);

                // 2. Buscar info del grupo para poner el título bonito
                var infoGrupo = _manejador.ObtenerGrupoPorId(idGrupo);

                // 3. Llenar el GridView del Modal (gvProyectosDetalle)
                // Nota: gvProyectosDetalle debe existir en tu .aspx (lo pusimos en el paso anterior)
                if (gvProyectosDetalle != null)
                {
                    gvProyectosDetalle.DataSource = listaProyectos;
                    gvProyectosDetalle.DataBind();
                }

                // 4. Actualizar título del modal
                if (lblGrupoTitulo != null && infoGrupo != null)
                {
                    lblGrupoTitulo.InnerText = $"PORTAFOLIO: {infoGrupo.strNombre_gru}";
                }

                // 5. Abrir el Modal con JavaScript
                string script = "var m = new bootstrap.Modal(document.getElementById('modalProyectosDetalle')); m.show();";
                ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalProys", script, true);
            }
            catch (Exception ex)
            {
                Msg("Error al cargar detalles: " + ex.Message, "ee");
            }
        }

        private void CargarEdicionGrupo(string id)
        {
            var g = _manejador.ObtenerGrupoPorId(id);
            if (g != null)
            {
                hfIdGrupoEdit.Value = g.strId_gru;
                txtGrupoInvEdit.Text = g.strNombre_gru;
                txtNombreCoorGruInvEdit.Text = g.strCoordinador_gru;
                dtEditFechaCreaEdit.Text = g.dtFechacrea_gru.ToString("yyyy-MM-dd");

                if (ddlFacultadGruEdit.Items.FindByValue(g.strFacultad_gru) != null)
                    ddlFacultadGruEdit.SelectedValue = g.strFacultad_gru;
                else
                    ddlFacultadGruEdit.SelectedIndex = 0;

                if (ddlEditCategoria.Items.FindByValue(g.strCategoria_gru) != null)
                    ddlEditCategoria.SelectedValue = g.strCategoria_gru;

                if (txtEditLineaIGru1.Items.FindByValue(g.strLineasinv_gru) != null)
                    txtEditLineaIGru1.SelectedValue = g.strLineasinv_gru;

                if (txtEditSLineaIGru1.Items.FindByValue(g.strSublineasinv_gru) != null)
                    txtEditSLineaIGru1.SelectedValue = g.strSublineasinv_gru;

                hfFotoActual.Value = g.strFoto_gru;
                hfArchivoActual.Value = g.strArchivo_gru;
                imgFotoGruEdit.ImageUrl = ObtenerImagenBase64(g.strFoto_gru);

                CambiarVista(Vista.EditarGrupo);
            }
        }

        // =============================================
        // GESTIÓN DE INTEGRANTES
        // =============================================

        private void CargarIntegrantesPanel(string idGrupo)
        {
            hfGrupoIdActual.Value = idGrupo;
            CambiarVista(Vista.ListaIntegrantes);
            RefrescarTablaIntegrantes();
        }

        private void RefrescarTablaIntegrantes()
        {
            string idGrupo = hfGrupoIdActual.Value;
            rptIntegrantes.DataSource = _manejador.ObtenerIntegrantes(idGrupo);
            rptIntegrantes.DataBind();
        }

        protected void btnVolverGrupos_Click(object sender, EventArgs e)
        {
            Response.Redirect("GruposInvestigacion.aspx");
        }

        protected void btnNuevoIntegrante_Click(object sender, EventArgs e)
        {
            CambiarVista(Vista.FormularioIntegrante);
            LimpiarFormularioIntegrante();

            ScriptManager.RegisterStartupScript(this, GetType(), "initForm", "InitFormulario();", true);
        }

        protected void btnGuardarInt_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCedulaInt.Text) || string.IsNullOrWhiteSpace(txtNombresInt.Text))
                {
                    Msg("Complete los campos obligatorios.", "ww");
                    return;
                }

                var i = new InvgccGrupoIntegrantes
                {
                    fkId_gru = hfGrupoIdActual.Value,
                    strCedula_int = txtCedulaInt.Text.Trim(),
                    strNombres_int = txtNombresInt.Text.Trim(),
                    strApellidos_int = txtApellidosInt.Text.Trim(),
                    strCorreo_int = txtCorreoInt.Text.Trim(),
                    strFuncion_int = ddlFuncionInt.SelectedValue,
                    strCertificado_int = hfCertificadoIntActual.Value,
                    dtFechaini_int = DateTime.Parse(dtFechaIniInt.Text),
                    strObservacion_int = txtObservacionInt.Text.Trim(),
                    strTipo_int = ddlTipoInt.SelectedValue
                };

                if (i.strFuncion_int == "Investigador Principal")
                {
                    if (flpCertificadoInt.HasFile)
                    {
                        string nombre = $"CERT_{DateTime.Now.Ticks}{Path.GetExtension(flpCertificadoInt.FileName)}";
                        i.strCertificado_int = GuardarArchivoFisico(flpCertificadoInt, "CERTIFICADOS", nombre);
                    }
                    else if (string.IsNullOrEmpty(i.strId_int) && string.IsNullOrEmpty(i.strCertificado_int))
                    {
                    }
                }
                else
                {
                    i.strCertificado_int = null;
                }

                if (i.strTipo_int == "Externo")
                {
                    if (string.IsNullOrWhiteSpace(txtEntidadInt.Text))
                    {
                        Msg("Debe especificar la Institución de Origen.", "ww");
                        return;
                    }
                    i.strEntidad_int = txtEntidadInt.Text.Trim();
                    i.strCarrera_int = null;
                    i.strFacultad_int = null;
                }
                else 
                {
                    i.strEntidad_int = null;
                    i.strCarrera_int = txtCarreraInt.Text.Trim();
                    i.strFacultad_int = ddlFacultadInt.SelectedValue;
                }

                if (string.IsNullOrEmpty(hfIdIntEdit.Value))
                {
                    string usuarioLogueado = Session["UsuarioLogueado"]?.ToString() ?? "Sistema";

                    _manejador.GuardarIntegrante(i, usuarioLogueado);

                    SetFlashMessage("Integrante agregado e historial registrado.", "ss");
                }
                else
                {
                    i.strId_int = hfIdIntEdit.Value;
                    var original = _manejador.ObtenerIntegrantePorId(i.strId_int);
                    if (original != null)
                    {
                        i.dtFechafin_int = original.dtFechafin_int;
                        i.bitActivo_int = original.bitActivo_int;
                        i.bitPertenece_int = original.bitPertenece_int;
                    }
                    _manejador.ActualizarIntegrante(i);
                    SetFlashMessage("Integrante actualizado.", "ss");
                }

                Response.Redirect($"GruposInvestigacion.aspx?idGrupo={hfGrupoIdActual.Value}", false);
            }
            catch (Exception ex) { Msg("Error al guardar integrante: " + ex.Message, "ee"); }
        }

        protected void btnCancelarInt_Click(object sender, EventArgs e)
        {
            CambiarVista(Vista.ListaIntegrantes);
        }

        protected void rptIntegrantes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string idInt = e.CommandArgument.ToString();
            string idGrupo = hfGrupoIdActual.Value;
            string argumento = e.CommandArgument.ToString();

            switch (e.CommandName)
            {
                case "VerCertificado":
                    if (!string.IsNullOrEmpty(argumento))
                    {
                        DescargarArchivo(argumento);
                    }
                    else
                    {
                        Msg("El archivo no se encuentra disponible.", "ww");
                    }
                    break;

                case "EliminarInt":
                    try
                    {
                        _manejador.EliminarIntegranteFisico(idInt);
                        SetFlashMessage("Integrante eliminado permanentemente.", "ss");
                        Response.Redirect($"GruposInvestigacion.aspx?idGrupo={idGrupo}", false);
                    }
                    catch (Exception ex) { Msg(ex.Message, "ee"); }
                    break;

                case "EditarInt":
                    CargarEdicionIntegrante(idInt);
                    break;

                case "CambiarEstado":
                    CargarModalEstado(idInt);
                    break;

                case "Historial":
                    CargarHistorial(idInt);
                    break;
            }
        }

        private void CargarEdicionIntegrante(string idInt)
        {
            var obj = _manejador.ObtenerIntegrantePorId(idInt);
            if (obj != null)
            {
                hfIdIntEdit.Value = obj.strId_int;
                txtCedulaInt.Text = obj.strCedula_int;
                txtNombresInt.Text = obj.strNombres_int;
                txtApellidosInt.Text = obj.strApellidos_int;
                txtCorreoInt.Text = obj.strCorreo_int;
                dtFechaIniInt.Text = obj.dtFechaini_int.ToString("yyyy-MM-dd");
                txtObservacionInt.Text = obj.strObservacion_int;

                if (ddlFuncionInt.Items.FindByValue(obj.strFuncion_int) != null)
                    ddlFuncionInt.SelectedValue = obj.strFuncion_int;

                hfCertificadoIntActual.Value = obj.strCertificado_int;

                // Tipo y Campos Específicos
                if (ddlTipoInt.Items.FindByValue(obj.strTipo_int) != null)
                    ddlTipoInt.SelectedValue = obj.strTipo_int;
                else
                    ddlTipoInt.SelectedIndex = 0;

                if (obj.strTipo_int == "Externo")
                {
                    txtEntidadInt.Text = obj.strEntidad_int;
                    txtCarreraInt.Text = "";
                    ddlFacultadInt.SelectedIndex = 0;
                }
                else
                {
                    txtEntidadInt.Text = "";
                    txtCarreraInt.Text = obj.strCarrera_int;
                    if (ddlFacultadInt.Items.FindByValue(obj.strFacultad_int) != null)
                        ddlFacultadInt.SelectedValue = obj.strFacultad_int;
                }

                lblTituloFormInt.Text = "Editar Integrante";
                CambiarVista(Vista.FormularioIntegrante);
                ScriptManager.RegisterStartupScript(this, GetType(), "initForm", "InitFormulario();", true);
            }
        }

        private void CargarModalEstado(string idInt)
        {
            hfIdIntegranteEstado.Value = idInt;
            var obj = _manejador.ObtenerIntegrantePorId(idInt);
            if (obj != null)
            {
                string estadoStr = obj.bitActivo_int ? "Activo" : "Inactivo";
                string accion = obj.bitActivo_int ? "dar de baja" : "reactivar";

                string script = $@"
                    document.getElementById('txtMotivoEstado').value = '';
                    document.getElementById('infoNombre').innerText = '{obj.strNombres_int} {obj.strApellidos_int}';
                    document.getElementById('infoCedula').innerText = '{obj.strCedula_int}';
                    document.getElementById('infoFuncion').innerText = '{obj.strFuncion_int}';
                    document.getElementById('infoEstado').innerText = '{estadoStr}';
                    document.getElementById('accionEstadoTexto').innerText = '{accion}';
                    AbrirModalEstado();";

                ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalEstado", script, true);
            }
        }

        protected void btnConfirmarCambioEstado_Click(object sender, EventArgs e)
        {
            try
            {
                string idInt = hfIdIntegranteEstado.Value;
                string motivo = hfMotivoEstado.Value;
                string usuario = Session["UsuarioLogueado"]?.ToString() ?? "Usuario Desconocido";

                var obj = _manejador.ObtenerIntegrantePorId(idInt);
                _manejador.CambiarEstadoIntegrante(idInt, !obj.bitActivo_int, motivo, usuario);

                SetFlashMessage("Estado del integrante actualizado.", "ss");
                Response.Redirect($"GruposInvestigacion.aspx?idGrupo={hfGrupoIdActual.Value}", false);
            }
            catch (Exception ex) { Msg("Error al cambiar estado: " + ex.Message, "ee"); }
        }

        private void CargarHistorial(string idInt)
        {
            try
            {
                var integrante = _manejador.ObtenerIntegrantePorId(idInt);
                if (integrante != null)
                {
                    lblNombreHistorial.Text = $"{integrante.strNombres_int} {integrante.strApellidos_int}";
                    hfIdIntegranteHistorial.Value = idInt;
                    rptHistorial.DataSource = _manejador.ObtenerHistorial(idInt);
                    rptHistorial.DataBind();

                    ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalHist", "new bootstrap.Modal(document.getElementById('modalHistorial')).show();", true);
                }
            }
            catch (Exception ex) { Msg("Error al cargar historial: " + ex.Message, "ee"); }
        }

        // =============================================
        // REPORTES Y ARCHIVOS
        // =============================================

        protected void btnGenerarReporte_Click(object sender, EventArgs e)
        {
            string idInt = hfIdIntegranteHistorial.Value;
            if (!string.IsNullOrEmpty(idInt))
            {
                litReporteGenerado.Text = ConstruirHtmlReporte(idInt);
                string script = @"
                    var m1 = bootstrap.Modal.getInstance(document.getElementById('modalHistorial'));
                    if(m1) m1.hide();
                    var m2 = new bootstrap.Modal(document.getElementById('modalVistaPrevia'));
                    m2.show();";
                ScriptManager.RegisterStartupScript(this, GetType(), "SwapModals", script, true);
            }
        }

        private string ConstruirHtmlReporte(string idInt)
        {
            var integrante = _manejador.ObtenerIntegrantePorId(idInt);
            var historial = _manejador.ObtenerHistorial(idInt);
            var grupo = _manejador.ObtenerGrupoPorId(integrante?.fkId_gru);

            if (integrante == null) return "<h4 class='text-danger'>Datos no encontrados</h4>";

            StringBuilder html = new StringBuilder();
            html.Append("<div class='report-header'><img src='https://aplicaciones.utc.edu.ec/sigutc/img/bnUTC.png' width='150' class='mb-3'><br><h4 class='text-primary fw-bold'>REPORTE DE HISTORIAL DE MOVIMIENTOS</h4><small class='text-muted'>Sistema Integrado de Gestión - Dirección de Investigación</small></div>");

            html.Append($@"<div class='info-card'><div class='row'>
                <div class='col-md-6'><strong>INTEGRANTE:</strong> {integrante.strNombres_int} {integrante.strApellidos_int}</div>
                <div class='col-md-6'><strong>CÉDULA:</strong> {integrante.strCedula_int}</div>
                <div class='col-md-12 mt-2'><strong>GRUPO:</strong> {(grupo != null ? grupo.strNombre_gru : "Sin Grupo")}</div>
                <div class='col-md-12 mt-2'><strong>FECHA REPORTE:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</div>
                </div></div>");

            html.Append("<div class='timeline-report'>");
            foreach (var h in historial)
            {
                string color = h.strAccion == "BAJA" ? "#dc3545" : "#198754";
                html.Append($@"<div class='timeline-item'>
                    <div class='timeline-dot' style='background:{color}; box-shadow:0 0 0 2px {color};'></div>
                    <div class='timeline-date'>{h.dtFecha:dd MMM yyyy - HH:mm}</div>
                    <div class='timeline-content'>
                        <div class='d-flex justify-content-between'><span class='timeline-title' style='color:{color}'>{h.strAccion}</span><span class='timeline-user small text-muted'><i class='fa-solid fa-user me-1'></i> {h.strUsuario}</span></div>
                        <p class='mb-0 mt-2 text-secondary'>{h.strMotivo}</p>
                    </div></div>");
            }
            html.Append("</div><div class='text-center mt-5 pt-3 border-top text-muted small'>Documento generado automáticamente por SIG-UTC.</div>");

            return html.ToString();
        }

        private string GuardarArchivoFisico(FileUpload control, string subCarpeta, string nombre)
        {
            string carpeta = Path.Combine(RUTA_BASE_GRUPOS, subCarpeta);
            if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);
            string ruta = Path.Combine(carpeta, nombre);
            control.SaveAs(ruta);
            return ruta;
        }

        private void DescargarArchivo(string rutaDesdeBd)
        {
            if (File.Exists(rutaDesdeBd))
            {
                string nombre = Path.GetFileName(rutaDesdeBd);
                string ext = Path.GetExtension(rutaDesdeBd).ToLower();
                Response.Clear();
                Response.ContentType = ext == ".pdf" ? "application/pdf" : "application/octet-stream";
                Response.AppendHeader("Content-Disposition", "inline; filename=" + nombre);
                Response.TransmitFile(rutaDesdeBd);
                Response.End();
            }
            else { Msg("El archivo físico no existe en la ruta especificada.", "ww"); }
        }

        protected string ObtenerImagenBase64(object rutaObj)
        {
            string ruta = rutaObj as string;
            if (string.IsNullOrEmpty(ruta) || !File.Exists(ruta)) return "img/default-user.png";
            try { return "data:image/jpeg;base64," + Convert.ToBase64String(File.ReadAllBytes(ruta)); }
            catch { return "img/default-user.png"; }
        }

        // =============================================
        // UTILIDADES Y AYUDAS VISUALES
        // =============================================

        private enum Vista { ListaGrupos, NuevoGrupo, EditarGrupo, ListaIntegrantes, FormularioIntegrante }

        private void CambiarVista(Vista vista)
        {
            pnlGrilla.Visible = vista == Vista.ListaGrupos;
            headerGrupos.Visible = vista == Vista.ListaGrupos;

            pnlAgregarGruInv.Visible = vista == Vista.NuevoGrupo;
            pnlEditarGrupoInv.Visible = vista == Vista.EditarGrupo;

            pnlIntegrantes.Visible = vista == Vista.ListaIntegrantes;
            pnlFormularioIntegrante.Visible = vista == Vista.FormularioIntegrante;
        }

        private void LimpiarFormularioGrupo()
        {
            strNombreGru.Text = "";
            strNombreCoorGru.Text = "";
            dtFechaCreaGru.Text = DateTime.Now.ToString("yyyy-MM-dd");
            ddlFacultadGru.SelectedIndex = 0;
            ddlCatGruInv.SelectedIndex = 0;
            strLineaInvGru1.SelectedIndex = 0;
            strSLineaInvGru1.SelectedIndex = 0;
        }

        private void LimpiarFormularioIntegrante()
        {
            hfIdIntEdit.Value = "";
            lblTituloFormInt.Text = "Nuevo Integrante";
            txtCedulaInt.Text = ""; txtNombresInt.Text = ""; txtApellidosInt.Text = "";
            txtCorreoInt.Text = ""; txtCarreraInt.Text = ""; txtEntidadInt.Text = "";
            ddlTipoInt.SelectedIndex = 0; ddlFacultadInt.SelectedIndex = 0;
            dtFechaIniInt.Text = DateTime.Now.ToString("yyyy-MM-dd");
            ddlFuncionInt.SelectedIndex = 0; 
            hfCertificadoIntActual.Value = "";
            txtObservacionInt.Text = "";
        }

        private void Redireccionar(string msg, string type)
        {
            SetFlashMessage(msg, type);
            Response.Redirect("GruposInvestigacion.aspx", false);
        }

        private void SetFlashMessage(string msg, string type)
        {
            Session["TempMsg"] = msg;
            Session["TempTipo"] = type;
        }

        private void Msg(string msg, string type)
        {
            if (string.IsNullOrEmpty(msg)) return;

            string cleanMsg = msg
                .Replace("\\", "\\\\") 
                .Replace("'", "\\'")   
                .Replace("\"", "\\\"") 
                .Replace("\r\n", " ")  
                .Replace("\n", " "); 

            string titulo = type == "ss" ? "Éxito" : (type == "ee" ? "Error" : "Atención");

            string script = $"$(function() {{ toastify('{type}', '{cleanMsg}', '{titulo}'); }});";

            ScriptManager.RegisterStartupScript(this, GetType(), "alert", script, true);
        }
    }
}