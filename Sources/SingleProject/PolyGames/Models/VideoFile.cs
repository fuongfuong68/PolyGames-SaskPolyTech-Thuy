using System;

namespace PolyGames.Models
{
    //Needed a model for video as you can have multiple videos per one game
    public class VideoFile
    {
        public int VideoId { get; set; }
        public string VideoFileName { get; set; }
        public Nullable<int> VideoFileSize { get; set; }
        public string VideoFilePath { get; set; }
        public int GameID { get; set; }
    }
}