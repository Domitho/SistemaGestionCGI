using System;
using Newtonsoft.Json;

namespace SistemaGestionCGI.Models
{
    public class InvgccEjecucionInformes
    {
        [JsonProperty("strId_informe")]
        public int strId_informe { get; set; } // CAMBIO: int

        [JsonProperty("fkId_ejec")]
        public int fkId_ejec { get; set; } // CAMBIO: int

        [JsonProperty("strNombrePeriodo")]
        public string strNombrePeriodo { get; set; }

        [JsonProperty("strArchivo_path")]
        public string strArchivo_path { get; set; }

        [JsonProperty("dtFechaSubida")]
        public DateTime dtFechaSubida { get; set; }
    }
}