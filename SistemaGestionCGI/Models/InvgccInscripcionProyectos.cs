using System;
using Newtonsoft.Json;

namespace SistemaGestionCGI.Models
{
    public class InvgccInscripcionProyectos
    {
        [JsonProperty("strId_pro")]
        public string strId_pro { get; set; }

        [JsonProperty("strTema_pro")]
        public string strTema_pro { get; set; }

        [JsonProperty("strDuracion_pro")]
        public string strDuracion_pro { get; set; }

        [JsonProperty("dtFehains_pro")]
        public DateTime dtFehains_pro { get; set; }

        [JsonProperty("strArchivo_pro")]
        public string strArchivo_pro { get; set; }

        [JsonProperty("strEstado_pro")]
        public string strEstado_pro { get; set; }

        [JsonProperty("fkId_conv")]
        public string fkId_conv { get; set; }

        [JsonProperty("fkId_gru")]
        public string fkId_gru { get; set; }

        [JsonProperty("strCoordinador_pro")]
        public string strCoordinador_pro { get; set; } 

        [JsonProperty("intPuntaje_pro")]
        public int? intPuntaje_pro { get; set; }

        [JsonProperty("strNombre_gru")]
        public string strNombre_gru { get; set; }

        [JsonProperty("strNombre_conv")]
        public string strNombre_conv { get; set; }
    }
}