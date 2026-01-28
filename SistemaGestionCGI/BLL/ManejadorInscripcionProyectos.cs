using System;
using System.Collections.Generic;
using System.Linq;
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
                SELECT P.strId_pro, P.strTema_pro, 
                       P.fkId_coordinador, 
                       P.strCoordinador_pro, 
                       P.strCedulaCoordinador_pro,

                       ISNULL(
                           (I.strApellidos_int + ' ' + I.strNombres_int + ' (' + I.strFuncion_int + ')'), 
                           P.strCoordinador_pro
                       ) as NombreCoordinadorCompleto,

                       P.strDuracion_pro, P.dtFehains_pro, P.strEstado_pro, P.intPuntaje_pro,
                       P.strArchivo_pro, 
                       G.strNombre_gru, C.strNombre_conv
                FROM INVGCCINSCRIPCION_PROYECTOS P 
                INNER JOIN INVGCCGRUPO_INVESTIGACION G ON P.fkId_gru = G.strId_gru
                INNER JOIN INVGCCCONVOCATORIA_GRUPOS_INVESTIGACION C ON P.fkId_conv = C.strId_conv
                LEFT JOIN INVGCCGRUPO_INTEGRANTES I ON P.fkId_coordinador = I.strId_int
                ORDER BY ISNULL(P.intPuntaje_pro, -1) DESC, P.dtFehains_pro DESC";

            return _dal.SelectSql<InvgccInscripcionProyectos>(sql);
        }

        public InvgccInscripcionProyectos ObtenerPorId(string id)
        {
            string sql = $"SELECT * FROM INVGCCINSCRIPCION_PROYECTOS WHERE strId_pro = '{id}'";
            var lista = _dal.SelectSql<InvgccInscripcionProyectos>(sql);
            return lista?.FirstOrDefault(); 
        }

        // =============================================================
        // CRUD PRINCIPAL
        // =============================================================

        public void Guardar(InvgccInscripcionProyectos pro)
        {
            int anioBase = ObtenerAnioDeConvocatoria(pro.fkId_conv);
            pro.strId_pro = GenerarNuevoIdProyecto(anioBase);
            pro.strEstado_pro = "Pendiente";

            string puntajeSql = pro.intPuntaje_pro.HasValue ? pro.intPuntaje_pro.Value.ToString() : "NULL";
            string coordinadorSql = string.IsNullOrEmpty(pro.fkId_coordinador) ? "NULL" : $"'{pro.fkId_coordinador}'";

            string cedula = ObtenerCedulaPorId(pro.fkId_coordinador);
            string cedulaSql = string.IsNullOrEmpty(cedula) ? "NULL" : $"'{cedula}'";

            string sql = $@"
                INSERT INTO INVGCCINSCRIPCION_PROYECTOS 
                (strId_pro, strTema_pro, strCoordinador_pro, strDuracion_pro, 
                    dtFehains_pro, fkId_gru, fkId_conv, strArchivo_pro, strEstado_pro, intPuntaje_pro, 
                    fkId_coordinador, strCedulaCoordinador_pro) -- <--- CAMPO AGREGADO
                VALUES 
                ('{pro.strId_pro}', '{pro.strTema_pro}', '{pro.strCoordinador_pro}',
                    '{pro.strDuracion_pro}', '{pro.dtFehains_pro:yyyy-MM-dd}', '{pro.fkId_gru}', '{pro.fkId_conv}', 
                    '{pro.strArchivo_pro}', '{pro.strEstado_pro}', {puntajeSql}, 
                    {coordinadorSql}, {cedulaSql})";

            _dal.UpdateSql(sql);
        }

        public void Actualizar(InvgccInscripcionProyectos pro)
        {
            string puntajeSql = pro.intPuntaje_pro.HasValue ? pro.intPuntaje_pro.Value.ToString() : "NULL";
            string coordinadorSql = string.IsNullOrEmpty(pro.fkId_coordinador) ? "NULL" : $"'{pro.fkId_coordinador}'";

            string cedula = ObtenerCedulaPorId(pro.fkId_coordinador);
            string cedulaSql = string.IsNullOrEmpty(cedula) ? "NULL" : $"'{cedula}'";

            string sql = $@"
                UPDATE INVGCCINSCRIPCION_PROYECTOS SET 
                    strTema_pro = '{pro.strTema_pro}',
                    strCoordinador_pro = '{pro.strCoordinador_pro}',
                    fkId_coordinador = {coordinadorSql}, 
                    strCedulaCoordinador_pro = {cedulaSql}, -- <--- ACTUALIZAMOS CÉDULA TAMBIÉN
                    strDuracion_pro = '{pro.strDuracion_pro}',
                    dtFehains_pro = '{pro.dtFehains_pro:yyyy-MM-dd}',
                    fkId_gru = '{pro.fkId_gru}',
                    fkId_conv = '{pro.fkId_conv}',
                    strArchivo_pro = '{pro.strArchivo_pro}',
                    intPuntaje_pro = {puntajeSql}
                WHERE strId_pro = '{pro.strId_pro}'";

            _dal.UpdateSql(sql);
        }

        public void Eliminar(string idProyecto)
        {
            string sqlGetInfo = $@"
                SELECT P.fkId_coordinador, I.strFuncion_int 
                FROM INVGCCINSCRIPCION_PROYECTOS P
                LEFT JOIN INVGCCGRUPO_INTEGRANTES I ON P.fkId_coordinador = I.strId_int
                WHERE P.strId_pro = '{idProyecto}'";

            var info = _dal.SelectSql<dynamic>(sqlGetInfo).FirstOrDefault();

            string idCoordinador = null;
            string funcionCoordinador = "";

            if (info != null)
            {
                try
                {
                    idCoordinador = info.fkId_coordinador;
                    funcionCoordinador = info.strFuncion_int != null ? info.strFuncion_int.ToString().ToUpper() : "";
                }
                catch { }
            }

            _dal.Delete("INVGCCINSCRIPCION_PROYECTOS", $"strId_pro = '{idProyecto}'");

            if (!string.IsNullOrEmpty(idCoordinador))
            {
                if (funcionCoordinador.StartsWith("COORDINADOR DE PROYECTO"))
                {
                    string sqlCheck = $"SELECT COUNT(*) as Total FROM INVGCCINSCRIPCION_PROYECTOS WHERE fkId_coordinador = '{idCoordinador}'";
                    var conteo = _dal.SelectSql<dynamic>(sqlCheck);

                    int proyectosRestantes = 0;
                    if (conteo != null && conteo.Count > 0)
                        int.TryParse(conteo[0].Total.ToString(), out proyectosRestantes);

                    if (proyectosRestantes == 0)
                    {
                        _dal.Delete("INVGCCGRUPO_INTEGRANTES", $"strId_int = '{idCoordinador}'");
                    }
                }
            }
        }

        public void CambiarEstado(string id, string nuevoEstado, string observacion)
        {
            string obsSanitizada = string.IsNullOrEmpty(observacion) ? "" : observacion.Replace("'", "");

            string sql = $@"
                UPDATE INVGCCINSCRIPCION_PROYECTOS 
                SET strEstado_pro = '{nuevoEstado}',
                    strObservacionEstado_pro = '{obsSanitizada}'
                WHERE strId_pro = '{id}'";

            _dal.UpdateSql(sql);
        }

        public dynamic ObtenerDocenteCategorizado(string cedula)
        {
            string sql = @"
                SELECT TOP 1 
                    strId_doc, strCedula_doc, strNombres_doc, strApellidos_doc, 
                    strFacultad_doc, strCarrera_doc, strCertificado_doc, strCorreo_doc
                FROM INVGCCCATEGORIZACION_DOCENTES 
                WHERE strCedula_doc = '" + cedula + "' AND bitActivo_doc = 1";

            var resultado = _dal.SelectSql<dynamic>(sql);
            return (resultado != null && resultado.Count > 0) ? resultado[0] : null;
        }


        public string VerificarIntegranteEnOtroGrupo(string cedula)
        {
            string sql = $@"
                SELECT G.strNombre_gru 
                FROM INVGCCGRUPO_INTEGRANTES I
                INNER JOIN INVGCCGRUPO_INVESTIGACION G ON I.fkId_gru = G.strId_gru
                WHERE I.strCedula_int = '{cedula}' 
                AND I.bitActivo_int = 1
                AND I.strTipo_int = 'Docente'";

            var resultado = _dal.SelectSql<dynamic>(sql);

            if (resultado != null && resultado.Count > 0)
            {
                try
                {
                    dynamic item = resultado[0];
                    return item.strNombre_gru.ToString();
                }
                catch { return "OTRO GRUPO"; }
            }
            return null;
        }

        // =============================================================
        // MÉTODOS AUXILIARES
        // =============================================================

        private string ObtenerCedulaPorId(string idIntegrante)
        {
            if (string.IsNullOrEmpty(idIntegrante)) return null;

            string sql = $"SELECT strCedula_int FROM INVGCCGRUPO_INTEGRANTES WHERE strId_int = '{idIntegrante}'";

            var resultado = _dal.SelectSql<dynamic>(sql);

            if (resultado != null && resultado.Count > 0)
            {
                try { return ((dynamic)resultado[0]).strCedula_int; }
                catch { return null; }
            }
            return null;
        }

        public string GuardarIntegranteExpress(InvgccGrupoIntegrantes intg, string usuarioResponsable)
        {
            string nuevoId = GenerarNuevoIdIntegrante();

            string idDocenteSql = string.IsNullOrEmpty(intg.fkId_docente_origen) ? "NULL" : $"'{intg.fkId_docente_origen}'";
            string certificadoSql = string.IsNullOrEmpty(intg.strCertificado_int) ? "NULL" : $"'{intg.strCertificado_int}'";

            string sqlIntegrante = $@"
                INSERT INTO INVGCCGRUPO_INTEGRANTES 
                (
                    strId_int, strCedula_int, strApellidos_int, strNombres_int, 
                    strCorreo_int, strCarrera_int, strFuncion_int, 
                    strTipo_int, fkId_gru, bitActivo_int, dtFechaini_int, 
                    strEntidad_int, strFacultad_int, 
                    strCertificado_int, fkId_docente_origen
                ) 
                VALUES 
                (
                    '{nuevoId}', '{intg.strCedula_int}', '{intg.strApellidos_int}', '{intg.strNombres_int}', 
                    '{intg.strCorreo_int}', '{intg.strCarrera_int}', '{intg.strFuncion_int}', 
                    '{intg.strTipo_int}', '{intg.fkId_gru}', 1, GETDATE(), 
                    '{intg.strEntidad_int}', '{intg.strFacultad_int}', 
                    {certificadoSql}, {idDocenteSql}
                )";

            _dal.UpdateSql(sqlIntegrante);

            try
            {
                string motivo = "Vinculación Automática por Creación de Proyecto";

                string sqlHistorial = $@"
                    INSERT INTO INVGCCINTEGRANTES_HISTORIAL
                    (strId_int, dtFecha, strAccion, strMotivo, strUsuario)
                    VALUES
                    ('{nuevoId}', GETDATE(), 'VINCULACION', '{motivo}', '{usuarioResponsable}')";

                _dal.UpdateSql(sqlHistorial);
            }
            catch (Exception ex)
            {
            }

            return nuevoId;
        }

        public List<InvgccGrupoInvestigacion> ObtenerGruposCombo()
        {
            string sql = "SELECT strId_gru, strNombre_gru FROM INVGCCGRUPO_INVESTIGACION ORDER BY strNombre_gru";
            return _dal.SelectSql<InvgccGrupoInvestigacion>(sql);
        }

        public List<InvgccConvocatoria> ObtenerConvocatoriasCombo()
        {
            string sql = "SELECT strId_conv, strNombre_conv FROM INVGCCCONVOCATORIA_GRUPOS_INVESTIGACION ORDER BY strNombre_conv";
            return _dal.SelectSql<InvgccConvocatoria>(sql);
        }

        public InvgccGrupoInvestigacion ObtenerInfoGrupo(string idGrupo)
        {
            string sql = $"SELECT * FROM INVGCCGRUPO_INVESTIGACION WHERE strId_gru = '{idGrupo}'";
            return _dal.SelectSql<InvgccGrupoInvestigacion>(sql)?.FirstOrDefault();
        }

        public List<InvgccGrupoIntegrantes> ObtenerIntegrantesPorGrupo(string idGrupo)
        {
            string sql = $@"
                SELECT strId_int, 
                       (strApellidos_int + ' ' + strNombres_int + ' (' + UPPER(strFuncion_int) + ')') as NombreCompleto
                FROM INVGCCGRUPO_INTEGRANTES 
                WHERE fkId_gru = '{idGrupo}' AND bitActivo_int = 1
                ORDER BY strApellidos_int";

            return _dal.SelectSql<InvgccGrupoIntegrantes>(sql);
        }

        // =============================================================
        // LÓGICA DE NEGOCIO PRIVADA (IDS Y FECHAS)
        // =============================================================

        private int ObtenerAnioDeConvocatoria(string idConvocatoria)
        {
            string sql = $"SELECT dtFechaini_conv FROM INVGCCCONVOCATORIA_GRUPOS_INVESTIGACION WHERE strId_conv = '{idConvocatoria}'";
            var lista = _dal.SelectSql<dynamic>(sql);

            if (lista != null && lista.Count > 0)
            {
                try
                {
                    DateTime fecha = Convert.ToDateTime(lista[0].dtFechaini_conv);
                    return fecha.Year;
                }
                catch { return DateTime.Now.Year; }
            }
            return DateTime.Now.Year;
        }

        private string GenerarNuevoIdProyecto(int anio)
        {
            string prefijo = $"DIRGI-CP{anio}-";
            string sql = $"SELECT TOP 1 strId_pro FROM INVGCCINSCRIPCION_PROYECTOS WHERE strId_pro LIKE '{prefijo}%' ORDER BY strId_pro DESC";

            var lista = _dal.SelectSql<InvgccInscripcionProyectos>(sql);
            int siguienteNumero = 1;

            if (lista != null && lista.Count > 0)
            {
                string ultimoId = lista[0].strId_pro;
                if (!string.IsNullOrEmpty(ultimoId) && ultimoId.Contains("-"))
                {
                    string numeroStr = ultimoId.Substring(ultimoId.LastIndexOf('-') + 1);
                    if (int.TryParse(numeroStr, out int numeroActual))
                    {
                        siguienteNumero = numeroActual + 1;
                    }
                }
            }

            return $"{prefijo}{siguienteNumero:D3}";
        }

        private string GenerarNuevoIdIntegrante()
        {
            string sql = "SELECT strId_int FROM INVGCCGRUPO_INTEGRANTES WHERE strId_int LIKE 'I%'";
            var lista = _dal.SelectSql<InvgccGrupoIntegrantes>(sql);

            int max = 0;
            if (lista != null)
            {
                foreach (var item in lista)
                {
                    string id = item.strId_int;
                    if (!string.IsNullOrEmpty(id) &&
                        id.StartsWith("I", StringComparison.OrdinalIgnoreCase) &&
                        !id.StartsWith("INT", StringComparison.OrdinalIgnoreCase))
                    {
                        if (int.TryParse(id.Substring(1), out int num))
                        {
                            if (num > max) max = num;
                        }
                    }
                }
            }
            return $"I{max + 1}";
        }

        //

        public List<dynamic> ObtenerDocentesDisponibles()
        {
            string sql = @"
                SELECT strCedula_doc, 
                       (strApellidos_doc + ' ' + strNombres_doc) as NombreCompleto
                FROM INVGCCCATEGORIZACION_DOCENTES
                WHERE bitActivo_doc = 1 
                AND strCedula_doc NOT IN (
                    SELECT strCedula_int 
                    FROM INVGCCGRUPO_INTEGRANTES 
                    WHERE bitActivo_int = 1
                )
                ORDER BY strApellidos_doc ASC";

            return _dal.SelectSql<dynamic>(sql); 
        }

        public List<dynamic> ObtenerDocentesSinGrupo()
        {
            string sql = @"
                SELECT strCedula_doc, 
                       (strApellidos_doc + ' ' + strNombres_doc) as NombreCompleto
                FROM INVGCCCATEGORIZACION_DOCENTES
                WHERE bitActivo_doc = 1 
                AND strCedula_doc NOT IN (
                    SELECT strCedula_int FROM INVGCCGRUPO_INTEGRANTES WHERE bitActivo_int = 1
                )
                ORDER BY strApellidos_doc ASC";

            return _dal.SelectSql<dynamic>(sql);
        }

    }
}