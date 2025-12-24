using System;
using Newtonsoft.Json;

namespace SistemaGestionCGI.Models
{
    public class InvgccGrupoInvestigacion
    {
        [JsonProperty("strId_gru")]
        public string strId_gru { get; set; }

        [JsonProperty("strNombre_gru")]
        public string strNombre_gru { get; set; }

        [JsonProperty("strCoordinador_gru")]
        public string strCoordinador_gru { get; set; }

        [JsonProperty("dtFechacrea_gru")]
        public DateTime dtFechacrea_gru { get; set; }

        [JsonProperty("strCategoria_gru")]
        public string strCategoria_gru { get; set; }

        [JsonProperty("strLineasinv_gru")]
        public string strLineasinv_gru { get; set; }

        [JsonProperty("strSublineasinv_gru")]
        public string strSublineasinv_gru { get; set; }

        [JsonProperty("strArchivo_gru")]
        public string strArchivo_gru { get; set; }

        [JsonProperty("strFoto_gru")]
        public string strFoto_gru { get; set; }

        [JsonProperty("TotalProyectos")]
        public int TotalProyectos { get; set; }

        [JsonProperty("strFacultad_gru")]
        public string strFacultad_gru { get; set; }
    }
}