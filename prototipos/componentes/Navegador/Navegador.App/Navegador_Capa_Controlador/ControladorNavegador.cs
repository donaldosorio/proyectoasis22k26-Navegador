using System.Data;
using Navegador.App.Navegador_Capa_Modelo;

namespace Navegador.App.Navegador_Capa_Controlador
{
    public class ControladorNavegador
    {
        private readonly Pagina pagina = new Pagina();

        public DataTable cargarTabla(string tabla)
        {
            return pagina.ObtenerTabla(tabla);
        }
    }
}