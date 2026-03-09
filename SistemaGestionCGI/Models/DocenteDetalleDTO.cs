using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaGestionCGI.Models
{
    public class DocenteDetalleDTO
    {
        public string Id { get; set; }                // strId_doc
        public string Cedula { get; set; }            // strCedula_doc
        public string Nombres { get; set; }           // strNombres_doc
        public string Apellidos { get; set; }         // strApellidos_doc
        public string Facultad { get; set; }          // strFacultad_doc
        public string Carrera { get; set; }           // strCarrera_doc
        public bool Activo { get; set; }              // bitActivo_doc
        public string Categoria { get; set; }         // strCategorizacion
        public DateTime FechaCategorizacion { get; set; } // dtFechaCategorizacion
        public string Certificado { get; set; }       // strCertificado_doc
        public string Correo { get; set; }            // strCorreo_doc
    }
}