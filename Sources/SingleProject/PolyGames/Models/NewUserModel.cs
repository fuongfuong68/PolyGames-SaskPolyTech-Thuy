using System;
using System.ComponentModel.DataAnnotations;

namespace PolyGames.Models
{
    public class NewUserModel
    {
        public int MemberID { set; get; }

        [Display(Name = "Email*")]
        [Required]
        [EmailAddress]
        public string Email { set; get; }

        [Display(Name = "Temporary Password*")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { set; get; }

        [Display(Name = "Is User Active?")]
        [Required]
        public bool IsActive { set; get; }

        [Display(Name = "Name*")]
        [Required]
        public string Name { set; get; }

        [Display(Name = "Is User Admin?")]
        [Required]
        public bool IsAdmin { set; get; }

        [Display(Name = "Registration Date")]
        public DateTime? RegistrationDate { get; set; }

        [Display(Name = "Assign Team")]
        [Required]
        public int GroupId { get; set; }

        [Display(Name = "Password Reset Request?")]
        [Required]
        public bool PasswordResetRequest { set; get; }
    }
}