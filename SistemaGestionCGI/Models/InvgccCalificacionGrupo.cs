using System;
using Newtonsoft.Json;

namespace SistemaGestionCGI.Models
{
    public class InvgccCalificacionGrupo
    {
        [JsonProperty("strId_valo")]
        public int strId_valo { get; set; } 

        [JsonProperty("fkId_gru")]
        public string fkId_gru { get; set; }

        [JsonProperty("intPuntaje_valo")]
        public int intPuntaje_valo { get; set; }

        [JsonProperty("strCategoria_valo")]
        public string strCategoria_valo { get; set; }

        [JsonProperty("dtFecha_valo")]
        public DateTime dtFecha_valo { get; set; }

        [JsonProperty("strReconocimiento_valo")]
        public string strReconocimiento_valo { get; set; }

        [JsonProperty("strInforme_valo")]
        public string strInforme_valo { get; set; }

        [JsonProperty("intAnioMetrica")]
        public int intAnioMetrica { get; set; }

        // Propiedad extendida para mostrar el nombre del grupo en la grilla (JOIN)
        public string NombreGrupo { get; set; }
    }
}