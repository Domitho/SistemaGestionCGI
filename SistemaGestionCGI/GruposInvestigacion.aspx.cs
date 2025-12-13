using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;

namespace SistemaGestionCGI
{
    public partial class GruposInvestigacion : System.Web.UI.Page
    {
        private readonly ManejadorGruposInvestigacion _manejador = new ManejadorGruposInvestigacion();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UsuarioLogueado"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                CargarGrillaGrupos();

                string idGrupoRedirect = Request.QueryString["idGrupo"];

                if (!string.IsNullOrEmpty(idGrupoRedirect))
                {
                    CargarIntegrantesPanel(idGrupoRedirect);
                }

                if (Session["TempMsg"] != null)
                {
                    string msg = Session["TempMsg"].ToString();
                    string type = Session["TempTipo"].ToString();
                    Msg(msg, type);

                    Session["TempMsg"] = null;
                    Session["TempTipo"] = null;
                }
            }
        }

        private void CargarGrillaGrupos()
        {
            try
            {
                var lista = _manejador.ObtenerGrupos();
                rptGrupoInv.DataSource = lista;
                rptGrupoInv.DataBind();
            }
            catch (Exception ex)
            {
                Msg("Error al cargar grupos: " + ex.Message, "ee");
            }
        }

        protected void lbtNuevoGruInv_Click(object sender, EventArgs e)
        {
            pnlGrilla.Visible = false;
            pnlAgregarGruInv.Visible = true;
            pnlEditarGrupoInv.Visible = false;
            headerGrupos.Visible = false; 

            strNombreGru.Text = "";
            strNombreCoorGru.Text = "";
            dtFechaCreaGru.Text = DateTime.Now.ToString("yyyy-MM-dd");
            ddlCatGruInv.SelectedIndex = 0;
            strLineaInvGru1.SelectedIndex = 0;
            strSLineaInvGru1.SelectedIndex = 0;
        }

        protected void btnRegresarGruInv_Click(object sender, EventArgs e)
        {
            Response.Redirect("GruposInvestigacion.aspx");
        }

        protected void lbtADDGruInv_Click(object sender, EventArgs e)
        {
            try
            {
                InvgccGrupoInvestigacion g = new InvgccGrupoInvestigacion();
                g.strNombre_gru = strNombreGru.Text.Trim();
                g.strCoordinador_gru = strNombreCoorGru.Text.Trim();
                g.dtFechacrea_gru = DateTime.Parse(dtFechaCreaGru.Text);
                g.strCategoria_gru = ddlCatGruInv.SelectedValue;
                g.strLineasinv_gru = strLineaInvGru1.SelectedValue;
                g.strSublineasinv_gru = strSLineaInvGru1.SelectedValue;

                if (fuFotoGrupoAdd.HasFile)
                {
                    string ext = Path.GetExtension(fuFotoGrupoAdd.FileName);
                    string nombre = "FOTO_" + DateTime.Now.Ticks + ext;
                    g.strFoto_gru = GuardarArchivo(fuFotoGrupoAdd, "FotosGrupos", nombre);
                }

                if (flpArchivoAdd.HasFile)
                {
                    string ext = Path.GetExtension(flpArchivoAdd.FileName);
                    string nombre = "DOC_" + DateTime.Now.Ticks + ext;
                    g.strArchivo_gru = GuardarArchivo(flpArchivoAdd, "DocumentosGrupos", nombre);
                }

                _manejador.GuardarGrupo(g);

                SetFlashMessage("Grupo creado correctamente.", "ss");
                Response.Redirect("GruposInvestigacion.aspx", false);
            }
            catch (Exception ex)
            {
                Msg("Error al guardar: " + ex.Message, "ee");
            }
        }

        protected void lbtCancelarGruInv_Click(object sender, EventArgs e)
        {
            Response.Redirect("GruposInvestigacion.aspx");
        }

        protected void rptGrupoInv_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();

            if (e.CommandName == "Eliminar")
            {
                try
                {
                    _manejador.EliminarGrupo(id);
                    SetFlashMessage("Grupo eliminado.", "ss");
                    Response.Redirect("GruposInvestigacion.aspx", false);
                }
                catch (Exception ex)
                {
                    Msg("Error: " + ex.Message, "ee");
                }
            }
            else if (e.CommandName == "Editar")
            {
                CargarEdicionGrupo(id);
            }
            else if (e.CommandName == "VerIntegrantes")
            {
                Response.Redirect($"GruposInvestigacion.aspx?idGrupo={id}", false);
            }
            else if (e.CommandName == "Archivo")
            {
                var grupo = _manejador.ObtenerGrupoPorId(id);
                if (grupo != null && !string.IsNullOrEmpty(grupo.strArchivo_gru))
                {
                    DescargarArchivo(grupo.strArchivo_gru);
                }
                else
                {
                    Msg("No hay archivo adjunto.", "ww");
                }
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

                if (ddlEditCategoria.Items.FindByValue(g.strCategoria_gru) != null)
                    ddlEditCategoria.SelectedValue = g.strCategoria_gru;

                hfFotoActual.Value = g.strFoto_gru;
                hfArchivoActual.Value = g.strArchivo_gru;

                if (!string.IsNullOrEmpty(g.strFoto_gru))
                    imgFotoGruEdit.ImageUrl = g.strFoto_gru;
                else
                    imgFotoGruEdit.ImageUrl = "~/img/default-user.png";

                pnlGrilla.Visible = false;
                headerGrupos.Visible = false;
                pnlEditarGrupoInv.Visible = true;
            }
        }

        protected void lbnEditGruInv_Click(object sender, EventArgs e)
        {
            try
            {
                InvgccGrupoInvestigacion g = new InvgccGrupoInvestigacion();
                g.strId_gru = hfIdGrupoEdit.Value;
                g.strNombre_gru = txtGrupoInvEdit.Text.Trim();
                g.strCoordinador_gru = txtNombreCoorGruInvEdit.Text.Trim();
                g.dtFechacrea_gru = DateTime.Parse(dtEditFechaCreaEdit.Text);
                g.strCategoria_gru = ddlEditCategoria.SelectedValue;
                g.strLineasinv_gru = txtEditLineaIGru1.SelectedValue;
                g.strSublineasinv_gru = txtEditSLineaIGru1.SelectedValue;

                g.strFoto_gru = hfFotoActual.Value;
                g.strArchivo_gru = hfArchivoActual.Value;

                if (fuFotoGrupoEdit.HasFile)
                {
                    string nombre = "FOTO_" + DateTime.Now.Ticks + Path.GetExtension(fuFotoGrupoEdit.FileName);
                    g.strFoto_gru = GuardarArchivo(fuFotoGrupoEdit, "FotosGrupos", nombre);
                }

                if (flpArchivoEdit.HasFile)
                {
                    string nombre = "DOC_" + DateTime.Now.Ticks + Path.GetExtension(flpArchivoEdit.FileName);
                    g.strArchivo_gru = GuardarArchivo(flpArchivoEdit, "DocumentosGrupos", nombre);
                }

                _manejador.ActualizarGrupo(g);
                SetFlashMessage("Grupo actualizado.", "ss");
                Response.Redirect("GruposInvestigacion.aspx", false);
            }
            catch (Exception ex)
            {
                Msg("Error al actualizar: " + ex.Message, "ee");
            }
        }

        protected void lbnCancellEditGruInv_Click(object sender, EventArgs e)
        {
            Response.Redirect("GruposInvestigacion.aspx");
        }

        private void CargarIntegrantesPanel(string idGrupo)
        {
            hfGrupoIdActual.Value = idGrupo;

            pnlGrilla.Visible = false;
            headerGrupos.Visible = false;
            pnlAgregarGruInv.Visible = false;
            pnlEditarGrupoInv.Visible = false;

            pnlIntegrantes.Visible = true;
            pnlFormularioIntegrante.Visible = false;

            RefrescarTablaIntegrantes();
        }

        private void RefrescarTablaIntegrantes()
        {
            string idGrupo = hfGrupoIdActual.Value;
            var lista = _manejador.ObtenerIntegrantes(idGrupo);
            rptIntegrantes.DataSource = lista;
            rptIntegrantes.DataBind();
        }

        protected void btnVolverGrupos_Click(object sender, EventArgs e)
        {
            Response.Redirect("GruposInvestigacion.aspx");
        }

        protected void btnNuevoIntegrante_Click(object sender, EventArgs e)
        {
            pnlIntegrantes.Visible = false;
            pnlFormularioIntegrante.Visible = true;
            lblTituloFormInt.Text = "Nuevo Integrante";
            hfIdIntEdit.Value = "";

            txtCedulaInt.Text = "";
            txtNombresInt.Text = "";
            txtApellidosInt.Text = "";
            txtCorreoInt.Text = "";
            txtCarreraInt.Text = "";
            dtFechaIniInt.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtObservacionInt.Text = "";
        }

        protected void btnGuardarInt_Click(object sender, EventArgs e)
        {
            try
            {
                InvgccGrupoIntegrantes i = new InvgccGrupoIntegrantes();
                i.fkId_gru = hfGrupoIdActual.Value;
                i.strCedula_int = txtCedulaInt.Text.Trim();
                i.strNombres_int = txtNombresInt.Text.Trim();
                i.strApellidos_int = txtApellidosInt.Text.Trim();
                i.strCorreo_int = txtCorreoInt.Text.Trim();
                i.strCarrera_int = txtCarreraInt.Text.Trim();
                i.strFuncion_int = ddlFuncionInt.SelectedValue;
                i.dtFechaini_int = DateTime.Parse(dtFechaIniInt.Text);
                i.strObservacion_int = txtObservacionInt.Text.Trim(); 

                if (string.IsNullOrEmpty(hfIdIntEdit.Value))
                {
                    _manejador.GuardarIntegrante(i);
                    SetFlashMessage("Integrante agregado.", "ss");
                }
                else
                {
                    i.strId_int = hfIdIntEdit.Value;

                    var integranteOriginal = _manejador.ObtenerIntegrantePorId(i.strId_int);

                    if (integranteOriginal != null)
                    {
                        i.dtFechafin_int = integranteOriginal.dtFechafin_int;
                        i.bitActivo_int = integranteOriginal.bitActivo_int;
                    }

                    _manejador.ActualizarIntegrante(i);
                    SetFlashMessage("Integrante actualizado.", "ss");
                }

                Response.Redirect($"GruposInvestigacion.aspx?idGrupo={hfGrupoIdActual.Value}", false);
            }
            catch (Exception ex)
            {
                Msg("Error al guardar integrante: " + ex.Message, "ee");
            }
        }

        protected void btnCancelarInt_Click(object sender, EventArgs e)
        {
            pnlFormularioIntegrante.Visible = false;
            pnlIntegrantes.Visible = true;
        }

        protected void rptIntegrantes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string idInt = e.CommandArgument.ToString();
            string idGrupo = hfGrupoIdActual.Value;

            if (e.CommandName == "EliminarInt")
            {
                try
                {
                    _manejador.EliminarIntegranteFisico(idInt);
                    SetFlashMessage("Integrante eliminado permanentemente.", "ss");
                    Response.Redirect($"GruposInvestigacion.aspx?idGrupo={idGrupo}", false);
                }
                catch (Exception ex) { Msg(ex.Message, "ee"); }
            }
            else if (e.CommandName == "EditarInt")
            {
                var obj = _manejador.ObtenerIntegrantePorId(idInt);
                if (obj != null)
                {
                    hfIdIntEdit.Value = obj.strId_int;
                    txtCedulaInt.Text = obj.strCedula_int;
                    txtNombresInt.Text = obj.strNombres_int;
                    txtApellidosInt.Text = obj.strApellidos_int;
                    txtCorreoInt.Text = obj.strCorreo_int;
                    txtCarreraInt.Text = obj.strCarrera_int;
                    dtFechaIniInt.Text = obj.dtFechaini_int.ToString("yyyy-MM-dd");
                    txtObservacionInt.Text = obj.strObservacion_int;

                    if (ddlFuncionInt.Items.FindByValue(obj.strFuncion_int) != null)
                        ddlFuncionInt.SelectedValue = obj.strFuncion_int;

                    lblTituloFormInt.Text = "Editar Integrante";
                    pnlIntegrantes.Visible = false;
                    pnlFormularioIntegrante.Visible = true;
                }
            }
            else if (e.CommandName == "CambiarEstado")
            {
                hfIdIntegranteEstado.Value = idInt;
                var obj = _manejador.ObtenerIntegrantePorId(idInt);
                if (obj != null)
                {
                    string scriptLimpiar = "document.getElementById('txtMotivoEstado').value = '';";

                    string estadoStr = obj.bitActivo_int ? "Activo" : "Inactivo";

                    string scriptLlenar = $@"
                        document.getElementById('infoNombre').innerText = '{obj.strNombres_int} {obj.strApellidos_int}';
                        document.getElementById('infoCedula').innerText = '{obj.strCedula_int}';
                        document.getElementById('infoFuncion').innerText = '{obj.strFuncion_int}';
                        document.getElementById('infoEstado').innerText = '{estadoStr}';
                        document.getElementById('accionEstadoTexto').innerText = '{(obj.bitActivo_int ? "dar de baja" : "reactivar")}';
                        AbrirModalEstado();";

                    ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalEstado", scriptLimpiar + scriptLlenar, true);
                }
            }
            else if (e.CommandName == "Historial")
            {
                try
                {
                    var integrante = _manejador.ObtenerIntegrantePorId(idInt);
                    if (integrante != null)
                    {
                        lblNombreHistorial.Text = $"{integrante.strNombres_int} {integrante.strApellidos_int}";
                        hfIdIntegranteHistorial.Value = idInt; 

                        var listaHistorial = _manejador.ObtenerHistorial(idInt);

                        rptHistorial.DataSource = listaHistorial;
                        rptHistorial.DataBind();

                        ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalHist",
                            "new bootstrap.Modal(document.getElementById('modalHistorial')).show();", true);
                    }
                }
                catch (Exception ex)
                {
                    Msg("Error al cargar el historial: " + ex.Message, "ee");
                }
            }
        }

        protected void btnConfirmarCambioEstado_Click(object sender, EventArgs e)
        {
            try
            {
                string idInt = hfIdIntegranteEstado.Value;
                string motivo = hfMotivoEstado.Value;

                string usuario = "Usuario Desconocido";
                if (Session["UsuarioLogueado"] != null)
                {
                    usuario = Session["UsuarioLogueado"].ToString();
                }

                var obj = _manejador.ObtenerIntegrantePorId(idInt);
                bool nuevoEstado = !obj.bitActivo_int;

                _manejador.CambiarEstadoIntegrante(idInt, nuevoEstado, motivo, usuario);

                SetFlashMessage("Estado del integrante actualizado.", "ss");
                Response.Redirect($"GruposInvestigacion.aspx?idGrupo={hfGrupoIdActual.Value}", false);
            }
            catch (Exception ex)
            {
                Msg("Error al cambiar estado: " + ex.Message, "ee");
            }
        }

        private string GuardarArchivo(FileUpload control, string tipo, string nombre)
        {
            string subCarpeta = tipo.Contains("Foto") ? "FOTOS" : "DOCUMENTOS";
            string carpetaFisica = Path.Combine(@"C:\UTC\GRUPOS\", subCarpeta);
            if (!Directory.Exists(carpetaFisica))
                Directory.CreateDirectory(carpetaFisica);
            string rutaCompleta = Path.Combine(carpetaFisica, nombre);
            control.SaveAs(rutaCompleta);

            return rutaCompleta;
        }

        private void DescargarArchivo(string rutaVirtual)
        {
            string rutaFisica = Server.MapPath(rutaVirtual);
            if (File.Exists(rutaFisica))
            {
                string nombreArchivo = Path.GetFileName(rutaFisica);
                string extension = Path.GetExtension(rutaFisica).ToLower();

                Response.Clear();
                Response.ContentType = extension == ".pdf" ? "application/pdf" : "application/octet-stream";
                Response.AppendHeader("Content-Disposition", "inline; filename=" + nombreArchivo);
                Response.TransmitFile(rutaFisica);
                Response.End();
            }
            else
            {
                Msg("El archivo solicitado no existe en el servidor.", "ww");
            }
        }

        // --- NUEVO EVENTO: Generar Reporte desde el servidor ---
        protected void btnGenerarReporte_Click(object sender, EventArgs e)
        {
            string idInt = hfIdIntegranteHistorial.Value;
            if (!string.IsNullOrEmpty(idInt))
            {
                // 1. Construir el HTML
                string htmlReporte = ConstruirHtmlReporte(idInt);

                // 2. Inyectarlo en el Literal del Modal Grande
                litReporteGenerado.Text = htmlReporte;

                // 3. Cerrar Modal Pequeño y Abrir Grande
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

            string nombreCompleto = $"{integrante.strNombres_int} {integrante.strApellidos_int}";
            string nombreGrupo = grupo != null ? grupo.strNombre_gru : "Sin Grupo";

            System.Text.StringBuilder html = new System.Text.StringBuilder();

            // Encabezado
            html.Append(@"
                <div class='report-header'>
                    <img src='https://aplicaciones.utc.edu.ec/sigutc/img/bnUTC.png' width='150' class='mb-3'><br>
                    <h4 class='text-primary fw-bold'>REPORTE DE HISTORIAL DE MOVIMIENTOS</h4>
                    <small class='text-muted'>Sistema Integrado de Gestión - Dirección de Investigación</small>
                </div>");

            // Info Card
            html.Append($@"
                <div class='info-card'>
                    <div class='row'>
                        <div class='col-md-6'><strong>INTEGRANTE:</strong> {nombreCompleto}</div>
                        <div class='col-md-6'><strong>CÉDULA:</strong> {integrante.strCedula_int}</div>
                        <div class='col-md-12 mt-2'><strong>GRUPO:</strong> {nombreGrupo}</div>
                        <div class='col-md-12 mt-2'><strong>FECHA REPORTE:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</div>
                    </div>
                </div>");

            // Timeline
            html.Append("<div class='timeline-report'>");

            foreach (var h in historial)
            {
                string color = h.strAccion == "BAJA" ? "#dc3545" : "#198754";
                html.Append($@"
                    <div class='timeline-item'>
                        <div class='timeline-dot' style='background:{color}; box-shadow:0 0 0 2px {color};'></div>
                        <div class='timeline-date'>{h.dtFecha:dd MMM yyyy - HH:mm}</div>
                        <div class='timeline-content'>
                            <div class='d-flex justify-content-between'>
                                <span class='timeline-title' style='color:{color}'>{h.strAccion}</span>
                                <span class='timeline-user small text-muted'><i class='fa-solid fa-user me-1'></i> {h.strUsuario}</span>
                            </div>
                            <p class='mb-0 mt-2 text-secondary'>{h.strMotivo}</p>
                        </div>
                    </div>");
            }

            html.Append("</div>"); // Fin Timeline

            // Footer
            html.Append(@"
                <div class='text-center mt-5 pt-3 border-top text-muted small'>
                    Documento generado automáticamente por SIG-UTC.
                </div>");

            return html.ToString();
        }

        private void SetFlashMessage(string msg, string type)
        {
            Session["TempMsg"] = msg;
            Session["TempTipo"] = type;
        }

        private void Msg(string msg, string type)
        {
            if (string.IsNullOrEmpty(msg)) return;

            string cleanMsg = msg.Replace("'", "\\'");

            cleanMsg = cleanMsg.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");

            cleanMsg = cleanMsg.Replace("\\", "\\\\");

            string script = $"$(function() {{ toastify('{type}', '{cleanMsg}', 'Sistema'); }});";
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", script, true);
        }
    }
}