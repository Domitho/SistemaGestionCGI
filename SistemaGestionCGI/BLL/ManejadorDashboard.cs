using System;
using System.Collections.Generic;
using System.Linq;
using SistemaGestionCGI.Models;
using SistemaGestionCGI.Settings;

namespace SistemaGestionCGI.BLL
{
    public class ManejadorDashboard
    {
        private readonly ConnectionSqlServer _dal = ConnectionSqlServer.Instance;

        // 1. KPI - CONTEOS GENERALES
        public InvgccDashboardKPI ObtenerKPIs()
        {
            var kpi = new InvgccDashboardKPI();

            // Usamos nombres de PK reales según tu reporte SQL
            kpi.Centros = ContarRegistros("INVGCCCENTRO_INESTIGACION", "strId_cent"); // Asumiendo PK estándar
            kpi.Convocatorias = ContarRegistros("INVGCCCONVOCATORI", "strId_conv"); // Asumiendo PK estándar
            kpi.Grupos = ContarRegistros("INVGCCGRUPO_INVESTIGACION", "strId_gru");
            kpi.Integrantes = ContarRegistros("INVGCCEJECUCION_MIEMBROS", "strId_miembro"); // Tu SQL dice strId_miembro (int)

            return kpi;
        }

        private int ContarRegistros(string tabla, string campoId)
        {
            try
            {
                // Select simple para contar filas
                string sql = $"SELECT {campoId} FROM {tabla}";
                var lista = _dal.SelectSql<object>(sql);
                return lista != null ? lista.Count : 0;
            }
            catch { return 0; }
        }

        // 2. GRÁFICOS

        public List<InvgccDashboardChart> ObtenerDocentesPorCategoria()
        {
            string sql = "SELECT strCategorizacion FROM INVGCCCATEGORIA";
            var lista = _dal.SelectSql<InvgccCategoriaMap>(sql);

            if (lista == null) return new List<InvgccDashboardChart>();

            string[] categoriasInteres = { "PRINCIPAL 1", "PRINCIPAL 2", "PRINCIPAL 3", "AGREGADO 1", "AGREGADO 2", "AUXILIAR 1" };

            return lista
                .Where(x => !string.IsNullOrEmpty(x.strCategorizacion) && categoriasInteres.Contains(x.strCategorizacion.ToUpper()))
                .GroupBy(x => x.strCategorizacion)
                .Select(g => new InvgccDashboardChart { Label = g.Key, Value = g.Count() })
                .ToList();
        }

        public List<InvgccDashboardChart> ObtenerProyectosPorEstado()
        {
            string sql = "SELECT strEstado_pro FROM INVGCCINSCRIPCION_PROYECTOS";
            var lista = _dal.SelectSql<InvgccProyectoMap>(sql);

            if (lista == null) return new List<InvgccDashboardChart>();

            string[] estadosInteres = { "APROBADO", "PENDIENTE", "NO APROBADO" };

            return lista
                .Where(x => !string.IsNullOrEmpty(x.strEstado_pro))
                .Select(x => x.strEstado_pro.Trim().ToUpper()) // Normalizamos (quita espacios y mayúsculas)
                .Where(x => estadosInteres.Contains(x))
                .GroupBy(x => x)
                .Select(g => new InvgccDashboardChart { Label = g.Key, Value = g.Count() })
                .ToList();
        }

        public List<InvgccDashboardChart> ObtenerPublicacionesPorTipo()
        {
            // CORREGIDO: Usamos la columna real strTipo_publi
            string sql = "SELECT strTipo_publi FROM INVGCCPUBLICACION";
            var lista = _dal.SelectSql<InvgccPublicacionMap>(sql);

            if (lista == null) return new List<InvgccDashboardChart>();

            string[] tiposInteres = { "LIBRO", "CAPITULO DE LIBRO", "REVISTA", "MEMORIA" };

            return lista
                .Where(x => !string.IsNullOrEmpty(x.strTipo_publi))
                .Select(x => x.strTipo_publi.Trim().ToUpper()) // Normalizamos
                .Where(x => tiposInteres.Contains(x))
                .GroupBy(x => x)
                .Select(g => new InvgccDashboardChart { Label = g.Key, Value = g.Count() })
                .ToList();
        }
    }
}