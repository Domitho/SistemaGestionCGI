using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;

namespace SistemaGestionCGI
{
    public partial class EjecucionProAprobados : System.Web.UI.Page
    {
        // ==========================================
        // 1. INSTANCIAS Y VARIABLES GLOBALES
        // ==========================================
        private readonly ManejadorEjecucionProyectos _manejador = new ManejadorEjecucionProyectos();
        private readonly ManejadorInscripcionProyectos _manejadorProyectos = new ManejadorInscripcionProyectos();
        private const string RUTA_BASE_ARCHIVOS = @"C:\UTC\EJECUCION_INFORMES\";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // A. Validar Sesión
                if (Session["UsuarioLogueado"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                // B. Cargar Datos Iniciales
                CargarGrillaEjecucion();

                // C. Verificar redirección de equipo
                string idTeamRedirect = Request.QueryString["idTeam"];
                if (!string.IsNullOrEmpty(idTeamRedirect) && int.TryParse(idTeamRedirect, out int idTeam))
                {
                    CargarEquipo(idTeam);
                }

                // D. Mostrar Mensajes Flash
                if (Session["TempMsg"] != null)
                {
                    Msg(Session["TempMsg"].ToString(), Session["TempTipo"].ToString());
                    Session["TempMsg"] = null;
                    Session["TempTipo"] = null;
                }
            }
        }

        // ==========================================
        // 2. GESTIÓN PRINCIPAL (PROYECTOS)
        // ==========================================

        private void CargarGrillaEjecucion()
        {
            try
            {
                rptEjecucion.DataSource = _manejador.ObtenerEjecuciones();
                rptEjecucion.DataBind();
            }
            catch (Exception ex) { Msg("Error al cargar ejecuciones: " + ex.Message, "ee"); }
        }

        private void CargarProyectosAprobados()
        {
            try
            {
                ddlProyectosAprobados.DataSource = _manejador.ObtenerProyectosAprobadosSinEjecucion();
                ddlProyectosAprobados.DataTextField = "strTema_pro";
                ddlProyectosAprobados.DataValueField = "strId_pro";
                ddlProyectosAprobados.DataBind();
                ddlProyectosAprobados.Items.Insert(0, new ListItem("-- Seleccione Proyecto --", ""));
            }
            catch (Exception ex) { Msg("Error al cargar proyectos: " + ex.Message, "ee"); }
        }

        protected void btnNuevoEjecucion_Click(object sender, EventArgs e)
        {
            pnlGrilla.Visible = false;
            pnlAgregar.Visible = true;
            headerEjecucion.Visible = true;
            btnNuevoEjecucion.Visible = false;
            btnRegresar.Visible = true;

            CargarProyectosAprobados();
            txtCoordinadorAdd.Text = "";
            txtFechaIniAdd.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtPeriodoAdd.Text = "";
        }

        protected void ddlProyectosAprobados_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlProyectosAprobados.SelectedValue))
            {
                var pro = _manejadorProyectos.ObtenerPorId(ddlProyectosAprobados.SelectedValue);
                if (pro != null) txtCoordinadorAdd.Text = pro.strCoordinador_pro;
            }
        }

        protected void btnGuardarNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ddlProyectosAprobados.SelectedValue))
                {
                    Msg("Debe seleccionar un proyecto aprobado.", "ww");
                    return;
                }

                var obj = new InvgccEjecucionProyectos
                {
                    fkId_pro = ddlProyectosAprobados.SelectedValue,
                    strCoordinador_ejec = txtCoordinadorAdd.Text.Trim(),
                    strPeriodo_ejec = txtPeriodoAdd.Text.Trim(),
                    dtFechaini_ejec = DateTime.Parse(txtFechaIniAdd.Text),
                    dtFechafin_ejec = null
                };

                if (flpArchivoAdd.HasFile)
                {
                    string nombre = "PLAN_" + DateTime.Now.Ticks + Path.GetExtension(flpArchivoAdd.FileName);
                    obj.strInforme_ejec = GuardarArchivoFisico(flpArchivoAdd, nombre);
                }

                _manejador.GuardarEjecucion(obj);
                Redireccionar("Ejecución iniciada correctamente.", "ss");
            }
            catch (Exception ex) { Msg("Error al guardar: " + ex.Message, "ee"); }
        }

        protected void btnGuardarEdit_Click(object sender, EventArgs e)
        {
            try
            {
                var obj = new InvgccEjecucionProyectos
                {
                    strId_ejec = int.Parse(hfIdEjecEdit.Value),
                    strCoordinador_ejec = txtCoordinadorEdit.Text.Trim(),
                    dtFechaini_ejec = DateTime.Parse(txtFechaIniEdit.Text),
                    strPeriodo_ejec = txtPeriodoEdit.Text.Trim(),
                    strInforme_ejec = hfArchivoActual.Value,
                    dtFechafin_ejec = string.IsNullOrEmpty(txtFechaFinEdit.Text) ? (DateTime?)null : DateTime.Parse(txtFechaFinEdit.Text)
                };

                if (flpArchivoEdit.HasFile)
                {
                    string nombre = "PLAN_" + DateTime.Now.Ticks + Path.GetExtension(flpArchivoEdit.FileName);
                    obj.strInforme_ejec = GuardarArchivoFisico(flpArchivoEdit, nombre);
                }

                _manejador.ActualizarEjecucion(obj);
                Redireccionar("Datos actualizados correctamente.", "ss");
            }
            catch (Exception ex) { Msg("Error al actualizar: " + ex.Message, "ee"); }
        }

        protected void rptEjecucion_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());

            switch (e.CommandName)
            {
                case "Editar":
                    CargarEdicion(id);
                    break;
                case "Equipo":
                    CargarEquipo(id);
                    break;
                case "Informes":
                    hfIdEjecucionInforme.Value = id.ToString();
                    CargarInformes(id);
                    ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalInf", "AbrirModalInformes();", true);
                    break;
                case "Eliminar":
                    try
                    {
                        _manejador.EliminarEjecucion(id);
                        Redireccionar("Registro eliminado correctamente.", "ss");
                    }
                    catch (Exception ex) { Msg("Error al eliminar: " + ex.Message, "ee"); }
                    break;
            }
        }

        private void CargarEdicion(int id)
        {
            var obj = _manejador.ObtenerEjecucionPorId(id);
            if (obj != null)
            {
                hfIdEjecEdit.Value = obj.strId_ejec.ToString();
                txtProyectoReadOnly.Text = obj.TituloProyecto;
                txtCoordinadorEdit.Text = obj.strCoordinador_ejec;
                txtFechaIniEdit.Text = obj.dtFechaini_ejec.ToString("yyyy-MM-dd");
                txtFechaFinEdit.Text = obj.dtFechafin_ejec?.ToString("yyyy-MM-dd") ?? "";
                txtPeriodoEdit.Text = obj.strPeriodo_ejec;
                hfArchivoActual.Value = obj.strInforme_ejec;

                pnlGrilla.Visible = false;
                pnlAgregar.Visible = false;
                pnlEditar.Visible = true;
                headerEjecucion.Visible = true;
                btnNuevoEjecucion.Visible = false;
                btnRegresar.Visible = true;
            }
        }

        // ==========================================
        // 3. NAVEGACIÓN Y BOTONES CANCELAR (RESTAURADOS)
        // ==========================================

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            pnlAgregar.Visible = false;
            pnlEditar.Visible = false;
            pnlEquipoListado.Visible = false;
            pnlFormularioMiembro.Visible = false;

            pnlGrilla.Visible = true;
            headerEjecucion.Visible = true;
            btnNuevoEjecucion.Visible = true;
            btnRegresar.Visible = false;

            CargarGrillaEjecucion();
        }

        // ESTOS ERAN LOS MÉTODOS QUE FALTABAN:
        protected void btnCancelarNew_Click(object sender, EventArgs e)
        {
            btnRegresar_Click(sender, e);
        }

        protected void btnCancelarEdit_Click(object sender, EventArgs e)
        {
            btnRegresar_Click(sender, e);
        }

        // ==========================================
        // 4. GESTIÓN DE EQUIPO
        // ==========================================

        private void CargarEquipo(int idEjecucion)
        {
            hfIdEjecucionEquipo.Value = idEjecucion.ToString();
            pnlGrilla.Visible = false;
            headerEjecucion.Visible = false;
            pnlEquipoListado.Visible = true;
            pnlFormularioMiembro.Visible = false;
            RefrescarTablaMiembros();
        }

        private void RefrescarTablaMiembros()
        {
            if (int.TryParse(hfIdEjecucionEquipo.Value, out int id))
            {
                rptMiembros.DataSource = _manejador.ObtenerMiembros(id);
                rptMiembros.DataBind();
            }
        }

        protected void btnAbrirFormMiembro_Click(object sender, EventArgs e)
        {
            pnlEquipoListado.Visible = false;
            pnlFormularioMiembro.Visible = true;
            hfIdMiembroEdit.Value = "";
            lblTituloFormMiembro.Text = "Nuevo Integrante";

            // Limpiar campos
            txtCedulaMiembro.Text = "";
            txtNombresMiembro.Text = "";
            txtApellidosMiembro.Text = "";
            ddlRolMiembro.SelectedIndex = 0;
            ddlFacultadMiembro.SelectedIndex = 0;
        }

        protected void btnGuardarMiembro_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCedulaMiembro.Text) || string.IsNullOrWhiteSpace(txtNombresMiembro.Text))
                {
                    Msg("Complete los campos obligatorios.", "ww");
                    return;
                }

                var m = new InvgccEjecucionMiembros
                {
                    fkId_ejec = int.Parse(hfIdEjecucionEquipo.Value),
                    strCedula_miembro = txtCedulaMiembro.Text.Trim(),
                    strNombres_miembro = txtNombresMiembro.Text.Trim(),
                    strApellidos_miembro = txtApellidosMiembro.Text.Trim(),
                    strRol_miembro = ddlRolMiembro.SelectedValue,
                    strFacultad_miembro = ddlFacultadMiembro.SelectedValue
                };

                if (string.IsNullOrEmpty(hfIdMiembroEdit.Value))
                {
                    _manejador.GuardarMiembro(m);
                    SetFlashMessage("Integrante agregado.", "ss");
                }
                else
                {
                    m.strId_miembro = int.Parse(hfIdMiembroEdit.Value);
                    _manejador.ActualizarMiembro(m);
                    SetFlashMessage("Integrante actualizado.", "ss");
                }

                Response.Redirect($"EjecucionProAprobados.aspx?idTeam={m.fkId_ejec}", false);
            }
            catch (Exception ex) { Msg("Error al guardar miembro: " + ex.Message, "ee"); }
        }

        protected void btnCancelarMiembro_Click(object sender, EventArgs e)
        {
            pnlFormularioMiembro.Visible = false;
            pnlEquipoListado.Visible = true;
            RefrescarTablaMiembros();
        }

        protected void rptMiembros_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int idMiembro = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "CambiarEstado")
            {
                var m = _manejador.ObtenerMiembroPorId(idMiembro);
                if (m != null)
                {
                    hfIdMiembroEstado.Value = idMiembro.ToString();
                    string nombre = $"{m.strNombres_miembro} {m.strApellidos_miembro}";
                    string script = $@"
                        document.getElementById('lblNombreMiembroEstado').innerText = '{nombre}';
                        document.getElementById('lblRolMiembroEstado').innerText = '{m.strRol_miembro}';
                        document.getElementById('txtMotivoCambio').value = ''; 
                        new bootstrap.Modal(document.getElementById('modalEstadoMiembro')).show();";
                    ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalEstado", script, true);
                }
            }
            else if (e.CommandName == "VerHistorial")
            {
                var m = _manejador.ObtenerMiembroPorId(idMiembro);
                if (m != null)
                {
                    lblNombreHistorial.Text = $"{m.strNombres_miembro} {m.strApellidos_miembro}";
                    hfIdMiembroEstado.Value = idMiembro.ToString();
                    rptHistorialMiembro.DataSource = _manejador.ObtenerHistorialMiembro(idMiembro);
                    rptHistorialMiembro.DataBind();
                    ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalHist", "new bootstrap.Modal(document.getElementById('modalHistorialMiembro')).show();", true);
                }
            }
            else if (e.CommandName == "EliminarMiembro")
            {
                try
                {
                    _manejador.EliminarMiembro(idMiembro);
                    SetFlashMessage("Integrante eliminado.", "ss");
                    Response.Redirect($"EjecucionProAprobados.aspx?idTeam={hfIdEjecucionEquipo.Value}", false);
                }
                catch (Exception ex) { Msg("Error al eliminar: " + ex.Message, "ee"); }
            }
            else if (e.CommandName == "EditarMiembro")
            {
                var m = _manejador.ObtenerMiembroPorId(idMiembro);
                if (m != null)
                {
                    hfIdMiembroEdit.Value = m.strId_miembro.ToString();
                    txtCedulaMiembro.Text = m.strCedula_miembro;
                    txtNombresMiembro.Text = m.strNombres_miembro;
                    txtApellidosMiembro.Text = m.strApellidos_miembro;

                    if (ddlRolMiembro.Items.FindByValue(m.strRol_miembro) != null)
                        ddlRolMiembro.SelectedValue = m.strRol_miembro;

                    if (ddlFacultadMiembro.Items.FindByValue(m.strFacultad_miembro) != null)
                        ddlFacultadMiembro.SelectedValue = m.strFacultad_miembro;

                    lblTituloFormMiembro.Text = "Editar Integrante";
                    pnlEquipoListado.Visible = false;
                    pnlFormularioMiembro.Visible = true;
                }
            }
        }

        protected void btnConfirmarEstado_Click(object sender, EventArgs e)
        {
            try
            {
                int idMiembro = int.Parse(hfIdMiembroEstado.Value);
                string motivo = hfMotivoHidden.Value;
                string usuario = Session["UsuarioLogueado"]?.ToString() ?? "SISTEMA";

                var m = _manejador.ObtenerMiembroPorId(idMiembro);
                _manejador.CambiarEstadoMiembro(idMiembro, !m.bitActivo_miembro, motivo, usuario);

                Msg("Estado actualizado correctamente.", "ss");
                RefrescarTablaMiembros();
            }
            catch (Exception ex) { Msg("Error: " + ex.Message, "ee"); }
        }

        protected void btnVolverDeEquipo_Click(object sender, EventArgs e)
        {
            Response.Redirect("EjecucionProAprobados.aspx");
        }

        // ==========================================
        // 5. GESTIÓN DE INFORMES
        // ==========================================

        private void CargarInformes(int idEjecucion)
        {
            rptInformes.DataSource = _manejador.ObtenerInformes(idEjecucion);
            rptInformes.DataBind();
        }

        protected void rptInformes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int idInforme = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "EliminarInforme")
            {
                try
                {
                    _manejador.EliminarInforme(idInforme);
                    Redireccionar("Documento eliminado correctamente.", "ss");
                }
                catch (Exception ex) { Msg("Error al eliminar: " + ex.Message, "ee"); }
            }
            else if (e.CommandName == "EditarInforme")
            {
                var informe = _manejador.ObtenerInformePorId(idInforme);
                if (informe != null)
                {
                    hfIdInformeEdit.Value = informe.strId_informe.ToString();
                    txtNombrePeriodoInf.Text = informe.strNombrePeriodo;
                    lblTituloModalInforme.InnerText = "Editar / Corregir Informe";
                    ScriptManager.RegisterStartupScript(this, GetType(), "OpenModal", "AbrirSubModalUpload();", true);
                }
            }
        }

        protected void btnGuardarInforme_Click(object sender, EventArgs e)
        {
            try
            {
                // Validación de extensión
                if (flpArchivoInf.HasFile)
                {
                    string ext = Path.GetExtension(flpArchivoInf.FileName).ToLower();
                    if (ext != ".doc" && ext != ".docx")
                    {
                        Msg("Solo se permiten archivos Word (.doc, .docx).", "ww");
                        ScriptManager.RegisterStartupScript(this, GetType(), "Reopen", "AbrirSubModalUpload();", true);
                        return;
                    }
                }

                if (!int.TryParse(hfIdEjecucionInforme.Value, out int idEjec)) return;

                var inf = new InvgccEjecucionInformes
                {
                    fkId_ejec = idEjec,
                    strNombrePeriodo = string.IsNullOrEmpty(txtNombrePeriodoInf.Text) ? "Informe de Avance" : txtNombrePeriodoInf.Text.Trim(),
                    strArchivo_path = flpArchivoInf.HasFile ? GuardarArchivoFisico(flpArchivoInf, $"INF_{DateTime.Now.Ticks}{Path.GetExtension(flpArchivoInf.FileName)}") : ""
                };

                if (string.IsNullOrEmpty(hfIdInformeEdit.Value))
                {
                    if (!flpArchivoInf.HasFile)
                    {
                        Msg("Debe seleccionar un archivo Word.", "ww");
                        ScriptManager.RegisterStartupScript(this, GetType(), "Reopen", "AbrirSubModalUpload();", true);
                        return;
                    }
                    _manejador.GuardarInforme(inf);
                    Redireccionar("Informe subido correctamente.", "ss");
                }
                else
                {
                    inf.strId_informe = int.Parse(hfIdInformeEdit.Value);
                    _manejador.ActualizarInforme(inf);
                    Redireccionar("Informe corregido correctamente.", "ss");
                }
            }
            catch (Exception ex) { Msg("Error: " + ex.Message, "ee"); }
        }

        // ==========================================
        // 6. GENERACIÓN DE REPORTES (HTML)
        // ==========================================

        protected void btnGenerarReporteHistorial_Click(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(hfIdMiembroEstado.Value, out int idMiembro))
                {
                    litReporteGenerado.Text = ConstruirReporteHistorial(idMiembro);
                    pnlReporteHtml.Visible = true;
                    btnImprimirReporte.Style["display"] = "inline-block";
                    lblTituloPreview.InnerText = "Reporte Oficial de Movimientos";

                    string script = @"
                        document.getElementById('framePdf').style.display = 'none';
                        document.getElementById('btnDescargarDirecto').style.display = 'none';
                        var mHist = bootstrap.Modal.getInstance(document.getElementById('modalHistorialMiembro'));
                        if(mHist) mHist.hide();
                        new bootstrap.Modal(document.getElementById('modalVistaPrevia')).show();";
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowReport", script, true);
                }
                else
                {
                    Msg("Error al identificar al integrante.", "ww");
                }
            }
            catch (Exception ex) { Msg("Error al generar reporte: " + ex.Message, "ee"); }
        }

        private string ConstruirReporteHistorial(int idMiembro)
        {
            var miembro = _manejador.ObtenerMiembroPorId(idMiembro);
            var historial = _manejador.ObtenerHistorialMiembro(idMiembro);
            var ejecucion = _manejador.ObtenerEjecucionPorId(miembro.fkId_ejec);

            StringBuilder sb = new StringBuilder();
            sb.Append("<style>.rep-header{text-align:center;border-bottom:2px solid #312783;padding-bottom:15px}.rep-logo{width:200px}.rep-title{color:#312783;font-size:20px;font-weight:bold;text-transform:uppercase}.rep-card{background:#f8f9fa;border:1px solid #ddd;padding:20px;margin:20px 0}.rep-label{font-weight:bold;color:#312783}.rep-timeline{padding-left:30px;border-left:2px solid #e0e0e0}.rep-item{margin-bottom:25px;position:relative}.rep-dot{width:16px;height:16px;background:#fff;border:3px solid #312783;border-radius:50%;position:absolute;left:-39px;top:0}.rep-date{font-size:13px;color:#999;font-weight:600}.badge-baja{color:#dc3545}.badge-alta{color:#198754}</style>");

            sb.Append("<div class='rep-header'><img src='https://aplicaciones.utc.edu.ec/sigutc/img/bnUTC.png' class='rep-logo'><br><h3 class='rep-title'>Historial de Movimientos</h3></div>");
            sb.Append("<div class='rep-card'><div class='row'>");
            sb.Append($"<div class='col-6 mb-2'><span class='rep-label'>NOMBRE:</span> {miembro.strNombres_miembro} {miembro.strApellidos_miembro}</div>");
            sb.Append($"<div class='col-6 mb-2'><span class='rep-label'>CÉDULA:</span> {miembro.strCedula_miembro}</div>");
            sb.Append($"<div class='col-6 mb-2'><span class='rep-label'>ROL:</span> {miembro.strRol_miembro}</div>");
            sb.Append($"<div class='col-12 border-top pt-2 mt-2'><span class='rep-label'>PROYECTO:</span> {ejecucion.TituloProyecto}</div></div></div>");

            sb.Append("<div class='p-3'><h5 class='mb-4'>Línea de Tiempo</h5><div class='rep-timeline'>");
            foreach (var h in historial)
            {
                string color = h.strAccion == "BAJA" ? "#dc3545" : "#198754";
                sb.Append($"<div class='rep-item'><div class='rep-dot' style='border-color:{color}'></div><div class='rep-date'>{h.dtFecha:dddd, dd MMM yyyy HH:mm}</div><div><strong style='color:{color}'>{h.strAccion}</strong> - Usuario: {h.strUsuario}</div><div class='mt-1 small bg-white border p-2 rounded'>Motivo: {h.strMotivo}</div></div>");
            }
            sb.Append("</div></div>");

            return sb.ToString();
        }

        // ==========================================
        // 7. UTILIDADES
        // ==========================================

        private string GuardarArchivoFisico(FileUpload control, string nombreArchivo)
        {
            if (!Directory.Exists(RUTA_BASE_ARCHIVOS)) Directory.CreateDirectory(RUTA_BASE_ARCHIVOS);
            string ruta = Path.Combine(RUTA_BASE_ARCHIVOS, nombreArchivo);
            control.SaveAs(ruta);
            return ruta;
        }

        private void SetFlashMessage(string msg, string type)
        {
            Session["TempMsg"] = msg;
            Session["TempTipo"] = type;
        }

        private void Redireccionar(string msg, string type)
        {
            SetFlashMessage(msg, type);
            Response.Redirect("EjecucionProAprobados.aspx", false);
        }

        private void Msg(string msg, string type)
        {
            string cleanMsg = msg.Replace("'", "\\'").Replace("\r\n", " ").Replace("\n", " ").Replace("\\", "\\\\");
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"$(function() {{ toastify('{type}', '{cleanMsg}', 'Sistema'); }});", true);
        }
    }
}