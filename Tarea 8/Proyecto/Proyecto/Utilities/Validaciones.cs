using System.Collections.Generic;
using System.Linq;
using Proyecto.Models;

namespace Proyecto.Utilities
{
    /// <summary>
    /// Validaciones de negocio. Recolecta mensajes de error claros.
    /// </summary>
    public static class Validaciones
    {
        public static List<string> ValidarVehiculo(Vehiculo v)
        {
            var errores = new List<string>();

            if (string.IsNullOrWhiteSpace(v.Placa))
                errores.Add("Debe indicar la placa del vehiculo.");
            if (string.IsNullOrWhiteSpace(v.Marca))
                errores.Add("Debe indicar la marca del vehiculo.");
            if (string.IsNullOrWhiteSpace(v.Modelo))
                errores.Add("Debe indicar el modelo del vehiculo.");
            if (v.CapacidadPasajeros < 0)
                errores.Add("La capacidad de pasajeros no puede ser negativa.");
            if (v.CapacidadPasajeros == 0)
                errores.Add("La capacidad de pasajeros debe ser mayor que cero.");
            if (v.CapacidadCargaKg < 0)
                errores.Add("La capacidad de carga no puede ser negativa.");
            if (v.RendimientoPorUnidad <= 0)
                errores.Add("El rendimiento del combustible debe ser mayor que cero.");
            if (v.KilometrajeMensual <= 0)
                errores.Add("El kilometraje mensual debe ser mayor que cero.");

            return errores;
        }

        public static List<string> ValidarConductor(Conductor c)
        {
            var errores = new List<string>();

            if (string.IsNullOrWhiteSpace(c.Nombre))
                errores.Add("Debe indicar el nombre del conductor.");
            if (string.IsNullOrWhiteSpace(c.Documento))
                errores.Add("Debe indicar el documento de identificacion del conductor.");
            if (c.SalarioMensual < 0)
                errores.Add("El salario no puede ser negativo.");

            return errores;
        }

        public static List<string> ValidarCosto(CostoVehiculo costo)
        {
            var errores = new List<string>();

            if (string.IsNullOrWhiteSpace(costo.Nombre))
                errores.Add("Debe indicar el nombre del costo.");
            if (costo.Monto < 0)
                errores.Add("El monto del costo no puede ser negativo.");
            if (costo.Tipo == TipoCosto.Fijo && costo.Periodicidad == PeriodicidadCosto.PorKilometro)
                errores.Add("Un costo fijo no puede tener periodicidad 'Por km'. Use 'Variable (por km)'.");
            if (costo.Tipo == TipoCosto.Variable && costo.Periodicidad != PeriodicidadCosto.PorKilometro)
                errores.Add("Un costo variable debe tener periodicidad 'Por km'.");

            return errores;
        }

        public static List<string> ValidarServicio(Vehiculo vehiculo, decimal distancia,
            int pasajeros, decimal carga)
        {
            var errores = new List<string>();

            if (vehiculo == null)
                errores.Add("Debe seleccionar un vehiculo.");
            if (distancia <= 0)
                errores.Add("La distancia debe ser mayor que cero (km).");
            if (pasajeros < 0)
                errores.Add("La cantidad de pasajeros no puede ser negativa.");
            if (pasajeros > vehiculo.CapacidadPasajeros)
                errores.Add(string.Format("La cantidad de pasajeros ({0}) supera la capacidad del vehiculo ({1}).",
                    pasajeros, vehiculo.CapacidadPasajeros));
            if (carga < 0)
                errores.Add("La carga no puede ser negativa.");
            if (vehiculo.CapacidadCargaKg > 0 && carga > vehiculo.CapacidadCargaKg)
                errores.Add(string.Format("La carga ({0} kg) supera la capacidad del vehiculo ({1} kg).",
                    carga, vehiculo.CapacidadCargaKg));

            return errores;
        }
    }
}