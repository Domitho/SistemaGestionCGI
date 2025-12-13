using System;
using Newtonsoft.Json;

namespace SistemaGestionCGI.Models
{
    public class InvgccEjecucionProyectos
    {
        [JsonProperty("strId_ejec")]
        public string strId_ejec { get; set; }

        [JsonProperty("fkId_pro")]
        public string fkId_pro { get; set; }

        [JsonProperty("TituloProyecto")]
        public string TituloProyecto { get; set; }

        [JsonProperty("strCoordinador_ejec")]
        public string strCoordinador_ejec { get; set; }

        [JsonProperty("strPeriodo_ejec")]
        public string strPeriodo_ejec { get; set; }

        [JsonProperty("dtFechaini_ejec")]
        public DateTime dtFechaini_ejec { get; set; }

        [JsonProperty("dtFechafin_ejec")]
        public DateTime? dtFechafin_ejec { get; set; }

        [JsonProperty("strInforme_ejec")]
        public string strInforme_ejec { get; set; }

        [JsonProperty("strEstado_ejec")]
        public string strEstado_ejec { get; set; }
    }
}