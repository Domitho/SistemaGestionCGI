using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;
using System.Diagnostics;

namespace SistemaGestionCGI
{
    public partial class EjecucionProAprobados : System.Web.UI.Page
    {
        // ==========================================
        // 1. INSTANCIAS Y VARIABLES GLOBALES
        // ==========================================
        private readonly ManejadorEjecucionProyectos _manejador = new ManejadorEjecucionProyectos();
        private readonly ManejadorInscripcionProyectos _manejadorProyectos = new ManejadorInscripcionProyectos();
        private const string RUTA_VIRTUAL_ARCHIVOS = "~/RepositorioUTC/EjecucionInformes/";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UsuarioLogueado"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                CargarGrillaEjecucion();

                string idTeamRedirect = Request.QueryString["idTeam"];
                if (!string.IsNullOrEmpty(idTeamRedirect) && int.TryParse(idTeamRedirect, out int idTeam))
                {
                    CargarEquipo(idTeam);
                }

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

                    // --- SEGURIDAD DEL MODAL ---
                    ConfigurarPermisosModalInformes(); // <--- LLAMADA AL NUEVO MÉTODO
                                                       // ---------------------------

                    // Configurar estado de botones de cierre (si aplica)
                    if (Session["RolUsuario"]?.ToString() == "ADMINISTRADOR")
                    {
                        ConfigurarBotonesFaseFinal(id);
                    }

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

        protected void rptEjecucion_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            // Solo nos interesa filtrar las filas de datos (no el encabezado ni el pie)
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // 1. Obtener el Rol del usuario actual
                string rol = Session["RolUsuario"]?.ToString() ?? "";

                // 2. Buscar los botones en la fila actual
                var btnEditar = (LinkButton)e.Item.FindControl("btnEditar");
                var btnEquipo = (LinkButton)e.Item.FindControl("btnEquipo");
                var btnEliminar = (LinkButton)e.Item.FindControl("btnEliminar");
                var btnInformes = (LinkButton)e.Item.FindControl("btnInformes");

                // 3. APLICAR LÓGICA DE SEGURIDAD
                if (rol == "COORDINADOR")
                {
                    // El Coordinador NO puede editar, ni gestionar equipo, ni eliminar.
                    if (btnEditar != null) btnEditar.Visible = false;
                    if (btnEquipo != null) btnEquipo.Visible = false;
                    if (btnEliminar != null) btnEliminar.Visible = false;

                    // El Coordinador SÍ puede ver informes (botón verde)
                    if (btnInformes != null) btnInformes.Visible = true;
                }
                else if (rol == "ADMINISTRADOR")
                {
                    // El Admin ve todo
                    // (No es necesario hacer nada porque Visible="true" es el default)
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
                if (flpArchivoInf.HasFile)
                {
                    string ext = Path.GetExtension(flpArchivoInf.FileName).ToLower();

                    if (ext != ".doc" && ext != ".docx" && ext != ".pdf")
                    {
                        Msg("Formato no válido. Solo se permiten archivos Word (.doc, .docx) o PDF (.pdf).", "ww");
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


        // Evento para el Botón 1: Abrir Modal de Cierre
        protected void btnInformeCierre_Click(object sender, EventArgs e)
        {
            string idEjecucionStr = hfIdEjecucionInforme.Value;

            if (!string.IsNullOrEmpty(idEjecucionStr) && int.TryParse(idEjecucionStr, out int idEjec))
            {
                var proyecto = _manejador.ObtenerEjecucionPorId(idEjec);
                string estado = proyecto.strEstado_ejec.ToUpper();
                bool tieneArchivo = !string.IsNullOrEmpty(proyecto.strInforme_Cierre);

                // --- LÓGICA DE ESTADOS DEL MODAL ---

                if (estado == "CIERRE APROBADO")
                {
                    // CASO 1: YA ESTÁ APROBADO (Modo solo lectura bloqueado)
                    pnlCierreBloqueado.Visible = true;       // Mensaje verde grande
                    divAlertaCierre.Visible = false;         // Ocultar alerta amarilla
                    pnlCargaCierre.Visible = false;          // Ocultar Dropzone (No se puede subir más)
                    btnGuardarCierre.Visible = false;        // Ocultar botón guardar
                    btnAprobarCierre.Visible = false;        // Ocultar botón aprobar (ya lo está)

                    // Mostrar archivo para descargar
                    pnlArchivoCierreActual.Visible = true;
                    lblNombreArchivoCierre.Text = Path.GetFileName(proyecto.strInforme_Cierre);
                    lnkVerCierreActual.HRef = ResolveUrl(proyecto.strInforme_Cierre);
                }
                else
                {
                    // CASO 2: AÚN NO APROBADO (Permite edición)
                    pnlCierreBloqueado.Visible = false;
                    divAlertaCierre.Visible = true;
                    pnlCargaCierre.Visible = true;
                    btnGuardarCierre.Visible = true;

                    // Configurar visualización del archivo actual
                    if (tieneArchivo)
                    {
                        pnlArchivoCierreActual.Visible = true;
                        lblNombreArchivoCierre.Text = Path.GetFileName(proyecto.strInforme_Cierre);
                        lnkVerCierreActual.HRef = ResolveUrl(proyecto.strInforme_Cierre);

                        lblTituloInputCierre.InnerText = "Sustituir Documento";
                        litBtnCierreTexto.Text = "Actualizar Archivo";

                        if (estado == "EN REVISION")
                        {
                            btnAprobarCierre.Visible = true; // <--- AQUÍ APARECE EL BOTÓN
                        }
                        else
                        {
                            btnAprobarCierre.Visible = false;
                        }
                    }
                    else
                    {
                        // Modo Limpio
                        pnlArchivoCierreActual.Visible = false;
                        btnAprobarCierre.Visible = false;
                        lblTituloInputCierre.InnerText = "Documento de Cierre";
                        litBtnCierreTexto.Text = "Enviar a Revisión";
                    }
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalCierre",
                    "new bootstrap.Modal(document.getElementById('modalSubirCierre')).show();", true);
            }
        }

        protected void btnGuardarCierre_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Validaciones
                if (!flpCierre.HasFile)
                {
                    Msg("Debe adjuntar el informe de cierre.", "ww");
                    // Reabrimos el modal si falla la validación
                    ScriptManager.RegisterStartupScript(this, GetType(), "ReopenCierre",
                        "new bootstrap.Modal(document.getElementById('modalSubirCierre')).show();", true);
                    return;
                }

                // CORREGIDO: Usamos hfIdEjecucionInforme
                if (!int.TryParse(hfIdEjecucionInforme.Value, out int idEjec))
                {
                    Msg("Error al identificar el proyecto.", "ee");
                    return;
                }

                // 2. Guardar Físicamente
                // Nomenclatura: CIERRE_Ticks_Nombre.pdf
                string nombreArchivo = $"CIERRE_{DateTime.Now.Ticks}{Path.GetExtension(flpCierre.FileName)}";
                string rutaGuardada = GuardarArchivoFisico(flpCierre, nombreArchivo);

                // 3. Lógica de Negocio (BLL)
                string usuario = Session["UsuarioLogueado"]?.ToString() ?? "SISTEMA";
                _manejador.SubirInformeCierre(idEjec, rutaGuardada, usuario);

                string mensaje = pnlArchivoCierreActual.Visible
                    ? "Informe de cierre actualizado/corregido correctamente."
                    : "Informe de cierre subido. El proyecto está EN REVISIÓN.";

                Msg(mensaje, "ss");

                // 4. Feedback y Refresco
                ScriptManager.RegisterStartupScript(this, GetType(), "CloseAll",
                            "bootstrap.Modal.getInstance(document.getElementById('modalSubirCierre')).hide(); $('#modalInformes').modal('hide');", true);

                CargarGrillaEjecucion();

            }
            catch (Exception ex)
            {
                Msg("Error al subir cierre: " + ex.Message, "ee");
            }
        }

        protected void btnAprobarCierre_Click(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(hfIdEjecucionInforme.Value, out int idEjec))
                {
                    string usuario = Session["UsuarioLogueado"]?.ToString() ?? "SISTEMA";

                    // 1. Llamar al BLL para aprobar
                    _manejador.AprobarCierre(idEjec, usuario);

                    // 2. Feedback
                    Msg("Documento APROBADO correctamente. Se ha habilitado la fase final.", "ss");

                    // 3. Cerrar todo y recargar
                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseAllApprove",
                        "bootstrap.Modal.getInstance(document.getElementById('modalSubirCierre')).hide(); $('#modalInformes').modal('hide');", true);

                    // 4. Recargar grilla (Esto actualizará el método ConfigurarBotonesFaseFinal automáticamente)
                    CargarGrillaEjecucion();
                }
            }
            catch (Exception ex)
            {
                Msg("Error al aprobar: " + ex.Message, "ee");
            }
        }

        // Evento para el Botón 2
        // 1. Abrir Modal Final
        protected void btnInformeFinal_Click(object sender, EventArgs e)
        {
            string idEjecucionStr = hfIdEjecucionInforme.Value;

            if (!string.IsNullOrEmpty(idEjecucionStr) && int.TryParse(idEjecucionStr, out int idEjec))
            {

                ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalFinal",
                    "new bootstrap.Modal(document.getElementById('modalSubirFinal')).show();", true);
            }
        }

        // 2. Guardar Final
        protected void btnGuardarFinal_Click(object sender, EventArgs e)
        {
            try
            {
                if (!flpFinal.HasFile) { Msg("Debe adjuntar el informe final.", "ww"); return; }
                if (!int.TryParse(hfIdEjecucionInforme.Value, out int idEjec)) return;

                string nombreArchivo = $"FINAL_{DateTime.Now.Ticks}{Path.GetExtension(flpFinal.FileName)}";
                string rutaGuardada = GuardarArchivoFisico(flpFinal, nombreArchivo);
                string usuario = Session["UsuarioLogueado"]?.ToString() ?? "SISTEMA";

                _manejador.SubirInformeFinal(idEjec, rutaGuardada, usuario);

                Msg("¡Proyecto FINALIZADO con éxito!", "ss");

                ScriptManager.RegisterStartupScript(this, GetType(), "CloseAllFinal",
                     "bootstrap.Modal.getInstance(document.getElementById('modalSubirFinal')).hide(); $('#modalInformes').modal('hide');", true);

                CargarGrillaEjecucion();
            }
            catch (Exception ex) { Msg("Error: " + ex.Message, "ee"); }
        }


        private void ConfigurarBotonesFaseFinal(int idEjecucion)
        {
            // 1. Obtenemos datos del proyecto
            var proyecto = _manejador.ObtenerEjecucionPorId(idEjecucion);

            if (proyecto == null) return;

            string estado = proyecto.strEstado_ejec.ToUpper();
            bool tieneCierre = !string.IsNullOrEmpty(proyecto.strInforme_Cierre);

            // ====================================================
            // LÓGICA DE BLOQUEO (STATE MACHINE)
            // ====================================================

            btnInformeCierre.CssClass = "btn btn-white border shadow-sm p-3 text-start position-relative hover-lift";
            btnInformeCierre.Enabled = true;

            if (estado == "CIERRE APROBADO")
            {
                // DESBLOQUEADO
                btnInformeFinal.CssClass = "btn btn-white border shadow-sm p-3 text-start hover-lift";
                btnInformeFinal.Enabled = true;
                btnInformeFinal.Attributes.Remove("title"); // Quitamos el tooltip de bloqueo
            }
            else
            {
                // BLOQUEADO (Si está en 'EJECUCION' o 'EN REVISION')
                btnInformeFinal.CssClass = "btn btn-white border shadow-sm p-3 text-start hover-lift btn-locked";
                btnInformeFinal.Enabled = false;
                btnInformeFinal.Attributes.Add("title", "Disponible solo cuando el Informe de Cierre sea APROBADO.");
            }

            // (Opcional) Visual Feedback en el botón de Cierre si ya se subió
            if (tieneCierre)
            {
                // Podríamos ponerle un borde verde o algo para indicar que ya hay algo subido
                btnInformeCierre.Style["border-left"] = "5px solid var(--utc-verde) !important";
            }
            else
            {
                btnInformeCierre.Style["border-left"] = "5px solid var(--utc-azul) !important";
            }
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
            // 1. Obtener Datos
            var miembro = _manejador.ObtenerMiembroPorId(idMiembro);
            var historial = _manejador.ObtenerHistorialMiembro(idMiembro);
            var ejecucion = _manejador.ObtenerEjecucionPorId(miembro.fkId_ejec);

            StringBuilder sb = new StringBuilder();

            // 2. HERO BANNER (LOGO)
            sb.Append("<div class='header-hero-banner'>");
            sb.Append("<img src='https://aplicaciones.utc.edu.ec/sigutc/img/bnUTC.png' alt='UTC Logo' />");
            sb.Append("</div>");

            // 3. CABECERA DIVIDIDA (TÍTULO Y METADATA)
            sb.Append("<div class='header-info-split'>");

            // Lado Izquierdo
            sb.Append("<div class='info-left'>");
            sb.Append("<span class='system-label'>Dirección de Investigación</span>");
            sb.Append("<h1 class='doc-title'>Historial de Movimientos</h1>");
            sb.Append("</div>");

            // Lado Derecho
            sb.Append("<div class='info-right'>");
            sb.Append("<div class='meta-group'>");
            sb.Append($"<span class='meta-label'>Referencia ID</span>");
            sb.Append($"<span class='meta-value ref-highlight'>{miembro.strId_miembro}</span>");
            sb.Append("</div>");
            sb.Append("<div class='meta-group'>");
            sb.Append($"<span class='meta-label'>Fecha Emisión</span>");
            sb.Append($"<span class='meta-value'>{DateTime.Now:dd/MM/yyyy}</span>");
            sb.Append("</div>");
            sb.Append("</div>"); // Fin info-right
            sb.Append("</div>"); // Fin header-info-split

            sb.Append("<div class='mt-5'></div>");

            // 4. TARJETA DE INFORMACIÓN (RESEARCHER CARD)
            sb.Append("<div class='researcher-card'>");

            // Fila 1
            sb.Append("<div class='card-row'>");
            sb.Append("<div class='card-item'>");
            sb.Append("<span class='label'>INTEGRANTE</span>");
            sb.Append($"<span class='value'>{miembro.strApellidos_miembro} {miembro.strNombres_miembro}</span>");
            sb.Append("</div>");
            sb.Append("<div class='card-item'>");
            sb.Append("<span class='label'>CÉDULA</span>");
            sb.Append($"<span class='value'>{miembro.strCedula_miembro}</span>");
            sb.Append("</div>");
            sb.Append("</div>");

            // Fila 2
            sb.Append("<div class='card-row'>");
            sb.Append("<div class='card-item'>");
            sb.Append("<span class='label'>PROYECTO</span>");
            sb.Append($"<span class='value' style='font-size: 0.9rem;'>{ejecucion.TituloProyecto}</span>");
            sb.Append("</div>");
            sb.Append("</div>");

            // Fila 3
            sb.Append("<div class='card-row'>");
            sb.Append("<div class='card-item'>");
            sb.Append("<span class='label'>ROL / FUNCIÓN</span>");
            sb.Append($"<span class='value'>{miembro.strRol_miembro}</span>");
            sb.Append("</div>");
            sb.Append("<div class='card-item'>");
            sb.Append("<span class='label'>ESTADO ACTUAL</span>");
            string estado = miembro.bitActivo_miembro ? "ACTIVO" : "INACTIVO";
            string colorEstado = miembro.bitActivo_miembro ? "#198754" : "#dc3545"; // Verde o Rojo
            sb.Append($"<span class='value' style='color:{colorEstado}'>{estado}</span>");
            sb.Append("</div>");
            sb.Append("</div>");

            sb.Append("</div>"); // Fin Card

            // 5. TIMELINE (LÍNEA DE TIEMPO)
            sb.Append("<div class='timeline-container'>");
            sb.Append("<h4 class='timeline-title'>Registro Cronológico</h4>");
            sb.Append("<ul class='timeline-list'>");

            if (historial != null && historial.Count > 0)
            {
                foreach (var h in historial)
                {
                    sb.Append("<li class='timeline-item'>");
                    sb.Append("<div class='timeline-marker'></div>");
                    sb.Append("<div class='timeline-content'>");

                    // Header del item
                    sb.Append("<div class='timeline-header'>");
                    sb.Append($"<span class='date'>{h.dtFecha:dd 'de' MMMM, yyyy}</span>");
                    sb.Append($"<span class='time'>{h.dtFecha:HH:mm}</span>");
                    sb.Append("</div>");

                    // Body del item
                    sb.Append("<div class='timeline-body'>");

                    // Badge Acción
                    string badgeClass = h.strAccion.Contains("BAJA") ? "bad" : "good";
                    sb.Append($"<div class='action-badge {badgeClass}'>{h.strAccion}</div>");

                    // Motivo
                    sb.Append($"<p class='description'><strong>Detalle:</strong> {h.strMotivo}</p>");

                    // Firma Usuario
                    sb.Append($"<div class='user-signature'><i class='fa-solid fa-user-check'></i> Procesado por: {h.strUsuario}</div>");

                    sb.Append("</div>"); // Fin timeline-body
                    sb.Append("</div>"); // Fin timeline-content
                    sb.Append("</li>");
                }
            }
            else
            {
                sb.Append("<li class='timeline-item'><p class='text-muted'>Sin historial registrado.</p></li>");
            }

            sb.Append("</ul>");
            sb.Append("</div>"); 

            // 6. FOOTER LEGAL
            sb.Append("<div class='report-legal-footer'>");
            sb.Append("Documento generado automáticamente por el Sistema de Gestión CGI-UTC.<br>");
            sb.Append("La validez de este reporte está sujeta a los registros digitales institucionales.");
            sb.Append("</div>");

            return sb.ToString();
        }

        protected void btnAbrirGenerador_Click(object sender, EventArgs e)
        {
            if (int.TryParse(hfIdEjecucionInforme.Value, out int id))
            {
                // Llamamos al método público del control
                ucGenerador.Mostrar(id);
            }
            else
            {
                Msg("Seleccione un proyecto primero (Botón Informes)", "ww");
            }
        }

        // GENERACION DEL DOCUMENTO
        protected void ucGenerador_InformeGuardado(object sender, EventArgs e)
        {
            // Este evento se dispara automáticamente cuando el UserControl termina de guardar
            if (int.TryParse(hfIdEjecucionInforme.Value, out int id))
            {
                CargarInformes(id); // Refresca la tabla de archivos
                Msg("Documento generado y guardado correctamente.", "ss");
            }
        }

        // ==========================================
        // 7. UTILIDADES
        // ==========================================

        private void ConfigurarPermisosModalInformes()
        {
            string rol = Session["RolUsuario"]?.ToString() ?? "";

            if (rol == "COORDINADOR")
            {
                // Ocultar toda la sección inferior del modal
                tituloEtapaFinal.Visible = false;
                divContenedorBotonesFinales.Visible = false;

                // Los botones individuales ya no importan porque ocultamos su contenedor padre,
                // pero por seguridad también los apagamos.
                btnInformeCierre.Visible = false;
                btnInformeFinal.Visible = false;
            }
            else
            {
                tituloEtapaFinal.Visible = true;
                divContenedorBotonesFinales.Visible = true;
                btnInformeCierre.Visible = true;
                btnInformeFinal.Visible = true;
            }
        }

        private string GuardarArchivoFisico(FileUpload control, string nombreArchivo)
        {
            string rutaFisicaCarpeta = Server.MapPath(RUTA_VIRTUAL_ARCHIVOS);

            if (!Directory.Exists(rutaFisicaCarpeta))
            {
                Directory.CreateDirectory(rutaFisicaCarpeta);
            }

            string rutaFisicaCompleta = Path.Combine(rutaFisicaCarpeta, nombreArchivo);
            control.SaveAs(rutaFisicaCompleta);

            return Path.Combine(RUTA_VIRTUAL_ARCHIVOS, nombreArchivo).Replace("\\", "/");
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