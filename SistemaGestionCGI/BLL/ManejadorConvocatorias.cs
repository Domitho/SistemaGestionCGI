using System;
using System.Collections.Generic;
using System.Linq; // Necesario para .FirstOrDefault()
using Newtonsoft.Json.Linq;
using SistemaGestionCGI.Models;
using SistemaGestionCGI.Settings;

namespace SistemaGestionCGI.BLL
{
    public class ManejadorConvocatorias
    {
        private readonly ConnectionSqlServer _dal = ConnectionSqlServer.Instance;

        // =============================================================
        // 1. GESTIÓN DE CONVOCATORIAS
        // =============================================================

        public List<InvgccConvocatoriaGruInvestigacion> ObtenerConvocatorias()
        {
            string sql = "SELECT * FROM INVGCCCONVOCATORIA_GRUPOS_INVESTIGACION ORDER BY dtFechaini_conv DESC";
            return _dal.SelectSql<InvgccConvocatoriaGruInvestigacion>(sql);
        }

        public InvgccConvocatoriaGruInvestigacion ObtenerConvocatoriaPorId(string id)
        {
            string sql = $"SELECT * FROM INVGCCCONVOCATORIA_GRUPOS_INVESTIGACION WHERE strId_conv = '{id}'";
            var lista = _dal.SelectSql<InvgccConvocatoriaGruInvestigacion>(sql);
            return lista?.FirstOrDefault();
        }

        public void GuardarConvocatoria(InvgccConvocatoriaGruInvestigacion conv)
        {
            // 1. Generar ID
            conv.strId_conv = GenerarCodigoAlfanumerico("INVGCCCONVOCATORIA_GRUPOS_INVESTIGACION", "strId_conv", "CONV");

            // 2. Sanitizar textos (evitar error por comillas simples)
            string nombre = conv.strNombre_conv.Replace("'", "''");
            string desc = conv.strDescripcion_conv.Replace("'", "''");
            string archivo = conv.strArchivo_conv.Replace("'", "''");

            // 3. SQL Manual (Copiando el estilo de GuardarIntegrante del ejemplo)
            string sql = $@"
                INSERT INTO INVGCCCONVOCATORIA_GRUPOS_INVESTIGACION 
                (strId_conv, strNombre_conv, strDescripcion_conv, strArchivo_conv, dtFechaini_conv, dtFechafin_conv)
                VALUES 
                ('{conv.strId_conv}', '{nombre}', '{desc}', '{archivo}', 
                 '{conv.dtFechaini_conv:yyyy-MM-dd HH:mm:ss}', '{conv.dtFechafin_conv:yyyy-MM-dd HH:mm:ss}')";

            _dal.UpdateSql(sql); // Usamos UpdateSql que ejecuta cualquier comando SQL
        }

        public void ActualizarConvocatoria(InvgccConvocatoriaGruInvestigacion conv)
        {
            // Sanitizar textos
            string nombre = conv.strNombre_conv.Replace("'", "''");
            string desc = conv.strDescripcion_conv.Replace("'", "''");
            string archivo = conv.strArchivo_conv.Replace("'", "''");

            string sql = $@"
                UPDATE INVGCCCONVOCATORIA_GRUPOS_INVESTIGACION SET 
                    strNombre_conv = '{nombre}',
                    strDescripcion_conv = '{desc}',
                    strArchivo_conv = '{archivo}',
                    dtFechaini_conv = '{conv.dtFechaini_conv:yyyy-MM-dd HH:mm:ss}',
                    dtFechafin_conv = '{conv.dtFechafin_conv:yyyy-MM-dd HH:mm:ss}'
                WHERE strId_conv = '{conv.strId_conv}'";

            _dal.UpdateSql(sql);
        }

        public void EliminarConvocatoria(string id)
        {
            _dal.Delete("INVGCCCONVOCATORIA_GRUPOS_INVESTIGACION", $"strId_conv = '{id}'");
        }

        // =============================================================
        // 2. UTILIDADES (Generador de IDs - Patrón Idéntico)
        // =============================================================

        private string GenerarCodigoAlfanumerico(string tabla, string campoId, string prefijo)
        {
            string sql = $"SELECT TOP 1 {campoId} FROM {tabla} ORDER BY Len({campoId}) DESC, {campoId} DESC";
            var lista = _dal.SelectSql<dynamic>(sql);
            int siguienteNumero = 1;

            if (lista != null && lista.Count > 0)
            {
                string ultimoId = "";
                var item = lista[0];

                // Manejo robusto de dynamic (Copiado de tu ejemplo)
                if (item is JObject jobj)
                    ultimoId = jobj[campoId]?.ToString();
                else
                    try { ultimoId = ((dynamic)item).GetType().GetProperty(campoId).GetValue(item, null).ToString(); } catch { }

                if (!string.IsNullOrEmpty(ultimoId) && ultimoId.StartsWith(prefijo))
                {
                    string numeroStr = ultimoId.Substring(prefijo.Length);
                    if (int.TryParse(numeroStr, out int numeroActual))
                    {
                        siguienteNumero = numeroActual + 1;
                    }
                }
            }

            return prefijo + siguienteNumero;
        }
    }
}