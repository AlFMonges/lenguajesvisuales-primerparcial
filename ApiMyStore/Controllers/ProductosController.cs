using ApiMyStore.Data;
using ApiMyStore.DTO.Productos;
using ApiMyStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiMyStore.Controllers
{
    [ApiController]
    [Route("api/productos")]
    public class ProductosController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ProductosController(ApplicationDbContext db) => _db = db;

        // GET: api/productos
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var productos = await _db.Productos
                .Include(p => p.Category)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    CategoryName = p.Category.Name
                })
                .ToListAsync();
            return Ok(productos);
        }

        // GET: api/productos/{id}
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            var producto = await _db.Productos
                .Include(p => p.Category)
                .Where(p => p.Id == id)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    CategoryName = p.Category.Name
                })
                .FirstOrDefaultAsync();

            if (producto == null)
                return NotFound();

            return Ok(producto);
        }

        // POST: api/productos
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            // Validar categoría
            var categoria = await _db.Categorias.FindAsync(dto.CategoryId);
            if (categoria == null)
                return BadRequest(new { message = "La categoría no existe." });

            // Crear producto
            var producto = new Producto
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                CategoryId = dto.CategoryId
            };

            _db.Productos.Add(producto);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = producto.Id }, producto);
        }

        // PUT: api/productos/{id}
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] CreateProductDto dto)
        {
            var producto = await _db.Productos.FindAsync(id);
            if (producto == null)
                return NotFound();

            // Validar categoría
            var categoria = await _db.Categorias.FindAsync(dto.CategoryId);
            if (categoria == null)
                return BadRequest(new { message = "La categoría no existe." });

            // Actualizar campos
            producto.Name = dto.Name;
            producto.Description = dto.Description;
            producto.Price = dto.Price;
            producto.Stock = dto.Stock;
            producto.CategoryId = dto.CategoryId;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/productos/{id}
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var producto = await _db.Productos.FindAsync(id);
            if (producto == null)
                return NotFound();

            _db.Productos.Remove(producto);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

