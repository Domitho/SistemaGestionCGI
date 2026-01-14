namespace SistemaGestionCGI.Models
{
    // DTO para las Tarjetas (KPIs)
    public class DashboardCountersDTO
    {
        public int TotalCentros { get; set; }
        public int TotalIntegrantesCentros { get; set; }
        public int TotalConvocatorias { get; set; }
        public int TotalGrupos { get; set; }
        public int TotalIntegrantesGrupos { get; set; }
        public int TotalDocentes { get; set; } // Nuevo requerimiento
    }

    // DTO para Gráficos (Reutilizado)
    public class DashboardChartDTO
    {
        public string Label { get; set; }
        public int Value { get; set; }
    }
}