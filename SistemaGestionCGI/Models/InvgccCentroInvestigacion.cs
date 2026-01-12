using System;
using Newtonsoft.Json; 

namespace SistemaGestionCGI.Models
{
    public class InvgccCentroInvestigacion
    {
        // Identificadores
        [JsonProperty("strId_cen")]
        public string strId_cen { get; set; }

        [JsonProperty("strNombre_cen")]
        public string strNombre_cen { get; set; }

        // Campos Específicos
        [JsonProperty("strFacultad_cen")]
        public string strFacultad_cen { get; set; }

        [JsonProperty("strArea_cen")]
        public string strArea_cen { get; set; }

        [JsonProperty("strUbicacion_cen")]
        public string strUbicacion_cen { get; set; }

        [JsonProperty("strLineaInv_cen")]
        public string strLineaInv_cen { get; set; }

        // Información Estratégica
        [JsonProperty("strMision_cen")]
        public string strMision_cen { get; set; }

        [JsonProperty("strVision_cen")]
        public string strVision_cen { get; set; }

        // Fechas y Estado
        [JsonProperty("dtFechaAprobacion_cen")]
        public DateTime dtFechaAprobacion_cen { get; set; }

        [JsonProperty("dtFechaRegistro")]
        public DateTime dtFechaRegistro { get; set; }

        [JsonProperty("fkId_director")]
        public string fkId_director { get; set; }

        [JsonProperty("NombreDirector")]
        public string NombreDirector { get; set; }

        [JsonProperty("bitActivo_cen")]
        public bool bitActivo_cen { get; set; }
    }
}