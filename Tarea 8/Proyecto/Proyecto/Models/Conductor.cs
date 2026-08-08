namespace Proyecto.Models
{
    public class Conductor
    {
        public int Id { get; set; }
        public string Documento { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public decimal SalarioMensual { get; set; }

        public string SalarioTexto => Utilities.Formato.Moneda(SalarioMensual);

        public string Descripcion => string.IsNullOrWhiteSpace(Nombre) ? "(Sin conductor)" : Nombre;
        public override string ToString() => Descripcion;
    }
}