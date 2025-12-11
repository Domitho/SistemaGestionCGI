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
        // 1. GESTIÓN DE EJECUCIÓN (Tabla: INVGCCEJECUCION_PROYECTO)
        // =============================================================

        public List<InvgccEjecucionProyectos> ObtenerEjecuciones()
        {
            string sql = @"
                SELECT E.*, P.strTema_pro as TituloProyecto 
                FROM INVGCCEJECUCION_PROYECTO E
                INNER JOIN INVGCCPROYECTO P ON E.fkId_pro = P.strId_pro
                ORDER BY E.strId_ejec DESC"; // Orden por ID descendente

            return _dal.SelectSql<InvgccEjecucionProyectos>(sql);
        }

        public InvgccEjecucionProyectos ObtenerEjecucionPorId(string id)
        {
            string sql = $@"
                SELECT E.*, P.strTema_pro as TituloProyecto 
                FROM INVGCCEJECUCION_PROYECTO E
                INNER JOIN INVGCCPROYECTO P ON E.fkId_pro = P.strId_pro
                WHERE E.strId_ejec = '{id}'";

            var lista = _dal.SelectSql<InvgccEjecucionProyectos>(sql);
            return (lista != null && lista.Count > 0) ? lista[0] : null;
        }

        public void GuardarEjecucion(InvgccEjecucionProyectos obj)
        {

            obj.strEstado_ejec = "En Ejecución";

            _dal.Insert("INVGCCEJECUCION_PROYECTO", obj);
        }

        public void ActualizarEjecucion(InvgccEjecucionProyectos obj)
        {
            string fechaFin = obj.dtFechafin_ejec.HasValue
                ? $"'{obj.dtFechafin_ejec.Value:yyyy-MM-dd}'"
                : "NULL";

            string sql = $@"
                UPDATE INVGCCEJECUCION_PROYECTO SET 
                    strCoordinador_ejec = '{obj.strCoordinador_ejec}',
                    strPeriodo_ejec = '{obj.strPeriodo_ejec}',
                    dtFechaini_ejec = '{obj.dtFechaini_ejec:yyyy-MM-dd}',
                    dtFechafin_ejec = {fechaFin}, 
                    strInforme_ejec = '{obj.strInforme_ejec}'
                WHERE strId_ejec = '{obj.strId_ejec}'";

            _dal.UpdateSql(sql);
        }

        public void EliminarEjecucion(string id)
        {
            // 1. Eliminar hijos primero
            _dal.DeleteSql($"DELETE FROM INVGCCEJECUCION_MIEMBROS WHERE fkId_ejec = '{id}'");
            _dal.DeleteSql($"DELETE FROM INVGCCEJECUCION_INFORMES WHERE fkId_ejec = '{id}'");

            // 2. Eliminar padre
            _dal.Delete("INVGCCEJECUCION_PROYECTO", $"strId_ejec = '{id}'");
        }

        // =============================================================
        // 2. GESTIÓN DE MIEMBROS (Tabla: INVGCCEJECUCION_MIEMBROS)
        // =============================================================

        public List<InvgccEjecucionMiembros> ObtenerMiembros(string idEjecucion)
        {
            string sql = $"SELECT * FROM INVGCCEJECUCION_MIEMBROS WHERE fkId_ejec = '{idEjecucion}' AND bitActivo_miembro = 1";
            return _dal.SelectSql<InvgccEjecucionMiembros>(sql);
        }

        public void GuardarMiembro(InvgccEjecucionMiembros miembro)
        {
            // ⚠️ CORRECCIÓN: ID Autoincremental
            miembro.bitActivo_miembro = true;

            _dal.Insert("INVGCCEJECUCION_MIEMBROS", miembro);
        }

        public void EliminarMiembro(string idMiembro)
        {
            // Soft Delete
            string sql = $"UPDATE INVGCCEJECUCION_MIEMBROS SET bitActivo_miembro = 0 WHERE strId_miembro = '{idMiembro}'";
            _dal.UpdateSql(sql);
        }

        public InvgccEjecucionMiembros ObtenerMiembroPorId(string id)
        {
            string sql = $"SELECT * FROM INVGCCEJECUCION_MIEMBROS WHERE strId_miembro = '{id}'";
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
                    strRol_miembro = '{obj.strRol_miembro}'
                WHERE strId_miembro = '{obj.strId_miembro}'";

            _dal.UpdateSql(sql);
        }

        // =============================================================
        // 3. GESTIÓN DE INFORMES (Tabla: INVGCCEJECUCION_INFORMES)
        // =============================================================

        public List<InvgccEjecucionInformes> ObtenerInformes(string idEjecucion)
        {
            string sql = $"SELECT * FROM INVGCCEJECUCION_INFORMES WHERE fkId_ejec = '{idEjecucion}' ORDER BY dtFechaSubida DESC";
            return _dal.SelectSql<InvgccEjecucionInformes>(sql);
        }

        public void GuardarInforme(InvgccEjecucionInformes inf)
        {
            // ⚠️ CORRECCIÓN: ID Autoincremental
            inf.dtFechaSubida = DateTime.Now;

            _dal.Insert("INVGCCEJECUCION_INFORMES", inf);
        }

        public void EliminarInforme(string idInforme)
        {
            _dal.Delete("INVGCCEJECUCION_INFORMES", $"strId_informe = '{idInforme}'");
        }

        // =============================================================
        // 4. UTILIDADES
        // =============================================================

        public List<InvgccInsPro> ObtenerProyectosAprobadosSinEjecucion()
        {
            string sql = @"
                SELECT strId_pro, strTema_pro, strCoordinador_pro 
                FROM INVGCCPROYECTO 
                WHERE strEstado_pro = 'Aprobado' 
                AND strId_pro NOT IN (SELECT fkId_pro FROM INVGCCEJECUCION_PROYECTO)";

            return _dal.SelectSql<InvgccInsPro>(sql);
        }
    }
}