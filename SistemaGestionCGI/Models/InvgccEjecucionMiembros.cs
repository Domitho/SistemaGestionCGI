using System;
using Newtonsoft.Json;

namespace SistemaGestionCGI.Models
{
    public class InvgccEjecucionMiembros
    {
        [JsonProperty("strId_miembro")]
        public int strId_miembro { get; set; } // CAMBIO: int

        [JsonProperty("fkId_ejec")]
        public int fkId_ejec { get; set; } // CAMBIO: int

        [JsonProperty("strCedula_miembro")]
        public string strCedula_miembro { get; set; }

        [JsonProperty("strNombres_miembro")]
        public string strNombres_miembro { get; set; }

        [JsonProperty("strApellidos_miembro")]
        public string strApellidos_miembro { get; set; }

        [JsonProperty("strRol_miembro")]
        public string strRol_miembro { get; set; }

        [JsonProperty("bitActivo_miembro")]
        public bool bitActivo_miembro { get; set; }
    }
}