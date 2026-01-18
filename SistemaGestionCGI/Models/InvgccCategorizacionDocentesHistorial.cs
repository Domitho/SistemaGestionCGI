using System;
using Newtonsoft.Json;

namespace SistemaGestionCGI.Models
{
    public class InvgccCategorizacionDocentesHistorial
    {
        [JsonProperty("intId_hist")]
        public int intId_hist { get; set; }

        [JsonProperty("fkId_doc")]
        public string fkId_doc { get; set; }

        [JsonProperty("dtFecha")]
        public DateTime dtFecha { get; set; }

        [JsonProperty("strAccion")]
        public string strAccion { get; set; }

        [JsonProperty("strValorAnterior")]
        public string strValorAnterior { get; set; }

        [JsonProperty("strValorNuevo")]
        public string strValorNuevo { get; set; }

        [JsonProperty("strMotivo")]
        public string strMotivo { get; set; }

        [JsonProperty("strUsuario")]
        public string strUsuario { get; set; }
    }
}