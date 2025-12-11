using System;
using Newtonsoft.Json;

namespace SistemaGestionCGI.Models
{
    public class InvgccConvocatoria
    {
        [JsonProperty("strId_conv")]
        public string strId_conv { get; set; }

        [JsonProperty("strNombre_conv")]
        public string strNombre_conv { get; set; }
    }
}