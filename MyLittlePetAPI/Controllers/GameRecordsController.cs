using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLittlePetAPI.Data;
using MyLittlePetAPI.DTOs;
using MyLittlePetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MyLittlePetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GameRecordsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GameRecordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/GameRecords
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<GameRecordDTO>>> GetAllGameRecords()
        {
            var gameRecords = await _context.GameRecords
                .Include(gr => gr.Player)
                .Include(gr => gr.Minigame)
                .Select(gr => new GameRecordDTO
                {
                    PlayerID = gr.PlayerID,
                    MinigameID = gr.MinigameID,
                    PlayedAt = gr.PlayedAt,
                    Score = gr.Score,
                    PlayerName = gr.Player.UserName,
                    Minigame = new MinigameDTO
                    {
                        MinigameID = gr.Minigame.MinigameID,
                        Name = gr.Minigame.Name,
                        Description = gr.Minigame.Description
                    }
                })
                .OrderByDescending(gr => gr.PlayedAt)
                .ToListAsync();

            return gameRecords;
        }

        // GET: api/GameRecords/user
        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<GameRecordDTO>>> GetUserGameRecords()
        {
            // Get the current user ID from the claims
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                return Unauthorized();
            }

            var gameRecords = await _context.GameRecords
                .Where(gr => gr.PlayerID == userId)
                .Include(gr => gr.Minigame)
                .Select(gr => new GameRecordDTO
                {
                    PlayerID = gr.PlayerID,
                    MinigameID = gr.MinigameID,
                    PlayedAt = gr.PlayedAt,
                    Score = gr.Score,
                    PlayerName = User.FindFirstValue(ClaimTypes.Name),
                    Minigame = new MinigameDTO
                    {
                        MinigameID = gr.Minigame.MinigameID,
                        Name = gr.Minigame.Name,
                        Description = gr.Minigame.Description
                    }
                })
                .OrderByDescending(gr => gr.PlayedAt)
                .ToListAsync();

            return gameRecords;
        }

        // GET: api/GameRecords/minigame/5
        [HttpGet("minigame/{minigameId}")]
        public async Task<ActionResult<IEnumerable<GameRecordDTO>>> GetMinigameRecords(int minigameId)
        {
            // Check if minigame exists
            var minigame = await _context.Minigames.FindAsync(minigameId);
            if (minigame == null)
            {
                return NotFound("Minigame not found");
            }

            var gameRecords = await _context.GameRecords
                .Where(gr => gr.MinigameID == minigameId)
                .Include(gr => gr.Player)
                .Select(gr => new GameRecordDTO
                {
                    PlayerID = gr.PlayerID,
                    MinigameID = gr.MinigameID,
                    PlayedAt = gr.PlayedAt,
                    Score = gr.Score,
                    PlayerName = gr.Player.UserName,
                    Minigame = new MinigameDTO
                    {
                        MinigameID = minigame.MinigameID,
                        Name = minigame.Name,
                        Description = minigame.Description
                    }
                })
                .OrderByDescending(gr => gr.Score) // Sort by highest score
                .Take(10) // Top 10 scores
                .ToListAsync();

            return gameRecords;
        }

        // GET: api/GameRecords/leaderboard/5
        [HttpGet("leaderboard/{minigameId}")]
        public async Task<ActionResult<IEnumerable<GameRecordDTO>>> GetLeaderboard(int minigameId)
        {
            // Check if minigame exists
            var minigame = await _context.Minigames.FindAsync(minigameId);
            if (minigame == null)
            {
                return NotFound("Minigame not found");
            }

            // Get top scores for each player (only the best score per player)
            var topScores = await _context.GameRecords
                .Where(gr => gr.MinigameID == minigameId)
                .GroupBy(gr => gr.PlayerID)
                .Select(g => g.OrderByDescending(gr => gr.Score).First()) // Get the highest score for each player
                .Include(gr => gr.Player)
                .Select(gr => new GameRecordDTO
                {
                    PlayerID = gr.PlayerID,
                    MinigameID = gr.MinigameID,
                    PlayedAt = gr.PlayedAt,
                    Score = gr.Score,
                    PlayerName = gr.Player.UserName,
                    Minigame = new MinigameDTO
                    {
                        MinigameID = minigame.MinigameID,
                        Name = minigame.Name,
                        Description = minigame.Description
                    }
                })
                .OrderByDescending(gr => gr.Score) // Sort by highest score
                .Take(10) // Top 10 players
                .ToListAsync();

            return topScores;
        }

        // POST: api/GameRecords
        [HttpPost]
        public async Task<ActionResult<GameRecordDTO>> CreateGameRecord(CreateGameRecordDTO createGameRecordDTO)
        {
            // Get the current user ID from the claims
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                return Unauthorized();
            }

            // Check if minigame exists
            var minigame = await _context.Minigames.FindAsync(createGameRecordDTO.MinigameID);
            if (minigame == null)
            {
                return NotFound("Minigame not found");
            }

            // Create the game record
            var gameRecord = new GameRecord
            {
                PlayerID = userId,
                MinigameID = createGameRecordDTO.MinigameID,
                PlayedAt = DateTime.Now,
                Score = createGameRecordDTO.Score
            };

            // Check if the player already has a record for this minigame
            var existingRecord = await _context.GameRecords
                .FirstOrDefaultAsync(gr => gr.PlayerID == userId && gr.MinigameID == createGameRecordDTO.MinigameID);

            if (existingRecord != null)
            {
                // Update the record if the new score is higher
                if (createGameRecordDTO.Score > existingRecord.Score)
                {
                    existingRecord.Score = createGameRecordDTO.Score;
                    existingRecord.PlayedAt = DateTime.Now;
                    
                    await _context.SaveChangesAsync();

                    // Award coins based on the new high score
                    await AwardCoinsForHighScore(userId, createGameRecordDTO.Score);
                }
                else
                {
                    // Still record the play, but as a new record
                    _context.GameRecords.Add(gameRecord);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // New record
                _context.GameRecords.Add(gameRecord);
                await _context.SaveChangesAsync();

                // Award coins for first play
                await AwardCoinsForHighScore(userId, createGameRecordDTO.Score);
            }

            // Return the created game record
            var result = new GameRecordDTO
            {
                PlayerID = gameRecord.PlayerID,
                MinigameID = gameRecord.MinigameID,
                PlayedAt = gameRecord.PlayedAt,
                Score = gameRecord.Score,
                PlayerName = User.FindFirstValue(ClaimTypes.Name),
                Minigame = new MinigameDTO
                {
                    MinigameID = minigame.MinigameID,
                    Name = minigame.Name,
                    Description = minigame.Description
                }
            };

            return CreatedAtAction("GetUserGameRecords", result);
        }

        // DELETE: api/GameRecords/{playerId}/{minigameId}
        [HttpDelete("{playerId}/{minigameId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteGameRecord(int playerId, int minigameId)
        {
            var gameRecord = await _context.GameRecords
                .FirstOrDefaultAsync(gr => gr.PlayerID == playerId && gr.MinigameID == minigameId);
                
            if (gameRecord == null)
            {
                return NotFound();
            }

            _context.GameRecords.Remove(gameRecord);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Helper method to award coins based on score
        private async Task AwardCoinsForHighScore(int userId, int score)
        {
            // Find the user
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                // Award coins based on score (customize this logic as needed)
                int coinsToAward = score / 10; // For example, 1 coin per 10 points
                
                // Ensure a minimum reward
                if (coinsToAward < 5)
                {
                    coinsToAward = 5;
                }
                
                // Cap the maximum reward
                if (coinsToAward > 100)
                {
                    coinsToAward = 100;
                }
                
                // Update user's coins
                user.Coin += coinsToAward;
                await _context.SaveChangesAsync();
            }
        }
    }
}
