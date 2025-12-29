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
        private const string TABLA_CAT = "INVGCCCATEGORIZACION_DOCENTES";
        private const string TABLA_DOC = "INVGCCDOCENTE";

        // ==========================================
        // 1. CONSULTAS DE LISTADO (SELECT)
        // ==========================================

        public List<InvgccDocente> ObtenerDocentesCombo()
        {
            // Trae solo docentes activos para el combo
            var docentes = _dal.SelectSql<InvgccDocente>($"SELECT * FROM {TABLA_DOC} WHERE bitActivo_doc = 1");
            foreach (var d in docentes)
            {
                // Formateamos para mostrar en el DropDownList estándar
                d.strApellidos_doc = $"{d.strApellidos_doc} {d.strNombres_doc} - [{d.strCedula_doc}]";
            }
            return docentes.OrderBy(x => x.strApellidos_doc).ToList();
        }

        public List<dynamic> ObtenerCategorizacionesActivas()
        {
            // INNER JOIN para evitar errores de "Invalid Column Name" en el listado
            string sql = $@"
                SELECT 
                    C.strId_cat, 
                    C.dtFecha_cat, 
                    C.strCategorizacion,
                    D.strId_doc,
                    D.strCedula_doc, 
                    D.strNombres_doc, 
                    D.strApellidos_doc,
                    D.strFacultad_doc,
                    D.strCarrera_doc
                FROM {TABLA_CAT} C
                INNER JOIN {TABLA_DOC} D ON C.fkId_doc = D.strId_doc
                WHERE C.strEstado_cat = 'activo'";

            return _dal.SelectSql<dynamic>(sql);
        }

        public InvgccCategoriaDocentes ObtenerCategoriaPorId(string id)
        {
            string sql = $"SELECT * FROM {TABLA_CAT} WHERE strId_cat = '{id}'";
            return _dal.SelectSql<InvgccCategoriaDocentes>(sql).FirstOrDefault();
        }

        public InvgccDocente ObtenerDocentePorSql(string sql)
        {
            return _dal.SelectSql<InvgccDocente>(sql).FirstOrDefault();
        }

        // ==========================================
        // 2. LÓGICA DE VALIDACIÓN
        // ==========================================

        public bool DocenteTieneCategoria(string idDocente)
        {
            string sql = $"SELECT * FROM {TABLA_CAT} WHERE fkId_doc = '{idDocente}' AND strEstado_cat = 'activo'";
            var resultado = _dal.SelectSql<InvgccCategoriaDocentes>(sql);
            return resultado != null && resultado.Count > 0;
        }

        // ==========================================
        // 3. OPERACIONES CRUD (LIMPIAS)
        // ==========================================

        public void GuardarCategorizacion(InvgccCategoriaDocentes cat)
        {
            cat.strId_cat = GenerarCodigoAlfanumerico(TABLA_CAT, "strId_cat", "CAT");

            // OBJETO LIMPIO: Solo campos que existen físicamente en la tabla de categorización
            var datos = new
            {
                strId_cat = cat.strId_cat,
                fkId_doc = cat.fkId_doc,
                dtFecha_cat = cat.dtFecha_cat,
                strCategorizacion = cat.strCategorizacion,
                strEstado_cat = "activo"
            };

            _dal.Insert(TABLA_CAT, datos);
        }

        public void ActualizarCategorizacion(InvgccCategoriaDocentes cat)
        {
            // CORRECCIÓN: Objeto anónimo para evitar enviar columnas de 'Docente' en el UPDATE
            var datosActualizar = new
            {
                fkId_doc = cat.fkId_doc,
                dtFecha_cat = cat.dtFecha_cat,
                strCategorizacion = cat.strCategorizacion,
                strEstado_cat = "activo"
            };

            _dal.Update(TABLA_CAT, datosActualizar, $"strId_cat = '{cat.strId_cat}'");
        }

        public bool EliminarCategorizacion(string id)
        {
            // Eliminación física según tu requerimiento
            return _dal.Delete(TABLA_CAT, $"strId_cat = '{id}'");
        }

        public string GuardarDocenteSimple(InvgccDocente docente)
        {
            docente.strId_doc = GenerarCodigoAlfanumerico(TABLA_DOC, "strId_doc", "D");
            docente.bitActivo_doc = true;

            _dal.Insert(TABLA_DOC, docente);
            return docente.strId_doc;
        }

        // ==========================================
        // 4. UTILIDADES (CENTRALIZADAS)
        // ==========================================

        private string GenerarCodigoAlfanumerico(string tabla, string campoId, string prefijo)
        {
            // Query dinámico para extraer el número después del prefijo
            string sql = $@"SELECT TOP 1 {campoId} FROM {tabla} 
                            WHERE {campoId} LIKE '{prefijo}%' 
                            ORDER BY CAST(SUBSTRING({campoId}, {prefijo.Length + 1}, LEN({campoId})) AS INT) DESC";

            var lista = _dal.SelectSql<dynamic>(sql);

            int siguiente = 1;
            if (lista != null && lista.Count > 0)
            {
                // Acceso dinámico al primer campo del primer registro
                var dict = (IDictionary<string, object>)lista[0];
                string ultimoId = dict[campoId].ToString();
                siguiente = int.Parse(ultimoId.Substring(prefijo.Length)) + 1;
            }
            return prefijo + siguiente;
        }
    }
}