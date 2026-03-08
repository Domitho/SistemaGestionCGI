using System;
using System.Collections.Generic;
using System.Linq;
using SistemaGestionCGI.Models;
using SistemaGestionCGI.Settings;
using System.Transactions; 

namespace SistemaGestionCGI.BLL
{
    public class ManejadorEjecucionProyectos
    {
        private readonly ConnectionSqlServer _dal = ConnectionSqlServer.Instance;
        public List<InvgccEjecucionProyectos> ObtenerEjecuciones(string cedulaUsuario = null)
        {
            string sql = @"
                SELECT E.*, 
                       P.strTema_pro as TituloProyecto,
                       P.strDuracion_pro,
                       C.dtInicio_ciclo as InicioCicloActual,
                       C.dtFin_ciclo as FinCicloActual,
                       (SELECT COUNT(*) FROM INVGCCEJECUCION_INFORMES I WHERE I.fkId_ejec = E.strId_ejec) as CantidadInformes
                FROM INVGCCEJECUCION_PROYECTO E
                INNER JOIN INVGCCINSCRIPCION_PROYECTOS P ON E.fkId_pro = P.strId_pro 
                LEFT JOIN INVGCCEJECUCION_PROYECTO_CICLOS C ON E.fkId_ciclo = C.id_ciclo";

            if (!string.IsNullOrEmpty(cedulaUsuario))
            {
                sql += $" WHERE E.strCedulaCoordinador_ejec = '{cedulaUsuario}' ";
            }

            sql += " ORDER BY E.strId_ejec DESC";

            return _dal.SelectSql<InvgccEjecucionProyectos>(sql);
        }

        public InvgccEjecucionProyectos ObtenerEjecucionPorId(int id)
        {
            string sql = $@"
                SELECT E.*, 
                       P.strTema_pro as TituloProyecto,
                       P.strDuracion_pro,
                       C.dtInicio_ciclo as InicioCicloActual,  
                       C.dtFin_ciclo as FinCicloActual 
                FROM INVGCCEJECUCION_PROYECTO E
                INNER JOIN INVGCCINSCRIPCION_PROYECTOS P ON E.fkId_pro = P.strId_pro
                LEFT JOIN INVGCCEJECUCION_PROYECTO_CICLOS C ON E.fkId_ciclo = C.id_ciclo 
                WHERE E.strId_ejec = {id}";

            var lista = _dal.SelectSql<InvgccEjecucionProyectos>(sql);
            return lista?.FirstOrDefault();
        }

        public int GuardarEjecucion(InvgccEjecucionProyectos obj)
        {
            string sqlInfo = $"SELECT strCedulaCoordinador_pro FROM INVGCCINSCRIPCION_PROYECTOS WHERE strId_pro = '{obj.fkId_pro}'";
            var listaTemp = _dal.SelectSql<DtoCedulaTemp>(sqlInfo);
            string cedulaHeredada = "";

            if (listaTemp != null && listaTemp.Count > 0)
            {
                cedulaHeredada = listaTemp[0].strCedulaCoordinador_pro;
            }

            string cedulaSql = string.IsNullOrEmpty(cedulaHeredada) ? "NULL" : $"'{cedulaHeredada}'";

            string sqlInsert = $@"
                INSERT INTO INVGCCEJECUCION_PROYECTO 
                (fkId_pro, strCoordinador_ejec, strCedulaCoordinador_ejec, 
                 strPeriodo_ejec, fkId_ciclo, dtFechaini_ejec, dtFechafin_ejec, strInforme_ejec, strEstado_ejec)
                VALUES 
                ('{obj.fkId_pro}', '{obj.strCoordinador_ejec}', {cedulaSql}, 
                 '{obj.strPeriodo_ejec}', {obj.fkId_ciclo}, '{obj.dtFechaini_ejec:yyyy-MM-dd}', NULL, '{obj.strInforme_ejec}', 'En Ejecución')";

            _dal.UpdateSql(sqlInsert);

            string sqlGetId = $@"
                SELECT TOP 1 strId_ejec 
                FROM INVGCCEJECUCION_PROYECTO 
                WHERE fkId_pro = '{obj.fkId_pro}' 
                ORDER BY strId_ejec DESC";

            var resultado = _dal.SelectSql<dynamic>(sqlGetId);

            if (resultado != null && resultado.Count > 0)
            {
                return (int)resultado[0].strId_ejec;
            }

            return 0;
        }

        public void ActualizarEjecucion(InvgccEjecucionProyectos obj)
        {
            string sql = $@"
                UPDATE INVGCCEJECUCION_PROYECTO SET 
                    strCoordinador_ejec = '{obj.strCoordinador_ejec}',
                    strPeriodo_ejec = '{obj.strPeriodo_ejec}',
                    dtFechaini_ejec = '{obj.dtFechaini_ejec:yyyy-MM-dd}',
                    strInforme_ejec = '{obj.strInforme_ejec}'
                WHERE strId_ejec = {obj.strId_ejec}";

            _dal.UpdateSql(sql);
        }

        public void EliminarEjecucion(int id)
        {
            _dal.DeleteSql($"DELETE FROM INVGCCEJECUCION_MIEMBROS_HISTORIAL WHERE fkId_miembro IN (SELECT strId_miembro FROM INVGCCEJECUCION_MIEMBROS WHERE fkId_ejec = {id})");
            _dal.DeleteSql($"DELETE FROM INVGCCEJECUCION_MIEMBROS WHERE fkId_ejec = {id}");
            _dal.DeleteSql($"DELETE FROM INVGCCEJECUCION_INFORMES WHERE fkId_ejec = {id}");
            _dal.Delete("INVGCCEJECUCION_PROYECTO", $"strId_ejec = {id}");
        }

        public void RenovarCicloProyecto(int idEjecucion, int idNuevoCiclo, string nombreNuevoPeriodo)
        {
            string sql = $@"
                UPDATE INVGCCEJECUCION_PROYECTO 
                SET 
                    fkId_ciclo = {idNuevoCiclo},
                    strPeriodo_ejec = '{nombreNuevoPeriodo}'
                WHERE strId_ejec = {idEjecucion}";

            _dal.UpdateSql(sql);
        }

        public List<InvgccEjecucionMiembros> ObtenerMiembros(int idEjecucion)
        {
            string sql = $@"
                SELECT * FROM INVGCCEJECUCION_MIEMBROS 
                WHERE fkId_ejec = {idEjecucion} 
                ORDER BY bitActivo_miembro DESC, strApellidos_miembro ASC";

            return _dal.SelectSql<InvgccEjecucionMiembros>(sql);
        }

        public void GuardarMiembro(InvgccEjecucionMiembros m)
        {
            string sql = $@"
                INSERT INTO INVGCCEJECUCION_MIEMBROS 
                (fkId_ejec, strCedula_miembro, strNombres_miembro, strApellidos_miembro, 
                 strRol_miembro, strFacultad_miembro, bitActivo_miembro,
                 strCorreo_miembro, strCarrera_miembro, strTipo_miembro, strEntidad_miembro, dtFechaInicio_miembro)
                VALUES 
                ({m.fkId_ejec}, '{m.strCedula_miembro}', '{m.strNombres_miembro}', '{m.strApellidos_miembro}', 
                 '{m.strRol_miembro}', '{m.strFacultad_miembro}', 1,
                 '{m.strCorreo_miembro}', '{m.strCarrera_miembro}', '{m.strTipo_miembro}', '{m.strEntidad_miembro}', GETDATE())";

            _dal.UpdateSql(sql);
        }

        public InvgccEjecucionMiembros ObtenerMiembroPorId(int id)
        {
            string sql = $"SELECT * FROM INVGCCEJECUCION_MIEMBROS WHERE strId_miembro = {id}";
            return _dal.SelectSql<InvgccEjecucionMiembros>(sql)?.FirstOrDefault();
        }

        public void ActualizarMiembro(InvgccEjecucionMiembros obj)
        {
            string sql = $@"
                UPDATE INVGCCEJECUCION_MIEMBROS SET 
                    strCedula_miembro = '{obj.strCedula_miembro}',
                    strNombres_miembro = '{obj.strNombres_miembro}',
                    strApellidos_miembro = '{obj.strApellidos_miembro}',
                    strRol_miembro = '{obj.strRol_miembro}',
                    strFacultad_miembro = '{obj.strFacultad_miembro}',
                    strCorreo_miembro = '{obj.strCorreo_miembro}',
                    strCarrera_miembro = '{obj.strCarrera_miembro}',
                    strTipo_miembro = '{obj.strTipo_miembro}',
                    strEntidad_miembro = '{obj.strEntidad_miembro}'
                WHERE strId_miembro = {obj.strId_miembro}";

            _dal.UpdateSql(sql);
        }

        public void EliminarMiembro(int idMiembro) =>
            _dal.UpdateSql($"UPDATE INVGCCEJECUCION_MIEMBROS SET bitActivo_miembro = 0 WHERE strId_miembro = {idMiembro}");

        public void CambiarEstadoMiembro(int idMiembro, bool nuevoEstado, string motivo, string usuario)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    var miembro = ObtenerMiembroPorId(idMiembro);
                    if (miembro == null) return;

                    if (nuevoEstado)
                    {
                        string rol = miembro.strRol_miembro?.ToUpper() ?? "";
                        bool esPerfilMando = rol.Contains("COORDINADOR") || rol.Contains("DIRECTOR") || rol.Contains("PRINCIPAL");

                        if (esPerfilMando)
                        {
                            var proyectoCheck = ObtenerEjecucionPorId(miembro.fkId_ejec);
                            if (proyectoCheck != null)
                            {
                                string cedulaJefeActual = proyectoCheck.strCedulaCoordinador_ejec?.Trim() ?? "";
                                string nombreJefeActual = proyectoCheck.strCoordinador_ejec?.ToUpper() ?? "";
                                string cedulaMiembro = miembro.strCedula_miembro.Trim();

                                bool puestoOcupadoPorOtro = !string.IsNullOrEmpty(cedulaJefeActual) &&
                                                            !nombreJefeActual.Contains("SIN ASIGNAR") &&
                                                            cedulaJefeActual != cedulaMiembro;

                                if (puestoOcupadoPorOtro)
                                {
                                    throw new Exception($"NO SE PUEDE RESTAURAR: El proyecto ya tiene un Coordinador activo ({proyectoCheck.strCoordinador_ejec}). Debe dar de baja al actual antes de restaurar al anterior.");
                                }
                            }
                        }
                    }

                    string cedulaFija = miembro.strCedula_miembro.Trim();

                    int bit = nuevoEstado ? 1 : 0;
                    string fechaFinSql = nuevoEstado ? "NULL" : $"'{DateTime.Now:yyyy-MM-dd HH:mm:ss}'";

                    _dal.UpdateSql($@"UPDATE INVGCCEJECUCION_MIEMBROS 
                              SET bitActivo_miembro = {bit}, dtFechaFin_miembro = {fechaFinSql}
                              WHERE strId_miembro = {idMiembro}");

                    RegistrarHistorialMiembro(idMiembro, nuevoEstado ? "REACTIVACIÓN" : "BAJA", motivo, usuario);

                    if (miembro.fkId_ejec > 0)
                    {
                        var proyecto = ObtenerEjecucionPorId(miembro.fkId_ejec);

                        if (proyecto != null)
                        {
                            string cedulaJefeActual = proyecto.strCedulaCoordinador_ejec?.Trim() ?? "";

                            if (!nuevoEstado)
                            {
                                if (!string.IsNullOrEmpty(cedulaJefeActual) && cedulaFija == cedulaJefeActual)
                                {
                                    _dal.UpdateSql($@"UPDATE INVGCCEJECUCION_PROYECTO 
                                              SET strCoordinador_ejec = '-- SIN ASIGNAR --'
                                              WHERE strId_ejec = {miembro.fkId_ejec}");

                                    _dal.UpdateSql($@"UPDATE INVGCCINSCRIPCION_PROYECTOS 
                                              SET strCoordinador_pro = '-- SIN ASIGNAR --', 
                                                  fkId_coordinador = NULL 
                                              WHERE strId_pro = '{proyecto.fkId_pro}'");
                                }
                            }
                            else if (nuevoEstado)
                            {
                                bool puestoVacio = string.IsNullOrEmpty(cedulaJefeActual) ||
                                                   proyecto.strCoordinador_ejec.Contains("SIN ASIGNAR");

                                string rolLocal = miembro.strRol_miembro?.ToUpper() ?? "";
                                bool esJefe = rolLocal.Contains("COORDINADOR") || rolLocal.Contains("DIRECTOR") || rolLocal.Contains("PRINCIPAL");

                                if ((puestoVacio || cedulaJefeActual == cedulaFija) && esJefe)
                                {
                                    string nombreCompleto = $"{miembro.strApellidos_miembro} {miembro.strNombres_miembro}";
                                    string nombreConRol = $"{nombreCompleto} ({miembro.strRol_miembro})";

                                    string sqlGetIdOriginal = $@"
                                        SELECT TOP 1 G.strId_int 
                                        FROM INVGCCGRUPO_INTEGRANTES G
                                        INNER JOIN INVGCCINSCRIPCION_PROYECTOS P ON G.fkId_gru = P.fkId_gru
                                        WHERE P.strId_pro = '{proyecto.fkId_pro}' 
                                        AND G.strCedula_int = '{cedulaFija}'";

                                    var resId = _dal.SelectSql<dynamic>(sqlGetIdOriginal);
                                    string idIntegranteOriginal = (resId != null && resId.Count > 0) ? resId[0].strId_int : "NULL";
                                    string valFk = (idIntegranteOriginal == "NULL") ? "NULL" : $"'{idIntegranteOriginal}'";

                                    string sqlEjec = $@"
                                        UPDATE INVGCCEJECUCION_PROYECTO 
                                        SET strCoordinador_ejec = '{nombreCompleto.Replace("'", "''")}',
                                            strCedulaCoordinador_ejec = '{cedulaFija}'
                                        WHERE strId_ejec = {miembro.fkId_ejec}";
                                    _dal.UpdateSql(sqlEjec);

                                    string sqlInsc = $@"
                                        UPDATE INVGCCINSCRIPCION_PROYECTOS 
                                        SET strCoordinador_pro = '{nombreConRol.Replace("'", "''")}',
                                            strCedulaCoordinador_pro = '{cedulaFija}',
                                            fkId_coordinador = {valFk}
                                        WHERE strId_pro = '{proyecto.fkId_pro}'";
                                    _dal.UpdateSql(sqlInsc);
                                }
                            }
                        }
                    }
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message); 
                }
            }
        }

        public void RegistrarHistorialMiembro(int idMiembro, string accion, string motivo, string usuario)
        {
            string fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string motivoLimpio = motivo.Replace("'", "");

            string sql = $@"
                INSERT INTO INVGCCEJECUCION_MIEMBROS_HISTORIAL 
                (fkId_miembro, dtFecha, strAccion, strMotivo, strUsuario)
                VALUES 
                ({idMiembro}, '{fecha}', '{accion}', '{motivoLimpio}', '{usuario}')";

            _dal.UpdateSql(sql);
        }

        public List<InvgccEjecucionMiembrosHistorial> ObtenerHistorialMiembro(int idMiembro)
        {
            string sql = $"SELECT * FROM INVGCCEJECUCION_MIEMBROS_HISTORIAL WHERE fkId_miembro = {idMiembro} ORDER BY dtFecha DESC";
            return _dal.SelectSql<InvgccEjecucionMiembrosHistorial>(sql);
        }

        public dynamic BuscarDocentePorCedula(string cedula)
        {
            if (string.IsNullOrEmpty(cedula)) return null;

            string sql = string.Format(@"
                SELECT TOP 1 
                    strNombres_doc,  
                    strApellidos_doc, 
                    strCarrera_doc, 
                    strFacultad_doc,
                    strCedula_doc
                FROM INVGCCCATEGORIZACION_DOCENTES 
                WHERE strCedula_doc = '{0}' AND bitActivo_doc = 1", cedula);

            var resultados = _dal.SelectSql<dynamic>(sql);

            if (resultados != null && resultados.Count > 0)
            {
                return resultados[0];
            }

            return null;
        }


        public List<InvgccEjecucionInformes> ObtenerInformes(int idEjecucion)
        {
            string sql = $"SELECT * FROM INVGCCEJECUCION_INFORMES WHERE fkId_ejec = {idEjecucion} ORDER BY strCiclo_informe DESC, dtFechaSubida DESC";
            return _dal.SelectSql<InvgccEjecucionInformes>(sql);
        }

        public InvgccEjecucionInformes ObtenerInformePorId(int id)
        {
            string sql = $"SELECT * FROM INVGCCEJECUCION_INFORMES WHERE strId_informe = {id}";
            return _dal.SelectSql<InvgccEjecucionInformes>(sql)?.FirstOrDefault();
        }

        public void GuardarInforme(InvgccEjecucionInformes inf, string nombreCiclo)
        {
            string fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string rutaSegura = inf.strArchivo_path.Replace("'", "''");
            string nombreArchivo = inf.strNombrePeriodo.Replace("'", "''");
            string cicloSeguro = string.IsNullOrEmpty(nombreCiclo) ? "Periodo Anterior" : nombreCiclo.Replace("'", "''");

            string sql = $@"
                INSERT INTO INVGCCEJECUCION_INFORMES 
                (fkId_ejec, strNombrePeriodo, strArchivo_path, dtFechaSubida, strCiclo_informe)
                VALUES 
                ({inf.fkId_ejec}, '{nombreArchivo}', '{rutaSegura}', '{fecha}', '{cicloSeguro}')";

            _dal.UpdateSql(sql);
        }

        public void ActualizarInforme(InvgccEjecucionInformes inf)
        {
            string nombreLimpio = inf.strNombrePeriodo.Replace("'", "''");
            string sql;

            if (!string.IsNullOrEmpty(inf.strArchivo_path))
            {
                string fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string rutaSegura = inf.strArchivo_path.Replace("'", "''");

                sql = $@"UPDATE INVGCCEJECUCION_INFORMES SET 
                        strNombrePeriodo = '{nombreLimpio}', 
                        strArchivo_path = '{rutaSegura}', 
                        dtFechaSubida = '{fecha}'
                        WHERE strId_informe = {inf.strId_informe}";
            }
            else
            {
                sql = $@"UPDATE INVGCCEJECUCION_INFORMES SET 
                        strNombrePeriodo = '{nombreLimpio}'
                        WHERE strId_informe = {inf.strId_informe}";
            }

            _dal.UpdateSql(sql);
        }

        public void EliminarInforme(int idInforme) =>
            _dal.Delete("INVGCCEJECUCION_INFORMES", $"strId_informe = {idInforme}");

        public void SubirInformeCierre(int idEjecucion, string rutaArchivo, string usuario, string nombreOriginal)
        {
            string sqlHistorial = $@"
                INSERT INTO INVGCCEJECUCION_CIERRE_HISTORIAL 
                (fkId_ejec, strNombreArchivo, strRutaArchivo, dtFechaSubida, strUsuarioSubida)
                VALUES 
                ({idEjecucion}, '{nombreOriginal}', '{rutaArchivo}', GETDATE(), '{usuario}')";

            _dal.UpdateSql(sqlHistorial);

            string sqlUpdate = $@"
                UPDATE INVGCCEJECUCION_PROYECTO 
                SET strInforme_Cierre = '{rutaArchivo}',
                    strEstado_ejec = 'EN REVISION'
                WHERE strId_ejec = {idEjecucion}";

            _dal.UpdateSql(sqlUpdate);
        }

        public List<dynamic> ObtenerHistorialCierre(int idEjecucion)
        {
            string sql = $"SELECT * FROM INVGCCEJECUCION_CIERRE_HISTORIAL WHERE fkId_ejec = {idEjecucion} ORDER BY dtFechaSubida DESC";
            return _dal.SelectSql<dynamic>(sql);
        }

        public void SubirInformeFinal(int idEjecucion, string rutaArchivo, string usuario)
        {
            string sql = $@"
                UPDATE INVGCCEJECUCION_PROYECTO 
                SET strInforme_Final = '{rutaArchivo}',
                    strEstado_ejec = 'FINALIZADO',
                    dtFechafin_ejec = GETDATE()
                WHERE strId_ejec = {idEjecucion}";

            _dal.UpdateSql(sql);
        }

        public void AprobarCierre(int idEjecucion, string usuario)
        {
            string sql = $@"
                UPDATE INVGCCEJECUCION_PROYECTO 
                SET strEstado_ejec = 'CIERRE APROBADO'
                WHERE strId_ejec = {idEjecucion}";

            _dal.UpdateSql(sql);
        }

        public List<dynamic> ObtenerRepositorioCompleto(int idEjecucion)
        {
            string sql = $@"
                SELECT 
                    'AVANCE' as TipoDoc, 
                    strNombrePeriodo as Nombre, 
                    strArchivo_path as Ruta, 
                    dtFechaSubida as Fecha,
                    'fa-file-pdf text-primary' as Icono
                FROM INVGCCEJECUCION_INFORMES 
                WHERE fkId_ejec = {idEjecucion}

                UNION ALL

                SELECT 
                    'CIERRE' as TipoDoc, 
                    'Informe de Cierre' as Nombre, 
                    strInforme_Cierre as Ruta, 
                    dtFechafin_ejec as Fecha,
                    'fa-file-contract text-warning' as Icono
                FROM INVGCCEJECUCION_PROYECTO 
                WHERE strId_ejec = {idEjecucion} AND strInforme_Cierre IS NOT NULL

                UNION ALL

                SELECT 
                    'FINAL' as TipoDoc, 
                    'Informe Final' as Nombre, 
                    strInforme_Final as Ruta, 
                    dtFechafin_ejec as Fecha,
                    'fa-award text-success' as Icono
                FROM INVGCCEJECUCION_PROYECTO 
                WHERE strId_ejec = {idEjecucion} AND strInforme_Final IS NOT NULL
        
                ORDER BY Fecha DESC";

            return _dal.SelectSql<dynamic>(sql);
        }


        public List<InvgccInscripcionProyectos> ObtenerProyectosAprobadosSinEjecucion()
        {
            string sql = @"
                SELECT strId_pro, strTema_pro, strCoordinador_pro 
                FROM INVGCCINSCRIPCION_PROYECTOS 
                WHERE strEstado_pro = 'Aprobado' 
                AND strId_pro NOT IN (SELECT fkId_pro FROM INVGCCEJECUCION_PROYECTO)";

            return _dal.SelectSql<InvgccInscripcionProyectos>(sql);
        }

        public void GuardarCiclo(DateTime fechaInicio, DateTime fechaFin)
        {
            string nombreCiclo = $"{fechaInicio.ToString("MMMM yyyy").ToUpper()} - {fechaFin.ToString("MMMM yyyy").ToUpper()}";

            string sqlInsert = $@"
                INSERT INTO INVGCCEJECUCION_PROYECTO_CICLOS 
                (strNombre_ciclo, dtInicio_ciclo, dtFin_ciclo) 
                VALUES 
                ('{nombreCiclo}', '{fechaInicio:yyyy-MM-dd}', '{fechaFin:yyyy-MM-dd}')";

            _dal.UpdateSql(sqlInsert);
        }

        public List<dynamic> ObtenerCiclos()
        {
            string sql = "SELECT id_ciclo, strNombre_ciclo FROM INVGCCEJECUCION_PROYECTO_CICLOS ORDER BY dtInicio_ciclo DESC";
            return _dal.SelectSql<dynamic>(sql);
        }

        public List<dynamic> ObtenerCiclosConFechas()
        {
            string sql = "SELECT id_ciclo, strNombre_ciclo, dtInicio_ciclo, dtFin_ciclo FROM INVGCCEJECUCION_PROYECTO_CICLOS ORDER BY dtInicio_ciclo ASC";
            return _dal.SelectSql<dynamic>(sql);
        }

        public List<CicloAcademico> ObtenerTodosLosCiclos()
        {
            string sql = @"
                SELECT id_ciclo, strNombre_ciclo, dtInicio_ciclo 
                FROM INVGCCEJECUCION_PROYECTO_CICLOS 
                ORDER BY dtInicio_ciclo DESC";

            return _dal.SelectSql<CicloAcademico>(sql);
        }

        public dynamic ObtenerDatosIntegranteGrupo(string idIntegranteGrupo)
        {
            string sql = $"SELECT * FROM INVGCCGRUPO_INTEGRANTES WHERE strId_int = '{idIntegranteGrupo}'";
            var lista = _dal.SelectSql<dynamic>(sql);
            return lista.Count > 0 ? lista[0] : null;
        }

        public List<InvgccEjecucionMiembros> ObtenerMiembrosActivos(int idEjecucion)
        {
            string sql = $@"
                SELECT * FROM INVGCCEJECUCION_MIEMBROS 
                WHERE fkId_ejec = {idEjecucion} 
                AND bitActivo_miembro = 1
                ORDER BY strApellidos_miembro ASC";
            return _dal.SelectSql<InvgccEjecucionMiembros>(sql);
        }

        public List<InvgccEjecucionMiembros> ObtenerMiembrosPapelera(int idEjecucion)
        {
            string sql = $@"
                SELECT * FROM INVGCCEJECUCION_MIEMBROS 
                WHERE fkId_ejec = {idEjecucion} 
                AND bitActivo_miembro = 0
                ORDER BY strApellidos_miembro ASC";
            return _dal.SelectSql<InvgccEjecucionMiembros>(sql);
        }

        public bool ExisteMiembroActivoPorCedula(string cedula, int idEjecucion)
        {
            string sql = $@"
                SELECT TOP 1 strId_miembro
                FROM INVGCCEJECUCION_MIEMBROS 
                WHERE fkId_ejec = {idEjecucion} 
                AND strCedula_miembro = '{cedula}' 
                AND bitActivo_miembro = 1";

            var resultado = _dal.SelectSql<dynamic>(sql);
            return resultado != null && resultado.Count > 0;
        }

        public void RestaurarMiembro(int idMiembro, string usuario)
        {
            CambiarEstadoMiembro(idMiembro, true, "Recuperado desde Papelera", usuario);
        }

        public string ObtenerEmailCoordinador(int idEjecucion)
        {
            string sql = $@"
                SELECT TOP 1 I.strCorreo_int 
                FROM INVGCCEJECUCION_PROYECTO E
                INNER JOIN INVGCCGRUPO_INTEGRANTES I ON E.strCedulaCoordinador_ejec = I.strCedula_int
                WHERE E.strId_ejec = {idEjecucion} 
                AND I.bitActivo_int = 1
                ORDER BY I.strId_int DESC";

            try
            {
                var resultado = _dal.SelectSql<dynamic>(sql);
                if (resultado != null && resultado.Count > 0)
                {
                    return resultado[0].strCorreo_int?.ToString().Trim() ?? "";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error en ObtenerEmailCoordinador: " + ex.Message);
            }
            return "";
        }

        //
        public void GuardarObservacionInforme(int idInforme, string observacion)
        {
            string obsSegura = string.IsNullOrEmpty(observacion) ? "NULL" : $"'{observacion.Replace("'", "''")}'";

            string sql = $@"
                UPDATE INVGCCEJECUCION_INFORMES 
                SET strObservacion_informe = {obsSegura}
                WHERE strId_informe = {idInforme}";

            _dal.UpdateSql(sql);
        }

        public void MarcarObservacionComoLeida(int idInforme)
        {
            string sql = $@"
                UPDATE INVGCCEJECUCION_INFORMES 
                SET dtFechaLectura_informe = GETDATE()
                WHERE strId_informe = {idInforme}";

            _dal.UpdateSql(sql);
        }

        //

        public string VerificarVinculacionEnOtrosProyectos(string cedula)
        {

            string sql = $@"
                SELECT TOP 1 P.strTema_pro
                FROM INVGCCEJECUCION_MIEMBROS M
                INNER JOIN INVGCCEJECUCION_PROYECTO E ON M.fkId_ejec = E.strId_ejec
                INNER JOIN INVGCCINSCRIPCION_PROYECTOS P ON E.fkId_pro = P.strId_pro
                WHERE M.strCedula_miembro = '{cedula}'
                  AND M.bitActivo_miembro = 1
                  AND E.strEstado_ejec != 'FINALIZADO'";

            try
            {
                var resultado = _dal.SelectSql<dynamic>(sql);

                if (resultado != null && resultado.Count > 0)
                {
                    return resultado[0].strTema_pro;
                }
            }
            catch {}

            return null;
        }

        //
        public List<dynamic> ObtenerCandidatosDelGrupo(int idEjecucion)
        {

            string sql = $@"
        SELECT 
            I.strId_int,
            (I.strApellidos_int + ' ' + I.strNombres_int) as NombreCompleto,
            I.strCedula_int,
            I.strNombres_int,
            I.strApellidos_int,
            I.strCorreo_int,
            I.strFacultad_int,
            I.strCarrera_int,
            I.strTipo_int,
            I.strEntidad_int,
            I.strCertificado_int  -- <--- Archivo subido al grupo
        FROM INVGCCGRUPO_INTEGRANTES I
        INNER JOIN INVGCCINSCRIPCION_PROYECTOS P ON I.fkId_gru = P.fkId_gru
        INNER JOIN INVGCCEJECUCION_PROYECTO E ON P.strId_pro = E.fkId_pro
        WHERE E.strId_ejec = {idEjecucion}
          AND I.bitActivo_int = 1  -- Solo miembros activos del grupo
          AND I.strCedula_int NOT IN (
              -- VALIDACIÓN CRÍTICA: Que NO estén activos en NINGÚN proyecto de ejecución
              SELECT M.strCedula_miembro 
              FROM INVGCCEJECUCION_MIEMBROS M 
              WHERE M.bitActivo_miembro = 1
          )
        ORDER BY I.strApellidos_int";

            return _dal.SelectSql<dynamic>(sql);
        }

        public dynamic ObtenerDatosCandidatoGrupo(string idIntegranteGrupo)
        {
            string sql = $"SELECT * FROM INVGCCGRUPO_INTEGRANTES WHERE strId_int = '{idIntegranteGrupo}'";
            var lista = _dal.SelectSql<dynamic>(sql);
            return lista.FirstOrDefault();
        }

        public List<dynamic> ObtenerDocentesCategorizadosLibres()
        {

            string sql = @"
        SELECT 
            D.strId_doc as ID_REAL,
            (D.strApellidos_doc + ' ' + D.strNombres_doc) as NombreCompleto,
            D.strCedula_doc,
            D.strNombres_doc,
            D.strApellidos_doc,
            D.strCorreo_doc,
            D.strFacultad_doc,
            D.strCarrera_doc,
            'Docente' as Tipo,
            D.strCertificado_doc as RutaDocumento -- <--- Archivo de categorización
        FROM INVGCCCATEGORIZACION_DOCENTES D
        WHERE D.bitActivo_doc = 1
          -- REGLA 1: Que NO pertenezcan a ningún Grupo de Investigación activo
          AND D.strCedula_doc NOT IN (
              SELECT I.strCedula_int 
              FROM INVGCCGRUPO_INTEGRANTES I 
              WHERE I.bitActivo_int = 1
          )
          -- REGLA 2: Que NO estén trabajando en otro proyecto actualmente
          AND D.strCedula_doc NOT IN (
              SELECT M.strCedula_miembro 
              FROM INVGCCEJECUCION_MIEMBROS M 
              WHERE M.bitActivo_miembro = 1
          )
        ORDER BY D.strApellidos_doc";

            return _dal.SelectSql<dynamic>(sql);
        }

        public dynamic ObtenerDatosDocenteCategorizado(string idDocente)
        {
            string sql = $"SELECT * FROM INVGCCCATEGORIZACION_DOCENTES WHERE strId_doc = '{idDocente}'";
            var lista = _dal.SelectSql<dynamic>(sql);
            return lista.FirstOrDefault();
        }

        public List<dynamic> ObtenerFacultades()
        {
            string sql = "SELECT IdFacultad, Codigo, Nombre FROM INVGCCFACULTADES ORDER BY Nombre";
            return _dal.SelectSql<dynamic>(sql);
        }

        public List<dynamic> ObtenerCarrerasPorFacultad(int idFacultad)
        {
            string sql = $"SELECT IdCarrera, IdFacultad, Nombre FROM INVGCCCARRERAS WHERE IdFacultad = {idFacultad} ORDER BY Nombre";
            return _dal.SelectSql<dynamic>(sql);
        }

    }

    public class DtoCedulaTemp
    {
        public string strCedulaCoordinador_pro { get; set; }
    }

    public class GrupoPeriodo
    {
        public string NombrePeriodo { get; set; }
        public DateTime FechaInicioCiclo { get; set; }
        public List<dynamic> Archivos { get; set; }
    }

}