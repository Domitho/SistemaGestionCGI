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
                // 1. VALIDACIÓN BÁSICA
                if (string.IsNullOrWhiteSpace(txtNombreGru.Text))
                {
                    Msg("El nombre del grupo es obligatorio.", "ww");
                    return;
                }

                // 2. CREACIÓN DEL OBJETO GRUPO (Datos del formulario)
                var g = new InvgccGrupoInvestigacion
                {
                    strNombre_gru = txtNombreGru.Text.Trim(),
                    fkId_cen = (ddlCentro.SelectedValue == "") ? null : ddlCentro.SelectedValue,
                    strCoordinador_gru = txtCoordinadorGru.Text.Trim(),
                    strCategoria_gru = ddlCategoriaGru.SelectedValue,
                    strLineasinv_gru = ddlLineaInv.SelectedValue,
                    strSublineasinv_gru = ddlSublineaInv.SelectedValue,
                    dtFechacrea_gru = !string.IsNullOrEmpty(txtFechaCreaGru.Text) ? DateTime.Parse(txtFechaCreaGru.Text) : DateTime.Now,
                    strFoto_gru = hfFotoActual.Value,
                    strArchivo_gru = hfArchivoActual.Value
                };

                // 3. GESTIÓN DE ARCHIVOS
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
                            strEntidad_int = string.IsNullOrEmpty(hfCoordEntidad.Value) ? null : hfCoordEntidad.Value,
                            strFacultad_int = string.IsNullOrEmpty(hfCoordFacultad.Value) ? null : hfCoordFacultad.Value,
                            strCarrera_int = string.IsNullOrEmpty(hfCoordCarrera.Value) ? null : hfCoordCarrera.Value,
                            strCertificado_int = hfCoordArchivo.Value,
                            fkId_docente_origen = string.IsNullOrEmpty(hfCoordIdDocente.Value) ? null : hfCoordIdDocente.Value,

                            strFuncion_int = "INVESTIGADOR PRINCIPAL",
                            dtFechaini_int = DateTime.Now,
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
                    string idGrupoLimpio = hfIdGrupo.Value.Trim();

                    if (string.IsNullOrEmpty(idGrupoLimpio))
                    {
                        Msg("Error crítico: ID de grupo perdido.", "ee"); return;
                    }

                    g.strId_gru = idGrupoLimpio;
                    _manejador.ActualizarGrupo(g);

                    if (!string.IsNullOrEmpty(hfCoordCedula.Value))
                    {
                        var coord = new InvgccGrupoIntegrantes
                        {
                            fkId_gru = idGrupoLimpio, 

                            strNombres_int = hfCoordNombre.Value,
                            strApellidos_int = hfCoordApellidos.Value,
                            strCedula_int = hfCoordCedula.Value,
                            strCorreo_int = hfCoordCorreo.Value,
                            strTipo_int = hfCoordTipo.Value,
                            strEntidad_int = string.IsNullOrEmpty(hfCoordEntidad.Value) ? null : hfCoordEntidad.Value,
                            strFacultad_int = string.IsNullOrEmpty(hfCoordFacultad.Value) ? null : hfCoordFacultad.Value,
                            strCarrera_int = string.IsNullOrEmpty(hfCoordCarrera.Value) ? null : hfCoordCarrera.Value,
                            strCertificado_int = hfCoordArchivo.Value,
                            fkId_docente_origen = string.IsNullOrEmpty(hfCoordIdDocente.Value) ? null : hfCoordIdDocente.Value,

                            strFuncion_int = "INVESTIGADOR PRINCIPAL",
                            dtFechaini_int = DateTime.Now,
                            bitActivo_int = true
                        };

                        _manejador.GuardarIntegrante(coord, usuario);
                        SetFlashMessage("Grupo actualizado y Nuevo Coordinador designado.", "ss");
                    }
                    else
                    {
                        SetFlashMessage("Datos del grupo actualizados.", "ss");
                    }
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
            txtFechaCreaGru.Text = g.dtFechacrea_gru.ToString("yyyy-MM-dd");

            if (ddlCentro.Items.FindByValue(g.fkId_cen) != null)
                ddlCentro.SelectedValue = g.fkId_cen;

            if (ddlCategoriaGru.Items.FindByValue(g.strCategoria_gru) != null)
                ddlCategoriaGru.SelectedValue = g.strCategoria_gru;

            // --- LOGICA DEL COORDINADOR (DIRECTOR) ---
            // Consultamos la tabla de integrantes en tiempo real
            var coordinadorActivo = _manejador.ObtenerInvestigadorPrincipalActivo(g.strId_gru);

            if (coordinadorActivo != null)
            {
                // CASO A: HAY UN REY VIGENTE
                txtCoordinadorGru.Text = $"{coordinadorActivo.strApellidos_int} {coordinadorActivo.strNombres_int}";
                txtCoordinadorGru.CssClass = "form-control bg-light fw-bold text-success";

                // Ocultamos el botón para que no puedan agregar otro encima
                btnAgregarCoordinador.Visible = false;
            }
            else
            {
                // CASO B: EL PUESTO ESTÁ VACANTE (Lo dieron de baja en integrantes)
                txtCoordinadorGru.Text = "SIN ASIGNAR (Requiere designación)";
                txtCoordinadorGru.CssClass = "form-control bg-warning bg-opacity-10 text-danger fw-bold";

                // Mostramos el botón para permitir nombrar uno nuevo
                btnAgregarCoordinador.Visible = true;
            }
            // -----------------------------------------

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
            pnlDatosPersonalesCoord.Style["display"] = "block";
            btnGuardarCoordModal.Style["display"] = "inline-block";

            pnlCargaArchivo.Visible = true;
            pnlArchivoRecuperado.Visible = false;
            hfCoordArchivo.Value = "";

            hfCoordIdDocente.Value = "";

            pnlFormularioGrupo.Visible = true;
            ScriptManager.RegisterStartupScript(this, GetType(), "Open", "abrirModalCoord(); toggleTipoCoordinador();", true);
        }

        protected void btnGuardarCoordModal_Click(object sender, EventArgs e)
        {
            pnlFormularioGrupo.Visible = true;

            string cedulaValidar = txtCedulaCoord.Text.Trim();
            string grupoOcupado = _manejador.VerificarIntegranteEnOtroGrupo(cedulaValidar);

            if (!string.IsNullOrEmpty(grupoOcupado))
            {
                Msg($"IMPOSIBLE VINCULAR: Esta persona ya es miembro activo del grupo '{grupoOcupado}'.", "ee");

                ScriptManager.RegisterStartupScript(this, GetType(), "ReOpenDuplicado", "abrirModalCoord(); toggleTipoCoordinador();", true);
                return;
            }

            if (pnlCargaArchivo.Visible && !flpArchivoCoord.HasFile)
            {
                Msg("Adjunte la resolución (PDF).", "ww");
                ScriptManager.RegisterStartupScript(this, GetType(), "ReOpenF", "abrirModalCoord(); toggleTipoCoordinador();", true);
                return;
            }

            bool subioNuevo = flpArchivoCoord.HasFile;
            bool tienePrevio = !string.IsNullOrEmpty(hfCoordArchivo.Value);

            if (!subioNuevo && !tienePrevio)
            {
                Msg("El documento de resolución es obligatorio. Súbalo o busque un docente con certificado.", "ww");
                ScriptManager.RegisterStartupScript(this, GetType(), "ReOpenF", "abrirModalCoord(); toggleTipoCoordinador();", true);
                return;
            }


            try
            {
                string tipo = ddlTipoCoord.SelectedValue;

                string rutaFinal = hfCoordArchivo.Value;

                if (subioNuevo)
                {
                    string nombreArchivo = "CERT_" + DateTime.Now.Ticks + "_" + flpArchivoCoord.FileName;
                    rutaFinal = GuardarArchivoFisico(flpArchivoCoord, "CERTIFICADOS", nombreArchivo);
                }

                hfCoordArchivo.Value = rutaFinal;

                hfCoordNombre.Value = txtNombreCoord.Text.ToUpper().Trim();
                hfCoordApellidos.Value = txtApellidoCoord.Text.ToUpper().Trim();
                hfCoordCedula.Value = txtCedulaCoord.Text.Trim();
                hfCoordCorreo.Value = txtCorreoCoord.Text.ToLower().Trim();
                hfCoordTipo.Value = tipo;

                if (tipo == "Externo")
                {
                    if (string.IsNullOrEmpty(txtEntidadCoord.Text)) throw new Exception("Ingrese la entidad externa.");

                    hfCoordEntidad.Value = txtEntidadCoord.Text.ToUpper().Trim();
                    hfCoordFacultad.Value = "";
                    hfCoordCarrera.Value = "";
                    hfCoordIdDocente.Value = "";
                }
                else
                {
                    hfCoordCarrera.Value = txtCarreraCoord.Text.ToUpper().Trim();
                    hfCoordFacultad.Value = ddlFacultadCoord.SelectedValue;
                    hfCoordEntidad.Value = "";

                    if (tipo == "Interno") hfCoordIdDocente.Value = "";
                }

                txtCoordinadorGru.Text = $"{txtApellidoCoord.Text} {txtNombreCoord.Text}";

                ScriptManager.RegisterStartupScript(this, GetType(), "Close", "cerrarModalCoord();", true);

                if (tienePrevio && !subioNuevo)
                    Msg("Coordinador asignado (Certificado recuperado).", "ss");
                else
                    Msg("Coordinador asignado correctamente.", "ss");
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
                string grupoOcupado = _manejador.VerificarIntegranteEnOtroGrupo(cedula);
                if (!string.IsNullOrEmpty(grupoOcupado))
                {
                    Msg($"El docente existe, PERO ya pertenece al grupo '{grupoOcupado}'. No puede ser registrado dos veces.", "ww");

                    LimpiarCoordParcial();
                    return;
                }

                dynamic docente = _manejador.ObtenerDocenteCategorizado(cedula);

                if (docente != null)
                {
                    pnlDatosPersonalesCoord.Style["display"] = "block";
                    btnGuardarCoordModal.Style["display"] = "inline-block";

                    txtCedulaCoord.Text = GetProp(docente, "strCedula_doc");
                    txtNombreCoord.Text = GetProp(docente, "strNombres_doc");
                    txtApellidoCoord.Text = GetProp(docente, "strApellidos_doc");
                    txtCarreraCoord.Text = GetProp(docente, "strCarrera_doc");
                    hfCoordIdDocente.Value = GetProp(docente, "strId_doc");

                    string fac = GetProp(docente, "strFacultad_doc");
                    if (ddlFacultadCoord.Items.FindByValue(fac) != null)
                        ddlFacultadCoord.SelectedValue = fac;

                    string rutaCert = GetProp(docente, "strCertificado_doc");

                    if (!string.IsNullOrEmpty(rutaCert))
                    {
                        hfCoordArchivo.Value = rutaCert; 

                        lnkVerArchivo.NavigateUrl = ResolveUrl(rutaCert);

                        pnlCargaArchivo.Visible = false;     
                        pnlArchivoRecuperado.Visible = true;  

                        Msg("Docente y Certificado encontrados.", "ss");
                    }
                    else
                    {
                        hfCoordArchivo.Value = "";
                        pnlCargaArchivo.Visible = true;
                        pnlArchivoRecuperado.Visible = false;
                        Msg("Docente encontrado (Sin certificado adjunto).", "ii");
                    }
                }
                else
                {
                    pnlDatosPersonalesCoord.Style["display"] = "none";
                    btnGuardarCoordModal.Style["display"] = "none";

                    Msg("Docente no encontrado.", "ww");
                    LimpiarCoordParcial(); 
                }
            }
            catch (Exception ex) { Msg("Error: " + ex.Message, "ee"); }
            finally
            {
                string script = "abrirModalCoord(); toggleTipoCoordinador();";
                ScriptManager.RegisterStartupScript(this, GetType(), "ReOpenSearch", script, true);
            }
        }

        protected void btnCambiarArchivo_Click(object sender, EventArgs e)
        {
            pnlFormularioGrupo.Visible = true;

            hfCoordArchivo.Value = "";

            pnlCargaArchivo.Visible = true;
            pnlArchivoRecuperado.Visible = false;

            ScriptManager.RegisterStartupScript(this, GetType(), "ReOpenChange", "abrirModalCoord(); toggleTipoCoordinador();", true);
        }

        private void LimpiarCoordParcial()
        {
            txtNombreCoord.Text = "";
            txtApellidoCoord.Text = "";
            txtCedulaCoord.Text = "";
            txtCarreraCoord.Text = "";
            txtCorreoCoord.Text = "";
            ddlFacultadCoord.SelectedIndex = 0;

            hfCoordIdDocente.Value = "";
            hfCoordArchivo.Value = "";

            pnlCargaArchivo.Visible = true;
            pnlArchivoRecuperado.Visible = false;
        }

        private string GetProp(object obj, string propName)
        {
            if (obj == null) return "";
            try
            {
                if (obj is Newtonsoft.Json.Linq.JObject jobj)
                    return jobj[propName]?.ToString() ?? "";

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

                    strFuncion_int = txtFuncionInt.Text,

                    strTipo_int = ddlTipoInt.SelectedValue,
                    dtFechaini_int = !string.IsNullOrEmpty(dtFechaIniInt.Text) ? DateTime.Parse(dtFechaIniInt.Text) : DateTime.Now,
                    strCertificado_int = null,
                    bitActivo_int = true
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
                    i.strCertificado_int = null;
                    i.fkId_docente_origen = null;
                }
                else if (i.strTipo_int == "Docente")
                {
                    i.strEntidad_int = null;
                    i.strCarrera_int = txtCarreraInt.Text.Trim();
                    i.strFacultad_int = ddlFacultadInt.SelectedValue;

                    i.strCertificado_int = hfCertificadoIntVinculado.Value;
                    i.fkId_docente_origen = hfIdDocenteInt.Value;
                }
                else 
                {
                    i.strEntidad_int = null;
                    i.strCarrera_int = txtCarreraInt.Text.Trim();
                    i.strFacultad_int = ddlFacultadInt.SelectedValue;
                    i.strCertificado_int = null;
                    i.fkId_docente_origen = null;
                }

                string usuario = Session["UsuarioLogueado"]?.ToString() ?? "Sistema";

                if (string.IsNullOrEmpty(hfIdIntEdit.Value))
                {
                    _manejador.GuardarIntegrante(i, usuario);
                    SetFlashMessage("Integrante agregado.", "ss");
                }
                else
                {
                    i.strId_int = hfIdIntEdit.Value;
                    _manejador.ActualizarIntegrante(i, usuario);
                    SetFlashMessage("Integrante actualizado.", "ss");
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

            txtFuncionInt.Text = i.strFuncion_int;

            hfCertificadoIntVinculado.Value = i.strCertificado_int;
            hfIdDocenteInt.Value = i.fkId_docente_origen;

            if (ddlTipoInt.Items.FindByValue(i.strTipo_int) != null)
                ddlTipoInt.SelectedValue = i.strTipo_int;

            ddlTipoInt.Enabled = false;
            ddlTipoInt.CssClass = "form-select shadow-sm border-primary bg-light";

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

        protected void btnBuscarDocenteInt_Click(object sender, EventArgs e)
        {
            string cedula = txtBuscarCedulaInt.Text.Trim();
            if (string.IsNullOrEmpty(cedula)) return;

            try
            {
                string grupoOcupado = _manejador.VerificarIntegranteEnOtroGrupo(cedula);
                if (!string.IsNullOrEmpty(grupoOcupado))
                {
                    Msg($"El docente existe, PERO ya pertenece al grupo '{grupoOcupado}'. No puede ser registrado dos veces.", "ww");

                    LimpiarCoordParcial();
                    return;
                }

                dynamic docente = _manejador.ObtenerDocenteCategorizado(cedula);

                if (docente != null)
                {
                    txtCedulaInt.Text = GetProp(docente, "strCedula_doc");
                    txtNombresInt.Text = GetProp(docente, "strNombres_doc");
                    txtApellidosInt.Text = GetProp(docente, "strApellidos_doc");
                    txtCarreraInt.Text = GetProp(docente, "strCarrera_doc");

                    string fac = GetProp(docente, "strFacultad_doc");
                    if (ddlFacultadInt.Items.FindByValue(fac) != null)
                        ddlFacultadInt.SelectedValue = fac;

                    hfIdDocenteInt.Value = GetProp(docente, "strId_doc");
                    hfCertificadoIntVinculado.Value = GetProp(docente, "strCertificado_doc");

                    Msg("Docente encontrado. Verifique la información.", "ss");
                }
                else
                {
                    Msg("Docente no encontrado en la base de datos.", "ww");

                    txtNombresInt.Text = "";
                    txtApellidosInt.Text = "";
                    txtCedulaInt.Text = "";
                    txtCarreraInt.Text = "";
                    hfIdDocenteInt.Value = "";
                }
            }
            catch (Exception ex) { Msg("Error búsqueda: " + ex.Message, "ee"); }
            finally
            {
                string script = $"ToggleTipoIntegranteForm(document.getElementById('{ddlTipoInt.ClientID}'));";
                ScriptManager.RegisterStartupScript(this, GetType(), "RefreshIntForm", script, true);
            }
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

                var integrante = _manejador.ObtenerIntegrantePorId(idInt);

                bool nuevoEstado = !integrante.bitActivo_int;

                if (nuevoEstado == true && integrante.strFuncion_int == "INVESTIGADOR PRINCIPAL")
                {
                    var jefeActual = _manejador.ObtenerInvestigadorPrincipalActivo(integrante.fkId_gru);

                    if (jefeActual != null && jefeActual.strId_int != integrante.strId_int)
                    {
                        Msg($"No se puede reactivar. Ya existe un Director activo: {jefeActual.strApellidos_int} {jefeActual.strNombres_int}. Primero debe dar de baja al actual.", "ww");
                        return; 
                    }
                }

                _manejador.CambiarEstadoIntegrante(idInt, nuevoEstado, motivo, usuario);

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
            ddlTipoInt.Enabled = true; 
            ddlTipoInt.CssClass = "form-select shadow-sm border-primary"; 

            ddlFacultadInt.SelectedIndex = 0;
            dtFechaIniInt.Text = DateTime.Now.ToString("yyyy-MM-dd");

            hfCertificadoIntVinculado.Value = "";
            hfIdDocenteInt.Value = "";

            txtFuncionInt.Text = "Miembro Investigador";
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

        //

        protected void btnVerPapeleraInt_Click(object sender, EventArgs e)
        {
            string idGrupo = hfGrupoIdActual.Value;
            // Cargar solo los inactivos de este grupo específico
            var inactivos = _manejador.ObtenerIntegrantesPapelera(idGrupo);
            rptPapeleraIntegrantes.DataSource = inactivos;
            rptPapeleraIntegrantes.DataBind();

            ScriptManager.RegisterStartupScript(this, GetType(), "PopTrash",
                "new bootstrap.Modal(document.getElementById('modalPapeleraIntegrantes')).show();", true);
        }

        protected void rptPapeleraIntegrantes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "restaurar")
            {
                try
                {
                    string idInt = e.CommandArgument.ToString();
                    string usuario = Session["UsuarioLogueado"]?.ToString() ?? "SISTEMA";

                    if (_manejador.RestaurarIntegrante(idInt, usuario))
                    {
                        CargarIntegrantes(hfGrupoIdActual.Value);
                        Msg("Integrante reincorporado correctamente.", "ss");
                    }
                    else
                    {
                        Msg("No se puede restaurar: El investigador ya está ACTIVO en un grupo.", "ww");
                    }
                }
                catch (Exception ex) { Msg("Error: " + ex.Message, "ee"); }
            }
        }

        private void CargarIntegrantes(string idGrupo)
        {
            try
            {
                var lista = _manejador.ObtenerIntegrantes(idGrupo);
                rptIntegrantes.DataSource = lista;
                rptIntegrantes.DataBind();

                pnlGrilla.Visible = false;
                pnlIntegrantes.Visible = true;
                hfGrupoIdActual.Value = idGrupo;
            }
            catch (Exception ex)
            {
                Msg("Error al cargar integrantes: " + ex.Message, "ee");
            }
        }

    }
}