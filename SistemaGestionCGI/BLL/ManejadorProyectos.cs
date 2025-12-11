using System;
using System.Collections.Generic;
using SistemaGestionCGI.Models;
using SistemaGestionCGI.Settings;

namespace SistemaGestionCGI.BLL
{
    public class ManejadorProyectos
    {
        private readonly ConnectionSqlServer _dal = ConnectionSqlServer.Instance;

        public List<InvgccInsPro> ObtenerTodos()
        {
            string sql = @"
                SELECT P.strId_pro, P.strTema_pro, P.strCoordinador_pro, P.strFacultad_pro, 
                       P.strDuracion_pro, P.dtFehains_pro, P.strArchivo_pro, P.strEstado_pro, 
                       P.fkId_conv, P.fkId_gru, 
                       G.strNombre_gru, C.strNombre_conv 
                FROM INVGCCPROYECTO P 
                INNER JOIN INVGCCGRUPO_INVESTIGACION G ON P.fkId_gru = G.strId_gru
                INNER JOIN INVGCCCONVOCATORI C ON P.fkId_conv = C.strId_conv
                ORDER BY P.dtFehains_pro DESC";

            return _dal.SelectSql<InvgccInsPro>(sql);
        }

        public InvgccInsPro ObtenerPorId(string id)
        {
            string sql = $"SELECT * FROM INVGCCPROYECTO WHERE strId_pro = '{id}'";
            var lista = _dal.SelectSql<InvgccInsPro>(sql);

            if (lista != null && lista.Count > 0)
                return lista[0];
            else
                return null;
        }

        public void Guardar(InvgccInsPro pro)
        {
            string nuevoId = GenerarNuevoId();
            pro.strId_pro = nuevoId;
            pro.strEstado_pro = "Pendiente";

            _dal.Insert("INVGCCPROYECTO", pro);
        }

        public void Actualizar(InvgccInsPro pro)
        {
            string sql = $@"
                UPDATE INVGCCPROYECTO SET 
                    strTema_pro = '{pro.strTema_pro}',
                    strCoordinador_pro = '{pro.strCoordinador_pro}',
                    strFacultad_pro = '{pro.strFacultad_pro}',
                    strDuracion_pro = '{pro.strDuracion_pro}',
                    dtFehains_pro = '{pro.dtFehains_pro:yyyy-MM-dd HH:mm:ss}',
                    fkId_gru = '{pro.fkId_gru}',
                    fkId_conv = '{pro.fkId_conv}',
                    strArchivo_pro = '{pro.strArchivo_pro}'
                WHERE strId_pro = '{pro.strId_pro}'";

            _dal.UpdateSql(sql);
        }

        public void Eliminar(string id)
        {
            _dal.Delete("INVGCCPROYECTO", $"strId_pro = '{id}'");
        }

        public void AlternarEstado(string id)
        {
            string sql = $@"
                UPDATE INVGCCPROYECTO
                SET strEstado_pro = CASE 
                    WHEN strEstado_pro = 'Pendiente' THEN 'Aprobado' 
                    ELSE 'Pendiente' 
                END
                WHERE strId_pro = '{id}'";

            _dal.UpdateSql(sql);
        }

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

        private string GenerarNuevoId()
        {
            string sql = "SELECT TOP 1 strId_pro FROM INVGCCPROYECTO ORDER BY Len(strId_pro) DESC, strId_pro DESC";
            var lista = _dal.SelectSql<InvgccInsPro>(sql);

            int n = 1;
            if (lista != null && lista.Count > 0)
            {
                string ultimo = lista[0].strId_pro;
                if (ultimo.StartsWith("P"))
                {
                    int.TryParse(ultimo.Substring(1), out n);
                    n++;
                }
            }
            return "P" + n;
        }
    }
}