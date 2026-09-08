using System.Collections.Generic;
using CapaModelo_Navegador;

namespace CapaVista_Navegador
{
    public static class SesionUsuario
    {
        public static int IdUsuario { get; set; }

        public static int IdPerfil { get; set; }

        public static List<Permiso> Permisos { get; set; }
            = new List<Permiso>();
    }
}