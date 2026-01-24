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
        // 1. LECTURA DE DATOS
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
                    dtFechaCategorizacion,
                    strCertificado_doc
                FROM INVGCCCATEGORIZACION_DOCENTES
                WHERE bitActivo_doc = 1
                ORDER BY strApellidos_doc";

            return _dal.SelectSql<InvgccCategorizacionDocentes>(sql);
        }

        public InvgccCategorizacionDocentes ObtenerPorId(string id)
        {
            // Sanitizamos el ID por seguridad
            string idSanitizado = Limpiar(id);
            string sql = $"SELECT * FROM INVGCCCATEGORIZACION_DOCENTES WHERE strId_doc = '{idSanitizado}'";

            var lista = _dal.SelectSql<InvgccCategorizacionDocentes>(sql);
            return lista?.FirstOrDefault();
        }

        public List<InvgccCategorizacionDocentesHistorial> ObtenerHistorial(string idDoc)
        {
            string idSanitizado = Limpiar(idDoc);
            string sql = $"SELECT * FROM INVGCCCATEGORIZACION_DOCENTES_HISTORIAL WHERE fkId_doc = '{idSanitizado}' ORDER BY dtFecha DESC";
            return _dal.SelectSql<InvgccCategorizacionDocentesHistorial>(sql);
        }

        // ==========================================
        // 2. LÓGICA DE NEGOCIO PRINCIPAL (GUARDAR)
        // ==========================================

        public bool ValidarCedulaEcuatoriana(string cedula)
        {
            if (string.IsNullOrEmpty(cedula) || cedula.Length != 10) return false;

            if (!long.TryParse(cedula, out _)) return false;

            int provincia = int.Parse(cedula.Substring(0, 2));
            if (provincia < 1 || provincia > 24) return false;

            int tercerDigito = int.Parse(cedula.Substring(2, 1));
            if (tercerDigito > 6) return false;

            int[] coeficientes = { 2, 1, 2, 1, 2, 1, 2, 1, 2 };
            int suma = 0;

            for (int i = 0; i < coeficientes.Length; i++)
            {
                int valor = int.Parse(cedula[i].ToString()) * coeficientes[i];
                suma += (valor >= 10) ? valor - 9 : valor;
            }

            int digitoVerificador = int.Parse(cedula[9].ToString());
            int superior = ((suma / 10) + 1) * 10;
            if (suma % 10 == 0) superior = suma;

            return (superior - suma) == digitoVerificador;
        }

        public bool ExisteCedula(string cedula, string idActual = "")
        {
            string sql = $"SELECT * FROM INVGCCCATEGORIZACION_DOCENTES WHERE strCedula_doc = '{Limpiar(cedula)}' AND bitActivo_doc = 1";

            if (!string.IsNullOrEmpty(idActual))
                sql += $" AND strId_doc <> '{Limpiar(idActual)}'";

            var lista = _dal.SelectSql<InvgccCategorizacionDocentes>(sql);
            return lista != null && lista.Count > 0;
        }

        public void GuardarDocenteCompleto(InvgccCategorizacionDocentes obj, string usuario, string motivo)
        {
            string cedula = Limpiar(obj.strCedula_doc);
            string nombres = Limpiar(obj.strNombres_doc);
            string apellidos = Limpiar(obj.strApellidos_doc);
            string facultad = Limpiar(obj.strFacultad_doc);
            string carrera = Limpiar(obj.strCarrera_doc);
            string categoria = Limpiar(obj.strCategorizacion);
            string fechaCat = obj.dtFechaCategorizacion.HasValue ? $"'{obj.dtFechaCategorizacion:yyyy-MM-dd}'" : "NULL";
            string certificado = Limpiar(obj.strCertificado_doc);

            if (string.IsNullOrEmpty(obj.strId_doc))
            {
                obj.strId_doc = GenerarNuevoIdDocente();

                string sqlInsert = $@"
                    INSERT INTO INVGCCCATEGORIZACION_DOCENTES
                    (strId_doc, strCedula_doc, strNombres_doc, strApellidos_doc, strFacultad_doc, strCarrera_doc, bitActivo_doc, strCategorizacion, dtFechaCategorizacion, strCertificado_doc)
                    VALUES
                    ('{obj.strId_doc}', '{cedula}', '{nombres}', '{apellidos}', '{facultad}', '{carrera}', 1, '{categoria}', {fechaCat}, '{certificado}')";

                _dal.UpdateSql(sqlInsert);

                RegistrarHistorial(obj.strId_doc, "NUEVO INGRESO", "NO EXISTÍA", categoria, motivo, usuario);
            }
            else
            {
                var actual = ObtenerPorId(obj.strId_doc);
                string catAnterior = actual?.strCategorizacion ?? "SIN ASIGNAR";
                string catNueva = string.IsNullOrEmpty(obj.strCategorizacion) ? "SIN ASIGNAR" : obj.strCategorizacion;
                bool huboCambioCategoria = (catAnterior != catNueva);

                string sqlUpdate = $@"
                    UPDATE INVGCCCATEGORIZACION_DOCENTES SET
                        strCedula_doc = '{cedula}',
                        strNombres_doc = '{nombres}',
                        strApellidos_doc = '{apellidos}',
                        strFacultad_doc = '{facultad}',
                        strCarrera_doc = '{carrera}',
                        strCategorizacion = '{categoria}',
                        dtFechaCategorizacion = {fechaCat},
                        strCertificado_doc = '{certificado}'
                    WHERE strId_doc = '{obj.strId_doc}'";

                _dal.UpdateSql(sqlUpdate); 

                if (huboCambioCategoria)
                {
                    string accion = (catAnterior == "SIN ASIGNAR") ? "ASIGNACION INICIAL" : "CAMBIO DE CATEGORIA";
                    RegistrarHistorial(obj.strId_doc, accion, catAnterior, catNueva, motivo, usuario);
                }
                else
                {
                    RegistrarHistorial(obj.strId_doc, "ACTUALIZACION DATOS", "N/A", "N/A", "Modificación de datos personales/académicos", usuario);
                }
            }
        }

        // ==========================================
        // 3. OPERACIONES ESPECÍFICAS
        // ==========================================

        public void EliminarCategorizacion(string idDocente, string usuario, string motivo)
        {
            var actual = ObtenerPorId(idDocente);
            string catAnterior = actual?.strCategorizacion ?? "SIN ASIGNAR";

            string sql = $"UPDATE INVGCCCATEGORIZACION_DOCENTES SET strCategorizacion = NULL, dtFechaCategorizacion = NULL WHERE strId_doc = '{Limpiar(idDocente)}'";
            _dal.UpdateSql(sql);

            RegistrarHistorial(idDocente, "ELIMINACION CATEGORIA", catAnterior, "SIN ASIGNAR", motivo, usuario);
        }

        // ==========================================
        // 4. MÉTODOS PRIVADOS Y UTILITARIOS
        // ==========================================

        private void RegistrarHistorial(string idDoc, string accion, string anterior, string nuevo, string motivo, string usuario)
        {
            string sql = $@"
                INSERT INTO INVGCCCATEGORIZACION_DOCENTES_HISTORIAL
                (fkId_doc, dtFecha, strAccion, strValorAnterior, strValorNuevo, strMotivo, strUsuario)
                VALUES
                ('{idDoc}', GETDATE(), '{accion}', '{Limpiar(anterior)}', '{Limpiar(nuevo)}', '{Limpiar(motivo)}', '{Limpiar(usuario)}')";

            _dal.UpdateSql(sql);
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
                // Extrae números del final (DOC005 -> 5)
                if (int.TryParse(ultimoId.Substring(prefijo.Length), out int num))
                    siguiente = num + 1;
            }
            return $"{prefijo}{siguiente:D3}";
        }

        private string Limpiar(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return "";
            return texto.Replace("'", "''").Trim().ToUpper();
        }

        //
        public void DarDeBajaDocente(string idDocente, string usuario, string motivo)
        {
            string sql = $"UPDATE INVGCCCATEGORIZACION_DOCENTES SET bitActivo_doc = 0 WHERE strId_doc = '{Limpiar(idDocente)}'";
            _dal.UpdateSql(sql);

            RegistrarHistorial(idDocente, "BAJA A PAPELERA", "ACTIVO", "INACTIVO", motivo, usuario);
        }

        public bool RestaurarDocente(string idDocente, string usuario)
        {
            var docente = ObtenerPorId(idDocente);
            if (docente == null) return false;

            if (ExisteCedula(docente.strCedula_doc))
            {
                return false;
            }

            string sql = $"UPDATE INVGCCCATEGORIZACION_DOCENTES SET bitActivo_doc = 1 WHERE strId_doc = '{Limpiar(idDocente)}'";
            _dal.UpdateSql(sql);

            RegistrarHistorial(idDocente, "RESTAURACIÓN", "INACTIVO", "ACTIVO", "Recuperado de papelera", usuario);
            return true;
        }

        public List<InvgccCategorizacionDocentes> ObtenerPapelera()
        {
            string sql = "SELECT *, (strApellidos_doc + ' ' + strNombres_doc) as NombreCompleto FROM INVGCCCATEGORIZACION_DOCENTES WHERE bitActivo_doc = 0";
            return _dal.SelectSql<InvgccCategorizacionDocentes>(sql);
        }

    }
}