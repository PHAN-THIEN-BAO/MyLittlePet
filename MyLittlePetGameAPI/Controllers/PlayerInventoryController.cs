using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLittlePetGameAPI.Models;

namespace MyLittlePetGameAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerInventoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public PlayerInventoryController(AppDbContext context)
        {
            _context = context;
        }
        
        // GET: PlayerInventory - Get all player inventory items
        [HttpGet]
        public ActionResult<IEnumerable<object>> Get()
        {
            try
            {
                var inventoryItems = _context.PlayerInventories
                    .Include(i => i.Player)
                    .Include(i => i.ShopProduct)
                    .Select(i => new
                    {
                        PlayerId = i.PlayerId,
                        ShopProductId = i.ShopProductId,
                        Quantity = i.Quantity,
                        AcquiredAt = i.AcquiredAt,
                        PlayerInfo = new
                        {
                            Id = i.Player.Id,
                            UserName = i.Player.UserName
                        },
                        ProductInfo = new
                        {
                            Id = i.ShopProduct.ShopProductId,
                            Name = i.ShopProduct.Name,
                            Type = i.ShopProduct.Type,
                            Description = i.ShopProduct.Description,
                            Price = i.ShopProduct.Price,
                            CurrencyType = i.ShopProduct.CurrencyType,
                            ImageUrl = i.ShopProduct.ImageUrl
                        }
                    })
                    .ToList();
                
                return Ok(inventoryItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // GET: PlayerInventory/Player/{playerId} - Get inventory for a specific player
        [HttpGet("Player/{playerId}")]
        public ActionResult<IEnumerable<object>> GetByPlayerId(int playerId)
        {
            try
            {
                var player = _context.Users.Find(playerId);
                if (player == null)
                {
                    return NotFound("Player not found");
                }
                
                var inventory = _context.PlayerInventories
                    .Include(i => i.ShopProduct)
                    .Where(i => i.PlayerId == playerId)
                    .Select(i => new
                    {
                        PlayerId = i.PlayerId,
                        ShopProductId = i.ShopProductId,
                        Quantity = i.Quantity,
                        AcquiredAt = i.AcquiredAt,
                        ProductInfo = new
                        {
                            Id = i.ShopProduct.ShopProductId,
                            Name = i.ShopProduct.Name,
                            Type = i.ShopProduct.Type,
                            Description = i.ShopProduct.Description,
                            Price = i.ShopProduct.Price,
                            CurrencyType = i.ShopProduct.CurrencyType,
                            ImageUrl = i.ShopProduct.ImageUrl
                        }
                    })
                    .ToList();
                    
                return Ok(inventory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // GET: PlayerInventory/FoodItems/{playerId} - Get food items for a specific player
        [HttpGet("FoodItems/{playerId}")]
        public ActionResult<IEnumerable<object>> GetFoodItemsByPlayerId(int playerId)
        {
            try
            {
                var player = _context.Users.Find(playerId);
                if (player == null)
                {
                    return NotFound("Player not found");
                }
                
                var foodItems = _context.PlayerInventories
                    .Include(i => i.ShopProduct)
                    .Where(i => i.PlayerId == playerId && i.ShopProduct.Type.ToLower() == "food")
                    .Select(i => new
                    {
                        PlayerId = i.PlayerId,
                        ShopProductId = i.ShopProductId,
                        Quantity = i.Quantity,
                        AcquiredAt = i.AcquiredAt,
                        ProductInfo = new
                        {
                            Id = i.ShopProduct.ShopProductId,
                            Name = i.ShopProduct.Name,
                            Type = i.ShopProduct.Type,
                            Description = i.ShopProduct.Description,
                            ImageUrl = i.ShopProduct.ImageUrl
                        }
                    })
                    .ToList();
                    
                return Ok(foodItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // POST: PlayerInventory - Add item to player's inventory
        [HttpPost]
        public ActionResult<PlayerInventory> Create(int playerId, int shopProductId, int? quantity)
        {
            try
            {
                // Validate player exists
                var player = _context.Users.Find(playerId);
                if (player == null)
                {
                    return BadRequest("Player not found");
                }
                
                // Validate product exists
                var product = _context.ShopProducts.Find(shopProductId);
                if (product == null)
                {
                    return BadRequest("Shop product not found");
                }
                
                // Check if player already has this item
                var existingItem = _context.PlayerInventories
                    .FirstOrDefault(i => i.PlayerId == playerId && i.ShopProductId == shopProductId);
                    
                if (existingItem != null)
                {
                    // Update quantity instead of adding new item
                    existingItem.Quantity += quantity ?? 1;
                    _context.PlayerInventories.Update(existingItem);
                    _context.SaveChanges();
                    
                    // Return simplified response to avoid serialization issues
                    return Ok(new {
                        message = "Item quantity updated successfully",
                        inventoryItem = new {
                            playerId = existingItem.PlayerId,
                            shopProductId = existingItem.ShopProductId,
                            quantity = existingItem.Quantity,
                            acquiredAt = existingItem.AcquiredAt
                        }
                    });
                }
                
                // Add new inventory item
                var inventoryItem = new PlayerInventory
                {
                    PlayerId = playerId,
                    ShopProductId = shopProductId,
                    Quantity = quantity ?? 1,
                    AcquiredAt = DateTime.Now
                };
                
                _context.PlayerInventories.Add(inventoryItem);
                _context.SaveChanges();
                
                // Return simplified response to avoid serialization issues
                return Ok(new {
                    message = "Item added to inventory successfully",
                    inventoryItem = new {
                        playerId = inventoryItem.PlayerId,
                        shopProductId = inventoryItem.ShopProductId,
                        quantity = inventoryItem.Quantity,
                        acquiredAt = inventoryItem.AcquiredAt
                    }
                });
            }
            catch (Exception ex)
            {
                // Log the exception but return a clean error message
                Console.WriteLine($"Error in Create method: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
          // PUT: PlayerInventory - Update item quantity
        [HttpPut]
        public ActionResult<PlayerInventory> Update(int playerId, int shopProductId, int quantity)
        {
            try
            {
                // Validate quantity
                if (quantity < 0)
                {
                    return BadRequest("Quantity cannot be negative");
                }
                
                // Find the inventory item
                var inventoryItem = _context.PlayerInventories
                    .FirstOrDefault(i => i.PlayerId == playerId && i.ShopProductId == shopProductId);
                
                // If the inventory item doesn't exist, create it (like POST method)
                if (inventoryItem == null)
                {
                    // Validate player exists
                    var player = _context.Users.Find(playerId);
                    if (player == null)
                    {
                        return BadRequest("Player not found");
                    }
                    
                    // Validate product exists
                    var product = _context.ShopProducts.Find(shopProductId);
                    if (product == null)
                    {
                        return BadRequest("Shop product not found");
                    }
                    
                    // Create a new inventory item
                    inventoryItem = new PlayerInventory
                    {
                        PlayerId = playerId,
                        ShopProductId = shopProductId,
                        Quantity = quantity,
                        AcquiredAt = DateTime.Now
                    };
                    
                    _context.PlayerInventories.Add(inventoryItem);
                    _context.SaveChanges();
                    
                    return Ok(new {
                        message = "Item added to inventory successfully",
                        inventoryItem = new {
                            playerId = inventoryItem.PlayerId,
                            shopProductId = inventoryItem.ShopProductId,
                            quantity = inventoryItem.Quantity,
                            acquiredAt = inventoryItem.AcquiredAt
                        }
                    });
                }
                
                if (quantity == 0)
                {
                    // Remove item if quantity is 0
                    _context.PlayerInventories.Remove(inventoryItem);
                    _context.SaveChanges();
                    return Ok(new { message = "Item removed from inventory as quantity was set to zero" });
                }
                
                // Update quantity - now we'll ADD the quantity instead of replacing it
                inventoryItem.Quantity += quantity;
                _context.PlayerInventories.Update(inventoryItem);
                _context.SaveChanges();
                
                // Return simplified response to avoid serialization issues
                return Ok(new {
                    message = "Item quantity updated successfully",
                    inventoryItem = new {
                        playerId = inventoryItem.PlayerId,
                        shopProductId = inventoryItem.ShopProductId,
                        quantity = inventoryItem.Quantity,
                        acquiredAt = inventoryItem.AcquiredAt
                    }
                });            }
            catch (Exception ex)
            {
                // Log the exception but return a clean error message
                Console.WriteLine($"Error in Update method: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
          // DELETE: PlayerInventory - Remove item from inventory
        [HttpDelete]
        public ActionResult Delete(int playerId, int shopProductId)
        {
            try
            {
                var inventoryItem = _context.PlayerInventories
                    .FirstOrDefault(i => i.PlayerId == playerId && i.ShopProductId == shopProductId);
                    
                if (inventoryItem == null)
                {
                    return NotFound("Item not found in player's inventory");
                }
                
                _context.PlayerInventories.Remove(inventoryItem);
                _context.SaveChanges();
                
                return Ok(new { message = "Item successfully removed from inventory" });
            }
            catch (Exception ex)
            {
                // Log the exception but return a clean error message
                Console.WriteLine($"Error in Delete method: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}
