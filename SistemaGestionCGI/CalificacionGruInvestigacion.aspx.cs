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
        // 1. Instancias y Constantes
        private readonly ManejadorCalificacionGrupo _manejador = new ManejadorCalificacionGrupo();
        private const string RUTA_CALIFICACIONES = @"C:\UTC\CALIFICACIONES\";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            // Seguridad
            if (Session["UsuarioLogueado"] == null)
            {
                Response.Redirect("Login.aspx", true);
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
        // CARGA DE DATOS Y COMBOS
        // =============================================

        private void CargarGrilla()
        {
            try
            {
                int anio = (ddlFiltroAnio.SelectedIndex > 0 && int.TryParse(ddlFiltroAnio.SelectedValue, out int a)) ? a : 0;
                rptCalificaciones.DataSource = _manejador.ObtenerCalificaciones(anio);
                rptCalificaciones.DataBind();
            }
            catch (Exception ex) { Msg("Error al cargar calificaciones: " + ex.Message, "ee"); }
        }

        private void CargarCombos()
        {
            try
            {
                // Filtro Años
                ddlFiltroAnio.Items.Clear();
                ddlFiltroAnio.Items.Add(new ListItem("Todos los Años", "0"));
                foreach (int y in _manejador.ObtenerAniosDisponibles())
                {
                    ddlFiltroAnio.Items.Add(new ListItem(y.ToString(), y.ToString()));
                }

                // Combo Métricas (Año actual +/- 2)
                int currentYear = DateTime.Now.Year;
                ddlAnioMetricas.Items.Clear();
                for (int i = currentYear - 2; i <= currentYear + 2; i++)
                {
                    ddlAnioMetricas.Items.Add(new ListItem(i.ToString(), i.ToString()));
                }
                ddlAnioMetricas.SelectedValue = currentYear.ToString();
            }
            catch (Exception ex) { Msg("Error cargando listas: " + ex.Message, "ee"); }
        }

        private void CargarGruposPendientes()
        {
            try
            {
                int anioSeleccionado = int.TryParse(ddlAnioMetricaSeleccion.SelectedValue, out int a) ? a : DateTime.Now.Year;
                var grupos = _manejador.ObtenerGruposParaCombo(anioSeleccionado);

                ddlGrupoAdd.DataSource = grupos;
                ddlGrupoAdd.DataTextField = "strNombre_gru";
                ddlGrupoAdd.DataValueField = "strId_gru";
                ddlGrupoAdd.DataBind();

                if (grupos.Count > 0)
                    ddlGrupoAdd.Items.Insert(0, new ListItem("-- Seleccione Grupo --", ""));
                else
                    ddlGrupoAdd.Items.Insert(0, new ListItem("-- Todos los grupos ya fueron calificados este año --", ""));
            }
            catch (Exception ex) { Msg("Error al cargar grupos pendientes: " + ex.Message, "ee"); }
        }

        // =============================================
        // EVENTOS DE INTERFAZ
        // =============================================

        protected void ddlFiltroAnio_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarGrilla();
        }

        protected void ddlAnioMetricaSeleccion_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarGruposPendientes();
            ActualizarMetricaVisual();
        }

        protected void btnNuevaCalif_Click(object sender, EventArgs e)
        {
            CambiarVista(Vista.Formulario);

            // Reset Formulario
            txtFechaAdd.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtPuntajeAdd.Text = "";
            txtReconocimientoAdd.Text = "";

            // Cargar Años Métricas
            ddlAnioMetricaSeleccion.DataSource = _manejador.ObtenerAniosConMetricasConfiguradas();
            ddlAnioMetricaSeleccion.DataBind();

            string anioActual = DateTime.Now.Year.ToString();
            if (ddlAnioMetricaSeleccion.Items.FindByValue(anioActual) != null)
                ddlAnioMetricaSeleccion.SelectedValue = anioActual;

            CargarGruposPendientes();
            ActualizarMetricaVisual();
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            Response.Redirect("CalificacionGruInvestigacion.aspx");
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

        // =============================================
        // CRUD CALIFICACIONES
        // =============================================

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ddlGrupoAdd.SelectedValue)) { Msg("Seleccione un grupo.", "ww"); return; }
                if (string.IsNullOrEmpty(txtPuntajeAdd.Text)) { Msg("Ingrese el puntaje.", "ww"); return; }
                if (string.IsNullOrEmpty(txtFechaAdd.Text)) { Msg("Ingrese la fecha.", "ww"); return; }

                if (!flpArchivoAdd.HasFile) { Msg("Debe subir el informe PDF.", "ww"); return; }
                if (Path.GetExtension(flpArchivoAdd.FileName).ToLower() != ".pdf") { Msg("Solo se permiten archivos PDF.", "ww"); return; }

                int puntaje = int.Parse(txtPuntajeAdd.Text);
                int anioM = int.Parse(ddlAnioMetricaSeleccion.SelectedValue);
                int minConsolidado = _manejador.ObtenerMinimoConsolidado(anioM);

                var obj = new InvgccCalificacionGrupo
                {
                    fkId_gru = ddlGrupoAdd.SelectedValue,
                    dtFecha_valo = DateTime.Parse(txtFechaAdd.Text),
                    intPuntaje_valo = puntaje,
                    strReconocimiento_valo = txtReconocimientoAdd.Text.Trim(),
                    intAnioMetrica = anioM,
                    strCategoria_valo = (puntaje >= minConsolidado) ? "CONSOLIDADO" : "EMERGENTE",
                    strInforme_valo = GuardarArchivoFisico(flpArchivoAdd, $"VAL_{DateTime.Now.Ticks}.pdf")
                };

                _manejador.GuardarCalificacion(obj);
                Redireccionar($"Calificación registrada. El grupo ahora es <b>{obj.strCategoria_valo}</b>.", "ss");
            }
            catch (Exception ex) { Msg("Error al guardar: " + ex.Message, "ee"); }
        }

        protected void rptCalificaciones_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();

            if (e.CommandName == "Eliminar")
            {
                try
                {
                    _manejador.EliminarCalificacion(id);
                    Redireccionar("Calificación eliminada correctamente.", "ss");
                }
                catch (Exception ex) { Msg("Error al eliminar: " + ex.Message, "ee"); }
            }
            else if (e.CommandName == "Ver")
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalPDF", $"VerPDF('{id}');", true);
            }
        }

        // =============================================
        // GESTIÓN DE MÉTRICAS (CONFIGURACIÓN)
        // =============================================

        protected void btnGuardarMetricas_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtMinConsolidado.Text))
                {
                    Msg("Ingrese el puntaje mínimo.", "ww");
                    return;
                }

                var m = new InvgccMetricas
                {
                    anio = int.Parse(ddlAnioMetricas.SelectedValue),
                    minConsolidado = int.Parse(txtMinConsolidado.Text)
                };

                _manejador.GuardarMetrica(m);
                Redireccionar($"Métrica del año {m.anio} actualizada correctamente.", "ss");
            }
            catch (Exception ex) { Msg("Error al guardar métrica: " + ex.Message, "ee"); }
        }

        // =============================================
        // UTILIDADES Y AYUDAS
        // =============================================

        private enum Vista { Lista, Formulario }

        private void CambiarVista(Vista vista)
        {
            pnlGrilla.Visible = vista == Vista.Lista;
            pnlFiltros.Visible = vista == Vista.Lista;
            headerCalificacion.Visible = vista == Vista.Lista;
            pnlFormulario.Visible = vista == Vista.Formulario;
        }

        private string GuardarArchivoFisico(FileUpload control, string nombre)
        {
            if (!Directory.Exists(RUTA_CALIFICACIONES)) Directory.CreateDirectory(RUTA_CALIFICACIONES);
            string ruta = Path.Combine(RUTA_CALIFICACIONES, nombre);
            control.SaveAs(ruta);
            return ruta;
        }

        private void Redireccionar(string msg, string type)
        {
            Session["TempMsg"] = msg;
            Session["TempTipo"] = type;
            Response.Redirect("CalificacionGruInvestigacion.aspx", false);
        }

        private void Msg(string msg, string type)
        {
            if (string.IsNullOrEmpty(msg)) return;
            string cleanMsg = msg.Replace("'", "\\'").Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("\\", "\\\\");
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"$(function() {{ toastify('{type}', '{cleanMsg}', 'Sistema'); }});", true);
        }
    }
}