using System.ComponentModel.DataAnnotations;

namespace MyLittlePetAPI.DTOs
{
    public class PetDTO
    {
        public int PetID { get; set; }
        
        public int? AdminID { get; set; }
        
        public string PetType { get; set; }
        
        public string Description { get; set; }
        
        public string AdminName { get; set; }
    }

    public class CreatePetDTO
    {
        [Required]
        [StringLength(50)]
        public string PetType { get; set; }
        
        public string Description { get; set; }
    }

    public class UpdatePetDTO
    {
        [StringLength(50)]
        public string PetType { get; set; }
        
        public string Description { get; set; }
    }

    public class PlayerPetDTO
    {
        public int PlayerPetID { get; set; }
        
        public int PlayerID { get; set; }
        
        public int PetID { get; set; }
        
        public string PetName { get; set; }
        
        public DateTime AdoptedAt { get; set; }
        
        public int Level { get; set; }
        
        public string Status { get; set; }
        
        public DateTime LastStatusUpdate { get; set; }
        
        public string PlayerName { get; set; }
        
        public PetDTO Pet { get; set; }
    }

    public class CreatePlayerPetDTO
    {
        [Required]
        public int PetID { get; set; }
        
        [Required]
        [StringLength(50)]
        public string PetName { get; set; }
    }

    public class UpdatePlayerPetDTO
    {
        [StringLength(50)]
        public string PetName { get; set; }
        
        public string Status { get; set; }
    }
}
