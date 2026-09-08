using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaModelo_Navegador;


// ------------------------------------------------------------------
// Hecho por: Natali Sofía Montenegro Portillo - 0901-23-10017
// Clase en el controlador encargado de gestionar la validación de acceso entre la capas (Vista y Modelo)
// ------------------------------------------------------------------
namespace CapaControlador_Navegador
{
    public class ClsPermisoControlador
    {
        private readonly ClsPermisoModelo _PermisoModelo;

        public ClsPermisoControlador()
        {
            _PermisoModelo = new ClsPermisoModelo();
        }

        public bool ValidarAcceso(string UsuarioActual,string CodigoModulo)
        {
            if (string.IsNullOrWhiteSpace(UsuarioActual))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(CodigoModulo))
            {
                return false;
            }

            return _PermisoModelo.ValidarPermiso(UsuarioActual,CodigoModulo
                );
        }
    }
}
