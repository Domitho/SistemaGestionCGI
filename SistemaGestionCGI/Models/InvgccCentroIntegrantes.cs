using System;
using Newtonsoft.Json;

namespace SistemaGestionCGI.Models
{
    [Serializable]
    public class InvgccCentroIntegrantes
    {
        [JsonProperty("strId_cin")]
        public string strId_cin { get; set; }

        [JsonProperty("fkId_cen")]
        public string fkId_cen { get; set; }

        [JsonProperty("strCedula_cin")]
        public string strCedula_cin { get; set; }

        [JsonProperty("strNombres_cin")]
        public string strNombres_cin { get; set; }

        [JsonProperty("strApellidos_cin")]
        public string strApellidos_cin { get; set; }

        [JsonProperty("strCorreo_cin")]
        public string strCorreo_cin { get; set; }

        [JsonProperty("strFuncion_cin")]
        public string strFuncion_cin { get; set; }

        [JsonProperty("strTipo_cin")]
        public string strTipo_cin { get; set; } 

        [JsonProperty("strCarrera_cin")]
        public string strCarrera_cin { get; set; }

        [JsonProperty("strFacultad_cin")]
        public string strFacultad_cin { get; set; }

        [JsonProperty("strEntidad_cin")]
        public string strEntidad_cin { get; set; } 

        [JsonProperty("bitActivo_cin")]
        public bool bitActivo_cin { get; set; }

        public string NombreCompleto
        {
            get { return $"{strApellidos_cin} {strNombres_cin}"; }
        }
    }
}