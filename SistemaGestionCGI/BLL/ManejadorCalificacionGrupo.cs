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
        // Instancia del DAL
        private readonly ConnectionSqlServer _dal = ConnectionSqlServer.Instance;

        // =============================================================
        // 1. GESTIÓN DE CALIFICACIONES (CRUD)
        // =============================================================

        public List<InvgccCalificacionGrupo> ObtenerCalificaciones(int anioFiltro = 0)
        {
            string sql = @"
                SELECT c.*, g.strNombre_gru as NombreGrupo
                FROM INVGCCCALIFICACION_GRUPO c
                INNER JOIN INVGCCGRUPO_INVESTIGACION g ON c.fkId_gru = g.strId_gru";

            if (anioFiltro > 0)
            {
                sql += $" WHERE YEAR(c.dtFecha_valo) = {anioFiltro}";
            }

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

            string sqlInsert = $@"
                INSERT INTO INVGCCCALIFICACION_GRUPO
                (strId_valo, fkId_gru, dtFecha_valo, intPuntaje_valo, 
                 strReconocimiento_valo, strInforme_valo, intAnioMetrica, strCategoria_valo)
                VALUES
                ('{obj.strId_valo}', '{obj.fkId_gru}', '{obj.dtFecha_valo:yyyy-MM-dd}', {obj.intPuntaje_valo},
                 '{obj.strReconocimiento_valo}', '{obj.strInforme_valo}', {obj.intAnioMetrica}, '{obj.strCategoria_valo}')";

            _dal.InsertSql(sqlInsert);

            string sqlUpdateGrupo = $@"
                UPDATE INVGCCGRUPO_INVESTIGACION 
                SET strCategoria_gru = '{obj.strCategoria_valo}' 
                WHERE strId_gru = '{obj.fkId_gru}'";

            _dal.UpdateSql(sqlUpdateGrupo);
        }

        public void EliminarCalificacion(string id)
        {
            string sql = $"DELETE FROM INVGCCCALIFICACION_GRUPO WHERE strId_valo = '{id}'";
            _dal.DeleteSql(sql);
        }

        // =============================================================
        // 2. GESTIÓN DE MÉTRICAS (CONFIGURACIÓN)
        // =============================================================

        public int ObtenerMinimoConsolidado(int anio)
        {
            string sql = $"SELECT minConsolidado FROM INVGCC_METRICAS WHERE anio = {anio}";

            var res = _dal.SelectSql<dynamic>(sql)?.FirstOrDefault();

            if (res != null)
            {
                try
                {
                    if (res is JObject jobj) return (int)jobj["minConsolidado"];
                    return (int)((dynamic)res).minConsolidado;
                }
                catch { }
            }
            return 70;
        }

        public void GuardarMetrica(InvgccMetricas metrica)
        {
            string check = $"SELECT COUNT(*) as conteo FROM INVGCC_METRICAS WHERE anio = {metrica.anio}";
            var res = _dal.SelectSql<dynamic>(check)?.FirstOrDefault();

            int count = 0;
            if (res != null)
            {
                try
                {
                    if (res is JObject jobj) count = (int)jobj["conteo"];
                    else count = (int)((dynamic)res).conteo;
                }
                catch { }
            }

            if (count > 0)
            {
                string sqlUpdate = $"UPDATE INVGCC_METRICAS SET minConsolidado = {metrica.minConsolidado} WHERE anio = {metrica.anio}";
                _dal.UpdateSql(sqlUpdate);
            }
            else
            {
                string sqlInsert = $"INSERT INTO INVGCC_METRICAS (anio, minConsolidado) VALUES ({metrica.anio}, {metrica.minConsolidado})";
                _dal.InsertSql(sqlInsert);
            }
        }

        // =============================================================
        // 3. UTILIDADES Y COMBOS
        // =============================================================

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
                    try
                    {
                        if (item is JObject jobj) anios.Add((int)jobj["Anio"]);
                        else anios.Add((int)((dynamic)item).Anio);
                    }
                    catch { }
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
                    try
                    {
                        if (item is JObject jobj) anios.Add((int)jobj["anio"]);
                        else anios.Add((int)((dynamic)item).anio);
                    }
                    catch { }
                }
            }
            return anios;
        }

        // =============================================================
        // 4. GENERADOR DE CÓDIGOS
        // =============================================================

        private string GenerarCodigoAlfanumerico(string tabla, string campoId, string prefijo)
        {
            string sql = $"SELECT TOP 1 {campoId} FROM {tabla} ORDER BY Len({campoId}) DESC, {campoId} DESC";
            var lista = _dal.SelectSql<dynamic>(sql);
            int siguienteNumero = 1;

            if (lista != null && lista.Count > 0)
            {
                string ultimoId = "";
                var item = lista[0];

                if (item is JObject jobj)
                    ultimoId = jobj[campoId]?.ToString();
                else
                    try { ultimoId = ((dynamic)item).GetType().GetProperty(campoId).GetValue(item, null).ToString(); } catch { }

                if (!string.IsNullOrEmpty(ultimoId) && ultimoId.StartsWith(prefijo))
                {
                    string numeroStr = ultimoId.Substring(prefijo.Length).Replace("-", ""); // Robustez por si acaso
                    if (int.TryParse(numeroStr, out int numeroActual))
                    {
                        siguienteNumero = numeroActual + 1;
                    }
                }
            }

            return $"{prefijo}{siguienteNumero}";
        }
    }
}