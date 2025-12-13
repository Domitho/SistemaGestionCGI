using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SistemaGestionCGI.Models;
using SistemaGestionCGI.Settings;

namespace SistemaGestionCGI.BLL
{
    public class ManejadorGruposInvestigacion
    {
        private readonly ConnectionSqlServer _dal = ConnectionSqlServer.Instance;

        // =============================================================
        // 1. GESTIÓN DE GRUPOS (INVGCCGRUPO_INVESTIGACION)
        // =============================================================

        public List<InvgccGrupoInvestigacion> ObtenerGrupos()
        {
            string sql = "SELECT * FROM INVGCCGRUPO_INVESTIGACION ORDER BY dtFechacrea_gru DESC";
            return _dal.SelectSql<InvgccGrupoInvestigacion>(sql);
        }

        public InvgccGrupoInvestigacion ObtenerGrupoPorId(string id)
        {
            string sql = $"SELECT * FROM INVGCCGRUPO_INVESTIGACION WHERE strId_gru = '{id}'";
            var lista = _dal.SelectSql<InvgccGrupoInvestigacion>(sql);
            return (lista != null && lista.Count > 0) ? lista[0] : null;
        }

        public void GuardarGrupo(InvgccGrupoInvestigacion grupo)
        {
            grupo.strId_gru = GenerarCodigoAlfanumerico("INVGCCGRUPO_INVESTIGACION", "strId_gru", "G");

            _dal.Insert("INVGCCGRUPO_INVESTIGACION", grupo);
        }

        public void ActualizarGrupo(InvgccGrupoInvestigacion grupo)
        {
            string sql = $@"
                UPDATE INVGCCGRUPO_INVESTIGACION SET 
                    strNombre_gru = '{grupo.strNombre_gru}',
                    strCoordinador_gru = '{grupo.strCoordinador_gru}',
                    dtFechacrea_gru = '{grupo.dtFechacrea_gru:yyyy-MM-dd HH:mm:ss}',
                    strCategoria_gru = '{grupo.strCategoria_gru}',
                    strLineasinv_gru = '{grupo.strLineasinv_gru}',
                    strSublineasinv_gru = '{grupo.strSublineasinv_gru}',
                    strArchivo_gru = '{grupo.strArchivo_gru}',
                    strFoto_gru = '{grupo.strFoto_gru}'
                WHERE strId_gru = '{grupo.strId_gru}'";

            _dal.UpdateSql(sql);
        }

        public void EliminarGrupo(string id)
        {
            string sqlDelHistorial = $"DELETE FROM INVGCCINTEGRANTES_HISTORIAL WHERE strId_int IN (SELECT strId_int FROM INVGCCGRUPO_INTEGRANTES WHERE fkId_gru = '{id}')";
            _dal.DeleteSql(sqlDelHistorial);

            _dal.Delete("INVGCCGRUPO_INTEGRANTES", $"fkId_gru = '{id}'");
            _dal.Delete("INVGCCGRUPO_INVESTIGACION", $"strId_gru = '{id}'");
        }

        // =============================================================
        // 2. GESTIÓN DE INTEGRANTES (INVGCCGRUPO_INTEGRANTES)
        // =============================================================

        public List<InvgccGrupoIntegrantes> ObtenerIntegrantes(string idGrupo)
        {
            string sql = $"SELECT * FROM INVGCCGRUPO_INTEGRANTES WHERE fkId_gru = '{idGrupo}' ORDER BY strApellidos_int";
            return _dal.SelectSql<InvgccGrupoIntegrantes>(sql);
        }

        public InvgccGrupoIntegrantes ObtenerIntegrantePorId(string id)
        {
            string sql = $"SELECT * FROM INVGCCGRUPO_INTEGRANTES WHERE strId_int = '{id}'";
            var lista = _dal.SelectSql<InvgccGrupoIntegrantes>(sql);
            return (lista != null && lista.Count > 0) ? lista[0] : null;
        }

        public void GuardarIntegrante(InvgccGrupoIntegrantes integrante)
        {
            integrante.strId_int = GenerarCodigoAlfanumerico("INVGCCGRUPO_INTEGRANTES", "strId_int", "I");
            integrante.bitActivo_int = true;
            integrante.bitPertenece_int = true;

            _dal.Insert("INVGCCGRUPO_INTEGRANTES", integrante);
        }

        public void ActualizarIntegrante(InvgccGrupoIntegrantes integrante)
        {
            string fechaFin = "NULL";
            if (integrante.dtFechafin_int.HasValue && integrante.dtFechafin_int.Value.Year > 1900)
            {
                fechaFin = $"'{integrante.dtFechafin_int.Value:yyyyMMdd}'";
            }

            int activo = integrante.bitActivo_int ? 1 : 0;
            string fechaIni = $"'{integrante.dtFechaini_int:yyyyMMdd}'";

            string sql = $@"
                UPDATE INVGCCGRUPO_INTEGRANTES SET 
                    strCedula_int = '{integrante.strCedula_int}',
                    strNombres_int = '{integrante.strNombres_int}',
                    strApellidos_int = '{integrante.strApellidos_int}',
                    strCorreo_int = '{integrante.strCorreo_int}',
                    strCarrera_int = '{integrante.strCarrera_int}',
                    strFuncion_int = '{integrante.strFuncion_int}',
                    dtFechaini_int = {fechaIni}, 
                    dtFechafin_int = {fechaFin},
                    strObservacion_int = '{integrante.strObservacion_int}',
                    bitActivo_int = {activo}
                WHERE strId_int = '{integrante.strId_int}'";

            _dal.UpdateSql(sql);
        }

        public void CambiarEstadoIntegrante(string id, bool estado, string motivo, string usuario)
        {
            int bit = estado ? 1 : 0;

            string fechaFin = estado ? "NULL" : $"'{DateTime.Now:yyyyMMdd}'";

            string sql = $@"
                UPDATE INVGCCGRUPO_INTEGRANTES 
                SET bitActivo_int = {bit}, 
                    dtFechafin_int = {fechaFin}
                WHERE strId_int = '{id}'";

            _dal.UpdateSql(sql);

            string accion = estado ? "REACTIVACIÓN" : "BAJA";
            RegistrarHistorial(id, accion, motivo, usuario);
        }

        public void EliminarIntegranteFisico(string id)
        {
            _dal.Delete("INVGCCINTEGRANTES_HISTORIAL", $"strId_int = '{id}'");
            _dal.Delete("INVGCCGRUPO_INTEGRANTES", $"strId_int = '{id}'");
        }

        // =============================================================
        // 3. GESTIÓN DE HISTORIAL (INVGCCINTEGRANTES_HISTORIAL)
        // =============================================================

        public void RegistrarHistorial(string idIntegrante, string accion, string motivo, string usuario)
        {
            string fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string sql = $@"
                INSERT INTO INVGCCINTEGRANTES_HISTORIAL 
                (strId_int, dtFecha, strAccion, strMotivo, strUsuario) 
                VALUES 
                ('{idIntegrante}', '{fecha}', '{accion}', '{motivo}', '{usuario}')";

            _dal.UpdateSql(sql);
        }

        public List<InvgccIntegrantesHistorial> ObtenerHistorial(string idIntegrante)
        {
            string sql = $"SELECT * FROM INVGCCINTEGRANTES_HISTORIAL WHERE strId_int = '{idIntegrante}' ORDER BY dtFecha DESC";
            return _dal.SelectSql<InvgccIntegrantesHistorial>(sql);
        }

        // =============================================================
        // 4. UTILIDADES (Generador G# / I#) - CORREGIDO
        // =============================================================

        private string GenerarCodigoAlfanumerico(string tabla, string campoId, string prefijo)
        {
            string sql = $"SELECT {campoId} FROM {tabla}";
            var lista = _dal.SelectSql<dynamic>(sql);

            if (lista == null || lista.Count == 0) return prefijo + "1";

            int max = 0;
            foreach (var item in lista)
            {
                string val = "";

                if (item is JObject jobj)
                {
                    val = jobj[campoId]?.ToString();
                }
                else
                {
                    try
                    {
                        val = ((dynamic)item)[campoId].ToString();
                    }
                    catch { continue; }
                }

                if (!string.IsNullOrEmpty(val) && val.StartsWith(prefijo))
                {
                    string numStr = val.Substring(prefijo.Length);
                    if (int.TryParse(numStr, out int n))
                    {
                        if (n > max) max = n;
                    }
                }
            }

            return prefijo + (max + 1);
        }
    }
}