using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;

namespace SistemaGestionCGI
{
    public partial class EjecucionProAprobados : System.Web.UI.Page
    {
        private readonly ManejadorEjecucionProyectos _manejador = new ManejadorEjecucionProyectos();
        private readonly ManejadorProyectos _manejadorProyectos = new ManejadorProyectos();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 1. Validar Sesión
                if (Session["UsuarioLogueado"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                // 2. Cargar Datos Iniciales
                CargarGrillaEjecucion();

                // 3. Verificar si venimos de una redirección (Ej: al guardar un miembro)
                string idTeamRedirect = Request.QueryString["idTeam"];
                if (!string.IsNullOrEmpty(idTeamRedirect))
                {
                    if (int.TryParse(idTeamRedirect, out int idTeam))
                    {
                        CargarEquipo(idTeam);
                    }
                }

                // 4. Mostrar Mensajes Flash (Toastify)
                if (Session["TempMsg"] != null)
                {
                    string mensaje = Session["TempMsg"].ToString();
                    string tipo = Session["TempTipo"].ToString();
                    Msg(mensaje, tipo);

                    Session["TempMsg"] = null;
                    Session["TempTipo"] = null;
                }
            }
        }

        // =======================================================
        // 1. GESTIÓN PRINCIPAL (GRIDS Y COMBOS)
        // =======================================================

        private void CargarGrillaEjecucion()
        {
            try
            {
                List<InvgccEjecucionProyectos> lista = _manejador.ObtenerEjecuciones();
                rptEjecucion.DataSource = lista;
                rptEjecucion.DataBind();
            }
            catch (Exception ex)
            {
                Msg("Error al cargar ejecuciones: " + ex.Message, "ee");
            }
        }

        private void CargarProyectosAprobados()
        {
            try
            {
                var lista = _manejador.ObtenerProyectosAprobadosSinEjecucion();
                ddlProyectosAprobados.DataSource = lista;
                ddlProyectosAprobados.DataTextField = "strTema_pro";
                ddlProyectosAprobados.DataValueField = "strId_pro";
                ddlProyectosAprobados.DataBind();
                ddlProyectosAprobados.Items.Insert(0, new ListItem("-- Seleccione Proyecto --", ""));
            }
            catch (Exception ex)
            {
                Msg("Error al cargar proyectos: " + ex.Message, "ee");
            }
        }

        protected void btnNuevoEjecucion_Click(object sender, EventArgs e)
        {
            pnlGrilla.Visible = false;
            pnlAgregar.Visible = true;
            headerEjecucion.Visible = true;
            btnNuevoEjecucion.Visible = false;
            btnRegresar.Visible = true;

            CargarProyectosAprobados();

            txtCoordinadorAdd.Text = "";
            txtFechaIniAdd.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtPeriodoAdd.Text = "";
        }

        protected void ddlProyectosAprobados_SelectedIndexChanged(object sender, EventArgs e)
        {
            string idPro = ddlProyectosAprobados.SelectedValue;
            if (!string.IsNullOrEmpty(idPro))
            {
                var pro = _manejadorProyectos.ObtenerPorId(idPro);
                if (pro != null)
                {
                    txtCoordinadorAdd.Text = pro.strCoordinador_pro;
                }
            }
        }

        protected void btnGuardarNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ddlProyectosAprobados.SelectedValue))
                {
                    Msg("Debe seleccionar un proyecto aprobado.", "ww");
                    return;
                }

                InvgccEjecucionProyectos obj = new InvgccEjecucionProyectos();
                obj.fkId_pro = ddlProyectosAprobados.SelectedValue; // Es string (VARCHAR)
                obj.strCoordinador_ejec = txtCoordinadorAdd.Text.Trim();
                obj.strPeriodo_ejec = txtPeriodoAdd.Text.Trim();
                obj.dtFechaini_ejec = DateTime.Parse(txtFechaIniAdd.Text);
                obj.dtFechafin_ejec = null;

                if (flpArchivoAdd.HasFile)
                {
                    string nombre = "PLAN_" + DateTime.Now.Ticks + Path.GetExtension(flpArchivoAdd.FileName);
                    // IMPORTANTE: Aquí se usa el método de 2 parámetros
                    obj.strInforme_ejec = GuardarArchivo(flpArchivoAdd, nombre);
                }

                _manejador.GuardarEjecucion(obj);

                SetFlashMessage("Ejecución iniciada correctamente.", "ss");
                Response.Redirect("EjecucionProAprobados.aspx", false);
            }
            catch (Exception ex)
            {
                Msg("Error al guardar: " + ex.Message, "ee");
            }
        }

        protected void rptEjecucion_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            // Parseamos ID a INT porque es Identity en BD
            int id = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "Editar")
            {
                CargarEdicion(id);
            }
            else if (e.CommandName == "Equipo")
            {
                CargarEquipo(id);
            }
            else if (e.CommandName == "Informes")
            {
                hfIdEjecucionInforme.Value = id.ToString();
                CargarInformes(id);
                // Abrir modal
                ScriptManager.RegisterStartupScript(this, GetType(), "OpenModalInf", "AbrirModalInformes();", true);
            }
            else if (e.CommandName == "Eliminar")
            {
                try
                {
                    _manejador.EliminarEjecucion(id);
                    SetFlashMessage("Registro eliminado correctamente.", "ss");
                    Response.Redirect("EjecucionProAprobados.aspx", false);
                }
                catch (Exception ex)
                {
                    Msg("Error al eliminar: " + ex.Message, "ee");
                }
            }
        }

        private void CargarEdicion(int id)
        {
            var obj = _manejador.ObtenerEjecucionPorId(id);
            if (obj != null)
            {
                hfIdEjecEdit.Value = obj.strId_ejec.ToString();
                txtProyectoReadOnly.Text = obj.TituloProyecto;
                txtCoordinadorEdit.Text = obj.strCoordinador_ejec;
                txtFechaIniEdit.Text = obj.dtFechaini_ejec.ToString("yyyy-MM-dd");
                txtFechaFinEdit.Text = obj.dtFechafin_ejec.HasValue ? obj.dtFechafin_ejec.Value.ToString("yyyy-MM-dd") : "";
                txtPeriodoEdit.Text = obj.strPeriodo_ejec;
                hfArchivoActual.Value = obj.strInforme_ejec;

                pnlGrilla.Visible = false;
                pnlAgregar.Visible = false;
                pnlEditar.Visible = true;

                headerEjecucion.Visible = true;
                btnNuevoEjecucion.Visible = false;
                btnRegresar.Visible = true;
            }
        }

        protected void btnGuardarEdit_Click(object sender, EventArgs e)
        {
            try
            {
                InvgccEjecucionProyectos obj = new InvgccEjecucionProyectos();
                obj.strId_ejec = int.Parse(hfIdEjecEdit.Value); // Parse INT
                obj.strCoordinador_ejec = txtCoordinadorEdit.Text.Trim();
                obj.dtFechaini_ejec = DateTime.Parse(txtFechaIniEdit.Text);

                if (!string.IsNullOrEmpty(txtFechaFinEdit.Text))
                    obj.dtFechafin_ejec = DateTime.Parse(txtFechaFinEdit.Text);
                else
                    obj.dtFechafin_ejec = null;

                obj.strPeriodo_ejec = txtPeriodoEdit.Text.Trim();
                obj.strInforme_ejec = hfArchivoActual.Value;

                if (flpArchivoEdit.HasFile)
                {
                    string nombre = "PLAN_" + DateTime.Now.Ticks + Path.GetExtension(flpArchivoEdit.FileName);
                    obj.strInforme_ejec = GuardarArchivo(flpArchivoEdit, nombre);
                }

                _manejador.ActualizarEjecucion(obj);

                SetFlashMessage("Datos de ejecución actualizados correctamente.", "ss");
                Response.Redirect("EjecucionProAprobados.aspx", false);
            }
            catch (Exception ex)
            {
                Msg("Error al actualizar: " + ex.Message, "ee");
            }
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            pnlAgregar.Visible = false;
            pnlEditar.Visible = false;
            pnlEquipoListado.Visible = false;
            pnlFormularioMiembro.Visible = false;

            pnlGrilla.Visible = true;
            headerEjecucion.Visible = true;
            btnNuevoEjecucion.Visible = true;
            btnRegresar.Visible = false;

            CargarGrillaEjecucion();
        }

        // =======================================================
        // 2. GESTIÓN DE EQUIPO
        // =======================================================

        private void CargarEquipo(int idEjecucion)
        {
            hfIdEjecucionEquipo.Value = idEjecucion.ToString();

            pnlGrilla.Visible = false;
            headerEjecucion.Visible = false;
            pnlEquipoListado.Visible = true;
            pnlFormularioMiembro.Visible = false;

            RefrescarTablaMiembros();
        }

        private void RefrescarTablaMiembros()
        {
            if (int.TryParse(hfIdEjecucionEquipo.Value, out int id))
            {
                var miembros = _manejador.ObtenerMiembros(id);
                rptMiembros.DataSource = miembros;
                rptMiembros.DataBind();
            }
        }

        protected void btnAbrirFormMiembro_Click(object sender, EventArgs e)
        {
            pnlEquipoListado.Visible = false;
            pnlFormularioMiembro.Visible = true;

            hfIdMiembroEdit.Value = "";
            lblTituloFormMiembro.Text = "Nuevo Integrante";

            txtCedulaMiembro.Text = "";
            txtNombresMiembro.Text = "";
            txtApellidosMiembro.Text = "";
            ddlRolMiembro.SelectedIndex = 0;
        }

        protected void btnGuardarMiembro_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCedulaMiembro.Text) || string.IsNullOrWhiteSpace(txtNombresMiembro.Text))
                {
                    Msg("Complete los campos obligatorios.", "ww");
                    return;
                }

                InvgccEjecucionMiembros m = new InvgccEjecucionMiembros();
                m.fkId_ejec = int.Parse(hfIdEjecucionEquipo.Value); // Parse INT
                m.strCedula_miembro = txtCedulaMiembro.Text.Trim();
                m.strNombres_miembro = txtNombresMiembro.Text.Trim();
                m.strApellidos_miembro = txtApellidosMiembro.Text.Trim();
                m.strRol_miembro = ddlRolMiembro.SelectedValue;

                if (string.IsNullOrEmpty(hfIdMiembroEdit.Value))
                {
                    _manejador.GuardarMiembro(m);
                    SetFlashMessage("Integrante agregado correctamente.", "ss");
                }
                else
                {
                    m.strId_miembro = int.Parse(hfIdMiembroEdit.Value); // Parse INT
                    _manejador.ActualizarMiembro(m);
                    SetFlashMessage("Datos del integrante actualizados.", "ss");
                }

                Response.Redirect($"EjecucionProAprobados.aspx?idTeam={m.fkId_ejec}", false);
            }
            catch (Exception ex)
            {
                Msg("Error al guardar miembro: " + ex.Message, "ee");
            }
        }

        protected void btnCancelarMiembro_Click(object sender, EventArgs e)
        {
            pnlFormularioMiembro.Visible = false;
            pnlEquipoListado.Visible = true;
            RefrescarTablaMiembros();
        }

        protected void rptMiembros_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int idMiembro = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "EliminarMiembro")
            {
                try
                {
                    _manejador.EliminarMiembro(idMiembro);
                    SetFlashMessage("Integrante eliminado del equipo.", "ss");

                    // Recargar misma vista
                    string currentTeamId = hfIdEjecucionEquipo.Value;
                    Response.Redirect($"EjecucionProAprobados.aspx?idTeam={currentTeamId}", false);
                }
                catch (Exception ex)
                {
                    Msg("Error al eliminar: " + ex.Message, "ee");
                }
            }
            else if (e.CommandName == "EditarMiembro")
            {
                try
                {
                    var miembro = _manejador.ObtenerMiembroPorId(idMiembro);
                    if (miembro != null)
                    {
                        hfIdMiembroEdit.Value = miembro.strId_miembro.ToString();
                        txtCedulaMiembro.Text = miembro.strCedula_miembro;
                        txtNombresMiembro.Text = miembro.strNombres_miembro;
                        txtApellidosMiembro.Text = miembro.strApellidos_miembro;

                        if (ddlRolMiembro.Items.FindByValue(miembro.strRol_miembro) != null)
                            ddlRolMiembro.SelectedValue = miembro.strRol_miembro;

                        lblTituloFormMiembro.Text = "Editar Integrante";
                        pnlEquipoListado.Visible = false;
                        pnlFormularioMiembro.Visible = true;
                    }
                }
                catch (Exception ex)
                {
                    Msg("Error al cargar miembro: " + ex.Message, "ee");
                }
            }
        }

        protected void btnVolverDeEquipo_Click(object sender, EventArgs e)
        {
            Response.Redirect("EjecucionProAprobados.aspx");
        }

        // =======================================================
        // 3. GESTIÓN DE INFORMES
        // =======================================================

        private void CargarInformes(int idEjecucion)
        {
            var informes = _manejador.ObtenerInformes(idEjecucion);
            rptInformes.DataSource = informes;
            rptInformes.DataBind();
        }

        protected void btnGuardarInforme_Click(object sender, EventArgs e)
        {
            try
            {
                if (!flpArchivoInf.HasFile)
                {
                    Msg("Seleccione un archivo.", "ww");
                    ScriptManager.RegisterStartupScript(this, GetType(), "Reopen", "AbrirSubModalUpload();", true);
                    return;
                }

                // Parsear ID Padre
                if (!int.TryParse(hfIdEjecucionInforme.Value, out int idEjec))
                {
                    Msg("Error identificando el proyecto. Recargue.", "ee");
                    return;
                }

                string nombrePeriodo = txtNombrePeriodoInf.Text.Trim();
                if (string.IsNullOrEmpty(nombrePeriodo)) nombrePeriodo = "Informe de Avance";

                string fileName = "INF_" + DateTime.Now.Ticks + Path.GetExtension(flpArchivoInf.FileName);
                string ruta = GuardarArchivo(flpArchivoInf, fileName);

                InvgccEjecucionInformes inf = new InvgccEjecucionInformes();
                inf.fkId_ejec = idEjec;
                inf.strNombrePeriodo = nombrePeriodo;
                inf.strArchivo_path = ruta;

                _manejador.GuardarInforme(inf);

                Msg("Informe subido correctamente.", "ss");

                txtNombrePeriodoInf.Text = "";
                CargarInformes(idEjec);

                ScriptManager.RegisterStartupScript(this, GetType(), "CloseSubModal", "CerrarSubModalUpload();", true);
            }
            catch (Exception ex)
            {
                Msg("Error al subir informe: " + ex.Message, "ee");
            }
        }

        protected void rptInformes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "EliminarInforme")
            {
                int idInforme = int.Parse(e.CommandArgument.ToString());
                try
                {
                    _manejador.EliminarInforme(idInforme);
                    SetFlashMessage("Informe eliminado correctamente.", "ss");
                    Response.Redirect("EjecucionProAprobados.aspx", false);
                }
                catch (Exception ex)
                {
                    Msg("Error al eliminar informe: " + ex.Message, "ee");
                }
            }
        }

        // =======================================================
        // 4. UTILIDADES Y BOTONES EXTRA
        // =======================================================

        private string GuardarArchivo(FileUpload control, string nombreArchivo)
        {
            string carpetaFisica = @"C:\UTC\EJECUCION_INFORMES\";
            if (!Directory.Exists(carpetaFisica))
                Directory.CreateDirectory(carpetaFisica);

            string rutaCompleta = Path.Combine(carpetaFisica, nombreArchivo);
            control.SaveAs(rutaCompleta);
            return rutaCompleta;
        }

        protected void btnCancelarNew_Click(object sender, EventArgs e)
        {
            btnRegresar_Click(null, null);
        }

        protected void btnCancelarEdit_Click(object sender, EventArgs e)
        {
            btnRegresar_Click(null, null);
        }

        private void SetFlashMessage(string msg, string type)
        {
            Session["TempMsg"] = msg;
            Session["TempTipo"] = type;
        }

        private void Msg(string msg, string type)
        {
            string cleanMsg = msg.Replace("'", "\\'").Replace("\r\n", " ").Replace("\n", " ").Replace("\\", "\\\\");
            string script = $"$(function() {{ toastify('{type}', '{cleanMsg}', 'Sistema'); }});";
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", script, true);
        }
    }
}