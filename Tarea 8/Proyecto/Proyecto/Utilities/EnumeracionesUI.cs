using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Proyecto.Models;

namespace Proyecto.Models
{
    /// <summary>
    /// Traducciones de las enumeraciones para mostrarlas en la interfaz.
    /// </summary>
    public static class EnumeracionesUI
    {
        private static readonly Dictionary<TipoVehiculo, string> Vehiculos =
            new Dictionary<TipoVehiculo, string>
            {
                { TipoVehiculo.Automovil, "Automovil" },
                { TipoVehiculo.SUV, "SUV" },
                { TipoVehiculo.Minivan, "Minivan" },
                { TipoVehiculo.Minibus, "Minibus" },
                { TipoVehiculo.Autobus, "Autobus" },
                { TipoVehiculo.Camion, "Camion" }
            };

        private static readonly Dictionary<TipoServicio, string> Servicios =
            new Dictionary<TipoServicio, string>
            {
                { TipoServicio.Pasajeros, "Transporte de pasajeros" },
                { TipoServicio.Escolar, "Transporte escolar" },
                { TipoServicio.Turistico, "Transporte turistico" },
                { TipoServicio.Empresarial, "Transporte empresarial" },
                { TipoServicio.Mercancias, "Transporte de mercancias" },
                { TipoServicio.Privado, "Servicio privado" },
                { TipoServicio.PorHora, "Servicio por hora" },
                { TipoServicio.PorKilometro, "Servicio por kilometraje" }
            };

        private static readonly Dictionary<TipoCosto, string> TiposCosto =
            new Dictionary<TipoCosto, string>
            {
                { TipoCosto.Fijo, "Fijo (por periodo)" },
                { TipoCosto.Variable, "Variable (por km)" }
            };

        private static readonly Dictionary<PeriodicidadCosto, string> Periodicidades =
            new Dictionary<PeriodicidadCosto, string>
            {
                { PeriodicidadCosto.Mensual, "Mensual" },
                { PeriodicidadCosto.Semanal, "Semanal" },
                { PeriodicidadCosto.Diario, "Diario" },
                { PeriodicidadCosto.PorKilometro, "Por km" }
            };

        private static readonly Dictionary<CategoriaCosto, string> Categorias =
            new Dictionary<CategoriaCosto, string>
            {
                { CategoriaCosto.Mantenimiento, "Mantenimiento" },
                { CategoriaCosto.Seguro, "Seguro" },
                { CategoriaCosto.Peajes, "Peajes" },
                { CategoriaCosto.Otros, "Otros costos" }
            };

        private static readonly Dictionary<UnidadCombustible, string> Unidades =
            new Dictionary<UnidadCombustible, string>
            {
                { UnidadCombustible.Galon, "Gallon" },
                { UnidadCombustible.Litro, "Litro" }
            };

        public static string Etiqueta<T>(T valor) where T : struct, IConvertible
        {
            switch (valor)
            {
                case TipoVehiculo v: return Vehiculos[v];
                case TipoServicio s: return Servicios[s];
                case TipoCosto t: return TiposCosto[t];
                case PeriodicidadCosto p: return Periodicidades[p];
                case CategoriaCosto c: return Categorias[c];
                case UnidadCombustible u: return Unidades[u];
                default: return valor.ToString();
            }
        }

        /// <summary>Llena un ComboBox con una enumeración (valor = item, texto = etiqueta).</summary>
        public static void LlenarCombo<T>(ComboBox combo) where T : struct, IConvertible
        {
            combo.BeginUpdate();
            combo.Items.Clear();
            foreach (T valor in Enum.GetValues(typeof(T)))
            {
                combo.Items.Add(new ComboItem<T>(valor, Etiqueta(valor)));
            }
            combo.EndUpdate();
        }

        public static T ValorCombo<T>(ComboBox combo, T defecto) where T : struct, IConvertible
        {
            if (combo.SelectedItem is ComboItem<T> item) return item.Valor;
            return defecto;
        }

        public static void SeleccionarValor<T>(ComboBox combo, T valor) where T : struct, IConvertible
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (combo.Items[i] is ComboItem<T> item && item.Valor.Equals(valor))
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
        }
    }

    public class ComboItem<T>
    {
        public T Valor { get; }
        public string Texto { get; }
        public ComboItem(T valor, string texto) { Valor = valor; Texto = texto; }
        public override string ToString() => Texto;
    }
}