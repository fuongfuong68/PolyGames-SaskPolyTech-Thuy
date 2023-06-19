using System.ComponentModel.DataAnnotations;

namespace PolyGames.Models
{
    public class RegisterModel
    {
        [Display(Name = "Name")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Email")]
        [Required]
        [EmailAddress]
        public string Email { set; get; }

        [Display(Name = "Password")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Access Code")]
        [Required]
        public string Code { get; set; }
    }
}