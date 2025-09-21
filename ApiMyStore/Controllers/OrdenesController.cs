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
    [Authorize]
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

            var items = new List<OrderItemDto>();

            foreach (var i in cabeceraPedido.Items)
            {
                var prod = await _db.Productos.FindAsync(i.ProductId);

                items.Add(new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = prod?.Name ?? string.Empty,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                });
            }

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
    }
}

