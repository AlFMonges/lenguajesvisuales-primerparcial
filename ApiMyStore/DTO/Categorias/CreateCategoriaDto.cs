namespace ApiMyStore.DTO.Categorias
{
    public class CreateCategoriaDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public List<int>? ProductoIds { get; set; }
    }
}
