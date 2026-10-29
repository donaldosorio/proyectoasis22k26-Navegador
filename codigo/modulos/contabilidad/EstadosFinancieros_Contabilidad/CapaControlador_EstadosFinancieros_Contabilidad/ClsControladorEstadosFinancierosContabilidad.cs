/*
 * ==================================================================
 * Área : Contabilidad
 * Autor : Gabriel André Guillén Pocón
 * Carné : 0901-23-1998
 * Fecha : 17/10/2026
 * ==================================================================
 * Propósito :
 *  Reglas de los estados financieros: arma el árbol del catálogo con
 *  la cuenta padre y totaliza cada cuenta de forma recursiva para el
 *  balance general, el estado de resultados y el flujo de caja.
 * Reglas especificas:
 *  Solo las cuentas de detalle aportan monto propio; cada cuenta mayor
 *  es la suma recursiva de sus hijas. Balance y resultados llevan los
 *  totales con signo deudor, así las cuentas complementarias restan
 *  solas. El flujo de caja usa el método directo: entradas y salidas de
 *  Caja y Bancos clasificadas por la cuenta contrapartida. El nivel de
 *  detalle filtra la profundidad del árbol sin quitar los totales.
 * ===================================================================
*/

using System;
using System.Collections.Generic;
using System.Data;
// Inicio de cambio: Gabriel André Guillén Pocón - 0901-23-1998 - 28/10/2026
using CapaModelo_Contabilidad.Entidades;
// Fin de cambio: Gabriel André Guillén Pocón - 0901-23-1998 - 28/10/2026
using CapaModelo_Contabilidad.Repositorios;

// Inicio de cambio: Mario Taracena - 0901-23-9335 - 27/10/2026
namespace CapaControlador_Contabilidad
// Fin de cambio: Mario Taracena - 0901-23-9335 - 27/10/2026
{
    public class ClsControladorEstadosFinancierosContabilidad
    {
        // Inicio de cambio: Gabriel André Guillén Pocón - 0901-23-1998 - 28/10/2026
        public const string BalanceGeneral = "Balance general";
        public const string EstadoResultados = "Estado de resultados";
        public const string FlujoCaja = "Flujo de caja";

        private readonly ClsRepositorioEstadosFinancierosContabilidad _Repositorio = new ClsRepositorioEstadosFinancierosContabilidad();

        private sealed class ClsNodoCuentaContabilidad
        {
            public ClsCuentaSaldoContabilidad Cuenta;
            public readonly List<ClsNodoCuentaContabilidad> Hijos = new List<ClsNodoCuentaContabilidad>();
            public decimal Propio;
            public decimal Total;
            public bool ConSaldo;
        }
        // Fin de cambio: Gabriel André Guillén Pocón - 0901-23-1998 - 28/10/2026

        public string ContabilidadFuncErrorFechas(DateTime Inicio, DateTime Fin)
        {
            return ClsValidacionDatosContabilidad.ContabilidadFuncErrorRangoFechas(Inicio, Fin);
        }

        // Inicio de cambio: Gabriel André Guillén Pocón - 0901-23-1998 - 28/10/2026
        public string[] ContabilidadFuncTiposEstado()
        {
            return new[] { BalanceGeneral, EstadoResultados, FlujoCaja };
        }

        public string[] ContabilidadFuncNiveles()
        {
            return new[] { "Nivel 1 - Grupos", "Nivel 2 - Subgrupos", "Nivel 3 - Cuentas mayores", "Nivel 4 - Detalle completo" };
        }

        public ClsModeloEstadoFinancieroContabilidad ContabilidadFuncGenerar(string Tipo, DateTime Desde, DateTime Hasta)
        {
            if (Tipo == BalanceGeneral) return ContabilidadFuncBalanceGeneral(Hasta);
            if (Tipo == FlujoCaja) return ContabilidadFuncFlujoCaja(Desde, Hasta);
            return ContabilidadFuncEstadoResultados(Desde, Hasta);
        }

        public DataTable ContabilidadFuncFiltrarNivel(DataTable Filas, int Nivel)
        {
            DataView Vista = new DataView(Filas) { RowFilter = "EsTotal = true OR Nivel < " + Math.Max(1, Math.Min(4, Nivel)) };
            return Vista.ToTable();
        }

        public ClsModeloEstadoFinancieroContabilidad ContabilidadFuncBalanceGeneral(DateTime Periodo)
        {
            string Mes = Periodo.ToString("yyyy-MM");
            Dictionary<string, ClsNodoCuentaContabilidad> Arbol = ContabilidadFuncArbol(_Repositorio.ContabilidadFuncObtenerCuentas(Mes, Mes),
                Cuenta => Cuenta.Tipo == "DEUDORA" ? Cuenta.SaldoFinal : -Cuenta.SaldoFinal);
            ClsModeloEstadoFinancieroContabilidad Estado = new ClsModeloEstadoFinancieroContabilidad { Filas = ContabilidadFuncTabla() };
            Estado.Activo = ContabilidadFuncAgregarRubro(Estado.Filas, Arbol, "1", "ACTIVO", true);
            Estado.Pasivo = ContabilidadFuncAgregarRubro(Estado.Filas, Arbol, "2", "PASIVO", true);
            Estado.Capital = ContabilidadFuncAgregarRubro(Estado.Filas, Arbol, "3", "PATRIMONIO", true);
            Estado.Ingresos = ContabilidadFuncImporte(Arbol, "4");
            Estado.Gastos = ContabilidadFuncImporte(Arbol, "5");
            Estado.Resultado = Estado.Ingresos - Estado.Gastos;
            ContabilidadMetAgregarTotal(Estado.Filas, "PATRIMONIO", "Resultado del ejercicio", Estado.Resultado);
            ContabilidadMetAgregarTotal(Estado.Filas, "TOTAL", "Total pasivo, capital y resultado", Estado.Pasivo + Estado.Capital + Estado.Resultado);
            Estado.Cuadra = Estado.Activo == Estado.Pasivo + Estado.Capital + Estado.Resultado;
            return Estado;
        }

        public ClsModeloEstadoFinancieroContabilidad ContabilidadFuncEstadoResultados(DateTime Desde, DateTime Hasta)
        {
            if (Hasta.Date < Desde.Date) throw new ArgumentException("El mes final debe ser igual o posterior al inicial.");
            Dictionary<string, ClsNodoCuentaContabilidad> Arbol = ContabilidadFuncArbol(
                _Repositorio.ContabilidadFuncObtenerCuentas(Desde.ToString("yyyy-MM"), Hasta.ToString("yyyy-MM")), Cuenta => Cuenta.Cargo - Cuenta.Abono);
            ClsModeloEstadoFinancieroContabilidad Estado = new ClsModeloEstadoFinancieroContabilidad { Filas = ContabilidadFuncTabla() };
            Estado.Ingresos = ContabilidadFuncAgregarRubro(Estado.Filas, Arbol, "4", "INGRESOS", true);
            decimal CostosYGastos = ContabilidadFuncAgregarRubro(Estado.Filas, Arbol, "5", "COSTOS Y GASTOS", true);
            Estado.Costos = ContabilidadFuncImporte(Arbol, "5.01") + ContabilidadFuncImporte(Arbol, "5.02");
            Estado.Gastos = CostosYGastos - Estado.Costos;
            Estado.Resultado = Estado.Ingresos - CostosYGastos;
            ContabilidadMetAgregarTotal(Estado.Filas, "TOTAL", Estado.Resultado >= 0m ? "Utilidad neta" : "Pérdida neta", Estado.Resultado);
            Estado.Cuadra = true;
            return Estado;
        }

        public ClsModeloEstadoFinancieroContabilidad ContabilidadFuncFlujoCaja(DateTime Desde, DateTime Hasta)
        {
            if (Hasta.Date < Desde.Date) throw new ArgumentException("El mes final debe ser igual o posterior al inicial.");
            DateTime Inicio = new DateTime(Desde.Year, Desde.Month, 1);
            DateTime Fin = new DateTime(Hasta.Year, Hasta.Month, 1).AddMonths(1).AddDays(-1);
            List<ClsCuentaSaldoContabilidad> Cuentas = _Repositorio.ContabilidadFuncObtenerCuentas(Inicio.ToString("yyyy-MM"), Fin.ToString("yyyy-MM"));
            Dictionary<int, decimal> Flujo = _Repositorio.ContabilidadFuncObtenerFlujoEfectivo(Inicio, Fin);
            ClsModeloEstadoFinancieroContabilidad Estado = new ClsModeloEstadoFinancieroContabilidad { Filas = ContabilidadFuncTabla() };
            Estado.EfectivoInicial = _Repositorio.ContabilidadFuncObtenerEfectivo(Inicio.AddMonths(-1).ToString("yyyy-MM"));
            Estado.EfectivoFinal = _Repositorio.ContabilidadFuncObtenerEfectivo(Fin.ToString("yyyy-MM"));
            ContabilidadMetAgregarTotal(Estado.Filas, "EFECTIVO", "Efectivo al inicio del período", Estado.EfectivoInicial);
            Estado.Operacion = ContabilidadFuncAgregarActividad(Estado.Filas, Cuentas, Flujo, "OPERACIÓN", "Flujo neto de actividades de operación");
            Estado.Inversion = ContabilidadFuncAgregarActividad(Estado.Filas, Cuentas, Flujo, "INVERSIÓN", "Flujo neto de actividades de inversión");
            Estado.Financiamiento = ContabilidadFuncAgregarActividad(Estado.Filas, Cuentas, Flujo, "FINANCIAMIENTO", "Flujo neto de actividades de financiamiento");
            decimal Neto = Estado.Operacion + Estado.Inversion + Estado.Financiamiento;
            ContabilidadMetAgregarTotal(Estado.Filas, "EFECTIVO", "Aumento (disminución) neto de efectivo", Neto);
            ContabilidadMetAgregarTotal(Estado.Filas, "EFECTIVO", "Efectivo al final del período", Estado.EfectivoFinal);
            Estado.Cuadra = Estado.EfectivoInicial + Neto == Estado.EfectivoFinal;
            return Estado;
        }

        public static string ContabilidadFuncActividad(string Codigo)
        {
            if (Codigo.StartsWith("1.02") || Codigo.StartsWith("1.03") || Codigo.StartsWith("1.04") || Codigo.StartsWith("1.01.03")) return "INVERSIÓN";
            if (Codigo.StartsWith("2.02") || Codigo.StartsWith("3")) return "FINANCIAMIENTO";
            return "OPERACIÓN";
        }

        private static decimal ContabilidadFuncAgregarActividad(DataTable Filas, List<ClsCuentaSaldoContabilidad> Cuentas,
            Dictionary<int, decimal> Flujo, string Actividad, string Titulo)
        {
            Dictionary<string, ClsNodoCuentaContabilidad> Arbol = ContabilidadFuncArbol(Cuentas, Cuenta =>
                Flujo.ContainsKey(Cuenta.IdCuenta) && ContabilidadFuncActividad(Cuenta.Codigo) == Actividad ? Flujo[Cuenta.IdCuenta] : 0m);
            decimal Total = 0m;
            foreach (string Raiz in new[] { "1", "2", "3", "4", "5", "6", "7" })
                if (Arbol.ContainsKey(Raiz)) Total += Arbol[Raiz].Total;
            ContabilidadMetAgregarTotal(Filas, Actividad, "Actividades de " + Actividad.ToLower(), null);
            foreach (string Raiz in new[] { "1", "2", "3", "4", "5", "6", "7" })
                if (Arbol.ContainsKey(Raiz) && Arbol[Raiz].ConSaldo) ContabilidadMetAgregarFilas(Filas, Actividad, Arbol[Raiz], 0, false);
            ContabilidadMetAgregarTotal(Filas, Actividad, Titulo, Total);
            return Total;
        }

        private static Dictionary<string, ClsNodoCuentaContabilidad> ContabilidadFuncArbol(List<ClsCuentaSaldoContabilidad> Cuentas,
            Func<ClsCuentaSaldoContabilidad, decimal> MontoPropio)
        {
            Dictionary<int, ClsNodoCuentaContabilidad> PorId = new Dictionary<int, ClsNodoCuentaContabilidad>();
            Dictionary<string, ClsNodoCuentaContabilidad> PorCodigo = new Dictionary<string, ClsNodoCuentaContabilidad>();
            foreach (ClsCuentaSaldoContabilidad Cuenta in Cuentas)
            {
                ClsNodoCuentaContabilidad Nodo = new ClsNodoCuentaContabilidad { Cuenta = Cuenta };
                if (Cuenta.Clasificacion == "DETALLE") Nodo.Propio = MontoPropio(Cuenta);
                PorId[Cuenta.IdCuenta] = Nodo;
                PorCodigo[Cuenta.Codigo] = Nodo;
            }
            foreach (ClsNodoCuentaContabilidad Nodo in PorId.Values)
                if (Nodo.Cuenta.IdCuentaPadre.HasValue && PorId.ContainsKey(Nodo.Cuenta.IdCuentaPadre.Value))
                    PorId[Nodo.Cuenta.IdCuentaPadre.Value].Hijos.Add(Nodo);
            foreach (ClsNodoCuentaContabilidad Nodo in PorId.Values)
                if (!Nodo.Cuenta.IdCuentaPadre.HasValue || !PorId.ContainsKey(Nodo.Cuenta.IdCuentaPadre.Value))
                    ContabilidadFuncTotalizar(Nodo);
            return PorCodigo;
        }

        private static decimal ContabilidadFuncTotalizar(ClsNodoCuentaContabilidad Nodo)
        {
            Nodo.Total = Nodo.Propio;
            Nodo.ConSaldo = Nodo.Propio != 0m;
            foreach (ClsNodoCuentaContabilidad Hijo in Nodo.Hijos)
            {
                Nodo.Total += ContabilidadFuncTotalizar(Hijo);
                Nodo.ConSaldo |= Hijo.ConSaldo;
            }
            return Nodo.Total;
        }

        private static void ContabilidadMetAgregarFilas(DataTable Filas, string Rubro, ClsNodoCuentaContabilidad Nodo, int Nivel, bool PorNaturaleza)
        {
            if (Nivel > 0 && !Nodo.ConSaldo) return;
            Filas.Rows.Add(Rubro, Nodo.Cuenta.Codigo, new string(' ', Nivel * 4) + Nodo.Cuenta.Nombre, Nodo.Cuenta.Nombre,
                PorNaturaleza ? ContabilidadFuncImporte(Nodo) : Nodo.Total, Nivel, false);
            foreach (ClsNodoCuentaContabilidad Hijo in Nodo.Hijos)
                ContabilidadMetAgregarFilas(Filas, Rubro, Hijo, Nivel + 1, PorNaturaleza);
        }

        private static void ContabilidadMetAgregarTotal(DataTable Filas, string Rubro, string Texto, decimal? Importe)
        {
            Filas.Rows.Add(Rubro, string.Empty, Texto, Texto, Importe.HasValue ? (object)Importe.Value : DBNull.Value, 0, true);
        }

        private static decimal ContabilidadFuncAgregarRubro(DataTable Filas, Dictionary<string, ClsNodoCuentaContabilidad> Arbol, string Codigo,
            string Rubro, bool PorNaturaleza)
        {
            if (!Arbol.ContainsKey(Codigo)) return 0m;
            ContabilidadMetAgregarFilas(Filas, Rubro, Arbol[Codigo], 0, PorNaturaleza);
            return ContabilidadFuncImporte(Arbol[Codigo]);
        }

        private static decimal ContabilidadFuncImporte(Dictionary<string, ClsNodoCuentaContabilidad> Arbol, string Codigo)
        {
            return Arbol.ContainsKey(Codigo) ? ContabilidadFuncImporte(Arbol[Codigo]) : 0m;
        }

        private static decimal ContabilidadFuncImporte(ClsNodoCuentaContabilidad Nodo)
        {
            return Nodo.Cuenta.Tipo == "DEUDORA" ? Nodo.Total : -Nodo.Total;
        }

        private static DataTable ContabilidadFuncTabla()
        {
            DataTable Tabla = new DataTable("DsEstadoFinanciero");
            Tabla.Columns.Add("Rubro", typeof(string));
            Tabla.Columns.Add("Codigo", typeof(string));
            Tabla.Columns.Add("Cuenta", typeof(string));
            Tabla.Columns.Add("Nombre", typeof(string));
            Tabla.Columns.Add("Importe", typeof(decimal));
            Tabla.Columns.Add("Nivel", typeof(int));
            Tabla.Columns.Add("EsTotal", typeof(bool));
            return Tabla;
        }
        // Fin de cambio: Gabriel André Guillén Pocón - 0901-23-1998 - 28/10/2026
    }
}
