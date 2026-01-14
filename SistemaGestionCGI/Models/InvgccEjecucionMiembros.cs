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

        [JsonProperty("strFacultad_miembro")]
        public string strFacultad_miembro { get; set; }

        [JsonProperty("bitActivo_miembro")]
        public bool bitActivo_miembro { get; set; }

        [JsonProperty("strCorreo_miembro")]
        public string strCorreo_miembro { get; set; }

        [JsonProperty("strCarrera_miembro")]
        public string strCarrera_miembro { get; set; }

        [JsonProperty("strTipo_miembro")]
        public string strTipo_miembro { get; set; }

        [JsonProperty("strEntidad_miembro")]
        public string strEntidad_miembro { get; set; }

        [JsonProperty("dtFechaInicio_miembro")]
        public DateTime? dtFechaInicio_miembro { get; set; }

        [JsonProperty("dtFechaFin_miembro")]
        public DateTime? dtFechaFin_miembro { get; set; }
    }
}