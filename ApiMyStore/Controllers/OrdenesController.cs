using ApiMyStore.Data;
using ApiMyStore.DTO.Ordenes;
using ApiMyStore.Models;
using ApiMyStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiMyStore.Controllers
{
    [ApiController]
    [Route("api/ordenes")]
    public class OrdenesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserService _userService;

        public OrdenesController(ApplicationDbContext db, IUserService userService)
        {
            _db = db;
            _userService = userService;
        }

        // GET api/ordenes
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var userId = _userService.GetUserId();
            var ordenes = await _db.CabeceraPedidos
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.UserId == userId)
                .ToListAsync();

            var result = ordenes.Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                Date = o.Date,
                Total = o.Total,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product != null ? i.Product.Name : string.Empty,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            });

            return Ok(result);
        }

        // POST api/ordenes
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            var userId = _userService.GetUserId();

            if (dto.Items == null || !dto.Items.Any())
                return BadRequest(new { message = "La orden debe contener al menos un producto." });

            var cabeceraPedido = new CabeceraPedido
            {
                UserId = userId,
                Date = DateTime.UtcNow,
                Items = new List<Pedido>()
            };

            foreach (var item in dto.Items)
            {
                var producto = await _db.Productos.FindAsync(item.ProductId);
                if (producto == null)
                    return BadRequest(new { message = $"Producto {item.ProductId} no existe" });

                if (producto.Stock < item.Quantity)
                    return BadRequest(new { message = $"Stock insuficiente para {producto.Name}" });

                producto.Stock -= item.Quantity;

                cabeceraPedido.Items.Add(new Pedido
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = producto.Price
                });
            }

            // Calcular el total de la ordenusing ApiMyStore.Data;
            cabeceraPedido.Total = cabeceraPedido.Items.Sum(i => i.Quantity * i.UnitPrice);

            _db.CabeceraPedidos.Add(cabeceraPedido);
            await _db.SaveChangesAsync();

            // recargar Items con el Product incluido
            await _db.Entry(cabeceraPedido)
                     .Collection(o => o.Items)
                     .Query()
                     .Include(i => i.Product)
                     .LoadAsync();

            // mapear a DTO usando i.Product ya cargado
            var items = cabeceraPedido.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? string.Empty,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList();

            var result = new OrderDto
            {
                Id = cabeceraPedido.Id,
                UserId = cabeceraPedido.UserId,
                Date = cabeceraPedido.Date,
                Total = cabeceraPedido.Total,
                Items = items
            };

            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] CreateOrderDto dto)
        {
            var userId = _userService.GetUserId();

            var orden = await _db.CabeceraPedidos
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (orden == null)
                return NotFound(new { message = "Orden no encontrada" });

            if (dto.Items == null || !dto.Items.Any())
                return BadRequest(new { message = "La orden debe contener al menos un producto." });

            // restaurar stock previo
            foreach (var item in orden.Items)
            {
                var producto = await _db.Productos.FindAsync(item.ProductId);
                if (producto != null)
                    producto.Stock += item.Quantity;
            }

            // limpiar ítems anteriores
            _db.Pedidos.RemoveRange(orden.Items);
            orden.Items.Clear();

            // agregar los nuevos ítems
            foreach (var item in dto.Items)
            {
                var producto = await _db.Productos.FindAsync(item.ProductId);
                if (producto == null)
                    return BadRequest(new { message = $"Producto {item.ProductId} no existe" });

                if (producto.Stock < item.Quantity)
                    return BadRequest(new { message = $"Stock insuficiente para {producto.Name}" });

                producto.Stock -= item.Quantity;

                orden.Items.Add(new Pedido
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = producto.Price
                });
            }

            // recalcular total
            orden.Total = orden.Items.Sum(i => i.Quantity * i.UnitPrice);
            orden.Date = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Ok(new { message = "Orden actualizada con éxito", orden.Id, orden.Total });
        }

        // DELETE api/ordenes/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userService.GetUserId();

            var orden = await _db.CabeceraPedidos
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (orden == null)
                return NotFound(new { message = "Orden no encontrada" });

            // restaurar stock
            foreach (var item in orden.Items)
            {
                var producto = await _db.Productos.FindAsync(item.ProductId);
                if (producto != null)
                    producto.Stock += item.Quantity;
            }

            _db.CabeceraPedidos.Remove(orden);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Orden eliminada con éxito" });
        }
    }
}

