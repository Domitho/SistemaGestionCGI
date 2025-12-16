using System;
using Newtonsoft.Json;

namespace SistemaGestionCGI.Models
{
    public class InvgccEjecucionMiembrosHistorial
    {
        [JsonProperty("id_historial")]
        public int id_historial { get; set; }

        [JsonProperty("fkId_miembro")]
        public int fkId_miembro { get; set; }

        [JsonProperty("dtFecha")]
        public DateTime dtFecha { get; set; }

        [JsonProperty("strAccion")]
        public string strAccion { get; set; } // Ej: 'BAJA', 'REACTIVACIÓN'

        [JsonProperty("strMotivo")]
        public string strMotivo { get; set; }

        [JsonProperty("strUsuario")]
        public string strUsuario { get; set; }
    }
}