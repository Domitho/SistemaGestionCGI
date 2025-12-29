using System;
using Newtonsoft.Json;

namespace SistemaGestionCGI.Models
{
    public class InvgccCategorizacionDocentes
    {
        // ==========================================
        // DATOS DEL DOCENTE (IDENTIFICACIÓN)
        // ==========================================
        [JsonProperty("strId_doc")]
        public string strId_doc { get; set; }

        [JsonProperty("strCedula_doc")]
        public string strCedula_doc { get; set; }

        [JsonProperty("strNombres_doc")]
        public string strNombres_doc { get; set; }

        [JsonProperty("strApellidos_doc")]
        public string strApellidos_doc { get; set; }

        [JsonProperty("strFacultad_doc")]
        public string strFacultad_doc { get; set; }

        [JsonProperty("strCarrera_doc")]
        public string strCarrera_doc { get; set; }

        [JsonProperty("bitActivo_doc")]
        public bool bitActivo_doc { get; set; }

        // ==========================================
        // DATOS DE LA CATEGORÍA (UNIFICADOS)
        // ==========================================
        [JsonProperty("strCategorizacion")]
        public string strCategorizacion { get; set; }

        [JsonProperty("dtFechaCategorizacion")]
        public DateTime? dtFechaCategorizacion { get; set; }

        // ==========================================
        // PROPIEDADES AUXILIARES (NO ESTÁN EN TABLA)
        // Útiles para mostrar en Grillas o Combos
        // ==========================================
        [JsonProperty("NombreCompleto")]
        public string NombreCompleto { get; set; }
    }
}