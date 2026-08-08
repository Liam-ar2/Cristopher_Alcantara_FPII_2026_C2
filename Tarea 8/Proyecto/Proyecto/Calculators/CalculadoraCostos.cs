using System;
using System.Collections.Generic;
using System.Linq;
using Proyecto.Models;

namespace Proyecto.Calculators
{
    /// <summary>
    /// Realiza todos los calculos economicos de un servicio de transporte.
    ///
    /// Conceptos:
    ///  - Costo por km: se distribuye entre los kilometros recorridos.
    ///      * Costos FIJOS (seguro, salario, otros de periodo) se convierten al
    ///        monto mensual y se dividen entre el kilometraje mensual del vehiculo.
    ///      * Costos VARIABLES (combustible, mantenimiento por uso) se dividen
    ///        entre el rendimiento del vehiculo o ya vienen expresados por km.
    ///  - Costo por servicio: = CostoPorKm * distancia + peajes de la ruta (por servicio).
    ///  - Ganancia: porcentaje sobre el costo total del servicio.
    ///  - Precio final: costo total + ganancia.
    /// </summary>
    public static class CalculadoraCostos
    {
        // Factores para convertir un costo de periodo a mensual.
        private const decimal DiasPorMes = 30m;
        private const decimal SemanasPorMes = 4.333m;

        public static ResultadoCalculo Calcular(
            Vehiculo vehiculo,
            Conductor conductor,
            ConfiguracionGlobal config,
            IEnumerable<CostoVehiculo> costos,
            decimal distanciaKm,
            int pasajeros,
            decimal cargaKg,
            decimal horas,
            decimal peajesManualRd,
            decimal margenGanancia,
            TipoServicio tipoServicio)
        {
            var r = new ResultadoCalculo { DistanciaKm = distanciaKm };

            // 1. Combustible por km = precio por unidad / rendimiento (km por unidad)
            decimal combustiblePorKm = config.PrecioCombustible / vehiculo.RendimientoPorUnidad;
            r.CombustibleUtilizado = distanciaKm / vehiculo.RendimientoPorUnidad;
            r.CostoCombustible = combustiblePorKm * distanciaKm;

            // 2. Costos fijos y variables agrupados por categoria.
            decimal kmMes = vehiculo.KilometrajeMensual;
            decimal mantKm = 0m, segKm = 0m, peajesKm = 0m, otrosKm = 0m;

            foreach (var costo in costos ?? Enumerable.Empty<CostoVehiculo>())
            {
                decimal porKm = CostoPorKm(costo, kmMes);
                switch (costo.Categoria)
                {
                    case CategoriaCosto.Mantenimiento: mantKm += porKm; break;
                    case CategoriaCosto.Seguro: segKm += porKm; break;
                    case CategoriaCosto.Peajes: peajesKm += porKm; break;
                    case CategoriaCosto.Otros: otrosKm += porKm; break;
                }
            }

            // 3. Salario del conductor (fijo) distribuido por km.
            decimal salarioMes = conductor != null ? conductor.SalarioMensual : 0m;
            decimal salarioPorKm = salarioMes / kmMes;

            // 4. Costos del servicio por concepto.
            r.CostoMantenimiento = mantKm * distanciaKm;
            r.CostoSeguro = segKm * distanciaKm;
            r.CostoConductor = salarioPorKm * distanciaKm;
            r.CostoPeajes = peajesKm * distanciaKm + peajesManualRd;
            r.CostoOtros = otrosKm * distanciaKm;

            // 5. Total por km y total del servicio.
            decimal totalPorKm = combustiblePorKm + mantKm + segKm + salarioPorKm + peajesKm + otrosKm;
            r.CostoPorKilometro = totalPorKm;
            r.CostoTotalServicio = (totalPorKm * distanciaKm) + peajesManualRd;

            // 6. Costo por pasajero y por kg de carga (cuando aplica).
            r.CostoPorPasajero = pasajeros > 0 ? r.CostoTotalServicio / pasajeros : 0m;
            r.CostoPorCargaKg = cargaKg > 0 ? r.CostoTotalServicio / cargaKg : 0m;

            // 7. Margen de ganancia y precio final.
            r.Ganancia = r.CostoTotalServicio * (margenGanancia / 100m);
            r.PrecioFinalRecomendado = r.CostoTotalServicio + r.Ganancia;

            return r;
        }

        /// <summary>
        /// Convierte un costo a su valor por kilometraje.
        /// Fijo: (monto * factor a mes) / kilometraje mensual.
        /// Variable: se usa directamente el monto (ya es por km).
        /// </summary>
        public static decimal CostoPorKm(CostoVehiculo costo, decimal kmMes)
        {
            if (costo.Periodicidad == PeriodicidadCosto.PorKilometro)
                return costo.Monto;

            decimal factor = costo.Periodicidad switch
            {
                PeriodicidadCosto.Mensual => 1m,
                PeriodicidadCosto.Semanal => SemanasPorMes,
                PeriodicidadCosto.Diario => DiasPorMes,
                _ => 1m
            };

            return (costo.Monto * factor) / kmMes;
        }
    }
}