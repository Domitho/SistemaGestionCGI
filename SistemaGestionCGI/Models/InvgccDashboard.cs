using System;
using Newtonsoft.Json;

namespace SistemaGestionCGI.Models
{
    // 1. DTOs PARA LA VISTA (No tocan la BD directamente)
    public class InvgccDashboardKPI
    {
        public int Centros { get; set; }
        public int Convocatorias { get; set; }
        public int Grupos { get; set; }
        public int Integrantes { get; set; }
    }

    public class InvgccDashboardChart
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("value")]
        public int Value { get; set; }
    }

    // 2. MAPEOS DE BASE DE DATOS (Ajustados a tu SQL real)

    public class InvgccCategoriaMap
    {
        [JsonProperty("strCategorizacion")]
        public string strCategorizacion { get; set; }
    }

    public class InvgccProyectoMap
    {
        [JsonProperty("strEstado_pro")]
        public string strEstado_pro { get; set; }
    }

    public class InvgccPublicacionMap
    {
        // CORREGIDO: En tu SQL sale como 'strTipo_publi'
        [JsonProperty("strTipo_publi")]
        public string strTipo_publi { get; set; }
    }
}