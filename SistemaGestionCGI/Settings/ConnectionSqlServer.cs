using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace SistemaGestionCGI.Settings
{
    public class ConnectionSqlServer
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public static ConnectionSqlServer Instance { get; } = new ConnectionSqlServer();
        readonly NumberFormatInfo nfi = new NumberFormatInfo();

        private string CreateConnectionString()
        {
            string connection = @"server=DESKTOP-A925LIU\SQLEXPRESS2019;database=INVESTIGACION;INTEGRATED SECURITY=true";
            connection = string.Format(connection, Server, Database);
            return connection;
        }


        /// <summary>
        /// Inicializacion de Conexion
        /// </summary>
        private SqlConnection InitConnection(SqlConnection databaseConnection)
        {
            if (databaseConnection == null || databaseConnection.State != ConnectionState.Open)
            {
                databaseConnection.Open();
            }
            return databaseConnection;
        }

        /// <summary>
        /// Cerrar Conexion
        /// </summary>
        private void CloseConnection(SqlConnection databaseConnection)
        {
            if (databaseConnection != null && databaseConnection.State != ConnectionState.Closed)
            {
                databaseConnection.Close();
            }
        }

        /// <summary>
        /// Sentencia Select
        /// </summary>
        /// <param name="table">Tabla de consulta</param>
        /// <param name="where">Where de Sentencia</param>
        /// <param name="filter">Datos a consultar, por defecto es todos</param>
        /// <returns></returns>
        public string Select(string table, string where = "", string filter = "*")
        {
            string connectionString = CreateConnectionString();
            SqlConnection databaseConnection = new SqlConnection(connectionString);
            ArrayList data = new ArrayList();
            string sql = "SELECT {0} FROM {1}";
            if (where.Trim() != "")
            {
                sql = sql + " WHERE " + where;
            }
            sql = string.Format(sql, filter, table);
            try
            {
                databaseConnection = InitConnection(databaseConnection);

                var commandDatabase = new SqlCommand(sql, databaseConnection)
                {
                    CommandTimeout = 60
                };

                var reader = commandDatabase.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Hashtable fields = new Hashtable();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            fields.Add(reader.GetName(i), GetTypeValue(reader.GetValue(i), reader.GetFieldType(i)));
                        }
                        data.Add(fields);
                    }
                }
                else
                {
                    Console.WriteLine("No se encontraron datos.");
                }
            }
            catch (SqlException ex)
            {
                CloseConnection(databaseConnection);
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                CloseConnection(databaseConnection);
                throw new Exception(ex.Message);
            }
            finally
            {
                CloseConnection(databaseConnection);
            }
            return JsonConvert.SerializeObject(data);
        }

        internal int ExecuteScalar<T>(string query)
        {
            throw new NotImplementedException();
        }

        internal object SelectSql<T>(string query, object p)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sentencia Select
        /// </summary>
        /// <param name="table">Tabla de consulta</param>
        /// <param name="where">Where de Sentencia</param>
        /// <param name="filter">Datos a consultar, por defecto es todos</param>
        /// <returns></returns>
        public List<T> Select<T>(string table, string where = "", string filter = "*")
        {
            string connectionString = CreateConnectionString();
            SqlConnection databaseConnection = new SqlConnection(connectionString);
            ArrayList data = new ArrayList();
            string sql = "SELECT {0} FROM {1}";
            if (where.Trim() != "")
            {
                sql = sql + " WHERE " + where;
            }
            sql = string.Format(sql, filter, table);

            try
            {
                databaseConnection = InitConnection(databaseConnection);
                var commandDatabase = new SqlCommand(sql, databaseConnection)
                {
                    CommandTimeout = 60
                };

                var reader = commandDatabase.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Hashtable fields = new Hashtable();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            fields.Add(reader.GetName(i), GetTypeValue(reader.GetValue(i), reader.GetFieldType(i)));
                        }
                        data.Add(fields);
                    }
                }
                else
                {
                    Console.WriteLine("No se encontraron datos.");
                }

            }
            catch (SqlException ex)
            {
                CloseConnection(databaseConnection);
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                CloseConnection(databaseConnection);
                throw new Exception(ex.Message);
            }
            finally
            {
                CloseConnection(databaseConnection);
            }
            return JsonConvert.DeserializeObject<List<T>>(JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// Sentencia Select
        /// </summary>
        /// <param name="sql">Código SQL para consulta</param>
        /// <returns></returns>
        public string SelectSql(string sql)
        {
            string connectionString = CreateConnectionString();
            SqlConnection databaseConnection = new SqlConnection(connectionString);
            ArrayList data = new ArrayList();

            try
            {
                databaseConnection = InitConnection(databaseConnection);
                var commandDatabase = new SqlCommand(sql, databaseConnection)
                {
                    CommandTimeout = 60
                };
                var reader = commandDatabase.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Hashtable fields = new Hashtable();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            fields.Add(reader.GetName(i), GetTypeValue(reader.GetValue(i), reader.GetFieldType(i)));
                        }
                        data.Add(fields);
                    }
                }
                else
                {
                    Console.WriteLine("No se encontraron datos.");
                }
            }
            catch (SqlException ex)
            {
                CloseConnection(databaseConnection);
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                CloseConnection(databaseConnection);
                throw new Exception(ex.Message);
            }
            finally
            {
                CloseConnection(databaseConnection);
            }
            return JsonConvert.SerializeObject(data);
        }

        /// <summary>
        /// Sentencia Select
        /// </summary>
        /// <param name="sql">Código SQL para consulta</param>
        /// <returns></returns>
        public List<T> SelectSql<T>(string sql)
        {
            string connectionString = CreateConnectionString();
            SqlConnection databaseConnection = new SqlConnection(connectionString);
            ArrayList data = new ArrayList();

            try
            {
                databaseConnection = InitConnection(databaseConnection);
                var commandDatabase = new SqlCommand(sql, databaseConnection)
                {
                    CommandTimeout = 60
                };
                var reader = commandDatabase.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Hashtable fields = new Hashtable();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            fields.Add(reader.GetName(i), GetTypeValue(reader.GetValue(i), reader.GetFieldType(i)));
                        }
                        data.Add(fields);
                    }
                }
                else
                {
                    Console.WriteLine("No se encontraron datos.");
                }

            }
            catch (SqlException ex)
            {
                CloseConnection(databaseConnection);
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                CloseConnection(databaseConnection);
                throw new Exception(ex.Message);
            }
            finally
            {
                CloseConnection(databaseConnection);
            }
            return JsonConvert.DeserializeObject<List<T>>(JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// Sentencia Insert
        /// </summary>
        /// <param name="table">Tabla de consulta</param>
        /// <param name="data">Lista de datos a insertar</param>
        /// <returns></returns>
        public string Insert(string table, List<Hashtable> data)
        {
            string connectionString = CreateConnectionString();
            SqlConnection databaseConnection = new SqlConnection(connectionString);

            string sql = string.Empty;
            int cont = 0;
            foreach (var hashdata in data)
            {
                var list = SetStandarValues(hashdata);
                string fields = "";
                string values = "";
                foreach (string k in list.Keys)
                {
                    // Console.WriteLine(list[k].GetType() + " - " + list[k]);
                    string value = GetType(list[k]);
                    if (value.Trim() != "")
                    {
                        fields = fields + table + "." + k + ",";
                        values = values + value + ",";
                    }
                }
                if (values != "" && fields != "")
                {
                    fields = fields.TrimEnd(',');
                    values = values.TrimEnd(',');
                    sql = sql + "INSERT INTO " + table + "(" + fields + ") VALUES (" + values + ");";
                    cont++;
                }
            }
            if (cont != 0)
            {
                try
                {
                    InitConnection(databaseConnection);
                    var commandDatabase = new SqlCommand(sql, databaseConnection)
                    {
                        CommandTimeout = 60
                    };
                    SqlDataReader myReader = commandDatabase.ExecuteReader();
                }
                catch (SqlException ex)
                {
                    CloseConnection(databaseConnection);
                    throw new Exception(ex.Message);
                }
                catch (Exception ex)
                {
                    CloseConnection(databaseConnection);
                    throw new Exception(ex.Message);
                }
                finally
                {
                    CloseConnection(databaseConnection);
                }
            }
            return "Proceso Terminado";
        }

        /// <summary>
        /// Sentencia Insert
        /// </summary>
        /// <param name="table">Tabla de consulta</param>
        /// <param name="data">Lista de datos a insertar</param>
        /// <returns></returns>
        public T Insert<T>(string table, object data)
        {
            var element = JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(data));
            List<Hashtable> dataList = new List<Hashtable> { element };
            string connectionString = CreateConnectionString();
            SqlConnection databaseConnection = new SqlConnection(connectionString);
            string uuid = "";
            string sql = string.Empty;
            int cont = 0;
            foreach (var hashdata in dataList)
            {
                var list = SetStandarValues(hashdata);
                string fields = "";
                string values = "";
                foreach (string k in list.Keys)
                {
                    // Console.WriteLine(list[k].GetType() + " - " + list[k]);
                    string value = GetType(list[k]);
                    if (k == "uuid")
                    {
                        uuid = value;
                    }
                    if (value.Trim() != "")
                    {
                        fields = fields + table + "." + k + ",";
                        values = values + value + ",";
                    }
                }
                if (values != "" && fields != "")
                {
                    fields = fields.TrimEnd(',');
                    values = values.TrimEnd(',');
                    sql = sql + "INSERT INTO " + table + "(" + fields + ") VALUES (" + values + ");";
                    cont++;
                }
            }
            if (cont != 0)
            {
                try
                {
                    InitConnection(databaseConnection);
                    var commandDatabase = new SqlCommand(sql, databaseConnection)
                    {
                        CommandTimeout = 60
                    };
                    SqlDataReader myReader = commandDatabase.ExecuteReader();
                }
                catch (SqlException ex)
                {
                    CloseConnection(databaseConnection);
                    throw new Exception(ex.Message);
                }
                catch (Exception ex)
                {
                    CloseConnection(databaseConnection);
                    throw new Exception(ex.Message);
                }
                finally
                {
                    CloseConnection(databaseConnection);
                }
            }
            return Select<T>(table, $"uuid = {uuid}").First();
        }

        /// <summary>
        /// Sentencia Insert
        /// </summary>
        /// <param name="table">Tabla de consulta</param>
        /// <param name="data">Lista de datos a insertar</param>
        /// <returns></returns>
        public string Insert(string table, List<object> data)
        {
            var dataList = JsonConvert.DeserializeObject<List<Hashtable>>(JsonConvert.SerializeObject(data));
            Insert(table, dataList);
            return "Proceso Terminado";
        }

        /// <summary>
        /// Sentencia Insert
        /// </summary>
        /// <param name="table">Tabla de consulta</param>
        /// <param name="data">Lista de datos a insertar</param>
        /// <returns></returns>
        public string Insert(string table, object data)
        {
            var list = JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(data));
            return Insert(table, list);
        }

        /// <summary>
        /// Sentencia Insert
        /// </summary>
        /// <param name="table">Tabla de consulta</param>
        /// <param name="data">Lista de datos a insertar</param>
        /// <returns></returns>
        public string Insert(string table, Hashtable data)
        {
            return Insert(table, new List<Hashtable>
            {
                data
            });
        }

        /// <summary>
        /// Sentencia Insert
        /// </summary>
        /// <param name="sql">Código SQL para inserción</param>
        /// <returns></returns>
        public string InsertSql(string sql)
        {
            string connectionString = CreateConnectionString();
            SqlConnection databaseConnection = new SqlConnection(connectionString);

            try
            {
                databaseConnection = InitConnection(databaseConnection);
                var commandDatabase = new SqlCommand(sql, databaseConnection)
                {
                    CommandTimeout = 60
                };
                SqlDataReader myReader = commandDatabase.ExecuteReader();
            }
            catch (SqlException ex)
            {
                CloseConnection(databaseConnection);
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                CloseConnection(databaseConnection);
                throw new Exception(ex.Message);
            }
            finally
            {
                CloseConnection(databaseConnection);
            }
            return "Proceso Terminado";
        }

        /// <summary>
        /// Sentencia Update
        /// </summary>
        /// <param name="table">Tabla de consulta</param>
        /// <param name="data">Datos a actualizar</param>
        /// <param name="where">Where de sentencia</param>
        /// <returns></returns>
        public bool Update(string table, Hashtable data, string where = "")
        {
            bool response = false;
            string connectionString = CreateConnectionString();
            SqlConnection databaseConnection = new SqlConnection(connectionString);
            string sql = string.Empty;
            string values = string.Empty;

            data = SetStandarValues(data, 2);
            foreach (string k in data.Keys)
            {
                string value = GetType(data[k]);
                if (value.Trim() != "")
                {
                    values = values + k + " = " + value + ",";
                }
            }
            if (values != "")
            {
                values = values.TrimEnd(',');
                sql = sql + "UPDATE " + table + " SET " + values;

                if (where.Trim() != "")
                {
                    sql = sql + " WHERE " + where;
                }

                try
                {
                    databaseConnection = InitConnection(databaseConnection);
                    var commandDatabase = new SqlCommand(sql, databaseConnection)
                    {
                        CommandTimeout = 60
                    };
                    SqlDataReader myReader = commandDatabase.ExecuteReader();
                    response = true;
                }
                catch (SqlException ex)
                {
                    CloseConnection(databaseConnection);
                    throw new Exception(ex.Message);
                }
                catch (Exception ex)
                {
                    CloseConnection(databaseConnection);
                    throw new Exception(ex.Message);
                }
                finally
                {
                    CloseConnection(databaseConnection);
                }
            }
            return response;
        }

        /// <summary>
        /// Sentencia Update
        /// </summary>
        /// <param name="table">Tabla de consulta</param>
        /// <param name="data">Datos a actualizar</param>
        /// <param name="where">Where de sentencia</param>
        /// <returns></returns>
        public bool Update(string table, object data, string where = "")
        {
            var list = JsonConvert.DeserializeObject<Hashtable>(JsonConvert.SerializeObject(data));
            return Update(table, list, where);
        }

        /// <summary>
        /// Sentencia Update
        /// </summary>
        /// <param name="sql">Código SQL para Update</param>
        /// <returns></returns>
        public bool UpdateSql(string sql)
        {
            bool response = false;
            string connectionString = CreateConnectionString();
            SqlConnection databaseConnection = new SqlConnection(connectionString);
            try
            {
                databaseConnection = InitConnection(databaseConnection);
                var commandDatabase = new SqlCommand(sql, databaseConnection)
                {
                    CommandTimeout = 60
                };
                SqlDataReader myReader = commandDatabase.ExecuteReader();
                response = true;
            }
            catch (SqlException ex)
            {
                CloseConnection(databaseConnection);
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                CloseConnection(databaseConnection);
                throw new Exception(ex.Message);
            }
            finally
            {
                CloseConnection(databaseConnection);
            }
            return response;
        }

        /// <summary>
        /// Sentencia Delete
        /// </summary>
        /// <param name="table">Tabla de consulta</param>
        /// <param name="where">Where de sentencia</param>
        /// <returns></returns>
        public bool SoftDelete(string table, string where = "")
        {
            bool response = false;
            string connectionString = CreateConnectionString();
            SqlConnection databaseConnection = new SqlConnection(connectionString);

            var date = DateTime.Now;
            var currentDate = $"'{date:yyyy-MM-dd hh:mm:ss}'";
            string sql = $"UPDATE {table} SET deleted_at = {currentDate}";

            if (where.Trim() != "")
            {
                sql = sql + " WHERE " + where;
            }

            try
            {
                databaseConnection = InitConnection(databaseConnection);
                var commandDatabase = new SqlCommand(sql, databaseConnection)
                {
                    CommandTimeout = 60
                };
                SqlDataReader myReader = commandDatabase.ExecuteReader();
                response = true;
            }
            catch (SqlException ex)
            {
                CloseConnection(databaseConnection);
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                CloseConnection(databaseConnection);
                throw new Exception(ex.Message);
            }
            finally
            {
                CloseConnection(databaseConnection);
            }
            return response;
        }

        /// <summary>
        /// Sentencia Delete
        /// </summary>
        /// <param name="table">Tabla de consulta</param>
        /// <param name="where">Where de sentencia</param>
        /// <returns></returns>
        public bool Delete(string table, string where = "")
        {
            bool response = false;
            string connectionString = CreateConnectionString();
            SqlConnection databaseConnection = new SqlConnection(connectionString);

            string sql = string.Empty;
            sql = sql + "DELETE FROM " + table;

            if (where.Trim() != "")
            {
                sql = sql + " WHERE " + where;
            }

            try
            {
                databaseConnection = InitConnection(databaseConnection);
                var commandDatabase = new SqlCommand(sql, databaseConnection)
                {
                    CommandTimeout = 60
                };
                SqlDataReader myReader = commandDatabase.ExecuteReader();
                response = true;
            }
            catch (SqlException ex)
            {
                CloseConnection(databaseConnection);
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                CloseConnection(databaseConnection);
                throw new Exception(ex.Message);
            }
            finally
            {
                CloseConnection(databaseConnection);
            }
            return response;
        }

        /// <summary>
        /// Sentencia Delete
        /// </summary>
        /// <param name="sql">Código SQL para Delete</param>
        /// <returns></returns>
        public bool DeleteSql(string sql)
        {
            bool response = false;
            string connectionString = CreateConnectionString();
            SqlConnection databaseConnection = new SqlConnection(connectionString);

            try
            {
                databaseConnection = InitConnection(databaseConnection);
                var commandDatabase = new SqlCommand(sql, databaseConnection)
                {
                    CommandTimeout = 60
                };
                SqlDataReader myReader = commandDatabase.ExecuteReader();
                response = true;
            }
            catch (SqlException ex)
            {
                CloseConnection(databaseConnection);
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                CloseConnection(databaseConnection);
                throw new Exception(ex.Message);
            }
            finally
            {
                CloseConnection(databaseConnection);
            }
            return response;
        }

        /// <summary>
        /// Sentencia CALL
        /// </summary>
        /// <param name="method">Nombre del método a consultar</param>
        /// <param name="parameters">Parametros del método</param>
        /// <returns></returns>
        public string Call(string method, string parameters)
        {
            string connectionString = CreateConnectionString();
            SqlConnection databaseConnection = new SqlConnection(connectionString);
            ArrayList data = new ArrayList();
            string sql = "CALL {0} ({1})";
            sql = string.Format(sql, method, parameters);

            try
            {
                databaseConnection = InitConnection(databaseConnection);
                var commandDatabase = new SqlCommand(sql, databaseConnection)
                {
                    CommandTimeout = 60
                };
                var reader = commandDatabase.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Hashtable fields = new Hashtable();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            fields.Add(reader.GetName(i), GetTypeValue(reader.GetValue(i), reader.GetFieldType(i)));
                        }
                        data.Add(fields);
                    }
                }
                else
                {
                    Console.WriteLine("No se encontraron datos.");
                }

            }
            catch (SqlException ex)
            {
                CloseConnection(databaseConnection);
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                CloseConnection(databaseConnection);
                throw new Exception(ex.Message);
            }
            finally
            {
                CloseConnection(databaseConnection);
            }
            return JsonConvert.SerializeObject(data);
        }

        /// <summary>
        /// Sentencia CALL
        /// </summary>
        /// <param name="sql">Código SQL para sentencia CALL</param>
        /// <returns></returns>
        public string CallSql(string sql)
        {
            string connectionString = CreateConnectionString();
            SqlConnection databaseConnection = new SqlConnection(connectionString);

            ArrayList data = new ArrayList();
            if (!sql.ToUpper().StartsWith("CALL "))
            {
                sql = "CALL " + sql;
            }

            try
            {
                databaseConnection = InitConnection(databaseConnection);
                var commandDatabase = new SqlCommand(sql, databaseConnection)
                {
                    CommandTimeout = 60
                };
                var reader = commandDatabase.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Hashtable fields = new Hashtable();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            fields.Add(reader.GetName(i), GetTypeValue(reader.GetValue(i), reader.GetFieldType(i)));
                        }
                        data.Add(fields);
                    }
                }
                else
                {
                    Console.WriteLine("No se encontraron datos.");
                }

            }
            catch (SqlException ex)
            {
                CloseConnection(databaseConnection);
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                CloseConnection(databaseConnection);
                throw new Exception(ex.Message);
            }
            finally
            {
                CloseConnection(databaseConnection);
            }
            return JsonConvert.SerializeObject(data);
        }

        private Hashtable SetStandarValues(Hashtable data, int op = 1)
        {
            data.Remove("id"); ///// poner el campo autoincremntal
            return data;
        }

        private object GetTypeValue(object data, Type type)
        {
            string value = data != null ? data.ToString() : "";
            value = value.Trim();
            object result = null;
            switch (type.Name)
            {
                case "Boolean":
                    result = value != "" && Convert.ToBoolean(value);
                    break;
                case "SByte":
                    result = value != "" && Convert.ToInt32(value) == 1;
                    break;
                case "Int32":
                    result = value != "" ? Convert.ToInt32(value) : 0;
                    break;
                case "Decimal":
                    result = value != "" ? Convert.ToDouble(value) : 0;
                    break;
                case "Double":
                    result = value != "" ? Convert.ToDouble(value) : 0;
                    break;
                case "Int64":
                    result = value != "" ? Convert.ToInt32(value) : 0;
                    break;
                case "String":
                    result = value.ToString();
                    break;
                case "UInt64":
                    result = value != "" ? Convert.ToInt32(value) : 0;
                    break;
                case "DateTime":
                    result = value != "" ? Convert.ToDateTime(value) : new DateTime();
                    break;
            }
            return result;
        }

        private string GetType(object value)
        {
            string result = string.Empty;
            if (value != null)
            {
                switch (value.GetType().Name)
                {
                    case "Boolean":
                        result = Convert.ToBoolean(value) == true ? "1" : "0";
                        break;
                    case "Double":
                        nfi.NumberDecimalSeparator = ".";
                        result = Convert.ToDouble(value.ToString()).ToString(nfi);
                        break;
                    case "Int32":
                        result = value.ToString();
                        break;
                    case "Int64":
                        result = value.ToString();
                        break;
                    case "String":
                        result = "'" + value.ToString() + "'";
                        break;
                    case "DateTime":
                        var date = (DateTime)value;
                        result = $"'{date:yyyy-MM-dd hh:mm:ss}'";
                        break;
                }
            }
            return result;
        }
    }
}