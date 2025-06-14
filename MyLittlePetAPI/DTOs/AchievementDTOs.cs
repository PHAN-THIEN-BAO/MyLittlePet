using System.ComponentModel.DataAnnotations;

namespace MyLittlePetAPI.DTOs
{
    public class AchievementDTO
    {
        public int AchievementID { get; set; }
        
        public string AchievementName { get; set; }
        
        public string Description { get; set; }
    }

    public class CreateAchievementDTO
    {
        [Required]
        [StringLength(100)]
        public string AchievementName { get; set; }
        
        public string Description { get; set; }
    }

    public class UpdateAchievementDTO
    {
        [StringLength(100)]
        public string AchievementName { get; set; }
        
        public string Description { get; set; }
    }

    public class PlayerAchievementDTO
    {
        public int PlayerID { get; set; }
        
        public int AchievementID { get; set; }
        
        public DateTime EarnedAt { get; set; }
        
        public string PlayerName { get; set; }
        
        public AchievementDTO Achievement { get; set; }
    }

    public class CreatePlayerAchievementDTO
    {
        [Required]
        public int PlayerID { get; set; }
        
        [Required]
        public int AchievementID { get; set; }
    }
}
