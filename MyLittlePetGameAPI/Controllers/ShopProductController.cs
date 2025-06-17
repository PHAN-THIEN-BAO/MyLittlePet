using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLittlePetGameAPI.Models;

namespace MyLittlePetGameAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShopProductController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public ShopProductController(AppDbContext context)
        {
            _context = context;
        }
        
        // GET: ShopProduct - Get all products
        [HttpGet]
        public ActionResult<IEnumerable<ShopProduct>> Get()
        {
            return Ok(_context.ShopProducts
                .Include(p => p.Shop)
                .Include(p => p.Admin)
                .ToList());
        }
        
        // GET: ShopProduct/{id} - Get product by ID
        [HttpGet("{id}")]
        public ActionResult<ShopProduct> GetById(int id)
        {
            var product = _context.ShopProducts
                .Include(p => p.Shop)
                .Include(p => p.Admin)
                .FirstOrDefault(p => p.ShopProductId == id);
            
            if (product == null)
            {
                return NotFound();
            }
            
            return Ok(product);
        }
        
        // GET: ShopProduct/Type/{type} - Get products by type
        [HttpGet("Type/{type}")]
        public ActionResult<IEnumerable<ShopProduct>> GetByType(string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                return BadRequest("Type is required");
            }
            
            var products = _context.ShopProducts
                .Include(p => p.Shop)
                .Where(p => p.Type == type)
                .ToList();
                
            return Ok(products);
        }
          // GET: ShopProduct/Status/{status} - Get products by status
        [HttpGet("Status/{status}")]
        public ActionResult<IEnumerable<ShopProduct>> GetByStatus(int status)
        {
            var products = _context.ShopProducts
                .Include(p => p.Shop)
                .Include(p => p.Admin)
                .Where(p => p.Status == status)
                .ToList();
                
            return Ok(products);
        }
        
        // POST: ShopProduct - Create a new product
        [HttpPost]
        public ActionResult<ShopProduct> Create(int shopId, int adminId, string name, string type, string? description, 
            string? imageUrl, int price, string currencyType, int? quality, int? status)
        {
            // Validate required fields
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type) || string.IsNullOrEmpty(currencyType))
            {
                return BadRequest("Name, type, and currency type are required");
            }
            
            // Validate shop exists
            var shop = _context.Shops.Find(shopId);
            if (shop == null)
            {
                return BadRequest("Shop not found");
            }
            
            // Validate admin exists
            var admin = _context.Users.Find(adminId);
            if (admin == null)
            {
                return BadRequest("Admin user not found");
            }
            
            var product = new ShopProduct
            {
                ShopId = shopId,
                AdminId = adminId,
                Name = name,
                Type = type,
                Description = description,
                ImageUrl = imageUrl,
                Price = price,
                CurrencyType = currencyType,
                Quality = quality ?? 100, // Default to 100 if not provided
                Status = status ?? 1 // Default to 1 (active) if not provided
            };
            
            _context.ShopProducts.Add(product);
            _context.SaveChanges();
            
            return CreatedAtAction(nameof(GetById), new { id = product.ShopProductId }, product);
        }
          // PUT: ShopProduct/{id} - Update a product
        [HttpPut("{id}")]
        public ActionResult<ShopProduct> Update(int id, string? name, string? type, string? description, 
            string? imageUrl, int? price, string? currencyType, int? quality, int? status)
        {
            var product = _context.ShopProducts.Find(id);
            
            if (product == null)
            {
                return NotFound();
            }
            
            // Update only provided fields
            if (!string.IsNullOrEmpty(name))
            {
                product.Name = name;
            }
            
            if (!string.IsNullOrEmpty(type))
            {
                product.Type = type;
            }
            
            if (description != null) // Allow setting description to null
            {
                product.Description = description;
            }
            
            if (imageUrl != null) // Allow setting imageUrl to null
            {
                product.ImageUrl = imageUrl;
            }
            
            if (price.HasValue)
            {
                product.Price = price.Value;
            }
            
            if (!string.IsNullOrEmpty(currencyType))
            {
                product.CurrencyType = currencyType;
            }
            
            if (quality.HasValue)
            {
                product.Quality = quality.Value;
            }
            
            if (status.HasValue)
            {
                product.Status = status.Value;
            }
            
            _context.ShopProducts.Update(product);
            _context.SaveChanges();
            
            return Ok(product);
        }
        
        // DELETE: ShopProduct/{id} - Delete a product
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var product = _context.ShopProducts.Find(id);
            
            if (product == null)
            {
                return NotFound();
            }
            
            // Check if product is in any player's inventory
            var isInInventory = _context.PlayerInventories.Any(i => i.ShopProductId == id);
            if (isInInventory)
            {
                return BadRequest("Cannot delete product that exists in player inventories");
            }
            
            _context.ShopProducts.Remove(product);
            _context.SaveChanges();
            
            return NoContent();
        }
    }
}
