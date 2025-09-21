namespace ApiMyStore.Models
{
    public class CabeceraPedido
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Usuario? User { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public decimal Total { get; set; }
        public ICollection<Pedido>? Items { get; set; }
    }
}
