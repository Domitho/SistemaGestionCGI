using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SistemaGestionCGI.Models;
using SistemaGestionCGI.Settings;

namespace SistemaGestionCGI.BLL
{
    public class ManejadorCalificacionGrupo
    {
        private readonly ConnectionSqlServer _dal = ConnectionSqlServer.Instance;

        // ======================= CALIFICACIONES =======================

        public List<InvgccCalificacionGrupo> ObtenerCalificaciones(int anioFiltro = 0)
        {
            string sql = @"
                SELECT c.*, g.strNombre_gru as NombreGrupo 
                FROM INVGCCCALIFICACION_GRUPO c
                INNER JOIN INVGCCGRUPO_INVESTIGACION g ON c.fkId_gru = g.strId_gru";

            if (anioFiltro > 0)
                sql += $" WHERE YEAR(c.dtFecha_valo) = {anioFiltro}";

            sql += " ORDER BY c.dtFecha_valo DESC";

            return _dal.SelectSql<InvgccCalificacionGrupo>(sql);
        }

        public InvgccCalificacionGrupo ObtenerPorId(string id)
        {
            string sql = $"SELECT * FROM INVGCCCALIFICACION_GRUPO WHERE strId_valo = '{id}'";
            var lista = _dal.SelectSql<InvgccCalificacionGrupo>(sql);
            return lista?.FirstOrDefault();
        }

        public void GuardarCalificacion(InvgccCalificacionGrupo obj)
        {
            obj.strId_valo = GenerarCodigoAlfanumerico("INVGCCCALIFICACION_GRUPO", "strId_valo", "VAL");
            _dal.Insert("INVGCCCALIFICACION_GRUPO", obj);

            // Sincronización de Categoría
            string sqlUpdateGrupo = $@"
                UPDATE INVGCCGRUPO_INVESTIGACION 
                SET strCategoria_gru = '{obj.strCategoria_valo}' 
                WHERE strId_gru = '{obj.fkId_gru}'";

            _dal.UpdateSql(sqlUpdateGrupo);
        }

        public void EliminarCalificacion(string id) =>
            _dal.Delete("INVGCCCALIFICACION_GRUPO", $"strId_valo = '{id}'");

        // ======================= MÉTRICAS =======================

        public int ObtenerMinimoConsolidado(int anio)
        {
            string sql = $"SELECT minConsolidado FROM INVGCC_METRICAS WHERE anio = {anio}";
            var res = _dal.SelectSql<dynamic>(sql)?.FirstOrDefault();

            if (res != null)
            {
                try { return (int)((dynamic)res).minConsolidado; } catch { }
            }
            return 70;
        }

        public void GuardarMetrica(InvgccMetricas metrica)
        {
            string check = $"SELECT COUNT(*) as conteo FROM INVGCC_METRICAS WHERE anio = {metrica.anio}";
            var res = _dal.SelectSql<dynamic>(check)?.FirstOrDefault();

            int count = 0;
            if (res != null) { try { count = (int)((dynamic)res).conteo; } catch { } }

            if (count > 0)
            {
                string sqlUpdate = $"UPDATE INVGCC_METRICAS SET minConsolidado = {metrica.minConsolidado} WHERE anio = {metrica.anio}";
                _dal.UpdateSql(sqlUpdate);
            }
            else
            {
                _dal.Insert("INVGCC_METRICAS", metrica);
            }
        }

        // ======================= UTILIDADES =======================

        public List<InvgccGrupoInvestigacion> ObtenerGruposParaCombo(int anio)
        {
            string sql = $@"
                SELECT strId_gru, strNombre_gru 
                FROM INVGCCGRUPO_INVESTIGACION 
                WHERE strId_gru NOT IN (
                    SELECT DISTINCT fkId_gru 
                    FROM INVGCCCALIFICACION_GRUPO 
                    WHERE intAnioMetrica = {anio}
                )
                ORDER BY strNombre_gru";

            return _dal.SelectSql<InvgccGrupoInvestigacion>(sql);
        }

        public List<int> ObtenerAniosDisponibles()
        {
            string sql = "SELECT DISTINCT YEAR(dtFecha_valo) as Anio FROM INVGCCCALIFICACION_GRUPO ORDER BY Anio DESC";
            var lista = _dal.SelectSql<dynamic>(sql);

            var anios = new List<int>();
            if (lista != null)
            {
                foreach (var item in lista)
                {
                    try { anios.Add((int)((dynamic)item).Anio); } catch { }
                }
            }
            return anios;
        }

        public List<int> ObtenerAniosConMetricasConfiguradas()
        {
            string sql = "SELECT DISTINCT anio FROM INVGCC_METRICAS ORDER BY anio DESC";
            var lista = _dal.SelectSql<dynamic>(sql);

            var anios = new List<int>();
            if (lista != null)
            {
                foreach (var item in lista)
                {
                    try { anios.Add((int)((dynamic)item).anio); } catch { }
                }
            }
            return anios;
        }

        private string GenerarCodigoAlfanumerico(string tabla, string campoId, string prefijo)
        {
            try
            {
                string sql = $"SELECT {campoId} FROM {tabla}";
                var lista = _dal.SelectSql<dynamic>(sql);

                if (lista == null || lista.Count == 0) return $"{prefijo}-1";

                int max = 0;
                foreach (var item in lista)
                {
                    string val = "";
                    if (item is JObject jobj) val = jobj[campoId]?.ToString();
                    else try { val = ((dynamic)item)[campoId].ToString(); } catch { continue; }

                    if (!string.IsNullOrEmpty(val) && val.StartsWith(prefijo))
                    {
                        string numStr = val.Substring(prefijo.Length).Replace("-", "");
                        if (int.TryParse(numStr, out int n))
                        {
                            if (n > max) max = n;
                        }
                    }
                }
                return $"{prefijo}-{max + 1}";
            }
            catch
            {
                return $"{prefijo}-{DateTime.Now.Ticks.ToString().Substring(10)}";
            }
        }
    }
}