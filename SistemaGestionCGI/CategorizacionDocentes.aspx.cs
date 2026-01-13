using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;

namespace SistemaGestionCGI
{
    public partial class CategorizacionDocentes : System.Web.UI.Page
    {
        // Instancia
        private readonly ManejadorCategorizacionDocentes _manejador = new ManejadorCategorizacionDocentes();

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

            CargarGrilla();
            MostrarMensajesFlash();
        }

        // ==========================================
        // MÉTODOS DE CARGA
        // ==========================================
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

                hfIdDocente.Value = docente.strId_doc;
                txtCedula.Text = docente.strCedula_doc;
                txtNombres.Text = docente.strNombres_doc;
                txtApellidos.Text = docente.strApellidos_doc;

                SeleccionarCombo(ddlFacultad, docente.strFacultad_doc);
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
            catch (Exception ex) { Msg("Error: " + ex.Message, "ee"); }
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
            string idDocente = e.CommandArgument.ToString();

            switch (e.CommandName)
            {
                case "editar":
                    CargarFormulario(idDocente);
                    break;

                case "historial":
                    CargarHistorial(idDocente);
                    break;

                case "ReporteIndividual":
                    GenerarVistaPrevia(idDocente);
                    break;

                case "eliminar":
                    try
                    {
                        string usuario = Session["UsuarioLogueado"]?.ToString() ?? "SISTEMA";
                        _manejador.EliminarCategorizacion(idDocente, usuario, "Eliminación directa desde listado");

                        Redireccionar("Se ha quitado la categoría correctamente.", "ss");
                    }
                    catch (Exception ex)
                    {
                        Msg("Error al eliminar: " + ex.Message, "ee");
                    }
                    break;
            }
        }

        protected void btnNuevo_Click(object sender, EventArgs e)
        {
            hfIdDocente.Value = "";
            txtCedula.Text = "";
            txtNombres.Text = "";
            txtApellidos.Text = "";

            ddlFacultad.SelectedIndex = 0;
            ddlCarrera.SelectedIndex = 0;
            ddlCategoria.SelectedIndex = 0;

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
                if (string.IsNullOrWhiteSpace(txtCedula.Text) ||
                    string.IsNullOrWhiteSpace(txtApellidos.Text))
                {
                    Msg("Cédula y Apellidos son obligatorios.", "ww");
                    return;
                }

                if (ddlFacultad.SelectedIndex == 0 || ddlCarrera.SelectedIndex == 0 || ddlCategoria.SelectedIndex == 0)
                {
                    Msg("Debe seleccionar Facultad, Carrera y Categoría.", "ww");
                    return;
                }

                var obj = new InvgccCategorizacionDocentes
                {
                    strId_doc = hfIdDocente.Value,
                    strCedula_doc = txtCedula.Text.Trim(),
                    strNombres_doc = txtNombres.Text.Trim().ToUpper(),
                    strApellidos_doc = txtApellidos.Text.Trim().ToUpper(),

                    strFacultad_doc = ddlFacultad.SelectedValue,
                    strCarrera_doc = ddlCarrera.SelectedValue,
                    strCategorizacion = ddlCategoria.SelectedValue,

                    dtFechaCategorizacion = DateTime.Parse(txtFecha.Text)
                };

                string usuario = Session["UsuarioLogueado"]?.ToString() ?? "SISTEMA";

                string motivoAuto = string.IsNullOrEmpty(hfIdDocente.Value)
                    ? "REGISTRO INICIAL DE DOCENTE"
                    : "ACTUALIZACIÓN DE FICHA / CATEGORÍA";

                _manejador.GuardarDocenteCompleto(obj, usuario, motivoAuto);

                Redireccionar("Docente procesado correctamente.", "ss");
            }
            catch (Exception ex) { Msg("Error al guardar: " + ex.Message, "ee"); }
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            Response.Redirect("CategorizacionDocentes.aspx");
        }

        // ==========================================
        // UTILIDADES Y MENSAJES (TOAST)
        // ==========================================

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
                lblReporteFacultad.Text = docente.strFacultad_doc;
                lblReporteCarrera.Text = docente.strCarrera_doc;

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
    }
}