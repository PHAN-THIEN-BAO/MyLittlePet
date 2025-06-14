using System.ComponentModel.DataAnnotations;

namespace MyLittlePetAPI.DTOs
{
    public class CareActivityDTO
    {
        public int ActivityID { get; set; }
        
        public string ActivityType { get; set; }
        
        public string Description { get; set; }
    }

    public class CreateCareActivityDTO
    {
        [Required]
        [StringLength(50)]
        public string ActivityType { get; set; }
        
        public string Description { get; set; }
    }

    public class UpdateCareActivityDTO
    {
        [StringLength(50)]
        public string ActivityType { get; set; }
        
        public string Description { get; set; }
    }

    public class CareHistoryDTO
    {
        public int CareHistoryID { get; set; }
        
        public int PlayerPetID { get; set; }
        
        public int PlayerID { get; set; }
        
        public int ActivityID { get; set; }
        
        public DateTime PerformedAt { get; set; }
        
        public string PlayerName { get; set; }
        
        public string PetName { get; set; }
        
        public CareActivityDTO Activity { get; set; }
    }

    public class CreateCareHistoryDTO
    {
        [Required]
        public int PlayerPetID { get; set; }
        
        [Required]
        public int ActivityID { get; set; }
    }
}
