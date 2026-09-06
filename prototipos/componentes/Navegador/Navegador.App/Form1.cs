using System;
using System.Data;
using System.Windows.Forms;
using Navegador.App.Navegador_Capa_Controlador;

namespace Navegador.App.Navegador_Capa_Vista
{
    public partial class Form1 : Form
    {
        private readonly ControladorNavegador controlador = new ControladorNavegador();
        private readonly BindingSource miBindingSource = new BindingSource();
        private readonly string tablaActual = "tu_tabla_de_prueba";

        public Form1()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                DataTable dt = controlador.cargarTabla(tablaActual);
                miBindingSource.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos: " + ex.Message);
            }
        }

        private void btn_anterior_Click(object sender, EventArgs e)
        {
            if (miBindingSource.Position > 0)
            {
                miBindingSource.MovePrevious();
            }
        }

        private void btn_siguiente_Click(object sender, EventArgs e)
        {
            if (miBindingSource.Position < miBindingSource.Count - 1)
            {
                miBindingSource.MoveNext();
            }
        }

        private void btn_fin_Click(object sender, EventArgs e)
        {
            miBindingSource.MoveLast();
        }

        private void btn_refrescar_Click(object sender, EventArgs e)
        {
            CargarDatos();
        }
    }
}