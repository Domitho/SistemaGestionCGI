using System;
using Newtonsoft.Json;

namespace SistemaGestionCGI.Models
{
    [Serializable]
    public class InvgccCentroIntegrantes
    {
        // Identificadores
        [JsonProperty("strId_cin")]
        public string strId_cin { get; set; }

        [JsonProperty("fkId_cen")]
        public string fkId_cen { get; set; } // Relación con el Centro

        // Datos Personales
        [JsonProperty("strCedula_cin")]
        public string strCedula_cin { get; set; }

        [JsonProperty("strNombres_cin")]
        public string strNombres_cin { get; set; }

        [JsonProperty("strApellidos_cin")]
        public string strApellidos_cin { get; set; }

        [JsonProperty("strCorreo_cin")]
        public string strCorreo_cin { get; set; }

        // Datos Institucionales / Cargo
        [JsonProperty("strFuncion_cin")]
        public string strFuncion_cin { get; set; } // Aquí irá "Director", "Investigador", etc.

        [JsonProperty("strTipo_cin")]
        public string strTipo_cin { get; set; } // Interno / Externo

        [JsonProperty("strCarrera_cin")]
        public string strCarrera_cin { get; set; }

        [JsonProperty("strFacultad_cin")]
        public string strFacultad_cin { get; set; }

        [JsonProperty("strEntidad_cin")]
        public string strEntidad_cin { get; set; } // Para externos

        // Auditoría
        [JsonProperty("bitActivo_cin")]
        public bool bitActivo_cin { get; set; }

        // Propiedad extra para mostrar nombre completo en grillas/combos fácilmente
        public string NombreCompleto
        {
            get { return $"{strApellidos_cin} {strNombres_cin}"; }
        }
    }
}