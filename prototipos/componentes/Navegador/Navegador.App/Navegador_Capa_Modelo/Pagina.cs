using System.Data;

namespace Navegador.App.Navegador_Capa_Modelo
{
    public class Pagina
    {
        public DataTable ObtenerTabla(string nombreTabla)
        {
            DataTable dt = new DataTable();

            // Definir columnas de prueba
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Nombre", typeof(string));
            dt.Columns.Add("Estado", typeof(string));

            // Agregar registros simulados para probar la navegación
            dt.Rows.Add(1, "Registro Ejemplo 1", "Activo");
            dt.Rows.Add(2, "Registro Ejemplo 2", "Inactivo");
            dt.Rows.Add(3, "Registro Ejemplo 3", "Activo");

            return dt;
        }
    }
}
