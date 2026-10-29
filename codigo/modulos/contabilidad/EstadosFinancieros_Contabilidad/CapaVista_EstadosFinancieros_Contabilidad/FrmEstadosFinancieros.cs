/* Área: Contabilidad. Autor: Gabriel André Guillén Pocón. Carné: 0901-23-1998.
 * Fecha: 14/10/2026. Propósito: vista previa del balance general, estado de resultados y flujo de caja por nivel de detalle. */
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
// Inicio de cambio: Mario Taracena - 0901-23-9335 - 27/10/2026
using Diseno = CapaVista_Contabilidad.Componentes.ClsDisenoContabilidad;
// Fin de cambio: Mario Taracena - 0901-23-9335 - 27/10/2026
using CapaControlador_Contabilidad;
// Inicio de cambio: Mario Taracena - 0901-23-9335 - 27/10/2026
using CapaVista_Contabilidad.Componentes;
// Fin de cambio: Mario Taracena - 0901-23-9335 - 27/10/2026

// Inicio de cambio: Mario Taracena - 0901-23-9335 - 27/10/2026
namespace CapaVista_Contabilidad
// Fin de cambio: Mario Taracena - 0901-23-9335 - 27/10/2026
{
    public partial class FrmEstadosFinancieros : Form
    {
        private readonly ClsControladorEstadosFinancierosContabilidad _Controlador = new ClsControladorEstadosFinancierosContabilidad();
        // Inicio de cambio: Gabriel André Guillén Pocón - 0901-23-1998 - 28/10/2026
        private ClsModeloEstadoFinancieroContabilidad _Estado;
        private DataTable _Vista;
        private string _Titulo = string.Empty;
        private string _Periodo = string.Empty;
        private Font _Negrita;
        // Fin de cambio: Gabriel André Guillén Pocón - 0901-23-1998 - 28/10/2026
        private readonly ClsValidacionContabilidad _Validacion = new ClsValidacionContabilidad();

        // Inicio de cambio: Mario Taracena - 0901-23-9335 - 27/10/2026
        public FrmEstadosFinancieros() : this(ClsModeloPermisosPantallaContabilidad.Total)
        // Fin de cambio: Mario Taracena - 0901-23-9335 - 27/10/2026
        {
        }

        // Inicio de cambio: Mario Taracena - 0901-23-9335 - 27/10/2026
        public FrmEstadosFinancieros(ClsModeloPermisosPantallaContabilidad Permisos)
        // Fin de cambio: Mario Taracena - 0901-23-9335 - 27/10/2026
        {
            InitializeComponent();
            ContabilidadMetAplicarPermisos(Permisos);
            ClsPresentacionContabilidad.ContabilidadMetPrepararVentana(this, "Estados financieros", new Size(1180, 688));
            _Validacion.ContabilidadMetObligatorio(ContabilidadDtpDesde);
            _Validacion.ContabilidadMetObligatorio(ContabilidadDtpHasta);
            // Inicio de cambio: Gabriel André Guillén Pocón - 0901-23-1998 - 28/10/2026
            Disposed += (Remitente, Argumentos) =>
            {
                _Validacion.Dispose();
                if (_Negrita != null) _Negrita.Dispose();
                if (_Vista != null) _Vista.Dispose();
                if (_Estado != null && _Estado.Filas != null) _Estado.Filas.Dispose();
            };
            DateTime Mes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            ContabilidadDtpDesde.Value = new DateTime(Mes.Year, 1, 1);
            ContabilidadDtpHasta.Value = Mes;
            ContabilidadCboInforme.DataSource = _Controlador.ContabilidadFuncTiposEstado();
            ContabilidadCboNivel.DataSource = _Controlador.ContabilidadFuncNiveles();
            ContabilidadCboNivel.SelectedIndex = ContabilidadCboNivel.Items.Count - 1;
            ContabilidadDtpDesde.Enabled = false;
            ContabilidadCboInforme.SelectedIndexChanged += (Remitente, Argumentos) =>
            {
                ContabilidadDtpDesde.Enabled = Convert.ToString(ContabilidadCboInforme.SelectedItem) != ClsControladorEstadosFinancierosContabilidad.BalanceGeneral;
                ContabilidadMetInvalidar();
            };
            ContabilidadCboNivel.SelectedIndexChanged += (Remitente, Argumentos) => ContabilidadMetMostrar();
            ContabilidadDgvResultado.CellFormatting += ContabilidadMetFormatoFila;
            ContabilidadBtnReporte.Click += ContabilidadMetReporte;
            // Fin de cambio: Gabriel André Guillén Pocón - 0901-23-1998 - 28/10/2026
            ContabilidadBtnConsultar.Click += (Remitente, Argumentos) => ContabilidadMetConsultar();
            ContabilidadDtpDesde.ValueChanged += (Remitente, Argumentos) => ContabilidadMetInvalidar();
            ContabilidadDtpHasta.ValueChanged += (Remitente, Argumentos) => ContabilidadMetInvalidar();
            ContabilidadMetConsultar();
        }

        // Inicio de cambio: Mario Taracena - 0901-23-9335 - 27/10/2026
        private void ContabilidadMetAplicarPermisos(ClsModeloPermisosPantallaContabilidad Permisos)
        // Fin de cambio: Mario Taracena - 0901-23-9335 - 27/10/2026
        {
        }

        private void ContabilidadMetInvalidar()
        {
            // Inicio de cambio: Gabriel André Guillén Pocón - 0901-23-1998 - 28/10/2026
            ContabilidadDgvResultado.DataSource = null;
            if (_Vista != null) { _Vista.Dispose(); _Vista = null; }
            if (_Estado != null && _Estado.Filas != null) _Estado.Filas.Dispose();
            _Estado = null;
            ContabilidadLblPeriodo.Text = "Filtros modificados · Pulse Vista previa para actualizar el estado financiero.";
            // Fin de cambio: Gabriel André Guillén Pocón - 0901-23-1998 - 28/10/2026
            ContabilidadLblEstado.Text = string.Empty;
            ContabilidadLblResumen.Text = string.Empty;
        }

        // Inicio de cambio: Gabriel André Guillén Pocón - 0901-23-1998 - 28/10/2026
        private void ContabilidadMetFormatoFila(object Remitente, DataGridViewCellFormattingEventArgs Argumentos)
        {
            DataRowView Fila = ContabilidadDgvResultado.Rows[Argumentos.RowIndex].DataBoundItem as DataRowView;
            if (Fila == null || ((int)Fila["Nivel"] != 0 && !(bool)Fila["EsTotal"])) return;
            Argumentos.CellStyle.Font = _Negrita ?? (_Negrita = new Font(ContabilidadDgvResultado.Font, FontStyle.Bold));
        }

        private void ContabilidadMetMostrar()
        {
            if (_Estado == null) return;
            DataTable Anterior = _Vista;
            _Vista = _Controlador.ContabilidadFuncFiltrarNivel(_Estado.Filas, ContabilidadCboNivel.SelectedIndex + 1);
            ContabilidadDgvResultado.DataSource = _Vista;
            if (Anterior != null) Anterior.Dispose();
        }

        private void ContabilidadMetReporte(object Remitente, EventArgs Argumentos)
        {
            if (_Estado == null) ContabilidadMetConsultar();
            if (_Estado == null || _Vista == null) return;
            using (FrmReporteEstadoFinanciero Reporte = new FrmReporteEstadoFinanciero(_Vista, _Titulo, _Periodo,
                Convert.ToString(ContabilidadCboNivel.SelectedItem)))
                Reporte.ShowDialog(this);
        }

        private void ContabilidadMetConsultar()
        {
            string Tipo = Convert.ToString(ContabilidadCboInforme.SelectedItem);
            bool EsBalance = Tipo == ClsControladorEstadosFinancierosContabilidad.BalanceGeneral;
            DateTime Desde = new DateTime(ContabilidadDtpDesde.Value.Year, ContabilidadDtpDesde.Value.Month, 1);
            DateTime Hasta = new DateTime(ContabilidadDtpHasta.Value.Year, ContabilidadDtpHasta.Value.Month, 1);
            string ErrorFechas = EsBalance ? string.Empty : _Controlador.ContabilidadFuncErrorFechas(Desde, Hasta);
            if (!_Validacion.ContabilidadFuncValidar(ContabilidadDtpHasta, ErrorFechas))
            {
                ContabilidadMetInvalidar();
                ContabilidadLblEstado.Text = ErrorFechas;
                return;
            }
            ClsModeloEstadoFinancieroContabilidad Estado;
            try
            {
                Estado = _Controlador.ContabilidadFuncGenerar(Tipo, Desde, Hasta);
            }
            catch (Exception Excepcion)
            {
                ContabilidadMetInvalidar();
                ClsDialogosContabilidad.ContabilidadMetError(this, "No se pudo generar el estado financiero: " + Excepcion.Message);
                return;
            }
            ContabilidadMetInvalidar();
            _Estado = Estado;
            _Titulo = Tipo;
            DateTime FinMes = Hasta.AddMonths(1).AddDays(-1);
            _Periodo = EsBalance ? "Al " + FinMes.ToString("dd/MM/yyyy") : "Del " + Desde.ToString("dd/MM/yyyy") + " al " + FinMes.ToString("dd/MM/yyyy");
            ContabilidadLblPeriodo.Text = _Titulo + " · " + _Periodo;
            ContabilidadLblEstado.ForeColor = Estado.Cuadra ? Color.FromArgb(35, 50, 61) : Color.FromArgb(197, 143, 62);
            if (EsBalance)
            {
                ContabilidadLblEstado.Text = Estado.Cuadra ? string.Empty
                    : "Descuadre: el activo no es igual al pasivo más el capital y el resultado. Revise las pólizas aplicadas.";
                ContabilidadLblResumen.Text = "Activo: " + Diseno.ContabilidadFuncMoneda(Estado.Activo) + "     Pasivo: " + Diseno.ContabilidadFuncMoneda(Estado.Pasivo)
                    + "     Capital: " + Diseno.ContabilidadFuncMoneda(Estado.Capital) + "     Resultado: " + Diseno.ContabilidadFuncMoneda(Estado.Resultado);
            }
            else if (Tipo == ClsControladorEstadosFinancierosContabilidad.FlujoCaja)
            {
                ContabilidadLblEstado.Text = Estado.Cuadra ? string.Empty
                    : "Descuadre: el flujo neto no explica la variación de Caja y Bancos. Revise que el período tenga los saldos actualizados.";
                ContabilidadLblResumen.Text = "Operación: " + Diseno.ContabilidadFuncMoneda(Estado.Operacion) + "     Inversión: " + Diseno.ContabilidadFuncMoneda(Estado.Inversion)
                    + "     Financiamiento: " + Diseno.ContabilidadFuncMoneda(Estado.Financiamiento) + "     Efectivo final: " + Diseno.ContabilidadFuncMoneda(Estado.EfectivoFinal);
            }
            else
            {
                ContabilidadLblEstado.Text = Estado.Ingresos == 0m && Estado.Costos == 0m && Estado.Gastos == 0m ? "Sin movimientos en el rango seleccionado."
                    : string.Empty;
                ContabilidadLblResumen.Text = "Ingresos: " + Diseno.ContabilidadFuncMoneda(Estado.Ingresos) + "     Costos: " + Diseno.ContabilidadFuncMoneda(Estado.Costos)
                    + "     Gastos: " + Diseno.ContabilidadFuncMoneda(Estado.Gastos) + "     Resultado: " + Diseno.ContabilidadFuncMoneda(Estado.Resultado);
            }
            ContabilidadMetMostrar();
        }
        // Fin de cambio: Gabriel André Guillén Pocón - 0901-23-1998 - 28/10/2026
    }
}
