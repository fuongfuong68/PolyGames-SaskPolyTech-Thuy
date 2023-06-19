using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PolyGames.Models
{
    public class Group
    {
        public int GroupId { get; set; }

        [Display(Name = "Temporary Group Name")]
        [Required]
        public string GroupName { get; set; }

        public List<int> ids { get; set; }
    }
}