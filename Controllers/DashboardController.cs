using EcommerceAPI.Models;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly MongoDbService _db;

        public DashboardController(MongoDbService mongoService)
        {
            _db = mongoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardStats()
        {
            var products = _db.Products;
            var orders = _db.Orders;
            var users = _db.Users;

            var totalProducts = await products.CountDocumentsAsync(FilterDefinition<Product>.Empty);
            var totalOrders = await orders.CountDocumentsAsync(FilterDefinition<Order>.Empty);
            var totalSalesAgg = await orders.Aggregate()
                .Group(new BsonDocument { { "_id", BsonNull.Value }, { "total", new BsonDocument("$sum", "$Amount") } })
                .FirstOrDefaultAsync();
            var totalSales = totalSalesAgg?["total"].AsDecimal ?? 0;
            var totalUsers = await users.CountDocumentsAsync(FilterDefinition<User>.Empty);

            return Ok(new
            {
                totalProducts,
                totalOrders,
                totalSales,
                totalUsers
            });
        }
    }
}
