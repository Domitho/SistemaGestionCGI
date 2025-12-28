using System;
using System.Collections.Generic;
using System.Linq;
using SistemaGestionCGI.Models;
using SistemaGestionCGI.Settings;

namespace SistemaGestionCGI.BLL
{
    public class ManejadorCentroInvestigacion
    {
        // Instancia del DAL (Data Access Layer)
        private readonly ConnectionSqlServer _dal = ConnectionSqlServer.Instance;

        // ========================
        // LECTURA (READ)
        // ========================
        public List<InvgccCentroInvestigacion> ObtenerTodos()
        {
            // Usamos un alias 'NombreDirector' para que coincida con el JsonProperty del modelo
            string sql = @"
                SELECT C.*, 
                       (I.strApellidos_int + ' ' + I.strNombres_int) as NombreDirector
                FROM INVGCCCENTRO_INVESTIGACION C
                LEFT JOIN INVGCCGRUPO_INTEGRANTES I ON C.fkId_director = I.strId_int
                WHERE C.bitActivo_cen = 1
                ORDER BY C.strNombre_cen ASC";

            return _dal.SelectSql<InvgccCentroInvestigacion>(sql);
        }

        public InvgccCentroInvestigacion ObtenerPorId(string id)
        {
            string sql = $"SELECT * FROM INVGCCCENTRO_INVESTIGACION WHERE strId_cen = '{id}'";
            return _dal.SelectSql<InvgccCentroInvestigacion>(sql)?.FirstOrDefault();
        }

        // ========================
        // ESCRITURA (CREATE / UPDATE)
        // ========================
        public void Guardar(InvgccCentroInvestigacion centro)
        {
            // Generar ID Institucional (Ej: CEN-2025-001)
            centro.strId_cen = GenerarNuevoId();

            string sql = $@"
                INSERT INTO INVGCCCENTRO_INVESTIGACION
                (strId_cen, strNombre_cen, strFacultad_cen, strArea_cen, strUbicacion_cen, 
                 strLineaInv_cen, strMision_cen, strVision_cen, dtFechaAprobacion_cen, 
                 bitActivo_cen, fkId_director, dtFechaRegistro)
                VALUES
                ('{centro.strId_cen}', '{centro.strNombre_cen}', '{centro.strFacultad_cen}', 
                 '{centro.strArea_cen}', '{centro.strUbicacion_cen}', '{centro.strLineaInv_cen}', 
                 '{centro.strMision_cen}', '{centro.strVision_cen}', 
                 '{centro.dtFechaAprobacion_cen:yyyy-MM-dd}', 1, '{centro.fkId_director}', GETDATE())";

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
                    fkId_director = '{centro.fkId_director}'
                WHERE strId_cen = '{centro.strId_cen}'";

            _dal.UpdateSql(sql);
        }

        // ========================
        // BORRADO LÓGICO (SOFT DELETE)
        // ========================
        public void Eliminar(string id)
        {
            string sql = $"UPDATE INVGCCCENTRO_INVESTIGACION SET bitActivo_cen = 0 WHERE strId_cen = '{id}'";
            _dal.UpdateSql(sql);
        }

        // ========================
        // UTILIDADES Y COMBOS
        // ========================

        // Dentro de la clase ManejadorCentroInvestigacion

        public List<dynamic> ObtenerIntegrantesPorCentro(string idCentro)
        {
            string sql = $@"
                SELECT 
                    I.strId_int,
                    (I.strApellidos_int + ' ' + I.strNombres_int) as NombreCompleto,
                    I.strFuncion_int,
                    I.strCorreo_int,
                    I.strTipo_int, 
                    G.strNombre_gru
                FROM INVGCCGRUPO_INTEGRANTES I
                INNER JOIN INVGCCGRUPO_INVESTIGACION G ON I.fkId_gru = G.strId_gru
                WHERE G.fkId_cen = '{idCentro}' AND I.bitActivo_int = 1
                ORDER BY I.strApellidos_int ASC";

            return _dal.SelectSql<dynamic>(sql);
        }

        public List<InvgccGrupoIntegrantes> ObtenerCandidatosDirector()
        {
            // Se agregó el filtro: AND strFuncion_int = 'Investigador Principal'
            string sql = @"
                SELECT strId_int, (strApellidos_int + ' ' + strNombres_int) as NombreCompleto 
                FROM INVGCCGRUPO_INTEGRANTES 
                WHERE bitActivo_int = 1 
                AND strFuncion_int = 'Investigador Principal'
                ORDER BY strApellidos_int";

            return _dal.SelectSql<InvgccGrupoIntegrantes>(sql);
        }

        private string GenerarNuevoId()
        {
            int anio = DateTime.Now.Year;
            string prefijo = $"CEN-{anio}-";
            string sql = $"SELECT TOP 1 strId_cen FROM INVGCCCENTRO_INVESTIGACION WHERE strId_cen LIKE '{prefijo}%' ORDER BY strId_cen DESC";

            var lista = _dal.SelectSql<InvgccCentroInvestigacion>(sql);
            int siguiente = 1;

            if (lista != null && lista.Count > 0)
            {
                string ultimoId = lista[0].strId_cen;
                // Formato esperado: CEN-2025-001 (Extraer últimos 3 dígitos)
                string numeroStr = ultimoId.Substring(ultimoId.LastIndexOf('-') + 1);
                if (int.TryParse(numeroStr, out int numeroActual))
                {
                    siguiente = numeroActual + 1;
                }
            }
            return $"{prefijo}{siguiente:D3}";
        }
    }
}