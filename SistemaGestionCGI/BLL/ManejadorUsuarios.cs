using System.Collections.Generic;
using SistemaGestionCGI.Models;
using SistemaGestionCGI.Settings;

namespace SistemaGestionCGI.BLL
{
    public class ManejadorUsuarios
    {
        private readonly ConnectionSqlServer _dal = ConnectionSqlServer.Instance;

        public InvgccUsuario Autenticar(string usuario, string clave)
        {
            string userLimpio = usuario.Trim();
            string claveLimpia = clave.Trim();

            // Consulta a la tabla REAL "Users"
            string sql = $@"
                SELECT UserID, Username, Password, Role, IsActive
                FROM Users 
                WHERE Username = '{userLimpio}' AND Password = '{claveLimpia}'";

            List<InvgccUsuario> resultado = _dal.SelectSql<InvgccUsuario>(sql);

            if (resultado != null && resultado.Count > 0)
            {
                var usuarioEncontrado = resultado[0];

                if (usuarioEncontrado.bActivo_usu == true)
                {
                    return usuarioEncontrado;
                }
                else
                {
                    return null; // Usuario inactivo
                }
            }
            else
            {
                return null;
            }
        }
    }
}