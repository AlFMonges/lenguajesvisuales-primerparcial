namespace ApiMyStore.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }
        public Categoria? Category { get; set; }
        public ICollection<Pedido>? Pedidos{ get; set; }
    }
}
