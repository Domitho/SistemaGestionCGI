using System;
using Newtonsoft.Json;

namespace SistemaGestionCGI.Models
{
    public class InvgccMetricas
    {
        [JsonProperty("anio")]
        public int anio { get; set; }

        [JsonProperty("minConsolidado")]
        public int minConsolidado { get; set; }
    }
}