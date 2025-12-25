using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;

namespace SistemaGestionCGI
{
    public partial class InscripcionProyectos : System.Web.UI.Page
    {
        // 1. Instancias y Constantes
        private readonly ManejadorInscripcionProyectos _manejador = new ManejadorInscripcionProyectos();
        private const string RUTA_PROYECTOS = @"C:\UTC\PROYECTOS\";

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
                LlenarListControl(ddlGrupoEdit, grupos, "strNombre_gru", "strId_gru");

                var convocatorias = _manejador.ObtenerConvocatoriasCombo();
                LlenarListControl(ddlConv, convocatorias, "strNombre_conv", "strId_conv");
                LlenarListControl(ddlConvEdit, convocatorias, "strNombre_conv", "strId_conv");
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

        private void CargarCoordinadores(DropDownList ddl, string idGrupo)
        {
            ddl.Items.Clear();
            if (!string.IsNullOrEmpty(idGrupo))
            {
                ddl.DataSource = _manejador.ObtenerIntegrantesPorGrupo(idGrupo);
                ddl.DataTextField = "NombreCompleto";
                ddl.DataValueField = "NombreCompleto";
                ddl.DataBind();
            }
            ddl.Items.Insert(0, new ListItem("-- Seleccione Coordinador --", ""));
        }

        // =============================================
        // EVENTOS DE INTERFAZ (DROPDOWNS)
        // =============================================

        protected void ddlGrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string idGrupo = ddlGrupo.SelectedValue;
            CargarCoordinadores(ddlCoordinador, idGrupo);

            // Mostrar Info Card
            if (!string.IsNullOrEmpty(idGrupo))
            {
                var info = _manejador.ObtenerInfoGrupo(idGrupo);
                if (info != null)
                {
                    lblNombreGrupoInfo.Text = info.strNombre_gru;
                    lblLineasInfo.Text = !string.IsNullOrEmpty(info.strLineasinv_gru) ? info.strLineasinv_gru : "General";
                    pnlInfoGrupo.Visible = true;
                    return;
                }
            }
            pnlInfoGrupo.Visible = false;
        }

        protected void ddlGrupoEdit_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarCoordinadores(ddlCoordinadorEdit, ddlGrupoEdit.SelectedValue);
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
                    Msg("Error: No hay grupo seleccionado.", "ww");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtNombresInt.Text) || string.IsNullOrWhiteSpace(txtCedulaInt.Text))
                {
                    Msg("Complete los campos obligatorios.", "ww");
                    ScriptManager.RegisterStartupScript(this, GetType(), "reopen", "AbrirModalNuevoIntegrante();", true);
                    return;
                }

                // Construcción limpia del objeto
                var nuevo = new InvgccGrupoIntegrantes
                {
                    fkId_gru = ddlGrupo.SelectedValue,
                    strCedula_int = txtCedulaInt.Text.Trim(),
                    strNombres_int = txtNombresInt.Text.Trim(),
                    strApellidos_int = txtApellidosInt.Text.Trim(),
                    strCorreo_int = txtCorreoInt.Text.Trim(),
                    strFuncion_int = ddlFuncionIntModal.SelectedValue,
                    strObservacion_int = txtObservacionInt.Text.Trim(),
                    strTipo_int = ddlTipoInt.SelectedValue
                };

                if (nuevo.strFuncion_int == "Investigador Principal")
                {
                    if (flpCertificadoModal.HasFile)
                    {
                        string nombre = $"CERT_{DateTime.Now.Ticks}{Path.GetExtension(flpCertificadoModal.FileName)}";
                        string rutaBase = @"C:\UTC\GRUPOS\CERTIFICADOS\";
                        if (!Directory.Exists(rutaBase)) Directory.CreateDirectory(rutaBase);

                        string rutaCompleta = Path.Combine(rutaBase, nombre);
                        flpCertificadoModal.SaveAs(rutaCompleta);

                        nuevo.strCertificado_int = rutaCompleta;
                    }
                }

                if (nuevo.strTipo_int == "Externo")
                {
                    nuevo.strEntidad_int = txtEntidadInt.Text.Trim();
                    if (string.IsNullOrEmpty(nuevo.strEntidad_int))
                    {
                        Msg("Especifique la Institución para externos.", "ww");
                        ScriptManager.RegisterStartupScript(this, GetType(), "reopen", "AbrirModalNuevoIntegrante(); ToggleTipoIntegrante(document.getElementById('ddlTipoInt'));", true);
                        return;
                    }
                }
                else
                {
                    nuevo.strCarrera_int = txtCarreraInt.Text.Trim();
                    nuevo.strFacultad_int = ddlFacultadInt.SelectedValue;
                }

                _manejador.GuardarIntegranteExpress(nuevo);
                CargarCoordinadores(ddlCoordinador, ddlGrupo.SelectedValue);
                string nombreCompleto = $"{nuevo.strApellidos_int} {nuevo.strNombres_int}";
                var item = ddlCoordinador.Items.FindByText(nombreCompleto);
                if (item != null) item.Selected = true;

                txtCedulaInt.Text = ""; txtNombresInt.Text = ""; txtApellidosInt.Text = "";
                txtCorreoInt.Text = ""; txtCarreraInt.Text = ""; ddlFuncionIntModal.SelectedIndex = 0; txtObservacionInt.Text = "";
                ddlFacultadInt.SelectedIndex = 0;

                Msg("Integrante registrado y seleccionado.", "ss");
            }
            catch (Exception ex) { Msg("Error al guardar integrante: " + ex.Message, "ee"); }
        }

        // =============================================
        // CRUD PRINCIPAL: GUARDAR Y ACTUALIZAR
        // =============================================

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            pnlGrilla.Visible = false;
            pnlFormulario.Visible = true;
            pnlEdicion.Visible = false;
            btnNuevo.Visible = false;
            btnRegresar.Visible = true;

            txtFecha.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtTema.Text = "";
            ddlGrupo.SelectedIndex = 0;
            ddlConv.SelectedIndex = 0;
            ddlCoordinador.Items.Clear();
            ddlCoordinador.Items.Add(new ListItem("-- Seleccione Grupo Primero --", ""));
            pnlInfoGrupo.Visible = false;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTema.Text) || ddlCoordinador.SelectedIndex <= 0)
                {
                    Msg("Complete los campos obligatorios.", "ww");
                    return;
                }

                string duracionFinal = ConstruirDuracion(hfAnios.Value, hfMeses.Value, hfSemanas.Value, hfDias.Value);

                if (string.IsNullOrWhiteSpace(txtTema.Text) || duracionFinal == "Indefinida")
                {
                    Msg("Complete el tema y defina una duración.", "ww");
                    return;
                }

                var nuevo = new InvgccInscripcionProyectos
                {
                    strTema_pro = txtTema.Text.Trim(),
                    strCoordinador_pro = ddlCoordinador.SelectedValue,
                    strDuracion_pro = duracionFinal,
                    dtFehains_pro = DateTime.Parse(txtFecha.Text),
                    fkId_gru = ddlGrupo.SelectedValue,
                    fkId_conv = ddlConv.SelectedValue,
                    intPuntaje_pro = int.TryParse(txtPuntaje.Text, out int pt) ? (int?)pt : null
                };

                if (flpArchivo.HasFile)
                {
                    if (!ValidarExtension(flpArchivo.FileName)) return;
                    nuevo.strArchivo_pro = GuardarArchivoFisico(flpArchivo, $"PROY_{DateTime.Now.Ticks}{Path.GetExtension(flpArchivo.FileName)}");
                }

                _manejador.Guardar(nuevo);
                Redireccionar("Proyecto registrado correctamente.", "ss");
            }
            catch (Exception ex) { Msg("Error: " + ex.Message, "ee"); }
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            try
            {
                string duracionFinal = ConstruirDuracion(hfAniosEdit.Value, hfMesesEdit.Value, hfSemanasEdit.Value, hfDiasEdit.Value);

                var edit = new InvgccInscripcionProyectos
                {
                    strId_pro = hfIdEdit.Value,
                    strTema_pro = txtTemaEdit.Text.Trim(),
                    strCoordinador_pro = ddlCoordinadorEdit.SelectedValue,
                    strDuracion_pro = duracionFinal,
                    dtFehains_pro = DateTime.Parse(txtFechaEdit.Text),
                    fkId_gru = ddlGrupoEdit.SelectedValue,
                    fkId_conv = ddlConvEdit.SelectedValue,
                    strArchivo_pro = hfArchivoActual.Value,
                    intPuntaje_pro = int.TryParse(txtPuntajeEdit.Text, out int pt) ? (int?)pt : null
                };

                if (flpArchivoEdit.HasFile)
                {
                    if (!ValidarExtension(flpArchivoEdit.FileName)) return;
                    edit.strArchivo_pro = GuardarArchivoFisico(flpArchivoEdit, $"PROY_{DateTime.Now.Ticks}{Path.GetExtension(flpArchivoEdit.FileName)}");
                }

                _manejador.Actualizar(edit);
                Redireccionar("Proyecto actualizado.", "ss");
            }
            catch (Exception ex) { Msg("Error al actualizar: " + ex.Message, "ee"); }
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
                    CargarEdicion(id);
                    break;
            }
        }

        private void CargarEdicion(string id)
        {
            var pro = _manejador.ObtenerPorId(id);
            if (pro == null) return;

            hfIdEdit.Value = pro.strId_pro;
            txtTemaEdit.Text = pro.strTema_pro;
            txtPuntajeEdit.Text = pro.intPuntaje_pro?.ToString() ?? "";
            // --- LÓGICA DE DURACIÓN (DECONSTRUCCIÓN) ---
            // Llenamos los HiddenFields extrayendo los números del texto guardado
            hfAniosEdit.Value = ExtraerNumeroDeTexto(pro.strDuracion_pro, "Año");
            hfMesesEdit.Value = ExtraerNumeroDeTexto(pro.strDuracion_pro, "Mes");
            hfSemanasEdit.Value = ExtraerNumeroDeTexto(pro.strDuracion_pro, "Semana");
            hfDiasEdit.Value = ExtraerNumeroDeTexto(pro.strDuracion_pro, "Día"); // Ojo con la tilde, el helper busca match parcial

            // Mostramos el texto completo en el input visible
            txtDuracionDisplayEdit.Text = pro.strDuracion_pro;
            txtFechaEdit.Text = pro.dtFehains_pro.ToString("yyyy-MM-dd");
            hfArchivoActual.Value = pro.strArchivo_pro;
            lblArchivoActual.Text = string.IsNullOrEmpty(pro.strArchivo_pro) ? "Sin archivo" : Path.GetFileName(pro.strArchivo_pro);

            if (ddlConvEdit.Items.FindByValue(pro.fkId_conv) != null)
                ddlConvEdit.SelectedValue = pro.fkId_conv;

            if (ddlGrupoEdit.Items.FindByValue(pro.fkId_gru) != null)
            {
                ddlGrupoEdit.SelectedValue = pro.fkId_gru;
                CargarCoordinadores(ddlCoordinadorEdit, pro.fkId_gru);
                if (ddlCoordinadorEdit.Items.FindByValue(pro.strCoordinador_pro) != null)
                    ddlCoordinadorEdit.SelectedValue = pro.strCoordinador_pro;
            }

            pnlGrilla.Visible = false;
            pnlFormulario.Visible = false;
            pnlEdicion.Visible = true;
            btnNuevo.Visible = false;
            btnRegresar.Visible = true;
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

        // Helper para construir el String final (Ej: "1 Año, 2 Meses")
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

        // Helper para extraer los números de un string existente (Ej: "2 Meses" -> saca el 2)
        private string ExtraerNumeroDeTexto(string textoCompleto, string palabraClave)
        {
            if (string.IsNullOrEmpty(textoCompleto)) return "0";

            // Divide por comas
            var partes = textoCompleto.Split(',');
            foreach (var parte in partes)
            {
                if (parte.Contains(palabraClave) || parte.Contains(palabraClave.ToLower())) // Busca "Año", "Mes", etc
                {
                    // Extrae solo los dígitos
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

            // Intentamos separar "6 Meses" por el espacio
            string[] partes = duracionTexto.Split(' ');

            if (partes.Length >= 2)
            {
                txtNum.Text = partes[0]; // El número (Ej: 6)

                // Buscamos la unidad en el combo
                string unidad = partes[1];
                if (ddlUnidad.Items.FindByValue(unidad) != null)
                    ddlUnidad.SelectedValue = unidad;
            }
            else
            {
                // Si el formato antiguo no coincide (ej: "Medio año"), lo ponemos todo en el número para no perder datos
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
            if (!Directory.Exists(RUTA_PROYECTOS)) Directory.CreateDirectory(RUTA_PROYECTOS);
            string ruta = Path.Combine(RUTA_PROYECTOS, nombre);
            ctl.SaveAs(ruta);
            return ruta;
        }

        private void DescargarArchivo(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            string rutaFisica = (!path.StartsWith("~") && Path.IsPathRooted(path)) ? path : Server.MapPath(path);

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
                Msg("El archivo no existe en la ruta física.", "ww");
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
    }
}