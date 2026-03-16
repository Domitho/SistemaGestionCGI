using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestionCGI.Models
{
    public class HistorialIntegranteDTO
    {
        public int IdHistorial { get; set; }
        public string IdIntegrante { get; set; }
        public string Fecha { get; set; }
        public string Accion { get; set; }
        public string Motivo { get; set; }
        public string Usuario { get; set; }
    }
}