namespace ApiMyStore.DTO.Categorias
{
    public class CategoriaDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public List<int>? ProductoIds { get; set; }
    }
}
