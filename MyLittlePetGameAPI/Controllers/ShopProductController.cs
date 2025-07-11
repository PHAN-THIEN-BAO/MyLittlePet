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
        public ActionResult<IEnumerable<object>> Get()
        {
            try
            {
                var products = _context.ShopProducts
                    .Include(p => p.Shop)
                    .Include(p => p.Admin)
                    .Include(p => p.Pet)
                    .Select(p => new
                    {
                        ShopProductId = p.ShopProductId,
                        ShopId = p.ShopId,
                        AdminId = p.AdminId,
                        PetId = p.PetId,
                        Name = p.Name,
                        Type = p.Type,
                        Description = p.Description,
                        ImageUrl = p.ImageUrl,
                        Price = p.Price,
                        CurrencyType = p.CurrencyType,
                        Status = p.Status,
                        ShopInfo = new
                        {
                            ShopId = p.Shop.ShopId,
                            Name = p.Shop.Name,
                            Type = p.Shop.Type
                        },
                        AdminInfo = new
                        {
                            Id = p.Admin.Id,
                            UserName = p.Admin.UserName
                        },
                        PetInfo = p.Pet == null ? null : new
                        {
                            PetId = p.Pet.PetId,
                            PetType = p.Pet.PetType,
                            PetDefaultName = p.Pet.PetDefaultName,
                            Description = p.Pet.Description
                        }
                    })
                    .ToList();
                
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // GET: ShopProduct/{id} - Get product by ID
        [HttpGet("{id}")]
        public ActionResult<object> GetById(int id)
        {
            try
            {                var product = _context.ShopProducts
                    .Include(p => p.Shop)
                    .Include(p => p.Admin)
                    .Include(p => p.Pet)
                    .FirstOrDefault(p => p.ShopProductId == id);
                
                if (product == null)
                {
                    return NotFound();
                }
                
                var result = new
                {                    ShopProductId = product.ShopProductId,
                    ShopId = product.ShopId,
                    AdminId = product.AdminId,
                    PetId = product.PetId,
                    Name = product.Name,
                    Type = product.Type,
                    Description = product.Description,                    ImageUrl = product.ImageUrl,
                    Price = product.Price,
                    CurrencyType = product.CurrencyType,
                    Status = product.Status,
                    ShopInfo = new
                    {
                        ShopId = product.Shop.ShopId,
                        Name = product.Shop.Name,
                        Type = product.Shop.Type
                    },
                    AdminInfo = new
                    {
                        Id = product.Admin.Id,
                        UserName = product.Admin.UserName
                    },
                    PetInfo = product.Pet == null ? null : new
                    {
                        PetId = product.Pet.PetId,
                        PetType = product.Pet.PetType,
                        PetDefaultName = product.Pet.PetDefaultName,
                        Description = product.Pet.Description
                    }
                };
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // GET: ShopProduct/Type/{type} - Get products by type
        [HttpGet("Type/{type}")]
        public ActionResult<IEnumerable<object>> GetByType(string type)
        {
            try
            {
                if (string.IsNullOrEmpty(type))
                {
                    return BadRequest("Type is required");
                }
                
                var products = _context.ShopProducts
                    .Include(p => p.Shop)
                    .Include(p => p.Admin)
                    .Include(p => p.Pet)
                    .Where(p => p.Type == type)
                    .Select(p => new
                    {
                        ShopProductId = p.ShopProductId,
                        ShopId = p.ShopId,
                        AdminId = p.AdminId,
                        PetId = p.PetId,
                        Name = p.Name,
                        Type = p.Type,
                        Description = p.Description,
                        ImageUrl = p.ImageUrl,
                        Price = p.Price,
                        CurrencyType = p.CurrencyType,
                        Status = p.Status,
                        ShopInfo = new
                        {
                            ShopId = p.Shop.ShopId,
                            Name = p.Shop.Name,
                            Type = p.Shop.Type
                        },
                        AdminInfo = new
                        {
                            Id = p.Admin.Id,
                            UserName = p.Admin.UserName
                        },
                        PetInfo = p.Pet == null ? null : new
                        {
                            PetId = p.Pet.PetId,
                            PetType = p.Pet.PetType,
                            PetDefaultName = p.Pet.PetDefaultName,
                            Description = p.Pet.Description
                        }
                    })
                    .ToList();
                    
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
          // GET: ShopProduct/Status/{status} - Get products by status
        [HttpGet("Status/{status}")]
        public ActionResult<IEnumerable<object>> GetByStatus(int status)
        {
            try
            {
                var products = _context.ShopProducts
                    .Include(p => p.Shop)
                    .Include(p => p.Admin)
                    .Include(p => p.Pet)
                    .Where(p => p.Status == status)
                    .Select(p => new
                    {
                        ShopProductId = p.ShopProductId,
                        ShopId = p.ShopId,
                        AdminId = p.AdminId,
                        PetId = p.PetId,
                        Name = p.Name,
                        Type = p.Type,
                        Description = p.Description,
                        ImageUrl = p.ImageUrl,
                        Price = p.Price,
                        CurrencyType = p.CurrencyType,
                        Status = p.Status,
                        ShopInfo = new
                        {
                            ShopId = p.Shop.ShopId,
                            Name = p.Shop.Name,
                            Type = p.Shop.Type
                        },
                        AdminInfo = new
                        {
                            Id = p.Admin.Id,
                            UserName = p.Admin.UserName
                        },
                        PetInfo = p.Pet == null ? null : new
                        {
                            PetId = p.Pet.PetId,
                            PetType = p.Pet.PetType,
                            PetDefaultName = p.Pet.PetDefaultName,
                            Description = p.Pet.Description
                        }
                    })
                    .ToList();
                    
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
          // POST: ShopProduct - Create a new product
        [HttpPost]        public ActionResult<ShopProduct> Create(int shopId, int adminId, string name, string type, string? description, 
            string? imageUrl, int price, string currencyType, int? status, int? petId)
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
            
            // Validate pet exists if PetId is provided
            if (petId.HasValue)
            {
                var pet = _context.Pets.Find(petId.Value);
                if (pet == null)
                {
                    return BadRequest("Pet not found");
                }
            }
              var product = new ShopProduct
            {
                ShopId = shopId,
                AdminId = adminId,
                PetId = petId,
                Name = name,
                Type = type,
                Description = description,                ImageUrl = imageUrl,
                Price = price,
                CurrencyType = currencyType,
                Status = status ?? 1 // Default to 1 (active) if not provided
            };
            
            _context.ShopProducts.Add(product);
            _context.SaveChanges();
            
            return CreatedAtAction(nameof(GetById), new { id = product.ShopProductId }, product);
        }          // PUT: ShopProduct/{id} - Update a product
        [HttpPut("{id}")]        public ActionResult<ShopProduct> Update(int id, string? name, string? type, string? description, 
            string? imageUrl, int? price, string? currencyType, int? status, int? petId)
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
              if (status.HasValue)
            {
                product.Status = status.Value;
            }
            
            if (petId.HasValue)
            {
                // Validate pet exists if petId is provided
                var pet = _context.Pets.Find(petId.Value);
                if (pet == null)
                {
                    return BadRequest("Pet not found");
                }
                product.PetId = petId;
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
        
        // PUT: ShopProduct/{id}/Quantity - Update product quantity
        [HttpPut("{id}/Quantity")]
        public IActionResult UpdateProductQuantity(int id, [FromBody] int quantity)
        {
            try
            {
                var product = _context.ShopProducts.Find(id);
                
                if (product == null)
                {
                    return NotFound("Product not found");
                }
                
                if (quantity < 0)
                {
                    return BadRequest("Quantity cannot be negative");
                }
                
                product.Quantity = quantity;
                _context.SaveChanges();
                
                return Ok(new { message = "Product quantity updated successfully", newQuantity = quantity });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // GET: ShopProduct/InStock - Get products that are in stock (quantity > 0)
        [HttpGet("InStock")]
        public ActionResult<IEnumerable<object>> GetProductsInStock()
        {
            try
            {
                var products = _context.ShopProducts
                    .Include(p => p.Shop)
                    .Include(p => p.Admin)
                    .Include(p => p.Pet)
                    .Where(p => p.Quantity > 0 && p.Status == 1)
                    .Select(p => new
                    {
                        ShopProductId = p.ShopProductId,
                        Name = p.Name,
                        Type = p.Type,
                        Description = p.Description,
                        ImageUrl = p.ImageUrl,
                        Price = p.Price,
                        CurrencyType = p.CurrencyType,
                        Quantity = p.Quantity,
                        Status = p.Status,
                        ShopName = p.Shop.Name,
                        PetInfo = p.Pet == null ? null : new
                        {
                            PetType = p.Pet.PetType,
                            PetDefaultName = p.Pet.PetDefaultName
                        }
                    })
                    .OrderBy(p => p.Name)
                    .ToList();
                
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // GET: ShopProduct/OutOfStock - Get products that are out of stock (quantity = 0)
        [HttpGet("OutOfStock")]
        public ActionResult<IEnumerable<object>> GetProductsOutOfStock()
        {
            try
            {
                var products = _context.ShopProducts
                    .Include(p => p.Shop)
                    .Include(p => p.Admin)
                    .Where(p => p.Quantity == 0 || p.Quantity == null)
                    .Select(p => new
                    {
                        ShopProductId = p.ShopProductId,
                        Name = p.Name,
                        Type = p.Type,
                        Price = p.Price,
                        CurrencyType = p.CurrencyType,
                        Quantity = p.Quantity,
                        Status = p.Status,
                        ShopName = p.Shop.Name
                    })
                    .OrderBy(p => p.Name)
                    .ToList();
                
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // POST: ShopProduct/{id}/Purchase - Handle product purchase (decreases quantity)
        [HttpPost("{id}/Purchase")]
        public IActionResult PurchaseProduct(int id, [FromBody] int quantityToPurchase = 1)
        {
            try
            {
                var product = _context.ShopProducts.Find(id);
                
                if (product == null)
                {
                    return NotFound("Product not found");
                }
                
                if (product.Status != 1)
                {
                    return BadRequest("Product is not available for purchase");
                }
                
                if (product.Quantity == null || product.Quantity < quantityToPurchase)
                {
                    return BadRequest("Insufficient stock available");
                }
                
                product.Quantity -= quantityToPurchase;
                _context.SaveChanges();
                
                return Ok(new 
                { 
                    message = "Product purchased successfully", 
                    quantityPurchased = quantityToPurchase,
                    remainingStock = product.Quantity 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // GET: ShopProduct/Pet/{petId} - Get products by pet ID
        [HttpGet("Pet/{petId}")]
        public ActionResult<IEnumerable<object>> GetByPetId(int petId)
        {
            try
            {
                var products = _context.ShopProducts
                    .Include(p => p.Shop)
                    .Include(p => p.Admin)
                    .Include(p => p.Pet)
                    .Where(p => p.PetId == petId)
                    .Select(p => new
                    {
                        ShopProductId = p.ShopProductId,
                        ShopId = p.ShopId,
                        AdminId = p.AdminId,
                        PetId = p.PetId,
                        Name = p.Name,
                        Type = p.Type,
                        Description = p.Description,
                        ImageUrl = p.ImageUrl,
                        Price = p.Price,
                        CurrencyType = p.CurrencyType,
                        Status = p.Status,
                        ShopInfo = new
                        {
                            ShopId = p.Shop.ShopId,
                            Name = p.Shop.Name,
                            Type = p.Shop.Type
                        },
                        AdminInfo = new
                        {
                            Id = p.Admin.Id,
                            UserName = p.Admin.UserName
                        },
                        PetInfo = p.Pet == null ? null : new
                        {
                            PetId = p.Pet.PetId,
                            PetType = p.Pet.PetType,
                            PetDefaultName = p.Pet.PetDefaultName,
                            Description = p.Pet.Description
                        }
                    })
                    .ToList();
                
                if (!products.Any())
                {
                    return Ok(new { Message = "No products found for this pet", Products = new List<object>() });
                }
                    
                return Ok(new { Message = $"Found {products.Count} product(s) for pet ID {petId}", Products = products });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
