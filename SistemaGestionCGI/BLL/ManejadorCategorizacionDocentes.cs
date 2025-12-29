using System;
using System.Collections.Generic;
using System.Linq;
using SistemaGestionCGI.Models;
using SistemaGestionCGI.Settings;

namespace SistemaGestionCGI.BLL
{
    public class ManejadorCategorizacionDocentes
    {
        // Instancia Singleton del DAL (Base de Datos)
        private readonly ConnectionSqlServer _dal = ConnectionSqlServer.Instance;

        // ==========================================
        // 1. OBTENER TODOS (Para la Grilla Principal)
        // ==========================================
        public List<InvgccCategorizacionDocentes> ObtenerTodos()
        {
            // Traemos todos los docentes activos, tengan o no categoría asignada
            // Concatenamos Apellidos y Nombres para mostrarlo limpio en la vista
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
            // Usamos interpolación de strings segura
            string sql = $"SELECT * FROM INVGCCCATEGORIZACION_DOCENTES WHERE strId_doc = '{id}'";

            var lista = _dal.SelectSql<InvgccCategorizacionDocentes>(sql);
            return lista?.FirstOrDefault();
        }

        // ==========================================
        // 3. GUARDAR / ACTUALIZAR CATEGORÍA (NÚCLEO)
        // ==========================================
        public void GuardarCategorizacion(string idDocente, string nuevaCategoria, DateTime fecha, string usuario, string motivo)
        {
            // PASO A: Obtenemos el dato actual para poder comparar (Auditoría)
            var docenteActual = ObtenerPorId(idDocente);

            // Si no tiene categoría previa, lo manejamos como "SIN ASIGNAR"
            string catAnterior = docenteActual?.strCategorizacion ?? "SIN ASIGNAR";

            // Verificamos si realmente hubo un cambio de categoría
            bool huboCambio = (catAnterior != nuevaCategoria);

            // PASO B: Actualizamos la Tabla Maestra (Unificada)
            // Solo tocamos los campos de categoría, respetando los datos personales
            string sqlUpdate = $@"
                UPDATE INVGCCCATEGORIZACION_DOCENTES SET
                    strCategorizacion = '{nuevaCategoria}',
                    dtFechaCategorizacion = '{fecha:yyyy-MM-dd}'
                WHERE strId_doc = '{idDocente}'";

            _dal.UpdateSql(sqlUpdate);

            // PASO C: Insertar en el Historial SOLO si cambió la categoría o si es la primera vez
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

            // Ponemos los campos en NULL (No borramos al docente, solo su categoría)
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

        // EN: ManejadorCategorizacionDocentes.cs

        // A. Generar ID Automático
        private string GenerarNuevoIdDocente()
        {
            string prefijo = "DOC";
            // Ordenar por longitud para que DOC100 no salga antes de DOC99
            string sql = $"SELECT TOP 1 strId_doc FROM INVGCCCATEGORIZACION_DOCENTES WHERE strId_doc LIKE '{prefijo}%' ORDER BY LEN(strId_doc) DESC, strId_doc DESC";

            var lista = _dal.SelectSql<InvgccCategorizacionDocentes>(sql);
            int siguiente = 1;

            if (lista != null && lista.Count > 0)
            {
                string ultimoId = lista[0].strId_doc;
                // Cortar "DOC" y parsear el número
                if (int.TryParse(ultimoId.Substring(3), out int num))
                    siguiente = num + 1;
            }
            return $"{prefijo}{siguiente:D3}"; // Retorna DOC001, DOC002...
        }

        // B. Método Guardar Completo (Insertar o Actualizar)
        // MÉTODO: GuardarDocenteCompleto

        public void GuardarDocenteCompleto(InvgccCategorizacionDocentes obj, string usuario, string motivo)
        {
            // 1. SI ES NUEVO (INSERT)
            if (string.IsNullOrEmpty(obj.strId_doc))
            {
                obj.strId_doc = GenerarNuevoIdDocente();

                // CORRECCIÓN: Se agregó strCarrera_doc
                string sqlInsert = $@"
                    INSERT INTO INVGCCCATEGORIZACION_DOCENTES
                    (strId_doc, strCedula_doc, strNombres_doc, strApellidos_doc, strFacultad_doc, strCarrera_doc, bitActivo_doc, strCategorizacion, dtFechaCategorizacion)
                    VALUES
                    ('{obj.strId_doc}', '{obj.strCedula_doc}', '{obj.strNombres_doc}', '{obj.strApellidos_doc}', '{obj.strFacultad_doc}', '{obj.strCarrera_doc}', 1, '{obj.strCategorizacion}', '{obj.dtFechaCategorizacion:yyyy-MM-dd}')";

                _dal.UpdateSql(sqlInsert);

                // Registrar Historial Automático
                RegistrarHistorial(obj.strId_doc, "NUEVO INGRESO", "NO EXISTÍA", obj.strCategorizacion, motivo, usuario);
            }
            // 2. SI YA EXISTE (UPDATE)
            else
            {
                // CORRECCIÓN: Se agregó strCarrera_doc al Update
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
        // MÉTODOS PRIVADOS (AYUDANTES)
        // ==========================================
        private void RegistrarHistorial(string idDoc, string accion, string anterior, string nuevo, string motivo, string usuario)
        {
            // Insertamos el registro de auditoría
            string sql = $@"
                INSERT INTO INVGCCCATEGORIZACION_DOCENTES_HISTORIAL
                (fkId_doc, dtFecha, strAccion, strValorAnterior, strValorNuevo, strMotivo, strUsuario)
                VALUES
                ('{idDoc}', GETDATE(), '{accion}', '{anterior}', '{nuevo}', '{motivo}', '{usuario}')";

            _dal.UpdateSql(sql);
        }
    }
}