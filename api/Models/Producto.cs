namespace api.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = String.Empty;
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
    }
}