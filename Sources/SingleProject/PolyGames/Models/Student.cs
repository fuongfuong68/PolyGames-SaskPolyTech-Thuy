using System;

namespace PolyGames.Models
{
    //Needed a model for student as you can have multiple pictures per one game
    public class Student
    {
        public int MemberId { set; get; }
        public int GroupId { set; get; }
        public string StudentName { get; set; }
        public string StudentRole { get; set; }
        public bool IsHidden { get; set; }
    }
}