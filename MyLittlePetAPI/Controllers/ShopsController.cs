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
    public class ShopsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ShopsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShopDTO>>> GetShops()
        {
            var shops = await _context.Shops.ToListAsync();
            var shopDtos = shops.Select(shop => new ShopDTO
            {
                ShopID = shop.ShopID,
                Name = shop.Name,
                Type = shop.Type,
                Description = shop.Description
            }).ToList();

            return Ok(shopDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShopDTO>> GetShop(int id)
        {
            var shop = await _context.Shops.FindAsync(id);

            if (shop == null)
            {
                return NotFound();
            }

            var shopDto = new ShopDTO
            {
                ShopID = shop.ShopID,
                Name = shop.Name,
                Type = shop.Type,
                Description = shop.Description
            };

            return Ok(shopDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ShopDTO>> CreateShop(CreateShopDTO createShopDto)
        {
            var shop = new Shop
            {
                Name = createShopDto.Name,
                Type = createShopDto.Type,
                Description = createShopDto.Description
            };

            _context.Shops.Add(shop);
            await _context.SaveChangesAsync();

            var shopDto = new ShopDTO
            {
                ShopID = shop.ShopID,
                Name = shop.Name,
                Type = shop.Type,
                Description = shop.Description
            };

            return CreatedAtAction(nameof(GetShop), new { id = shop.ShopID }, shopDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateShop(int id, UpdateShopDTO updateShopDto)
        {
            var shop = await _context.Shops.FindAsync(id);

            if (shop == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(updateShopDto.Name))
            {
                shop.Name = updateShopDto.Name;
            }

            if (!string.IsNullOrEmpty(updateShopDto.Type))
            {
                shop.Type = updateShopDto.Type;
            }

            if (updateShopDto.Description != null)
            {
                shop.Description = updateShopDto.Description;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShopExists(id))
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
        public async Task<IActionResult> DeleteShop(int id)
        {
            var shop = await _context.Shops.FindAsync(id);
            if (shop == null)
            {
                return NotFound();
            }

            _context.Shops.Remove(shop);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ShopExists(int id)
        {
            return _context.Shops.Any(e => e.ShopID == id);
        }
    }
}
