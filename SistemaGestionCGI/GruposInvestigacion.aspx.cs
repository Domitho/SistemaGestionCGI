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
        // =============================================
        // 1. CONFIGURACIÓN Y CARGA INICIAL
        // =============================================
        private readonly ManejadorGruposInvestigacion _manejador = new ManejadorGruposInvestigacion();
        private const string RUTA_VIRTUAL_BASE = "~/RepositorioUTC/Grupos/";

        protected void Page_Load(object sender, EventArgs e)
        {
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

            if (IsPostBack) return;

            try
            {
                CargarCombosGlobales();
                CargarGrillaGrupos();

                string idGrupoRedirect = Request.QueryString["idGrupo"];
                if (!string.IsNullOrEmpty(idGrupoRedirect))
                {
                    CargarIntegrantesPanel(idGrupoRedirect);
                }
            }
            catch (Exception ex) { Msg("Error al iniciar módulo: " + ex.Message, "ee"); }

            if (Session["TempMsg"] != null)
            {
                Msg(Session["TempMsg"].ToString(), Session["TempTipo"].ToString());
                Session["TempMsg"] = null;
                Session["TempTipo"] = null;
            }
        }

        private void CargarCombosGlobales()
        {
            var centros = _manejador.ObtenerCentrosCombo();
            ddlCentro.DataSource = centros;
            ddlCentro.DataTextField = "strNombre_cen";
            ddlCentro.DataValueField = "strId_cen";
            ddlCentro.DataBind();
            ddlCentro.Items.Insert(0, new ListItem("-- Seleccione Centro --", ""));
        }

        private void CargarGrillaGrupos()
        {
            rptGrupoInv.DataSource = _manejador.ObtenerGrupos();
            rptGrupoInv.DataBind();
        }

        // =============================================
        // 2. GESTIÓN DE GRUPOS (CRUD UNIFICADO)
        // =============================================

        protected void btnNuevoGrupo_Click(object sender, EventArgs e)
        {
            LimpiarFormularioGrupo();
            lblTituloFormulario.Text = "Registrar Nuevo Grupo";
            CambiarVista(Vista.FormularioGrupo);
        }

        protected void btnGuardarGrupo_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNombreGru.Text))
                {
                    Msg("El nombre del grupo es obligatorio.", "ww");
                    return;
                }

                var g = new InvgccGrupoInvestigacion
                {
                    strNombre_gru = txtNombreGru.Text.Trim(),
                    fkId_cen = (ddlCentro.SelectedValue == "") ? null : ddlCentro.SelectedValue,
                    strCoordinador_gru = txtCoordinadorGru.Text.Trim(),
                    strCategoria_gru = ddlCategoriaGru.SelectedValue,
                    strLineasinv_gru = ddlLineaInv.SelectedValue,
                    strSublineasinv_gru = ddlSublineaInv.SelectedValue,

                    dtFechacrea_gru = !string.IsNullOrEmpty(txtFechaCreaGru.Text)
                                    ? DateTime.Parse(txtFechaCreaGru.Text)
                                    : DateTime.Now,

                    strFoto_gru = hfFotoActual.Value,
                    strArchivo_gru = hfArchivoActual.Value
                };

                if (flpFotoGrupo.HasFile)
                {
                    string nombre = $"FOTO_{DateTime.Now.Ticks}{Path.GetExtension(flpFotoGrupo.FileName)}";
                    g.strFoto_gru = GuardarArchivoFisico(flpFotoGrupo, "FOTOS", nombre);
                }

                if (flpArchivoGrupo.HasFile)
                {
                    string nombre = $"DOC_{DateTime.Now.Ticks}{Path.GetExtension(flpArchivoGrupo.FileName)}";
                    g.strArchivo_gru = GuardarArchivoFisico(flpArchivoGrupo, "DOCUMENTOS", nombre);
                }

                string usuario = Session["UsuarioLogueado"]?.ToString() ?? "Sistema";


                if (string.IsNullOrEmpty(hfIdGrupo.Value))
                {
                    if (!string.IsNullOrEmpty(hfCoordCedula.Value))
                    {
                        var coord = new InvgccGrupoIntegrantes
                        {
                            strNombres_int = hfCoordNombre.Value,
                            strApellidos_int = hfCoordApellidos.Value,
                            strCedula_int = hfCoordCedula.Value,
                            strCorreo_int = hfCoordCorreo.Value,

                            strTipo_int = hfCoordTipo.Value, 
                            strEntidad_int = string.IsNullOrEmpty(hfCoordEntidad.Value) ? null : hfCoordEntidad.Value, // Nuevo
                            strFacultad_int = string.IsNullOrEmpty(hfCoordFacultad.Value) ? null : hfCoordFacultad.Value,
                            strCarrera_int = string.IsNullOrEmpty(hfCoordCarrera.Value) ? null : hfCoordCarrera.Value,

                            strCertificado_int = hfCoordArchivo.Value,
                            fkId_docente_origen = string.IsNullOrEmpty(hfCoordIdDocente.Value) ? null : hfCoordIdDocente.Value, // Nuevo FK

                            strFuncion_int = "INVESTIGADOR PRINCIPAL",
                            bitActivo_int = true
                        };

                        _manejador.RegistrarGrupoConCoordinador(g, coord, usuario);

                        SetFlashMessage("Grupo creado y Coordinador asignado correctamente.", "ss");
                    }
                    else
                    {
                        _manejador.GuardarGrupo(g);
                        SetFlashMessage("Grupo creado exitosamente (Sin coordinador).", "ss");
                    }
                }
                else
                {
                    g.strId_gru = hfIdGrupo.Value;
                    _manejador.ActualizarGrupo(g);
                    SetFlashMessage("Datos del grupo actualizados.", "ss");
                }

                Response.Redirect("GruposInvestigacion.aspx", false);
            }
            catch (Exception ex)
            {
                Msg("Error al guardar: " + ex.Message, "ee");
            }
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            Response.Redirect("GruposInvestigacion.aspx");
        }

        protected void rptGrupoInv_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();

            switch (e.CommandName)
            {
                case "Editar":
                    CargarEdicionGrupo(id);
                    break;

                case "Eliminar":
                    try
                    {
                        _manejador.EliminarGrupo(id);
                        Redireccionar("Grupo eliminado.", "ss");
                    }
                    catch (Exception ex) { Msg("No se puede eliminar: " + ex.Message, "ee"); }
                    break;

                case "VerIntegrantes":
                    Response.Redirect($"GruposInvestigacion.aspx?idGrupo={id}", false);
                    break;

                case "VerProyectos":
                    CargarModalProyectos(id);
                    break;

                case "Archivo":
                    var grupo = _manejador.ObtenerGrupoPorId(id);
                    if (grupo != null && !string.IsNullOrEmpty(grupo.strArchivo_gru))
                    {
                        VisualizarArchivo(grupo.strArchivo_gru);
                    }
                    else
                    {
                        Msg("No hay archivo adjunto para este grupo.", "ww");
                    }
                    break;
            }
        }

        private void CargarEdicionGrupo(string id)
        {
            var g = _manejador.ObtenerGrupoPorId(id);
            if (g == null) return;

            lblTituloFormulario.Text = $"Editar Grupo: {g.strId_gru}";
            hfIdGrupo.Value = g.strId_gru;

            txtNombreGru.Text = g.strNombre_gru;
            txtCoordinadorGru.Text = g.strCoordinador_gru;
            txtFechaCreaGru.Text = g.dtFechacrea_gru.ToString("yyyy-MM-dd");

            if (ddlCentro.Items.FindByValue(g.fkId_cen) != null)
                ddlCentro.SelectedValue = g.fkId_cen;

            if (ddlCategoriaGru.Items.FindByValue(g.strCategoria_gru) != null)
                ddlCategoriaGru.SelectedValue = g.strCategoria_gru;

            hfFotoActual.Value = g.strFoto_gru;
            hfArchivoActual.Value = g.strArchivo_gru;

            if (!string.IsNullOrEmpty(g.strFoto_gru))
            {
                imgFotoActual.ImageUrl = ObtenerImagenBase64(g.strFoto_gru);
                imgFotoActual.Visible = true;
            }
            else
            {
                imgFotoActual.Visible = false;
            }

            CambiarVista(Vista.FormularioGrupo);
        }

        protected void btnAgregarCoordinador_Click(object sender, EventArgs e)
        {
            txtCedulaCoord.Text = ""; txtNombreCoord.Text = ""; txtApellidoCoord.Text = "";
            txtCorreoCoord.Text = ""; txtCarreraCoord.Text = ""; txtEntidadCoord.Text = "";
            ddlFacultadCoord.SelectedIndex = 0;
            ddlTipoCoord.SelectedIndex = 0;

            hfCoordIdDocente.Value = "";

            pnlFormularioGrupo.Visible = true;
            ScriptManager.RegisterStartupScript(this, GetType(), "Open", "abrirModalCoord(); toggleTipoCoordinador();", true);
        }

        protected void btnGuardarCoordModal_Click(object sender, EventArgs e)
        {
            pnlFormularioGrupo.Visible = true;

            if (!flpArchivoCoord.HasFile)
            {
                Msg("Adjunte la resolución (PDF).", "ww");
                ScriptManager.RegisterStartupScript(this, GetType(), "ReOpenF", "abrirModalCoord(); toggleTipoCoordinador();", true);
                return;
            }

            try
            {
                string tipo = ddlTipoCoord.SelectedValue;

                // Guardar Archivo
                string nombreArchivo = "CERT_" + DateTime.Now.Ticks + "_" + flpArchivoCoord.FileName;
                string rutaFinal = GuardarArchivoFisico(flpArchivoCoord, "CERTIFICADOS", nombreArchivo);

                // Llenar HiddenFields Comunes
                hfCoordArchivo.Value = rutaFinal;
                hfCoordNombre.Value = txtNombreCoord.Text.ToUpper().Trim();
                hfCoordApellidos.Value = txtApellidoCoord.Text.ToUpper().Trim();
                hfCoordCedula.Value = txtCedulaCoord.Text.Trim();
                hfCoordCorreo.Value = txtCorreoCoord.Text.ToLower().Trim();
                hfCoordTipo.Value = tipo;

                // Lógica Específica
                if (tipo == "Externo")
                {
                    if (string.IsNullOrEmpty(txtEntidadCoord.Text)) throw new Exception("Ingrese la entidad externa.");

                    hfCoordEntidad.Value = txtEntidadCoord.Text.ToUpper().Trim();
                    hfCoordFacultad.Value = "";
                    hfCoordCarrera.Value = "";
                    hfCoordIdDocente.Value = ""; // Externo no tiene ID Docente
                }
                else
                {
                    hfCoordCarrera.Value = txtCarreraCoord.Text.ToUpper().Trim();
                    hfCoordFacultad.Value = ddlFacultadCoord.SelectedValue;
                    hfCoordEntidad.Value = "";

                    // Si es "Interno Manual", nos aseguramos que ID Docente vaya vacío para no romper la FK
                    if (tipo == "Interno") hfCoordIdDocente.Value = "";
                }

                // Feedback
                txtCoordinadorGru.Text = $"{txtApellidoCoord.Text} {txtNombreCoord.Text}";

                ScriptManager.RegisterStartupScript(this, GetType(), "Close", "cerrarModalCoord();", true);
                Msg("Coordinador asignado.", "ss");
            }
            catch (Exception ex)
            {
                Msg(ex.Message, "ee");
                ScriptManager.RegisterStartupScript(this, GetType(), "ReOpenE", "abrirModalCoord(); toggleTipoCoordinador();", true);
            }
        }

        protected void btnBuscarDocente_Click(object sender, EventArgs e)
        {
            pnlFormularioGrupo.Visible = true;
            string cedula = txtBuscarCedulaDoc.Text.Trim();
            if (string.IsNullOrEmpty(cedula)) return;

            try
            {
                // Llamada a la BLL
                dynamic docente = _manejador.ObtenerDocenteCategorizado(cedula);

                if (docente != null)
                {
                    // Usamos Reflection seguro para leer propiedades dinámicas
                    txtCedulaCoord.Text = GetProp(docente, "strCedula_doc");
                    txtNombreCoord.Text = GetProp(docente, "strNombres_doc");
                    txtApellidoCoord.Text = GetProp(docente, "strApellidos_doc");
                    txtCarreraCoord.Text = GetProp(docente, "strCarrera_doc");

                    string fac = GetProp(docente, "strFacultad_doc");
                    if (ddlFacultadCoord.Items.FindByValue(fac) != null)
                        ddlFacultadCoord.SelectedValue = fac;

                    // GUARDAMOS EL ID DEL DOCENTE EN HIDDENFIELD
                    hfCoordIdDocente.Value = GetProp(docente, "strId_doc");

                    Msg("Docente encontrado.", "ss");
                }
                else
                {
                    Msg("Docente no encontrado.", "ww");
                    hfCoordIdDocente.Value = ""; // Limpiar si no encuentra
                }
            }
            catch (Exception ex) { Msg("Error búsqueda: " + ex.Message, "ee"); }
            finally
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ReOpenBusc", "abrirModalCoord(); toggleTipoCoordinador();", true);
            }
        }

        private string GetProp(object obj, string propName)
        {
            if (obj == null) return "";
            try
            {
                // Opción A: Si el DAL devuelve un JObject (Newtonsoft)
                if (obj is Newtonsoft.Json.Linq.JObject jobj)
                    return jobj[propName]?.ToString() ?? "";

                // Opción B: Si es un objeto estándar (Reflection)
                return obj.GetType().GetProperty(propName)?.GetValue(obj, null)?.ToString() ?? "";
            }
            catch
            {
                return "";
            }
        }

        // =============================================
        // 3. GESTIÓN DE INTEGRANTES
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
                    Msg("Cédula y Nombres son obligatorios.", "ww");
                    return;
                }

                if (string.IsNullOrEmpty(hfIdIntEdit.Value))
                {
                    string grupoExistente = _manejador.VerificarIntegranteEnOtroGrupo(txtCedulaInt.Text.Trim());
                    if (grupoExistente != null)
                    {
                        Msg($"La cédula ya pertenece al grupo: {grupoExistente}.", "ee");
                        return;
                    }
                }

                var i = new InvgccGrupoIntegrantes
                {
                    fkId_gru = hfGrupoIdActual.Value,
                    strCedula_int = txtCedulaInt.Text.Trim(),
                    strNombres_int = txtNombresInt.Text.Trim(),
                    strApellidos_int = txtApellidosInt.Text.Trim(),
                    strCorreo_int = txtCorreoInt.Text.Trim(),
                    strFuncion_int = ddlFuncionInt.SelectedValue,
                    strTipo_int = ddlTipoInt.SelectedValue,
                    dtFechaini_int = !string.IsNullOrEmpty(dtFechaIniInt.Text) ? DateTime.Parse(dtFechaIniInt.Text) : DateTime.Now,
                    strCertificado_int = hfCertificadoIntActual.Value
                };

                if (i.strTipo_int == "Externo")
                {
                    if (string.IsNullOrWhiteSpace(txtEntidadInt.Text))
                    {
                        Msg("Debe indicar la Entidad de Origen.", "ww");
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

                if (flpCertificadoInt.HasFile)
                {
                    string nombre = $"CERT_{DateTime.Now.Ticks}{Path.GetExtension(flpCertificadoInt.FileName)}";
                    i.strCertificado_int = GuardarArchivoFisico(flpCertificadoInt, "CERTIFICADOS", nombre);
                }

                string usuario = Session["UsuarioLogueado"]?.ToString() ?? "Sistema";

                if (string.IsNullOrEmpty(hfIdIntEdit.Value))
                {
                    _manejador.GuardarIntegrante(i, usuario);
                    SetFlashMessage("Integrante agregado correctamente.", "ss");
                }
                else
                {
                    i.strId_int = hfIdIntEdit.Value;
                    var original = _manejador.ObtenerIntegrantePorId(i.strId_int);

                    if (original != null)
                    {
                        i.dtFechafin_int = original.dtFechafin_int;
                        i.bitActivo_int = original.bitActivo_int;
                    }

                    _manejador.ActualizarIntegrante(i, usuario);
                    SetFlashMessage("Datos del integrante actualizados.", "ss");
                }

                Response.Redirect($"GruposInvestigacion.aspx?idGrupo={hfGrupoIdActual.Value}", false);
            }
            catch (Exception ex) { Msg("Error integrante: " + ex.Message, "ee"); }
        }

        protected void rptIntegrantes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string idInt = e.CommandArgument.ToString();

            switch (e.CommandName)
            {
                case "EditarInt":
                    CargarEdicionIntegrante(idInt);
                    break;

                case "CambiarEstado":
                    CargarModalEstado(idInt);
                    break;

                case "Historial":
                    CargarModalHistorial(idInt);
                    break;

                case "VerCertificado":
                    string rutaCertificado = idInt;

                    if (!string.IsNullOrEmpty(rutaCertificado))
                    {
                        VisualizarArchivo(rutaCertificado);
                    }
                    else
                    {
                        Msg("No hay certificado cargado.", "ww");
                    }
                    break;
            }
        }

        private void CargarEdicionIntegrante(string id)
        {
            var i = _manejador.ObtenerIntegrantePorId(id);
            if (i == null) return;

            hfIdIntEdit.Value = i.strId_int;
            lblTituloFormInt.Text = "Editar Integrante";

            txtCedulaInt.Text = i.strCedula_int;
            txtNombresInt.Text = i.strNombres_int;
            txtApellidosInt.Text = i.strApellidos_int;
            txtCorreoInt.Text = i.strCorreo_int;
            dtFechaIniInt.Text = i.dtFechaini_int.ToString("yyyy-MM-dd");

            if (ddlFuncionInt.Items.FindByValue(i.strFuncion_int) != null)
                ddlFuncionInt.SelectedValue = i.strFuncion_int;

            hfCertificadoIntActual.Value = i.strCertificado_int;

            if (ddlTipoInt.Items.FindByValue(i.strTipo_int) != null)
                ddlTipoInt.SelectedValue = i.strTipo_int;

            if (i.strTipo_int == "Externo")
            {
                txtEntidadInt.Text = i.strEntidad_int;
            }
            else
            {
                txtCarreraInt.Text = i.strCarrera_int;
                if (ddlFacultadInt.Items.FindByValue(i.strFacultad_int) != null)
                    ddlFacultadInt.SelectedValue = i.strFacultad_int;
            }

            CambiarVista(Vista.FormularioIntegrante);
            ScriptManager.RegisterStartupScript(this, GetType(), "initForm", "InitFormulario();", true);
        }

        protected void btnCancelarInt_Click(object sender, EventArgs e)
        {
            CambiarVista(Vista.ListaIntegrantes);
        }

        protected void btnVolverGrupos_Click(object sender, EventArgs e)
        {
            Response.Redirect("GruposInvestigacion.aspx");
        }

        // =============================================
        // 4. MODALS (PROYECTOS, ESTADO, HISTORIAL)
        // =============================================

        private void CargarModalProyectos(string idGrupo)
        {
            try
            {
                var lista = _manejador.ObtenerProyectosDeGrupo(idGrupo);
                var grupo = _manejador.ObtenerGrupoPorId(idGrupo);

                if (grupo != null)
                    lblGrupoTitulo.InnerText = $"PORTAFOLIO: {grupo.strNombre_gru}";

                if (lista != null && lista.Count > 0)
                {
                    rptProyectosDetalle.DataSource = lista;
                    rptProyectosDetalle.DataBind();

                    rptProyectosDetalle.Visible = true;
                    pnlSinProyectos.Visible = false; 
                }
                else
                {
                    rptProyectosDetalle.Visible = false;
                    pnlSinProyectos.Visible = true;
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalProy",
                    "var m = new bootstrap.Modal(document.getElementById('modalProyectosDetalle')); m.show();", true);
            }
            catch (Exception ex) { Msg("Error al cargar proyectos: " + ex.Message, "ee"); }
        }

        private void CargarModalEstado(string idInt)
        {
            hfIdIntegranteEstado.Value = idInt;
            var i = _manejador.ObtenerIntegrantePorId(idInt);

            if (i != null)
            {
                string estado = i.bitActivo_int ? "Activo" : "Inactivo";
                string accion = i.bitActivo_int ? "dar de baja" : "reactivar";

                string script = $@"
                    document.getElementById('txtMotivoEstado').value = '';
                    document.getElementById('infoNombre').innerText = '{i.strNombres_int} {i.strApellidos_int}';
                    document.getElementById('infoCedula').innerText = '{i.strCedula_int}';
                    document.getElementById('infoFuncion').innerText = '{i.strFuncion_int}';
                    document.getElementById('infoEstado').innerText = '{estado}';
                    document.getElementById('accionEstadoTexto').innerText = '{accion}';
                    AbrirModalEstado();";

                ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalEst", script, true);
            }
        }

        protected void btnConfirmarCambioEstado_Click(object sender, EventArgs e)
        {
            try
            {
                string idInt = hfIdIntegranteEstado.Value;
                string motivo = hfMotivoEstado.Value;
                string usuario = Session["UsuarioLogueado"]?.ToString() ?? "Sistema";

                var i = _manejador.ObtenerIntegrantePorId(idInt);
                _manejador.CambiarEstadoIntegrante(idInt, !i.bitActivo_int, motivo, usuario);

                SetFlashMessage("Estado actualizado correctamente.", "ss");
                Response.Redirect($"GruposInvestigacion.aspx?idGrupo={hfGrupoIdActual.Value}", false);
            }
            catch (Exception ex) { Msg("Error cambio estado: " + ex.Message, "ee"); }
        }

        private void CargarModalHistorial(string idInt)
        {
            var i = _manejador.ObtenerIntegrantePorId(idInt);
            if (i != null)
            {
                lblNombreHistorial.Text = $"{i.strNombres_int} {i.strApellidos_int}";
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

                // -- ASIGNACIÓN DE DATOS AL DISEÑO NUEVO --
                lblRefId.Text = integrante.strId_int;
                lblReporteNombre.Text = $"{integrante.strApellidos_int} {integrante.strNombres_int}";
                lblReporteCedula.Text = integrante.strCedula_int;
                lblReporteFuncion.Text = integrante.strFuncion_int;

                lblReporteEstado.Text = integrante.bitActivo_int ? "ACTIVO" : "INACTIVO";
                // Cambio de color dinámico para el estado
                lblReporteEstado.ForeColor = integrante.bitActivo_int ?
                    System.Drawing.ColorTranslator.FromHtml("#1b9e4b") :
                    System.Drawing.ColorTranslator.FromHtml("#d9534f");

                // Llenar el Timeline
                rptReporteHistorial.DataSource = historial;
                rptReporteHistorial.DataBind();

                // Abrir Modal
                string script = "var m = new bootstrap.Modal(document.getElementById('modalVistaPrevia')); m.show();";
                ScriptManager.RegisterStartupScript(this, GetType(), "OpenPreview", script, true);
            }
            catch (Exception ex)
            {
                Msg("Error: " + ex.Message, "ee");
            }
        }

        private string ConstruirHtmlReporte(InvgccGrupoIntegrantes integrante, List<InvgccIntegrantesHistorial> historial)
        {
            StringBuilder sb = new StringBuilder();

            // --- ENCABEZADO DEL REPORTE ---
            sb.Append("<div class='text-center mb-4'>");
            sb.Append("<h4 class='text-uppercase fw-bold'>Reporte de Movimientos</h4>");
            sb.Append($"<p class='text-muted small'>Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}</p>");
            sb.Append("</div>");

            // --- DATOS DEL INTEGRANTE ---
            sb.Append("<div class='card mb-4 border-0 shadow-sm'>");
            sb.Append("<div class='card-body bg-light rounded'>");
            sb.Append("<div class='row'>");
            sb.Append($"<div class='col-6'><strong>Cédula:</strong> {integrante.strCedula_int}</div>");
            sb.Append($"<div class='col-6'><strong>Nombre:</strong> {integrante.strApellidos_int} {integrante.strNombres_int}</div>");
            sb.Append($"<div class='col-6'><strong>Función:</strong> {integrante.strFuncion_int}</div>");
            sb.Append($"<div class='col-6'><strong>Estado Actual:</strong> {(integrante.bitActivo_int ? "ACTIVO" : "INACTIVO")}</div>");
            sb.Append("</div>");
            sb.Append("</div></div>");

            // --- TABLA DE HISTORIAL ---
            sb.Append("<table class='table table-bordered table-striped text-center small'>");
            sb.Append("<thead class='table-dark text-white'><tr>");
            sb.Append("<th>FECHA</th><th>ACCIÓN</th><th>MOTIVO</th><th>USUARIO</th>");
            sb.Append("</tr></thead>");
            sb.Append("<tbody>");

            if (historial != null && historial.Count > 0)
            {
                foreach (var item in historial)
                {
                    sb.Append("<tr>");
                    sb.Append($"<td>{item.dtFecha:dd/MM/yyyy HH:mm}</td>");

                    string colorBadge = item.strAccion == "BAJA" ? "bg-danger" : "bg-success";
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
            sb.Append("<br/><br/><div class='row mt-5 text-center'>");
            sb.Append("<div class='col-6'><hr class='w-50 mx-auto'/>Firma Responsable</div>");
            sb.Append("<div class='col-6'><hr class='w-50 mx-auto'/>Recibido Conforme</div>");
            sb.Append("</div>");

            return sb.ToString();
        }

        // =============================================
        // 5. UTILIDADES Y AYUDAS
        // =============================================

        private enum Vista { ListaGrupos, FormularioGrupo, ListaIntegrantes, FormularioIntegrante }

        private void CambiarVista(Vista vista)
        {
            pnlGrilla.Visible = vista == Vista.ListaGrupos;
            headerGrupos.Visible = vista == Vista.ListaGrupos;
            pnlFormularioGrupo.Visible = vista == Vista.FormularioGrupo;
            pnlIntegrantes.Visible = vista == Vista.ListaIntegrantes;
            pnlFormularioIntegrante.Visible = vista == Vista.FormularioIntegrante;
        }

        private void LimpiarFormularioGrupo()
        {
            hfIdGrupo.Value = "";
            txtNombreGru.Text = "";
            txtCoordinadorGru.Text = "";
            txtFechaCreaGru.Text = DateTime.Now.ToString("yyyy-MM-dd");
            ddlCentro.SelectedIndex = 0;
            ddlCategoriaGru.SelectedIndex = 0;
            hfFotoActual.Value = "";
            hfArchivoActual.Value = "";
            imgFotoActual.Visible = false;
        }

        private void LimpiarFormularioIntegrante()
        {
            hfIdIntEdit.Value = "";
            lblTituloFormInt.Text = "Nuevo Integrante";
            txtCedulaInt.Text = ""; txtNombresInt.Text = ""; txtApellidosInt.Text = "";
            txtCorreoInt.Text = ""; txtEntidadInt.Text = ""; txtCarreraInt.Text = "";
            ddlTipoInt.SelectedIndex = 0;
            ddlFacultadInt.SelectedIndex = 0;
            dtFechaIniInt.Text = DateTime.Now.ToString("yyyy-MM-dd");
            hfCertificadoIntActual.Value = "";
        }

        private string GuardarArchivoFisico(FileUpload control, string subCarpeta, string nombre)
        {
            string rutaVirtualCarpeta = Path.Combine(RUTA_VIRTUAL_BASE, subCarpeta);
            string rutaFisicaCarpeta = Server.MapPath(rutaVirtualCarpeta);

            if (!Directory.Exists(rutaFisicaCarpeta))
            {
                Directory.CreateDirectory(rutaFisicaCarpeta);
            }

            string rutaFisicaCompleta = Path.Combine(rutaFisicaCarpeta, nombre);
            control.SaveAs(rutaFisicaCompleta);
            return Path.Combine(rutaVirtualCarpeta, nombre).Replace("\\", "/");
        }

        private void VisualizarArchivo(string rutaRelativaDb)
        {
            string rutaFisica = rutaRelativaDb;

            if (rutaRelativaDb.StartsWith("~") || rutaRelativaDb.StartsWith("/"))
            {
                rutaFisica = Server.MapPath(rutaRelativaDb);
            }

            if (File.Exists(rutaFisica))
            {
                string nombre = Path.GetFileName(rutaFisica);
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "inline; filename=" + nombre);
                Response.TransmitFile(rutaFisica);
                Response.End();
            }
            else
            {
                Msg("El archivo no se encuentra en el servidor.", "ww");
            }
        }

        protected string ObtenerImagenBase64(object rutaObj)
        {
            string rutaRelativa = rutaObj as string;

            if (string.IsNullOrEmpty(rutaRelativa)) return "DesignersUTC/Images/default-group.png";

            try
            {
                string rutaFisica = rutaRelativa.StartsWith("~") ? Server.MapPath(rutaRelativa) : rutaRelativa;

                if (!File.Exists(rutaFisica)) return "DesignersUTC/Images/default-group.png";

                return "data:image/jpeg;base64," + Convert.ToBase64String(File.ReadAllBytes(rutaFisica));
            }
            catch { return ""; }
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
            string cleanMsg = msg.Replace("'", "").Replace("\r\n", " ");
            string script = $"$(function() {{ toastify('{type}', '{cleanMsg}', 'Sistema'); }});";
            ScriptManager.RegisterStartupScript(this, GetType(), "toast", script, true);
        }
    }
}