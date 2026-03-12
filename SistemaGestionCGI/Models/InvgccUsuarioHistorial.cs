using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestionCGI.Models
{
    public class InvgccUsuarioHistorial
    {
        public int IdHistorial { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Rol { get; set; }
        public bool Activo { get; set; }
        public string TipoEvento { get; set; }
        public DateTime FechaEvento { get; set; }
        public string RealizadoPor { get; set; }
    }
}