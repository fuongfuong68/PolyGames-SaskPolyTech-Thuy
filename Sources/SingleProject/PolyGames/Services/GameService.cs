using PolyGames.Common;
using PolyGames.DAO;
using PolyGames.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PolyGames.Services
{
    public class GameService
    {
        private readonly GameDAO _gameDAO;
        private readonly GameGroupDAO _gameGroupDAO;
        private readonly UploadConfiguration _uploadConfiguration;
        private readonly ContentTypeManager _contentTypeManager;
        private readonly FileNameGenerator _fileNameGenerator;

        internal GameService(GameDAO gameDAO, GameGroupDAO gameGroupDAO, UploadConfiguration uploadConfiguration, ContentTypeManager contentTypeManager, FileNameGenerator fileNameGenerator)
        {
            _gameDAO = gameDAO;
            _gameGroupDAO = gameGroupDAO;
            _uploadConfiguration = uploadConfiguration;
            _contentTypeManager = contentTypeManager;
            _fileNameGenerator = fileNameGenerator;
        }

        public List<Game> Search(string keyword)
        {
            List<Game> games = _gameDAO.Search(keyword);

            games.ForEach((x) =>
            {
                x.GamePictures = _gameDAO.GetGamePictures(x.Id);
            });

            return games;
        }

        public List<Game> SearchByGameName(string gameName)
        {
            List<Game> games = _gameDAO.SearchByGameName(gameName);

            games.ForEach((x) =>
            {
                x.GamePictures = _gameDAO.GetGamePictures(x.Id);
            });

            return games;
        }

        public List<Game> SearchByTeam(string groupName)
        {
            List<Game> games = _gameDAO.SearchByTeam(groupName);

            games.ForEach((x) =>
            {
                x.GamePictures = _gameDAO.GetGamePictures(x.Id);
            });

            return games;
        }

        public int CountGames()
        {
            return _gameDAO.CountGames();
        }

        public Game GetGameById(int id)
        {
            Game game = _gameDAO.GetGameById(id);
            if (game == null)
                return null;

            game.GameVideos = _gameDAO.GetGameVideos(game.Id);
            game.GamePictures = _gameDAO.GetGamePictures(game.Id);
            game.GroupMembers = _gameGroupDAO.GetGroupMembers(game.GroupId);

            return game;
        }

        public List<Game> GetGamesOrderedByMostRecentlyAdded()
        {
            List<Game> games = _gameDAO.GetGamesOrderedByMostRecentlyAdded();

            games.ForEach((x) =>
            {
                x.GamePictures = _gameDAO.GetGamePictures(x.Id);
                x.GameVideos = _gameDAO.GetGameVideos(x.Id);
            });

            return games;
        }

        public List<Game> GetGamesByYear(int year)
        {
            return _gameDAO.GetGamesByYear(year);
        }

        public List<Game> GetAllYears()
        {
            return _gameDAO.GetAllYears();
        }

        public List<Game> GetAllGames()
        {
            List<Game> games = _gameDAO.GetAllGames();

            games.ForEach((x) =>
            {
                x.GamePictures = _gameDAO.GetGamePictures(x.Id);
            });

            return games;
        }

        public List<Game> GetAllGamesList()
        {
            return _gameDAO.GetAllGamesList();
        }

        public void DeleteGame(int id, int groupId, HttpServerUtilityBase httpServerUtility)
        {
            Game game = _gameDAO.GetGameById(id);
            List<PictureFile> pictureFiles = _gameDAO.GetGamePictures(id);
            List<VideoFile> videoFiles = _gameDAO.GetGameVideos(id);

            _gameDAO.DeleteGame(id, groupId);

            foreach (PictureFile pictureFile in pictureFiles)
            {
                string pictureFileName = httpServerUtility.MapPath(pictureFile.PictureFilePath);
                if (File.Exists(pictureFileName))
                    File.Delete(pictureFileName);
            }

            string videoFilePath = httpServerUtility.MapPath(videoFiles[0].VideoFilePath);
            if (File.Exists(videoFilePath))
                File.Delete(videoFilePath);

            string exeFilePath = httpServerUtility.MapPath(game.ExecutableFilePath);
            if (File.Exists(exeFilePath))
                File.Delete(exeFilePath);
        }

        public ExecResult AddAGame(Game game, HttpServerUtilityBase httpServerUtility)
        {
            if (game.PicturesUpload.Count() == 1 && game.PicturesUpload.ToArray()[0] == null)
                return new ExecResult { Status = false, Message = "No picture" };

            bool isInvalidContentType = game.PicturesUpload.Any(x => !_contentTypeManager.IsImage(x.ContentType));
            if (isInvalidContentType)
                return new ExecResult { Status = false, Message = "Invalid image" };

            if (game.PicturesUpload.Any(x => x.ContentLength > _uploadConfiguration.MaxAllowedImageContentLength))
                return new ExecResult { Status = false, Message = "Pictures file too large" };

            HttpPostedFileBase videoUploaded = game.VideoUpload;
            if (videoUploaded == null)
                return new ExecResult { Status = false, Message = "No video" };

            if (!_contentTypeManager.IsVideo(videoUploaded.ContentType))
                return new ExecResult { Status = false, Message = "Invalid video" };

            if (videoUploaded.ContentLength > _uploadConfiguration.MaxAllowedVideoContentLength)
                return new ExecResult { Status = false, Message = "Video file too large" };

            HttpPostedFileBase exeFileUploaded = game.ExecutableUpload;
            if (exeFileUploaded == null)
                return new ExecResult { Status = false, Message = "No executable file" };

            if (!_contentTypeManager.IsZip(exeFileUploaded.ContentType))
                return new ExecResult { Status = false, Message = "Invalid executable file" };

            if (exeFileUploaded.ContentLength > _uploadConfiguration.MaxAllowedExecutableFileContentLength)
                return new ExecResult { Status = false, Message = "Executable file too large" };

            int count = 0;
            List<PictureFile> pictureFiles = new List<PictureFile>();
            foreach (HttpPostedFileBase pictureUploaded in game.PicturesUpload)
            {
                PictureFile pictureFile = new PictureFile();
                pictureFile.PictureFileName = $"{_fileNameGenerator.GetPictureFileName(count)}{Path.GetExtension(pictureUploaded.FileName)}";
                pictureFile.PictureFileSize = pictureUploaded.ContentLength / 1000;
                pictureUploaded.SaveAs(httpServerUtility.MapPath("~/PictureFileUpload/" + pictureFile.PictureFileName));
                pictureFiles.Add(pictureFile);

                count++;
                if (count == 5) break;
            }

            VideoFile videoFile = new VideoFile();
            videoFile.VideoFileName = $"{_fileNameGenerator.GetVideoFileName()}{Path.GetExtension(videoUploaded.FileName)}";
            videoFile.VideoFileSize = videoUploaded.ContentLength / 1000;
            videoUploaded.SaveAs(httpServerUtility.MapPath("~/VideoFileUpload/" + videoFile.VideoFileName));

            string exeFileName = $"{_fileNameGenerator.GetExeFileName()}{Path.GetExtension(exeFileUploaded.FileName)}";
            int exeFileSize = exeFileUploaded.ContentLength / 1000;
            exeFileUploaded.SaveAs(httpServerUtility.MapPath("~/ExecutableFileUpload/" + exeFileName));

            game.GamePictures = pictureFiles;
            game.GameVideos = new List<VideoFile>() { videoFile };
            game.ExecutableFileName = exeFileName;
            game.ExecutableFileSize = exeFileSize;

            _gameDAO.AddAGame(game);

            return new ExecResult { Status = true, Message = "Success" };
        }

        public ExecResult UpdateGame(Game game, HttpServerUtilityBase httpServerUtility)
        {
            game.GamePictures = new List<PictureFile>();
            game.GameVideos = new List<VideoFile>();
            game.ExecutableFileName = null;
            game.ExecutableFileSize = null;

            List<HttpPostedFileBase> picturesUploaded = game.PicturesUpload.ToList();
            if (picturesUploaded[0] != null)
            {
                bool isInvalidContentType = game.PicturesUpload.Any(x => !_contentTypeManager.IsImage(x.ContentType));
                if (isInvalidContentType)
                    return new ExecResult { Status = false, Message = "Invalid image" };

                if (game.PicturesUpload.Any(x => x.ContentLength > _uploadConfiguration.MaxAllowedImageContentLength))
                    return new ExecResult { Status = false, Message = "Pictures file too large" };

                int count = 0;
                foreach (HttpPostedFileBase pictureUploaded in picturesUploaded)
                {
                    PictureFile pictureFile = new PictureFile();
                    pictureFile.PictureFileName = $"{_fileNameGenerator.GetPictureFileName(count)}{Path.GetExtension(pictureUploaded.FileName)}";
                    pictureFile.PictureFileSize = pictureUploaded.ContentLength / 1000;
                    pictureUploaded.SaveAs(httpServerUtility.MapPath("~/PictureFileUpload/" + pictureFile.PictureFileName));
                    game.GamePictures.Add(pictureFile);

                    count++;
                    if (count == 5) break;
                }
            }

            HttpPostedFileBase videoUploaded = game.VideoUpload;
            if (videoUploaded != null)
            {
                if (!_contentTypeManager.IsVideo(videoUploaded.ContentType))
                    return new ExecResult { Status = false, Message = "Invalid video" };

                if (videoUploaded.ContentLength > _uploadConfiguration.MaxAllowedVideoContentLength)
                    return new ExecResult { Status = false, Message = "Video file too large" };

                VideoFile videoFile = new VideoFile();
                videoFile.VideoFileName = $"{_fileNameGenerator.GetVideoFileName()}{Path.GetExtension(videoUploaded.FileName)}";
                videoFile.VideoFileSize = videoUploaded.ContentLength / 1000;
                videoUploaded.SaveAs(httpServerUtility.MapPath("~/VideoFileUpload/" + videoFile.VideoFileName));

                game.GameVideos.Add(videoFile);
            }

            HttpPostedFileBase exeFileUploaded = game.ExecutableUpload;
            if (exeFileUploaded != null)
            {
                if (!_contentTypeManager.IsZip(exeFileUploaded.ContentType))
                    return new ExecResult { Status = false, Message = "Invalid executable file" };

                if (exeFileUploaded.ContentLength > _uploadConfiguration.MaxAllowedExecutableFileContentLength)
                    return new ExecResult { Status = false, Message = "Executable file too large" };

                string exeFileName = $"{_fileNameGenerator.GetExeFileName()}{Path.GetExtension(exeFileUploaded.FileName)}";
                int exeFileSize = exeFileUploaded.ContentLength / 1000;
                exeFileUploaded.SaveAs(httpServerUtility.MapPath("~/ExecutableFileUpload/" + exeFileName));

                game.ExecutableFileName = exeFileName;
                game.ExecutableFileSize = exeFileSize;
            }

            _gameDAO.UpdateGame(game);

            return new ExecResult { Status = true, Message = "Success" };
        }

        public List<Game> GetUserDataById(int id)
        {
            return _gameDAO.GetUserDataById(id);
        }
    }
}