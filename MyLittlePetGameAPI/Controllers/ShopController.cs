using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLittlePetGameAPI.Models;

namespace MyLittlePetGameAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShopController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public ShopController(AppDbContext context)
        {
            _context = context;
        }
        
        // GET: Shop - Get all shops
        [HttpGet]
        public ActionResult<IEnumerable<Shop>> Get()
        {
            return Ok(_context.Shops.ToList());
        }
        
        // GET: Shop/{id} - Get shop by ID
        [HttpGet("{id}")]
        public ActionResult<Shop> GetById(int id)
        {
            var shop = _context.Shops.Find(id);
            
            if (shop == null)
            {
                return NotFound();
            }
            
            return Ok(shop);
        }
        
        // GET: Shop/{id}/Products - Get all products in a shop
        [HttpGet("{id}/Products")]
        public ActionResult<IEnumerable<ShopProduct>> GetShopProducts(int id)
        {
            var shop = _context.Shops.Find(id);
            if (shop == null)
            {
                return NotFound("Shop not found");
            }
            
            var products = _context.ShopProducts
                .Where(p => p.ShopId == id)
                .ToList();
                
            return Ok(products);
        }
        
        // POST: Shop - Create a new shop
        [HttpPost]
        public ActionResult<Shop> Create(string name, string? type, string? description)
        {
            // Validate required fields
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Shop name is required");
            }
            
            var shop = new Shop
            {
                Name = name,
                Type = type,
                Description = description
            };
            
            _context.Shops.Add(shop);
            _context.SaveChanges();
            
            return CreatedAtAction(nameof(GetById), new { id = shop.ShopId }, shop);
        }
        
        // PUT: Shop/{id} - Update a shop
        [HttpPut("{id}")]
        public ActionResult<Shop> Update(int id, string name, string? type, string? description)
        {
            var shop = _context.Shops.Find(id);
            
            if (shop == null)
            {
                return NotFound();
            }
            
            // Validate required fields
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Shop name is required");
            }
            
            shop.Name = name;
            shop.Type = type;
            shop.Description = description;
            
            _context.Shops.Update(shop);
            _context.SaveChanges();
            
            return Ok(shop);
        }
        
        // DELETE: Shop/{id} - Delete a shop
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var shop = _context.Shops.Find(id);
            
            if (shop == null)
            {
                return NotFound();
            }
            
            // Check if shop has products
            var hasProducts = _context.ShopProducts.Any(p => p.ShopId == id);
            if (hasProducts)
            {
                return BadRequest("Cannot delete shop with existing products. Delete all products first.");
            }
            
            _context.Shops.Remove(shop);
            _context.SaveChanges();
            
            return NoContent();
        }
    }
}
