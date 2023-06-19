using System.ComponentModel.DataAnnotations;

namespace PolyGames.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string VerificationCode { get; set; }

        public string Password { get; set; }
    }
}