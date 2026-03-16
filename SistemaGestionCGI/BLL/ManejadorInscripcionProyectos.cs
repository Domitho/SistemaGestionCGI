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


        public List<dynamic> ObtenerTodos()
        {
            string sql = @"
                SELECT 
                    P.strId_pro, 
                    P.strTema_pro, 
                    P.strDuracion_pro, 
                    P.dtFehains_pro, 
                    P.strEstado_pro, 
                    P.intPuntaje_pro,
                    P.strArchivo_pro,
                    G.strNombre_gru, 
                    C.strNombre_conv,

                    -- 1. COLUMNA SOLO PARA EL NOMBRE
                    COALESCE(I.strApellidos_int + ' ' + I.strNombres_int, P.strCoordinador_pro) as NombreCoordinador,

                    -- 2. COLUMNA SOLO PARA EL CARGO (Si es nulo, ponemos 'COORDINADOR')
                    COALESCE(I.strFuncion_int, 'COORDINADOR') as CargoCoordinador

                FROM INVGCCINSCRIPCION_PROYECTOS P 
                INNER JOIN INVGCCGRUPO_INVESTIGACION G ON P.fkId_gru = G.strId_gru
                INNER JOIN INVGCCCONVOCATORIA_GRUPOS_INVESTIGACION C ON P.fkId_conv = C.strId_conv
                LEFT JOIN INVGCCGRUPO_INTEGRANTES I ON P.fkId_coordinador = I.strId_int
        
                ORDER BY ISNULL(P.intPuntaje_pro, -1) DESC, P.dtFehains_pro DESC";

            return _dal.SelectSql<dynamic>(sql);
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
                        _dal.Delete("INVGCCINTEGRANTES_HISTORIAL", $"strId_int = '{idCoordinador}'");
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

        //

        public List<dynamic> ObtenerCoordinadoresDisponibles(string idGrupo, string idProyectoEditando = null)
        {
            string sql = $@"
                SELECT I.strId_int, 
                       (I.strApellidos_int + ' ' + I.strNombres_int + ' (' + ISNULL(I.strFuncion_int, 'S/F') + ')') as NombreCompleto
                FROM INVGCCGRUPO_INTEGRANTES I
                WHERE I.fkId_gru = '{idGrupo}'
                AND I.bitActivo_int = 1
        
                AND NOT EXISTS (
                    SELECT 1 
                    FROM INVGCCINSCRIPCION_PROYECTOS P 
                    WHERE P.fkId_coordinador = I.strId_int
                    AND P.strEstado_pro IN ('Pendiente', 'Aprobado') 
            ";

            if (!string.IsNullOrEmpty(idProyectoEditando))
            {
                sql += $" AND P.strId_pro != '{idProyectoEditando}'";
            }

            sql += ")"; 

            return _dal.SelectSql<dynamic>(sql);
        }

        //

        public string ObtenerJefeActivoEnEjecucion(string idProyecto)
        {
            string sql = $@"
                SELECT strCoordinador_ejec, strCedulaCoordinador_ejec 
                FROM INVGCCEJECUCION_PROYECTO 
                WHERE fkId_pro = '{idProyecto}'";

            var res = _dal.SelectSql<dynamic>(sql);

            if (res != null && res.Count > 0)
            {
                string nombre = res[0].strCoordinador_ejec?.ToString() ?? "";
                string cedula = res[0].strCedulaCoordinador_ejec?.ToString() ?? "";

                if (!string.IsNullOrEmpty(cedula) && !nombre.Contains("SIN ASIGNAR"))
                {
                    return nombre; 
                }
            }
            return null;
        }

        public void SincronizarCoordinadorConEjecucion(string idProyecto, string idNuevoCoordinadorInt, string usuarioResponsable)
        {
            string sqlCheck = $"SELECT strId_ejec FROM INVGCCEJECUCION_PROYECTO WHERE fkId_pro = '{idProyecto}'";
            var resEjec = _dal.SelectSql<dynamic>(sqlCheck);

            if (resEjec != null && resEjec.Count > 0)
            {
                int idEjecucion = (int)resEjec[0].strId_ejec;

                string sqlDatos = $"SELECT * FROM INVGCCGRUPO_INTEGRANTES WHERE strId_int = '{idNuevoCoordinadorInt}'";
                var datos = _dal.SelectSql<dynamic>(sqlDatos);

                if (datos != null && datos.Count > 0)
                {
                    var nuevoCoord = datos[0];
                    string nombreCompleto = $"{nuevoCoord.strApellidos_int} {nuevoCoord.strNombres_int}";
                    string cedula = nuevoCoord.strCedula_int;

                    string sqlUpdateHeader = $@"
                        UPDATE INVGCCEJECUCION_PROYECTO 
                        SET strCoordinador_ejec = '{nombreCompleto.Replace("'", "''")}',
                            strCedulaCoordinador_ejec = '{cedula}'
                        WHERE strId_ejec = {idEjecucion}";
                    _dal.UpdateSql(sqlUpdateHeader);

                    string sqlCheckMember = $"SELECT strId_miembro FROM INVGCCEJECUCION_MIEMBROS WHERE fkId_ejec = {idEjecucion} AND strCedula_miembro = '{cedula}'";
                    var resMember = _dal.SelectSql<dynamic>(sqlCheckMember);
                    int idMiembroAfectado = 0;
                    string accion = "";

                    if (resMember == null || resMember.Count == 0)
                    {
                        string sqlInsertMember = $@"
                            INSERT INTO INVGCCEJECUCION_MIEMBROS 
                            (fkId_ejec, strCedula_miembro, strNombres_miembro, strApellidos_miembro, 
                             strRol_miembro, strFacultad_miembro, bitActivo_miembro,
                             strCorreo_miembro, strCarrera_miembro, strTipo_miembro, strEntidad_miembro, dtFechaInicio_miembro)
                            VALUES 
                            ({idEjecucion}, '{cedula}', '{nuevoCoord.strNombres_int}', '{nuevoCoord.strApellidos_int}', 
                             'COORDINADOR DE PROYECTO', '{nuevoCoord.strFacultad_int ?? "N/A"}', 1,
                             '{nuevoCoord.strCorreo_int}', '{nuevoCoord.strCarrera_int ?? "N/A"}', '{nuevoCoord.strTipo_int}', '{nuevoCoord.strEntidad_int ?? ""}', GETDATE())";
                        _dal.UpdateSql(sqlInsertMember);

                        var resNewId = _dal.SelectSql<dynamic>($"SELECT TOP 1 strId_miembro FROM INVGCCEJECUCION_MIEMBROS WHERE fkId_ejec={idEjecucion} ORDER BY strId_miembro DESC");
                        if (resNewId.Count > 0) idMiembroAfectado = (int)resNewId[0].strId_miembro;
                        accion = "DESIGNACIÓN (NUEVO)";
                    }
                    else
                    {
                        idMiembroAfectado = (int)resMember[0].strId_miembro;
                        string sqlUpdateRole = $"UPDATE INVGCCEJECUCION_MIEMBROS SET bitActivo_miembro=1, strRol_miembro='COORDINADOR DE PROYECTO', dtFechaFin_miembro=NULL WHERE strId_miembro={idMiembroAfectado}";
                        _dal.UpdateSql(sqlUpdateRole);
                        accion = "DESIGNACIÓN (REACTIVADO)";
                    }

                    if (idMiembroAfectado > 0)
                    {
                        _dal.UpdateSql($"INSERT INTO INVGCCEJECUCION_MIEMBROS_HISTORIAL (fkId_miembro, dtFecha, strAccion, strMotivo, strUsuario) VALUES ({idMiembroAfectado}, GETDATE(), '{accion}', 'Sincronización desde Inscripción', '{usuarioResponsable}')");
                    }
                }
            }
        }

        // CARRERAS - FACULTADES
        public List<dynamic> ObtenerFacultades()
        {
            string sql = @"
                SELECT 
                    IdFacultad,
                    Nombre
                FROM INVGCCFACULTADES
                ORDER BY Nombre";

            return _dal.SelectSql<dynamic>(sql);
        }

        public List<dynamic> ObtenerCarrerasPorFacultad(int idFacultad)
        {
            string sql = $@"
                SELECT 
                    IdCarrera,
                    Nombre
                FROM INVGCCCARRERAS
                WHERE IdFacultad = {idFacultad}
                ORDER BY Nombre";

            return _dal.SelectSql<dynamic>(sql);
        }
    }
}