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
        
        // POST: PlayerInventory - Add item to player's inventory
        [HttpPost]
        public ActionResult<PlayerInventory> Create(int playerId, int shopProductId, int? quantity)
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
                return Ok(existingItem);
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
            
            return CreatedAtAction(nameof(GetByPlayerId), new { playerId = playerId }, inventoryItem);
        }
        
        // PUT: PlayerInventory - Update item quantity
        [HttpPut]
        public ActionResult<PlayerInventory> Update(int playerId, int shopProductId, int quantity)
        {
            // Validate quantity
            if (quantity < 0)
            {
                return BadRequest("Quantity cannot be negative");
            }
            
            // Find the inventory item
            var inventoryItem = _context.PlayerInventories
                .FirstOrDefault(i => i.PlayerId == playerId && i.ShopProductId == shopProductId);
                
            if (inventoryItem == null)
            {
                return NotFound("Item not found in player's inventory");
            }
            
            if (quantity == 0)
            {
                // Remove item if quantity is 0
                _context.PlayerInventories.Remove(inventoryItem);
                _context.SaveChanges();
                return NoContent();
            }
            
            // Update quantity
            inventoryItem.Quantity = quantity;
            _context.PlayerInventories.Update(inventoryItem);
            _context.SaveChanges();
            
            return Ok(inventoryItem);
        }
        
        // DELETE: PlayerInventory - Remove item from inventory
        [HttpDelete]
        public ActionResult Delete(int playerId, int shopProductId)
        {
            var inventoryItem = _context.PlayerInventories
                .FirstOrDefault(i => i.PlayerId == playerId && i.ShopProductId == shopProductId);
                
            if (inventoryItem == null)
            {
                return NotFound("Item not found in player's inventory");
            }
            
            _context.PlayerInventories.Remove(inventoryItem);
            _context.SaveChanges();
            
            return NoContent();
        }
    }
}
