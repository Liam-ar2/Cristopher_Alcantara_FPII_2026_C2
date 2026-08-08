namespace Proyecto.Models
{
    public class Vehiculo
    {
        public int Id { get; set; }
        public string Placa { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public TipoVehiculo Tipo { get; set; }
        public int CapacidadPasajeros { get; set; }
        public decimal CapacidadCargaKg { get; set; }
        public decimal RendimientoPorUnidad { get; set; }
        public UnidadCombustible UnidadCombustible { get; set; }
        public decimal KilometrajeMensual { get; set; }

        public string TipoTexto => EnumeracionesUI.Etiqueta(Tipo);
        public string UnidadTexto => EnumeracionesUI.Etiqueta(UnidadCombustible);
        public string RendimientoTexto => string.Format("{0:0.##} km/{1}", RendimientoPorUnidad,
            EnumeracionesUI.Etiqueta(UnidadCombustible).ToLower());
        public string KmMesTexto => string.Format("{0:0} km", KilometrajeMensual);

        public string Descripcion =>
            string.Format("{0} - {1} {2} ({3})",
                EnumeracionesUI.Etiqueta(Tipo), Marca, Modelo, Placa);

        public override string ToString() => Descripcion;
    }
}