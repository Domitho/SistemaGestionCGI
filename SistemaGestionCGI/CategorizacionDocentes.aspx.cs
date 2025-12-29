using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;

namespace SistemaGestionCGI
{
    public partial class CategorizacionDocentes : System.Web.UI.Page
    {
        private readonly ManejadorCategorizacionDocentes _manejador = new ManejadorCategorizacionDocentes();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Validar Sesión según tu estándar institucional
            if (Session["Username"] == null)
            {
                Response.Redirect("login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarDatosPagina();

                // MANEJO DE POST-REDIRECT-GET (PRG) para evitar re-envío de formulario
                if (Session["TempMsgCat"] != null)
                {
                    Msg(Session["TempMsgCat"].ToString(), Session["TempTipoCat"].ToString());
                    Session["TempMsgCat"] = null;
                    Session["TempTipoCat"] = null;
                }
            }
        }

        private void CargarDatosPagina()
        {
            try
            {
                CargarGrilla();
                CargarCombos();
                txtFechaCat.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
            catch (Exception ex) { Msg("Error al cargar datos: " + ex.Message, "ee"); }
        }

        private void CargarGrilla()
        {
            // Obtiene datos con INNER JOIN desde BLL para evitar "Invalid Column Name"
            rptCategorias.DataSource = _manejador.ObtenerCategorizacionesActivas();
            rptCategorias.DataBind();
        }

        private void CargarCombos()
        {
            ddlDocente.DataSource = _manejador.ObtenerDocentesCombo();
            ddlDocente.DataTextField = "strApellidos_doc";
            ddlDocente.DataValueField = "strId_doc";
            ddlDocente.DataBind();
            ddlDocente.Items.Insert(0, new ListItem("-- Seleccione Docente --", ""));
        }

        // =============================================
        // REGISTRO DE DOCENTE (MODAL)
        // =============================================

        protected void btnGuardarDocenteRapido_Click(object sender, EventArgs e)
        {
            try
            {
                string cedula = txtCedulaNuevo.Text.Trim();

                // Validación de duplicados antes de insertar
                var existe = _manejador.ObtenerDocentePorSql($"SELECT * FROM INVGCCDOCENTE WHERE strCedula_doc = '{cedula}'");
                if (existe != null)
                {
                    Msg("La cédula ya se encuentra registrada.", "ww");
                    return;
                }

                var nuevoDocente = new InvgccDocente
                {
                    strCedula_doc = txtCedulaNuevo.Text.Trim(),
                    strNombres_doc = txtNombresNuevo.Text.Trim().ToUpper(),
                    strApellidos_doc = txtApellidosNuevo.Text.Trim().ToUpper(),
                    strFacultad_doc = ddlFacultadNuevo.SelectedValue,
                    strCarrera_doc = ddlCarreraNuevo.SelectedValue,
                    strFuncion_doc = ddlFuncionNuevo.SelectedValue,
                    bitActivo_doc = true
                };

                string nuevoId = _manejador.GuardarDocenteSimple(nuevoDocente);

                CargarCombos();
                ddlDocente.SelectedValue = nuevoId;
                LimpiarCamposModal();

                Msg("Docente registrado correctamente.", "ss");
                ScriptManager.RegisterStartupScript(this, GetType(), "hideModal", "bootstrap.Modal.getInstance(document.getElementById('modalNuevoDocente')).hide();", true);
            }
            catch (Exception ex) { Msg("Error al registrar docente: " + ex.Message, "ee"); }
        }

        // =============================================
        // CRUD CATEGORIZACIÓN
        // =============================================

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                var cat = new InvgccCategoriaDocentes
                {
                    fkId_doc = ddlDocente.SelectedValue,
                    dtFecha_cat = Convert.ToDateTime(txtFechaCat.Text),
                    strCategorizacion = ddlCategoria.SelectedValue
                };

                if (string.IsNullOrEmpty(hfIdCat.Value))
                {
                    _manejador.GuardarCategorizacion(cat); // Usa objeto anónimo en BLL para evitar errores SQL
                    Redireccionar("Categorización guardada con éxito.", "ss");
                }
                else
                {
                    cat.strId_cat = hfIdCat.Value;
                    _manejador.ActualizarCategorizacion(cat);
                    Redireccionar("Categorización actualizada.", "ss");
                }
            }
            catch (Exception ex) { Msg("Error al procesar: " + ex.Message, "ee"); }
        }

        protected void rptCategorias_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();
            if (e.CommandName == "Editar")
            {
                CargarEdicion(id);
            }
            else if (e.CommandName == "Eliminar")
            {
                if (_manejador.EliminarCategorizacion(id))
                    Redireccionar("Registro eliminado.", "ss");
            }
        }

        private void CargarEdicion(string id)
        {
            var cat = _manejador.ObtenerCategoriaPorId(id);
            if (cat != null)
            {
                hfIdCat.Value = cat.strId_cat;
                ddlDocente.SelectedValue = cat.fkId_doc;
                ddlCategoria.SelectedValue = cat.strCategorizacion;
                txtFechaCat.Text = cat.dtFecha_cat.ToString("yyyy-MM-dd");

                lblTituloFormulario.Text = "Editando Categorización: " + cat.strId_cat;
                CambiarVista(true);
            }
        }

        // =============================================
        // UTILIDADES Y NAVEGACIÓN
        // =============================================

        protected void btnNuevoRegistro_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            lblTituloFormulario.Text = "Nueva Categorización";
            CambiarVista(true);
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("CategorizacionDocentes.aspx");
        }

        private void CambiarVista(bool mostrarFormulario)
        {
            pnlFormulario.Visible = mostrarFormulario;
            pnlGrilla.Visible = !mostrarFormulario;
            headerListado.Visible = !mostrarFormulario;
        }

        private void LimpiarCampos()
        {
            hfIdCat.Value = "";
            ddlDocente.SelectedIndex = 0;
            ddlCategoria.SelectedIndex = 0;
            txtFechaCat.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void LimpiarCamposModal()
        {
            txtCedulaNuevo.Text = ""; txtNombresNuevo.Text = ""; txtApellidosNuevo.Text = "";
            ddlFacultadNuevo.SelectedIndex = 0;
            ddlCarreraNuevo.SelectedIndex = 0;
            ddlFuncionNuevo.SelectedIndex = 0;
        }

        private void Redireccionar(string msg, string type)
        {
            Session["TempMsgCat"] = msg;
            Session["TempTipoCat"] = type;
            Response.Redirect("CategorizacionDocentes.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void Msg(string msg, string type)
        {
            if (string.IsNullOrEmpty(msg)) return;
            string cleanMsg = msg.Replace("'", "").Replace("\r", "").Replace("\n", " ");
            string script = $"$(function() {{ toastify('{type.ToLower()}', '{cleanMsg}', 'SISTEMA INVESTIGACIÓN'); }});";
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", script, true);
        }
    }
}