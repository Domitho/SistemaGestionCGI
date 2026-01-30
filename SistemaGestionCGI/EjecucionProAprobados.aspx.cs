using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SistemaGestionCGI
{
    public partial class EjecucionProAprobados : System.Web.UI.Page
    {
        // INSTANCIAS Y VARIABLES GLOBALES
        private readonly ManejadorEjecucionProyectos _manejador = new ManejadorEjecucionProyectos();
        private readonly ManejadorInscripcionProyectos _manejadorProyectos = new ManejadorInscripcionProyectos();
        private const string RUTA_VIRTUAL_ARCHIVOS = "~/RepositorioUTC/EjecucionInformes/";
        private bool EsAdmin => Session["RolUsuario"]?.ToString() == "ADMINISTRADOR";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioLogueado"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!EsAdmin)
            {
                Response.Redirect("ProyectosAprobadosCoordinadores.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarCombosIniciales();
                CargarGrillaEjecucion();

                if (Session["TempMsg"] != null)
                {
                    Msg(Session["TempMsg"].ToString(), Session["TempTipo"].ToString());
                    Session["TempMsg"] = null;
                    Session["TempTipo"] = null;
                }
            }
        }

        private void CargarCombosIniciales()
        {
            ddlCiclo.DataSource = _manejador.ObtenerCiclos();
            ddlCiclo.DataTextField = "strNombre_ciclo";
            ddlCiclo.DataValueField = "id_ciclo";
            ddlCiclo.DataBind();
            ddlCiclo.Items.Insert(0, new ListItem("-- Seleccione Ciclo --", "0"));
        }

        // ==========================================
        // 2. GESTIÓN PRINCIPAL (PROYECTOS)
        // ==========================================

        private void CargarGrillaEjecucion()
        {
            try
            {
                rptEjecucion.DataSource = _manejador.ObtenerEjecuciones(null);
                rptEjecucion.DataBind();
            }
            catch (Exception ex) { Msg("Error al cargar datos: " + ex.Message, "ee"); }
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

            ddlProyectosAprobados.DataSource = _manejador.ObtenerProyectosAprobadosSinEjecucion();
            ddlProyectosAprobados.DataTextField = "strTema_pro";
            ddlProyectosAprobados.DataValueField = "strId_pro";
            ddlProyectosAprobados.DataBind();
            ddlProyectosAprobados.Items.Insert(0, new ListItem("-- Seleccione --", ""));

            txtCoordinadorAdd.Text = "";
            txtFechaIniAdd.Text = DateTime.Now.ToString("yyyy-MM-dd");
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

                    strPeriodo_ejec = ddlCiclo.SelectedItem.Text, 
                    fkId_ciclo = int.Parse(ddlCiclo.SelectedValue),

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
                    strInforme_ejec = hfArchivoActual.Value
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
                case "Notificar":
                    EnviarRecordatorio(id);
                    break;
                case "Editar":
                    CargarEdicion(id);
                    break;
                case "Equipo":
                    CargarEquipo(id);
                    break;
                case "Informes":
                    CargarInformes(id);
                    break;
                case "Eliminar":
                    try
                    {
                        _manejador.EliminarEjecucion(id);
                        Redireccionar("Registro eliminado correctamente.", "ss");
                    }
                    catch (Exception ex) { Msg("Error al eliminar: " + ex.Message, "ee"); }
                    break;

                case "RenovarCiclo":
                    hfIdEjecRenovar.Value = id.ToString();

                    CargarCiclosFuturos(id);

                    ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalRenovar",
                        "new bootstrap.Modal(document.getElementById('modalRenovarCiclo')).show();", true);
                    break;
            }
        }

        // PERIODOS

        private void CargarCiclosFuturos(int idEjecucion)
        {
            try
            {
                ddlCiclosFuturos.Items.Clear();

                var proy = _manejador.ObtenerEjecucionPorId(idEjecucion);
                if (proy == null) return;

                DateTime fechaInicioProy = proy.dtFechaini_ejec;
                DateTime fechaFinTeoricaProyecto = CalcularFechaFinReal(fechaInicioProy, proy.strDuracion_pro);

                DateTime finCicloActual = proy.FinCicloActual != null
                    ? Convert.ToDateTime(proy.FinCicloActual)
                    : Convert.ToDateTime(proy.InicioCicloActual).AddMonths(6);

                var todosCiclos = _manejador.ObtenerCiclosConFechas();

                int contadorOpciones = 0;

                foreach (dynamic c in todosCiclos)
                {
                    int cId = (int)c.id_ciclo;
                    string cNombre = (string)c.strNombre_ciclo;
                    DateTime cInicio = Convert.ToDateTime(c.dtInicio_ciclo);

                    DateTime cFin = c.dtFin_ciclo != null
                        ? Convert.ToDateTime(c.dtFin_ciclo)
                        : cInicio.AddMonths(6);

                    bool esFuturo = cInicio > finCicloActual;

                    bool estaEnPlazo = cFin <= fechaFinTeoricaProyecto.AddDays(5); 

                    if (esFuturo && estaEnPlazo)
                    {
                        ddlCiclosFuturos.Items.Add(new ListItem(cNombre, cId.ToString()));
                        contadorOpciones++;
                    }
                }

                if (contadorOpciones == 0)
                {
                    ddlCiclosFuturos.Items.Add(new ListItem("No hay ciclos disponibles que se ajusten a la duración restante.", "0"));
                    ddlCiclosFuturos.Enabled = false;
                    btnConfirmarRenovacion.Enabled = false;
                }
                else
                {
                    ddlCiclosFuturos.Enabled = true;
                    btnConfirmarRenovacion.Enabled = true;
                    ddlCiclosFuturos.Items.Insert(0, new ListItem("-- Seleccione Siguiente Periodo --", "0"));
                }
            }
            catch (Exception ex)
            {
                Msg("Error al filtrar ciclos: " + ex.Message, "ee");
            }
        }

        protected void btnConfirmarRenovacion_Click(object sender, EventArgs e)
        {
            try
            {
                int idEjec = int.Parse(hfIdEjecRenovar.Value);
                int idNuevoCiclo = int.Parse(ddlCiclosFuturos.SelectedValue);
                string nombreCiclo = ddlCiclosFuturos.SelectedItem.Text;

                _manejador.RenovarCicloProyecto(idEjec, idNuevoCiclo, nombreCiclo);

                Msg("Ciclo renovado correctamente. Ya puede subir informes del nuevo periodo.", "ss");

                ScriptManager.RegisterStartupScript(this, GetType(), "CloseRenovar",
                    "bootstrap.Modal.getInstance(document.getElementById('modalRenovarCiclo')).hide();", true);

                CargarGrillaEjecucion();
            }
            catch (Exception ex)
            {
                Msg("Error al renovar: " + ex.Message, "ee");
            }
        }

        // FIN PERIODOS

        protected void rptEjecucion_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Literal lit = (Literal)e.Item.FindControl("litNotificacionPlazo");
                var btnEditar = (LinkButton)e.Item.FindControl("btnEditar");
                var btnEliminar = (LinkButton)e.Item.FindControl("btnEliminar");
                var btnRenovar = (LinkButton)e.Item.FindControl("btnRenovarCiclo");

                string estado = DataBinder.Eval(e.Item.DataItem, "strEstado_ejec")?.ToString().ToUpper().Trim() ?? "";

                if (estado == "FINALIZADO")
                {
                    if (btnEditar != null) { btnEditar.Enabled = false; btnEditar.CssClass += " btn-disabled-utc"; }
                    if (btnEliminar != null) { btnEliminar.Enabled = false; btnEliminar.CssClass += " btn-disabled-utc"; }
                }

                object rawInicio = DataBinder.Eval(e.Item.DataItem, "dtFechaini_ejec");
                object rawDuracion = DataBinder.Eval(e.Item.DataItem, "strDuracion_pro");
                object rawInicioCiclo = DataBinder.Eval(e.Item.DataItem, "InicioCicloActual");

                if (rawInicio != null && rawInicioCiclo != null && estado != "FINALIZADO")
                {
                    object rawFinCiclo = DataBinder.Eval(e.Item.DataItem, "FinCicloActual");

                    DateTime fechaInicioProy = Convert.ToDateTime(rawInicio);
                    string textoDuracion = rawDuracion?.ToString() ?? "";

                    DateTime fechaFinRealProyecto = CalcularFechaFinReal(fechaInicioProy, textoDuracion);

                    DateTime fechaFinCicloActual;

                    if (rawFinCiclo != null && rawFinCiclo != DBNull.Value)
                    {
                        fechaFinCicloActual = Convert.ToDateTime(rawFinCiclo);
                    }
                    else
                    {
                        DateTime inicioCiclo = Convert.ToDateTime(rawInicioCiclo);
                        fechaFinCicloActual = inicioCiclo.AddMonths(6);
                    }

                    bool cicloVencido = DateTime.Now > fechaFinCicloActual;
                    bool proyectoVigente = DateTime.Now < fechaFinRealProyecto;

                    if (cicloVencido && proyectoVigente)
                    {
                        if (btnRenovar != null)
                        {
                            btnRenovar.Visible = true; 
                        }

                        if (lit != null) lit.Text = "";
                    }
                    else if (!proyectoVigente)
                    {
                        if (lit != null)
                            lit.Text = "<span class='badge bg-secondary border'>Tiempo Finalizado</span>";

                        if (btnRenovar != null) btnRenovar.Visible = false;
                    }
                    else
                    {
                        if (btnRenovar != null) btnRenovar.Visible = false;

                        if (lit != null) lit.Text = "<span class='text-success small fw-bold'><i class='fa-solid fa-check'></i> Vigente</span>";
                    }
                }
            }
        }

        protected void rptMiembros_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                bool esFinalizado = ViewState["EsProyectoFinalizado"] != null && (bool)ViewState["EsProyectoFinalizado"];

                if (esFinalizado)
                {
                    var btnEditarM = (LinkButton)e.Item.FindControl("btnEditarM");
                    var btnToggle = (LinkButton)e.Item.FindControl("btnToggleEstado");
                    var btnEliminarM = (LinkButton)e.Item.FindControl("btnEliminarMiembro");

                    if (btnEditarM != null) btnEditarM.Visible = false;
                    if (btnToggle != null) btnToggle.Visible = false;
                    if (btnEliminarM != null) btnEliminarM.Visible = false;

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
                txtFechaFinEdit.Text = obj.dtFechafin_ejec?.ToString("yyyy-MM-dd") ?? "En Ejecución";
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

            var proyecto = _manejador.ObtenerEjecucionPorId(idEjecucion);

            string estado = proyecto.strEstado_ejec?.Trim().ToUpper() ?? "";
            bool esFinalizado = (estado == "FINALIZADO");

            btnAbrirFormMiembro.Visible = EsAdmin && !esFinalizado;

            ViewState["EsProyectoFinalizado"] = esFinalizado;

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
                // 1. VALIDACIONES BÁSICAS
                if (string.IsNullOrWhiteSpace(txtCedulaMiembro.Text) ||
                    string.IsNullOrWhiteSpace(txtNombresMiembro.Text) ||
                    string.IsNullOrWhiteSpace(txtCorreoMiembro.Text))
                {
                    Msg("Complete Cédula, Nombres y Correo.", "ww");
                    return;
                }

                // 2. LOGICA SEGÚN TIPO
                string tipo = ddlTipoMiembro.SelectedValue;
                string entidad = "";
                string facultad = "";
                string carrera = "";

                if (tipo == "Externo")
                {
                    entidad = txtEntidadMiembro.Text.Trim();
                    if (string.IsNullOrEmpty(entidad))
                    {
                        Msg("Debe especificar la Entidad para miembros externos.", "ww");
                        return;
                    }
                    facultad = "EXTERNO";
                }
                else
                {
                    facultad = ddlFacultadMiembro.SelectedValue;
                    carrera = txtCarreraMiembro.Text.Trim();
                }

                // 3. CONSTRUIR OBJETO
                var m = new InvgccEjecucionMiembros
                {
                    fkId_ejec = int.Parse(hfIdEjecucionEquipo.Value),
                    strCedula_miembro = txtCedulaMiembro.Text.Trim(),
                    strNombres_miembro = txtNombresMiembro.Text.Trim().ToUpper(),
                    strApellidos_miembro = txtApellidosMiembro.Text.Trim().ToUpper(),
                    strCorreo_miembro = txtCorreoMiembro.Text.Trim().ToLower(), // Nuevo
                    strTipo_miembro = tipo, // Nuevo
                    strRol_miembro = ddlRolMiembro.SelectedValue,

                    // Campos Condicionales
                    strEntidad_miembro = entidad,
                    strFacultad_miembro = facultad,
                    strCarrera_miembro = carrera
                };

                // 4. GUARDAR O ACTUALIZAR
                if (string.IsNullOrEmpty(hfIdMiembroEdit.Value))
                {
                    _manejador.GuardarMiembro(m);
                    SetFlashMessage("Integrante agregado correctamente.", "ss");
                }
                else
                {
                    m.strId_miembro = int.Parse(hfIdMiembroEdit.Value);
                    _manejador.ActualizarMiembro(m);
                    SetFlashMessage("Datos del integrante actualizados.", "ss");
                }

                // Redirección suave (mismo ID de team)
                Response.Redirect($"EjecucionProAprobados.aspx?idTeam={m.fkId_ejec}", false);
            }
            catch (Exception ex)
            {
                Msg("Error al guardar: " + ex.Message, "ee");
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


        private void CargarComboCiclos()
        {
            var lista = _manejador.ObtenerCiclos();

            ddlCiclo.DataSource = lista;
            ddlCiclo.DataTextField = "strNombre_ciclo";
            ddlCiclo.DataValueField = "id_ciclo";
            ddlCiclo.DataBind();

            ddlCiclo.Items.Insert(0, new ListItem("-- Seleccione Ciclo --", "0"));
        }

        protected void btnGuardarCiclo_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMesInicio.Text) || string.IsNullOrEmpty(txtMesFin.Text))
                {
                    Msg("Debe seleccionar ambas fechas.", "ww");
                    return;
                }

                DateTime inicio = DateTime.Parse(txtMesInicio.Text);

                DateTime finRaw = DateTime.Parse(txtMesFin.Text);
                DateTime fin = new DateTime(finRaw.Year, finRaw.Month, DateTime.DaysInMonth(finRaw.Year, finRaw.Month));

                _manejador.GuardarCiclo(inicio, fin);

                CargarComboCiclos();

                if (ddlCiclo.Items.Count > 1)
                    ddlCiclo.SelectedIndex = 1;

                Msg("Ciclo registrado correctamente.", "ss");

                string scriptCierre = @"
                    var el = document.getElementById('modalCrearCiclo');
                    var modal = bootstrap.Modal.getInstance(el);
                    if(modal){ modal.hide(); } else { new bootstrap.Modal(el).hide(); }";

                ScriptManager.RegisterStartupScript(this, GetType(), "CloseModalCiclo", scriptCierre, true);
            }
            catch (Exception ex)
            {
                Msg("Error al crear ciclo: " + ex.Message, "ee");
            }
        }

        // ==========================================
        // 5. GESTIÓN DE INFORMES
        // ==========================================

        private void CargarInformes(int idEjecucion)
        {
            try
            {
                hfIdEjecucionInforme.Value = idEjecucion.ToString();

                rptInformes.DataSource = _manejador.ObtenerInformes(idEjecucion);
                rptInformes.DataBind();

                ConfigurarBotonesFaseFinal(idEjecucion);
                BloquearGestionInformes(idEjecucion);

                ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalInf",
                    "var m = new bootstrap.Modal(document.getElementById('modalInformes')); m.show();", true);
            }
            catch (Exception ex)
            {
                Msg("Error al cargar el repositorio: " + ex.Message, "ee");
            }
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

                var checkProy = _manejador.ObtenerEjecucionPorId(idEjec);
                if (checkProy.strEstado_ejec == "FINALIZADO")
                {
                    Msg("Error: El proyecto está FINALIZADO. No se admiten cambios.", "ee");
                    return;
                }

                if (checkProy.FinCicloActual != null)
                {
                    DateTime finCiclo = Convert.ToDateTime(checkProy.FinCicloActual);

                    if (DateTime.Now > finCiclo)
                    {
                        Msg("BLOQUEADO: El periodo académico ha vencido (" + finCiclo.ToString("dd/MM/yyyy") + "). Debe RENOVAR el ciclo antes de subir informes.", "ee");

                        ScriptManager.RegisterStartupScript(this, GetType(), "Reopen", "AbrirModalInformes();", true);
                        return;
                    }
                }

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


        protected void btnInformeCierre_Click(object sender, EventArgs e)
        {
            string idEjecucionStr = hfIdEjecucionInforme.Value;

            if (!string.IsNullOrEmpty(idEjecucionStr) && int.TryParse(idEjecucionStr, out int idEjec))
            {
                var proyecto = _manejador.ObtenerEjecucionPorId(idEjec);
                string estado = proyecto.strEstado_ejec.ToUpper();
                bool tieneArchivo = !string.IsNullOrEmpty(proyecto.strInforme_Cierre);


                if (estado == "CIERRE APROBADO" || estado == "FINALIZADO")
                {
                    pnlCierreBloqueado.Visible = true;
                    divAlertaCierre.Visible = false;
                    pnlCargaCierre.Visible = false;
                    btnGuardarCierre.Visible = false;
                    btnAprobarCierre.Visible = false;

                    // Mostrar archivo para descargar
                    pnlArchivoCierreActual.Visible = true;
                    lblNombreArchivoCierre.Text = Path.GetFileName(proyecto.strInforme_Cierre);
                    lnkVerCierreActual.HRef = ResolveUrl(proyecto.strInforme_Cierre);
                }
                else
                {
                    pnlCierreBloqueado.Visible = false;
                    divAlertaCierre.Visible = true;
                    pnlCargaCierre.Visible = true;
                    btnGuardarCierre.Visible = true;

                    if (tieneArchivo)
                    {
                        pnlArchivoCierreActual.Visible = true;
                        lblNombreArchivoCierre.Text = Path.GetFileName(proyecto.strInforme_Cierre);
                        lnkVerCierreActual.HRef = ResolveUrl(proyecto.strInforme_Cierre);

                        lblTituloInputCierre.InnerText = "Sustituir Documento";
                        litBtnCierreTexto.Text = "Actualizar Archivo";

                        if (estado == "EN REVISION")
                        {
                            btnAprobarCierre.Visible = true;
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
                if (!flpCierre.HasFile)
                {
                    Msg("Debe adjuntar el informe de cierre.", "ww");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ReopenCierre",
                        "new bootstrap.Modal(document.getElementById('modalSubirCierre')).show();", true);
                    return;
                }

                if (!int.TryParse(hfIdEjecucionInforme.Value, out int idEjec))
                {
                    Msg("Error al identificar el proyecto.", "ee");
                    return;
                }

                string nombreArchivo = $"CIERRE_{DateTime.Now.Ticks}{Path.GetExtension(flpCierre.FileName)}";
                string rutaGuardada = GuardarArchivoFisico(flpCierre, nombreArchivo);

                string usuario = Session["UsuarioLogueado"]?.ToString() ?? "SISTEMA";
                _manejador.SubirInformeCierre(idEjec, rutaGuardada, usuario);

                string mensaje = pnlArchivoCierreActual.Visible
                    ? "Informe de cierre actualizado/corregido correctamente."
                    : "Informe de cierre subido. El proyecto está EN REVISIÓN.";

                Msg(mensaje, "ss");

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

                    _manejador.AprobarCierre(idEjec, usuario);

                    string emailDestino = ObtenerCorreoCoordinador(idEjec);
                    if (!string.IsNullOrEmpty(emailDestino))
                    {
                        string cuerpo = @"
                    <p>Estimado Investigador,</p>
                    <p>Le informamos que su <strong>Informe de Cierre ha sido APROBADO</strong> por la Dirección de Investigación.</p>
                    <p>El sistema ha habilitado la opción para cargar su <strong>Informe Final</strong>. Por favor, proceda con este paso para culminar el proceso.</p>
                    <a href='https://tudominio.utc.edu.ec/Login.aspx' style='background: #198754; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Ir al Sistema</a>";

                        SistemaGestionCGI.Utilidades.EmailService.EnviarCorreo(
                            emailDestino,
                            "Notificación UTC: Informe de Cierre Aprobado",
                            cuerpo,
                            "ACTUALIZACIÓN DE ESTADO"
                        );
                    }

                    Msg("Documento APROBADO correctamente. Se ha habilitado la fase final.", "ss");

                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseAllApprove",
                        "bootstrap.Modal.getInstance(document.getElementById('modalSubirCierre')).hide(); $('#modalInformes').modal('hide');", true);

                    CargarGrillaEjecucion();
                }
            }
            catch (Exception ex)
            {
                Msg("Error al aprobar: " + ex.Message, "ee");
            }
        }

        protected void btnInformeFinal_Click(object sender, EventArgs e)
        {
            string idEjecucionStr = hfIdEjecucionInforme.Value;
            if (!string.IsNullOrEmpty(idEjecucionStr) && int.TryParse(idEjecucionStr, out int idEjec))
            {
                var proyecto = _manejador.ObtenerEjecucionPorId(idEjec);
                string estado = proyecto.strEstado_ejec.ToUpper();
                bool tieneArchivo = !string.IsNullOrEmpty(proyecto.strInforme_Final);

                if (estado == "FINALIZADO")
                {
                    pnlCargaFinal.Visible = false;
                    btnGuardarFinal.Visible = false;

                    if (tieneArchivo)
                    {
                        pnlArchivoFinalActual.Visible = true;
                        lblNombreArchivoFinal.Text = Path.GetFileName(proyecto.strInforme_Final);
                        lnkVerFinalActual.HRef = ResolveUrl(proyecto.strInforme_Final);
                    }
                }
                else
                {
                    pnlCargaFinal.Visible = true;
                    btnGuardarFinal.Visible = true;
                    pnlArchivoFinalActual.Visible = false;
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalFinal",
                    "new bootstrap.Modal(document.getElementById('modalSubirFinal')).show();", true);
            }
        }

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
            var proyecto = _manejador.ObtenerEjecucionPorId(idEjecucion);

            if (proyecto == null) return;

            string estado = proyecto.strEstado_ejec.ToUpper();
            bool tieneCierre = !string.IsNullOrEmpty(proyecto.strInforme_Cierre);

            // ====================================================
            // LÓGICA DE BLOQUEO (STATE MACHINE)
            // ====================================================

            btnInformeCierre.CssClass = "btn btn-white border shadow-sm p-3 text-start position-relative hover-lift";
            btnInformeCierre.Enabled = true;

            if (estado == "CIERRE APROBADO" || estado == "FINALIZADO")
            {
                btnInformeFinal.CssClass = "btn btn-white border shadow-sm p-3 text-start hover-lift";
                btnInformeFinal.Enabled = true;
                btnInformeFinal.Attributes.Remove("title");
            }
            else
            {
                btnInformeFinal.CssClass = "btn btn-white border shadow-sm p-3 text-start hover-lift btn-locked";
                btnInformeFinal.Enabled = false;
                btnInformeFinal.Attributes.Add("title", "Disponible solo cuando el Informe de Cierre sea APROBADO.");
            }

            if (tieneCierre)
            {
                btnInformeCierre.Style["border-left"] = "5px solid var(--utc-verde) !important";
            }
            else
            {
                btnInformeCierre.Style["border-left"] = "5px solid var(--utc-azul) !important";
            }
        }

        private void BloquearGestionInformes(int idEjecucion)
        {
            var proyecto = _manejador.ObtenerEjecucionPorId(idEjecucion);
            if (proyecto == null) return;

            DateTime? fechaFinCiclo = null;

            if (proyecto.FinCicloActual != null)
            {
                fechaFinCiclo = Convert.ToDateTime(proyecto.FinCicloActual);
            }
            else if (proyecto.InicioCicloActual != null)
            {
                fechaFinCiclo = Convert.ToDateTime(proyecto.InicioCicloActual).AddMonths(6);
            }

            bool esPeriodoVencido = fechaFinCiclo != null && DateTime.Now.Date > fechaFinCiclo.Value.Date;
            string estado = proyecto.strEstado_ejec?.ToUpper() ?? "";


            if (estado == "FINALIZADO")
            {
                DesactivarControlesDeSubida();
            }
            else if (esPeriodoVencido)
            {
                DesactivarControlesDeSubida();

                btnInformeCierre.Enabled = false;
                btnInformeCierre.CssClass += " btn-locked opacity-50";
                btnInformeCierre.ToolTip = "Debe renovar el periodo académico para realizar acciones.";

                btnInformeFinal.Enabled = false;
                btnInformeFinal.CssClass += " btn-locked opacity-50";
            }
            else
            {
                btnAbrirGenerador.Visible = true;
                btnSubirEscaneado.Visible = true;

                btnInformeCierre.Enabled = true;
                btnInformeCierre.CssClass = btnInformeCierre.CssClass.Replace(" btn-locked opacity-50", "");
            }
        }

        private void DesactivarControlesDeSubida()
        {
            btnAbrirGenerador.Visible = false;
            btnSubirEscaneado.Visible = false;

            foreach (RepeaterItem item in rptInformes.Items)
            {
                var btnEdit = item.FindControl("btnEditarInf") as LinkButton;
                var btnDel = item.FindControl("btnEliminarInf") as LinkButton;

                if (btnEdit != null) btnEdit.Visible = false;
                if (btnDel != null) btnDel.Visible = false;
            }
        }

        private DateTime? ObtenerFechaFinDelTexto(string periodoTexto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(periodoTexto) || !periodoTexto.Contains("-")) return null;

                string parteFinal = periodoTexto.Split('-')[1].Trim();
                string[] partes = parteFinal.Split(' ');

                if (partes.Length < 2) return null;

                string mesNombre = partes[0].ToUpper();
                int anio = int.Parse(partes[1]);
                int mes = 1;

                switch (mesNombre)
                {
                    case "ENERO": mes = 1; break;
                    case "FEBRERO": mes = 2; break;
                    case "MARZO": mes = 3; break;
                    case "ABRIL": mes = 4; break;
                    case "MAYO": mes = 5; break;
                    case "JUNIO": mes = 6; break;
                    case "JULIO": mes = 7; break;
                    case "AGOSTO": mes = 8; break;
                    case "SEPTIEMBRE": mes = 9; break;
                    case "OCTUBRE": mes = 10; break;
                    case "NOVIEMBRE": mes = 11; break;
                    case "DICIEMBRE": mes = 12; break;
                }

                return new DateTime(anio, mes, DateTime.DaysInMonth(anio, mes));
            }
            catch { return null; }
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
            sb.Append("</div>");
            sb.Append("</div>");

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
            string colorEstado = miembro.bitActivo_miembro ? "#198754" : "#dc3545";
            sb.Append($"<span class='value' style='color:{colorEstado}'>{estado}</span>");
            sb.Append("</div>");
            sb.Append("</div>");

            sb.Append("</div>");

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

                    sb.Append("</div>");
                    sb.Append("</div>");
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
                ucGenerador.Mostrar(id);
            }
            else
            {
                Msg("Seleccione un proyecto primero (Botón Informes)", "ww");
            }
        }

        protected void ucGenerador_InformeGuardado(object sender, EventArgs e)
        {
            if (int.TryParse(hfIdEjecucionInforme.Value, out int id))
            {
                CargarInformes(id);
                Msg("Documento generado y guardado correctamente.", "ss");
            }
        }

        // ==========================================
        // 7. UTILIDADES
        // ==========================================

        private void ConfigurarPermisosModalInformes()
        {
            tituloEtapaFinal.Visible = true;
            divContenedorBotonesFinales.Visible = true;
            btnInformeCierre.Visible = true;
            btnInformeFinal.Visible = true;
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
            string cleanMsg = msg.Replace("'", "\\'").Replace("\r\n", " ").Replace("\n", " ");

            string script = $"$(function() {{ toastify('{type}', '{cleanMsg}', 'Sistema'); }});";

            ScriptManager.RegisterStartupScript(this, GetType(), "alert", script, true);
        }


        private string ObtenerCorreoCoordinador(int idEjecucion)
        {
            return _manejador.ObtenerEmailCoordinador(idEjecucion);
        }

        private void EnviarRecordatorio(int idEjecucion)
        {
            try
            {
                var proy = _manejador.ObtenerEjecucionPorId(idEjecucion);
                string emailDestino = ObtenerCorreoCoordinador(idEjecucion);

                if (string.IsNullOrWhiteSpace(emailDestino))
                {
                    Msg("El coordinador no tiene un correo registrado en la base de datos de Integrantes.", "ww");
                    return;
                }

                string cuerpo = $@"
            <p>Estimado/a <strong>{proy.strCoordinador_ejec}</strong>,</p>
            <p>Se ha detectado que el proyecto <em>""{proy.TituloProyecto}""</em> presenta retrasos en la entrega de evidencias o informes de avance.</p>
            <p style='color: #d9534f; font-weight: bold;'>Por favor, acceda al sistema y regularice su documentación lo antes posible.</p>";

                // Aquí capturamos el resultado del EmailService
                bool enviado = SistemaGestionCGI.Utilidades.EmailService.EnviarCorreo(
                    emailDestino,
                    "ALERTA UTC: Recordatorio de Informes Pendientes",
                    cuerpo,
                    "RECORDATORIO DE AVANCE"
                );

                if (enviado)
                    Msg("Recordatorio enviado correctamente a: " + emailDestino, "ss");
                else
                    // Si el EmailService devolvió false, es un problema de Gmail/Web.config
                    Msg("Error SMTP: No se pudo conectar con el servidor de correo. Revise el Web.config.", "ee");
            }
            catch (Exception ex)
            {
                // Esto captura errores de código (ej: nulos o formato)
                Msg("Error al procesar la notificación: " + ex.Message, "ee");
            }
        }

        //
        // ==========================================
        // LÓGICA DE CÁLCULO DE FECHAS (NUEVO)
        // ==========================================
        private DateTime CalcularFechaFinReal(DateTime fechaInicio, string textoDuracion)
        {
            if (string.IsNullOrEmpty(textoDuracion)) return fechaInicio;

            DateTime fechaCalculada = fechaInicio;
            string duracionNorm = textoDuracion.ToLower();

            // 1. Extraer AÑOS
            var matchAnios = Regex.Match(duracionNorm, @"(\d+)\s*año");
            if (matchAnios.Success) fechaCalculada = fechaCalculada.AddYears(int.Parse(matchAnios.Groups[1].Value));

            // 2. Extraer MESES
            var matchMeses = Regex.Match(duracionNorm, @"(\d+)\s*mes");
            if (matchMeses.Success) fechaCalculada = fechaCalculada.AddMonths(int.Parse(matchMeses.Groups[1].Value));

            // 3. Extraer SEMANAS
            var matchSemanas = Regex.Match(duracionNorm, @"(\d+)\s*semana");
            if (matchSemanas.Success) fechaCalculada = fechaCalculada.AddDays(int.Parse(matchSemanas.Groups[1].Value) * 7);

            // 4. Extraer DÍAS
            var matchDias = Regex.Match(duracionNorm, @"(\d+)\s*día");
            if (!matchDias.Success) matchDias = Regex.Match(duracionNorm, @"(\d+)\s*dia");
            if (matchDias.Success) fechaCalculada = fechaCalculada.AddDays(int.Parse(matchDias.Groups[1].Value));

            return fechaCalculada;
        }

    }
}