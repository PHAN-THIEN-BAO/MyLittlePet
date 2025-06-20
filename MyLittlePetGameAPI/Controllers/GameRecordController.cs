using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyLittlePetGameAPI.Models;

namespace MyLittlePetGameAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameRecordController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public GameRecordController(AppDbContext context)
        {
            _context = context;
        }
        
        // GET: GameRecord - Get all game records
        [HttpGet]
        public ActionResult<IEnumerable<object>> Get()
        {
            try
            {
                var records = _context.GameRecords
                    .Include(gr => gr.Player)
                    .Include(gr => gr.Minigame)
                    .Select(gr => new
                    {
                        PlayerId = gr.PlayerId,
                        MinigameId = gr.MinigameId,
                        Score = gr.Score,
                        PlayedAt = gr.PlayedAt,
                        PlayerInfo = new
                        {
                            Id = gr.Player.Id,
                            UserName = gr.Player.UserName
                        },
                        MinigameInfo = new
                        {
                            MinigameId = gr.Minigame.MinigameId,
                            Name = gr.Minigame.Name
                        }
                    })
                    .ToList();
                
                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // GET: GameRecord/Player/{playerId} - Get game records for a specific player
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
                
                var records = _context.GameRecords
                    .Include(gr => gr.Minigame)
                    .Where(gr => gr.PlayerId == playerId)
                    .OrderByDescending(gr => gr.PlayedAt)
                    .Select(gr => new
                    {
                        PlayerId = gr.PlayerId,
                        MinigameId = gr.MinigameId,
                        Score = gr.Score,
                        PlayedAt = gr.PlayedAt,
                        MinigameInfo = new
                        {
                            MinigameId = gr.Minigame.MinigameId,
                            Name = gr.Minigame.Name
                        }
                    })
                    .ToList();
                    
                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // GET: GameRecord/Minigame/{minigameId} - Get game records for a specific minigame
        [HttpGet("Minigame/{minigameId}")]
        public ActionResult<IEnumerable<object>> GetByMinigameId(int minigameId)
        {
            try
            {
                var minigame = _context.Minigames.Find(minigameId);
                if (minigame == null)
                {
                    return NotFound("Minigame not found");
                }
                
                var records = _context.GameRecords
                    .Include(gr => gr.Player)
                    .Where(gr => gr.MinigameId == minigameId)
                    .OrderByDescending(gr => gr.Score)
                    .ThenByDescending(gr => gr.PlayedAt)
                    .Select(gr => new
                    {
                        PlayerId = gr.PlayerId,
                        MinigameId = gr.MinigameId,
                        Score = gr.Score,
                        PlayedAt = gr.PlayedAt,
                        PlayerInfo = new
                        {
                            Id = gr.Player.Id,
                            UserName = gr.Player.UserName
                        }
                    })
                    .ToList();
                    
                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // GET: GameRecord/Leaderboard/{minigameId} - Get leaderboard for a specific minigame
        [HttpGet("Leaderboard/{minigameId}")]
        public ActionResult<IEnumerable<object>> GetLeaderboard(int minigameId)
        {
            try
            {
                var minigame = _context.Minigames.Find(minigameId);
                if (minigame == null)
                {
                    return NotFound("Minigame not found");
                }
                
                var leaderboard = _context.GameRecords
                    .Include(gr => gr.Player)
                    .Where(gr => gr.MinigameId == minigameId)
                    .GroupBy(gr => gr.PlayerId)
                    .Select(group => group.OrderByDescending(gr => gr.Score).First())
                    .OrderByDescending(gr => gr.Score)
                    .Select(gr => new
                    {
                        PlayerId = gr.PlayerId,
                        MinigameId = gr.MinigameId,
                        Score = gr.Score,
                        PlayedAt = gr.PlayedAt,
                        PlayerInfo = new
                        {
                            Id = gr.Player.Id,
                            UserName = gr.Player.UserName
                        }
                    })
                    .ToList();
                    
                return Ok(leaderboard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        // POST: GameRecord - Create a new game record
        [HttpPost]
        public ActionResult<GameRecord> Create(int playerId, int minigameId, int score)
        {
            // Validate player exists
            var player = _context.Users.Find(playerId);
            if (player == null)
            {
                return BadRequest("Player not found");
            }
            
            // Validate minigame exists
            var minigame = _context.Minigames.Find(minigameId);
            if (minigame == null)
            {
                return BadRequest("Minigame not found");
            }
            
            // Check if player already has a record for this minigame
            var existingRecord = _context.GameRecords
                .FirstOrDefault(gr => gr.PlayerId == playerId && gr.MinigameId == minigameId);
                
            if (existingRecord != null)
            {
                // Update record if new score is higher
                if (score > existingRecord.Score)
                {
                    existingRecord.Score = score;
                    existingRecord.PlayedAt = DateTime.Now;
                    _context.GameRecords.Update(existingRecord);
                    _context.SaveChanges();
                }
                return Ok(existingRecord);
            }
            
            // Create new record
            var gameRecord = new GameRecord
            {
                PlayerId = playerId,
                MinigameId = minigameId,
                Score = score,
                PlayedAt = DateTime.Now
            };
            
            _context.GameRecords.Add(gameRecord);
            _context.SaveChanges();
            
            return CreatedAtAction(nameof(GetByPlayerId), new { playerId = playerId }, gameRecord);
        }
        
        // PUT: GameRecord - Update a game record
        [HttpPut]
        public ActionResult<GameRecord> Update(int playerId, int minigameId, int score)
        {
            var gameRecord = _context.GameRecords
                .FirstOrDefault(gr => gr.PlayerId == playerId && gr.MinigameId == minigameId);
                
            if (gameRecord == null)
            {
                return NotFound("Game record not found");
            }
            
            gameRecord.Score = score;
            gameRecord.PlayedAt = DateTime.Now;
            
            _context.GameRecords.Update(gameRecord);
            _context.SaveChanges();
            
            return Ok(gameRecord);
        }
        
        // DELETE: GameRecord - Delete a game record
        [HttpDelete]
        public ActionResult Delete(int playerId, int minigameId)
        {
            var gameRecord = _context.GameRecords
                .FirstOrDefault(gr => gr.PlayerId == playerId && gr.MinigameId == minigameId);
                
            if (gameRecord == null)
            {
                return NotFound("Game record not found");
            }
            
            _context.GameRecords.Remove(gameRecord);
            _context.SaveChanges();
            
            return NoContent();
        }
    }
}
