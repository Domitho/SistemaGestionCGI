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

        // =============================================================
        // 1. KPI - CONTEOS GENERALES
        // =============================================================

        public InvgccDashboardKPI ObtenerKPIs()
        {
            return new InvgccDashboardKPI
            {
                Centros = ContarRegistros("INVGCCCENTRO_INESTIGACION", "strId_cent"),
                Convocatorias = ContarRegistros("INVGCCCONVOCATORI", "strId_conv"),
                Grupos = ContarRegistros("INVGCCGRUPO_INVESTIGACION", "strId_gru"),
                Integrantes = ContarRegistros("INVGCCEJECUCION_MIEMBROS", "strId_miembro")
            };
        }

        private int ContarRegistros(string tabla, string campoId)
        {
            try
            {
                string sql = $"SELECT {campoId} FROM {tabla}";
                var lista = _dal.SelectSql<object>(sql);
                return lista?.Count ?? 0;
            }
            catch { return 0; }
        }

        // =============================================================
        // 2. GRÁFICOS (Chart.js Data)
        // =============================================================

        public List<InvgccDashboardChart> ObtenerDocentesPorCategoria()
        {
            string sql = "SELECT strCategorizacion FROM INVGCCCATEGORIA";
            var lista = _dal.SelectSql<InvgccCategoriaMap>(sql);

            if (lista == null) return new List<InvgccDashboardChart>();

            var categoriasInteres = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "PRINCIPAL 1", "PRINCIPAL 2", "PRINCIPAL 3",
                "AGREGADO 1", "AGREGADO 2", "AUXILIAR 1"
            };

            return lista
                .Where(x => !string.IsNullOrEmpty(x.strCategorizacion) && categoriasInteres.Contains(x.strCategorizacion))
                .GroupBy(x => x.strCategorizacion.ToUpper())
                .Select(g => new InvgccDashboardChart { Label = g.Key, Value = g.Count() })
                .ToList();
        }

        public List<InvgccDashboardChart> ObtenerProyectosPorEstado()
        {
            string sql = "SELECT strEstado_pro FROM INVGCCINSCRIPCION_PROYECTOS";
            var lista = _dal.SelectSql<InvgccProyectoMap>(sql);

            if (lista == null) return new List<InvgccDashboardChart>();

            var estadosInteres = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "APROBADO", "PENDIENTE", "NO APROBADO"
            };

            return lista
                .Where(x => !string.IsNullOrEmpty(x.strEstado_pro))
                .Select(x => x.strEstado_pro.Trim())
                .Where(estado => estadosInteres.Contains(estado))
                .GroupBy(estado => estado.ToUpper())
                .Select(g => new InvgccDashboardChart { Label = g.Key, Value = g.Count() })
                .ToList();
        }

        public List<InvgccDashboardChart> ObtenerPublicacionesPorTipo()
        {
            string sql = "SELECT strTipo_publi FROM INVGCCPUBLICACION";
            var lista = _dal.SelectSql<InvgccPublicacionMap>(sql);

            if (lista == null) return new List<InvgccDashboardChart>();

            var tiposInteres = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "LIBRO", "CAPITULO DE LIBRO", "REVISTA", "MEMORIA"
            };

            return lista
                .Where(x => !string.IsNullOrEmpty(x.strTipo_publi))
                .Select(x => x.strTipo_publi.Trim())
                .Where(tipo => tiposInteres.Contains(tipo))
                .GroupBy(tipo => tipo.ToUpper())
                .Select(g => new InvgccDashboardChart { Label = g.Key, Value = g.Count() })
                .ToList();
        }
    }
}