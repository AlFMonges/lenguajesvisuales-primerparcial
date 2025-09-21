using ApiMyStore.Data;
using ApiMyStore.DTO.Categorias;
using ApiMyStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiMyStore.Controllers
{
    [ApiController]
    [Route("api/categorias")]
    public class CategoriasController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public CategoriasController(ApplicationDbContext db) => _db = db;

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _db.Categorias.ToListAsync());
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            var cat = await _db.Categorias.FindAsync(id);
            if (cat == null) return NotFound();
            return Ok(cat);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateCategoriaDto dto)
        {
            var categoria = new Categoria
            {
                Name = dto.Name,
                Description = dto.Description
            };

            _db.Categorias.Add(categoria);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = categoria.Id }, categoria);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> PutCategoria(int id, CategoriaDTO categoriaDto)
        {
            if (id != categoriaDto.Id)
                return BadRequest();

            var categoria = await _db.Categorias
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria == null)
                return NotFound();

            // Actualizar campos simples
            categoria.Name = categoriaDto.Name;
            categoria.Description = categoriaDto.Description;

            // Actualizar productos asociados (opcional)
            if (categoriaDto.ProductoIds != null)
            {
                categoria.Productos = await _db.Productos
                    .Where(p => categoriaDto.ProductoIds.Contains(p.Id))
                    .ToListAsync();
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var cat = await _db.Categorias.FindAsync(id);
            if (cat == null) return NotFound();

            _db.Categorias.Remove(cat);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}

