using System;
using Proyecto.Models;
using Proyecto.Utilities;

namespace Proyecto.Models
{
    public class ServicioTransporte
    {
        public int Id { get; set; }
        public int VehiculoId { get; set; }
        public int? ConductorId { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public TipoServicio TipoServicio { get; set; }
        public decimal DistanciaKm { get; set; }
        public int Pasajeros { get; set; }
        public decimal CargaKg { get; set; }
        public decimal Horas { get; set; }
        public decimal PeajesManualRd { get; set; }
        public decimal MargenGanancia { get; set; }

        public override string ToString()
            => $"{Fecha:dd/MM/yyyy} - {EnumeracionesUI.Etiqueta(TipoServicio)} - {DistanciaKm:0.##} km";
    }
}