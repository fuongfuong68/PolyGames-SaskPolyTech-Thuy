using System.ComponentModel.DataAnnotations;

namespace PolyGames.Models
{
    public class UpdatePasswordModel
    {
        public int ID { get; set; }

        [Display(Name = "OldPassword")]
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Display(Name = "Enter New Password")]
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "Re-Enter New Password")]
        [Required]
        [DataType(DataType.Password)]
        public string RetypePassword { get; set; }
    }
}