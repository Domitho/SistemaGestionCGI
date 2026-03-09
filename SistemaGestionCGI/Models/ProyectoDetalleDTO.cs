using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestionCGI.Models
{
    public class ProyectoDetalleDTO
    {
        public string Id { get; set; }
        public string Codigo { get; set; }
        public string NombreProyecto { get; set; }
        public string Coordinador { get; set; }
        public string Periodo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Informe { get; set; }
        public string Estado { get; set; }
    }
}