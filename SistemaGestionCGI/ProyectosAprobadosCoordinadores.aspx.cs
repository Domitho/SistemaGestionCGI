using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;

namespace SistemaGestionCGI
{
    public partial class ProyectosAprobadosCoordinadores : System.Web.UI.Page
    {
        private readonly ManejadorEjecucionProyectos _manejador = new ManejadorEjecucionProyectos();
        private const string RUTA_VIRTUAL_ARCHIVOS = "~/RepositorioUTC/EjecucionInformes/";

        private string CedulaUsuario => Session["CedulaUsuario"]?.ToString();

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
                }
                catch (Exception ex)
                {
                    Msg("Error al iniciar el módulo: " + ex.Message, "ee");
                }
            }
        }

        // =========================================================================================
        // 1. DASHBOARD PRINCIPAL (LISTADO HORIZONTAL)
        // =========================================================================================
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
                        lit.Text = "<span class='badge bg-danger'><i class='fa-solid fa-clock me-1'></i> Plazo Vencido</span>";
                    else if (restante.TotalDays < 30)
                        lit.Text = $"<span class='badge bg-warning text-dark'><i class='fa-solid fa-hourglass-half me-1'></i> Cierra en {Math.Ceiling(restante.TotalDays)} días</span>";
                }
            }
        }

        protected void rptProyectosCoordinador_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();

            switch (e.CommandName)
            {
                case "Informes":
                    // AQUÍ ESTÁ EL CAMBIO CLAVE: Cambiamos de vista en lugar de abrir modal
                    CargarVistaGestion(int.Parse(id));
                    break;

                case "Equipo":
                    CargarEquipo(int.Parse(id));
                    break;
            }
        }

        // =========================================================================================
        // 2. VISTA DE GESTIÓN (PANEL DIVIDIDO 8/4)
        // =========================================================================================
        private void CargarVistaGestion(int idEjecucion)
        {
            hfIdEjecucionInforme.Value = idEjecucion.ToString();

            // A. Cargar Columna Izquierda (Informes de Avance)
            var listaInformes = _manejador.ObtenerInformes(idEjecucion);
            rptInformes.DataSource = listaInformes;
            rptInformes.DataBind();

            // Control de mensaje "Sin Datos"
            var divSinDatos = pnlGestionProyecto.FindControl("sinDatos") as System.Web.UI.HtmlControls.HtmlGenericControl;
            if (divSinDatos != null) divSinDatos.Visible = (listaInformes.Count == 0);

            // B. Cargar Columna Derecha (Cronología Histórica)
            // NOTA: Asegúrate de haber agregado el método ObtenerRepositorioCompleto en el BLL como hablamos antes.
            try
            {
                var historial = _manejador.ObtenerRepositorioCompleto(idEjecucion);
                rptCronologiaSide.DataSource = historial;
                rptCronologiaSide.DataBind();
            }
            catch
            {
                // Fallback si no has actualizado el BLL aun, para que no rompa
                rptCronologiaSide.DataSource = null;
                rptCronologiaSide.DataBind();
            }

            // C. Configurar Footer Lateral (Cierre y Final)
            ConfigurarDocumentosFinales(idEjecucion);

            // D. Bloqueo si está finalizado
            BloquearGestionSiFinalizado(idEjecucion);

            // E. SWITCH DE PANELES
            pnlListadoTarjetas.Visible = false;
            pnlEquipoListado.Visible = false;
            pnlGestionProyecto.Visible = true;
        }

        private void ConfigurarDocumentosFinales(int idEjecucion)
        {
            var proy = _manejador.ObtenerEjecucionPorId(idEjecucion);

            // Informe Cierre
            bool hayCierre = !string.IsNullOrEmpty(proy.strInforme_Cierre);
            lnkVerCierre.Enabled = hayCierre;
            lnkVerCierre.NavigateUrl = hayCierre ? ResolveUrl(proy.strInforme_Cierre) : "#";
            lnkVerCierre.CssClass = hayCierre
                ? "btn btn-sm btn-outline-warning text-start text-dark w-100 mb-2"
                : "btn btn-sm btn-light text-start text-muted w-100 mb-2 disabled border";

            // Informe Final
            bool hayFinal = !string.IsNullOrEmpty(proy.strInforme_Final);
            lnkVerFinal.Enabled = hayFinal;
            lnkVerFinal.NavigateUrl = hayFinal ? ResolveUrl(proy.strInforme_Final) : "#";
            lnkVerFinal.CssClass = hayFinal
                ? "btn btn-sm btn-outline-success text-start text-dark w-100"
                : "btn btn-sm btn-light text-start text-muted w-100 disabled border";
        }

        // Método auxiliar para el Frontend (Colores de borde)
        public string GetBorderColor(string tipo)
        {
            if (tipo == "AVANCE") return "border-avance";
            if (tipo == "CIERRE") return "border-cierre";
            if (tipo == "FINAL") return "border-final";
            return "";
        }

        // Botón "Volver" (Desde cualquier panel secundario)
        protected void btnVolverTarjeta_Click(object sender, EventArgs e)
        {
            pnlGestionProyecto.Visible = false;
            pnlEquipoListado.Visible = false;
            pnlListadoTarjetas.Visible = true;
            CargarMisProyectos();
        }

        // =========================================================================================
        // 3. GESTIÓN DE ARCHIVOS (SUBIDA Y ELIMINACIÓN)
        // =========================================================================================
        protected void btnGuardarInforme_Click(object sender, EventArgs e)
        {
            try
            {
                if (!flpArchivoInf.HasFile) { Msg("Seleccione un archivo.", "ww"); return; }
                if (!int.TryParse(hfIdEjecucionInforme.Value, out int idEjec)) return;

                if (!EsProyectoDelCoordinador(idEjec)) { Msg("No autorizado.", "ee"); return; }

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

                // Refrescar la VISTA DE GESTIÓN (No el modal antiguo)
                CargarVistaGestion(idEjec);

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
                lblTituloModalInforme.InnerText = "Editar Etiqueta";
                ScriptManager.RegisterStartupScript(this, GetType(), "OpenSub", "AbrirSubModalUpload();", true);
            }
            else if (e.CommandName == "EliminarInforme")
            {
                try
                {
                    _manejador.EliminarInforme(idInf);
                    CargarVistaGestion(int.Parse(hfIdEjecucionInforme.Value)); // Refrescar panel
                    Msg("Informe eliminado.", "ss");
                }
                catch (Exception ex) { Msg(ex.Message, "ee"); }
            }
        }

        protected void btnAbrirGenerador_Click(object sender, EventArgs e)
        {
            if (int.TryParse(hfIdEjecucionInforme.Value, out int id)) ucGenerador.Mostrar(id);
        }

        protected void ucGenerador_InformeGuardado(object sender, EventArgs e)
        {
            if (int.TryParse(hfIdEjecucionInforme.Value, out int id))
            {
                CargarVistaGestion(id);
                Msg("Informe generado automáticamente.", "ss");
            }
        }

        // =========================================================================================
        // 4. GESTIÓN DE EQUIPO
        // =========================================================================================
        private void CargarEquipo(int idEjecucion)
        {
            hfIdEjecucionEquipo.Value = idEjecucion.ToString();
            pnlListadoTarjetas.Visible = false;
            pnlGestionProyecto.Visible = false; // Asegurar que el otro panel esté cerrado
            pnlEquipoListado.Visible = true;

            // Ocultar legacy form
            pnlFormularioMiembro.Visible = false;
            btnAbrirFormMiembro.Visible = false;

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

        protected void rptMiembros_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            // Lógica para ocultar botones de edición a coordinadores (Solo lectura)
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var btnEdit = e.Item.FindControl("btnEditarM");
                if (btnEdit != null) btnEdit.Visible = false;

                var btnToggle = e.Item.FindControl("btnToggleEstado");
                if (btnToggle != null) btnToggle.Visible = false;
            }
        }

        // =========================================================================================
        // 5. UTILIDADES Y SEGURIDAD
        // =========================================================================================
        private bool EsProyectoDelCoordinador(int idEjecucion)
        {
            var misProyectos = _manejador.ObtenerEjecuciones(CedulaUsuario);
            return misProyectos.Exists(x => x.strId_ejec == idEjecucion);
        }

        private void BloquearGestionSiFinalizado(int idEjecucion)
        {
            var proy = _manejador.ObtenerEjecucionPorId(idEjecucion);

            bool cerrado = (proy.strEstado_ejec == "FINALIZADO" || proy.strEstado_ejec == "CIERRE APROBADO");

            btnAbrirGenerador.Visible = !cerrado;

            var btnSubir = pnlGestionProyecto.FindControl("btnSubirEscaneado") as System.Web.UI.HtmlControls.HtmlButton;
            if (btnSubir != null)
            {
                btnSubir.Visible = !cerrado;
            }
            else if (this.FindControl("btnSubirEscaneado") != null) 
            {
                this.FindControl("btnSubirEscaneado").Visible = !cerrado;
            }

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

        // Métodos Legacy (Stubs para evitar errores de compilación con paneles ocultos)
        protected void btnAbrirFormMiembro_Click(object sender, EventArgs e) { }
        protected void btnGuardarMiembro_Click(object sender, EventArgs e) { }
        protected void btnCancelarMiembro_Click(object sender, EventArgs e) { }
        protected void btnGuardarCierre_Click(object sender, EventArgs e) { }
        protected void btnGuardarFinal_Click(object sender, EventArgs e) { }

        //

        // Método para devolver la clase CSS del icono según el tipo
        public string GetIconClass(string tipo)
        {
            if (tipo == "AVANCE") return "fa-regular fa-file-pdf"; // Icono PDF
            if (tipo == "CIERRE") return "fa-solid fa-flag-checkered"; // Icono Meta
            if (tipo == "FINAL") return "fa-solid fa-award"; // Icono Premio
            return "fa-solid fa-file";
        }

        // Método para el color de fondo del icono
        public string GetIconBgClass(string tipo)
        {
            if (tipo == "AVANCE") return "bg-icon-avance";
            if (tipo == "CIERRE") return "bg-icon-cierre";
            if (tipo == "FINAL") return "bg-icon-final";
            return "bg-light";
        }

        // Método para texto descriptivo extra
        public string GetDescripcion(string tipo)
        {
            if (tipo == "AVANCE") return "Informe de seguimiento periódico";
            if (tipo == "CIERRE") return "Documento de cierre administrativo";
            if (tipo == "FINAL") return "Aprobación final del proyecto";
            return "Documento del proyecto";
        }

        // Devuelve la clase del icono según la extensión del archivo
        public string GetFileIconClass(object pathObj)
        {
            string path = pathObj?.ToString() ?? "";
            string ext = System.IO.Path.GetExtension(path).ToLower();

            if (ext == ".pdf") return "fa-solid fa-file-pdf text-danger";
            if (ext == ".doc" || ext == ".docx") return "fa-solid fa-file-word text-primary";
            if (ext == ".xls" || ext == ".xlsx") return "fa-solid fa-file-excel text-success";
            if (ext == ".jpg" || ext == ".png" || ext == ".jpeg") return "fa-solid fa-file-image text-warning";

            return "fa-solid fa-file text-secondary"; // Por defecto
        }

        // Devuelve el texto "PDF", "WORD", etc.
        public string GetFileTypeLabel(object pathObj)
        {
            string path = pathObj?.ToString() ?? "";
            string ext = System.IO.Path.GetExtension(path).ToLower().Replace(".", "").ToUpper();
            return string.IsNullOrEmpty(ext) ? "ARCHIVO" : ext;
        }
    }
}