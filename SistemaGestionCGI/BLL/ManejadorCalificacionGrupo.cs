using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SistemaGestionCGI.Models;
using SistemaGestionCGI.Settings;

namespace SistemaGestionCGI.BLL
{
    public class ManejadorCalificacionGrupo
    {
        // Instancia del DAL (Singleton)
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
            // Nota: Usar parámetros sería mejor, pero mantenemos compatibilidad con tu DAL actual
            string sql = $"SELECT * FROM INVGCCCALIFICACION_GRUPO WHERE strId_valo = '{id}'";
            var lista = _dal.SelectSql<InvgccCalificacionGrupo>(sql);
            return lista?.FirstOrDefault();
        }

        public void GuardarCalificacion(InvgccCalificacionGrupo obj)
        {
            // 1. Generar ID único
            obj.strId_valo = GenerarCodigoAlfanumerico("INVGCCCALIFICACION_GRUPO", "strId_valo", "VAL");

            // 2. Preparar datos para inserción segura usando Hashtable (Evita SQL Injection manual)
            Hashtable data = new Hashtable
            {
                { "strId_valo", obj.strId_valo },
                { "fkId_gru", obj.fkId_gru },
                { "dtFecha_valo", obj.dtFecha_valo.ToString("yyyy-MM-dd HH:mm:ss") },
                { "intPuntaje_valo", obj.intPuntaje_valo.HasValue ? (object)obj.intPuntaje_valo.Value : DBNull.Value },
                { "strReconocimiento_valo", obj.strReconocimiento_valo ?? "" },
                { "strInforme_valo", obj.strInforme_valo ?? "" },
                { "strResolucion_valo", obj.strResolucion_valo ?? "" },
                { "intAnioMetrica", obj.intAnioMetrica },
                { "strCategoria_valo", obj.strCategoria_valo }
            };

            // 3. Insertar usando el método nativo del DAL
            _dal.Insert("INVGCCCALIFICACION_GRUPO", data);

            // 4. Actualizar estado del grupo vinculado
            ActualizarEstadoGrupo(obj.fkId_gru, obj.strCategoria_valo);
        }

        public void ActualizarCalificacion(InvgccCalificacionGrupo obj)
        {
            // 1. Preparar datos para actualización segura
            Hashtable data = new Hashtable
            {
                { "dtFecha_valo", obj.dtFecha_valo.ToString("yyyy-MM-dd HH:mm:ss") },
                { "intPuntaje_valo", obj.intPuntaje_valo.HasValue ? (object)obj.intPuntaje_valo.Value : DBNull.Value },
                { "strReconocimiento_valo", obj.strReconocimiento_valo ?? "" },
                { "strInforme_valo", obj.strInforme_valo ?? "" },
                { "strResolucion_valo", obj.strResolucion_valo ?? "" },
                { "strCategoria_valo", obj.strCategoria_valo }
                // No actualizamos fkId_gru ni intAnioMetrica por consistencia histórica
            };

            string where = $"strId_valo = '{obj.strId_valo}'";

            // 2. Actualizar usando el método nativo del DAL
            _dal.Update("INVGCCCALIFICACION_GRUPO", data, where);

            // 3. Sincronizar estado del grupo
            ActualizarEstadoGrupo(obj.fkId_gru, obj.strCategoria_valo);
        }

        private void ActualizarEstadoGrupo(string idGrupo, string nuevaCategoria)
        {
            string sqlUpdateGrupo = $"UPDATE INVGCCGRUPO_INVESTIGACION SET strCategoria_gru = '{nuevaCategoria}' WHERE strId_gru = '{idGrupo}'";
            _dal.UpdateSql(sqlUpdateGrupo);
        }

        public void EliminarCalificacion(string id)
        {
            string sql = $"DELETE FROM INVGCCCALIFICACION_GRUPO WHERE strId_valo = '{id}'";
            _dal.DeleteSql(sql);
        }

        // =============================================================
        // 2. GESTIÓN DE MÉTRICAS (CONFIGURACIÓN ACTUALIZADA)
        // =============================================================

        /// <summary>
        /// Obtiene la configuración completa (Consolidado y Emergente) para un año específico.
        /// </summary>
        public InvgccMetricas ObtenerConfiguracionMetricas(int anio)
        {
            // Se incluye minEmergente en la consulta
            string sql = $"SELECT anio, minConsolidado, minEmergente FROM INVGCC_METRICAS WHERE anio = {anio}";

            var res = _dal.SelectSql<InvgccMetricas>(sql)?.FirstOrDefault();

            if (res != null) return res;

            // Valores por defecto seguros si no existe configuración
            return new InvgccMetricas { anio = anio, minConsolidado = 80, minEmergente = 60 };
        }

        public void GuardarMetrica(InvgccMetricas metrica)
        {
            // Verificamos existencia
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
                // UPDATE incluyendo minEmergente
                string sqlUpdate = $@"UPDATE INVGCC_METRICAS 
                                      SET minConsolidado = {metrica.minConsolidado}, 
                                          minEmergente = {metrica.minEmergente} 
                                      WHERE anio = {metrica.anio}";
                _dal.UpdateSql(sqlUpdate);
            }
            else
            {
                // INSERT incluyendo minEmergente
                string sqlInsert = $@"INSERT INTO INVGCC_METRICAS (anio, minConsolidado, minEmergente) 
                                      VALUES ({metrica.anio}, {metrica.minConsolidado}, {metrica.minEmergente})";
                _dal.InsertSql(sqlInsert);
            }
        }

        // =============================================================
        // 3. UTILIDADES Y COMBOS
        // =============================================================

        public List<InvgccGrupoInvestigacion> ObtenerGruposParaCombo(int anio, string idGrupoIncluir = "")
        {
            string filtroExcepcion = "";
            if (!string.IsNullOrEmpty(idGrupoIncluir))
            {
                filtroExcepcion = $" AND fkId_gru <> '{idGrupoIncluir}'";
            }

            string sql = $@"
                SELECT strId_gru, strNombre_gru
                FROM INVGCCGRUPO_INVESTIGACION
                WHERE strId_gru NOT IN (
                    SELECT DISTINCT fkId_gru
                    FROM INVGCCCALIFICACION_GRUPO
                    WHERE intAnioMetrica = {anio} {filtroExcepcion}
                )
                ORDER BY strNombre_gru";

            return _dal.SelectSql<InvgccGrupoInvestigacion>(sql);
        }

        public List<int> ObtenerAniosDisponibles()
        {
            string sql = "SELECT DISTINCT YEAR(dtFecha_valo) as Anio FROM INVGCCCALIFICACION_GRUPO ORDER BY Anio DESC";
            return ExtraerListaEnteros(sql, "Anio");
        }

        public List<int> ObtenerAniosConMetricasConfiguradas()
        {
            string sql = "SELECT DISTINCT anio FROM INVGCC_METRICAS ORDER BY anio DESC";
            return ExtraerListaEnteros(sql, "anio");
        }

        // Método auxiliar para evitar repetir la lógica de extracción de enteros (dynamic/JObject)
        private List<int> ExtraerListaEnteros(string sql, string campo)
        {
            var lista = _dal.SelectSql<dynamic>(sql);
            var resultados = new List<int>();

            if (lista != null)
            {
                foreach (var item in lista)
                {
                    try
                    {
                        if (item is JObject jobj) resultados.Add((int)jobj[campo]);
                        else resultados.Add((int)((dynamic)item).GetType().GetProperty(campo).GetValue(item, null));
                    }
                    catch { }
                }
            }
            return resultados;
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
                    string numeroStr = ultimoId.Substring(prefijo.Length).Replace("-", "");
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