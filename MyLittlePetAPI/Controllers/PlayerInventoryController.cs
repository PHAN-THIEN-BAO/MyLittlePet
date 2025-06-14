using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLittlePetAPI.Data;
using MyLittlePetAPI.DTOs;
using MyLittlePetAPI.Models;
using System.Security.Claims;

namespace MyLittlePetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlayerInventoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PlayerInventoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerInventoryDTO>>> GetPlayerInventory()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            IQueryable<PlayerInventory> query = _context.PlayerInventories
                .Include(pi => pi.Player)
                .Include(pi => pi.ShopProduct)
                .ThenInclude(sp => sp.Shop);

            // If not admin, only show the user's inventory
            if (userRole != "Admin")
            {
                query = query.Where(pi => pi.PlayerID == userId);
            }

            var playerInventories = await query.ToListAsync();

            var playerInventoryDtos = playerInventories.Select(pi => new PlayerInventoryDTO
            {
                PlayerID = pi.PlayerID,
                ShopProductID = pi.ShopProductID,
                Quantity = pi.Quantity,
                AcquiredAt = pi.AcquiredAt,
                PlayerName = pi.Player.UserName,
                ShopProduct = new ShopProductDTO
                {
                    ShopProductID = pi.ShopProduct.ShopProductID,
                    ShopID = pi.ShopProduct.ShopID,
                    AdminID = pi.ShopProduct.AdminID,
                    Name = pi.ShopProduct.Name,
                    Type = pi.ShopProduct.Type,
                    Description = pi.ShopProduct.Description,
                    ImageUrl = pi.ShopProduct.ImageUrl,
                    Price = pi.ShopProduct.Price,
                    CurrencyType = pi.ShopProduct.CurrencyType,
                    Quality = pi.ShopProduct.Quality,
                    ShopName = pi.ShopProduct.Shop.Name,
                    AdminName = pi.ShopProduct.Admin.UserName
                }
            }).ToList();

            return Ok(playerInventoryDtos);
        }

        [HttpGet("player/{playerId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<PlayerInventoryDTO>>> GetPlayerInventoryByPlayer(int playerId)
        {
            var player = await _context.Users.FindAsync(playerId);
            if (player == null)
            {
                return NotFound("Player not found");
            }

            var playerInventories = await _context.PlayerInventories
                .Where(pi => pi.PlayerID == playerId)
                .Include(pi => pi.Player)
                .Include(pi => pi.ShopProduct)
                .ThenInclude(sp => sp.Shop)
                .Include(pi => pi.ShopProduct.Admin)
                .ToListAsync();

            var playerInventoryDtos = playerInventories.Select(pi => new PlayerInventoryDTO
            {
                PlayerID = pi.PlayerID,
                ShopProductID = pi.ShopProductID,
                Quantity = pi.Quantity,
                AcquiredAt = pi.AcquiredAt,
                PlayerName = pi.Player.UserName,
                ShopProduct = new ShopProductDTO
                {
                    ShopProductID = pi.ShopProduct.ShopProductID,
                    ShopID = pi.ShopProduct.ShopID,
                    AdminID = pi.ShopProduct.AdminID,
                    Name = pi.ShopProduct.Name,
                    Type = pi.ShopProduct.Type,
                    Description = pi.ShopProduct.Description,
                    ImageUrl = pi.ShopProduct.ImageUrl,
                    Price = pi.ShopProduct.Price,
                    CurrencyType = pi.ShopProduct.CurrencyType,
                    Quality = pi.ShopProduct.Quality,
                    ShopName = pi.ShopProduct.Shop.Name,
                    AdminName = pi.ShopProduct.Admin.UserName
                }
            }).ToList();

            return Ok(playerInventoryDtos);
        }

        [HttpGet("{playerId}/{shopProductId}")]
        public async Task<ActionResult<PlayerInventoryDTO>> GetPlayerInventoryItem(int playerId, int shopProductId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Only allow users to view their own inventory items unless they're an admin
            if (playerId != userId && userRole != "Admin")
            {
                return Forbid();
            }

            var playerInventory = await _context.PlayerInventories
                .Include(pi => pi.Player)
                .Include(pi => pi.ShopProduct)
                .ThenInclude(sp => sp.Shop)
                .Include(pi => pi.ShopProduct.Admin)
                .FirstOrDefaultAsync(pi => pi.PlayerID == playerId && pi.ShopProductID == shopProductId);

            if (playerInventory == null)
            {
                return NotFound();
            }

            var playerInventoryDto = new PlayerInventoryDTO
            {
                PlayerID = playerInventory.PlayerID,
                ShopProductID = playerInventory.ShopProductID,
                Quantity = playerInventory.Quantity,
                AcquiredAt = playerInventory.AcquiredAt,
                PlayerName = playerInventory.Player.UserName,
                ShopProduct = new ShopProductDTO
                {
                    ShopProductID = playerInventory.ShopProduct.ShopProductID,
                    ShopID = playerInventory.ShopProduct.ShopID,
                    AdminID = playerInventory.ShopProduct.AdminID,
                    Name = playerInventory.ShopProduct.Name,
                    Type = playerInventory.ShopProduct.Type,
                    Description = playerInventory.ShopProduct.Description,
                    ImageUrl = playerInventory.ShopProduct.ImageUrl,
                    Price = playerInventory.ShopProduct.Price,
                    CurrencyType = playerInventory.ShopProduct.CurrencyType,
                    Quality = playerInventory.ShopProduct.Quality,
                    ShopName = playerInventory.ShopProduct.Shop.Name,
                    AdminName = playerInventory.ShopProduct.Admin.UserName
                }
            };

            return Ok(playerInventoryDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PlayerInventoryDTO>> CreatePlayerInventory(CreatePlayerInventoryDTO createPlayerInventoryDto)
        {
            // Check if player exists
            var player = await _context.Users.FindAsync(createPlayerInventoryDto.PlayerID);
            if (player == null)
            {
                return NotFound("Player not found");
            }

            // Check if shop product exists
            var shopProduct = await _context.ShopProducts.FindAsync(createPlayerInventoryDto.ShopProductID);
            if (shopProduct == null)
            {
                return NotFound("Shop product not found");
            }

            // Check if player already has this item
            var existingInventory = await _context.PlayerInventories
                .FirstOrDefaultAsync(pi => pi.PlayerID == createPlayerInventoryDto.PlayerID 
                                        && pi.ShopProductID == createPlayerInventoryDto.ShopProductID);

            if (existingInventory != null)
            {
                // Update quantity
                existingInventory.Quantity += createPlayerInventoryDto.Quantity;
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetPlayerInventoryItem), 
                    new { playerId = existingInventory.PlayerID, shopProductId = existingInventory.ShopProductID }, 
                    "Inventory item quantity updated");
            }

            // Create new inventory entry
            var playerInventory = new PlayerInventory
            {
                PlayerID = createPlayerInventoryDto.PlayerID,
                ShopProductID = createPlayerInventoryDto.ShopProductID,
                Quantity = createPlayerInventoryDto.Quantity,
                AcquiredAt = DateTime.Now
            };

            _context.PlayerInventories.Add(playerInventory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlayerInventoryItem), 
                new { playerId = playerInventory.PlayerID, shopProductId = playerInventory.ShopProductID }, 
                "Inventory item created");
        }

        [HttpPut("{playerId}/{shopProductId}")]
        public async Task<IActionResult> UpdatePlayerInventory(int playerId, int shopProductId, UpdatePlayerInventoryDTO updatePlayerInventoryDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Only allow users to update their own inventory unless they're an admin
            if (playerId != userId && userRole != "Admin")
            {
                return Forbid();
            }

            var playerInventory = await _context.PlayerInventories
                .FirstOrDefaultAsync(pi => pi.PlayerID == playerId && pi.ShopProductID == shopProductId);

            if (playerInventory == null)
            {
                return NotFound();
            }

            playerInventory.Quantity = updatePlayerInventoryDto.Quantity;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerInventoryExists(playerId, shopProductId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{playerId}/{shopProductId}")]
        public async Task<IActionResult> DeletePlayerInventory(int playerId, int shopProductId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Only allow users to delete their own inventory items unless they're an admin
            if (playerId != userId && userRole != "Admin")
            {
                return Forbid();
            }

            var playerInventory = await _context.PlayerInventories
                .FirstOrDefaultAsync(pi => pi.PlayerID == playerId && pi.ShopProductID == shopProductId);

            if (playerInventory == null)
            {
                return NotFound();
            }

            _context.PlayerInventories.Remove(playerInventory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlayerInventoryExists(int playerId, int shopProductId)
        {
            return _context.PlayerInventories.Any(pi => pi.PlayerID == playerId && pi.ShopProductID == shopProductId);
        }
    }
}
