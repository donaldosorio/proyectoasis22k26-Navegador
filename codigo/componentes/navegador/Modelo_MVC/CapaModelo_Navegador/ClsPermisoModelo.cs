using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Odbc;

// ------------------------------------------------------------------
// Hecho por: Natali Sofía Montenegro Portillo - 0901-23-10017
// Clase encargada de consultar y validar los permisos del usuario
// ------------------------------------------------------------------
namespace CapaModelo_Navegador
{
    public class ClsPermisoModelo
    {
        public bool ValidarPermiso(string UsuarioActual,string CodigoModulo){
            bool TienePermiso = true;

            // AQUÍ irá la consulta real a la tabla
            // de permisos de la base de datos.

            return TienePermiso;
        }
    }
}
