using System;
using System.Collections.Generic;
using SistemaGestionCGI.Models;
using SistemaGestionCGI.Settings;

namespace SistemaGestionCGI.BLL
{
    public class ManejadorDashboard
    {
        private readonly ConnectionSqlServer _dal = ConnectionSqlServer.Instance;

        // 1. KPIs
        public DashboardCountersDTO ObtenerContadoresGenerales()
        {
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

        // 2. Gráfico: Estado de proyectos
        public List<DashboardChartDTO> ObtenerProyectosPorEstado()
        {
            string sql = @"
                SELECT 
                    CASE 
                        WHEN strEstado_ejec LIKE 'En Ejecución' THEN 'EN EJECUCION'
                        WHEN strEstado_ejec LIKE 'En Revisión' THEN 'EN REVISION'
                        WHEN strEstado_ejec LIKE 'Finalizado' THEN 'FINALIZADO'
                        ELSE strEstado_ejec
                    END AS Label,
                    COUNT(*) AS Value
                FROM INVGCCEJECUCION_PROYECTO
                GROUP BY 
                    CASE 
                        WHEN strEstado_ejec LIKE 'En Ejecución' THEN 'EN EJECUCION'
                        WHEN strEstado_ejec LIKE 'En Revisión' THEN 'EN REVISION'
                        WHEN strEstado_ejec LIKE 'Finalizado' THEN 'FINALIZADO'
                        ELSE strEstado_ejec
                    END";

            return _dal.SelectSql<DashboardChartDTO>(sql) ?? new List<DashboardChartDTO>();
        }

        // 3. Gráfico: Docentes por categoría
        public List<DashboardChartDTO> ObtenerDocentesPorCategoria()
        {
            string sql = @"
                SELECT 
                    cat.Nombre AS Label,
                    COUNT(d.strId_doc) AS Value
                FROM INVGCCCATEGORIZACION_DOCENTES d
                INNER JOIN INVGCCCATEGORIAS cat 
                    ON d.strCategorizacion = cat.IdCategoria
                WHERE cat.Estado = 1
                GROUP BY cat.Nombre
                ORDER BY cat.Nombre";

            return _dal.SelectSql<DashboardChartDTO>(sql) ?? new List<DashboardChartDTO>();
        }

        // 4. Detalle docente por categoría (filtrado)
        public List<DocenteDetalleDTO> ObtenerDocentesPorCategoriaDetalle(string categoria)
        {
            string sql = $@"
                SELECT 
                    d.strId_doc AS Id,
                    d.strCedula_doc AS Cedula,
                    d.strNombres_doc AS Nombres,
                    d.strApellidos_doc AS Apellidos,
                    f.Nombre AS Facultad,
                    c.Nombre AS Carrera,
                    d.bitActivo_doc AS Activo,
                    cat.Nombre AS Categoria,
                    d.dtFechaCategorizacion AS FechaCategorizacion,
                    d.strCertificado_doc AS Certificado,
                    d.strCorreo_doc AS Correo
                FROM INVGCCCATEGORIZACION_DOCENTES d
                LEFT JOIN INVGCCFACULTADES f ON d.strFacultad_doc = f.IdFacultad
                LEFT JOIN INVGCCCARRERAS c ON d.strCarrera_doc = c.IdCarrera
                LEFT JOIN INVGCCCATEGORIAS cat ON d.strCategorizacion = cat.IdCategoria
                WHERE cat.Nombre = '{categoria.Replace("'", "''")}'";

            return _dal.SelectSql<DocenteDetalleDTO>(sql) ?? new List<DocenteDetalleDTO>();
        }

        // 5. Detalle completo docentes para modal
        public List<DocenteDetalleDTO> ObtenerDocentesPorCategoriaDetalleTodos()
        {
            string sql = @"
                SELECT 
                    d.strId_doc AS Id,
                    d.strCedula_doc AS Cedula,
                    d.strNombres_doc AS Nombres,
                    d.strApellidos_doc AS Apellidos,
                    f.Nombre AS Facultad,
                    c.Nombre AS Carrera,
                    d.bitActivo_doc AS Activo,
                    cat.Nombre AS Categoria,
                    d.dtFechaCategorizacion AS FechaCategorizacion,
                    d.strCertificado_doc AS Certificado,
                    d.strCorreo_doc AS Correo
                FROM INVGCCCATEGORIZACION_DOCENTES d
                LEFT JOIN INVGCCFACULTADES f ON d.strFacultad_doc = f.IdFacultad
                LEFT JOIN INVGCCCARRERAS c ON d.strCarrera_doc = c.IdCarrera
                LEFT JOIN INVGCCCATEGORIAS cat ON d.strCategorizacion = cat.IdCategoria
                WHERE cat.Estado = 1";

            return _dal.SelectSql<DocenteDetalleDTO>(sql) ?? new List<DocenteDetalleDTO>();
        }

        // 6. Detalle proyectos por estado (para modal)
        public List<ProyectoDetalleDTO> ObtenerProyectosPorEstadoDetalle(string estado)
        {
            string sql = $@"
                SELECT 
                    e.strId_ejec AS Id,
                    e.fkId_pro AS Codigo,
                    COALESCE(p.strTema_pro, 'Nombre no disponible') AS NombreProyecto,
                    e.strCoordinador_ejec AS Coordinador,
                    e.strPeriodo_ejec AS Periodo,
                    e.dtFechaini_ejec AS FechaInicio,
                    e.dtFechafin_ejec AS FechaFin,
                    e.strInforme_ejec AS Informe,
                    e.strEstado_ejec AS Estado
                FROM INVGCCEJECUCION_PROYECTO e
                LEFT JOIN INVGCCINSCRIPCION_PROYECTOS p
                    ON UPPER(LTRIM(RTRIM(e.fkId_pro))) = UPPER(LTRIM(RTRIM(p.strId_pro)))
                WHERE e.strEstado_ejec LIKE '{estado.Replace("'", "''")}'";

            return _dal.SelectSql<ProyectoDetalleDTO>(sql) ?? new List<ProyectoDetalleDTO>();
        }

        // 7. Detalle completo proyectos para modal
        public List<ProyectoDetalleDTO> ObtenerProyectosDetalleTodos()
        {
            string sql = @"
                SELECT 
                    strId_ejec AS Id,
                    fkId_pro AS Codigo,
                    strCoordinador_ejec AS Coordinador,
                    strPeriodo_ejec AS Periodo,
                    dtFechaini_ejec AS FechaInicio,
                    dtFechafin_ejec AS FechaFin,
                    strInforme_ejec AS Informe,
                    strEstado_ejec AS Estado
                FROM INVGCCEJECUCION_PROYECTO";

            return _dal.SelectSql<ProyectoDetalleDTO>(sql) ?? new List<ProyectoDetalleDTO>();
        }
    }
}