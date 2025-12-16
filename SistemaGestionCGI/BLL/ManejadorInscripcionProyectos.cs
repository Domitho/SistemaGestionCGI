using System;
using System.Collections.Generic;
using SistemaGestionCGI.Models;
using SistemaGestionCGI.Settings;

namespace SistemaGestionCGI.BLL
{
    public class ManejadorInscripcionProyectos
    {
        private readonly ConnectionSqlServer _dal = ConnectionSqlServer.Instance;

        // =============================================================
        // LECTURA DE DATOS
        // =============================================================

        public List<InvgccInscripcionProyectos> ObtenerTodos()
        {
            string sql = @"
                SELECT P.strId_pro, P.strTema_pro, P.strCoordinador_pro,
                       P.strDuracion_pro, P.dtFehains_pro, P.strArchivo_pro, P.strEstado_pro, 
                       P.fkId_conv, P.fkId_gru, 
                       P.intPuntaje_pro, 
                       G.strNombre_gru, C.strNombre_conv 
                FROM INVGCCINSCRIPCION_PROYECTOS P 
                INNER JOIN INVGCCGRUPO_INVESTIGACION G ON P.fkId_gru = G.strId_gru
                INNER JOIN INVGCCCONVOCATORI C ON P.fkId_conv = C.strId_conv
                ORDER BY ISNULL(P.intPuntaje_pro, -1) DESC, P.dtFehains_pro DESC";

            return _dal.SelectSql<InvgccInscripcionProyectos>(sql);
        }

        public InvgccInscripcionProyectos ObtenerPorId(string id)
        {
            string sql = $"SELECT * FROM INVGCCINSCRIPCION_PROYECTOS WHERE strId_pro = '{id}'";
            var lista = _dal.SelectSql<InvgccInscripcionProyectos>(sql);

            if (lista != null && lista.Count > 0)
                return lista[0];
            else
                return null;
        }

        // =============================================================
        // CRUD PRINCIPAL
        // =============================================================

        public void Guardar(InvgccInscripcionProyectos pro)
        {
            int anioBase = ObtenerAnioDeConvocatoria(pro.fkId_conv);
            string nuevoId = GenerarNuevoIdProyecto(anioBase);

            pro.strId_pro = nuevoId;
            pro.strEstado_pro = "Pendiente";

            string puntajeSql = pro.intPuntaje_pro.HasValue ? pro.intPuntaje_pro.Value.ToString() : "NULL";

            string sql = $@"
                INSERT INTO INVGCCINSCRIPCION_PROYECTOS 
                (strId_pro, strTema_pro, strCoordinador_pro, strDuracion_pro, 
                 dtFehains_pro, fkId_gru, fkId_conv, strArchivo_pro, strEstado_pro, intPuntaje_pro)
                VALUES 
                ('{pro.strId_pro}', '{pro.strTema_pro}', '{pro.strCoordinador_pro}',
                 '{pro.strDuracion_pro}', '{pro.dtFehains_pro:yyyy-MM-dd}', '{pro.fkId_gru}', '{pro.fkId_conv}', 
                 '{pro.strArchivo_pro}', '{pro.strEstado_pro}', {puntajeSql})";

            _dal.UpdateSql(sql);
        }

        public void Actualizar(InvgccInscripcionProyectos pro)
        {
            string puntajeSql = pro.intPuntaje_pro.HasValue ? pro.intPuntaje_pro.Value.ToString() : "NULL";

            string sql = $@"
                UPDATE INVGCCINSCRIPCION_PROYECTOS SET 
                    strTema_pro = '{pro.strTema_pro}',
                    strCoordinador_pro = '{pro.strCoordinador_pro}',
                    strDuracion_pro = '{pro.strDuracion_pro}',
                    dtFehains_pro = '{pro.dtFehains_pro:yyyy-MM-dd}',
                    fkId_gru = '{pro.fkId_gru}',
                    fkId_conv = '{pro.fkId_conv}',
                    strArchivo_pro = '{pro.strArchivo_pro}',
                    intPuntaje_pro = {puntajeSql}
                WHERE strId_pro = '{pro.strId_pro}'";

            _dal.UpdateSql(sql);
        }

        public void Eliminar(string id)
        {
            _dal.Delete("INVGCCINSCRIPCION_PROYECTOS", $"strId_pro = '{id}'");
        }

        public void AlternarEstado(string id)
        {
            string sql = $@"
                UPDATE INVGCCINSCRIPCION_PROYECTOS
                SET strEstado_pro = CASE 
                    WHEN strEstado_pro = 'Pendiente' THEN 'Aprobado' 
                    ELSE 'Pendiente' 
                END
                WHERE strId_pro = '{id}'";

            _dal.UpdateSql(sql);
        }

        // =============================================================
        // MÉTODOS AUXILIARES (COMBOS, INFO GRUPO, INTEGRANTES)
        // =============================================================

        public List<InvgccGrupoInvestigacion> ObtenerGruposCombo()
        {
            string sql = "SELECT strId_gru, strNombre_gru FROM INVGCCGRUPO_INVESTIGACION ORDER BY strNombre_gru";
            return _dal.SelectSql<InvgccGrupoInvestigacion>(sql);
        }

        public List<InvgccConvocatoria> ObtenerConvocatoriasCombo()
        {
            string sql = "SELECT strId_conv, strNombre_conv FROM INVGCCCONVOCATORI ORDER BY strNombre_conv";
            return _dal.SelectSql<InvgccConvocatoria>(sql);
        }

        public InvgccGrupoInvestigacion ObtenerInfoGrupo(string idGrupo)
        {
            string sql = $"SELECT * FROM INVGCCGRUPO_INVESTIGACION WHERE strId_gru = '{idGrupo}'";
            var lista = _dal.SelectSql<InvgccGrupoInvestigacion>(sql);
            return (lista != null && lista.Count > 0) ? lista[0] : null;
        }

        public List<InvgccGrupoIntegrantes> ObtenerIntegrantesPorGrupo(string idGrupo)
        {
            string sql = $@"
                SELECT strId_int, (strApellidos_int + ' ' + strNombres_int) as NombreCompleto 
                FROM INVGCCGRUPO_INTEGRANTES 
                WHERE fkId_gru = '{idGrupo}' AND bitActivo_int = 1
                ORDER BY strApellidos_int";

            return _dal.SelectSql<InvgccGrupoIntegrantes>(sql);
        }

        public void GuardarIntegranteExpress(InvgccGrupoIntegrantes intg)
        {
            string nuevoId = GenerarNuevoIdIntegrante();

            string sql = $@"
                INSERT INTO INVGCCGRUPO_INTEGRANTES 
                (strId_int, strCedula_int, strApellidos_int, strNombres_int, strCorreo_int, 
                 strCarrera_int, strFuncion_int, strObservacion_int, strTipo_int, strFacultad_int,  
                 strEntidad_int, fkId_gru, bitActivo_int, dtFechaini_int, bitPertenece_int)
                VALUES 
                ('{nuevoId}', '{intg.strCedula_int}', '{intg.strApellidos_int}', '{intg.strNombres_int}', '{intg.strCorreo_int}',
                 '{intg.strCarrera_int}', '{intg.strFuncion_int}', '{intg.strObservacion_int}', '{intg.strTipo_int}', '{intg.strFacultad_int}',
                 '{intg.strEntidad_int}', '{intg.fkId_gru}', 1, GETDATE(), 1)";

            _dal.UpdateSql(sql);
        }

        // =============================================================
        // NUEVO: OBTENER AÑO DE CONVOCATORIA
        // =============================================================
        private int ObtenerAnioDeConvocatoria(string idConvocatoria)
        {
            // IMPORTANTE: Verifica que 'dtFechaInicio_conv' sea el nombre correcto en tu BD
            string sql = $"SELECT dtFechaini_conv FROM INVGCCCONVOCATORI WHERE strId_conv = '{idConvocatoria}'";

            var lista = _dal.SelectSql<dynamic>(sql);

            if (lista != null && lista.Count > 0)
            {
                try
                {
                    // Intentamos leer la fecha dinámicamente
                    dynamic item = lista[0];
                    // Si tu driver devuelve string o DateTime, Parse lo manejará
                    DateTime fecha = DateTime.Parse(item.dtFechaini_conv.ToString());
                    return fecha.Year;
                }
                catch
                {
                    return DateTime.Now.Year; // Fallback por seguridad
                }
            }
            return DateTime.Now.Year;
        }

        // =============================================================
        // GENERADORES DE ID (ACTUALIZADOS)
        // =============================================================

        // Generador INSTITUCIONAL (DIRGI-CP[Año]-001)
        private string GenerarNuevoIdProyecto(int anio)
        {
            // Prefijo basado en el año recibido (de la convocatoria)
            string prefijo = $"DIRGI-CP{anio}-";

            // Buscamos el último ID que coincida con ese año
            string sql = $"SELECT TOP 1 strId_pro FROM INVGCCINSCRIPCION_PROYECTOS WHERE strId_pro LIKE '{prefijo}%' ORDER BY strId_pro DESC";

            var lista = _dal.SelectSql<InvgccInscripcionProyectos>(sql);

            int siguienteNumero = 1; // Default: 001

            if (lista != null && lista.Count > 0)
            {
                string ultimoId = lista[0].strId_pro; // Ej: DIRGI-CP2021-003

                if (!string.IsNullOrEmpty(ultimoId) && ultimoId.Contains("-"))
                {
                    // Extraemos lo que está después del último guion
                    string numeroStr = ultimoId.Substring(ultimoId.LastIndexOf('-') + 1);

                    if (int.TryParse(numeroStr, out int numeroActual))
                    {
                        siguienteNumero = numeroActual + 1;
                    }
                }
            }

            // Retorna ej: DIRGI-CP2021-004
            return prefijo + siguienteNumero.ToString("D3");
        }

        // Generador para INTEGRANTES (Mantenemos la lógica blindada que ya tenías)
        private string GenerarNuevoIdIntegrante()
        {
            string sql = "SELECT strId_int FROM INVGCCGRUPO_INTEGRANTES WHERE strId_int LIKE 'I%'";
            var lista = _dal.SelectSql<InvgccGrupoIntegrantes>(sql);

            int max = 0;
            if (lista != null && lista.Count > 0)
            {
                foreach (var item in lista)
                {
                    string id = item.strId_int;
                    if (!string.IsNullOrEmpty(id) && id.ToUpper().StartsWith("I") && !id.ToUpper().StartsWith("INT"))
                    {
                        string numeroStr = id.Substring(1);
                        if (int.TryParse(numeroStr, out int num))
                        {
                            if (num > max) max = num;
                        }
                    }
                }
            }
            return "I" + (max + 1);
        }
    }
}