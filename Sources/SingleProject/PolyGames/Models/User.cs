using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PolyGames.Models
{
    public class User
    {
        public List<User> Items { get; set; }
        public int EditIndex { get; set; }
        public bool IsEditable { get; set; }
        public string Code { set; get; }

        [Required]
        [EmailAddress]
        public string Email { set; get; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { set; get; }

        public Boolean IsActive { set; get; }
        public string Name { set; get; }
        public int MemberID { set; get; }
        public Boolean IsAdmin { set; get; }
        public Boolean IsComp214 { set; get; }
        public Boolean PasswordResetRequest { set; get; }

        [DataType(DataType.Date)]
        public DateTime? RegistrationDate { get; set; }
        public int RegistrationYear { get; set; }
        public int regYear{get; set;}
        public List<Group> CurrentTeamName { set; get; }
    }
}