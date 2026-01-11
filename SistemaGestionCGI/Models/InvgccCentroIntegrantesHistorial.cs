using System;
using Newtonsoft.Json;

namespace SistemaGestionCGI.Models
{
    public class InvgccCentroIntegrantesHistorial
    {
        [JsonProperty("strId_his")]
        public string strId_his { get; set; }

        [JsonProperty("strId_cin")]
        public string strId_cin { get; set; }

        [JsonProperty("dtFecha")]
        public DateTime dtFecha { get; set; }

        [JsonProperty("strAccion")]
        public string strAccion { get; set; } // Valores: NUEVO, EDICIÓN, BAJA, REACTIVAR

        [JsonProperty("strMotivo")]
        public string strMotivo { get; set; }

        [JsonProperty("strUsuario")]
        public string strUsuario { get; set; }
    }
}