using EcommerceAPI.Models;
using EcommerceAPI.Models.Dto;
using EcommerceAPI.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Text.Json;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly MongoDbService _db;

        public ProductsController(MongoDbService db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct()
        {

            // Get the product JSON from FormData
            var productJson = Request.Form["product"].ToString();

            if (string.IsNullOrEmpty(productJson))
            {
                return BadRequest("Product data is required");
            }

            // Deserialize the product JSON
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var dto = JsonSerializer.Deserialize<CreateProductDto>(productJson, options);

            if (dto == null)
            {
                return BadRequest("Invalid product data");
            }

            // Validate the DTO
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(dto);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(dto, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(vr => vr.ErrorMessage);
                return BadRequest(new { errors });
            }

            // Process images
            var images = new List<ImageFile>();
            var imageFiles = Request.Form.Files.Where(f => f.Name == "images").ToList();

            foreach (var file in imageFiles)
            {
                if (!file.ContentType.StartsWith("image/"))
                    return BadRequest("Only image files allowed.");

                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                images.Add(new ImageFile
                {
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    Data = ms.ToArray()
                });
            }

            // Create product
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                BasePrice = dto.BasePrice,
                Category = dto.Category,
                Stock = dto.Stock,
                Images = images,
                Sizes = dto.Sizes,
                isActive = dto.isActive
            };

            await _db.Products.InsertOneAsync(product);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, new { Id = product.Id });

        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(string id)
        {
            try
            {
                if (!Request.HasFormContentType)
                {
                    return BadRequest("Request must be form content type");
                }

                var existingProduct = await _db.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
                if (existingProduct == null)
                {
                    return NotFound("Product not found");
                }

                var productJson = Request.Form["product"].ToString();
                if (string.IsNullOrEmpty(productJson))
                {
                    return BadRequest("Product data is required");
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var productData = JsonSerializer.Deserialize<CreateProductDto>(productJson, options);
                if (productData == null)
                {
                    return BadRequest("Invalid product data");
                }

                // Process new images
                var newImages = new List<ImageFile>();
                var imageFiles = Request.Form.Files;

                foreach (var file in imageFiles)
                {
                    if (!file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                    {
                        return BadRequest("Only image files allowed.");
                    }

                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    newImages.Add(new ImageFile
                    {
                        FileName = file.FileName,
                        ContentType = file.ContentType,
                        Data = ms.ToArray()
                    });
                }

                // Update existing product
                existingProduct.Name = productData.Name;
                existingProduct.Description = productData.Description;
                existingProduct.BasePrice = productData.BasePrice;
                existingProduct.Category = productData.Category;
                existingProduct.Stock = productData.Stock;
                existingProduct.Sizes = productData.Sizes;
                existingProduct.isActive = productData.isActive;

                // Keep existing images and add new ones
                existingProduct.Images.AddRange(newImages);

                await _db.Products.ReplaceOneAsync(p => p.Id == id, existingProduct);
                return Ok(existingProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the product");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var products = await _db.Products.Find(_ => true).ToListAsync();

                var dtos = products.Select(p => new ProductResponseDto
                {
                    id = p.Id,
                    name = p.Name,
                    description = p.Description,
                    basePrice = p.BasePrice,
                    category = p.Category,
                    stock = p.Stock,
                    sizes = p.Sizes,
                    isActive = p.isActive,
                    imagePaths = p.Images.Select(img => $"api/products/{p.Id}/images/{p.Images.IndexOf(img)}").ToList()
                }).ToList();

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching products");
            }
        }

        [HttpGet("{id}/images/{index}")]
        public async Task<IActionResult> GetProductImage(string id, int index)
        {
            try
            {
                // Validate ObjectId format
                if (!ObjectId.TryParse(id, out _))
                {
                    return BadRequest("Invalid product ID format");
                }

                // Find the product
                var product = await _db.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
                if (product == null)
                {
                    return NotFound("Product not found");
                }

                // Validate image index
                if (index < 0 || index >= product.Images.Count)
                {
                    return BadRequest("Invalid image index");
                }

                // Get the image
                var image = product.Images[index];

                // Return the image with proper content type
                return File(image.Data, image.ContentType, image.FileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching the image");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(string id)
        {
            var p = await _db.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (p == null) return NotFound();
            var dtos = new ProductResponseDto
            {
                id = p.Id,
                name = p.Name,
                description = p.Description,
                basePrice = p.BasePrice,
                category = p.Category,
                stock = p.Stock,
                sizes = p.Sizes,
                imagePaths = p.Images.Select(img => $"/api/products/{p.Id}/images/{p.Images.IndexOf(img)}").ToList()
            };
            return Ok(dtos); // Includes full images (use cautiously!)
        }

        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check for duplicates (optional)
            var existing = await _db.Categories.Find(c => c.Name == dto.Name).FirstOrDefaultAsync();
            if (existing != null)
                return Conflict($"Category '{dto.Name}' already exists.");

            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _db.Categories.InsertOneAsync(category);
            return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, category);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _db.Categories.Find(_ => true).ToListAsync();
            return Ok(categories);
        }

    }
}
