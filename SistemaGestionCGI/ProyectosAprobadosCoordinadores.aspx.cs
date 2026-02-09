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
                try
                {
                    if (data.FinCicloActual != null)
                        fin = data.FinCicloActual;
                    else
                        fin = data.dtFechafin_ejec; 
                }
                catch { }

                string estado = data.strEstado_ejec.ToString();

                if (fin.HasValue && fin.Value.Year > 1900 && (estado == "EN EJECUCIÓN" || estado == "En Ejecución"))
                {
                    TimeSpan restante = fin.Value.Date - DateTime.Now.Date;
                    int dias = (int)restante.TotalDays;

                    if (dias < 0)
                    {
                        lit.Text = $"<span class='badge bg-danger bg-opacity-10 text-danger border border-danger'><i class='fa-solid fa-circle-exclamation me-1'></i> Periodo Vencido hace {Math.Abs(dias)} días</span>";
                    }
                    else if (dias < 30)
                    {
                        lit.Text = $"<span class='badge bg-warning bg-opacity-10 text-dark border border-warning'><i class='fa-solid fa-clock me-1'></i> Cierra en {dias} días</span>";
                    }
                    else
                    {
                        lit.Text = $"<span class='badge bg-success bg-opacity-10 text-success border border-success'><i class='fa-regular fa-calendar-check me-1'></i> A tiempo ({dias} días restantes)</span>";
                    }
                }
                else if (estado == "FINALIZADO" || estado == "CIERRE APROBADO")
                {
                    lit.Text = "<span class='badge bg-secondary opacity-50'><i class='fa-solid fa-lock me-1'></i> Finalizado</span>";
                }
                else
                {
                    lit.Text = "<span class='text-muted small'>- Sin plazo definido -</span>";
                }
            }
        }

        protected void rptProyectosCoordinador_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string id = e.CommandArgument.ToString();

            switch (e.CommandName)
            {
                case "Informes":
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

            var listaInformes = _manejador.ObtenerInformes(idEjecucion);
            rptInformes.DataSource = listaInformes;
            rptInformes.DataBind();

            var divSinDatos = pnlGestionProyecto.FindControl("sinDatos") as System.Web.UI.HtmlControls.HtmlGenericControl;
            if (divSinDatos != null) divSinDatos.Visible = (listaInformes.Count == 0);

            try
            {
                CargarCronologiaAgrupada(idEjecucion);
            }
            catch
            {
                rptPeriodos.DataSource = null;
                rptPeriodos.DataBind();
            }

            ConfigurarDocumentosFinales(idEjecucion);

            BloquearGestionSiFinalizado(idEjecucion);

            pnlListadoTarjetas.Visible = false;
            pnlEquipoListado.Visible = false;
            pnlGestionProyecto.Visible = true;
        }

        private void ConfigurarDocumentosFinales(int idEjecucion)
        {
            var proy = _manejador.ObtenerEjecucionPorId(idEjecucion);

            bool hayCierre = !string.IsNullOrEmpty(proy.strInforme_Cierre);
            lnkVerCierre.Enabled = hayCierre;
            lnkVerCierre.NavigateUrl = hayCierre ? ResolveUrl(proy.strInforme_Cierre) : "#";
            lnkVerCierre.CssClass = hayCierre
                ? "btn btn-sm btn-outline-warning text-start text-dark w-100 mb-2"
                : "btn btn-sm btn-light text-start text-muted w-100 mb-2 disabled border";

            bool hayFinal = !string.IsNullOrEmpty(proy.strInforme_Final);
            lnkVerFinal.Enabled = hayFinal;
            lnkVerFinal.NavigateUrl = hayFinal ? ResolveUrl(proy.strInforme_Final) : "#";
            lnkVerFinal.CssClass = hayFinal
                ? "btn btn-sm btn-outline-success text-start text-dark w-100"
                : "btn btn-sm btn-light text-start text-muted w-100 disabled border";
        }

        public string GetBorderColor(string tipo)
        {
            if (tipo == "AVANCE") return "border-avance";
            if (tipo == "CIERRE") return "border-cierre";
            if (tipo == "FINAL") return "border-final";
            return "";
        }

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

                var proy = _manejador.ObtenerEjecucionPorId(idEjec);

                if (proy.FinCicloActual.HasValue)
                {
                    if (DateTime.Now.Date > proy.FinCicloActual.Value.Date)
                    {
                        Msg($"PERIODO VENCIDO: El ciclo actual finalizó el {proy.FinCicloActual:dd/MM/yyyy}. Contacte al administrador.", "ee");
                        return;
                    }
                }

                if (proy.InicioCicloActual.HasValue)
                {
                    if (DateTime.Now.Date < proy.InicioCicloActual.Value.Date)
                    {
                        Msg($"ESPERA: El periodo asignado inicia el {proy.InicioCicloActual:dd/MM/yyyy}.", "ww");
                        return;
                    }
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
                {
                    string nombreCiclo = proy.strPeriodo_ejec;

                    _manejador.GuardarInforme(inf, nombreCiclo);
                }
                else
                {
                    inf.strId_informe = int.Parse(hfIdInformeEdit.Value);
                    _manejador.ActualizarInforme(inf);
                }

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
                    CargarVistaGestion(int.Parse(hfIdEjecucionInforme.Value)); 
                    Msg("Informe eliminado.", "ss");
                }
                catch (Exception ex) { Msg(ex.Message, "ee"); }
            }

            if (e.CommandName == "MarcarLeido")
            {
                try
                {

                    _manejador.MarcarObservacionComoLeida(idInf);

                    int idEjec = int.Parse(hfIdEjecucionInforme.Value);
                    CargarVistaGestion(idEjec);

                    Msg("Se ha registrado su confirmación de lectura exitosamente.", "ss");
                }
                catch (Exception ex)
                {
                    Msg("Error al confirmar lectura: " + ex.Message, "ee");
                }
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
            pnlGestionProyecto.Visible = false; 
            pnlEquipoListado.Visible = true;

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

            lblEstadoPeriodo.Text = "";

            bool proyectoCerrado = (proy.strEstado_ejec == "FINALIZADO" || proy.strEstado_ejec == "CIERRE APROBADO");
            bool periodoVencido = false;

            if (proy.FinCicloActual.HasValue && !proyectoCerrado)
            {
                if (DateTime.Now.Date > proy.FinCicloActual.Value.Date)
                {
                    periodoVencido = true;
                }
            }

            bool bloquearEdicion = proyectoCerrado || periodoVencido;

            btnAbrirGenerador.Visible = !bloquearEdicion;
            var btnSubir = pnlGestionProyecto.FindControl("btnSubirEscaneado") as System.Web.UI.HtmlControls.HtmlButton;
            if (btnSubir != null) btnSubir.Visible = !bloquearEdicion;

            if (periodoVencido)
            {
                lblEstadoPeriodo.Text = @"
                    <div class='badge bg-danger bg-opacity-10 text-danger border border-danger px-3 py-2'>
                        <i class='fa-solid fa-lock me-2'></i> 
                        PERIODO VENCIDO: No se permiten cargas. Contacte al Administrador.
                    </div>";
            }
            else if (proyectoCerrado)
            {
                lblEstadoPeriodo.Text = @"
                    <div class='badge bg-success bg-opacity-10 text-success border border-success px-3 py-2'>
                        <i class='fa-solid fa-check-circle me-2'></i> 
                        PROYECTO FINALIZADO
                    </div>";
            }

            foreach (RepeaterItem item in rptInformes.Items)
            {
                var btnEdit = item.FindControl("btnEditarInf");
                var btnDel = item.FindControl("btnEliminarInf");
                if (btnEdit != null) btnEdit.Visible = !bloquearEdicion;
                if (btnDel != null) btnDel.Visible = !bloquearEdicion;
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

        protected void btnAbrirFormMiembro_Click(object sender, EventArgs e) { }
        protected void btnGuardarMiembro_Click(object sender, EventArgs e) { }
        protected void btnCancelarMiembro_Click(object sender, EventArgs e) { }
        protected void btnGuardarCierre_Click(object sender, EventArgs e) { }
        protected void btnGuardarFinal_Click(object sender, EventArgs e) { }

        //

        public string GetIconClass(string tipo)
        {
            if (tipo == "AVANCE") return "fa-regular fa-file-pdf"; 
            if (tipo == "CIERRE") return "fa-solid fa-flag-checkered"; 
            if (tipo == "FINAL") return "fa-solid fa-award"; 
            return "fa-solid fa-file";
        }

        public string GetIconBgClass(string tipo)
        {
            if (tipo == "AVANCE") return "bg-icon-avance";
            if (tipo == "CIERRE") return "bg-icon-cierre";
            if (tipo == "FINAL") return "bg-icon-final";
            return "bg-light";
        }

        public string GetDescripcion(string tipo)
        {
            if (tipo == "AVANCE") return "Informe de seguimiento periódico";
            if (tipo == "CIERRE") return "Documento de cierre administrativo";
            if (tipo == "FINAL") return "Aprobación final del proyecto";
            return "Documento del proyecto";
        }

        public string GetFileIconClass(object pathObj)
        {
            string path = pathObj?.ToString() ?? "";
            string ext = System.IO.Path.GetExtension(path).ToLower();

            if (ext == ".pdf") return "fa-solid fa-file-pdf text-danger";
            if (ext == ".doc" || ext == ".docx") return "fa-solid fa-file-word text-primary";
            if (ext == ".xls" || ext == ".xlsx") return "fa-solid fa-file-excel text-success";
            if (ext == ".jpg" || ext == ".png" || ext == ".jpeg") return "fa-solid fa-file-image text-warning";

            return "fa-solid fa-file text-secondary"; 
        }

        public string GetFileTypeLabel(object pathObj)
        {
            string path = pathObj?.ToString() ?? "";
            string ext = System.IO.Path.GetExtension(path).ToLower().Replace(".", "").ToUpper();
            return string.IsNullOrEmpty(ext) ? "ARCHIVO" : ext;
        }

        // PERIODOS

        private void CargarCronologiaAgrupada(int idEjecucion)
        {
            var listaArchivos = _manejador.ObtenerRepositorioCompleto(idEjecucion);

            List<CicloAcademico> listaCiclos = _manejador.ObtenerTodosLosCiclos();

            var listaAgrupada = listaArchivos
                .GroupBy(archivo => IdentificarCiclo(Convert.ToDateTime(archivo.Fecha), listaCiclos))
                .Select(g => new GrupoPeriodo
                {
                    NombrePeriodo = g.Key.strNombre_ciclo,
                    FechaInicioCiclo = g.Key.dtInicio_ciclo,
                    Archivos = g.ToList() 
                })
                .OrderByDescending(g => g.FechaInicioCiclo) 
                .ToList();

            rptPeriodos.DataSource = listaAgrupada;
            rptPeriodos.DataBind();

            var divSinHistorial = pnlGestionProyecto.FindControl("sinHistorial") as System.Web.UI.HtmlControls.HtmlGenericControl;
            if (divSinHistorial != null) divSinHistorial.Visible = (listaAgrupada.Count == 0);
        }

        private CicloAcademico IdentificarCiclo(DateTime fechaArchivo, List<CicloAcademico> ciclos)
        {

            var ciclo = ciclos.FirstOrDefault(c => fechaArchivo.Date >= c.dtInicio_ciclo.Date);

            if (ciclo != null) return ciclo;

            return new CicloAcademico { strNombre_ciclo = "HISTORIAL ANTIGUO / SIN PERIODO", dtInicio_ciclo = DateTime.MinValue };
        }

        protected void rptPeriodos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var grupo = (GrupoPeriodo)e.Item.DataItem;

                var rptHijo = (Repeater)e.Item.FindControl("rptArchivosPeriodo");

                if (rptHijo != null)
                {
                    rptHijo.DataSource = grupo.Archivos;
                    rptHijo.DataBind();
                }
            }
        }
    }
}