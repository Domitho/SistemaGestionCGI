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
                    return null; 
                }
            }
            else
            {
                return null;
            }
        }


        // 1. LISTAR
        public List<InvgccUsuario> ObtenerUsuarios()
        {
            string sql = "SELECT UserID, Username, Password, Role, IsActive FROM Users ORDER BY Username ASC";
            return _dal.SelectSql<InvgccUsuario>(sql);
        }

        // 2. OBTENER INDIVIDUAL
        public InvgccUsuario ObtenerUsuarioPorId(int id)
        {
            string sql = $"SELECT UserID, Username, Password, Role, IsActive FROM Users WHERE UserID = {id}";
            var lista = _dal.SelectSql<InvgccUsuario>(sql);
            return (lista != null && lista.Count > 0) ? lista[0] : null;
        }

        // 3. GUARDAR
        public void GuardarUsuario(InvgccUsuario u)
        {
            int activo = u.bActivo_usu ? 1 : 0;
            string sql = $@"
                INSERT INTO Users (Username, Password, Role, IsActive)
                VALUES ('{u.strNombre_usu.Trim()}', '{u.strClave_usu.Trim()}', '{u.strRol_usu}', {activo})";
            _dal.UpdateSql(sql);
        }

        // 4. ACTUALIZAR
        public void ActualizarUsuario(InvgccUsuario u)
        {
            int activo = u.bActivo_usu ? 1 : 0;
            string sql = $@"
                UPDATE Users 
                SET Username = '{u.strNombre_usu.Trim()}',
                    Password = '{u.strClave_usu.Trim()}', 
                    Role = '{u.strRol_usu}',
                    IsActive = {activo}
                WHERE UserID = {u.intId_usu}";
            _dal.UpdateSql(sql);
        }

        // 5. ELIMINAR (Baja Lógica)
        public void EliminarUsuario(int id)
        {
            string sql = $"UPDATE Users SET IsActive = 0 WHERE UserID = {id}";
            _dal.UpdateSql(sql);
        }

    }
}