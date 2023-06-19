using System;

namespace PolyGames.Models
{
    //Needed a model for pictures as you can have multiple pictures per one game
    public class PictureFile
    {
        public int PictureId { get; set; }
        public string PictureFileName { get; set; }
        public Nullable<int> PictureFileSize { get; set; }
        public string PictureFilePath { get; set; }
        public int GameID { get; set; }
    }
}