using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace SistemaGestionCGI
{
    public partial class InscripcionProyectos : System.Web.UI.Page
    {
        // 1. Instancias y Constantes
        private readonly ManejadorInscripcionProyectos _manejador = new ManejadorInscripcionProyectos();
        private const string RUTA_VIRTUAL_PROYECTOS = "~/Archivos/InscripcionProyectos/";

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

            // Carga Inicial
            CargarCombos();
            CargarGrilla();

            // Mensajes Flash
            if (Session["TempMsg"] != null)
            {
                Msg(Session["TempMsg"].ToString(), Session["TempTipo"].ToString());
                Session["TempMsg"] = null;
                Session["TempTipo"] = null;
            }
        }

        // =============================================
        // CARGA DE DATOS
        // =============================================

        private void CargarGrilla()
        {
            try
            {
                rptProyectos.DataSource = _manejador.ObtenerTodos();
                rptProyectos.DataBind();
            }
            catch (Exception ex) { Msg("Error al cargar la grilla: " + ex.Message, "ee"); }
        }

        private void CargarCombos()
        {
            try
            {
                var grupos = _manejador.ObtenerGruposCombo();
                LlenarListControl(ddlGrupo, grupos, "strNombre_gru", "strId_gru");

                var convocatorias = _manejador.ObtenerConvocatoriasCombo();
                LlenarListControl(ddlConv, convocatorias, "strNombre_conv", "strId_conv");
            }
            catch (Exception ex) { Msg("Error al cargar combos: " + ex.Message, "ee"); }
        }

        private void LlenarListControl(ListControl ddl, object dataSource, string textField, string valueField)
        {
            ddl.DataSource = dataSource;
            ddl.DataTextField = textField;
            ddl.DataValueField = valueField;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("-- Seleccione --", ""));
        }

        private void CargarCoordinadores(DropDownList ddl, string idGrupo, string idProyectoEdit = null)
        {
            ddl.Items.Clear();

            if (!string.IsNullOrEmpty(idGrupo))
            {
                var lista = _manejador.ObtenerCoordinadoresDisponibles(idGrupo, idProyectoEdit);

                ddl.DataSource = lista;
                ddl.DataTextField = "NombreCompleto";
                ddl.DataValueField = "strId_int";
                ddl.DataBind();
            }

            ddl.Items.Insert(0, new ListItem("-- Seleccione Coordinador Disponible --", ""));
        }

        // =============================================
        // EVENTOS DE INTERFAZ (DROPDOWNS)
        // =============================================

        protected void ddlGrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string idGrupo = ddlGrupo.SelectedValue;

            if (!string.IsNullOrEmpty(idGrupo))
            {
                CargarCoordinadores(ddlCoordinador, idGrupo, hfIdProyecto.Value);

                var info = _manejador.ObtenerInfoGrupo(idGrupo);
                if (info != null)
                {
                    lblNombreGrupoInfo.Text = info.strNombre_gru;
                    lblLineasInfo.Text = info.strLineasinv_gru;
                    pnlInfoGrupo.Visible = true;
                }
            }
            else
            {
                pnlInfoGrupo.Visible = false;
                ddlCoordinador.Items.Clear();
                ddlCoordinador.Items.Add(new ListItem("-- Seleccione Grupo Primero --", ""));
            }
        }

        protected void btnAbrirModalIntegrante_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlGrupo.SelectedValue))
            {
                Msg("Debe seleccionar un Grupo de Investigación antes de agregar un integrante.", "ww");
                return;
            }

            LimpiarCamposModal();

            ddlTipoInt.SelectedIndex = 0;

            pnlListadoDocente.Visible = false;  
            pnlDatosPersonales.Visible = true;

            txtFuncionDisplay.Text = "COORDINADOR DE PROYECTO";

            divInternoModal.Visible = true;
            divExternoModal.Visible = false;

            HabilitarEdicion(true);

            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "AbrirModalNuevoIntegrante();", true);
        }

        // =============================================
        // GESTIÓN DE INTEGRANTES (MODAL)
        // =============================================

        protected void btnGuardarIntegrante_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ddlGrupo.SelectedValue))
                {
                    Msg("Error: Primero seleccione un Grupo.", "ww");
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtNombresInt.Text) || string.IsNullOrWhiteSpace(txtCedulaInt.Text))
                {
                    Msg("Complete Cédula, Nombres y Apellidos.", "ww");
                    ScriptManager.RegisterStartupScript(this, GetType(), "reopen", "AbrirModalNuevoIntegrante(); ToggleTipoIntegranteModal();", true);
                    return;
                }

                string funcionAsignada = "";

                if (ddlTipoInt.SelectedValue == "Docente")
                {
                    funcionAsignada = "COORDINADOR DE PROYECTO (DOCENTE)";
                }
                else
                {
                    funcionAsignada = "COORDINADOR DE PROYECTO";
                }

                var integranteTemp = new InvgccGrupoIntegrantes
                {
                    fkId_gru = ddlGrupo.SelectedValue,
                    strCedula_int = txtCedulaInt.Text.Trim(),
                    strNombres_int = txtNombresInt.Text.Trim().ToUpper(),
                    strApellidos_int = txtApellidosInt.Text.Trim().ToUpper(),
                    strCorreo_int = txtCorreoInt.Text.Trim().ToLower(),
                    strTipo_int = ddlTipoInt.SelectedValue,
                    strFuncion_int = funcionAsignada,
                    fkId_docente_origen = (ddlTipoInt.SelectedValue == "Docente") ? hfIdDocenteInt.Value : null,
                    strCertificado_int = (ddlTipoInt.SelectedValue == "Docente") ? hfRutaArchivoDocente.Value : null,

                    strCarrera_int = (ddlTipoInt.SelectedValue != "Externo") ? ddlCarreraInt.SelectedValue : null,
                    strFacultad_int = (ddlTipoInt.SelectedValue != "Externo") ? ddlFacultadInt.SelectedValue : null,
                    strEntidad_int = (ddlTipoInt.SelectedValue == "Externo") ? txtEntidadInt.Text.Trim().ToUpper() : null
                };

                string jsonIntegrante = JsonConvert.SerializeObject(integranteTemp);
                ViewState["IntegrantePendiente"] = jsonIntegrante;

                string nombreVisual = $"{integranteTemp.strApellidos_int} {integranteTemp.strNombres_int} (NUEVO - PENDIENTE)";

                ddlCoordinador.ClearSelection();
                ListItem itemViejo = ddlCoordinador.Items.FindByValue("TEMP_NEW");
                if (itemViejo != null) ddlCoordinador.Items.Remove(itemViejo);

                ListItem itemTemp = new ListItem(nombreVisual, "TEMP_NEW");
                itemTemp.Selected = true;
                ddlCoordinador.Items.Add(itemTemp);

                LimpiarCamposModal();
                Msg("Integrante listo para vincular.", "ii");
            }
            catch (Exception ex)
            {
                Msg("Error: " + ex.Message, "ee");
            }
        }

        private string GetProp(object obj, string propName)
        {
            if (obj == null) return "";
            try
            {
                if (obj is JObject jobj)
                    return jobj[propName]?.ToString() ?? "";

                var prop = obj.GetType().GetProperty(propName);
                if (prop != null)
                    return prop.GetValue(obj, null)?.ToString() ?? "";

                return ((dynamic)obj).GetType().GetProperty(propName)?.GetValue(obj, null)?.ToString() ?? "";
            }
            catch
            {
                return "";
            }
        }

        // =============================================
        // CRUD PRINCIPAL: GUARDAR Y ACTUALIZAR
        // =============================================

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            PrepararGestion(null);
        }

        private void PrepararGestion(string idProyecto)
        {
            pnlGrilla.Visible = false;
            pnlGestion.Visible = true;
            btnNuevo.Visible = false;
            btnRegresar.Visible = true;

            if (string.IsNullOrEmpty(idProyecto))
            {
                lblTituloGestion.Text = "Registrar Nuevo Proyecto";
                lblBtnGuardar.Text = "Guardar";
                hfIdProyecto.Value = ""; 

                txtTema.Text = "";
                txtPuntaje.Text = "";
                txtFecha.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtDuracionDisplay.Text = "";

                hfAnios.Value = "0"; hfMeses.Value = "0"; hfSemanas.Value = "0"; hfDias.Value = "0";

                if (ddlGrupo.Items.Count > 0) ddlGrupo.SelectedIndex = 0;
                ddlCoordinador.Items.Clear();
                ddlCoordinador.Items.Add(new ListItem("-- Seleccione Grupo Primero --", ""));

                pnlArchivoActual.Visible = false;
                hfArchivoActual.Value = "";
                pnlInfoGrupo.Visible = false;
            }
            else
            {
                var pro = _manejador.ObtenerPorId(idProyecto);
                if (pro == null) { Msg("Proyecto no encontrado.", "ee"); return; }

                lblTituloGestion.Text = $"Editar Proyecto: {idProyecto}";
                lblBtnGuardar.Text = "Actualizar";
                hfIdProyecto.Value = pro.strId_pro;

                txtTema.Text = pro.strTema_pro;
                txtPuntaje.Text = pro.intPuntaje_pro?.ToString() ?? "";
                txtFecha.Text = pro.dtFehains_pro.ToString("yyyy-MM-dd");

                hfAnios.Value = ExtraerNumeroDeTexto(pro.strDuracion_pro, "Año");
                hfMeses.Value = ExtraerNumeroDeTexto(pro.strDuracion_pro, "Mes");
                hfSemanas.Value = ExtraerNumeroDeTexto(pro.strDuracion_pro, "Semana");
                hfDias.Value = ExtraerNumeroDeTexto(pro.strDuracion_pro, "Día");
                txtDuracionDisplay.Text = pro.strDuracion_pro;

                SeleccionarCombo(ddlConv, pro.fkId_conv);

                if (ddlGrupo.Items.FindByValue(pro.fkId_gru) != null)
                {
                    ddlGrupo.SelectedValue = pro.fkId_gru;
                    CargarCoordinadores(ddlCoordinador, pro.fkId_gru, pro.strId_pro);

                    if (!string.IsNullOrEmpty(pro.fkId_coordinador))
                        SeleccionarItemSeguro(ddlCoordinador, pro.fkId_coordinador);
                    else
                        SeleccionarItemSeguro(ddlCoordinador, pro.strCoordinador_pro);

                    var infoG = _manejador.ObtenerInfoGrupo(pro.fkId_gru);
                    if (infoG != null)
                    {
                        lblNombreGrupoInfo.Text = infoG.strNombre_gru;
                        lblLineasInfo.Text = infoG.strLineasinv_gru;
                        pnlInfoGrupo.Visible = true;
                    }
                }

                hfArchivoActual.Value = pro.strArchivo_pro;
                if (!string.IsNullOrEmpty(pro.strArchivo_pro))
                {
                    pnlArchivoActual.Visible = true;
                    lblNombreArchivoActual.Text = Path.GetFileName(pro.strArchivo_pro);
                }
                else
                {
                    pnlArchivoActual.Visible = false;
                }
            }
        }

        protected void btnGuardarGestion_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlCoordinador.SelectedIndex <= 0 || string.IsNullOrEmpty(ddlCoordinador.SelectedValue))
                {
                    Msg("Seleccione un Coordinador.", "ww"); return;
                }
                if (string.IsNullOrWhiteSpace(txtTema.Text))
                {
                    Msg("El tema es obligatorio.", "ww"); return;
                }

                string duracionFinal = ConstruirDuracion(hfAnios.Value, hfMeses.Value, hfSemanas.Value, hfDias.Value);
                string idCoordinadorFinal = ddlCoordinador.SelectedValue;

                if (idCoordinadorFinal == "TEMP_NEW")
                {
                    string jsonPendiente = ViewState["IntegrantePendiente"] as string;
                    if (!string.IsNullOrEmpty(jsonPendiente))
                    {
                        var nuevoInt = JsonConvert.DeserializeObject<InvgccGrupoIntegrantes>(jsonPendiente);
                        string usuario = Session["UsuarioLogueado"]?.ToString() ?? "SISTEMA";
                        idCoordinadorFinal = _manejador.GuardarIntegranteExpress(nuevoInt, usuario);
                        ViewState["IntegrantePendiente"] = null;
                    }
                }

                var proyecto = new InvgccInscripcionProyectos
                {
                    strTema_pro = txtTema.Text.Trim(),
                    fkId_coordinador = idCoordinadorFinal,
                    strCoordinador_pro = ddlCoordinador.SelectedItem.Text,
                    strDuracion_pro = duracionFinal,
                    dtFehains_pro = DateTime.Parse(txtFecha.Text),
                    fkId_gru = ddlGrupo.SelectedValue,
                    fkId_conv = ddlConv.SelectedValue,
                    intPuntaje_pro = int.TryParse(txtPuntaje.Text, out int pt) ? (int?)pt : null
                };

                string rutaArchivo = hfArchivoActual.Value;

                if (flpArchivo.HasFile)
                {
                    if (!ValidarExtension(flpArchivo.FileName)) return;
                    string nombreUnico = $"PROY_{DateTime.Now.Ticks}{Path.GetExtension(flpArchivo.FileName)}";
                    rutaArchivo = GuardarArchivoFisico(flpArchivo, nombreUnico);
                }

                proyecto.strArchivo_pro = rutaArchivo;

                if (string.IsNullOrEmpty(hfIdProyecto.Value))
                {
                    _manejador.Guardar(proyecto);
                    Redireccionar("Proyecto registrado exitosamente.", "ss");
                }
                else
                {
                    proyecto.strId_pro = hfIdProyecto.Value;
                    _manejador.Actualizar(proyecto);
                    Redireccionar("Proyecto actualizado correctamente.", "ss");
                }
            }
            catch (Exception ex) { Msg("Error al procesar: " + ex.Message, "ee"); }
        }

        // =============================================
        // ACCIONES DE GRILLA (COMANDOS)
        // =============================================

        protected void rptProyectos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();

            switch (e.CommandName)
            {
                case "ver":
                    DescargarArchivo(id);
                    break;

                case "eliminar":
                    try
                    {
                        _manejador.Eliminar(id);
                        Redireccionar("Proyecto eliminado.", "ss");
                    }
                    catch (Exception ex) { Msg("Error al eliminar: " + ex.Message, "ee"); }
                    break;

                case "CambiarEstado":
                    CargarModalEstado(id);
                    break;

                case "editar":
                    PrepararGestion(id);
                    break;
            }
        }

        private void CargarModalEstado(string id)
        {
            var pro = _manejador.ObtenerPorId(id);
            if (pro != null)
            {
                hfldProyectoEstado.Value = pro.strId_pro;

                string temaSeguro = (pro.strTema_pro ?? "").Replace("'", "").Replace("\r", "").Replace("\n", " ");
                string scriptInfo = $@"
                    document.getElementById('infoldPro').innerText = '{pro.strId_pro}';
                    document.getElementById('infoTemaPro').innerText = '{temaSeguro}';
                    document.getElementById('infoEstadoPro').innerText = '{pro.strEstado_pro}';
                ";

                ddlNuevoEstado.Items.Clear();
                ddlNuevoEstado.Enabled = true;
                btnConfirmarEstadoPro.Visible = true;
                txtObservacionEstado.Enabled = true;

                if (pro.strEstado_pro == "Pendiente")
                {
                    ddlNuevoEstado.Items.Add(new ListItem("-- Seleccione Acción --", ""));
                    ddlNuevoEstado.Items.Add(new ListItem("APROBAR PROYECTO", "Aprobado"));
                    ddlNuevoEstado.Items.Add(new ListItem("RECHAZAR PROYECTO", "Rechazado"));
                }
                else if (pro.strEstado_pro == "Rechazado")
                {
                    ddlNuevoEstado.Items.Add(new ListItem("⏳ DEVOLVER A PENDIENTE", "Pendiente"));
                    txtObservacionEstado.Attributes["placeholder"] = "Indique que se ha levantado la observación...";
                }
                else
                {
                    ddlNuevoEstado.Items.Add(new ListItem("PROYECTO CERRADO", ""));
                    ddlNuevoEstado.Enabled = false;
                    btnConfirmarEstadoPro.Visible = false;
                }

                string scriptFinal = scriptInfo + "AbrirModalEstadoPro();";
                ScriptManager.RegisterStartupScript(this, GetType(), "modalEstado", scriptFinal, true);
            }
        }

        protected void btnConfirmarEstadoPro_Click(object sender, EventArgs e)
        {
            try
            {
                string id = hfldProyectoEstado.Value;
                string nuevoEstado = ddlNuevoEstado.SelectedValue;
                string observacion = txtObservacionEstado.Text;

                if (string.IsNullOrEmpty(nuevoEstado))
                {
                    Msg("Debe seleccionar una acción válida.", "ww");
                    return;
                }

                _manejador.CambiarEstado(id, nuevoEstado, observacion);

                if (nuevoEstado == "Pendiente")
                {
                    Redireccionar("El proyecto ha vuelto a estado PENDIENTE para revisión.", "ss");
                }
                else
                {
                    string tipo = (nuevoEstado == "Rechazado") ? "ww" : "ss";
                    Redireccionar($"El proyecto ha sido: {nuevoEstado}", tipo);
                }
            }
            catch (Exception ex)
            {
                Msg("Error: " + ex.Message, "ee");
            }
        }

        // =============================================
        // NAVEGACIÓN Y CANCELACIÓN
        // =============================================

        private void LimpiarCamposModal()
        {
            hfIdDocenteInt.Value = "";
            txtCedulaInt.Text = "";
            txtNombresInt.Text = "";
            txtApellidosInt.Text = "";
            txtCorreoInt.Text = "";
            txtEntidadInt.Text = "";

            if (ddlFacultadInt.Items.Count > 0) ddlFacultadInt.SelectedIndex = 0;

            ddlCarreraInt.Items.Clear();
            ddlCarreraInt.Items.Add(new ListItem("-- Seleccione Facultad Primero --", ""));

            pnlAlertaDocente.Visible = false;
            hfRutaArchivoDocente.Value = "";
            lnkVerArchivoDocente.NavigateUrl = "";

            HabilitarEdicion(true);
        }

        private void HabilitarEdicion(bool habilitar)
        {
            txtCedulaInt.ReadOnly = !habilitar;
            txtNombresInt.ReadOnly = !habilitar;
            txtApellidosInt.ReadOnly = !habilitar;
            txtCorreoInt.ReadOnly = !habilitar;

            ddlFacultadInt.Enabled = habilitar;
            ddlCarreraInt.Enabled = habilitar; 

            string cssText = habilitar ? "form-control form-control-sm" : "form-control form-control-sm bg-light";
            string cssDdl = habilitar ? "form-select form-select-sm" : "form-select form-select-sm bg-light";

            txtCedulaInt.CssClass = cssText;
            txtNombresInt.CssClass = cssText;
            ddlFacultadInt.CssClass = cssDdl;
            ddlCarreraInt.CssClass = cssDdl;
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            Response.Redirect("InscripcionProyectos.aspx");
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            btnRegresar_Click(sender, e);
        }

        protected void btnCancelarEdit_Click(object sender, EventArgs e)
        {
            btnRegresar_Click(sender, e);
        }

        // =============================================
        // UTILIDADES Y ARCHIVOS
        // =============================================

        private string ConstruirDuracion(string anios, string meses, string semanas, string dias)
        {
            List<string> partes = new List<string>();

            int a = int.TryParse(anios, out int va) ? va : 0;
            int m = int.TryParse(meses, out int vm) ? vm : 0;
            int s = int.TryParse(semanas, out int vs) ? vs : 0;
            int d = int.TryParse(dias, out int vd) ? vd : 0;

            if (a > 0) partes.Add($"{a} {(a == 1 ? "Año" : "Años")}");
            if (m > 0) partes.Add($"{m} {(m == 1 ? "Mes" : "Meses")}");
            if (s > 0) partes.Add($"{s} {(s == 1 ? "Semana" : "Semanas")}");
            if (d > 0) partes.Add($"{d} {(d == 1 ? "Día" : "Días")}");

            if (partes.Count == 0) return "Indefinida";

            return string.Join(", ", partes);
        }

        private string ExtraerNumeroDeTexto(string textoCompleto, string palabraClave)
        {
            if (string.IsNullOrEmpty(textoCompleto)) return "0";

            var partes = textoCompleto.Split(',');
            foreach (var parte in partes)
            {
                if (parte.Contains(palabraClave) || parte.Contains(palabraClave.ToLower()))
                {
                    string numero = "";
                    foreach (char c in parte) if (char.IsDigit(c)) numero += c;
                    return numero;
                }
            }
            return "0";
        }

        private void CargarDuracionEnControles(string duracionTexto, TextBox txtNum, DropDownList ddlUnidad)
        {
            if (string.IsNullOrEmpty(duracionTexto))
            {
                txtNum.Text = "";
                ddlUnidad.SelectedIndex = 0;
                return;
            }

            string[] partes = duracionTexto.Split(' ');

            if (partes.Length >= 2)
            {
                txtNum.Text = partes[0]; 

                string unidad = partes[1];
                if (ddlUnidad.Items.FindByValue(unidad) != null)
                    ddlUnidad.SelectedValue = unidad;
            }
            else
            {
                txtNum.Text = duracionTexto;
            }
        }

        private bool ValidarExtension(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLower();
            if (ext != ".pdf" && ext != ".xls" && ext != ".xlsx")
            {
                Msg("Formato no permitido (Solo PDF, XLS, XLSX).", "ww");
                return false;
            }
            return true;
        }

        private string GuardarArchivoFisico(FileUpload ctl, string nombre)
        {
            string rutaFolderFisica = Server.MapPath(RUTA_VIRTUAL_PROYECTOS);

            if (!Directory.Exists(rutaFolderFisica)) Directory.CreateDirectory(rutaFolderFisica);

            string rutaCompletaFisica = Path.Combine(rutaFolderFisica, nombre);
            ctl.SaveAs(rutaCompletaFisica);

            return Path.Combine(RUTA_VIRTUAL_PROYECTOS, nombre).Replace("\\", "/");
        }

        private void DescargarArchivo(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            string rutaFisica = path.StartsWith("~") ? Server.MapPath(path) : path;

            if (File.Exists(rutaFisica))
            {
                string ext = Path.GetExtension(rutaFisica).ToLower();
                Response.Clear();
                switch (ext)
                {
                    case ".pdf": Response.ContentType = "application/pdf"; break;
                    case ".xls": Response.ContentType = "application/vnd.ms-excel"; break;
                    case ".xlsx": Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; break;
                    default: Response.ContentType = "application/octet-stream"; break;
                }
                Response.AddHeader("Content-Disposition", "inline; filename=" + Path.GetFileName(rutaFisica));
                Response.WriteFile(rutaFisica);
                Response.End();
            }
            else
            {
                Msg("El archivo no existe en el servidor.", "ww");
            }
        }

        private void Redireccionar(string msg, string type)
        {
            Session["TempMsg"] = msg;
            Session["TempTipo"] = type;
            Response.Redirect("InscripcionProyectos.aspx", false);
        }

        private void Msg(string msg, string type)
        {
            if (string.IsNullOrEmpty(msg)) return;
            string cleanMsg = msg.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\r\n", " ").Replace("\n", " ");
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"$(function() {{ toastify('{type}', '{cleanMsg}', 'Sistema'); }});", true);
        }

        //

        protected void ddlTipoInt_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tipo = ddlTipoInt.SelectedValue;
            LimpiarCamposModal();

            if (tipo == "Docente")
            {
                pnlListadoDocente.Visible = true;
                pnlDatosPersonales.Visible = false;

                txtFuncionDisplay.Text = "COORDINADOR DE PROYECTO (DOCENTE)";

                var lista = _manejador.ObtenerDocentesSinGrupo();
                LlenarListControl(ddlDocentesDisponibles, lista, "NombreCompleto", "strCedula_doc");
                ddlDocentesDisponibles.Items.Insert(0, new ListItem("-- Busque Docente --", ""));
            }
            else
            {
                pnlListadoDocente.Visible = false;
                pnlDatosPersonales.Visible = true;

                txtFuncionDisplay.Text = "COORDINADOR DE PROYECTO";
                HabilitarEdicion(true); 

                divInternoModal.Visible = (tipo == "Interno");
                divExternoModal.Visible = (tipo == "Externo");
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "reOpen", "AbrirModalNuevoIntegrante();", true);
        }

        protected void ddlDocentesDisponibles_SelectedIndexChanged(object sender, EventArgs e)
        {
            string cedula = ddlDocentesDisponibles.SelectedValue;

            pnlAlertaDocente.Visible = false;
            lnkVerArchivoDocente.NavigateUrl = "";
            hfRutaArchivoDocente.Value = "";

            if (!string.IsNullOrEmpty(cedula))
            {
                dynamic docente = _manejador.ObtenerDocenteCategorizado(cedula);

                if (docente != null)
                {
                    txtCedulaInt.Text = GetProp(docente, "strCedula_doc");
                    txtNombresInt.Text = GetProp(docente, "strNombres_doc");
                    txtApellidosInt.Text = GetProp(docente, "strApellidos_doc");
                    txtCorreoInt.Text = GetProp(docente, "strCorreo_doc");
                    hfIdDocenteInt.Value = GetProp(docente, "strId_doc");

                    string facultadBD = GetProp(docente, "strFacultad_doc");
                    SeleccionarCombo(ddlFacultadInt, facultadBD);

                    CargarCarrerasEnCombo(ddlCarreraInt, facultadBD);

                    string carreraBD = GetProp(docente, "strCarrera_doc");
                    SeleccionarCombo(ddlCarreraInt, carreraBD);

                    string rutaArchivo = GetProp(docente, "strCertificado_doc");
                    if (!string.IsNullOrEmpty(rutaArchivo))
                    {
                        pnlAlertaDocente.Visible = true;
                        lnkVerArchivoDocente.NavigateUrl = ResolveUrl(rutaArchivo);
                        hfRutaArchivoDocente.Value = rutaArchivo;
                    }

                    pnlDatosPersonales.Visible = true;
                    divInternoModal.Visible = true;
                    divExternoModal.Visible = false;

                    HabilitarEdicion(false); 

                    Msg("Datos del docente cargados y vinculados.", "ss");
                }
            }
            else
            {
                pnlDatosPersonales.Visible = false;
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "reOpen", "AbrirModalNuevoIntegrante();", true);
        }

        private void SeleccionarItemSeguro(DropDownList ddl, string valorBD)
        {
            if (string.IsNullOrEmpty(valorBD)) return;

            ddl.ClearSelection(); 
            valorBD = valorBD.Trim().ToUpper(); 

            ListItem item = ddl.Items.FindByValue(valorBD);

            if (item == null) item = ddl.Items.FindByText(valorBD);

            if (item == null)
            {
                foreach (ListItem li in ddl.Items)
                {
                    if (li.Text.ToUpper().Contains(valorBD) || li.Value.ToUpper().Contains(valorBD))
                    {
                        item = li;
                        break; 
                    }
                }
            }

            if (item != null)
            {
                item.Selected = true;
            }
        }

        private void CargarCarrerasEnCombo(DropDownList ddlCarrera, string facultad)
        {
            ddlCarrera.Items.Clear();
            ddlCarrera.Items.Add(new ListItem("-- Seleccione Carrera --", ""));

            if (string.IsNullOrEmpty(facultad)) return;

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
                    ddlCarrera.Items.Add(new ListItem("AGRONOMÍA", "AGRONOMIA"));
                    ddlCarrera.Items.Add(new ListItem("VETERINARIA", "VETERINARIA"));
                    ddlCarrera.Items.Add(new ListItem("AMBIENTE", "AMBIENTE"));
                    ddlCarrera.Items.Add(new ListItem("TURISMO", "TURISMO"));
                    ddlCarrera.Items.Add(new ListItem("AGROPECUARIAS", "AGROPECUARIAS"));
                    ddlCarrera.Items.Add(new ListItem("BIOTECNOLOGIA", "BIOTECNOLOGIA"));
                    break;
                case "CAYE":
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
                    ddlCarrera.Items.Add(new ListItem("COMUNICACIÓN", "COMUNICACIÓN"));
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

        protected void ddlFacultadInt_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarCarrerasEnCombo(ddlCarreraInt, ddlFacultadInt.SelectedValue);
            ScriptManager.RegisterStartupScript(this, GetType(), "ReOpenFac", "AbrirModalNuevoIntegrante();", true);
        }


        protected void btnValidarCedulaServer_Click(object sender, EventArgs e)
        {
            string cedula = txtCedulaInt.Text.Trim();

            if (string.IsNullOrEmpty(cedula))
            {
                Msg("Ingrese un número de cédula.", "ww");
            }
            else if (EsCedulaValida(cedula))
            {
                txtCedulaInt.CssClass = "form-control form-control-sm is-valid";
                Msg("El numero de cedula es valido.", "ss");
            }
            else
            {
                txtCedulaInt.CssClass = "form-control form-control-sm is-invalid";
                Msg("La cédula ingresada es INCORRECTA.", "ee");

            }

            ScriptManager.RegisterStartupScript(this, GetType(), "reOpenVal", "AbrirModalNuevoIntegrante();", true);
        }

        private bool EsCedulaValida(string cedula)
        {
            if (string.IsNullOrEmpty(cedula) || cedula.Length != 10) return false;

            if (!long.TryParse(cedula, out long _)) return false;

            try
            {
                int provincia = int.Parse(cedula.Substring(0, 2));
                if ((provincia < 1 || provincia > 24) && provincia != 30) return false;

                int tercerDigito = int.Parse(cedula.Substring(2, 1));
                if (tercerDigito >= 6) return false;

                int[] coeficientes = { 2, 1, 2, 1, 2, 1, 2, 1, 2 };
                int verificador = int.Parse(cedula.Substring(9, 1));
                int suma = 0;

                for (int i = 0; i < 9; i++)
                {
                    int valor = int.Parse(cedula.Substring(i, 1)) * coeficientes[i];

                    if (valor >= 10)
                        suma += (valor - 9);
                    else
                        suma += valor;
                }

                int digitoCalculado = 0;
                int residuo = suma % 10;

                if (residuo != 0)
                {
                    digitoCalculado = 10 - residuo;
                }

                return verificador == digitoCalculado;
            }
            catch
            {
                return false;
            }
        }

    }
}