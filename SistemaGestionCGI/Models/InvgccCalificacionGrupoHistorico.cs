using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestionCGI.Models
{
    public class InvgccCalificacionGrupoHistorico
    {
        public int idHistorico { get; set; }
        public string idCalificacion { get; set; }
        public string idGrupo { get; set; }
        public string accion { get; set; }
        public string usuarioAccion { get; set; }
        public DateTime fechaAccion { get; set; }
        public string descripcion { get; set; }
        public string datosAnteriores { get; set; }
        public string datosNuevos { get; set; }
    }
}