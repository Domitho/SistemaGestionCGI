using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;

namespace SistemaGestionCGI
{
    public partial class ProyectosAprobadosCoordinadores : System.Web.UI.Page
    {
        private readonly ManejadorEjecucionProyectos _manejador = new ManejadorEjecucionProyectos();
        private const string RUTA_VIRTUAL_ARCHIVOS = "~/RepositorioUTC/EjecucionInformes/";

        private string CedulaUsuario => Session["CedulaUsuario"]?.ToString();
        private string NombreUsuario => Session["UsuarioLogueado"]?.ToString() ?? "DOCENTE UTC";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioLogueado"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (string.IsNullOrEmpty(CedulaUsuario))
            {
                pnlListadoTarjetas.Visible = false;
                Msg("Acceso Restringido: Su usuario no tiene un perfil de Docente/Coordinador asociado.", "ee");
                return;
            }

            if (!IsPostBack)
            {
                try
                {
                    CargarMisProyectos();

                    string idTeamRedirect = Request.QueryString["idTeam"];
                    if (!string.IsNullOrEmpty(idTeamRedirect) && int.TryParse(idTeamRedirect, out int idTeam))
                    {
                        CargarEquipo(idTeam);
                    }

                    if (Session["TempMsg"] != null)
                    {
                        Msg(Session["TempMsg"].ToString(), Session["TempTipo"].ToString());
                        Session["TempMsg"] = null;
                        Session["TempTipo"] = null;
                    }
                }
                catch (Exception ex)
                {
                    Msg("Error al iniciar el módulo: " + ex.Message, "ee");
                }
            }
        }

        // ==========================================
        // SEGURIDAD: VERIFICAR PROPIEDAD
        // ==========================================
        private bool EsProyectoDelCoordinador(int idEjecucion)
        {
            var proyecto = _manejador.ObtenerEjecucionPorId(idEjecucion);
            if (proyecto == null) return false;

            var misProyectos = _manejador.ObtenerEjecuciones(CedulaUsuario);
            return misProyectos.Exists(x => x.strId_ejec == idEjecucion);
        }

        // ==========================================
        // 2. DASHBOARD
        // ==========================================
        private void CargarMisProyectos()
        {
            try
            {
                var lista = _manejador.ObtenerEjecuciones(CedulaUsuario);
                rptProyectosCoordinador.DataSource = lista;
                rptProyectosCoordinador.DataBind();
            }
            catch (Exception ex) { Msg("Error al cargar sus proyectos: " + ex.Message, "ee"); }
        }

        protected void rptProyectosCoordinador_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                dynamic data = e.Item.DataItem;
                Literal lit = (Literal)e.Item.FindControl("litAlertaPlazo");

                DateTime? fin = null;
                try { fin = data.dtFechafin_ejec; } catch { }

                if (fin.HasValue && fin.Value.Year > 1900 && data.strEstado_ejec == "EN EJECUCIÓN")
                {
                    TimeSpan restante = fin.Value - DateTime.Now;
                    if (restante.TotalDays < 0)
                        lit.Text = "<span class='badge bg-danger'><i class='fa-solid fa-clock'></i> Plazo Vencido</span>";
                    else if (restante.TotalDays < 30)
                        lit.Text = $"<span class='badge bg-warning text-dark'><i class='fa-solid fa-hourglass-half'></i> Cierra en {Math.Ceiling(restante.TotalDays)} días</span>";
                }
            }
        }

        protected void rptProyectosCoordinador_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();

            switch (e.CommandName)
            {
                case "Equipo":
                    CargarEquipo(int.Parse(id));
                    break;

                case "Informes":
                    CargarInformes(int.Parse(id));
                    break;
            }
        }

        // ==========================================
        // 3. GESTIÓN DE EQUIPO (SOLO LECTURA)
        // ==========================================
        private void CargarEquipo(int idEjecucion)
        {
            hfIdEjecucionEquipo.Value = idEjecucion.ToString();

            pnlListadoTarjetas.Visible = false;
            pnlEquipoListado.Visible = true;
            pnlFormularioMiembro.Visible = false;

            // BLOQUEO: Coordinador no agrega miembros
            btnAbrirFormMiembro.Visible = false;

            var proy = _manejador.ObtenerEjecucionPorId(idEjecucion);
            ViewState["EsProyectoFinalizado"] = (proy.strEstado_ejec == "FINALIZADO");

            RefrescarTablaMiembros();
        }

        private void RefrescarTablaMiembros()
        {
            if (int.TryParse(hfIdEjecucionEquipo.Value, out int id))
            {
                rptMiembros.DataSource = _manejador.ObtenerMiembros(id);
                rptMiembros.DataBind();
            }
        }

        protected void btnVolverTarjeta_Click(object sender, EventArgs e)
        {
            pnlEquipoListado.Visible = false;
            pnlFormularioMiembro.Visible = false;
            pnlListadoTarjetas.Visible = true;
            CargarMisProyectos();
        }

        // MÉTODOS DE EDICIÓN BLOQUEADOS / NO VISIBLES
        protected void btnAbrirFormMiembro_Click(object sender, EventArgs e) { }
        protected void btnGuardarMiembro_Click(object sender, EventArgs e) { Msg("Acceso Denegado.", "ee"); }
        protected void btnCancelarMiembro_Click(object sender, EventArgs e)
        {
            pnlFormularioMiembro.Visible = false;
            pnlEquipoListado.Visible = true;
        }

        protected void btnGuardarCierre_Click(object sender, EventArgs e)
        {
            Msg("Acceso Denegado: No tiene permisos para subir el cierre.", "ee");
        }

        protected void btnInformeCierre_Click(object sender, EventArgs e) { }

        protected void btnInformeFinal_Click(object sender, EventArgs e) { }

        protected void btnGuardarFinal_Click(object sender, EventArgs e)
        {
            Msg("Acceso Denegado: No tiene permisos para finalizar el proyecto.", "ee");
        }

        protected void rptMiembros_ItemCommand(object source, RepeaterCommandEventArgs e) { }

        protected void rptMiembros_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // Ocultar botones de acción en cada fila
                var btnEdit = e.Item.FindControl("btnEditarM");
                if (btnEdit != null) btnEdit.Visible = false;

                var btnToggle = e.Item.FindControl("btnToggleEstado");
                if (btnToggle != null) btnToggle.Visible = false;
            }
        }

        // ==========================================
        // 4. GESTIÓN DE INFORMES
        // ==========================================
        private void CargarInformes(int idEjecucion)
        {
            hfIdEjecucionInforme.Value = idEjecucion.ToString();

            // 1. Cargar lista de informes de avance
            rptInformes.DataSource = _manejador.ObtenerInformes(idEjecucion);
            rptInformes.DataBind();

            // 2. Si está finalizado, bloquear subidas de avances
            BloquearGestionInformes(idEjecucion);

            // 3. Configurar visor de documentos finales (Ver PDF)
            ConfigurarVisorCierre(idEjecucion);

            // 4. Mostrar Modal
            ScriptManager.RegisterStartupScript(this, GetType(), "OpenInf", "AbrirModalInformes();", true);
        }

        private void ConfigurarVisorCierre(int idEjecucion)
        {
            var proy = _manejador.ObtenerEjecucionPorId(idEjecucion);
            pnlFaseCierre.Visible = true;

            // ===============================================
            // 1. CONFIGURACIÓN INFORME DE CIERRE
            // ===============================================
            bool hayCierre = !string.IsNullOrEmpty(proy.strInforme_Cierre);

            if (hayCierre)
            {
                // Configuramos el enlace directo
                lnkVerCierre.Enabled = true;
                lnkVerCierre.NavigateUrl = ResolveUrl(proy.strInforme_Cierre);

                // Estilos de "Activo"
                lnkVerCierre.CssClass = "btn btn-white w-100 border p-3 text-start shadow-sm hover-lift border-warning text-decoration-none";
                iconCierre.Attributes["class"] = "fa-solid fa-file-pdf fs-3 text-danger";

                lblEstadoCierre.InnerText = "Clic para descargar";
                lblEstadoCierre.Attributes["class"] = "text-success fw-bold";
            }
            else
            {
                // Estado "Desactivado"
                lnkVerCierre.Enabled = false;
                lnkVerCierre.NavigateUrl = "#";

                lnkVerCierre.CssClass = "btn btn-light w-100 border p-3 text-start opacity-50 text-decoration-none";
                iconCierre.Attributes["class"] = "fa-solid fa-ban fs-3 text-muted";

                lblEstadoCierre.InnerText = "Pendiente de Admin";
                lblEstadoCierre.Attributes["class"] = "text-muted";
            }

            // ===============================================
            // 2. CONFIGURACIÓN INFORME FINAL
            // ===============================================
            bool hayFinal = !string.IsNullOrEmpty(proy.strInforme_Final);

            if (hayFinal)
            {
                // Configuramos el enlace directo
                lnkVerFinal.Enabled = true;
                lnkVerFinal.NavigateUrl = ResolveUrl(proy.strInforme_Final);

                // Estilos de "Activo"
                lnkVerFinal.CssClass = "btn btn-white w-100 border p-3 text-start shadow-sm hover-lift border-success text-decoration-none";
                iconFinal.Attributes["class"] = "fa-solid fa-award fs-3 text-success";

                lblEstadoFinal.InnerText = "Clic para descargar";
                lblEstadoFinal.Attributes["class"] = "text-success fw-bold";
            }
            else
            {
                // Estado "Desactivado"
                lnkVerFinal.Enabled = false;
                lnkVerFinal.NavigateUrl = "#";

                lnkVerFinal.CssClass = "btn btn-light w-100 border p-3 text-start opacity-50 text-decoration-none";
                iconFinal.Attributes["class"] = "fa-solid fa-lock fs-3 text-muted";

                lblEstadoFinal.InnerText = "No finalizado";
                lblEstadoFinal.Attributes["class"] = "text-muted";
            }
        }

        protected void btnGuardarInforme_Click(object sender, EventArgs e)
        {
            try
            {
                if (!flpArchivoInf.HasFile) { Msg("Seleccione un archivo.", "ww"); return; }
                if (!int.TryParse(hfIdEjecucionInforme.Value, out int idEjec)) return;

                // VALIDACIÓN DE SEGURIDAD
                if (!EsProyectoDelCoordinador(idEjec))
                {
                    Msg("ALERTA: Proyecto no autorizado.", "ee");
                    return;
                }

                var proy = _manejador.ObtenerEjecucionPorId(idEjec);
                if (proy.strEstado_ejec == "FINALIZADO")
                {
                    Msg("El proyecto está finalizado.", "ee");
                    return;
                }

                string nombreArchivo = $"INF_{DateTime.Now.Ticks}{Path.GetExtension(flpArchivoInf.FileName)}";
                string ruta = GuardarArchivoFisico(flpArchivoInf, nombreArchivo);

                var inf = new InvgccEjecucionInformes
                {
                    fkId_ejec = idEjec,
                    strNombrePeriodo = txtNombrePeriodoInf.Text.Trim(),
                    strArchivo_path = ruta,
                    dtFechaSubida = DateTime.Now
                };

                if (string.IsNullOrEmpty(hfIdInformeEdit.Value))
                    _manejador.GuardarInforme(inf);
                else
                {
                    inf.strId_informe = int.Parse(hfIdInformeEdit.Value);
                    _manejador.ActualizarInforme(inf);
                }

                CargarInformes(idEjec);
                Msg("Informe cargado exitosamente.", "ss");
                ScriptManager.RegisterStartupScript(this, GetType(), "CloseSub", "CerrarSubModalUpload();", true);
            }
            catch (Exception ex) { Msg("Error: " + ex.Message, "ee"); }
        }

        protected void rptInformes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int idInf = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "EditarInforme")
            {
                var inf = _manejador.ObtenerInformePorId(idInf);
                hfIdInformeEdit.Value = inf.strId_informe.ToString();
                txtNombrePeriodoInf.Text = inf.strNombrePeriodo;
                lblTituloModalInforme.InnerText = "Editar Informe";
                ScriptManager.RegisterStartupScript(this, GetType(), "OpenSub", "AbrirSubModalUpload();", true);
            }
            else if (e.CommandName == "EliminarInforme")
            {
                try
                {
                    _manejador.EliminarInforme(idInf);
                    CargarInformes(int.Parse(hfIdEjecucionInforme.Value));
                    Msg("Informe eliminado.", "ss");
                }
                catch (Exception ex) { Msg(ex.Message, "ee"); }
            }
        }

        // ==========================================
        // 5. ACCIONES DE VISUALIZACIÓN (SOLO DESCARGA)
        // ==========================================
        protected void btnVerCierre_Click(object sender, EventArgs e)
        {
            if (int.TryParse(hfIdEjecucionInforme.Value, out int id))
            {
                var proy = _manejador.ObtenerEjecucionPorId(id);
                if (!string.IsNullOrEmpty(proy.strInforme_Cierre))
                {
                    string url = ResolveUrl(proy.strInforme_Cierre);
                    ScriptManager.RegisterStartupScript(this, GetType(), "OpenDocCierre", $"window.open('{url}', '_blank');", true);
                }
                else { Msg("Documento no disponible.", "ww"); }
            }
        }

        protected void btnVerFinal_Click(object sender, EventArgs e)
        {
            if (int.TryParse(hfIdEjecucionInforme.Value, out int id))
            {
                var proy = _manejador.ObtenerEjecucionPorId(id);
                if (!string.IsNullOrEmpty(proy.strInforme_Final))
                {
                    string url = ResolveUrl(proy.strInforme_Final);
                    ScriptManager.RegisterStartupScript(this, GetType(), "OpenDocFinal", $"window.open('{url}', '_blank');", true);
                }
                else { Msg("Documento no disponible.", "ww"); }
            }
        }

        // ==========================================
        // 6. UTILIDADES
        // ==========================================
        private void BloquearGestionInformes(int idEjecucion)
        {
            var proy = _manejador.ObtenerEjecucionPorId(idEjecucion);
            bool cerrado = (proy.strEstado_ejec == "FINALIZADO" || proy.strEstado_ejec == "CIERRE APROBADO");

            btnAbrirGenerador.Visible = !cerrado;
            btnSubirEscaneado.Visible = !cerrado;

            foreach (RepeaterItem item in rptInformes.Items)
            {
                var btnEdit = item.FindControl("btnEditarInf");
                var btnDel = item.FindControl("btnEliminarInf");
                if (btnEdit != null) btnEdit.Visible = !cerrado;
                if (btnDel != null) btnDel.Visible = !cerrado;
            }
        }

        private string GuardarArchivoFisico(FileUpload control, string nombreArchivo)
        {
            string rutaFisicaCarpeta = Server.MapPath(RUTA_VIRTUAL_ARCHIVOS);
            if (!Directory.Exists(rutaFisicaCarpeta)) Directory.CreateDirectory(rutaFisicaCarpeta);
            string rutaCompleta = Path.Combine(rutaFisicaCarpeta, nombreArchivo);
            control.SaveAs(rutaCompleta);
            return Path.Combine(RUTA_VIRTUAL_ARCHIVOS, nombreArchivo).Replace("\\", "/");
        }

        private void Msg(string msg, string type)
        {
            string clean = msg.Replace("'", "").Replace("\r\n", "");
            string script = $"$(function() {{ toastify('{type}', '{clean}', 'Sistema UTC'); }});";
            ScriptManager.RegisterStartupScript(this, GetType(), "toast", script, true);
        }

        protected void btnAbrirGenerador_Click(object sender, EventArgs e)
        {
            if (int.TryParse(hfIdEjecucionInforme.Value, out int id)) ucGenerador.Mostrar(id);
        }

        protected void ucGenerador_InformeGuardado(object sender, EventArgs e)
        {
            if (int.TryParse(hfIdEjecucionInforme.Value, out int id))
            {
                CargarInformes(id);
                Msg("Informe generado automáticamente.", "ss");
            }
        }
    }
}