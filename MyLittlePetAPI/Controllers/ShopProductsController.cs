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
    public class ShopProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ShopProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShopProductDTO>>> GetShopProducts()
        {
            var shopProducts = await _context.ShopProducts
                .Include(sp => sp.Shop)
                .Include(sp => sp.Admin)
                .ToListAsync();

            var shopProductDtos = shopProducts.Select(sp => new ShopProductDTO
            {
                ShopProductID = sp.ShopProductID,
                ShopID = sp.ShopID,
                AdminID = sp.AdminID,
                Name = sp.Name,
                Type = sp.Type,
                Description = sp.Description,
                ImageUrl = sp.ImageUrl,
                Price = sp.Price,
                CurrencyType = sp.CurrencyType,
                Quality = sp.Quality,
                ShopName = sp.Shop.Name,
                AdminName = sp.Admin.UserName
            }).ToList();

            return Ok(shopProductDtos);
        }

        [HttpGet("shop/{shopId}")]
        public async Task<ActionResult<IEnumerable<ShopProductDTO>>> GetShopProductsByShop(int shopId)
        {
            var shopProducts = await _context.ShopProducts
                .Where(sp => sp.ShopID == shopId)
                .Include(sp => sp.Shop)
                .Include(sp => sp.Admin)
                .ToListAsync();

            var shopProductDtos = shopProducts.Select(sp => new ShopProductDTO
            {
                ShopProductID = sp.ShopProductID,
                ShopID = sp.ShopID,
                AdminID = sp.AdminID,
                Name = sp.Name,
                Type = sp.Type,
                Description = sp.Description,
                ImageUrl = sp.ImageUrl,
                Price = sp.Price,
                CurrencyType = sp.CurrencyType,
                Quality = sp.Quality,
                ShopName = sp.Shop.Name,
                AdminName = sp.Admin.UserName
            }).ToList();

            return Ok(shopProductDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShopProductDTO>> GetShopProduct(int id)
        {
            var shopProduct = await _context.ShopProducts
                .Include(sp => sp.Shop)
                .Include(sp => sp.Admin)
                .FirstOrDefaultAsync(sp => sp.ShopProductID == id);

            if (shopProduct == null)
            {
                return NotFound();
            }

            var shopProductDto = new ShopProductDTO
            {
                ShopProductID = shopProduct.ShopProductID,
                ShopID = shopProduct.ShopID,
                AdminID = shopProduct.AdminID,
                Name = shopProduct.Name,
                Type = shopProduct.Type,
                Description = shopProduct.Description,
                ImageUrl = shopProduct.ImageUrl,
                Price = shopProduct.Price,
                CurrencyType = shopProduct.CurrencyType,
                Quality = shopProduct.Quality,
                ShopName = shopProduct.Shop.Name,
                AdminName = shopProduct.Admin.UserName
            };

            return Ok(shopProductDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ShopProductDTO>> CreateShopProduct(CreateShopProductDTO createShopProductDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var shopProduct = new ShopProduct
            {
                ShopID = createShopProductDto.ShopID,
                AdminID = userId,
                Name = createShopProductDto.Name,
                Type = createShopProductDto.Type,
                Description = createShopProductDto.Description,
                ImageUrl = createShopProductDto.ImageUrl,
                Price = createShopProductDto.Price,
                CurrencyType = createShopProductDto.CurrencyType,
                Quality = createShopProductDto.Quality
            };

            _context.ShopProducts.Add(shopProduct);
            await _context.SaveChangesAsync();

            var shop = await _context.Shops.FindAsync(shopProduct.ShopID);
            var admin = await _context.Users.FindAsync(shopProduct.AdminID);

            var shopProductDto = new ShopProductDTO
            {
                ShopProductID = shopProduct.ShopProductID,
                ShopID = shopProduct.ShopID,
                AdminID = shopProduct.AdminID,
                Name = shopProduct.Name,
                Type = shopProduct.Type,
                Description = shopProduct.Description,
                ImageUrl = shopProduct.ImageUrl,
                Price = shopProduct.Price,
                CurrencyType = shopProduct.CurrencyType,
                Quality = shopProduct.Quality,
                ShopName = shop.Name,
                AdminName = admin.UserName
            };

            return CreatedAtAction(nameof(GetShopProduct), new { id = shopProduct.ShopProductID }, shopProductDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateShopProduct(int id, UpdateShopProductDTO updateShopProductDto)
        {
            var shopProduct = await _context.ShopProducts.FindAsync(id);

            if (shopProduct == null)
            {
                return NotFound();
            }

            if (updateShopProductDto.ShopID.HasValue)
            {
                shopProduct.ShopID = updateShopProductDto.ShopID.Value;
            }

            if (!string.IsNullOrEmpty(updateShopProductDto.Name))
            {
                shopProduct.Name = updateShopProductDto.Name;
            }

            if (!string.IsNullOrEmpty(updateShopProductDto.Type))
            {
                shopProduct.Type = updateShopProductDto.Type;
            }

            if (updateShopProductDto.Description != null)
            {
                shopProduct.Description = updateShopProductDto.Description;
            }

            if (updateShopProductDto.ImageUrl != null)
            {
                shopProduct.ImageUrl = updateShopProductDto.ImageUrl;
            }

            if (updateShopProductDto.Price.HasValue)
            {
                shopProduct.Price = updateShopProductDto.Price.Value;
            }

            if (!string.IsNullOrEmpty(updateShopProductDto.CurrencyType))
            {
                shopProduct.CurrencyType = updateShopProductDto.CurrencyType;
            }

            if (updateShopProductDto.Quality.HasValue)
            {
                shopProduct.Quality = updateShopProductDto.Quality.Value;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShopProductExists(id))
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteShopProduct(int id)
        {
            var shopProduct = await _context.ShopProducts.FindAsync(id);
            if (shopProduct == null)
            {
                return NotFound();
            }

            _context.ShopProducts.Remove(shopProduct);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ShopProductExists(int id)
        {
            return _context.ShopProducts.Any(e => e.ShopProductID == id);
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> PurchaseItem(PurchaseItemDTO purchaseItemDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user = await _context.Users.FindAsync(userId);
            
            if (user == null)
            {
                return NotFound("User not found");
            }

            var shopProduct = await _context.ShopProducts.FindAsync(purchaseItemDto.ShopProductID);
            
            if (shopProduct == null)
            {
                return NotFound("Product not found");
            }

            // Calculate total cost
            var totalCost = shopProduct.Price * purchaseItemDto.Quantity;

            // Check if user has enough currency
            if (shopProduct.CurrencyType.ToLower() == "coin")
            {
                if (user.Coin < totalCost)
                {
                    return BadRequest("Not enough coins");
                }
                user.Coin -= totalCost;
            }
            else if (shopProduct.CurrencyType.ToLower() == "diamond")
            {
                if (user.Diamond < totalCost)
                {
                    return BadRequest("Not enough diamonds");
                }
                user.Diamond -= totalCost;
            }
            else if (shopProduct.CurrencyType.ToLower() == "gem")
            {
                if (user.Gem < totalCost)
                {
                    return BadRequest("Not enough gems");
                }
                user.Gem -= totalCost;
            }
            else
            {
                return BadRequest("Invalid currency type");
            }

            // Check if player already has this item
            var existingInventory = await _context.PlayerInventories
                .FirstOrDefaultAsync(pi => pi.PlayerID == userId && pi.ShopProductID == purchaseItemDto.ShopProductID);

            if (existingInventory != null)
            {
                // Update quantity
                existingInventory.Quantity += purchaseItemDto.Quantity;
            }
            else
            {
                // Create new inventory entry
                var playerInventory = new PlayerInventory
                {
                    PlayerID = userId,
                    ShopProductID = purchaseItemDto.ShopProductID,
                    Quantity = purchaseItemDto.Quantity,
                    AcquiredAt = DateTime.Now
                };

                _context.PlayerInventories.Add(playerInventory);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Purchase successful" });
        }
    }
}
