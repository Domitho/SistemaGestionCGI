using System;
using System.Collections.Generic;
using System.Linq;
using SistemaGestionCGI.Models;
using SistemaGestionCGI.Settings;

namespace SistemaGestionCGI.BLL
{
    public class ManejadorCategorizacionDocentes
    {
        private readonly ConnectionSqlServer _dal = ConnectionSqlServer.Instance;

        // ==========================================
        // 1. OBTENER TODOS (Para la Grilla Principal)
        // ==========================================
        public List<InvgccCategorizacionDocentes> ObtenerTodos()
        {
            string sql = @"
                SELECT 
                    strId_doc, 
                    strCedula_doc, 
                    strFacultad_doc, 
                    strCarrera_doc,
                    bitActivo_doc,
                    (ISNULL(strApellidos_doc, '') + ' ' + ISNULL(strNombres_doc, '')) AS NombreCompleto,
                    strCategorizacion, 
                    dtFechaCategorizacion
                FROM INVGCCCATEGORIZACION_DOCENTES
                WHERE bitActivo_doc = 1
                ORDER BY strApellidos_doc";

            return _dal.SelectSql<InvgccCategorizacionDocentes>(sql);
        }

        // ==========================================
        // 2. OBTENER POR ID (Para cargar el Formulario)
        // ==========================================
        public InvgccCategorizacionDocentes ObtenerPorId(string id)
        {
            string sql = $"SELECT * FROM INVGCCCATEGORIZACION_DOCENTES WHERE strId_doc = '{id}'";

            var lista = _dal.SelectSql<InvgccCategorizacionDocentes>(sql);
            return lista?.FirstOrDefault();
        }

        // ==========================================
        // 3. GUARDAR / ACTUALIZAR CATEGORÍA (NÚCLEO)
        // ==========================================
        public void GuardarCategorizacion(string idDocente, string nuevaCategoria, DateTime fecha, string usuario, string motivo)
        {
            var docenteActual = ObtenerPorId(idDocente);

            string catAnterior = docenteActual?.strCategorizacion ?? "SIN ASIGNAR";

            bool huboCambio = (catAnterior != nuevaCategoria);

            string sqlUpdate = $@"
                UPDATE INVGCCCATEGORIZACION_DOCENTES SET
                    strCategorizacion = '{nuevaCategoria}',
                    dtFechaCategorizacion = '{fecha:yyyy-MM-dd}'
                WHERE strId_doc = '{idDocente}'";

            _dal.UpdateSql(sqlUpdate);

            if (huboCambio)
            {
                string accion = (catAnterior == "SIN ASIGNAR") ? "ASIGNACION INICIAL" : "CAMBIO DE CATEGORIA";

                RegistrarHistorial(idDocente, accion, catAnterior, nuevaCategoria, motivo, usuario);
            }
        }

        // ==========================================
        // 4. ELIMINAR / LIMPIAR CATEGORÍA
        // ==========================================
        public void EliminarCategorizacion(string idDocente, string usuario, string motivo)
        {
            var actual = ObtenerPorId(idDocente);
            string catAnterior = actual?.strCategorizacion ?? "SIN ASIGNAR";

            string sql = $"UPDATE INVGCCCATEGORIZACION_DOCENTES SET strCategorizacion = NULL, dtFechaCategorizacion = NULL WHERE strId_doc = '{idDocente}'";
            _dal.UpdateSql(sql);

            // Dejamos rastro en el historial
            RegistrarHistorial(idDocente, "ELIMINACION", catAnterior, "SIN ASIGNAR", motivo, usuario);
        }

        // ==========================================
        // 5. OBTENER HISTORIAL (Para el Modal)
        // ==========================================
        public List<InvgccCategorizacionDocentesHistorial> ObtenerHistorial(string idDoc)
        {
            string sql = $"SELECT * FROM INVGCCCATEGORIZACION_DOCENTES_HISTORIAL WHERE fkId_doc = '{idDoc}' ORDER BY dtFecha DESC";
            return _dal.SelectSql<InvgccCategorizacionDocentesHistorial>(sql);
        }

        private string GenerarNuevoIdDocente()
        {
            string prefijo = "DOC";
            string sql = $"SELECT TOP 1 strId_doc FROM INVGCCCATEGORIZACION_DOCENTES WHERE strId_doc LIKE '{prefijo}%' ORDER BY LEN(strId_doc) DESC, strId_doc DESC";

            var lista = _dal.SelectSql<InvgccCategorizacionDocentes>(sql);
            int siguiente = 1;

            if (lista != null && lista.Count > 0)
            {
                string ultimoId = lista[0].strId_doc;
                if (int.TryParse(ultimoId.Substring(3), out int num))
                    siguiente = num + 1;
            }
            return $"{prefijo}{siguiente:D3}"; 
        }


        public void GuardarDocenteCompleto(InvgccCategorizacionDocentes obj, string usuario, string motivo)
        {
            if (string.IsNullOrEmpty(obj.strId_doc))
            {
                obj.strId_doc = GenerarNuevoIdDocente();

                string sqlInsert = $@"
                    INSERT INTO INVGCCCATEGORIZACION_DOCENTES
                    (strId_doc, strCedula_doc, strNombres_doc, strApellidos_doc, strFacultad_doc, strCarrera_doc, bitActivo_doc, strCategorizacion, dtFechaCategorizacion)
                    VALUES
                    ('{obj.strId_doc}', '{obj.strCedula_doc}', '{obj.strNombres_doc}', '{obj.strApellidos_doc}', '{obj.strFacultad_doc}', '{obj.strCarrera_doc}', 1, '{obj.strCategorizacion}', '{obj.dtFechaCategorizacion:yyyy-MM-dd}')";

                _dal.UpdateSql(sqlInsert);

                RegistrarHistorial(obj.strId_doc, "NUEVO INGRESO", "NO EXISTÍA", obj.strCategorizacion, motivo, usuario);
            }
            else
            {
                string sqlUpdateDatos = $@"
                    UPDATE INVGCCCATEGORIZACION_DOCENTES SET
                        strCedula_doc = '{obj.strCedula_doc}',
                        strNombres_doc = '{obj.strNombres_doc}',
                        strApellidos_doc = '{obj.strApellidos_doc}',
                        strFacultad_doc = '{obj.strFacultad_doc}',
                        strCarrera_doc = '{obj.strCarrera_doc}'
                    WHERE strId_doc = '{obj.strId_doc}'";

                _dal.UpdateSql(sqlUpdateDatos);

                GuardarCategorizacion(obj.strId_doc, obj.strCategorizacion, obj.dtFechaCategorizacion.Value, usuario, motivo);
            }
        }

        // ==========================================
        // MÉTODOS PRIVADOS
        // ==========================================
        private void RegistrarHistorial(string idDoc, string accion, string anterior, string nuevo, string motivo, string usuario)
        {
            string sql = $@"
                INSERT INTO INVGCCCATEGORIZACION_DOCENTES_HISTORIAL
                (fkId_doc, dtFecha, strAccion, strValorAnterior, strValorNuevo, strMotivo, strUsuario)
                VALUES
                ('{idDoc}', GETDATE(), '{accion}', '{anterior}', '{nuevo}', '{motivo}', '{usuario}')";

            _dal.UpdateSql(sql);
        }
    }
}