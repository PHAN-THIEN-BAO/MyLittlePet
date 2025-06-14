using System.ComponentModel.DataAnnotations;

namespace MyLittlePetAPI.DTOs
{
    public class RegisterUserDTO
    {
        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "Player";

        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
    }

    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class UserDTO
    {
        public int ID { get; set; }
        public string Role { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int Level { get; set; }
        public int Coin { get; set; }
        public int Diamond { get; set; }
        public int Gem { get; set; }
        public DateTime JoinDate { get; set; }
    }

    public class UpdateUserDTO
    {
        [StringLength(100)]
        public string UserName { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
    }

    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public UserDTO User { get; set; }
    }
}
