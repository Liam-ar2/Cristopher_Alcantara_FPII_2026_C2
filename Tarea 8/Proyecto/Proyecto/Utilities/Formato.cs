using System;
using System.Globalization;

namespace Proyecto.Utilities
{
    public static class Formato
    {
        private static readonly CultureInfo CulturaRD = new CultureInfo("es-DO");

        public static string Moneda(decimal valor)
            => string.Format(CulturaRD, "RD$ {0:N2}", valor);

        public static string Kilometros(decimal valor)
            => string.Format(CulturaRD, "{0:N1} km", valor);

        public static string Numero(decimal valor)
            => valor.ToString("0.##", CulturaRD);

        public static bool ParsearDecimal(string texto, out decimal valor)
            => decimal.TryParse(texto, NumberStyles.Number, CulturaRD, out valor);

        public static bool ParsearEntero(string texto, out int valor)
            => int.TryParse(texto, NumberStyles.Integer, CulturaRD, out valor);
    }
}