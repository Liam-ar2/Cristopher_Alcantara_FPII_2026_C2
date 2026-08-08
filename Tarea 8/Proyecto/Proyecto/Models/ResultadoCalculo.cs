namespace Proyecto.Models
{
    public class ResultadoCalculo
    {
        public int Id { get; set; }
        public int ServicioId { get; set; }

        public decimal DistanciaKm { get; set; }
        public decimal CombustibleUtilizado { get; set; }
        public decimal CostoCombustible { get; set; }
        public decimal CostoMantenimiento { get; set; }
        public decimal CostoSeguro { get; set; }
        public decimal CostoConductor { get; set; }
        public decimal CostoPeajes { get; set; }
        public decimal CostoOtros { get; set; }

        public decimal CostoTotalServicio { get; set; }
        public decimal CostoPorKilometro { get; set; }
        public decimal CostoPorPasajero { get; set; }
        public decimal CostoPorCargaKg { get; set; }
        public decimal Ganancia { get; set; }
        public decimal PrecioFinalRecomendado { get; set; }
    }
}