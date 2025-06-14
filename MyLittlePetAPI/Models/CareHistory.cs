using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyLittlePetAPI.Models
{
    public class CareHistory
    {
        [Key]
        public int CareHistoryID { get; set; }
        
        public int PlayerPetID { get; set; }
        
        public int PlayerID { get; set; }
        
        public int ActivityID { get; set; }
        
        public DateTime PerformedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        [JsonIgnore]
        public virtual PlayerPet PlayerPet { get; set; }
        
        [JsonIgnore]
        public virtual User Player { get; set; }
        
        [JsonIgnore]
        public virtual CareActivity Activity { get; set; }
    }
}
