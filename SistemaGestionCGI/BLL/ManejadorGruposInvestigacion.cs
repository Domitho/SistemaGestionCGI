using System;
using System.Collections.Generic;
using System.Linq; // Necesario para .FirstOrDefault()
using Newtonsoft.Json.Linq;
using SistemaGestionCGI.Models;
using SistemaGestionCGI.Settings;

namespace SistemaGestionCGI.BLL
{
    public class ManejadorGruposInvestigacion
    {
        private readonly ConnectionSqlServer _dal = ConnectionSqlServer.Instance;

        // =============================================================
        // 1. GESTIÓN DE GRUPOS
        // =============================================================

        public List<InvgccGrupoInvestigacion> ObtenerGrupos()
        {
            string sql = "SELECT * FROM INVGCCGRUPO_INVESTIGACION ORDER BY dtFechacrea_gru DESC";
            return _dal.SelectSql<InvgccGrupoInvestigacion>(sql);
        }

        public List<InvgccGrupoInvestigacion> ObtenerGruposConConteo()
        {
            string sql = @"
                SELECT 
                    G.*, 
                    (SELECT COUNT(*) FROM INVGCCINSCRIPCION_PROYECTOS P WHERE P.fkId_gru = G.strId_gru) as TotalProyectos
                FROM INVGCCGRUPO_INVESTIGACION G
                ORDER BY TotalProyectos DESC, G.strNombre_gru ASC";

            return _dal.SelectSql<InvgccGrupoInvestigacion>(sql);
        }

        public InvgccGrupoInvestigacion ObtenerGrupoPorId(string id)
        {
            string sql = $"SELECT * FROM INVGCCGRUPO_INVESTIGACION WHERE strId_gru = '{id}'";
            var lista = _dal.SelectSql<InvgccGrupoInvestigacion>(sql);
            return lista?.FirstOrDefault();
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
                    strFacultad_gru = '{grupo.strFacultad_gru}',
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
            // Borrado en cascada manual
            string sqlDelHistorial = $"DELETE FROM INVGCCINTEGRANTES_HISTORIAL WHERE strId_int IN (SELECT strId_int FROM INVGCCGRUPO_INTEGRANTES WHERE fkId_gru = '{id}')";
            _dal.DeleteSql(sqlDelHistorial);

            _dal.Delete("INVGCCGRUPO_INVESTIGACION", $"strId_gru = '{id}'");
        }

        public List<InvgccInscripcionProyectos> ObtenerProyectosDeGrupo(string idGrupo)
        {
            // Traemos los proyectos filtrados por la llave foránea fkId_gru
            string sql = $"SELECT * FROM INVGCCINSCRIPCION_PROYECTOS WHERE fkId_gru = '{idGrupo}' ORDER BY dtFehains_pro DESC";
            return _dal.SelectSql<InvgccInscripcionProyectos>(sql);
        }

        // =============================================================
        // 2. GESTIÓN DE INTEGRANTES
        // =============================================================

        public List<InvgccGrupoIntegrantes> ObtenerIntegrantes(string idGrupo)
        {
            string sql = $"SELECT * FROM INVGCCGRUPO_INTEGRANTES WHERE fkId_gru = '{idGrupo}' ORDER BY strApellidos_int";
            return _dal.SelectSql<InvgccGrupoIntegrantes>(sql);
        }

        public InvgccGrupoIntegrantes ObtenerIntegrantePorId(string id)
        {
            string sql = $"SELECT * FROM INVGCCGRUPO_INTEGRANTES WHERE strId_int = '{id}'";
            return _dal.SelectSql<InvgccGrupoIntegrantes>(sql)?.FirstOrDefault();
        }

        public void GuardarIntegrante(InvgccGrupoIntegrantes integrante)
        {
            integrante.strId_int = GenerarCodigoAlfanumerico("INVGCCGRUPO_INTEGRANTES", "strId_int", "I");

            string sql = $@"
                INSERT INTO INVGCCGRUPO_INTEGRANTES 
                (strId_int, fkId_gru, strCedula_int, strNombres_int, strApellidos_int, strCorreo_int, 
                 strCarrera_int, strFuncion_int, dtFechaini_int, strObservacion_int, bitActivo_int, bitPertenece_int,
                 strTipo_int, strFacultad_int, strEntidad_int, strCertificado_int) 
                VALUES 
                ('{integrante.strId_int}', '{integrante.fkId_gru}', '{integrante.strCedula_int}', '{integrante.strNombres_int}', 
                 '{integrante.strApellidos_int}', '{integrante.strCorreo_int}', 
                 '{integrante.strCarrera_int}', '{integrante.strFuncion_int}', '{integrante.dtFechaini_int:yyyy-MM-dd}', 
                 '{integrante.strObservacion_int}', 1, 1,
                 '{integrante.strTipo_int}', '{integrante.strFacultad_int}', '{integrante.strEntidad_int}', '{integrante.strCertificado_int}')";

            _dal.UpdateSql(sql);
        }

        public void ActualizarIntegrante(InvgccGrupoIntegrantes integrante)
        {
            string fechaFin = (integrante.dtFechafin_int.HasValue && integrante.dtFechafin_int.Value.Year > 1900)
                ? $"'{integrante.dtFechafin_int.Value:yyyyMMdd}'"
                : "NULL";

            int activo = integrante.bitActivo_int ? 1 : 0;

            string sql = $@"
                UPDATE INVGCCGRUPO_INTEGRANTES SET 
                    strCedula_int = '{integrante.strCedula_int}',
                    strNombres_int = '{integrante.strNombres_int}',
                    strApellidos_int = '{integrante.strApellidos_int}',
                    strCorreo_int = '{integrante.strCorreo_int}',
                    strCarrera_int = '{integrante.strCarrera_int}',
                    strFuncion_int = '{integrante.strFuncion_int}',
                    dtFechaini_int = '{integrante.dtFechaini_int:yyyyMMdd}', 
                    dtFechafin_int = {fechaFin},
                    strObservacion_int = '{integrante.strObservacion_int}',
                    bitActivo_int = {activo},
                    strTipo_int = '{integrante.strTipo_int}',
                    strFacultad_int = '{integrante.strFacultad_int}',
                    strEntidad_int = '{integrante.strEntidad_int}',
                    strCertificado_int = '{integrante.strCertificado_int}',
                WHERE strId_int = '{integrante.strId_int}'";

            _dal.UpdateSql(sql);
        }

        // =============================================================
        // 3. GESTIÓN DE AUDITORÍA Y ESTADOS
        // =============================================================

        public void CambiarEstadoIntegrante(string id, bool estado, string motivo, string usuario)
        {
            int bit = estado ? 1 : 0;
            string fechaFin = estado ? "NULL" : $"'{DateTime.Now:yyyyMMdd}'";

            string sql = $@"
                UPDATE INVGCCGRUPO_INTEGRANTES 
                SET bitActivo_int = {bit}, dtFechafin_int = {fechaFin}
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

        private void RegistrarHistorial(string idIntegrante, string accion, string motivo, string usuario)
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
        // 4. UTILIDADES (Generador de IDs)
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

                // Manejo robusto de dynamic
                if (item is JObject jobj)
                    ultimoId = jobj[campoId]?.ToString();
                else
                    try { ultimoId = ((dynamic)item).GetType().GetProperty(campoId).GetValue(item, null).ToString(); } catch { }

                if (!string.IsNullOrEmpty(ultimoId) && ultimoId.StartsWith(prefijo))
                {
                    string numeroStr = ultimoId.Substring(prefijo.Length);
                    if (int.TryParse(numeroStr, out int numeroActual))
                    {
                        siguienteNumero = numeroActual + 1;
                    }
                }
            }

            return prefijo + siguienteNumero;
        }
    }
}