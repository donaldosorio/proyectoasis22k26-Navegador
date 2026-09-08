using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaModelo_Navegador
{
    public class Sentencias
    {
        conexionBD conn = new conexionBD();

        public OdbcDataAdapter llenarTbl(string nombreTabla)
        {
            string sSQL = "SELECT * FROM " + nombreTabla;
            OdbcConnection conexion = conn.conexion();
            OdbcDataAdapter daSentencias = new OdbcDataAdapter(sSQL, conexion);
            return daSentencias;
        }

        public DataTable ConsultarEmpleados()
        {
            string sSQL = "SELECT * FROM tbl_empleados";
            OdbcConnection conexion = conn.conexion();
            DataTable dtEmpleados = new DataTable();

            try
            {
                using (OdbcDataAdapter da = new OdbcDataAdapter(sSQL, conexion))
                {
                    da.Fill(dtEmpleados);
                }
            }
            finally
            {
                conn.desconexion(conexion);
            }

            return dtEmpleados;
        }
        // Dentro de la clase Sentencias en sentencias.cs
        public bool ExisteAplicacion(int idAplicacion)
        {
            // TODO: Consulta SQL a la BD cuando esté lista
            return idAplicacion > 0;
        }

        public bool ExisteModulo(int idModulo)
        {
            // TODO: Consulta SQL a la BD cuando esté lista
            return idModulo > 0;
        }

        public bool GuardarUsuarioPermisoBD(int idUsuario, int idAplicacion, int idModulo, int idPermiso)
        {
            // TODO: INSERT SQL a la BD cuando esté lista
            return true;
        }

        public List<string> ObtenerColumnas(
            string nombreTabla)
        {
            List<string> columnas = new List<string>();
            OdbcConnection conexion = conn.conexion();

            try
            {
                DataTable dtColumnas = conexion.GetSchema(
                    "Columns",
                    new string[] { null, null, nombreTabla, null }
                );

                foreach (DataRow fila in dtColumnas.Rows)
                {
                    string nombreColumna = fila["COLUMN_NAME"].ToString();

                    if (!columnas.Contains(nombreColumna))
                    {
                        columnas.Add(nombreColumna);
                    }
                }
            }
            finally
            {
                conn.desconexion(conexion);
            }

            return columnas;
        }

        public bool ExisteLlavePrimaria(string nombreTabla, string[] camposPK, string[] valoresPK)
        {
            if (camposPK == null || camposPK.Length == 0) return false;
            if (valoresPK == null || valoresPK.Length != camposPK.Length) return false;

            string condiciones = "";
            for (int i = 0; i < camposPK.Length; i++)
            {
                if (i > 0) condiciones += " AND ";
                condiciones += camposPK[i] + " = ?";
            }

            string sSQL = "SELECT COUNT(*) FROM " + nombreTabla + " WHERE " + condiciones;
            OdbcConnection conexion = conn.conexion();

            try
            {
                using (OdbcCommand comando = new OdbcCommand(sSQL, conexion))
                {
                    for (int i = 0; i < valoresPK.Length; i++)
                    {
                        comando.Parameters.AddWithValue("@p" + i, valoresPK[i]);
                    }

                    int cantidad = Convert.ToInt32(comando.ExecuteScalar());
                    return cantidad > 0;
                }
            }
            finally
            {
                conn.desconexion(conexion);
            }
        }

        public bool ExisteValorCampo(string nombreTabla, string nombreCampo, string valor)
        {
            string sSQL = "SELECT COUNT(*) FROM " + nombreTabla + " WHERE " + nombreCampo + " = ?";
            OdbcConnection conexion = conn.conexion();

            try
            {
                using (OdbcCommand comando = new OdbcCommand(sSQL, conexion))
                {
                    comando.Parameters.AddWithValue("@valor", valor);
                    int cantidad = Convert.ToInt32(comando.ExecuteScalar());
                    return cantidad > 0;
                }
            }
            finally
            {
                conn.desconexion(conexion);
            }
        }

        public bool InsertarRegistro(string nombreTabla, Dictionary<string, string> datos)
        {
            if (datos == null || datos.Count == 0) return false;

            string columnas = "";
            string valores = "";
            int contador = 0;

            foreach (KeyValuePair<string, string> dato in datos)
            {
                if (contador > 0)
                {
                    columnas += ", ";
                    valores += ", ";
                }
                columnas += dato.Key;
                valores += "?";
                contador++;
            }

            string sSQL = "INSERT INTO " + nombreTabla + " (" + columnas + ") VALUES (" + valores + ")";
            OdbcConnection conexion = conn.conexion();

            try
            {
                using (OdbcCommand comando = new OdbcCommand(sSQL, conexion))
                {
                    int posicion = 0;
                    foreach (KeyValuePair<string, string> dato in datos)
                    {
                        comando.Parameters.AddWithValue("@p" + posicion, dato.Value);
                        posicion++;
                    }

                    int resultado = comando.ExecuteNonQuery();
                    return resultado > 0;
                }
            }
            finally
            {
                conn.desconexion(conexion);
            }
        }

        public DataTable ObtenerEsquemaTabla(string nombreTabla)
        {
            string sSQL = @"
                SELECT 
                    COLUMN_NAME, 
                    DATA_TYPE, 
                    CHARACTER_MAXIMUM_LENGTH, 
                    IS_NULLABLE 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = ? AND TABLE_SCHEMA = DATABASE() 
                ORDER BY ORDINAL_POSITION";

            OdbcConnection conexion = conn.conexion();
            DataTable dtEsquema = new DataTable();

            try
            {
                using (OdbcCommand comando = new OdbcCommand(sSQL, conexion))
                {
                    comando.Parameters.AddWithValue("?", nombreTabla);
                    using (OdbcDataAdapter da = new OdbcDataAdapter(comando))
                    {
                        da.Fill(dtEsquema);
                    }
                }
            }
            finally
            {
                conn.desconexion(conexion);
            }

            return dtEsquema;
        }

        public void ejecutarSql(string sql)
        {
            OdbcConnection conexion = conn.conexion();
            try
            {
                using (OdbcCommand cmd = new OdbcCommand(sql, conexion))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                conn.desconexion(conexion);
            }
        }
        // Validacion de Usario Jose Torres
        public DataTable ValidarUsuario(string usuario, string clave)
        {
            string sSQL = "SELECT id_usuario, nombre_usuario, id_rol FROM tbl_usuarios " +
                           "WHERE nombre_usuario = ? AND contrasena = ? AND estado_usuario = 1";

            OdbcConnection conexion = conn.conexion();
            DataTable dt = new DataTable();

            try
            {
                using (OdbcCommand comando = new OdbcCommand(sSQL, conexion))
                {
                    comando.Parameters.AddWithValue("@usuario", usuario);
                    comando.Parameters.AddWithValue("@clave", clave);

                    using (OdbcDataAdapter da = new OdbcDataAdapter(comando))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en la validación: " + ex.Message);
            }
            finally
            {
                conn.desconexion(conexion); // Solo la desconexión dentro del finally
            }

            return dt; // El return va afuera
        }
        public void guardarDatos(string query)
        {
            try
            {
                using (OdbcConnection conexion = conn.conexion())
                {
                    using (OdbcCommand cmd = new OdbcCommand(query, conexion))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al ejecutar la sentencia en la base de datos: " + ex.Message, ex);
            }
        }

        public OdbcDataAdapter filtrarTbl(string nombreTabla, string columna, string valor)
        {
            string sSQL =
                "SELECT * FROM " +
                nombreTabla +
                " WHERE " +
                columna +
                " LIKE ?";

            OdbcConnection conexion =
                conn.conexion();

            OdbcCommand comando = new OdbcCommand(sSQL, conexion);
            comando.Parameters.AddWithValue("@valor", "%" + valor + "%");

            OdbcDataAdapter daSentencias =
                new OdbcDataAdapter(comando);

            return daSentencias;

         
        }


    }
}