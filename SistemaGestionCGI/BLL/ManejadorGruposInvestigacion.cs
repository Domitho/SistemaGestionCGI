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
        // 1. GESTIÓN DE GRUPOS
        // =============================================================

        public List<InvgccCentroInvestigacion> ObtenerCentrosCombo()
        {
            string sql = "SELECT strId_cen, strNombre_cen FROM INVGCCCENTRO_INVESTIGACION WHERE bitActivo_cen = 1 ORDER BY strNombre_cen";
            return _dal.SelectSql<InvgccCentroInvestigacion>(sql);
        }

        public List<InvgccGrupoInvestigacion> ObtenerGrupos()
        {
            string sql = @"
                SELECT 
                        G.*, 
                        C.strNombre_cen,
                        (SELECT COUNT(*) FROM INVGCCINSCRIPCION_PROYECTOS P WHERE P.fkId_gru = G.strId_gru) as TotalProyectos
                    FROM INVGCCGRUPO_INVESTIGACION G
                    LEFT JOIN INVGCCCENTRO_INVESTIGACION C ON G.fkId_cen = C.strId_cen
                    ORDER BY TotalProyectos DESC, G.dtFechacrea_gru DESC";

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

            string valorCentro = string.IsNullOrEmpty(grupo.fkId_cen)
                         ? "NULL"
                         : $"'{grupo.fkId_cen}'";

            string sql = $@"
                INSERT INTO INVGCCGRUPO_INVESTIGACION
                (strId_gru, strNombre_gru, strCoordinador_gru, dtFechacrea_gru, 
                 strCategoria_gru, strLineasinv_gru, strSublineasinv_gru, 
                 strArchivo_gru, strFoto_gru, fkId_cen)
                VALUES
                ('{grupo.strId_gru}', '{grupo.strNombre_gru}', '{grupo.strCoordinador_gru}', 
                 '{grupo.dtFechacrea_gru:yyyy-MM-dd HH:mm:ss}', '{grupo.strCategoria_gru}', 
                 '{grupo.strLineasinv_gru}', '{grupo.strSublineasinv_gru}', 
                 '{grupo.strArchivo_gru}', '{grupo.strFoto_gru}', {valorCentro})";

            _dal.InsertSql(sql);
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
                    strFoto_gru = '{grupo.strFoto_gru}',
                    fkId_cen = '{grupo.fkId_cen}' -- Agregamos fkId_cen
                WHERE strId_gru = '{grupo.strId_gru}'";

            _dal.UpdateSql(sql);
        }

        public void EliminarGrupo(string id)
        {
            string sqlDelHistorial = $"DELETE FROM INVGCCINTEGRANTES_HISTORIAL WHERE strId_int IN (SELECT strId_int FROM INVGCCGRUPO_INTEGRANTES WHERE fkId_gru = '{id}')";
            _dal.DeleteSql(sqlDelHistorial);

            _dal.Delete("INVGCCGRUPO_INVESTIGACION", $"strId_gru = '{id}'");
        }

        public List<InvgccInscripcionProyectos> ObtenerProyectosDeGrupo(string idGrupo)
        {
            string sql = $"SELECT * FROM INVGCCINSCRIPCION_PROYECTOS WHERE fkId_gru = '{idGrupo}' ORDER BY dtFehains_pro DESC";
            return _dal.SelectSql<InvgccInscripcionProyectos>(sql);
        }

        public void RegistrarGrupoConCoordinador(InvgccGrupoInvestigacion grupo, InvgccGrupoIntegrantes coordinador, string usuario)
        {
            grupo.strId_gru = GenerarCodigoAlfanumerico("INVGCCGRUPO_INVESTIGACION", "strId_gru", "G");
            coordinador.fkId_gru = grupo.strId_gru;

            coordinador.strId_int = GenerarCodigoAlfanumerico("INVGCCGRUPO_INTEGRANTES", "strId_int", "I");

            string valorCentro = string.IsNullOrEmpty(grupo.fkId_cen) ? "NULL" : $"'{grupo.fkId_cen}'";
            string fechaHoy = DateTime.Now.ToString("yyyy-MM-dd");
            string fechaHist = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string sqlGrupo = $@"
                INSERT INTO INVGCCGRUPO_INVESTIGACION
                (strId_gru, strNombre_gru, strCoordinador_gru, dtFechacrea_gru, 
                 strCategoria_gru, strLineasinv_gru, strSublineasinv_gru, 
                 strArchivo_gru, strFoto_gru, fkId_cen)
                VALUES
                ('{grupo.strId_gru}', '{grupo.strNombre_gru}', '{grupo.strCoordinador_gru}', 
                 '{grupo.dtFechacrea_gru:yyyy-MM-dd HH:mm:ss}', '{grupo.strCategoria_gru}', 
                 '{grupo.strLineasinv_gru}', '{grupo.strSublineasinv_gru}', 
                 '{grupo.strArchivo_gru}', '{grupo.strFoto_gru}', {valorCentro});";

            string sqlCoord = $@"
                INSERT INTO INVGCCGRUPO_INTEGRANTES
                (strId_int, fkId_gru, strCedula_int, strNombres_int, strApellidos_int, strCorreo_int,
                 strCarrera_int, strFuncion_int, dtFechaini_int, bitActivo_int, 
                 strTipo_int, strFacultad_int, strEntidad_int, strCertificado_int, fkId_docente_origen) -- <--- AGREGADO
                VALUES
                ('{coordinador.strId_int}', '{coordinador.fkId_gru}', '{coordinador.strCedula_int}', '{coordinador.strNombres_int}',
                 '{coordinador.strApellidos_int}', '{coordinador.strCorreo_int}',
                 '{coordinador.strCarrera_int}', '{coordinador.strFuncion_int}', '{DateTime.Now:yyyy-MM-dd}',
                 1, 
                 '{coordinador.strTipo_int}', '{coordinador.strFacultad_int}', '{coordinador.strEntidad_int}', '{coordinador.strCertificado_int}',
                 {(string.IsNullOrEmpty(coordinador.fkId_docente_origen) ? "NULL" : $"'{coordinador.fkId_docente_origen}'")} -- <--- AGREGADO
                );";

            string sqlHist = $@"
                INSERT INTO INVGCCINTEGRANTES_HISTORIAL 
                (strId_int, dtFecha, strAccion, strMotivo, strUsuario) 
                VALUES 
                ('{coordinador.strId_int}', '{fechaHist}', 'VINCULACIÓN', 'Coordinador Inicial del Grupo', '{usuario}');";

            string sqlFinal = $@"
                BEGIN TRANSACTION;
                BEGIN TRY
                    {sqlGrupo}
                    {sqlCoord}
                    {sqlHist}
                    COMMIT TRANSACTION;
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION;
                    THROW; -- Re-lanza el error para que C# lo capture
                END CATCH";

            _dal.InsertSql(sqlFinal);
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

        public void GuardarIntegrante(InvgccGrupoIntegrantes integrante, string usuario)
        {
            integrante.strId_int = GenerarCodigoAlfanumerico("INVGCCGRUPO_INTEGRANTES", "strId_int", "I");

            string sql = $@"
                INSERT INTO INVGCCGRUPO_INTEGRANTES
                (
                    strId_int, fkId_gru, strCedula_int, strNombres_int, strApellidos_int, 
                    strCorreo_int, strCarrera_int, strFuncion_int, dtFechaini_int, bitActivo_int, 
                    strTipo_int, strFacultad_int, strEntidad_int, strCertificado_int, 
                    fkId_docente_origen  -- <--- 1. AGREGADO AQUÍ
                ) 
                VALUES
                (
                    '{integrante.strId_int}', 
                    '{integrante.fkId_gru}', 
                    '{integrante.strCedula_int}', 
                    '{integrante.strNombres_int}',
                    '{integrante.strApellidos_int}', 
                    '{integrante.strCorreo_int}',
                    {(string.IsNullOrEmpty(integrante.strCarrera_int) ? "NULL" : $"'{integrante.strCarrera_int}'")},
                    '{integrante.strFuncion_int}', 
                    '{integrante.dtFechaini_int:yyyy-MM-dd}',
                    1,
                    '{integrante.strTipo_int}',
                    {(string.IsNullOrEmpty(integrante.strFacultad_int) ? "NULL" : $"'{integrante.strFacultad_int}'")},
                    {(string.IsNullOrEmpty(integrante.strEntidad_int) ? "NULL" : $"'{integrante.strEntidad_int}'")},
                    {(string.IsNullOrEmpty(integrante.strCertificado_int) ? "NULL" : $"'{integrante.strCertificado_int}'")},
                    {(string.IsNullOrEmpty(integrante.fkId_docente_origen) ? "NULL" : $"'{integrante.fkId_docente_origen}'")}
                )";

            _dal.UpdateSql(sql);

            RegistrarHistorial(integrante.strId_int, "VINCULACIÓN", "Registro inicial en el grupo.", usuario);
        }

        public void ActualizarIntegrante(InvgccGrupoIntegrantes integrante, string usuario)
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
                    strCarrera_int = {(string.IsNullOrEmpty(integrante.strCarrera_int) ? "NULL" : $"'{integrante.strCarrera_int}'")},
                    strFuncion_int = '{integrante.strFuncion_int}',
                    dtFechaini_int = '{integrante.dtFechaini_int:yyyy-MM-dd}',
                    strTipo_int = '{integrante.strTipo_int}',
                    strFacultad_int = {(string.IsNullOrEmpty(integrante.strFacultad_int) ? "NULL" : $"'{integrante.strFacultad_int}'")},
                    strEntidad_int = {(string.IsNullOrEmpty(integrante.strEntidad_int) ? "NULL" : $"'{integrante.strEntidad_int}'")},
            
                    strCertificado_int = {(string.IsNullOrEmpty(integrante.strCertificado_int) ? "strCertificado_int" : $"'{integrante.strCertificado_int}'")},

                    fkId_docente_origen = {(string.IsNullOrEmpty(integrante.fkId_docente_origen) ? "NULL" : $"'{integrante.fkId_docente_origen}'")}

                WHERE strId_int = '{integrante.strId_int}'";

            _dal.UpdateSql(sql);

            RegistrarHistorial(integrante.strId_int, "EDICIÓN", "Actualización de datos generales.", usuario);
        }

        public string VerificarIntegranteEnOtroGrupo(string cedula)
        {
            string sql = $@"
                SELECT G.strNombre_gru 
                FROM INVGCCGRUPO_INTEGRANTES I
                INNER JOIN INVGCCGRUPO_INVESTIGACION G ON I.fkId_gru = G.strId_gru
                WHERE I.strCedula_int = '{cedula}' 
                AND I.bitActivo_int = 1";

            var resultado = _dal.SelectSql<dynamic>(sql);

            if (resultado != null && resultado.Count > 0)
            {
                try
                {
                    return ((dynamic)resultado[0]).strNombre_gru.ToString();
                }
                catch
                {
                    return "OTRO GRUPO";
                }
            }

            return null; 
        }

        public dynamic ObtenerDocenteCategorizado(string cedula)
        {
            string sql = $@"
                SELECT TOP 1 
                    strId_doc, strCedula_doc, strNombres_doc, strApellidos_doc, 
                    strFacultad_doc, strCarrera_doc, strCertificado_doc 
                FROM INVGCCCATEGORIZACION_DOCENTES 
                WHERE strCedula_doc = '{cedula}' AND bitActivo_doc = 1";

            var resultado = _dal.SelectSql<dynamic>(sql);
            return (resultado != null && resultado.Count > 0) ? resultado[0] : null;
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