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
            string claveEncriptada = SeguridadHelper.EncriptarSHA256(clave.Trim());

            string sql = $@"
                SELECT UserID, Username, Password, Role, IsActive 
                FROM Users 
                WHERE Username = '{userLimpio}' 
                AND Password = '{claveEncriptada}' 
                AND IsActive = 1";

            List<InvgccUsuario> resultado = _dal.SelectSql<InvgccUsuario>(sql);

            if (resultado != null && resultado.Count > 0)
            {
                return resultado[0];
            }
            return null;
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
            string claveEncriptada = SeguridadHelper.EncriptarSHA256(u.strClave_usu.Trim());

            int activo = u.bActivo_usu ? 1 : 0;
            string sql = $@"
                INSERT INTO Users (Username, Password, Role, IsActive)
                VALUES ('{u.strNombre_usu.Trim()}', '{claveEncriptada}', '{u.strRol_usu}', {activo})";

            _dal.UpdateSql(sql);
        }

        // 4. ACTUALIZAR
        public void ActualizarUsuario(InvgccUsuario u)
        {
            int activo = u.bActivo_usu ? 1 : 0;
            string sql = "";

            string passwordFinal = u.strClave_usu.Trim();

            if (passwordFinal.Length != 64)
            {
                passwordFinal = SeguridadHelper.EncriptarSHA256(passwordFinal);
            }

            sql = $@"
                UPDATE Users 
                SET Username = '{u.strNombre_usu.Trim()}',
                    Password = '{passwordFinal}', 
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