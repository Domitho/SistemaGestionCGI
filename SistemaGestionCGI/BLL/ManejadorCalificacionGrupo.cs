using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SistemaGestionCGI.Models;
using SistemaGestionCGI.Settings;
using Newtonsoft.Json;

namespace SistemaGestionCGI.BLL
{
    public class ManejadorCalificacionGrupo
    {
        private readonly ConnectionSqlServer _dal = ConnectionSqlServer.Instance;

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

        public void GuardarCalificacion(InvgccCalificacionGrupo obj, string usuario, int idUsuario)
        {
            obj.strId_valo = GenerarCodigoAlfanumerico("INVGCCCALIFICACION_GRUPO", "strId_valo", "VAL");

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

            _dal.Insert("INVGCCCALIFICACION_GRUPO", data);

            ActualizarEstadoGrupo(obj.fkId_gru, obj.strCategoria_valo);

            RegistrarHistorico(
                obj.strId_valo,
                obj.fkId_gru,
                "REGISTRO",
                usuario,
                idUsuario,
                null,
                obj
            );
        }

        public void ActualizarCalificacion(InvgccCalificacionGrupo obj, string usuario, int idUsuario)
        {
            var anterior = ObtenerPorId(obj.strId_valo);

            Hashtable data = new Hashtable
            {
                { "dtFecha_valo", obj.dtFecha_valo.ToString("yyyy-MM-dd HH:mm:ss") },
                { "intPuntaje_valo", obj.intPuntaje_valo.HasValue ? (object)obj.intPuntaje_valo.Value : DBNull.Value },
                { "strReconocimiento_valo", obj.strReconocimiento_valo ?? "" },
                { "strInforme_valo", obj.strInforme_valo ?? "" },
                { "strResolucion_valo", obj.strResolucion_valo ?? "" },
                { "strCategoria_valo", obj.strCategoria_valo }
            };

            string idSeguro = (obj.strId_valo ?? "").Replace("'", "''");
            string where = $"strId_valo = '{idSeguro}'";

            _dal.Update("INVGCCCALIFICACION_GRUPO", data, where);

            ActualizarEstadoGrupo(obj.fkId_gru, obj.strCategoria_valo);

            RegistrarHistorico(
                obj.strId_valo,
                obj.fkId_gru,
                "ACTUALIZACION",
                usuario,
                idUsuario,
                anterior,
                obj
            );
        }

        private void ActualizarEstadoGrupo(string idGrupo, string nuevaCategoria)
        {
            string sqlUpdateGrupo = $"UPDATE INVGCCGRUPO_INVESTIGACION SET strCategoria_gru = '{nuevaCategoria}' WHERE strId_gru = '{idGrupo}'";
            _dal.UpdateSql(sqlUpdateGrupo);
        }

        public void EliminarCalificacion(string id, string usuario, int idUsuario)
        {
            var anterior = ObtenerPorId(id);

            if (anterior == null) return;

            string idSeguro = id.Replace("'", "''");

            string sql = $"DELETE FROM INVGCCCALIFICACION_GRUPO WHERE strId_valo = '{idSeguro}'";
            _dal.DeleteSql(sql);

            RegistrarHistorico(
                anterior.strId_valo,
                anterior.fkId_gru,
                "ELIMINACION",
                usuario,
                idUsuario,
                anterior,
                null
            );
        }

        public InvgccMetricas ObtenerConfiguracionMetricas(int anio)
        {
            string sql = $@"
                SELECT anio, minConsolidado, minEmergente, fechaInicio, fechaFin
                FROM INVGCC_METRICAS
                WHERE anio = {anio}";

            var res = _dal.SelectSql<InvgccMetricas>(sql)?.FirstOrDefault();

            if (res != null) return res;

            return new InvgccMetricas
            {
                anio = anio,
                minConsolidado = 80,
                minEmergente = 60,
                fechaInicio = new DateTime(anio - 1, 12, 12),
                fechaFin = new DateTime(anio, 12, 12)
            };
        }

        public void GuardarMetrica(InvgccMetricas metrica)
        {
            metrica.fechaInicio = new DateTime(metrica.anio - 1, 12, 12);
            metrica.fechaFin = new DateTime(metrica.anio, 12, 12);

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
                string sqlUpdate = $@"
                    UPDATE INVGCC_METRICAS 
                    SET minConsolidado = {metrica.minConsolidado}, 
                        minEmergente = {metrica.minEmergente},
                        fechaInicio = '{metrica.fechaInicio:yyyy-MM-dd}',
                        fechaFin = '{metrica.fechaFin:yyyy-MM-dd}'
                    WHERE anio = {metrica.anio}";
                _dal.UpdateSql(sqlUpdate);
            }
            else
            {
                string sqlInsert = $@"
                    INSERT INTO INVGCC_METRICAS (anio, minConsolidado, minEmergente, fechaInicio, fechaFin) 
                    VALUES (
                        {metrica.anio},
                        {metrica.minConsolidado},
                        {metrica.minEmergente},
                        '{metrica.fechaInicio:yyyy-MM-dd}',
                        '{metrica.fechaFin:yyyy-MM-dd}'
                    )";
                _dal.InsertSql(sqlInsert);
            }
        }


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

        private void RegistrarHistorico(string idCalificacion, string idGrupo, string accion, string usuario, int idUsuario, object datosAnteriores, object datosNuevos)
        {
            string nombreGrupo = ObtenerNombreGrupo(idGrupo);
            string descripcion = ConstruirDescripcion(accion, usuario, nombreGrupo);

            string jsonAntes = datosAnteriores != null
                ? JsonConvert.SerializeObject(datosAnteriores).Replace("'", "''")
                : null;

            string jsonNuevos = datosNuevos != null
                ? JsonConvert.SerializeObject(datosNuevos).Replace("'", "''")
                : null;

            idCalificacion = (idCalificacion ?? "").Replace("'", "''");
            idGrupo = (idGrupo ?? "").Replace("'", "''");
            accion = (accion ?? "").Replace("'", "''");
            usuario = (usuario ?? "").Replace("'", "''");

            string sql = $@"
                INSERT INTO INVGCCCALIFICACION_GRUPO_HISTORICO
                (
                    idCalificacion,
                    idGrupo,
                    accion,
                    usuarioAccion,
                    IdUsuario,
                    fechaAccion,
                    descripcion,
                    datosAnteriores,
                    datosNuevos
                )
                VALUES
                (
                    '{idCalificacion}',
                    '{idGrupo}',
                    '{accion}',
                    '{usuario}',
                    {idUsuario},
                    GETDATE(),
                    '{descripcion}',
                    {(jsonAntes != null ? $"N'{jsonAntes}'" : "NULL")},
                    {(jsonNuevos != null ? $"N'{jsonNuevos}'" : "NULL")}
                )";

            _dal.UpdateSql(sql);
        }


        public List<dynamic> ObtenerHistorialGlobal()
        {
            string sql = @"
            SELECT 
                idCalificacion,
                accion,
                usuarioAccion,
                fechaAccion,
                descripcion
            FROM INVGCCCALIFICACION_GRUPO_HISTORICO
            ORDER BY fechaAccion DESC";

            return _dal.SelectSql<dynamic>(sql);
        }

        private string ObtenerNombreGrupo(string idGrupo)
        {
            string idSeguro = (idGrupo ?? "").Replace("'", "''");

            string sql = $@"
                SELECT TOP 1 strNombre_gru
                FROM INVGCCGRUPO_INVESTIGACION
                WHERE strId_gru = '{idSeguro}'";

            var lista = _dal.SelectSql<dynamic>(sql);

            if (lista != null && lista.Count > 0)
            {
                var item = lista[0];
                try
                {
                    if (item is Newtonsoft.Json.Linq.JObject jobj)
                        return jobj["strNombre_gru"]?.ToString() ?? "";
                    return item.GetType().GetProperty("strNombre_gru")?.GetValue(item, null)?.ToString() ?? "";
                }
                catch
                {
                    return "";
                }
            }
            return "";
        }

        private string ConstruirDescripcion(string accion, string usuario, string nombreGrupo)
        {
            usuario = string.IsNullOrWhiteSpace(usuario) ? "Usuario no identificado" : usuario;
            nombreGrupo = string.IsNullOrWhiteSpace(nombreGrupo) ? "Grupo no identificado" : nombreGrupo;

            switch (accion)
            {
                case "REGISTRO":
                    return $"{usuario} registró una nueva calificación del grupo {nombreGrupo}.";
                case "ACTUALIZACION":
                    return $"{usuario} actualizó la calificación del grupo {nombreGrupo}.";
                case "ELIMINACION":
                    return $"{usuario} eliminó la calificación del grupo {nombreGrupo}.";
                default:
                    return $"{usuario} realizó una acción sobre la calificación del grupo {nombreGrupo}.";
            }
        }

        // METRICAS

        public List<InvgccMetricas> ObtenerMetricas()
        {
            string sql = @"
                SELECT 
                    anio,
                    minConsolidado,
                    minEmergente,
                    fechaInicio,
                    fechaFin
                FROM INVGCC_METRICAS
                ORDER BY anio DESC";

            return _dal.SelectSql<InvgccMetricas>(sql);
        }

        public InvgccMetricas ObtenerMetricaPorAnio(int anio)
        {
            string sql = $@"
                SELECT 
                    anio,
                    minConsolidado,
                    minEmergente,
                    fechaInicio,
                    fechaFin
                FROM INVGCC_METRICAS
                WHERE anio = {anio}";

            return _dal.SelectSql<InvgccMetricas>(sql)?.FirstOrDefault();
        }

        public void EliminarMetrica(int anio)
        {
            string sql = $"DELETE FROM INVGCC_METRICAS WHERE anio = {anio}";
            _dal.DeleteSql(sql);
        }

    }
}