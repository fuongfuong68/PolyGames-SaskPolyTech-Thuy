using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace PolyGames.Models
{
    public class Game
    {
        //Allows changing the view on the Game.cshtml page to the editable view
        public bool IsEditable { get; set; }
        public bool IsHidden { get; set; }
        //public int MemberId { get; set; }

        //Game Attributes
        public int Id { get; set; }

        [Display(Name = "Game name")]
        [Required]
        public string GameName { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Year { get; set; }


        //Video Attributes
        public List<VideoFile> GameVideos { get; set; }

        [Display(Name = "Video upload (up to 100 MB)")]
        public HttpPostedFileBase VideoUpload { get; set; }


        //Picture Attributes
        public string PictureFilePathOne { get; set; }

        [Display(Name = "Pictures upload (up to 5MB per image)")]
        public IEnumerable<HttpPostedFileBase> PicturesUpload { get; set; }
        public List<PictureFile> GamePictures { get; set; }


        //Executable File Attributes
        public int ExecutableId { get; set; }
        public string ExecutableFileName { get; set; }
        public Nullable<int> ExecutableFileSize { get; set; }
        public string ExecutableFilePath { get; set; }

        [Display(Name = "Executable upload (up to 100 MB)")]
        public HttpPostedFileBase ExecutableUpload { get; set; }


        //Group Attributes
        public int GroupId { get; set; }
        public string GroupName { get; set; }


        //Group Member Attributes
        public List<Student> GroupMembers { get; set; }

        [Display(Name = "Html version link")]
        public string HtmlVersionLink { get; set; }
    }
}