
using System.ComponentModel.DataAnnotations;

namespace PolyGames.Models
{
    public class NewPasswordModel
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}