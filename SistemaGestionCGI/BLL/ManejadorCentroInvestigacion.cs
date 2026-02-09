using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using SistemaGestionCGI.Models;
using SistemaGestionCGI.Settings;

namespace SistemaGestionCGI.BLL
{
    public class ManejadorCentroInvestigacion
    {
        private readonly ConnectionSqlServer _dal = ConnectionSqlServer.Instance;

        // ========================
        // 1. GESTIÓN DE CENTROS (CRUD)
        // ========================

        public List<InvgccCentroInvestigacion> ObtenerTodos()
        {
            string sql = @"
                SELECT 
                    C.strId_cen, C.strNombre_cen, C.strFacultad_cen, 
                    C.strArea_cen, C.strUbicacion_cen, C.strLineaInv_cen, 
                    C.strMision_cen, C.strVision_cen, C.dtFechaAprobacion_cen, 
                    C.bitActivo_cen, C.strResolucion_cen, C.strAceptacion_cen,
            
                    ISNULL(
                        (SELECT TOP 1 (I.strNombres_cin + ' ' + I.strApellidos_cin)
                         FROM INVGCCCENTRO_INVESTIGACION_INTEGRANTES I 
                         WHERE I.fkId_cen = C.strId_cen 
                         AND I.strFuncion_cin = 'Director' 
                         AND I.bitActivo_cin = 1), 
                        'SIN ASIGNAR'
                    ) AS NombreDirector

                FROM INVGCCCENTRO_INVESTIGACION C
                WHERE C.bitActivo_cen = 1
                ORDER BY C.strNombre_cen";

            return _dal.SelectSql<InvgccCentroInvestigacion>(sql);
        }

        public InvgccCentroInvestigacion ObtenerPorId(string id)
        {
            string sql = $"SELECT * FROM INVGCCCENTRO_INVESTIGACION WHERE strId_cen = '{id}'";
            return _dal.SelectSql<InvgccCentroInvestigacion>(sql)?.FirstOrDefault();
        }

        public void Guardar(InvgccCentroInvestigacion centro)
        {
            centro.strId_cen = GenerarNuevoIdCentro();
            string sql = $@"
                INSERT INTO INVGCCCENTRO_INVESTIGACION
                (strId_cen, strNombre_cen, strFacultad_cen, strArea_cen, strUbicacion_cen,
                 strLineaInv_cen, strMision_cen, strVision_cen, dtFechaAprobacion_cen,
                 strResolucion_cen, strAceptacion_cen,
                 bitActivo_cen, dtFechaRegistro)
                VALUES
                ('{centro.strId_cen}', '{centro.strNombre_cen}', '{centro.strFacultad_cen}',
                 '{centro.strArea_cen}', '{centro.strUbicacion_cen}', '{centro.strLineaInv_cen}',
                 '{centro.strMision_cen}', '{centro.strVision_cen}',
                 '{centro.dtFechaAprobacion_cen:yyyy-MM-dd}',
                 '{centro.strResolucion_cen}', '{centro.strAceptacion_cen}',
                 1, GETDATE())";

            _dal.InsertSql(sql);
        }

        public void Actualizar(InvgccCentroInvestigacion centro)
        {
            string sql = $@"
                UPDATE INVGCCCENTRO_INVESTIGACION SET
                strNombre_cen = '{centro.strNombre_cen}',
                strFacultad_cen = '{centro.strFacultad_cen}',
                strArea_cen = '{centro.strArea_cen}',
                strUbicacion_cen = '{centro.strUbicacion_cen}',
                strLineaInv_cen = '{centro.strLineaInv_cen}',
                strMision_cen = '{centro.strMision_cen}',
                strVision_cen = '{centro.strVision_cen}',
                dtFechaAprobacion_cen = '{centro.dtFechaAprobacion_cen:yyyy-MM-dd}',
                strResolucion_cen = '{centro.strResolucion_cen}',
                strAceptacion_cen = '{centro.strAceptacion_cen}'
                WHERE strId_cen = '{centro.strId_cen}'";

            _dal.UpdateSql(sql);
        }

        public void Eliminar(string id)
        {
            // Baja lógica
            string sql = $"UPDATE INVGCCCENTRO_INVESTIGACION SET bitActivo_cen = 0 WHERE strId_cen = '{id}'";
            _dal.UpdateSql(sql);
        }

        // ==========================================
        // 2. GESTIÓN DE INTEGRANTES DEL CENTRO
        // ==========================================

        public List<InvgccCentroIntegrantes> ObtenerIntegrantesPorCentro(string idCentro)
        {
            string sql = $@"
                SELECT 
                    I.strId_cin, I.fkId_cen, I.strCedula_cin, I.strNombres_cin, I.strApellidos_cin, 
                    I.strCorreo_cin, I.strFuncion_cin, I.strTipo_cin, I.strCarrera_cin, 
                    I.strFacultad_cin, I.strEntidad_cin, I.dtFechaRegistro_cin, I.bitActivo_cin,
                    I.dtFechaFin_cin, 
                    (I.strApellidos_cin + ' ' + I.strNombres_cin) as NombreCompleto
                FROM INVGCCCENTRO_INVESTIGACION_INTEGRANTES I
                WHERE fkId_cen = '{idCentro}' 
                AND bitActivo_cin = 1  
                ORDER BY strApellidos_cin ASC";

            return _dal.SelectSql<InvgccCentroIntegrantes>(sql);
        }

        public InvgccCentroIntegrantes ObtenerIntegrantePorId(string idIntegrante)
        {
            string sql = $"SELECT * FROM INVGCCCENTRO_INVESTIGACION_INTEGRANTES WHERE strId_cin = '{idIntegrante}'";
            return _dal.SelectSql<InvgccCentroIntegrantes>(sql)?.FirstOrDefault();
        }

        // UNIFICADO: Solo dejamos este método GuardarIntegrante
        public void GuardarIntegrante(InvgccCentroIntegrantes obj, string usuarioLogueado)
        {
            obj.strId_cin = GenerarCodigoAlfanumerico("INVGCCCENTRO_INVESTIGACION_INTEGRANTES", "strId_cin", "CIN");

            string sql = $@"
                INSERT INTO INVGCCCENTRO_INVESTIGACION_INTEGRANTES
                (strId_cin, fkId_cen, strCedula_cin, strNombres_cin, strApellidos_cin, strCorreo_cin,
                 strFuncion_cin, strTipo_cin, strCarrera_cin, strFacultad_cin, strEntidad_cin, bitActivo_cin, dtFechaRegistro_cin)
                VALUES
                ('{obj.strId_cin}', '{obj.fkId_cen}', '{obj.strCedula_cin}', '{obj.strNombres_cin}', 
                 '{obj.strApellidos_cin}', '{obj.strCorreo_cin}', '{obj.strFuncion_cin}', '{obj.strTipo_cin}',
                 '{obj.strCarrera_cin}', '{obj.strFacultad_cin}', '{obj.strEntidad_cin}', 1, GETDATE())";

            _dal.InsertSql(sql);

            // Registro automático en historial al crear
            GuardarHistorial(obj.strId_cin, "NUEVO", "Ingreso inicial al Centro de Investigación", usuarioLogueado);
        }

        public void ActualizarIntegrante(InvgccCentroIntegrantes obj, string usuario)
        {
            string sql = $@"
                UPDATE INVGCCCENTRO_INVESTIGACION_INTEGRANTES SET
                strCedula_cin = '{obj.strCedula_cin}',
                strNombres_cin = '{obj.strNombres_cin}',
                strApellidos_cin = '{obj.strApellidos_cin}',
                strCorreo_cin = '{obj.strCorreo_cin}',
                strFuncion_cin = '{obj.strFuncion_cin}',
                strTipo_cin = '{obj.strTipo_cin}',
                strCarrera_cin = '{obj.strCarrera_cin}',
                strFacultad_cin = '{obj.strFacultad_cin}',
                strEntidad_cin = '{obj.strEntidad_cin}'
                WHERE strId_cin = '{obj.strId_cin}'";

            _dal.UpdateSql(sql);
            GuardarHistorial(obj.strId_cin, "EDICIÓN", "Actualización de datos generales", usuario);
        }

        public void CambiarEstadoIntegrante(string idIntegrante, string motivo, string usuario)
        {
            var integrante = ObtenerIntegrantePorId(idIntegrante);
            if (integrante != null)
            {
                int nuevoBit = integrante.bitActivo_cin ? 0 : 1;
                string accion = (nuevoBit == 1) ? "REACTIVAR" : "BAJA";

                string sqlFecha = (nuevoBit == 0) ? "GETDATE()" : "NULL";

                string sql = $@"
                    UPDATE INVGCCCENTRO_INVESTIGACION_INTEGRANTES 
                    SET bitActivo_cin = {nuevoBit}, 
                        dtFechaFin_cin = {sqlFecha}
                    WHERE strId_cin = '{idIntegrante}'";

                _dal.UpdateSql(sql);

                GuardarHistorial(idIntegrante, accion, motivo, usuario);
            }
        }

        public InvgccCentroIntegrantes BuscarDirectorDelCentro(string idCentro)
        {
            // Buscar solo el director ACTIVO
            string sql = $@"
                SELECT TOP 1 * FROM INVGCCCENTRO_INVESTIGACION_INTEGRANTES 
                WHERE fkId_cen = '{idCentro}' AND strFuncion_cin = 'Director' AND bitActivo_cin = 1";

            return _dal.SelectSql<InvgccCentroIntegrantes>(sql)?.FirstOrDefault();
        }

        // ========================
        // 3. GENERADORES DE CÓDIGO
        // ========================

        private string GenerarNuevoIdCentro()
        {
            int anio = DateTime.Now.Year;
            string prefijo = $"CEN-{anio}-";
            string sql = $"SELECT TOP 1 strId_cen FROM INVGCCCENTRO_INVESTIGACION WHERE strId_cen LIKE '{prefijo}%' ORDER BY strId_cen DESC";

            var lista = _dal.SelectSql<InvgccCentroInvestigacion>(sql);
            int siguiente = 1;

            if (lista != null && lista.Count > 0)
            {
                string ultimoId = lista[0].strId_cen;
                string numeroStr = ultimoId.Substring(ultimoId.LastIndexOf('-') + 1);
                if (int.TryParse(numeroStr, out int numeroActual))
                {
                    siguiente = numeroActual + 1;
                }
            }
            return $"{prefijo}{siguiente:D3}";
        }

        private string GenerarCodigoAlfanumerico(string tabla, string campoId, string prefijo)
        {
            string sql = $"SELECT TOP 1 {campoId} FROM {tabla} WHERE {campoId} LIKE '{prefijo}%' ORDER BY Len({campoId}) DESC, {campoId} DESC";
            var lista = _dal.SelectSql<dynamic>(sql);
            int siguienteNumero = 1;

            if (lista != null && lista.Count > 0)
            {
                string ultimoId = "";
                var item = lista[0];

                if (item is JObject jobj) ultimoId = jobj[campoId]?.ToString();
                else
                {
                    try { ultimoId = ((dynamic)item).GetType().GetProperty(campoId).GetValue(item, null).ToString(); }
                    catch { }
                }

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

        // ==========================================
        // 4. GESTIÓN DE HISTORIAL (MOVIMIENTOS)
        // ==========================================

        public List<InvgccCentroIntegrantesHistorial> ObtenerHistorial(string idIntegrante)
        {
            string sql = $"SELECT * FROM INVGCCCENTRO_INVESTIGACION_INTEGRANTES_HISTORIAL WHERE strId_cin = '{idIntegrante}' ORDER BY dtFecha DESC";
            return _dal.SelectSql<InvgccCentroIntegrantesHistorial>(sql);
        }

        public void GuardarHistorial(string idIntegrante, string accion, string motivo, string usuario)
        {
            string idHistorial = GenerarCodigoAlfanumerico("INVGCCCENTRO_INVESTIGACION_INTEGRANTES_HISTORIAL", "strId_his", "HIS");
            string fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string sql = $@"
                INSERT INTO INVGCCCENTRO_INVESTIGACION_INTEGRANTES_HISTORIAL
                (strId_his, strId_cin, dtFecha, strAccion, strMotivo, strUsuario)
                VALUES
                ('{idHistorial}', '{idIntegrante}', '{fecha}', '{accion}', '{motivo}', '{usuario}')";

            _dal.InsertSql(sql);
        }

        //

        // ==========================================
        // 5. GESTIÓN DE PAPELERA (SOLO INTEGRANTES)
        // ==========================================

        public List<InvgccCentroIntegrantes> ObtenerIntegrantesPapelera(string idCentro)
        {
            string sql = $@"
                SELECT *, (strApellidos_cin + ' ' + strNombres_cin) as NombreCompleto 
                FROM INVGCCCENTRO_INVESTIGACION_INTEGRANTES 
                WHERE fkId_cen = '{idCentro}' 
                AND (bitActivo_cin = 0 OR bitActivo_cin IS NULL) 
                ORDER BY strApellidos_cin";

            return _dal.SelectSql<InvgccCentroIntegrantes>(sql);
        }

        public bool RestaurarIntegrante(string idIntegrante, string usuario)
        {
            var integrante = ObtenerIntegrantePorId(idIntegrante);
            if (integrante == null) return false;

            if (integrante.strFuncion_cin == "Director")
            {
                var directorActual = BuscarDirectorDelCentro(integrante.fkId_cen);

                if (directorActual != null && directorActual.strId_cin != idIntegrante)
                {
                    return false;
                }
            }

            string sql = $"UPDATE INVGCCCENTRO_INVESTIGACION_INTEGRANTES SET bitActivo_cin = 1, dtFechaFin_cin = NULL WHERE strId_cin = '{idIntegrante}'";
            _dal.UpdateSql(sql);

            GuardarHistorial(idIntegrante, "RESTAURACIÓN", "Recuperado desde papelera", usuario);

            return true;
        }

        public bool ExisteDirectorActivo(string idCentro)
        {
            string sql = $@"
                SELECT * FROM INVGCCCENTRO_INVESTIGACION_INTEGRANTES 
                WHERE fkId_cen = '{idCentro}' 
                AND strFuncion_cin = 'Director' 
                AND bitActivo_cin = 1";

            var lista = _dal.SelectSql<InvgccCentroIntegrantes>(sql);
            return lista != null && lista.Count > 0;
        }

        public string GuardarCentroCompleto(InvgccCentroInvestigacion c)
        {
            c.strNombre_cen = c.strNombre_cen.ToUpper().Trim().Replace("'", "''");
            c.strMision_cen = c.strMision_cen.Replace("'", "''");
            c.strVision_cen = c.strVision_cen.Replace("'", "''");

            if (string.IsNullOrEmpty(c.strId_cen))
            {
                c.strId_cen = GenerarNuevoIdCentro();

                string sql = $@"
                    INSERT INTO INVGCCCENTRO_INVESTIGACION
                    (strId_cen, strNombre_cen, strFacultad_cen, strArea_cen, strUbicacion_cen,
                     strLineaInv_cen, strMision_cen, strVision_cen, dtFechaAprobacion_cen,
                     strResolucion_cen, strAceptacion_cen,
                     bitActivo_cen, dtFechaRegistro)
                    VALUES
                    ('{c.strId_cen}', '{c.strNombre_cen}', '{c.strFacultad_cen}',
                     '{c.strArea_cen}', '{c.strUbicacion_cen}', '{c.strLineaInv_cen}',
                     '{c.strMision_cen}', '{c.strVision_cen}',
                     '{c.dtFechaAprobacion_cen:yyyy-MM-dd}',
                     '{c.strResolucion_cen}', '{c.strAceptacion_cen}',
                     1, GETDATE())";

                _dal.InsertSql(sql);
            }
            else
            {
                string sql = $@"
                    UPDATE INVGCCCENTRO_INVESTIGACION SET
                    strNombre_cen = '{c.strNombre_cen}',
                    strFacultad_cen = '{c.strFacultad_cen}',
                    strArea_cen = '{c.strArea_cen}',
                    strUbicacion_cen = '{c.strUbicacion_cen}',
                    strLineaInv_cen = '{c.strLineaInv_cen}',
                    strMision_cen = '{c.strMision_cen}',
                    strVision_cen = '{c.strVision_cen}',
                    dtFechaAprobacion_cen = '{c.dtFechaAprobacion_cen:yyyy-MM-dd}',
                    strResolucion_cen = '{c.strResolucion_cen}',
                    strAceptacion_cen = '{c.strAceptacion_cen}'
                    WHERE strId_cen = '{c.strId_cen}'";

                _dal.UpdateSql(sql);
            }

            return c.strId_cen;
        }

        //

        public InvgccCentroIntegrantes BuscarIntegranteActivoPorCedula(string cedula)
        {
            string sql = $@"
                SELECT TOP 1 * FROM INVGCCCENTRO_INVESTIGACION_INTEGRANTES 
                WHERE strCedula_cin = '{cedula}' 
                AND bitActivo_cin = 1";

            return _dal.SelectSql<InvgccCentroIntegrantes>(sql)?.FirstOrDefault();
        }

        //

        // ==========================================
        // MÉTODO ESPECÍFICO PARA ACTUALIZAR ARCHIVOS
        // ==========================================
        public void ActualizarArchivosCentro(string idCentro, string rutaResolucion, string rutaAceptacion)
        {
            string setSql = "";

            if (!string.IsNullOrEmpty(rutaResolucion))
                setSql += $"strResolucion_cen = '{rutaResolucion}', ";

            if (!string.IsNullOrEmpty(rutaAceptacion))
                setSql += $"strAceptacion_cen = '{rutaAceptacion}', ";

            if (string.IsNullOrEmpty(setSql)) return;

            setSql = setSql.TrimEnd(',', ' ');

            string sql = $"UPDATE INVGCCCENTRO_INVESTIGACION SET {setSql} WHERE strId_cen = '{idCentro}'";

            _dal.UpdateSql(sql);
        }
    }
}