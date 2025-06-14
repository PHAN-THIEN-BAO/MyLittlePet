using System.ComponentModel.DataAnnotations;

namespace MyLittlePetAPI.DTOs
{
    public class MinigameDTO
    {
        public int MinigameID { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
    }

    public class CreateMinigameDTO
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        public string Description { get; set; }
    }

    public class UpdateMinigameDTO
    {
        [StringLength(100)]
        public string Name { get; set; }
        
        public string Description { get; set; }
    }

    public class GameRecordDTO
    {
        public int PlayerID { get; set; }
        
        public int MinigameID { get; set; }
        
        public DateTime PlayedAt { get; set; }
        
        public int Score { get; set; }
        
        public string PlayerName { get; set; }
        
        public MinigameDTO Minigame { get; set; }
    }

    public class CreateGameRecordDTO
    {
        [Required]
        public int MinigameID { get; set; }
        
        [Required]
        public int Score { get; set; }
    }

    public class UpdateGameRecordDTO
    {
        public int Score { get; set; }
    }
}
