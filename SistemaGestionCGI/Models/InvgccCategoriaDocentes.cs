using System;
using Newtonsoft.Json;

namespace SistemaGestionCGI.Models
{
    public class InvgccCategoriaDocentes
    {
        // --- CAMPOS DE LA TABLA INVGCCCATEGORIA ---

        [JsonProperty("strId_cat")]
        public string strId_cat { get; set; }

        [JsonProperty("dtFecha_cat")]
        public DateTime dtFecha_cat { get; set; }

        [JsonProperty("strCategorizacion")]
        public string strCategorizacion { get; set; }

        [JsonProperty("fkId_doc")]
        public string fkId_doc { get; set; }

        [JsonProperty("strEstado_cat")]
        public string strEstado_cat { get; set; }

        [JsonProperty("strFechaBorrar_cat")]
        public string strFechaBorrar_cat { get; set; }

        // --- PROPIEDADES EXTENDIDAS (JOIN CON INVGCCDOCENTE) ---
        // Estas propiedades permiten que la grilla muestre nombres sin consultas extra.

        [JsonProperty("strId_doc")]
        public string strId_doc { get; set; }

        [JsonProperty("strCedula_doc")]
        public string strCedula_doc { get; set; }

        [JsonProperty("strApellidos_doc")]
        public string strApellidos_doc { get; set; }

        [JsonProperty("strNombres_doc")]
        public string strNombres_doc { get; set; }

        [JsonProperty("strFuncion_doc")]
        public string strFuncion_doc { get; set; }

        [JsonProperty("strFacultad_doc")]
        public string strFacultad_doc { get; set; }

        [JsonProperty("strCarrera_doc")]
        public string strCarrera_doc { get; set; }
    }
}