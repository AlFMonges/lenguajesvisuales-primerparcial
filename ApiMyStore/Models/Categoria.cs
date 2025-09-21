namespace ApiMyStore.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}
