using System;
using Newtonsoft.Json;

namespace SistemaGestionCGI.Models
{
    public class InvgccGrupoIntegrantes
    {
        [JsonProperty("strId_int")]
        public string strId_int { get; set; }

        [JsonProperty("bitPertenece_int")]
        public bool bitPertenece_int { get; set; }

        [JsonProperty("strCedula_int")]
        public string strCedula_int { get; set; }

        [JsonProperty("strApellidos_int")]
        public string strApellidos_int { get; set; }

        [JsonProperty("strNombres_int")]
        public string strNombres_int { get; set; }

        [JsonProperty("strCarrera_int")]
        public string strCarrera_int { get; set; }

        [JsonProperty("strFuncion_int")]
        public string strFuncion_int { get; set; }

        [JsonProperty("dtFechaini_int")]
        public DateTime dtFechaini_int { get; set; }

        [JsonProperty("dtFechafin_int")]
        public DateTime? dtFechafin_int { get; set; }

        [JsonProperty("strObservacion_int")]
        public string strObservacion_int { get; set; }

        [JsonProperty("fkId_gru")]
        public string fkId_gru { get; set; }

        [JsonProperty("fkId_doc")]
        public string fkId_doc { get; set; }

        [JsonProperty("strCorreo_int")]
        public string strCorreo_int { get; set; }

        [JsonProperty("bitActivo_int")]
        public bool bitActivo_int { get; set; }

        [JsonProperty("strTipo_int")]
        public string strTipo_int { get; set; }

        [JsonProperty("NombreCompleto")]
        public string NombreCompleto { get; set; }
    }
}