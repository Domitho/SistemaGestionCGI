using System;
using System.Linq;
using System.Collections.Generic;
using SistemaGestionCGI.Models;
using SistemaGestionCGI.Settings;

namespace SistemaGestionCGI.BLL
{
    public class ManejadorCertificados
    {
        private readonly ConnectionSqlServer _dal = ConnectionSqlServer.Instance;

        private string ObtenerSelectBase()
        {
            return @"
                SELECT
                    i.strId_int AS IdIntegrante,
                    i.strCedula_int AS Cedula,
                    i.strNombres_int AS Nombres,
                    i.strApellidos_int AS Apellidos,
                    'GRUPOS DE INVESTIGACION' AS Modulo,
                    g.strId_gru AS IdGrupo,
                    g.strNombre_gru AS NombreGrupo,
                    i.strFuncion_int AS Funcion,
                    CASE 
                        WHEN i.bitActivo_int = 1 THEN 'Activo'
                        ELSE 'Inactivo'
                    END AS Estado,
                    ISNULL(CONVERT(VARCHAR(10), i.dtFechaini_int, 103), '') AS FechaInicio,
                    ISNULL(CONVERT(VARCHAR(10), i.dtFechafin_int, 103), '') AS FechaFin,
                    ISNULL(i.strCertificado_int, '') AS Certificado
                FROM INVGCCGRUPO_INTEGRANTES i
                INNER JOIN INVGCCGRUPO_INVESTIGACION g
                    ON g.strId_gru = i.fkId_gru ";
        }

        public List<CertificadoGrupoDTO> BuscarGruposPorCedula(string cedula)
        {
            cedula = LimpiarTexto(cedula);

            string sql = $@"
                {ObtenerSelectBase()}
                WHERE i.strCedula_int = '{cedula}'
                ORDER BY i.dtFechaini_int DESC";

            return _dal.SelectSql<CertificadoGrupoDTO>(sql) ?? new List<CertificadoGrupoDTO>();
        }

        public CertificadoGrupoDTO ObtenerCertificadoGrupoPorIdIntegrante(string idIntegrante)
        {
            idIntegrante = LimpiarTexto(idIntegrante);

            string sql = $@"
                SELECT TOP 1 *
                FROM (
                    {ObtenerSelectBase()}
                ) AS Consulta
                WHERE Consulta.IdIntegrante = '{idIntegrante}'";

            return _dal.SelectSql<CertificadoGrupoDTO>(sql)?.FirstOrDefault();
        }

        public CertificadoGrupoDTO ObtenerDatosPersonaPorCedula(string cedula)
        {
            var lista = BuscarGruposPorCedula(cedula);
            return lista != null && lista.Count > 0 ? lista[0] : null;
        }

        public bool ExistePersonaEnGrupos(string cedula)
        {
            cedula = LimpiarTexto(cedula);

            string sql = $@"
                SELECT TOP 1
                    i.strId_int AS IdIntegrante
                FROM INVGCCGRUPO_INTEGRANTES i
                WHERE i.strCedula_int = '{cedula}'";

            var resultado = _dal.SelectSql<CertificadoGrupoDTO>(sql);
            return resultado != null && resultado.Count > 0;
        }

        private string LimpiarTexto(string texto)
        {
            return (texto ?? string.Empty).Trim().Replace("'", "''");
        }


        public List<HistorialIntegranteDTO> ObtenerHistorialPorIdIntegrante(string idIntegrante)
        {
            idIntegrante = LimpiarTexto(idIntegrante);

            string sql = $@"
                SELECT
                    idHistorial AS IdHistorial,
                    strId_int AS IdIntegrante,
                    ISNULL(CONVERT(VARCHAR(10), dtFecha, 103), '') AS Fecha,
                    ISNULL(strAccion, '') AS Accion,
                    ISNULL(strMotivo, '') AS Motivo,
                    ISNULL(strUsuario, '') AS Usuario
                FROM INVGCCINTEGRANTES_HISTORIAL
                WHERE strId_int = '{idIntegrante}'
                ORDER BY dtFecha ASC, idHistorial ASC";

            return _dal.SelectSql<HistorialIntegranteDTO>(sql) ?? new List<HistorialIntegranteDTO>();
        }

    }
}