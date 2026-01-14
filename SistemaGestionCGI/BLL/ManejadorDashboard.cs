using System;
using System.Collections.Generic;
using System.Linq; // Solo para .Sum() ligero
using SistemaGestionCGI.Models;
using SistemaGestionCGI.Settings;

namespace SistemaGestionCGI.BLL
{
    public class ManejadorDashboard
    {
        private readonly ConnectionSqlServer _dal = ConnectionSqlServer.Instance;

        // 1. OBTENER CONTADORES (Optimizada: 1 sola consulta trae todo)
        public DashboardCountersDTO ObtenerContadoresGenerales()
        {
            // Nota: Contamos (*) sin filtrar activos/inactivos según tu requerimiento #4
            string sql = @"
                SELECT 
                    (SELECT COUNT(*) FROM INVGCCCENTRO_INVESTIGACION) AS TotalCentros,
                    (SELECT COUNT(*) FROM INVGCCCENTRO_INVESTIGACION_INTEGRANTES) AS TotalIntegrantesCentros,
                    (SELECT COUNT(*) FROM INVGCCCONVOCATORIA_GRUPOS_INVESTIGACION) AS TotalConvocatorias,
                    (SELECT COUNT(*) FROM INVGCCGRUPO_INVESTIGACION) AS TotalGrupos,
                    (SELECT COUNT(*) FROM INVGCCGRUPO_INTEGRANTES) AS TotalIntegrantesGrupos,
                    (SELECT COUNT(*) FROM INVGCCCATEGORIZACION_DOCENTES) AS TotalDocentes";

            var resultado = _dal.SelectSql<DashboardCountersDTO>(sql);
            return resultado != null && resultado.Count > 0 ? resultado[0] : new DashboardCountersDTO();
        }

        // 2. GRÁFICO: PROYECTOS POR ESTADO (Ejecución, Revisión, Finalizado)
        public List<DashboardChartDTO> ObtenerProyectosPorEstado()
        {
            // Filtramos solo los estados que te interesan
            string sql = @"
                SELECT strEstado_ejec as Label, COUNT(*) as Value
                FROM INVGCCEJECUCION_PROYECTO
                WHERE strEstado_ejec IN ('EN EJECUCION', 'EN REVISION', 'FINALIZADO')
                GROUP BY strEstado_ejec";

            return _dal.SelectSql<DashboardChartDTO>(sql) ?? new List<DashboardChartDTO>();
        }

        // 3. GRÁFICO: DOCENTES POR CATEGORÍA
        public List<DashboardChartDTO> ObtenerDocentesPorCategoria()
        {
            string sql = @"
                SELECT strCategorizacion as Label, COUNT(*) as Value
                FROM INVGCCCATEGORIZACION_DOCENTES
                GROUP BY strCategorizacion";

            return _dal.SelectSql<DashboardChartDTO>(sql) ?? new List<DashboardChartDTO>();
        }
    }
}