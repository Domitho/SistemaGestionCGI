using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;

namespace SistemaGestionCGI
{
    public partial class CalificacionGruInvestigacion : System.Web.UI.Page
    {
        private readonly ManejadorCalificacionGrupo _manejador = new ManejadorCalificacionGrupo();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UsuarioLogueado"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                CargarCombos();
                CargarGrilla();

                if (Session["TempMsg"] != null)
                {
                    Msg(Session["TempMsg"].ToString(), Session["TempTipo"].ToString());
                    Session["TempMsg"] = null;
                    Session["TempTipo"] = null;
                }
            }
        }

        private void CargarGrilla()
        {
            try
            {
                int anio = 0;
                if (ddlFiltroAnio.SelectedIndex > 0)
                    int.TryParse(ddlFiltroAnio.SelectedValue, out anio);

                var lista = _manejador.ObtenerCalificaciones(anio);
                rptCalificaciones.DataSource = lista;
                rptCalificaciones.DataBind();
            }
            catch (Exception ex)
            {
                Msg("Error al cargar calificaciones: " + ex.Message, "ee");
            }
        }

        private void CargarCombos()
        {
            try
            {
                var grupos = _manejador.ObtenerGruposParaCombo();
                ddlGrupoAdd.DataSource = grupos;
                ddlGrupoAdd.DataTextField = "strNombre_gru";
                ddlGrupoAdd.DataValueField = "strId_gru";
                ddlGrupoAdd.DataBind();
                ddlGrupoAdd.Items.Insert(0, new ListItem("-- Seleccione Grupo --", ""));

                var aniosDisponibles = _manejador.ObtenerAniosDisponibles();
                ddlFiltroAnio.Items.Clear();
                ddlFiltroAnio.Items.Add(new ListItem("Todos los Años", "0"));
                foreach (int y in aniosDisponibles)
                {
                    ddlFiltroAnio.Items.Add(new ListItem(y.ToString(), y.ToString()));
                }

                int currentYear = DateTime.Now.Year;
                ddlAnioMetricas.Items.Clear();
                for (int i = currentYear - 2; i <= currentYear + 2; i++)
                {
                    ddlAnioMetricas.Items.Add(new ListItem(i.ToString(), i.ToString()));
                }
                ddlAnioMetricas.SelectedValue = currentYear.ToString();
            }
            catch (Exception ex)
            {
                Msg("Error cargando listas: " + ex.Message, "ee");
            }
        }

        protected void ddlFiltroAnio_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarGrilla();
        }

        protected void btnNuevaCalif_Click(object sender, EventArgs e)
        {
            pnlGrilla.Visible = false;
            pnlFiltros.Visible = false;
            pnlFormulario.Visible = true;
            headerCalificacion.Visible = false;

            // Limpiar campos
            ddlGrupoAdd.SelectedIndex = 0;
            txtFechaAdd.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtPuntajeAdd.Text = "";
            txtReconocimientoAdd.Text = "";

            // 1. Cargar Años de Métricas Disponibles
            var aniosMetricas = _manejador.ObtenerAniosConMetricasConfiguradas();
            ddlAnioMetricaSeleccion.DataSource = aniosMetricas;
            ddlAnioMetricaSeleccion.DataBind();

            // 2. Intentar seleccionar el año actual por defecto
            string anioActual = DateTime.Now.Year.ToString();
            if (ddlAnioMetricaSeleccion.Items.FindByValue(anioActual) != null)
                ddlAnioMetricaSeleccion.SelectedValue = anioActual;

            // 3. Mostrar la regla visualmente
            ActualizarMetricaVisual();
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Validaciones
                if (string.IsNullOrEmpty(ddlGrupoAdd.SelectedValue)) { Msg("Seleccione un grupo.", "ww"); return; }
                if (string.IsNullOrEmpty(txtPuntajeAdd.Text)) { Msg("Ingrese el puntaje.", "ww"); return; }
                if (string.IsNullOrEmpty(txtFechaAdd.Text)) { Msg("Ingrese la fecha.", "ww"); return; }
                if (!flpArchivoAdd.HasFile) { Msg("Debe subir el informe PDF.", "ww"); return; }

                // 2. Validar Archivo
                string ext = Path.GetExtension(flpArchivoAdd.FileName).ToLower();
                if (ext != ".pdf") { Msg("Solo se permiten archivos PDF.", "ww"); return; }

                // 3. Preparar Objeto
                InvgccCalificacionGrupo obj = new InvgccCalificacionGrupo();
                obj.fkId_gru = ddlGrupoAdd.SelectedValue;
                obj.dtFecha_valo = DateTime.Parse(txtFechaAdd.Text);
                obj.intPuntaje_valo = int.Parse(txtPuntajeAdd.Text);
                obj.strReconocimiento_valo = txtReconocimientoAdd.Text.Trim();

                int anioM = int.Parse(ddlAnioMetricaSeleccion.SelectedValue);
                obj.intAnioMetrica = anioM;

                // 4. LÓGICA DE NEGOCIO: Calcular Categoría
                // Obtenemos el mínimo requerido para ese año desde la BDD
                int minConsolidado = _manejador.ObtenerMinimoConsolidado(anioM);

                if (obj.intPuntaje_valo >= minConsolidado)
                    obj.strCategoria_valo = "CONSOLIDADO";
                else
                    obj.strCategoria_valo = "EMERGENTE";

                // 5. Guardar Archivo
                string nombreArchivo = $"VAL_{DateTime.Now.Ticks}.pdf";
                obj.strInforme_valo = GuardarArchivo(flpArchivoAdd, "Valoraciones", nombreArchivo);

                // 6. Guardar en BDD
                _manejador.GuardarCalificacion(obj);

                // 7. PRG
                SetFlashMessage($"Calificación registrada. El grupo ahora es <b>{obj.strCategoria_valo}</b>.", "ss");
                Response.Redirect("CalificacionGruInvestigacion.aspx", false);
            }
            catch (Exception ex)
            {
                Msg("Error al guardar: " + ex.Message, "ee");
            }
        }

        protected void rptCalificaciones_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string arg = e.CommandArgument.ToString();

            if (e.CommandName == "Eliminar")
            {
                try
                {
                    int id = int.Parse(arg);
                    _manejador.EliminarCalificacion(id);
                    SetFlashMessage("Calificación eliminada correctamente.", "ss");
                    Response.Redirect("CalificacionGruInvestigacion.aspx", false);
                }
                catch (Exception ex)
                {
                    Msg("Error al eliminar: " + ex.Message, "ee");
                }
            }
            else if (e.CommandName == "Ver")
            {
                string url = ResolveUrl(arg);

                ScriptManager.RegisterStartupScript(this, GetType(), "OpenPDF", $"VerPDF('{url}');", true);
            }
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            Response.Redirect("CalificacionGruInvestigacion.aspx");
        }

        protected void ddlAnioMetricaSeleccion_SelectedIndexChanged(object sender, EventArgs e)
        {
            ActualizarMetricaVisual();
        }

        private void ActualizarMetricaVisual()
        {
            if (int.TryParse(ddlAnioMetricaSeleccion.SelectedValue, out int anio))
            {
                int min = _manejador.ObtenerMinimoConsolidado(anio);
                lblReglaMetrica.Text = $"Según la normativa del <b>{anio}</b>, se requiere un mínimo de <b>{min} puntos</b> para ser <span class='badge bg-success'>CONSOLIDADO</span>. Menos de eso será <span class='badge bg-warning text-dark'>EMERGENTE</span>.";
            }
            else
            {
                lblReglaMetrica.Text = "No hay métricas configuradas para este año. Se usará el estándar (70 pts).";
            }
        }

        protected void btnGuardarMetricas_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMinConsolidado.Text))
                {
                    Msg("Ingrese el puntaje mínimo.", "ww");
                    return;
                }

                InvgccMetricas m = new InvgccMetricas();
                m.anio = int.Parse(ddlAnioMetricas.SelectedValue);
                m.minConsolidado = int.Parse(txtMinConsolidado.Text);

                _manejador.GuardarMetrica(m);

                SetFlashMessage($"Métrica del año {m.anio} actualizada correctamente.", "ss");
                Response.Redirect("CalificacionGruInvestigacion.aspx", false);
            }
            catch (Exception ex)
            {
                Msg("Error al guardar métrica: " + ex.Message, "ee");
            }
        }

        private string GuardarArchivo(FileUpload control, string carpetaIgnorada, string nombre)
        {
            string carpetaFisica = @"C:\UTC\CALIFICACIONES\";
            if (!Directory.Exists(carpetaFisica))
                Directory.CreateDirectory(carpetaFisica);
            string rutaCompleta = Path.Combine(carpetaFisica, nombre);
            control.SaveAs(rutaCompleta);

            return rutaCompleta;
        }

        private void SetFlashMessage(string msg, string type)
        {
            Session["TempMsg"] = msg;
            Session["TempTipo"] = type;
        }

        private void Msg(string msg, string type)
        {
            string cleanMsg = msg.Replace("'", "\\'");
            // Usamos Toastify que es tu librería actual
            string script = $"$(function() {{ toastify('{type}', '{cleanMsg}', 'Sistema'); }});";
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", script, true);
        }

    }
}