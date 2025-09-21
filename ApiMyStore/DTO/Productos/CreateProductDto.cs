using System.ComponentModel.DataAnnotations;

namespace ApiMyStore.DTO.Productos
{
    public class CreateProductDto
    {
        [Required]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Stock { get; set; }
        [Required]
        public int CategoryId { get; set; }
    }
}
