using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq; // Necesario para GenerarCodigoAlfanumerico
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
            // CORRECCIÓN: Ahora buscamos el nombre del Director en la NUEVA tabla de integrantes
            // usando una subconsulta, ya que el fkId_director ya no se usa.
            string sql = @"
                SELECT C.*, 
                (
                    SELECT TOP 1 (I.strApellidos_cin + ' ' + I.strNombres_cin)
                    FROM INVGCCCENTRO_INVESTIGACION_INTEGRANTES I
                    WHERE I.fkId_cen = C.strId_cen 
                    AND I.strFuncion_cin = 'Director' 
                    AND I.bitActivo_cin = 1
                ) as NombreDirector
                FROM INVGCCCENTRO_INVESTIGACION C
                WHERE C.bitActivo_cen = 1
                ORDER BY C.strNombre_cen ASC";

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

            // CORRECCIÓN: Se eliminó fkId_director del INSERT para evitar el error de Foreign Key
            string sql = $@"
                INSERT INTO INVGCCCENTRO_INVESTIGACION
                (strId_cen, strNombre_cen, strFacultad_cen, strArea_cen, strUbicacion_cen, 
                 strLineaInv_cen, strMision_cen, strVision_cen, dtFechaAprobacion_cen, 
                 bitActivo_cen, dtFechaRegistro)
                VALUES
                ('{centro.strId_cen}', '{centro.strNombre_cen}', '{centro.strFacultad_cen}', 
                 '{centro.strArea_cen}', '{centro.strUbicacion_cen}', '{centro.strLineaInv_cen}', 
                 '{centro.strMision_cen}', '{centro.strVision_cen}', 
                 '{centro.dtFechaAprobacion_cen:yyyy-MM-dd}', 1, GETDATE())";

            _dal.InsertSql(sql);
        }

        public void Actualizar(InvgccCentroInvestigacion centro)
        {
            // CORRECCIÓN: Se eliminó fkId_director del UPDATE
            string sql = $@"
                UPDATE INVGCCCENTRO_INVESTIGACION SET
                strNombre_cen = '{centro.strNombre_cen}',
                strFacultad_cen = '{centro.strFacultad_cen}',
                strArea_cen = '{centro.strArea_cen}',
                strUbicacion_cen = '{centro.strUbicacion_cen}',
                strLineaInv_cen = '{centro.strLineaInv_cen}',
                strMision_cen = '{centro.strMision_cen}',
                strVision_cen = '{centro.strVision_cen}',
                dtFechaAprobacion_cen = '{centro.dtFechaAprobacion_cen:yyyy-MM-dd}'
                WHERE strId_cen = '{centro.strId_cen}'";

            _dal.UpdateSql(sql);
        }

        public void Eliminar(string id)
        {
            string sql = $"UPDATE INVGCCCENTRO_INVESTIGACION SET bitActivo_cen = 0 WHERE strId_cen = '{id}'";
            _dal.UpdateSql(sql);
        }

        // ==========================================
        // 2. GESTIÓN DE INTEGRANTES DEL CENTRO
        // ==========================================

        public List<InvgccCentroIntegrantes> ObtenerIntegrantesPorCentro(string idCentro)
        {
            string sql = $@"
                SELECT * FROM INVGCCCENTRO_INVESTIGACION_INTEGRANTES 
                WHERE fkId_cen = '{idCentro}' 
                ORDER BY strApellidos_cin ASC";

            return _dal.SelectSql<InvgccCentroIntegrantes>(sql);
        }

        public InvgccCentroIntegrantes ObtenerIntegrantePorId(string idIntegrante)
        {
            string sql = $"SELECT * FROM INVGCCCENTRO_INVESTIGACION_INTEGRANTES WHERE strId_cin = '{idIntegrante}'";
            return _dal.SelectSql<InvgccCentroIntegrantes>(sql)?.FirstOrDefault();
        }

        public void GuardarIntegrante(InvgccCentroIntegrantes obj)
        {
            // Usamos el método genérico que faltaba
            obj.strId_cin = GenerarCodigoAlfanumerico("INVGCCCENTRO_INVESTIGACION_INTEGRANTES", "strId_cin", "CIN");

            string sql = $@"
                INSERT INTO INVGCCCENTRO_INVESTIGACION_INTEGRANTES
                (strId_cin, fkId_cen, strCedula_cin, strNombres_cin, strApellidos_cin, strCorreo_cin,
                 strFuncion_cin, strTipo_cin, strCarrera_cin, strFacultad_cin, strEntidad_cin, bitActivo_cin)
                VALUES
                ('{obj.strId_cin}', '{obj.fkId_cen}', '{obj.strCedula_cin}', '{obj.strNombres_cin}', 
                 '{obj.strApellidos_cin}', '{obj.strCorreo_cin}', '{obj.strFuncion_cin}', '{obj.strTipo_cin}',
                 '{obj.strCarrera_cin}', '{obj.strFacultad_cin}', '{obj.strEntidad_cin}', 1)";

            _dal.InsertSql(sql);
        }

        public void ActualizarIntegrante(InvgccCentroIntegrantes obj)
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
        }

        public void EliminarIntegrante(string id)
        {
            string sql = $"DELETE FROM INVGCCCENTRO_INVESTIGACION_INTEGRANTES WHERE strId_cin = '{id}'";
            _dal.DeleteSql(sql);
        }

        public InvgccCentroIntegrantes BuscarDirectorDelCentro(string idCentro)
        {
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

        // METODO AGREGADO: Necesario para los integrantes
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
            // Ordenamos por fecha descendente para ver lo más reciente arriba
            string sql = $"SELECT * FROM INVGCCCENTRO_INVESTIGACION_INTEGRANTES_HISTORIAL WHERE strId_cin = '{idIntegrante}' ORDER BY dtFecha DESC";
            return _dal.SelectSql<InvgccCentroIntegrantesHistorial>(sql);
        }

        public void GuardarHistorial(string idIntegrante, string accion, string motivo, string usuario)
        {
            // Generamos ID: HIS-1, HIS-2, etc.
            string idHistorial = GenerarCodigoAlfanumerico("INVGCCCENTRO_INVESTIGACION_INTEGRANTES_HISTORIAL", "strId_his", "HIS");
            string fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string sql = $@"
                INSERT INTO INVGCCCENTRO_INVESTIGACION_INTEGRANTES_HISTORIAL
                (strId_his, strId_cin, dtFecha, strAccion, strMotivo, strUsuario)
                VALUES
                ('{idHistorial}', '{idIntegrante}', '{fecha}', '{accion}', '{motivo}', '{usuario}')";

            _dal.InsertSql(sql);
        }

        // ==========================================
        // ACTUALIZACIÓN DE MÉTODOS DE INTEGRANTES
        // ==========================================

        // IMPORTANTE: Ahora este método recibe el 'usuarioLogueado' para el historial
        public void GuardarIntegrante(InvgccCentroIntegrantes obj, string usuarioLogueado)
        {
            obj.strId_cin = GenerarCodigoAlfanumerico("INVGCCCENTRO_INVESTIGACION_INTEGRANTES", "strId_cin", "CIN");

            string sql = $@"
                INSERT INTO INVGCCCENTRO_INVESTIGACION_INTEGRANTES
                (strId_cin, fkId_cen, strCedula_cin, strNombres_cin, strApellidos_cin, strCorreo_cin,
                 strFuncion_cin, strTipo_cin, strCarrera_cin, strFacultad_cin, strEntidad_cin, bitActivo_cin)
                VALUES
                ('{obj.strId_cin}', '{obj.fkId_cen}', '{obj.strCedula_cin}', '{obj.strNombres_cin}', 
                 '{obj.strApellidos_cin}', '{obj.strCorreo_cin}', '{obj.strFuncion_cin}', '{obj.strTipo_cin}',
                 '{obj.strCarrera_cin}', '{obj.strFacultad_cin}', '{obj.strEntidad_cin}', 1)";

            _dal.InsertSql(sql);

            // REGISTRO AUTOMÁTICO EN HISTORIAL
            GuardarHistorial(obj.strId_cin, "NUEVO", "Ingreso inicial al Centro de Investigación", usuarioLogueado);
        }

        // Este método maneja el botón de "Dar de Baja / Reactivar"
        public void CambiarEstadoIntegrante(string idIntegrante, string motivo, string usuario)
        {
            // 1. Obtenemos el integrante para saber su estado actual
            var integrante = ObtenerIntegrantePorId(idIntegrante);
            if (integrante != null)
            {
                // 2. Invertimos el estado (Si es true pasa a false, y viceversa)
                bool nuevoEstado = !integrante.bitActivo_cin;
                int bit = nuevoEstado ? 1 : 0;

                // 3. Definimos la acción para el historial
                string accion = nuevoEstado ? "REACTIVAR" : "BAJA";

                // 4. Actualizamos la tabla principal
                string sql = $"UPDATE INVGCCCENTRO_INVESTIGACION_INTEGRANTES SET bitActivo_cin = {bit} WHERE strId_cin = '{idIntegrante}'";
                _dal.UpdateSql(sql);

                // 5. Guardamos en el historial
                GuardarHistorial(idIntegrante, accion, motivo, usuario);
            }
        }

    }
}