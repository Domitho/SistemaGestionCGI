using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;
using System.IO;

namespace SistemaGestionCGI
{
    public partial class CategorizacionDocentes : System.Web.UI.Page
    {
        // Instancia
        private readonly ManejadorCategorizacionDocentes _manejador = new ManejadorCategorizacionDocentes();

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

            CargarFacultades();
            CargarCarrerasPorFacultad(string.Empty);
            CargarCategorias();
            CargarGrilla();
            MostrarMensajesFlash();
        }

        private void CargarGrilla()
        {
            try
            {
                var lista = _manejador.ObtenerTodos();
                rptDatos.DataSource = lista;
                rptDatos.DataBind();
            }
            catch (Exception ex)
            {
                Msg("Error al cargar listado: " + ex.Message, "ee");
            }
        }

        private void CargarFormulario(string idDocente)
        {
            try
            {
                var docente = _manejador.ObtenerPorId(idDocente);
                if (docente == null) return;

                CargarFacultades();
                CargarCategorias();

                hfIdDocente.Value = docente.strId_doc;
                txtCedula.Text = docente.strCedula_doc;
                txtNombres.Text = docente.strNombres_doc;
                txtApellidos.Text = docente.strApellidos_doc;
                txtCorreo.Text = docente.strCorreo_doc;
                hfCertificadoActual.Value = docente.strCertificado_doc;

                SeleccionarCombo(ddlFacultad, docente.strFacultad_doc);

                CargarCarrerasPorFacultad(docente.strFacultad_doc);
                SeleccionarCombo(ddlCarrera, docente.strCarrera_doc);

                SeleccionarCombo(ddlCategoria, docente.strCategorizacion);

                txtCedula.ReadOnly = true;

                if (docente.dtFechaCategorizacion.HasValue)
                    txtFecha.Text = docente.dtFechaCategorizacion.Value.ToString("yyyy-MM-dd");
                else
                    txtFecha.Text = DateTime.Now.ToString("yyyy-MM-dd");

                pnlGrilla.Visible = false;
                pnlFormulario.Visible = true;
                btnNuevo.Visible = false;
                btnRegresar.Visible = true;
            }
            catch (Exception ex)
            {
                Msg("Error: " + ex.Message, "ee");
            }
        }

        private void SeleccionarCombo(DropDownList ddl, string valor)
        {
            if (ddl.Items.FindByValue(valor) != null)
                ddl.SelectedValue = valor;
            else
                ddl.SelectedIndex = 0;
        }

        private void CargarHistorial(string idDocente)
        {
            try
            {
                hfIdDocenteHistorial.Value = idDocente;

                var historial = _manejador.ObtenerHistorial(idDocente);
                rptHistorial.DataSource = historial;
                rptHistorial.DataBind();

                ScriptManager.RegisterStartupScript(this, GetType(), "OpenModal", "new bootstrap.Modal(document.getElementById('modalHistorial')).show();", true);
            }
            catch (Exception ex)
            {
                Msg("Error al obtener historial: " + ex.Message, "ee");
            }
        }

        protected void btnGenerarReporte_Click(object sender, EventArgs e)
        {
            string idDocente = hfIdDocenteHistorial.Value;

            if (!string.IsNullOrEmpty(idDocente))
            {
                GenerarVistaPrevia(idDocente);
            }
        }

        // ==========================================
        // EVENTOS DE BOTONES
        // ==========================================
        protected void rptDatos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string argumento = e.CommandArgument.ToString();
            string idDocente = e.CommandArgument.ToString();

            switch (e.CommandName)
            {
                case "VerCertificado":
                    VisualizarArchivo(argumento);
                    break;
                case "editar":
                    CargarFormulario(argumento);
                    break;

                case "historial":
                    CargarHistorial(argumento);
                    break;

                case "ReporteIndividual":
                    GenerarVistaPrevia(argumento);
                    break;

                case "eliminar":
                    try
                    {
                        string usuario = Session["UsuarioLogueado"]?.ToString() ?? "SISTEMA";
                        _manejador.DarDeBajaDocente(idDocente, usuario, "Registro enviado a papelera por el usuario.");

                        Redireccionar("Docente enviado a la papelera correctamente.", "ss");
                    }
                    catch (Exception ex) { Msg("Error: " + ex.Message, "ee"); }
                    break;
            }
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            hfIdDocente.Value = "";
            txtCedula.Text = "";
            txtNombres.Text = "";
            txtApellidos.Text = "";
            txtCorreo.Text = "";
            hfCertificadoActual.Value = "";

            CargarFacultades();
            CargarCarrerasPorFacultad(string.Empty);
            CargarCategorias();

            txtFecha.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtCedula.ReadOnly = false;

            pnlGrilla.Visible = false;
            pnlFormulario.Visible = true;
            btnNuevo.Visible = false;
            btnRegresar.Visible = true;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string cedulaInput = txtCedula.Text.Trim();
                string idDocente = hfIdDocente.Value;
                string certificadoActual = hfCertificadoActual.Value;

                if (!_manejador.ValidarCedulaEcuatoriana(cedulaInput))
                {
                    Msg("La cédula ingresada no es válida!", "ee");
                    return;
                }

                if (_manejador.ExisteCedula(cedulaInput, idDocente))
                {
                    Msg("Ya existe un docente registrado con el número de cédula: " + cedulaInput, "ww");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtCedula.Text) || string.IsNullOrWhiteSpace(txtApellidos.Text))
                {
                    Msg("Cédula y Apellidos son obligatorios.", "ww");
                    return;
                }

                if (ddlFacultad.SelectedIndex == 0 || ddlCategoria.SelectedIndex == 0)
                {
                    Msg("Debe seleccionar obligatoriamente la FACULTAD y la CATEGORÍA.", "ww");
                    return;
                }

                var obj = new InvgccCategorizacionDocentes
                {
                    strId_doc = hfIdDocente.Value,
                    strCedula_doc = txtCedula.Text.Trim(),
                    strCorreo_doc = txtCorreo.Text.Trim().ToLower(),
                    strNombres_doc = txtNombres.Text.Trim().ToUpper(),
                    strApellidos_doc = txtApellidos.Text.Trim().ToUpper(),
                    strFacultad_doc = ddlFacultad.SelectedValue,
                    strCarrera_doc = ddlCarrera.SelectedValue,
                    strCategorizacion = ddlCategoria.SelectedValue,
                    dtFechaCategorizacion = DateTime.Parse(txtFecha.Text),

                    strCertificado_doc = hfCertificadoActual.Value
                };

                if (flpCertificado.HasFile)
                {
                    string rutaNueva = GuardarArchivoFisico(flpCertificado, "CERT");

                    if (!string.IsNullOrEmpty(rutaNueva))
                    {
                        obj.strCertificado_doc = rutaNueva;
                    }
                }

                string usuario = Session["UsuarioLogueado"]?.ToString() ?? "SISTEMA";
                string motivoAuto = string.IsNullOrEmpty(hfIdDocente.Value)
                    ? "REGISTRO INICIAL DE DOCENTE"
                    : "ACTUALIZACIÓN DE FICHA / CATEGORÍA";

                _manejador.GuardarDocenteCompleto(obj, usuario, motivoAuto);

                Redireccionar("Docente procesado correctamente.", "ss");
            }
            catch (Exception ex)
            {
                Msg("Error al guardar: " + ex.Message, "ee");
            }
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            Response.Redirect("CategorizacionDocentes.aspx");
        }


        private string GuardarArchivoFisico(FileUpload control, string nombreBase)
        {
            if (control.HasFile)
            {
                string carpeta = Server.MapPath("~/Archivos/DocentesCertificados/");
                if (!Directory.Exists(carpeta))
                {
                    Directory.CreateDirectory(carpeta);
                }

                string extension = Path.GetExtension(control.FileName);
                string nombreArchivo = $"{nombreBase}_{DateTime.Now.Ticks}{extension}";
                string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                control.SaveAs(rutaCompleta);

                return $"~/Archivos/DocentesCertificados/{nombreArchivo}";
            }
            return null;
        }

        private void VisualizarArchivo(string rutaRelativa)
        {
            if (string.IsNullOrEmpty(rutaRelativa))
            {
                Msg("Ruta de archivo no válida.", "ee");
                return;
            }

            string rutaFisica = Server.MapPath(rutaRelativa);

            if (File.Exists(rutaFisica))
            {
                try
                {
                    string nombreArchivo = Path.GetFileName(rutaFisica);

                    Response.Clear();
                    Response.Buffer = true;

                    Response.ContentType = "application/pdf";

                    Response.AddHeader("Content-Disposition", "inline; filename=" + nombreArchivo);

                    Response.TransmitFile(rutaFisica);
                    Response.End();
                }
                catch (Exception ex)
                {
                    if (!(ex is System.Threading.ThreadAbortException))
                    {
                        Msg("Error al abrir el archivo: " + ex.Message, "ee");
                    }
                }
            }
            else
            {
                Msg("El archivo físico no existe en el servidor.", "ww");
            }
        }

        private void GenerarVistaPrevia(string idDocente)
        {
            try
            {
                var docente = _manejador.ObtenerPorId(idDocente);
                var historial = _manejador.ObtenerHistorial(idDocente);

                if (docente == null) return;

                lblRefId.Text = docente.strId_doc;

                lblReporteNombre.Text = $"{docente.strApellidos_doc} {docente.strNombres_doc}";

                lblReporteCedula.Text = docente.strCedula_doc;
                lblReporteFacultad.Text = docente.NombreFacultad;
                lblReporteCarrera.Text = docente.NombreCarrera;

                string cat = string.IsNullOrEmpty(docente.strCategorizacion) ? "SIN ASIGNAR" : docente.strCategorizacion;
                lblReporteCategoria.Text = cat;

                lblReporteFecha.Text = docente.dtFechaCategorizacion.HasValue
                    ? docente.dtFechaCategorizacion.Value.ToString("dd/MM/yyyy")
                    : "-";

                rptReporteHistorial.DataSource = historial;
                rptReporteHistorial.DataBind();

                string script = "var m = new bootstrap.Modal(document.getElementById('modalVistaPrevia')); m.show();";
                ScriptManager.RegisterStartupScript(this, GetType(), "OpenPreview", script, true);
            }
            catch (Exception ex)
            {
                Msg("Error al generar vista previa: " + ex.Message, "ee");
            }
        }

        private void Redireccionar(string msg, string type)
        {
            Session["TempMsg"] = msg;
            Session["TempTipo"] = type;
            Response.Redirect("CategorizacionDocentes.aspx", false);
        }

        private void MostrarMensajesFlash()
        {
            if (Session["TempMsg"] != null)
            {
                Msg(Session["TempMsg"].ToString(), Session["TempTipo"].ToString());
                Session["TempMsg"] = null;
                Session["TempTipo"] = null;
            }
        }

        private void Msg(string msg, string type)
        {
            string cleanMsg = msg.Replace("'", "").Replace("\r\n", " ");
            ScriptManager.RegisterStartupScript(this, GetType(), "toast",
                $"$(function() {{ toastify('{type}', '{cleanMsg}', 'Sistema UTC'); }});", true);
        }

        // 

        protected void btnValidarCedula_Click(object sender, EventArgs e)
        {
            string cedulaInput = txtCedula.Text.Trim();
            string idDocente = hfIdDocente.Value;
            string cedula = txtCedula.Text.Trim();

            if (!_manejador.ValidarCedulaEcuatoriana(cedula))
            {
                Msg("ERROR: La cédula ingresada no es Valida!.", "ee");
                return;
            }

            if (_manejador.ExisteCedula(cedula, idDocente))
            {
                Msg("Ya existe un docente registrado con el número de cédula: " + cedulaInput, "ww");
                return;
            }

            Msg("Cédula válida y disponible para el nuevo registro.", "ss");
        }

        protected void ddlFacultad_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarCarrerasPorFacultad(ddlFacultad.SelectedValue);
        }

        protected void btnVerPapelera_Click(object sender, EventArgs e)
        {
            try
            {
                var listaInactivos = _manejador.ObtenerPapelera();
                rptPapelera.DataSource = listaInactivos;
                rptPapelera.DataBind();

                ScriptManager.RegisterStartupScript(this, GetType(), "OpenPapelera",
                    "new bootstrap.Modal(document.getElementById('modalPapelera')).show();", true);
            }
            catch (Exception ex) { Msg("Error al cargar papelera: " + ex.Message, "ee"); }
        }

        protected void rptPapelera_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "restaurar")
            {
                string idDocente = e.CommandArgument.ToString();
                string usuario = Session["UsuarioLogueado"]?.ToString() ?? "SISTEMA";

                if (_manejador.RestaurarDocente(idDocente, usuario))
                {
                    Redireccionar("Docente restaurado con éxito.", "ss");
                }
                else
                {
                    Msg("No se puede restaurar: Ya existe un docente ACTIVO con la misma cédula.", "ww");
                }
            }
        }

        //

        private void CargarCarrerasPorFacultad(string idFacultad)
        {
            try
            {
                ddlCarrera.Items.Clear();

                if (string.IsNullOrWhiteSpace(idFacultad))
                {
                    ddlCarrera.Items.Insert(0, new ListItem("-- Seleccione la Carrera --", ""));
                    return;
                }

                int id = Convert.ToInt32(idFacultad);

                var lista = _manejador.ObtenerCarrerasPorFacultad(id);

                ddlCarrera.DataSource = lista;
                ddlCarrera.DataTextField = "Nombre";
                ddlCarrera.DataValueField = "IdCarrera";
                ddlCarrera.DataBind();

                ddlCarrera.Items.Insert(0, new ListItem("-- Seleccione la Carrera --", ""));
            }
            catch (Exception ex)
            {
                Msg("Error al cargar carreras: " + ex.Message, "ee");
            }
        }

        //

        private void CargarFacultades()
        {
            try
            {
                var lista = _manejador.ObtenerFacultades();

                ddlFacultad.Items.Clear();
                ddlFacultad.DataSource = lista;
                ddlFacultad.DataTextField = "Nombre";
                ddlFacultad.DataValueField = "IdFacultad";
                ddlFacultad.DataBind();
                ddlFacultad.Items.Insert(0, new ListItem("-- Seleccione --", ""));
            }
            catch (Exception ex)
            {
                Msg("Error al cargar facultades: " + ex.Message, "ee");
            }
        }

        //
        private void CargarCategorias()
        {
            try
            {
                var lista = _manejador.ObtenerCategorias();

                ddlCategoria.Items.Clear();
                ddlCategoria.DataSource = lista;
                ddlCategoria.DataTextField = "Nombre";
                ddlCategoria.DataValueField = "IdCategoria";
                ddlCategoria.DataBind();
                ddlCategoria.Items.Insert(0, new ListItem("-- Seleccione --", ""));
            }
            catch (Exception ex)
            {
                Msg("Error al cargar categorías: " + ex.Message, "ee");
            }
        }

    }
}