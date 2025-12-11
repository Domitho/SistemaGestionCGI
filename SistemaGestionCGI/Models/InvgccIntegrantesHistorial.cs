using System;
using Newtonsoft.Json;

namespace SistemaGestionCGI.Models
{
    public class InvgccIntegrantesHistorial
    {
        [JsonProperty("idHistorial")]
        public int idHistorial { get; set; } // Este sí parece Identity (int)

        [JsonProperty("strId_int")]
        public string strId_int { get; set; }

        [JsonProperty("dtFecha")]
        public DateTime dtFecha { get; set; }

        [JsonProperty("strAccion")]
        public string strAccion { get; set; }

        [JsonProperty("strMotivo")]
        public string strMotivo { get; set; }

        [JsonProperty("strUsuario")]
        public string strUsuario { get; set; }
    }
}