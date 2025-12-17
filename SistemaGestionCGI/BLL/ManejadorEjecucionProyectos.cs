using System;
using System.Collections.Generic;
using System.Linq; // Necesario para .FirstOrDefault()
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
            return lista?.FirstOrDefault();
        }

        public void GuardarEjecucion(InvgccEjecucionProyectos obj)
        {
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
        // 2. GESTIÓN DE MIEMBROS
        // =============================================================

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
                (fkId_ejec, strCedula_miembro, strNombres_miembro, strApellidos_miembro, strRol_miembro, strFacultad_miembro, bitActivo_miembro)
                VALUES 
                ({m.fkId_ejec}, '{m.strCedula_miembro}', '{m.strNombres_miembro}', '{m.strApellidos_miembro}', '{m.strRol_miembro}', '{m.strFacultad_miembro}', 1)";

            _dal.UpdateSql(sql);
        }

        public void EliminarMiembro(int idMiembro) =>
            _dal.UpdateSql($"UPDATE INVGCCEJECUCION_MIEMBROS SET bitActivo_miembro = 0 WHERE strId_miembro = {idMiembro}");

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
                    strFacultad_miembro = '{obj.strFacultad_miembro}'
                WHERE strId_miembro = {obj.strId_miembro}";

            _dal.UpdateSql(sql);
        }

        // =============================================================
        // 3. GESTIÓN DE INFORMES
        // =============================================================

        public List<InvgccEjecucionInformes> ObtenerInformes(int idEjecucion)
        {
            string sql = $"SELECT * FROM INVGCCEJECUCION_INFORMES WHERE fkId_ejec = {idEjecucion} ORDER BY dtFechaSubida DESC";
            return _dal.SelectSql<InvgccEjecucionInformes>(sql);
        }

        public InvgccEjecucionInformes ObtenerInformePorId(int id)
        {
            string sql = $"SELECT * FROM INVGCCEJECUCION_INFORMES WHERE strId_informe = {id}";
            return _dal.SelectSql<InvgccEjecucionInformes>(sql)?.FirstOrDefault();
        }

        public void GuardarInforme(InvgccEjecucionInformes inf)
        {
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

        // =============================================================
        // 4. GESTIÓN DE AUDITORÍA Y ESTADOS
        // =============================================================

        public void CambiarEstadoMiembro(int idMiembro, bool nuevoEstado, string motivo, string usuario)
        {
            int bit = nuevoEstado ? 1 : 0;
            string sqlUpdate = $"UPDATE INVGCCEJECUCION_MIEMBROS SET bitActivo_miembro = {bit} WHERE strId_miembro = {idMiembro}";
            _dal.UpdateSql(sqlUpdate);

            string accion = nuevoEstado ? "REACTIVACIÓN" : "BAJA";
            RegistrarHistorialMiembro(idMiembro, accion, motivo, usuario);
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

        // =============================================================
        // 5. UTILIDADES
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