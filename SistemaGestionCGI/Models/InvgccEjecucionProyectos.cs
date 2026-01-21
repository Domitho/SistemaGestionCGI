using System;
using Newtonsoft.Json;

namespace SistemaGestionCGI.Models
{
    public class InvgccEjecucionProyectos
    {
        [JsonProperty("strId_ejec")]
        public int strId_ejec { get; set; } 

        [JsonProperty("fkId_pro")]
        public string fkId_pro { get; set; } 

        [JsonProperty("TituloProyecto")]
        public string TituloProyecto { get; set; }

        [JsonProperty("strCoordinador_ejec")]
        public string strCoordinador_ejec { get; set; }

        [JsonProperty("strCedulaCoordinador_ejec")]
        public string strCedulaCoordinador_ejec { get; set; }


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

        [JsonProperty("strInforme_Cierre")]
        public string strInforme_Cierre { get; set; }

        [JsonProperty("strInforme_Final")]
        public string strInforme_Final { get; set; }

        [JsonProperty("CantidadInformes")]
        public int CantidadInformes { get; set; }

    }
}