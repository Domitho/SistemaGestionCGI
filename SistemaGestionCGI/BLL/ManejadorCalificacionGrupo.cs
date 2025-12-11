using System;
using System.Collections.Generic;
using System.Linq;
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
            // Hacemos un JOIN manual en la consulta para traer el nombre del grupo
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

        public InvgccCalificacionGrupo ObtenerPorId(int id)
        {
            string sql = $"SELECT * FROM INVGCCCALIFICACION_GRUPO WHERE strId_valo = {id}";
            var lista = _dal.SelectSql<InvgccCalificacionGrupo>(sql);
            return (lista != null && lista.Count > 0) ? lista[0] : null;
        }

        public void GuardarCalificacion(InvgccCalificacionGrupo obj)
        {
            // 1. Obtener ID Manual (Max + 1) ya que parece no ser Identity
            string sqlMax = "SELECT MAX(strId_valo) as maxId FROM INVGCCCALIFICACION_GRUPO";
            var res = _dal.SelectSql<dynamic>(sqlMax).FirstOrDefault();
            int nextId = 1;

            // Validar si devolvió nulo o tiene valor
            if (res != null)
            {
                // Manejar JObject o Dynamic según tu DAL
                try { nextId = (int)res.maxId + 1; } catch { nextId = 1; }
            }

            obj.strId_valo = nextId;

            // 2. Insertar Calificación
            _dal.Insert("INVGCCCALIFICACION_GRUPO", obj);

            // 3. Actualizar Categoría en la tabla de Grupos (Sincronización)
            string sqlUpdateGrupo = $@"
                UPDATE INVGCCGRUPO_INVESTIGACION 
                SET strCategoria_gru = '{obj.strCategoria_valo}' 
                WHERE strId_gru = '{obj.fkId_gru}'";

            _dal.UpdateSql(sqlUpdateGrupo);
        }

        public void EliminarCalificacion(int id)
        {
            _dal.Delete("INVGCCCALIFICACION_GRUPO", $"strId_valo = {id}");
        }

        // ======================= MÉTRICAS Y LÓGICA =======================

        public int ObtenerMinimoConsolidado(int anio)
        {
            string sql = $"SELECT minConsolidado FROM INVGCC_METRICAS WHERE anio = {anio}";
            var res = _dal.SelectSql<dynamic>(sql).FirstOrDefault();

            if (res != null)
            {
                try { return (int)res.minConsolidado; } catch { return 70; } // Default 70
            }
            return 70; // Default si no existe config
        }

        public void GuardarMetrica(InvgccMetricas metrica)
        {
            // UPSERT (Update si existe, Insert si no)
            string check = $"SELECT COUNT(*) as conteo FROM INVGCC_METRICAS WHERE anio = {metrica.anio}";
            var res = _dal.SelectSql<dynamic>(check).First();
            int count = 0;
            try { count = (int)res.conteo; } catch { }

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

        public List<InvgccGrupoInvestigacion> ObtenerGruposParaCombo()
        {

            string sql = @"
                SELECT strId_gru, strNombre_gru 
                FROM INVGCCGRUPO_INVESTIGACION 
                WHERE strId_gru NOT IN (SELECT DISTINCT fkId_gru FROM INVGCCCALIFICACION_GRUPO)
                ORDER BY strNombre_gru";

            return _dal.SelectSql<InvgccGrupoInvestigacion>(sql);
        }

        public List<int> ObtenerAniosDisponibles()
        {
            string sql = "SELECT DISTINCT YEAR(dtFecha_valo) as Anio FROM INVGCCCALIFICACION_GRUPO ORDER BY Anio DESC";
            var lista = _dal.SelectSql<dynamic>(sql);
            List<int> anios = new List<int>();

            if (lista != null)
            {
                foreach (var item in lista)
                {
                    try { anios.Add((int)item.Anio); } catch { }
                }
            }
            return anios;
        }

        public List<int> ObtenerAniosConMetricasConfiguradas()
        {
            string sql = "SELECT DISTINCT anio FROM INVGCC_METRICAS ORDER BY anio DESC";
            var lista = _dal.SelectSql<dynamic>(sql);
            List<int> anios = new List<int>();

            if (lista != null)
            {
                foreach (var item in lista)
                {
                    // Manejo seguro de JObject/Dynamic
                    try { anios.Add((int)item.anio); } catch { }
                }
            }
            return anios;
        }
    }
}