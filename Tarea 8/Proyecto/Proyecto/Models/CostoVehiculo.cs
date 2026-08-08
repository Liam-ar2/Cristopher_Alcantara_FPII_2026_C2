namespace Proyecto.Models
{
    public class CostoVehiculo
    {
        public int Id { get; set; }
        public int VehiculoId { get; set; }
        public string Nombre { get; set; }
        public CategoriaCosto Categoria { get; set; }
        public TipoCosto Tipo { get; set; }
        public PeriodicidadCosto Periodicidad { get; set; }
        public decimal Monto { get; set; }

        public override string ToString() => Nombre;
    }
}