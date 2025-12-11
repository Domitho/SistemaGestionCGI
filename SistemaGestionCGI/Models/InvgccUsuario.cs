using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace SistemaGestionCGI.Models
{
    public class InvgccUsuario
    {
        [JsonProperty("UserID")]
        public int intId_usu { get; set; }

        [JsonProperty("Username")]
        public string strNombre_usu { get; set; }

        [JsonProperty("Password")]
        public string strClave_usu { get; set; }

        [JsonProperty("Role")]
        public string strRol_usu { get; set; }

        [JsonProperty("IsActive")]
        public bool bActivo_usu { get; set; }
    }
}