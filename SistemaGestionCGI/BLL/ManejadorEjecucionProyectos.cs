using System;
using System.Collections.Generic;
using System.Linq;
using SistemaGestionCGI.Models;
using SistemaGestionCGI.Settings;

namespace SistemaGestionCGI.BLL
{
    public class ManejadorEjecucionProyectos
    {
        private readonly ConnectionSqlServer _dal = ConnectionSqlServer.Instance;

        // =============================================================
        // 1. GESTIÓN DE EJECUCIÓN
        // =============================================================

        public List<InvgccEjecucionProyectos> ObtenerEjecuciones()
        {
            string sql = @"
                SELECT E.*, P.strTema_pro as TituloProyecto 
                FROM INVGCCEJECUCION_PROYECTO E
                INNER JOIN INVGCCINSCRIPCION_PROYECTOS P ON E.fkId_pro = P.strId_pro
                ORDER BY E.strId_ejec DESC";
            return _dal.SelectSql<InvgccEjecucionProyectos>(sql);
        }

        public InvgccEjecucionProyectos ObtenerEjecucionPorId(int id)
        {
            string sql = $@"
                SELECT E.*, P.strTema_pro as TituloProyecto 
                FROM INVGCCEJECUCION_PROYECTO E
                INNER JOIN INVGCCINSCRIPCION_PROYECTOS P ON E.fkId_pro = P.strId_pro
                WHERE E.strId_ejec = {id}";

            var lista = _dal.SelectSql<InvgccEjecucionProyectos>(sql);
            return (lista != null && lista.Count > 0) ? lista[0] : null;
        }

        public void GuardarEjecucion(InvgccEjecucionProyectos obj)
        {
            // IDENTITY: SI -> NO enviamos strId_ejec
            // FK_PRO: VARCHAR -> Lleva comillas simples '{obj.fkId_pro}'
            // FECHAS: Tipo DATE -> yyyy-MM-dd

            string sql = $@"
                INSERT INTO INVGCCEJECUCION_PROYECTO 
                (fkId_pro, strCoordinador_ejec, strPeriodo_ejec, dtFechaini_ejec, dtFechafin_ejec, strInforme_ejec, strEstado_ejec)
                VALUES 
                ('{obj.fkId_pro}', '{obj.strCoordinador_ejec}', '{obj.strPeriodo_ejec}', '{obj.dtFechaini_ejec:yyyy-MM-dd}', NULL, '{obj.strInforme_ejec}', 'En Ejecución')";

            _dal.UpdateSql(sql);
        }

        public void ActualizarEjecucion(InvgccEjecucionProyectos obj)
        {
            string fechaFin = obj.dtFechafin_ejec.HasValue ? $"'{obj.dtFechafin_ejec.Value:yyyy-MM-dd}'" : "NULL";

            string sql = $@"
                UPDATE INVGCCEJECUCION_PROYECTO SET 
                    strCoordinador_ejec = '{obj.strCoordinador_ejec}',
                    strPeriodo_ejec = '{obj.strPeriodo_ejec}',
                    dtFechaini_ejec = '{obj.dtFechaini_ejec:yyyy-MM-dd}',
                    dtFechafin_ejec = {fechaFin}, 
                    strInforme_ejec = '{obj.strInforme_ejec}'
                WHERE strId_ejec = {obj.strId_ejec}";

            _dal.UpdateSql(sql);
        }

        public void EliminarEjecucion(int id)
        {
            // Borrado en cascada manual
            _dal.DeleteSql($"DELETE FROM INVGCCEJECUCION_MIEMBROS WHERE fkId_ejec = {id}");
            _dal.DeleteSql($"DELETE FROM INVGCCEJECUCION_INFORMES WHERE fkId_ejec = {id}");
            _dal.Delete("INVGCCEJECUCION_PROYECTO", $"strId_ejec = {id}");
        }

        // =============================================================
        // 2. GESTIÓN DE MIEMBROS (Tabla: INVGCCEJECUCION_MIEMBROS)
        // ID: strId_miembro (INT IDENTITY) | FK_EJEC: fkId_ejec (INT)
        // =============================================================

        public List<InvgccEjecucionMiembros> ObtenerMiembros(int idEjecucion)
        {
            string sql = $"SELECT * FROM INVGCCEJECUCION_MIEMBROS WHERE fkId_ejec = {idEjecucion} AND bitActivo_miembro = 1";
            return _dal.SelectSql<InvgccEjecucionMiembros>(sql);
        }

        public void GuardarMiembro(InvgccEjecucionMiembros m)
        {
            // IDENTITY: SI -> NO enviamos strId_miembro
            // FK_EJEC: INT -> NO lleva comillas {m.fkId_ejec}

            string sql = $@"
                INSERT INTO INVGCCEJECUCION_MIEMBROS 
                (fkId_ejec, strCedula_miembro, strNombres_miembro, strApellidos_miembro, strRol_miembro, strFacultad_miembro, bitActivo_miembro)
                VALUES 
                ({m.fkId_ejec}, '{m.strCedula_miembro}', '{m.strNombres_miembro}', '{m.strApellidos_miembro}', '{m.strRol_miembro}', '{m.strFacultad_miembro}', 1)";

            _dal.UpdateSql(sql);
        }

        public void EliminarMiembro(int idMiembro)
        {
            string sql = $"UPDATE INVGCCEJECUCION_MIEMBROS SET bitActivo_miembro = 0 WHERE strId_miembro = {idMiembro}";
            _dal.UpdateSql(sql);
        }

        public InvgccEjecucionMiembros ObtenerMiembroPorId(int id)
        {
            string sql = $"SELECT * FROM INVGCCEJECUCION_MIEMBROS WHERE strId_miembro = {id}";
            var lista = _dal.SelectSql<InvgccEjecucionMiembros>(sql);
            return (lista != null && lista.Count > 0) ? lista[0] : null;
        }

        public void ActualizarMiembro(InvgccEjecucionMiembros obj)
        {
            string sql = $@"
                UPDATE INVGCCEJECUCION_MIEMBROS SET 
                    strCedula_miembro = '{obj.strCedula_miembro}',
                    strNombres_miembro = '{obj.strNombres_miembro}',
                    strApellidos_miembro = '{obj.strApellidos_miembro}',
                    strRol_miembro = '{obj.strRol_miembro}',
                    strFacultad_miembro = '{obj.strFacultad_miembro}'
                WHERE strId_miembro = {obj.strId_miembro}";

            _dal.UpdateSql(sql);
        }

        // =============================================================
        // 3. GESTIÓN DE INFORMES (Tabla: INVGCCEJECUCION_INFORMES)
        // ID: strId_informe (INT IDENTITY) | FK_EJEC: fkId_ejec (INT)
        // =============================================================

        public List<InvgccEjecucionInformes> ObtenerInformes(int idEjecucion)
        {
            string sql = $"SELECT * FROM INVGCCEJECUCION_INFORMES WHERE fkId_ejec = {idEjecucion} ORDER BY dtFechaSubida DESC";
            return _dal.SelectSql<InvgccEjecucionInformes>(sql);
        }

        public InvgccEjecucionInformes ObtenerInformePorId(int id)
        {
            string sql = $"SELECT * FROM INVGCCEJECUCION_INFORMES WHERE strId_informe = {id}";
            var lista = _dal.SelectSql<InvgccEjecucionInformes>(sql);
            return (lista != null && lista.Count > 0) ? lista[0] : null;
        }

        public void GuardarInforme(InvgccEjecucionInformes inf)
        {
            // IDENTITY: SI -> NO enviamos strId_informe
            // FK_EJEC: INT -> NO lleva comillas {inf.fkId_ejec}
            // FECHA: DATETIME -> yyyy-MM-dd HH:mm:ss

            string fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string rutaSegura = inf.strArchivo_path.Replace("'", "''");

            string sql = $@"
                INSERT INTO INVGCCEJECUCION_INFORMES 
                (fkId_ejec, strNombrePeriodo, strArchivo_path, dtFechaSubida)
                VALUES 
                ({inf.fkId_ejec}, '{inf.strNombrePeriodo}', '{rutaSegura}', '{fecha}')";

            _dal.UpdateSql(sql);
        }


        public void ActualizarInforme(InvgccEjecucionInformes inf)
        {
            // Usamos SQL Manual para no tocar el ID
            string fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string nombreLimpio = inf.strNombrePeriodo.Replace("'", "''");
            string rutaSegura = inf.strArchivo_path.Replace("'", "''");

            // Si la ruta viene vacía (no subió archivo nuevo), NO la actualizamos
            string sql;

            if (!string.IsNullOrEmpty(inf.strArchivo_path))
            {
                // Actualiza Nombre y Archivo
                sql = $@"UPDATE INVGCCEJECUCION_INFORMES SET 
                        strNombrePeriodo = '{nombreLimpio}', 
                        strArchivo_path = '{rutaSegura}', 
                        dtFechaSubida = '{fecha}'
                        WHERE strId_informe = {inf.strId_informe}";
            }
            else
            {
                // Actualiza solo Nombre (mantiene archivo viejo)
                sql = $@"UPDATE INVGCCEJECUCION_INFORMES SET 
                        strNombrePeriodo = '{nombreLimpio}'
                        WHERE strId_informe = {inf.strId_informe}";
            }

            _dal.UpdateSql(sql);
        }

        public void EliminarInforme(int idInforme)
        {
            _dal.Delete("INVGCCEJECUCION_INFORMES", $"strId_informe = {idInforme}");
        }

        // =============================================================
        // 4. UTILIDADES
        // =============================================================

        public List<InvgccInscripcionProyectos> ObtenerProyectosAprobadosSinEjecucion()
        {
            string sql = @"
                SELECT strId_pro, strTema_pro, strCoordinador_pro 
                FROM INVGCCINSCRIPCION_PROYECTOS 
                WHERE strEstado_pro = 'Aprobado' 
                AND strId_pro NOT IN (SELECT fkId_pro FROM INVGCCEJECUCION_PROYECTO)";

            return _dal.SelectSql<InvgccInscripcionProyectos>(sql);
        }
    }
}