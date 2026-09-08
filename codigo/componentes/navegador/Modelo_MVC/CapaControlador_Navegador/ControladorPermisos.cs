using System.Collections.Generic;
using CapaModelo_Navegador;

namespace CapaControlador_Navegador
{
    public class ControladorPermisos
    {
        // Obtiene los permisos correspondientes al perfil.
        public List<Permiso> ObtenerPermisosPorPerfil(int idPerfil)
        {
            List<Permiso> permisos = new List<Permiso>();

            // Perfil 1: ejemplo de perfil con acceso completo
            if (idPerfil == 1)
            {
                permisos.Add(new Permiso
                {
                    IdModulo = 1,
                    NombreModulo = "Clientes"
                });

                permisos.Add(new Permiso
                {
                    IdModulo = 2,
                    NombreModulo = "Productos"
                });

                permisos.Add(new Permiso
                {
                    IdModulo = 3,
                    NombreModulo = "Ventas"
                });
            }

            // Perfil 2: ejemplo de perfil con acceso limitado
            else if (idPerfil == 2)
            {
                permisos.Add(new Permiso
                {
                    IdModulo = 1,
                    NombreModulo = "Clientes"
                });
            }

            return permisos;
        }

        // Construye el mapa de permisos que tendrá
         public List<Permiso> ConstruirMapaPermisos(int idPerfil)
        {
            return ObtenerPermisosPorPerfil(idPerfil);
        }


        // Comprueba si un permiso se encuentra
 
        public bool TienePermiso(
            List<Permiso> permisos,
            string nombreModulo)
        {
            foreach (Permiso permiso in permisos)
            {
                if (permiso.NombreModulo == nombreModulo)
                {
                    return true;
                }
            }

            return false;
        }
    }
}