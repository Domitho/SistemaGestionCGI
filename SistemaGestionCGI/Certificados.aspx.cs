using System;
using System.Web.UI;
using System.Collections.Generic;
using System.IO;
using SistemaGestionCGI.BLL;
using SistemaGestionCGI.Models;

namespace SistemaGestionCGI
{
    public partial class Certificados : System.Web.UI.Page
    {
        private readonly ManejadorCertificados _manejador = new ManejadorCertificados();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            LimpiarVista();
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            BuscarPorCedula();
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            Response.Redirect("Dashboard.aspx", false);
        }

        protected void rptGrupos_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DescargarCertificado")
            {
                string idIntegrante = e.CommandArgument != null ? e.CommandArgument.ToString() : string.Empty;

                if (string.IsNullOrWhiteSpace(idIntegrante))
                {
                    MostrarToast("error", "No se pudo identificar el registro seleccionado.");
                    return;
                }

                string url = ResolveUrl("~/CertificadoGrupo.aspx?id=" + Server.UrlEncode(idIntegrante));
                string script = "abrirModalReporte('" + url + "');";

                ScriptManager.RegisterStartupScript(
                    this,
                    this.GetType(),
                    Guid.NewGuid().ToString(),
                    script,
                    true
                );
            }
        }

        private void BuscarPorCedula()
        {
            try
            {
                string cedula = (txtCedula.Text ?? string.Empty).Trim();

                LimpiarMensajes();
                OcultarResultados();

                if (!ValidarCedulaIngresada(cedula))
                    return;

                List<CertificadoGrupoDTO> lista = _manejador.BuscarGruposPorCedula(cedula);

                if (lista == null || lista.Count == 0)
                {
                    MostrarToast("error", "No se encontraron registros en Grupos de Investigación para la cédula ingresada.");
                    return;
                }

                CargarDatosPersona(lista[0]);
                CargarResultados(lista);

                MostrarToast("info", "Información recuperada correctamente.");
            }
            catch
            {
                MostrarToast("error", "Ocurrió un error al consultar la información.");
            }
        }

        private bool ValidarCedulaIngresada(string cedula)
        {
            if (string.IsNullOrWhiteSpace(cedula))
            {
                MostrarToast("error", "Ingrese una cédula.");
                txtCedula.Focus();
                return false;
            }

            if (cedula.Length != 10)
            {
                MostrarToast("error", "La cédula debe contener 10 dígitos.");
                txtCedula.Focus();
                return false;
            }

            if (!EsSoloNumeros(cedula))
            {
                MostrarToast("error", "La cédula solo debe contener números.");
                txtCedula.Focus();
                return false;
            }

            return true;
        }

        private bool EsSoloNumeros(string texto)
        {
            foreach (char c in texto)
            {
                if (!char.IsDigit(c))
                    return false;
            }

            return true;
        }

        private void CargarDatosPersona(CertificadoGrupoDTO persona)
        {
            if (persona == null)
            {
                pnlPersona.Visible = false;
                return;
            }

            lblCedula.Text = persona.Cedula;
            lblNombres.Text = persona.Nombres;
            lblApellidos.Text = persona.Apellidos;

            pnlPersona.Visible = true;
        }

        private void CargarResultados(List<CertificadoGrupoDTO> lista)
        {
            lblTotalGrupos.Text = lista.Count.ToString();
            rptGrupos.DataSource = lista;
            rptGrupos.DataBind();
            pnlResultados.Visible = true;
        }

        private void LimpiarVista()
        {
            txtCedula.Text = string.Empty;
            LimpiarMensajes();
            LimpiarDatosPersona();
            LimpiarRepeater();
            OcultarResultados();
        }

        private void LimpiarMensajes()
        {
            lblMensaje.Text = string.Empty;
        }

        private void LimpiarDatosPersona()
        {
            lblCedula.Text = string.Empty;
            lblNombres.Text = string.Empty;
            lblApellidos.Text = string.Empty;
            lblTotalGrupos.Text = "0";
        }

        private void LimpiarRepeater()
        {
            rptGrupos.DataSource = null;
            rptGrupos.DataBind();
        }

        private void OcultarResultados()
        {
            pnlPersona.Visible = false;
            pnlResultados.Visible = false;
        }

        private void MostrarToast(string tipo, string mensaje)
        {
            string msg = (mensaje ?? "").Replace("'", "\\'");
            string script = $"toastify('{tipo}', '{msg}', 'Sistema UTC');";

            System.Web.UI.ScriptManager.RegisterStartupScript(
                this,
                this.GetType(),
                Guid.NewGuid().ToString(),
                script,
                true
            );
        }
    }
}