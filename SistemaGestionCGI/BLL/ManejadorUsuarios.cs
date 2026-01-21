using System.Collections.Generic;
using SistemaGestionCGI.Models;
using SistemaGestionCGI.Settings;

namespace SistemaGestionCGI.BLL
{
    public class ManejadorUsuarios
    {
        private readonly ConnectionSqlServer _dal = ConnectionSqlServer.Instance;

        // =========================================================
        // 1. LOGIN
        // =========================================================
        public InvgccUsuario Autenticar(string usuario, string clave)
        {
            string userLimpio = usuario.Trim();
            // Asegúrate de que este hash coincida con el de tu BD
            string claveEncriptada = SeguridadHelper.EncriptarSHA256(clave.Trim());

            // Usamos nombres en INGLÉS porque tu modelo tiene [JsonProperty("UserID")]
            string sql = $@"
                SELECT 
                    UserID, 
                    Username, 
                    Password, 
                    Role, 
                    IsActive, 
                    strCedula_ref -- Este campo es vital para la seguridad
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

        // =========================================================
        // 2. LISTAR TODOS
        // =========================================================
        public List<InvgccUsuario> ObtenerUsuarios()
        {
            // Sin Alias, dejamos que JsonProperty haga el trabajo
            string sql = "SELECT UserID, Username, Password, Role, IsActive, strCedula_ref FROM Users ORDER BY Username ASC";
            return _dal.SelectSql<InvgccUsuario>(sql);
        }

        // =========================================================
        // 3. OBTENER POR ID (CORREGIDO: QUITAMOS LOS ALIAS)
        // =========================================================
        public InvgccUsuario ObtenerUsuarioPorId(int id)
        {
            // CORRECCIÓN: Quitamos 'AS intId_usu', usamos 'UserID' para que coincida con el JsonProperty
            string sql = $@"
                SELECT 
                    UserID, 
                    Username, 
                    Password, -- Necesitamos el hash para no perderlo al editar
                    Role, 
                    IsActive,
                    strCedula_ref
                FROM Users 
                WHERE UserID = {id}";

            var lista = _dal.SelectSql<InvgccUsuario>(sql);
            return (lista != null && lista.Count > 0) ? lista[0] : null;
        }

        // =========================================================
        // 4. GUARDAR
        // =========================================================
        public void GuardarUsuario(InvgccUsuario u)
        {
            string claveEncriptada = SeguridadHelper.EncriptarSHA256(u.strClave_usu.Trim());
            int activo = u.bActivo_usu ? 1 : 0;

            // Manejo de Nulos para la cédula
            string cedula = string.IsNullOrEmpty(u.strCedula_ref) ? "NULL" : $"'{u.strCedula_ref}'";

            string sql = $@"
                INSERT INTO Users (Username, Password, Role, IsActive, strCedula_ref)
                VALUES ('{u.strNombre_usu.Trim()}', '{claveEncriptada}', '{u.strRol_usu}', {activo}, {cedula})";

            _dal.UpdateSql(sql);
        }

        // =========================================================
        // 5. ACTUALIZAR
        // =========================================================
        public void ActualizarUsuario(InvgccUsuario u)
        {
            int activo = u.bActivo_usu ? 1 : 0;
            string cedula = string.IsNullOrEmpty(u.strCedula_ref) ? "NULL" : $"'{u.strCedula_ref}'";

            string passwordFinal = u.strClave_usu.Trim();
            string sql;

            // Si la longitud no es 64 (SHA256), asumimos que es una nueva clave en texto plano
            if (passwordFinal.Length != 64)
            {
                passwordFinal = SeguridadHelper.EncriptarSHA256(passwordFinal);

                sql = $@"
                    UPDATE Users 
                    SET Username = '{u.strNombre_usu.Trim()}',
                        Password = '{passwordFinal}', 
                        Role = '{u.strRol_usu}',
                        IsActive = {activo},
                        strCedula_ref = {cedula}
                    WHERE UserID = {u.intId_usu}";
            }
            else
            {
                // No tocamos el Password
                sql = $@"
                    UPDATE Users 
                    SET Username = '{u.strNombre_usu.Trim()}',
                        Role = '{u.strRol_usu}',
                        IsActive = {activo},
                        strCedula_ref = {cedula}
                    WHERE UserID = {u.intId_usu}";
            }

            _dal.UpdateSql(sql);
        }

        // =========================================================
        // 6. ELIMINAR
        // =========================================================
        public void EliminarUsuario(int id)
        {
            _dal.UpdateSql($"UPDATE Users SET IsActive = 0 WHERE UserID = {id}");
        }


        public List<InvgccUsuario> ObtenerCoordinadoresPendientes()
        {

            string sql = @"
                SELECT DISTINCT 
                    strCedulaCoordinador_ejec AS strCedula_ref,
                    (strCoordinador_ejec + ' (' + strCedulaCoordinador_ejec + ')') AS Username 
                FROM INVGCCEJECUCION_PROYECTO
                WHERE strEstado_ejec = 'En Ejecución'
                AND strCedulaCoordinador_ejec IS NOT NULL 
                AND strCedulaCoordinador_ejec NOT IN (SELECT strCedula_ref FROM Users WHERE strCedula_ref IS NOT NULL)";

            return _dal.SelectSql<InvgccUsuario>(sql);
        }

    }
}