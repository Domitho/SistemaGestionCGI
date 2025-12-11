using System;
using Newtonsoft.Json;

namespace SistemaGestionCGI.Models
{
    public class InvgccEjecucionInformes
    {
        [JsonProperty("strId_informe")]
        public string strId_informe { get; set; }

        [JsonProperty("fkId_ejec")]
        public string fkId_ejec { get; set; }

        [JsonProperty("strNombrePeriodo")]
        public string strNombrePeriodo { get; set; }

        [JsonProperty("strArchivo_path")]
        public string strArchivo_path { get; set; }

        [JsonProperty("dtFechaSubida")]
        public DateTime dtFechaSubida { get; set; }
    }
}