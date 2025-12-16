using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;

namespace SistemaGestionCGI
{
    public partial class EjecucionProAprobados : System.Web.UI.Page
    {
        private readonly ManejadorEjecucionProyectos _manejador = new ManejadorEjecucionProyectos();
        private readonly ManejadorInscripcionProyectos _manejadorProyectos = new ManejadorInscripcionProyectos();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 1. Validar Sesión
                if (Session["UsuarioLogueado"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                // 2. Cargar Datos Iniciales
                CargarGrillaEjecucion();

                // 3. Verificar si venimos de una redirección (Ej: al guardar un miembro)
                string idTeamRedirect = Request.QueryString["idTeam"];
                if (!string.IsNullOrEmpty(idTeamRedirect))
                {
                    if (int.TryParse(idTeamRedirect, out int idTeam))
                    {
                        CargarEquipo(idTeam);
                    }
                }

                // 4. Mostrar Mensajes Flash (Toastify)
                if (Session["TempMsg"] != null)
                {
                    string mensaje = Session["TempMsg"].ToString();
                    string tipo = Session["TempTipo"].ToString();
                    Msg(mensaje, tipo);

                    Session["TempMsg"] = null;
                    Session["TempTipo"] = null;
                }
            }
        }

        // =======================================================
        // 1. GESTIÓN PRINCIPAL (GRIDS Y COMBOS)
        // =======================================================

        private void CargarGrillaEjecucion()
        {
            try
            {
                List<InvgccEjecucionProyectos> lista = _manejador.ObtenerEjecuciones();
                rptEjecucion.DataSource = lista;
                rptEjecucion.DataBind();
            }
            catch (Exception ex)
            {
                Msg("Error al cargar ejecuciones: " + ex.Message, "ee");
            }
        }

        private void CargarProyectosAprobados()
        {
            try
            {
                var lista = _manejador.ObtenerProyectosAprobadosSinEjecucion();
                ddlProyectosAprobados.DataSource = lista;
                ddlProyectosAprobados.DataTextField = "strTema_pro";
                ddlProyectosAprobados.DataValueField = "strId_pro";
                ddlProyectosAprobados.DataBind();
                ddlProyectosAprobados.Items.Insert(0, new ListItem("-- Seleccione Proyecto --", ""));
            }
            catch (Exception ex)
            {
                Msg("Error al cargar proyectos: " + ex.Message, "ee");
            }
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
            string idPro = ddlProyectosAprobados.SelectedValue;
            if (!string.IsNullOrEmpty(idPro))
            {
                var pro = _manejadorProyectos.ObtenerPorId(idPro);
                if (pro != null)
                {
                    txtCoordinadorAdd.Text = pro.strCoordinador_pro;
                }
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

                InvgccEjecucionProyectos obj = new InvgccEjecucionProyectos();
                obj.fkId_pro = ddlProyectosAprobados.SelectedValue; // Es string (VARCHAR)
                obj.strCoordinador_ejec = txtCoordinadorAdd.Text.Trim();
                obj.strPeriodo_ejec = txtPeriodoAdd.Text.Trim();
                obj.dtFechaini_ejec = DateTime.Parse(txtFechaIniAdd.Text);
                obj.dtFechafin_ejec = null;

                if (flpArchivoAdd.HasFile)
                {
                    string nombre = "PLAN_" + DateTime.Now.Ticks + Path.GetExtension(flpArchivoAdd.FileName);
                    // IMPORTANTE: Aquí se usa el método de 2 parámetros
                    obj.strInforme_ejec = GuardarArchivo(flpArchivoAdd, nombre);
                }

                _manejador.GuardarEjecucion(obj);

                SetFlashMessage("Ejecución iniciada correctamente.", "ss");
                Response.Redirect("EjecucionProAprobados.aspx", false);
            }
            catch (Exception ex)
            {
                Msg("Error al guardar: " + ex.Message, "ee");
            }
        }

        protected void rptEjecucion_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            // Parseamos ID a INT porque es Identity en BD
            int id = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "Editar")
            {
                CargarEdicion(id);
            }
            else if (e.CommandName == "Equipo")
            {
                CargarEquipo(id);
            }
            else if (e.CommandName == "Informes")
            {
                hfIdEjecucionInforme.Value = id.ToString();
                CargarInformes(id);
                // Abrir modal
                ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalInf", "AbrirModalInformes();", true);
            }
            else if (e.CommandName == "Eliminar")
            {
                try
                {
                    _manejador.EliminarEjecucion(id);
                    SetFlashMessage("Registro eliminado correctamente.", "ss");
                    Response.Redirect("EjecucionProAprobados.aspx", false);
                }
                catch (Exception ex)
                {
                    Msg("Error al eliminar: " + ex.Message, "ee");
                }
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
                txtFechaFinEdit.Text = obj.dtFechafin_ejec.HasValue ? obj.dtFechafin_ejec.Value.ToString("yyyy-MM-dd") : "";
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

        protected void btnGuardarEdit_Click(object sender, EventArgs e)
        {
            try
            {
                InvgccEjecucionProyectos obj = new InvgccEjecucionProyectos();
                obj.strId_ejec = int.Parse(hfIdEjecEdit.Value); // Parse INT
                obj.strCoordinador_ejec = txtCoordinadorEdit.Text.Trim();
                obj.dtFechaini_ejec = DateTime.Parse(txtFechaIniEdit.Text);

                if (!string.IsNullOrEmpty(txtFechaFinEdit.Text))
                    obj.dtFechafin_ejec = DateTime.Parse(txtFechaFinEdit.Text);
                else
                    obj.dtFechafin_ejec = null;

                obj.strPeriodo_ejec = txtPeriodoEdit.Text.Trim();
                obj.strInforme_ejec = hfArchivoActual.Value;

                if (flpArchivoEdit.HasFile)
                {
                    string nombre = "PLAN_" + DateTime.Now.Ticks + Path.GetExtension(flpArchivoEdit.FileName);
                    obj.strInforme_ejec = GuardarArchivo(flpArchivoEdit, nombre);
                }

                _manejador.ActualizarEjecucion(obj);

                SetFlashMessage("Datos de ejecución actualizados correctamente.", "ss");
                Response.Redirect("EjecucionProAprobados.aspx", false);
            }
            catch (Exception ex)
            {
                Msg("Error al actualizar: " + ex.Message, "ee");
            }
        }

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

        // =======================================================
        // 2. GESTIÓN DE EQUIPO
        // =======================================================

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
                var miembros = _manejador.ObtenerMiembros(id);
                rptMiembros.DataSource = miembros;
                rptMiembros.DataBind();
            }
        }

        protected void btnAbrirFormMiembro_Click(object sender, EventArgs e)
        {
            pnlEquipoListado.Visible = false;
            pnlFormularioMiembro.Visible = true;

            hfIdMiembroEdit.Value = "";
            lblTituloFormMiembro.Text = "Nuevo Integrante";

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

                InvgccEjecucionMiembros m = new InvgccEjecucionMiembros();
                m.fkId_ejec = int.Parse(hfIdEjecucionEquipo.Value); // Parse INT
                m.strCedula_miembro = txtCedulaMiembro.Text.Trim();
                m.strNombres_miembro = txtNombresMiembro.Text.Trim();
                m.strApellidos_miembro = txtApellidosMiembro.Text.Trim();
                m.strRol_miembro = ddlRolMiembro.SelectedValue;
                m.strFacultad_miembro = ddlFacultadMiembro.SelectedValue;

                if (string.IsNullOrEmpty(hfIdMiembroEdit.Value))
                {
                    _manejador.GuardarMiembro(m);
                    SetFlashMessage("Integrante agregado correctamente.", "ss");
                }
                else
                {
                    m.strId_miembro = int.Parse(hfIdMiembroEdit.Value); // Parse INT
                    _manejador.ActualizarMiembro(m);
                    SetFlashMessage("Datos del integrante actualizados.", "ss");
                }

                Response.Redirect($"EjecucionProAprobados.aspx?idTeam={m.fkId_ejec}", false);
            }
            catch (Exception ex)
            {
                Msg("Error al guardar miembro: " + ex.Message, "ee");
            }
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
                    // Llenar datos visuales del modal usando JS
                    hfIdMiembroEstado.Value = idMiembro.ToString();
                    string nombre = $"{m.strNombres_miembro} {m.strApellidos_miembro}";

                    string script = $@"
                        document.getElementById('lblNombreMiembroEstado').innerText = '{nombre}';
                        document.getElementById('lblRolMiembroEstado').innerText = '{m.strRol_miembro}';
                        document.getElementById('txtMotivoCambio').value = ''; 
                        var myModal = new bootstrap.Modal(document.getElementById('modalEstadoMiembro'));
                        myModal.show();";

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
                    var historial = _manejador.ObtenerHistorialMiembro(idMiembro);
                    rptHistorialMiembro.DataSource = historial;
                    rptHistorialMiembro.DataBind();

                    string script = "new bootstrap.Modal(document.getElementById('modalHistorialMiembro')).show();";
                    ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalHist", script, true);
                }
            }

            else if (e.CommandName == "EliminarMiembro")
            {
                try
                {
                    _manejador.EliminarMiembro(idMiembro);
                    SetFlashMessage("Integrante eliminado del equipo.", "ss");

                    // Recargar misma vista
                    string currentTeamId = hfIdEjecucionEquipo.Value;
                    Response.Redirect($"EjecucionProAprobados.aspx?idTeam={currentTeamId}", false);
                }
                catch (Exception ex)
                {
                    Msg("Error al eliminar: " + ex.Message, "ee");
                }
            }
            else if (e.CommandName == "EditarMiembro")
            {
                try
                {
                    var miembro = _manejador.ObtenerMiembroPorId(idMiembro);
                    if (miembro != null)
                    {
                        hfIdMiembroEdit.Value = miembro.strId_miembro.ToString();
                        txtCedulaMiembro.Text = miembro.strCedula_miembro;
                        txtNombresMiembro.Text = miembro.strNombres_miembro;
                        txtApellidosMiembro.Text = miembro.strApellidos_miembro;

                        if (ddlRolMiembro.Items.FindByValue(miembro.strRol_miembro) != null)
                            ddlRolMiembro.SelectedValue = miembro.strRol_miembro;

                        if (ddlFacultadMiembro.Items.FindByValue(miembro.strFacultad_miembro) != null)
                            ddlFacultadMiembro.SelectedValue = miembro.strFacultad_miembro;

                        lblTituloFormMiembro.Text = "Editar Integrante";
                        pnlEquipoListado.Visible = false;
                        pnlFormularioMiembro.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    Msg("Error al cargar miembro: " + ex.Message, "ee");
                }
            }
        }

        protected void btnConfirmarEstado_Click(object sender, EventArgs e)
        {
            try
            {
                int idMiembro = int.Parse(hfIdMiembroEstado.Value);
                string motivo = hfMotivoHidden.Value;

                // Obtener usuario actual de la sesión
                string usuario = Session["UsuarioLogueado"] != null ? Session["UsuarioLogueado"].ToString() : "SISTEMA";

                // Obtener estado actual para invertirlo
                var m = _manejador.ObtenerMiembroPorId(idMiembro);
                bool nuevoEstado = !m.bitActivo_miembro;

                // Llamada a la BLL
                _manejador.CambiarEstadoMiembro(idMiembro, nuevoEstado, motivo, usuario);

                Msg("Estado actualizado correctamente.", "ss");

                // Recargar la grilla de miembros
                RefrescarTablaMiembros();
            }
            catch (Exception ex)
            {
                Msg("Error: " + ex.Message, "ee");
            }
        }

        protected void btnVolverDeEquipo_Click(object sender, EventArgs e)
        {
            Response.Redirect("EjecucionProAprobados.aspx");
        }

        // =======================================================
        // 3. GESTIÓN DE INFORMES
        // =======================================================

        private void CargarInformes(int idEjecucion)
        {
            var informes = _manejador.ObtenerInformes(idEjecucion);
            rptInformes.DataSource = informes;
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
                    SetFlashMessage("Documento eliminado correctamente.", "ss");
                    Response.Redirect("EjecucionProAprobados.aspx", false);
                }
                catch (Exception ex) { Msg("Error al eliminar: " + ex.Message, "ee"); }
            }
            else if (e.CommandName == "EditarInforme")
            {
                // Cargamos datos para editar
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

        // 2. BOTÓN GUARDAR (Ahora maneja Insertar y Actualizar + Validación Word)
        protected void btnGuardarInforme_Click(object sender, EventArgs e)
        {
            try
            {
                // A. VALIDACIÓN DE EXTENSIÓN (Solo Word)
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

                // B. PREPARAR DATOS
                if (!int.TryParse(hfIdEjecucionInforme.Value, out int idEjec)) return;

                InvgccEjecucionInformes inf = new InvgccEjecucionInformes();
                inf.fkId_ejec = idEjec;
                inf.strNombrePeriodo = txtNombrePeriodoInf.Text.Trim();
                if (string.IsNullOrEmpty(inf.strNombrePeriodo)) inf.strNombrePeriodo = "Informe de Avance";

                // Guardar físico si hay archivo
                if (flpArchivoInf.HasFile)
                {
                    string fileName = $"INF_{DateTime.Now.Ticks}{Path.GetExtension(flpArchivoInf.FileName)}";
                    inf.strArchivo_path = GuardarArchivo(flpArchivoInf, fileName);
                }
                else
                {
                    inf.strArchivo_path = ""; // Indica que no hay cambio de archivo
                }

                // C. DECIDIR SI ES INSERT O UPDATE
                if (string.IsNullOrEmpty(hfIdInformeEdit.Value))
                {
                    // === INSERTAR ===
                    if (!flpArchivoInf.HasFile)
                    {
                        Msg("Debe seleccionar un archivo Word.", "ww");
                        ScriptManager.RegisterStartupScript(this, GetType(), "Reopen", "AbrirSubModalUpload();", true);
                        return;
                    }
                    _manejador.GuardarInforme(inf);
                    SetFlashMessage("Informe subido correctamente.", "ss");
                }
                else
                {
                    // === ACTUALIZAR ===
                    inf.strId_informe = int.Parse(hfIdInformeEdit.Value);
                    _manejador.ActualizarInforme(inf);
                    SetFlashMessage("Informe corregido correctamente.", "ss");
                }

                // D. LIMPIEZA
                txtNombrePeriodoInf.Text = "";
                hfIdInformeEdit.Value = ""; // Limpiar ID de edición
                lblTituloModalInforme.InnerText = "Subir Informe"; // Resetear título

                Response.Redirect("EjecucionProAprobados.aspx", false);
            }
            catch (Exception ex) { Msg("Error: " + ex.Message, "ee"); }
        }

        protected void btnInformes_Click(object sender, EventArgs e)
        {
            // ... lógica existente ...
            hfIdInformeEdit.Value = ""; // Asegurarnos de que empiece en modo "Nuevo"
            txtNombrePeriodoInf.Text = "";
            lblTituloModalInforme.InnerText = "Subir Informe";
        }

        // =======================================================
        // 4. UTILIDADES Y BOTONES EXTRA
        // =======================================================

        private string GuardarArchivo(FileUpload control, string nombreArchivo)
        {
            string carpetaFisica = @"C:\UTC\EJECUCION_INFORMES\";
            if (!Directory.Exists(carpetaFisica))
                Directory.CreateDirectory(carpetaFisica);

            string rutaCompleta = Path.Combine(carpetaFisica, nombreArchivo);
            control.SaveAs(rutaCompleta);
            return rutaCompleta;
        }

        protected void btnCancelarNew_Click(object sender, EventArgs e)
        {
            btnRegresar_Click(null, null);
        }

        protected void btnCancelarEdit_Click(object sender, EventArgs e)
        {
            btnRegresar_Click(null, null);
        }

        private void SetFlashMessage(string msg, string type)
        {
            Session["TempMsg"] = msg;
            Session["TempTipo"] = type;
        }

        private void Msg(string msg, string type)
        {
            string cleanMsg = msg.Replace("'", "\\'").Replace("\r\n", " ").Replace("\n", " ").Replace("\\", "\\\\");
            string script = $"$(function() {{ toastify('{type}', '{cleanMsg}', 'Sistema'); }});";
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", script, true);
        }

        // =============================================================
        // 5. GENERACIÓN DE REPORTES (NUEVO)
        // =============================================================

        protected void btnGenerarReporteHistorial_Click(object sender, EventArgs e)
        {
            try
            {
                // CORRECCIÓN: Usamos directamente el HiddenField. 
                // No usamos 'e.CommandArgument' porque EventArgs no lo contiene.
                if (int.TryParse(hfIdMiembroEstado.Value, out int idMiembro))
                {
                    // 1. Construir el HTML
                    string html = ConstruirReporteHistorial(idMiembro);

                    // 2. Configurar Modal para modo REPORTE
                    litReporteGenerado.Text = html;
                    pnlReporteHtml.Visible = true; // Mostrar Panel HTML

                    // Ocultar iframe y mostrar botón imprimir
                    btnImprimirReporte.Style["display"] = "inline-block";
                    lblTituloPreview.InnerText = "Reporte Oficial de Movimientos";

                    // Script para ajustar la UI del modal (ocultar iframe, mostrar div)
                    string script = @"
                document.getElementById('framePdf').style.display = 'none';
                document.getElementById('btnDescargarDirecto').style.display = 'none';
                
                // Cerrar modal pequeño si está abierto
                var mHist = bootstrap.Modal.getInstance(document.getElementById('modalHistorialMiembro'));
                if(mHist) mHist.hide();
                
                // Abrir modal grande
                var mPrev = new bootstrap.Modal(document.getElementById('modalVistaPrevia'));
                mPrev.show();";

                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowReport", script, true);
                }
                else
                {
                    Msg("No se pudo identificar al integrante. Intente abrir el historial nuevamente.", "ww");
                }
            }
            catch (Exception ex)
            {
                Msg("Error al generar reporte: " + ex.Message, "ee");
            }
        }

        private string ConstruirReporteHistorial(int idMiembro)
        {
            var miembro = _manejador.ObtenerMiembroPorId(idMiembro);
            var historial = _manejador.ObtenerHistorialMiembro(idMiembro);
            var ejecucion = _manejador.ObtenerEjecucionPorId(miembro.fkId_ejec); // Necesitamos info del proyecto

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            // Estilos Inline para asegurar impresión limpia
            sb.Append(@"
        <style>
            .rep-header { text-align: center; margin-bottom: 30px; border-bottom: 2px solid #312783; padding-bottom: 15px; }
            .rep-logo { width: 200px; margin-bottom: 10px; }
            .rep-title { color: #312783; font-size: 20px; font-weight: bold; text-transform: uppercase; margin: 0; }
            .rep-sub { color: #666; font-size: 14px; }
            .rep-card { background: #f8f9fa; border: 1px solid #ddd; padding: 20px; border-radius: 8px; margin-bottom: 25px; }
            .rep-label { font-weight: bold; color: #312783; }
            .rep-timeline { position: relative; padding-left: 30px; border-left: 2px solid #e0e0e0; margin-left: 10px; }
            .rep-item { margin-bottom: 25px; position: relative; }
            .rep-dot { width: 16px; height: 16px; background: #fff; border: 3px solid #312783; border-radius: 50%; position: absolute; left: -39px; top: 0; }
            .rep-date { font-size: 13px; color: #999; margin-bottom: 4px; font-weight: 600; }
            .rep-action { font-weight: bold; font-size: 15px; }
            .rep-user { font-size: 12px; background: #e9ecef; padding: 2px 8px; border-radius: 10px; float: right; }
            .rep-reason { background: #fff; border: 1px solid #eee; padding: 10px; border-radius: 4px; margin-top: 5px; font-style: italic; color: #555; }
            .badge-baja { color: #dc3545; border-color: #dc3545; }
            .badge-alta { color: #198754; border-color: #198754; }
        </style>");

            // Encabezado
            sb.Append("<div class='rep-header'>");
            sb.Append("<img src='https://aplicaciones.utc.edu.ec/sigutc/img/bnUTC.png' class='rep-logo'><br>");
            sb.Append("<h3 class='rep-title'>Historial de Movimientos del Integrante</h3>");
            sb.Append("<div class='rep-sub'>Dirección de Investigación - Gestión de Proyectos</div>");
            sb.Append("</div>");

            // Datos Generales
            sb.Append("<div class='rep-card'>");
            sb.Append("<div class='row'>");
            sb.Append($"<div class='col-6 mb-2'><span class='rep-label'>NOMBRE:</span> {miembro.strNombres_miembro} {miembro.strApellidos_miembro}</div>");
            sb.Append($"<div class='col-6 mb-2'><span class='rep-label'>CÉDULA:</span> {miembro.strCedula_miembro}</div>");
            sb.Append($"<div class='col-6 mb-2'><span class='rep-label'>ROL:</span> {miembro.strRol_miembro}</div>");
            sb.Append($"<div class='col-6 mb-2'><span class='rep-label'>FACULTAD:</span> {miembro.strFacultad_miembro}</div>");
            sb.Append($"<div class='col-12 mt-2 pt-2 border-top'><span class='rep-label'>PROYECTO:</span> {ejecucion.TituloProyecto}</div>");
            sb.Append("</div></div>");

            // Timeline
            sb.Append("<div class='p-3'>");
            sb.Append("<h5 class='mb-4 text-secondary'>Línea de Tiempo</h5>");
            sb.Append("<div class='rep-timeline'>");

            foreach (var h in historial)
            {
                string colorClass = h.strAccion == "BAJA" ? "badge-baja" : "badge-alta";

                sb.Append("<div class='rep-item'>");
                sb.Append($"<div class='rep-dot {colorClass}' style='border-color:{(h.strAccion == "BAJA" ? "#dc3545" : "#198754")}'></div>");
                sb.Append($"<div class='rep-date'>{h.dtFecha:dddd, dd MMMM yyyy - HH:mm}</div>");

                sb.Append("<div>");
                sb.Append($"<span class='rep-action' style='color:{(h.strAccion == "BAJA" ? "#dc3545" : "#198754")}'>{h.strAccion}</span>");
                sb.Append($"<span class='rep-user'>Usuario: {h.strUsuario}</span>");
                sb.Append("</div>");

                sb.Append($"<div class='rep-reason'>Motivo: {h.strMotivo}</div>");
                sb.Append("</div>");
            }

            sb.Append("</div></div>"); // Fin Timeline

            // Pie de página
            sb.Append($"<div class='text-center text-muted small mt-5'>Reporte generado el {DateTime.Now:dd/MM/yyyy HH:mm:ss}</div>");

            return sb.ToString();
        }

    }
}