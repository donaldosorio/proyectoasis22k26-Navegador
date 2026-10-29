/*
 * ==================================================================
 * Área : Contabilidad
 * Autor : Gabriel André Guillén Pocón
 * Carné : 0901-23-1998
 * Fecha : 28/10/2026
 * ==================================================================
 * Propósito :
 *  Lee el catálogo de cuentas con su cuenta padre, los saldos de
 *  tblSaldoCuentaPeriodo y el movimiento de efectivo de las pólizas
 *  aplicadas para armar los estados financieros.
 * Reglas especificas:
 *  El saldo final es el del último período registrado hasta el
 *  período pedido. Cargo y abono suman los períodos del rango. El
 *  efectivo son las cuentas de detalle de Caja (1.01.01) y Bancos
 *  (1.01.02); el flujo se toma de la contrapartida de cada póliza.
 * ===================================================================
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using CapaModelo_Contabilidad.Entidades;

namespace CapaModelo_Contabilidad.Repositorios
{
    public class ClsRepositorioEstadosFinancierosContabilidad : ClsRepositorioMaestroContabilidad
    {
        private const string _SelectCuentas =
            "SELECT c.idCuenta, c.idCuentaPadre, c.codigoCuenta, c.nombreCuenta, c.tipoCuenta, c.clasificacionCuenta, " +
            "COALESCE((SELECT s.saldoFinalSaldoCuentaPeriodo FROM tblSaldoCuentaPeriodo s WHERE s.idCuenta = c.idCuenta " +
            "AND s.periodoContableSaldoCuentaPeriodo <= ? ORDER BY s.periodoContableSaldoCuentaPeriodo DESC LIMIT 1), 0) AS saldo, " +
            "COALESCE((SELECT SUM(s.cargoSaldoCuentaPeriodo) FROM tblSaldoCuentaPeriodo s WHERE s.idCuenta = c.idCuenta " +
            "AND s.periodoContableSaldoCuentaPeriodo BETWEEN ? AND ?), 0) AS cargo, " +
            "COALESCE((SELECT SUM(s.abonoSaldoCuentaPeriodo) FROM tblSaldoCuentaPeriodo s WHERE s.idCuenta = c.idCuenta " +
            "AND s.periodoContableSaldoCuentaPeriodo BETWEEN ? AND ?), 0) AS abono " +
            "FROM tblCuenta c WHERE c.is_active = TRUE ORDER BY c.codigoCuenta";

        private const string _CondicionEfectivo = "(ce.codigoCuenta LIKE '1.01.01.%' OR ce.codigoCuenta LIKE '1.01.02.%')";

        private const string _SelectFlujo =
            "SELECT pd.idCuenta, SUM(pd.haberPolizaDetalle - pd.debePolizaDetalle) FROM tblPolizaDetalle pd " +
            "INNER JOIN tblPolizaEncabezado pe ON pe.idPolizaEncabezado = pd.idPolizaEncabezado " +
            "INNER JOIN tblCuenta c ON c.idCuenta = pd.idCuenta " +
            "WHERE pe.estadoPolizaEncabezado = 'APLICADA' AND pe.is_active = TRUE AND pd.is_active = TRUE " +
            "AND pe.fechaPolizaEncabezado BETWEEN ? AND ? AND NOT (c.codigoCuenta LIKE '1.01.01.%' OR c.codigoCuenta LIKE '1.01.02.%') " +
            "AND EXISTS (SELECT 1 FROM tblPolizaDetalle e INNER JOIN tblCuenta ce ON ce.idCuenta = e.idCuenta " +
            "WHERE e.idPolizaEncabezado = pe.idPolizaEncabezado AND e.is_active = TRUE AND " + _CondicionEfectivo + ") " +
            "GROUP BY pd.idCuenta";

        private const string _SelectEfectivo =
            "SELECT SUM(CASE WHEN ce.tipoCuenta = 'DEUDORA' THEN s.saldoFinalSaldoCuentaPeriodo ELSE -s.saldoFinalSaldoCuentaPeriodo END) " +
            "FROM tblCuenta ce INNER JOIN tblSaldoCuentaPeriodo s ON s.idCuenta = ce.idCuenta " +
            "AND s.periodoContableSaldoCuentaPeriodo = (SELECT MAX(u.periodoContableSaldoCuentaPeriodo) FROM tblSaldoCuentaPeriodo u " +
            "WHERE u.idCuenta = ce.idCuenta AND u.periodoContableSaldoCuentaPeriodo <= ?) " +
            "WHERE ce.clasificacionCuenta = 'DETALLE' AND " + _CondicionEfectivo;

        public List<ClsCuentaSaldoContabilidad> ContabilidadFuncObtenerCuentas(string Desde, string Hasta)
        {
            List<ClsCuentaSaldoContabilidad> Cuentas = new List<ClsCuentaSaldoContabilidad>();
            List<OdbcParameter> Parametros = new List<OdbcParameter>
            {
                new OdbcParameter("Saldo", Hasta), new OdbcParameter("DesdeCargo", Desde), new OdbcParameter("HastaCargo", Hasta),
                new OdbcParameter("DesdeAbono", Desde), new OdbcParameter("HastaAbono", Hasta)
            };
            using (DataTable Tabla = ContabilidadFuncEjecutarConsulta(_SelectCuentas, CommandType.Text, Parametros))
                foreach (DataRow Fila in Tabla.Rows)
                    Cuentas.Add(new ClsCuentaSaldoContabilidad
                    {
                        IdCuenta = Convert.ToInt32(Fila["idCuenta"]),
                        IdCuentaPadre = Fila["idCuentaPadre"] == DBNull.Value ? (int?)null : Convert.ToInt32(Fila["idCuentaPadre"]),
                        Codigo = Convert.ToString(Fila["codigoCuenta"]),
                        Nombre = Convert.ToString(Fila["nombreCuenta"]),
                        Tipo = Convert.ToString(Fila["tipoCuenta"]),
                        Clasificacion = Convert.ToString(Fila["clasificacionCuenta"]),
                        SaldoFinal = Convert.ToDecimal(Fila["saldo"]),
                        Cargo = Convert.ToDecimal(Fila["cargo"]),
                        Abono = Convert.ToDecimal(Fila["abono"])
                    });
            return Cuentas;
        }

        public Dictionary<int, decimal> ContabilidadFuncObtenerFlujoEfectivo(DateTime Desde, DateTime Hasta)
        {
            Dictionary<int, decimal> Flujo = new Dictionary<int, decimal>();
            using (DataTable Tabla = ContabilidadFuncEjecutarConsulta(_SelectFlujo, CommandType.Text, new List<OdbcParameter>
                { new OdbcParameter("Desde", OdbcType.Date) { Value = Desde.Date }, new OdbcParameter("Hasta", OdbcType.Date) { Value = Hasta.Date } }))
                foreach (DataRow Fila in Tabla.Rows) Flujo[Convert.ToInt32(Fila[0])] = Convert.ToDecimal(Fila[1]);
            return Flujo;
        }

        public decimal ContabilidadFuncObtenerEfectivo(string Periodo)
        {
            using (DataTable Tabla = ContabilidadFuncEjecutarConsulta(_SelectEfectivo, CommandType.Text,
                new List<OdbcParameter> { new OdbcParameter("Periodo", Periodo) }))
                return Tabla.Rows.Count == 0 || Tabla.Rows[0][0] == DBNull.Value ? 0m : Convert.ToDecimal(Tabla.Rows[0][0]);
        }
    }
}
