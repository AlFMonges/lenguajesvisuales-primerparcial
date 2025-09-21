namespace ApiMyStore.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public CabeceraPedido? CabeceraPedido { get; set; }
        public int ProductId { get; set; }
        public Producto? Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
