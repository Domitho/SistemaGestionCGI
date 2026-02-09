using System;
using Newtonsoft.Json;

namespace SistemaGestionCGI.Models
{
    public class InvgccEjecucionInformes
    {
        [JsonProperty("strId_informe")]
        public int strId_informe { get; set; }

        [JsonProperty("fkId_ejec")]
        public int fkId_ejec { get; set; } 

        [JsonProperty("strNombrePeriodo")]
        public string strNombrePeriodo { get; set; }

        [JsonProperty("strArchivo_path")]
        public string strArchivo_path { get; set; }

        [JsonProperty("dtFechaSubida")]
        public DateTime dtFechaSubida { get; set; }

        [JsonProperty("strCiclo_informe")]
        public string strCiclo_informe { get; set; }

        [JsonProperty("strObservacion_informe")]
        public string strObservacion_informe { get; set; }

        [JsonProperty("dtFechaLectura_informe")]
        public DateTime? dtFechaLectura_informe { get; set; }
    }
}