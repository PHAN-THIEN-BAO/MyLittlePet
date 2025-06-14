using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyLittlePetAPI.Models
{
    public class CareActivity
    {
        [Key]
        public int ActivityID { get; set; }
        
        [Required]
        [StringLength(50)]
        public string ActivityType { get; set; }
        
        [Column(TypeName = "TEXT")]
        public string Description { get; set; }
        
        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<CareHistory> CareHistories { get; set; }
    }
}
