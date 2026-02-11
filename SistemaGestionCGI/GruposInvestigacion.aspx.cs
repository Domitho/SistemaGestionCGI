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
                    strFacultad_gru = ddlFacultadGrupo.SelectedValue,
                    strCarrera_gru = ddlCarreraGrupo.SelectedValue,
                    strCoordinador_gru = txtCoordinadorGru.Text.Trim(),
                    strCategoria_gru = ddlCategoriaGru.SelectedValue,
                    strLineasinv_gru = ddlLineaInv.SelectedValue,
                    dtFechacrea_gru = !string.IsNullOrEmpty(txtFechaCreaGru.Text) ? DateTime.Parse(txtFechaCreaGru.Text) : DateTime.Now,
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
                    if (string.IsNullOrEmpty(idGrupoLimpio)) { Msg("Error crítico: ID perdido.", "ee"); return; }

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

            if (ddlFacultadGrupo.Items.FindByValue(g.strFacultad_gru) != null)
            {
                ddlFacultadGrupo.SelectedValue = g.strFacultad_gru;
                CargarCarrerasEnCombo(ddlCarreraGrupo, g.strFacultad_gru);

                if (ddlCarreraGrupo.Items.FindByValue(g.strCarrera_gru) != null)
                    ddlCarreraGrupo.SelectedValue = g.strCarrera_gru;
            }
            else
            {
                ddlFacultadGrupo.SelectedIndex = 0;
                CargarCarrerasEnCombo(ddlCarreraGrupo, "");
            }

            var coordinadorActivo = _manejador.ObtenerInvestigadorPrincipalActivo(g.strId_gru);

            if (coordinadorActivo != null)
            {
                txtCoordinadorGru.Text = $"{coordinadorActivo.strApellidos_int} {coordinadorActivo.strNombres_int}";
                txtCoordinadorGru.CssClass = "form-control bg-light fw-bold text-success";
                btnAgregarCoordinador.Visible = false;
            }
            else
            {
                txtCoordinadorGru.Text = "SIN ASIGNAR (Requiere designación)";
                txtCoordinadorGru.CssClass = "form-control bg-warning bg-opacity-10 text-danger fw-bold";
                btnAgregarCoordinador.Visible = true;
            }

            hfFotoActual.Value = g.strFoto_gru;

            if (!string.IsNullOrEmpty(g.strFoto_gru))
            {
                imgFotoVisual.ImageUrl = ObtenerImagenBase64(g.strFoto_gru);
            }
            else
            {
                imgFotoVisual.ImageUrl = "~/DesignersUTC/Images/default-group.png";
            }

            CambiarVista(Vista.FormularioGrupo);
        }

        protected void btnAgregarCoordinador_Click(object sender, EventArgs e)
        {
            var docentes = _manejador.ObtenerDocentesCategorizadosCombo();
            ddlDocentesCoord.Items.Clear();
            ddlDocentesCoord.Items.Add(new ListItem("-- Seleccione Docente --", ""));

            foreach (var d in docentes)
            {
                ddlDocentesCoord.Items.Add(new ListItem(d.NombreCompleto, d.strId_doc));
            }

            LimpiarCamposCoordinador();

            string script = "RenderizarModalCoord('Interno');";
            ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalCoord", script, true);
        }

        protected void btnGuardarCoordModal_Click(object sender, EventArgs e)
        {
            pnlFormularioGrupo.Visible = true;

            try
            {
                string cedula = txtCedulaCoord.Text.Trim();
                string tipo = ddlTipoCoord.SelectedValue;

                if (tipo != "Docente")
                {
                    if (!EsCedulaValida(cedula))
                    {
                        txtCedulaCoord.CssClass = "form-control bg-light is-invalid";
                        Msg("Cédula inválida. Corrija antes de asignar.", "ee");
                        ScriptManager.RegisterStartupScript(this, GetType(), "ReOpenBadCed", $"RenderizarModalCoord('{tipo}');", true);
                        return;
                    }

                    string ocupado = _manejador.VerificarIntegranteEnOtroGrupo(cedula, hfIdGrupo.Value);

                    if (!string.IsNullOrEmpty(ocupado))
                    {
                        txtCedulaCoord.CssClass = "form-control bg-light is-invalid";
                        Msg($"El usuario ya coordina o pertenece a: '{ocupado}'.", "ww");
                        ScriptManager.RegisterStartupScript(this, GetType(), "ReOpenDup", $"RenderizarModalCoord('{tipo}');", true);
                        return;
                    }
                }

                if (pnlCargaArchivo.Visible && !flpArchivoCoord.HasFile)
                {
                    Msg("Adjunte la resolución (PDF).", "ww");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ReOpenFile", $"RenderizarModalCoord('{tipo}');", true);
                    return;
                }

                bool subioNuevo = flpArchivoCoord.HasFile;
                bool tienePrevio = !string.IsNullOrEmpty(hfCoordArchivo.Value);

                if (!subioNuevo && !tienePrevio)
                {
                    Msg("El documento de resolución es obligatorio.", "ww");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ReOpenFile2", $"RenderizarModalCoord('{tipo}');", true);
                    return;
                }

                string rutaFinal = hfCoordArchivo.Value;

                if (subioNuevo)
                {
                    string nombreArchivo = "CERT_" + DateTime.Now.Ticks + "_" + flpArchivoCoord.FileName;
                    rutaFinal = GuardarArchivoFisico(flpArchivoCoord, "CERTIFICADOS", nombreArchivo);
                }

                hfCoordArchivo.Value = rutaFinal;
                hfCoordNombre.Value = txtNombreCoord.Text.ToUpper().Trim();
                hfCoordApellidos.Value = txtApellidoCoord.Text.ToUpper().Trim();
                hfCoordCedula.Value = cedula;
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
                    hfCoordCarrera.Value = ddlCarreraCoord.SelectedValue;
                    hfCoordFacultad.Value = ddlFacultadCoord.SelectedValue;
                    hfCoordEntidad.Value = "";

                    if (tipo == "Interno") hfCoordIdDocente.Value = "";
                }

                txtCoordinadorGru.Text = $"{txtApellidoCoord.Text} {txtNombreCoord.Text}";

                ScriptManager.RegisterStartupScript(this, GetType(), "CloseModal", "cerrarModalCoord();", true);

                if (tienePrevio && !subioNuevo)
                    Msg("Coordinador asignado (Certificado recuperado).", "ss");
                else
                    Msg("Coordinador asignado correctamente.", "ss");
            }
            catch (Exception ex)
            {
                Msg(ex.Message, "ee");
                string tipo = ddlTipoCoord.SelectedValue;
                ScriptManager.RegisterStartupScript(this, GetType(), "ReOpenErr", $"RenderizarModalCoord('{tipo}');", true);
            }
        }


        protected void btnCambiarArchivo_Click(object sender, EventArgs e)
        {
            pnlFormularioGrupo.Visible = true;
            hfCoordArchivo.Value = "";
            pnlCargaArchivo.Visible = true;
            pnlArchivoRecuperado.Visible = false;

            string tipo = ddlTipoCoord.SelectedValue;
            ScriptManager.RegisterStartupScript(this, GetType(), "ReOpenChange", $"RenderizarModalCoord('{tipo}');", true);
        }

        private void LimpiarCoordParcial()
        {
            txtNombreCoord.Text = "";
            txtApellidoCoord.Text = "";
            txtCedulaCoord.Text = "";
            txtCorreoCoord.Text = "";
            ddlFacultadCoord.SelectedIndex = 0;
            ddlCarreraCoord.SelectedIndex = 0;

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
            CargarComboDocentes();

            string script = "RenderizarEstadoVisual('Interno');";
            ScriptManager.RegisterStartupScript(this, GetType(), "initForm", script, true);
        }

        protected void btnGuardarInt_Click(object sender, EventArgs e)
        {
            try
            {
                string cedula = txtCedulaInt.Text.Trim();
                string tipo = ddlTipoInt.SelectedValue;

                if (tipo != "Docente")
                {
                    if (!EsCedulaValida(cedula))
                    {
                        txtCedulaInt.CssClass = "form-control is-invalid";
                        Msg("IMPOSIBLE GUARDAR: La cédula ingresada no es válida.", "ee");
                        return;
                    }

                    if (string.IsNullOrEmpty(hfIdIntEdit.Value))
                    {
                        string grupoOcupado = _manejador.VerificarIntegranteEnOtroGrupo(cedula, hfGrupoIdActual.Value);
                        if (!string.IsNullOrEmpty(grupoOcupado))
                        {
                            txtCedulaInt.CssClass = "form-control is-invalid";
                            Msg($"DETENIDO: Esa persona ya pertenece al grupo '{grupoOcupado}'.", "ww");
                            return;
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(cedula) || string.IsNullOrWhiteSpace(txtNombresInt.Text))
                {
                    Msg("Cédula y Nombres son obligatorios.", "ww");
                    return;
                }

                var i = new InvgccGrupoIntegrantes
                {
                    fkId_gru = hfGrupoIdActual.Value,
                    strCedula_int = cedula,
                    strNombres_int = txtNombresInt.Text.Trim(),
                    strApellidos_int = txtApellidosInt.Text.Trim(),
                    strCorreo_int = txtCorreoInt.Text.Trim(),
                    strFuncion_int = txtFuncionInt.Text,
                    strTipo_int = tipo,
                    dtFechaini_int = !string.IsNullOrEmpty(dtFechaIniInt.Text) ? DateTime.Parse(dtFechaIniInt.Text) : DateTime.Now,
                    strCertificado_int = null,
                    bitActivo_int = true
                };

                if (i.strTipo_int == "Externo")
                {
                    if (string.IsNullOrWhiteSpace(txtEntidadInt.Text)) { Msg("Debe indicar la Entidad de Origen.", "ww"); return; }
                    i.strEntidad_int = txtEntidadInt.Text.Trim();
                    i.strCarrera_int = null; i.strFacultad_int = null; i.fkId_docente_origen = null;
                }
                else if (i.strTipo_int == "Docente")
                {
                    i.strEntidad_int = null;
                    i.strCarrera_int = ddlCarreraInt.SelectedValue;
                    i.strFacultad_int = ddlFacultadInt.SelectedValue;
                    i.strCertificado_int = hfCertificadoIntVinculado.Value;
                    i.fkId_docente_origen = hfIdDocenteInt.Value;
                }
                else
                {
                    i.strEntidad_int = null;
                    i.strCarrera_int = ddlCarreraInt.SelectedValue;
                    i.strFacultad_int = ddlFacultadInt.SelectedValue;
                    i.fkId_docente_origen = null;
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
            txtFuncionInt.Text = i.strFuncion_int;

            hfCertificadoIntVinculado.Value = i.strCertificado_int;
            hfIdDocenteInt.Value = i.fkId_docente_origen;

            if (ddlTipoInt.Items.FindByValue(i.strTipo_int) != null)
                ddlTipoInt.SelectedValue = i.strTipo_int;

            ddlTipoInt.Enabled = false;
            ddlTipoInt.CssClass = "form-select shadow-sm border-primary bg-light";

            if (i.strTipo_int == "Docente")
            {
                BloquearCamposDatosPersonales(true);

                if (ddlFacultadInt.Items.FindByValue(i.strFacultad_int) != null)
                    ddlFacultadInt.SelectedValue = i.strFacultad_int;

                CargarCarrerasEnCombo(ddlCarreraInt, i.strFacultad_int);

                if (ddlCarreraInt.Items.FindByValue(i.strCarrera_int) != null)
                    ddlCarreraInt.SelectedValue = i.strCarrera_int;
            }
            else if (i.strTipo_int == "Externo")
            {
                BloquearCamposDatosPersonales(false);
                txtEntidadInt.Text = i.strEntidad_int;
            }
            else 
            {
                BloquearCamposDatosPersonales(false);

                if (ddlFacultadInt.Items.FindByValue(i.strFacultad_int) != null)
                    ddlFacultadInt.SelectedValue = i.strFacultad_int;

                CargarCarrerasEnCombo(ddlCarreraInt, i.strFacultad_int);

                if (ddlCarreraInt.Items.FindByValue(i.strCarrera_int) != null)
                    ddlCarreraInt.SelectedValue = i.strCarrera_int;
            }

            CambiarVista(Vista.FormularioIntegrante);

            string script = $"RenderizarEstadoVisual('{i.strTipo_int}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "InitEditUI", script, true);
        }

        private void BloquearCamposDatosPersonales(bool bloquear)
        {
            txtCedulaInt.ReadOnly = bloquear;
            txtNombresInt.ReadOnly = bloquear;
            txtApellidosInt.ReadOnly = bloquear;
            txtCorreoInt.ReadOnly = bloquear;

            ddlFacultadInt.Enabled = !bloquear;
            ddlCarreraInt.Enabled = !bloquear;

            if (bloquear)
            {
                string estiloLocked = "form-control bg-light text-secondary fw-bold";
                txtCedulaInt.CssClass = estiloLocked;
                txtNombresInt.CssClass = estiloLocked;
                txtApellidosInt.CssClass = estiloLocked;
                txtCorreoInt.CssClass = estiloLocked;
            }
            else
            {
                string estiloNormal = "form-control";
                txtCedulaInt.CssClass = estiloNormal;
                txtNombresInt.CssClass = estiloNormal;
                txtApellidosInt.CssClass = estiloNormal;
                txtCorreoInt.CssClass = estiloNormal;
            }
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

                lblRefId.Text = integrante.strId_int;
                lblReporteNombre.Text = $"{integrante.strApellidos_int} {integrante.strNombres_int}";
                lblReporteCedula.Text = integrante.strCedula_int;
                lblReporteFuncion.Text = integrante.strFuncion_int;

                lblReporteEstado.Text = integrante.bitActivo_int ? "ACTIVO" : "INACTIVO";
                lblReporteEstado.ForeColor = integrante.bitActivo_int ?
                    System.Drawing.ColorTranslator.FromHtml("#1b9e4b") :
                    System.Drawing.ColorTranslator.FromHtml("#d9534f");

                rptReporteHistorial.DataSource = historial;
                rptReporteHistorial.DataBind();

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

            sb.Append("<div class='text-center mb-4'>");
            sb.Append("<h4 class='text-uppercase fw-bold'>Reporte de Movimientos</h4>");
            sb.Append($"<p class='text-muted small'>Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}</p>");
            sb.Append("</div>");

            sb.Append("<div class='card mb-4 border-0 shadow-sm'>");
            sb.Append("<div class='card-body bg-light rounded'>");
            sb.Append("<div class='row'>");
            sb.Append($"<div class='col-6'><strong>Cédula:</strong> {integrante.strCedula_int}</div>");
            sb.Append($"<div class='col-6'><strong>Nombre:</strong> {integrante.strApellidos_int} {integrante.strNombres_int}</div>");
            sb.Append($"<div class='col-6'><strong>Función:</strong> {integrante.strFuncion_int}</div>");
            sb.Append($"<div class='col-6'><strong>Estado Actual:</strong> {(integrante.bitActivo_int ? "ACTIVO" : "INACTIVO")}</div>");
            sb.Append("</div>");
            sb.Append("</div></div>");

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

            ddlFacultadGrupo.SelectedIndex = 0;
            ddlCarreraGrupo.Items.Clear();
            ddlCarreraGrupo.Items.Add(new ListItem("-- Seleccione Facultad Primero --", ""));

            txtCoordinadorGru.Text = "";
            txtFechaCreaGru.Text = DateTime.Now.ToString("yyyy-MM-dd");
            ddlCentro.SelectedIndex = 0;
            ddlCategoriaGru.SelectedIndex = 0;
            hfFotoActual.Value = "";
            hfArchivoActual.Value = "";
            imgFotoVisual.ImageUrl = "~/DesignersUTC/Images/default-group.png";
        }

        // FOTOS
        protected void btnEliminarFoto_Click(object sender, EventArgs e)
        {
            imgFotoVisual.ImageUrl = "~/DesignersUTC/Images/default-group.png";
            hfFotoActual.Value = "";

            pnlFormularioGrupo.Visible = true;
        }

        private void LimpiarFormularioIntegrante()
        {
            hfIdIntEdit.Value = "";
            lblTituloFormInt.Text = "Nuevo Integrante";

            txtCedulaInt.Text = "";
            txtNombresInt.Text = "";
            txtApellidosInt.Text = "";
            txtCorreoInt.Text = "";
            txtEntidadInt.Text = "";

            dtFechaIniInt.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtFuncionInt.Text = "Miembro Investigador";

            hfCertificadoIntVinculado.Value = "";
            hfIdDocenteInt.Value = "";

            ddlTipoInt.SelectedIndex = 0;
            if(ddlFacultadInt.Items.Count > 0) ddlFacultadInt.SelectedIndex = 0;
            CargarCarrerasEnCombo(ddlCarreraInt, "");

            ddlTipoInt.Enabled = true;
            ddlTipoInt.CssClass = "form-select shadow-sm border-primary";
            BloquearCamposDatosPersonales(false);

            string script = "RenderizarEstadoVisual('Interno');";
            ScriptManager.RegisterStartupScript(this, GetType(), "ResetUI", script, true);
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

        private void CargarComboDocentes()
        {
            try
            {
                var lista = _manejador.ObtenerDocentesCategorizadosCombo();

                ddlDocentesCategorizados.Items.Clear();
                ddlDocentesCategorizados.DataSource = lista;
                ddlDocentesCategorizados.DataTextField = "NombreCompleto";
                ddlDocentesCategorizados.DataValueField = "strId_doc";
                ddlDocentesCategorizados.DataBind();

                ddlDocentesCategorizados.Items.Insert(0, new ListItem("-- Seleccione un Docente Libre --", ""));
            }
            catch (Exception ex)
            {
                Msg("Error al cargar lista de docentes: " + ex.Message, "ee");
            }
        }

        protected void ddlDocentesCategorizados_SelectedIndexChanged(object sender, EventArgs e)
        {
            string idDocente = ddlDocentesCategorizados.SelectedValue;
            if (string.IsNullOrEmpty(idDocente))
            {
                LimpiarCamposDocente();
                return;
            }

            try
            {
                ddlTipoInt.SelectedValue = "Docente";

                var docente = _manejador.ObtenerDocenteCategorizadoPorId(idDocente);

                if (docente != null)
                {
                    string grupoOcupado = _manejador.VerificarIntegranteEnOtroGrupo(docente.strCedula_doc, hfGrupoIdActual.Value);
                    if (!string.IsNullOrEmpty(grupoOcupado))
                    {
                        Msg($"El docente ya ha sido vinculado al grupo '{grupoOcupado}'.", "ww");
                        LimpiarCamposDocente();
                        ddlDocentesCategorizados.SelectedIndex = 0;
                        return;
                    }

                    txtCedulaInt.Text = docente.strCedula_doc;
                    txtNombresInt.Text = docente.strNombres_doc;
                    txtApellidosInt.Text = docente.strApellidos_doc;

                    txtCorreoInt.Text = !string.IsNullOrEmpty(docente.strCorreo_doc) ? docente.strCorreo_doc : "";

                    SeleccionarCombo(ddlFacultadInt, docente.strFacultad_doc);
                    CargarCarrerasEnCombo(ddlCarreraInt, docente.strFacultad_doc);
                    SeleccionarCombo(ddlCarreraInt, docente.strCarrera_doc);

                    hfIdDocenteInt.Value = docente.strId_doc;
                    hfCertificadoIntVinculado.Value = docente.strCertificado_doc;

                    txtCedulaInt.ReadOnly = true;
                    txtNombresInt.ReadOnly = true;
                    txtApellidosInt.ReadOnly = true;
                    ddlFacultadInt.Enabled = false; 
                    ddlCarreraInt.Enabled = false;  
                    txtCorreoInt.ReadOnly = true;

                    Msg("Docente vinculado exitosamente.", "ss");
                }
            }
            catch (Exception ex)
            {
                Msg("Error al vincular: " + ex.Message, "ee");
            }
            finally
            {
                string script = "RenderizarEstadoVisual('Docente');";
                ScriptManager.RegisterStartupScript(this, GetType(), "RefreshIntForm", script, true);
            }
        }

        private void SeleccionarCombo(DropDownList ddl, string valor)
        {
            string valorLimpio = !string.IsNullOrEmpty(valor) ? valor.Trim() : "";

            if (ddl.Items.FindByValue(valorLimpio) != null)
            {
                ddl.SelectedValue = valorLimpio;
            }
            else
            {
                ddl.SelectedIndex = 0;
            }
        }

        private void LimpiarCamposDocente()
        {
            txtCedulaInt.Text = "";
            txtNombresInt.Text = "";
            txtApellidosInt.Text = "";
            ddlFacultadInt.Enabled = true;
            ddlCarreraInt.Enabled = true;
            hfIdDocenteInt.Value = "";
            hfCertificadoIntVinculado.Value = "";

            txtCedulaInt.ReadOnly = false;
            txtNombresInt.ReadOnly = false;
            txtApellidosInt.ReadOnly = false;
        }


        //

        protected void ddlDocentesCoord_SelectedIndexChanged(object sender, EventArgs e)
        {
            string idDocente = ddlDocentesCoord.SelectedValue;

            if (string.IsNullOrEmpty(idDocente))
            {
                LimpiarCamposCoordinador();
                return;
            }

            try
            {
                ddlTipoCoord.SelectedValue = "Docente";
                var docente = _manejador.ObtenerDocenteCategorizadoPorId(idDocente);

                if (docente != null)
                {

                    txtCedulaCoord.Text = docente.strCedula_doc;
                    txtNombreCoord.Text = docente.strNombres_doc;
                    txtApellidoCoord.Text = docente.strApellidos_doc;
                    txtCorreoCoord.Text = !string.IsNullOrEmpty(docente.strCorreo_doc) ? docente.strCorreo_doc : "";

                    SeleccionarCombo(ddlFacultadCoord, docente.strFacultad_doc);
                    CargarCarrerasEnCombo(ddlCarreraCoord, docente.strFacultad_doc);
                    SeleccionarCombo(ddlCarreraCoord, docente.strCarrera_doc);

                    hfCoordIdDocente.Value = docente.strId_doc;

                    if (!string.IsNullOrEmpty(docente.strCertificado_doc))
                    {
                        hfCoordArchivo.Value = docente.strCertificado_doc;
                        pnlCargaArchivo.Visible = false;
                        pnlArchivoRecuperado.Visible = true;
                        lnkVerArchivo.NavigateUrl = ResolveUrl(docente.strCertificado_doc);
                        btnCambiarArchivo.Visible = false;
                    }
                    else
                    {
                        hfCoordArchivo.Value = "";
                        pnlCargaArchivo.Visible = true;
                        pnlArchivoRecuperado.Visible = false;
                        btnCambiarArchivo.Visible = true;
                    }

                    AlternarBloqueoCamposCoord(true);
                    pnlDatosPersonalesCoord.Style["display"] = "block";
                    Msg("Docente libre vinculado.", "ss");
                }
            }
            catch (Exception ex)
            {
                Msg("Error: " + ex.Message, "ee");
            }
            finally
            {
                string script = "RenderizarModalCoord('Docente');";
                ScriptManager.RegisterStartupScript(this, GetType(), "ReOpenAfterSelect", script, true);
            }
        }

        private void LimpiarCamposCoordinador()
        {
            txtCedulaCoord.Text = "";
            txtNombreCoord.Text = "";
            txtApellidoCoord.Text = "";
            txtCorreoCoord.Text = "";
            ddlFacultadCoord.SelectedIndex = 0;
            ddlCarreraCoord.SelectedIndex = 0;
            hfCoordIdDocente.Value = "";

            hfCoordArchivo.Value = "";
            pnlCargaArchivo.Visible = true;  
            pnlArchivoRecuperado.Visible = false;  
            btnCambiarArchivo.Visible = true;      

            if (ddlDocentesCoord.Items.Count > 0) ddlDocentesCoord.SelectedIndex = 0;
            ddlTipoCoord.SelectedValue = "Interno";

            AlternarBloqueoCamposCoord(false);
            pnlDatosPersonalesCoord.Style["display"] = "block";
        }

        private void AlternarBloqueoCamposCoord(bool bloquear)
        {
            txtCedulaCoord.ReadOnly = bloquear;
            txtNombreCoord.ReadOnly = bloquear;
            txtApellidoCoord.ReadOnly = bloquear;
            txtCorreoCoord.ReadOnly = bloquear;

            ddlFacultadCoord.Enabled = !bloquear;
            ddlCarreraCoord.Enabled = !bloquear;

            string claseBase = bloquear ? "form-control bg-secondary bg-opacity-10" : "form-control bg-light";
            txtNombreCoord.CssClass = claseBase;
            txtApellidoCoord.CssClass = claseBase;
        }

        //

        private void CargarCarrerasEnCombo(DropDownList ddlCarrera, string facultad)
        {
            ddlCarrera.Items.Clear();

            if (string.IsNullOrEmpty(facultad) || facultad == "-- Seleccione --")
            {
                ddlCarrera.Items.Add(new ListItem("-- Seleccione Facultad Primero --", ""));
                return; 
            }

            ddlCarrera.Items.Add(new ListItem("-- Seleccione Carrera --", ""));
            switch (facultad)
            {
                case "CIYA":
                    ddlCarrera.Items.Add(new ListItem("SISTEMAS DE INFORMACIÓN", "SISTEMAS DE INFORMACIÓN"));
                    ddlCarrera.Items.Add(new ListItem("INDUSTRIAL", "INDUSTRIAL"));
                    ddlCarrera.Items.Add(new ListItem("ELECTROMECÁNICA", "ELECTROMECANICA"));
                    ddlCarrera.Items.Add(new ListItem("ELECTRICIDAD", "ELECTRICIDAD"));
                    ddlCarrera.Items.Add(new ListItem("HIDRAULICA", "HIDRAULICA"));
                    ddlCarrera.Items.Add(new ListItem("SOFTWARE", "SOFTWARE"));
                    break;
                case "CAREN":
                    ddlCarrera.Items.Add(new ListItem("AGROINDUSTRIAL", "AGROINDUSTRIAL"));
                    ddlCarrera.Items.Add(new ListItem("AGRONOMÍA", "AGRONOMIA"));
                    ddlCarrera.Items.Add(new ListItem("VETERINARIA", "VETERINARIA"));
                    ddlCarrera.Items.Add(new ListItem("AMBIENTE", "AMBIENTE"));
                    ddlCarrera.Items.Add(new ListItem("TURISMO", "TURISMO"));
                    ddlCarrera.Items.Add(new ListItem("AGROPECUARIAS", "AGROPECUARIAS"));
                    ddlCarrera.Items.Add(new ListItem("BIOTECNOLOGIA", "BIOTECNOLOGIA"));
                    break;
                case "CAYE":
                    ddlCarrera.Items.Add(new ListItem("GESTIÓN DE LA INFORMACION GERENCIAL", "GESTIÓN DE LA INFORMACION GERENCIAL"));
                    ddlCarrera.Items.Add(new ListItem("ADMINISTRACIÓN DE EMPRESAS", "ADMINISTRACIÓN DE EMPRESAS"));
                    ddlCarrera.Items.Add(new ListItem("CONTABILIDAD", "CONTABILIDAD"));
                    ddlCarrera.Items.Add(new ListItem("ECONOMIA", "ECONOMIA"));
                    ddlCarrera.Items.Add(new ListItem("FINANZAS", "FINANZAS"));
                    ddlCarrera.Items.Add(new ListItem("MERCADOTÉCNIA", "MERCADOTÉCNIA"));
                    ddlCarrera.Items.Add(new ListItem("GESTIÓN DEL TALENTO HUMANO", "GESTIÓN DEL TALENTO HUMANO"));
                    break;
                case "CSAYE":
                    ddlCarrera.Items.Add(new ListItem("DISEÑO GRAFICO", "DISEÑO GRAFICO"));
                    ddlCarrera.Items.Add(new ListItem("DISEÑO GRAFICO INTERACTIVO", "DISEÑO GRAFICO INTERACTIVO"));
                    ddlCarrera.Items.Add(new ListItem("COMUNICACIÓN DIGITAL ESTRATEGICA", "COMUNICACIÓN DIGITAL ESTRATEGICA"));
                    ddlCarrera.Items.Add(new ListItem("TRABAJO SOCIAL", "TRABAJO SOCIAL"));
                    ddlCarrera.Items.Add(new ListItem("ANIMACIÓN DIGITAL", "ANIMACIÓN DIGITAL"));
                    ddlCarrera.Items.Add(new ListItem("PSICOLOGÍA SOCIAL", "PSICOLOGÍA SOCIAL"));
                    break;
                case "SALUD":
                    ddlCarrera.Items.Add(new ListItem("ENFERMERIA", "ENFERMERIA"));
                    break;
                case "PUJILI":
                    ddlCarrera.Items.Add(new ListItem("EDUCACIÓN INICIAL", "EDUCACIÓN INICIAL"));
                    ddlCarrera.Items.Add(new ListItem("EDUCACIÓN BASICA", "EDUCACIÓN BASICA"));
                    ddlCarrera.Items.Add(new ListItem("PEDAGOGÍA DEL IDIOMA INGLÉS", "PEDAGOGÍA DEL IDIOMA INGLÉS"));
                    ddlCarrera.Items.Add(new ListItem("PEDAGOGÍA DE LA LENGUA Y LITERATURA", "PEDAGOGÍA DE LA LENGUA Y LITERATURA"));
                    ddlCarrera.Items.Add(new ListItem("PEDAGOGÍA DE LAS MATEMÁTICAS Y LA FÍSICA", "PEDAGOGÍA DE LAS MATEMÁTICAS Y LA FÍSICA"));
                    break;
                case "LAMANA":
                    ddlCarrera.Items.Add(new ListItem("CONTABILIDAD_LM", "CONTABILIDAD_LM"));
                    ddlCarrera.Items.Add(new ListItem("ADMINISTRACIÓN_LM", "ADMINISTRACIÓN_LM"));
                    ddlCarrera.Items.Add(new ListItem("ELECTROMECÁNICA_LM", "ELECTROMECÁNICA_LM"));
                    ddlCarrera.Items.Add(new ListItem("SISTEMAS DE INFORMACIÓN_LM", "SISTEMAS DE INFORMACIÓN_LM"));
                    ddlCarrera.Items.Add(new ListItem("TURISMO_LM", "TURISMO_LM"));
                    ddlCarrera.Items.Add(new ListItem("AGRONOMÍA_LM", "AGRONOMÍA_LM"));
                    ddlCarrera.Items.Add(new ListItem("AGROINDUSTRIAS_LM", "AGROINDUSTRIAS_LM"));
                    break;
            }
        }

        protected void ddlFacultadCoord_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarCarrerasEnCombo(ddlCarreraCoord, ddlFacultadCoord.SelectedValue);
            ScriptManager.RegisterStartupScript(this, GetType(), "ReOpenC", "abrirModalCoord(); toggleTipoCoordinador();", true);
        }

        protected void ddlFacultadInt_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarCarrerasEnCombo(ddlCarreraInt, ddlFacultadInt.SelectedValue);
            ScriptManager.RegisterStartupScript(this, GetType(), "ReOpenI", "InitFormulario();", true);
        }

        // CEDULAS
        protected void btnValidarCedulaInt_Click(object sender, EventArgs e)
        {
            CambiarVista(Vista.FormularioIntegrante);
            string cedula = txtCedulaInt.Text.Trim();

            if (!EsCedulaValida(cedula))
            {
                txtCedulaInt.CssClass = "form-control is-invalid";
                Msg("Cédula ingresada Incorrecta.", "ee");
            }
            else
            {
                string grupoOcupado = _manejador.VerificarIntegranteEnOtroGrupo(cedula, hfGrupoIdActual.Value);
                if (!string.IsNullOrEmpty(grupoOcupado))
                {
                    txtCedulaInt.CssClass = "form-control is-invalid";
                    Msg($"Esta persona YA EXISTE en el grupo: {grupoOcupado}.", "ww");
                }
                else
                {
                    txtCedulaInt.CssClass = "form-control is-valid";
                    Msg("Cédula Válida y Disponible.", "ss");
                    txtNombresInt.Focus();
                }
            }

            string script = $"RenderizarEstadoVisual('{ddlTipoInt.SelectedValue}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "RestaurarUI_Int", script, true);
        }

        protected void btnValidarCedulaCoord_Click(object sender, EventArgs e)
        {
            pnlFormularioGrupo.Visible = true;
            string cedula = txtCedulaCoord.Text.Trim();

            if (!EsCedulaValida(cedula))
            {
                txtCedulaCoord.CssClass = "form-control bg-light is-invalid";
                Msg("Cédula ingresada Incorrecta.", "ee");
            }
            else
            {
                string grupoOcupado = _manejador.VerificarIntegranteEnOtroGrupo(cedula, hfIdGrupo.Value);
                if (!string.IsNullOrEmpty(grupoOcupado))
                {
                    txtCedulaCoord.CssClass = "form-control bg-light is-invalid";
                    Msg($"Esta persona ya mantiene un registro actual en: {grupoOcupado}.", "ww");
                }
                else
                {
                    txtCedulaCoord.CssClass = "form-control bg-light is-valid";
                    Msg("Cédula Válida.", "ss");
                    txtNombreCoord.Focus();
                }
            }

            string script = $"RenderizarModalCoord('{ddlTipoCoord.SelectedValue}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "ReOpenModalVal", script, true);
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
            catch { return false; }
        }

        //
        protected void ddlTipoInt_SelectedIndexChanged(object sender, EventArgs e)
        {
            CambiarVista(Vista.FormularioIntegrante);

            txtCedulaInt.Text = "";
            txtNombresInt.Text = "";
            txtApellidosInt.Text = "";
            txtCorreoInt.Text = "";
            txtEntidadInt.Text = "";
            hfIdDocenteInt.Value = "";
            hfCertificadoIntVinculado.Value = "";

            txtCedulaInt.CssClass = "form-control";
            BloquearCamposDatosPersonales(false);

            string tipo = ddlTipoInt.SelectedValue;

            if (tipo == "Docente")
            {
                CargarComboDocentes();
            }
            else 
            {
                if (ddlFacultadInt.Items.Count > 0) ddlFacultadInt.SelectedIndex = 0;
                CargarCarrerasEnCombo(ddlCarreraInt, "");
            }

            string script = $"RenderizarEstadoVisual('{tipo}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "UI_Int", script, true);
        }

        protected void ddlTipoCoord_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtCedulaCoord.Text = "";
            txtNombreCoord.Text = "";
            txtApellidoCoord.Text = "";
            txtCorreoCoord.Text = "";
            txtEntidadCoord.Text = "";
            hfCoordIdDocente.Value = "";
            hfCoordArchivo.Value = "";

            txtCedulaCoord.CssClass = "form-control bg-light";
            AlternarBloqueoCamposCoord(false);

            pnlCargaArchivo.Visible = true;
            pnlArchivoRecuperado.Visible = false;
            btnCambiarArchivo.Visible = true;

            if (ddlFacultadCoord.Items.Count > 0) ddlFacultadCoord.SelectedIndex = 0;
            CargarCarrerasEnCombo(ddlCarreraCoord, "");
            if (ddlDocentesCoord.Items.Count > 0) ddlDocentesCoord.SelectedIndex = 0;

            string tipo = ddlTipoCoord.SelectedValue;

            if (tipo == "Docente")
            {
                var docentes = _manejador.ObtenerDocentesCategorizadosCombo();
                ddlDocentesCoord.Items.Clear();
                ddlDocentesCoord.Items.Add(new ListItem("-- Seleccione Docente --", ""));
                foreach (var d in docentes) ddlDocentesCoord.Items.Add(new ListItem(d.NombreCompleto, d.strId_doc));
            }

            string script = $"RenderizarModalCoord('{tipo}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "UI_Coord", script, true);
        }

        //

        protected void ddlFacultadGrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reutilizamos tu método existente "CargarCarrerasEnCombo"
            CargarCarrerasEnCombo(ddlCarreraGrupo, ddlFacultadGrupo.SelectedValue);
        }

    }
}