using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestionCGI.Models
{
    public class CertificadoGrupoDTO
    {
        public string IdIntegrante { get; set; }
        public string Cedula { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }

        public string Modulo { get; set; }
        public string IdGrupo { get; set; }
        public string NombreGrupo { get; set; }

        public string Funcion { get; set; }
        public string Estado { get; set; }

        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }

        public string Certificado { get; set; }

    }
}