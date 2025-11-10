using EcommerceAPI.Models;
using EcommerceAPI.Models.Dto;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly MongoDbService _db;

        public OrdersController(MongoDbService db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var items = new List<OrderItem>();
            decimal total = 0;

            foreach (var itemDto in dto.Items)
            {
                var product = await _db.Products.Find(p => p.Id == itemDto.ProductId).FirstOrDefaultAsync();
                if (product == null)
                    return BadRequest($"Product {itemDto.ProductId} not found");

                var size = product.Sizes.FirstOrDefault(s => s == itemDto.SizeLabel);
                

                var price = product.BasePrice;
                items.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    SizeLabel = itemDto.SizeLabel,
                    Quantity = itemDto.Quantity,
                    Price = price
                });
                total += price * itemDto.Quantity;
            }

            var order = new Order
            {
                UserId = userId,
                Items = items,
                TotalAmount = total
            };

            await _db.Orders.InsertOneAsync(order);

            // 🔒 TODO: Deduct stock in transaction (simplified)
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, new OrderResponseDto
            {
                Id = order.Id,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var order = await _db.Orders.Find(o => o.Id == id && o.UserId == userId).FirstOrDefaultAsync();
            if (order == null) return NotFound();
            return Ok(order);
        }
    }

}
