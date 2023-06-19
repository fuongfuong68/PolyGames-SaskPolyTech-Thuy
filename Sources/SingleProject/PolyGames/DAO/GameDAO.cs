using System;
using System.Collections.Generic;
using PolyGames.Models;
using System.Data.SqlClient;
using System.Data;

namespace PolyGames.DAO
{
    public class GameDAO : IDisposable
    {
        private SqlConnection _sqlConnection;
        private bool disposedValue;

        public GameDAO(SqlConnection sqlConnection)
        {
            _sqlConnection = sqlConnection;
        }

        public List<Game> Search(string keyword)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT distinct g. * FROM Games g , PolyGamesGroups t  WHERE g.GroupId = t.GroupId and t.GroupName LIKE '%' + @keyword + '%' or g.GameName LIKE '%' + @keyword + '%'";
                cmd.Parameters.AddWithValue("@keyword", keyword);
                reader = cmd.ExecuteReader();
                List<Game> games = new List<Game>();

                while (reader.Read())
                {
                    Game game = new Game
                    {
                        Id = (int)reader["GameId"],
                        GameName = reader["GameName"] as string,
                        Description = reader["GameDescription"] as string,
                        Year = (int)reader["Year"],
                    };

                    games.Add(game);
                }

                return games;
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public List<Game> SearchByGameName(string gameName)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM Games WHERE GameName LIKE '%' + @gameName + '%'";
                cmd.Parameters.AddWithValue("@gameName", gameName);
                reader = cmd.ExecuteReader();
                List<Game> games = new List<Game>();

                while (reader.Read())
                {
                    Game game = new Game
                    {
                        Id = (int)reader["GameId"],
                        GameName = reader["GameName"] as string,
                        Description = reader["GameDescription"] as string,
                        Year = (int)reader["Year"],
                    };
                    games.Add(game);
                }

                return games;
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public List<Game> SearchByTeam(string groupName)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT g. * FROM Games g , PolyGamesGroups t  WHERE g.GroupId = t.GroupId and t.GroupName LIKE '%' + @groupName + '%'";
                cmd.Parameters.AddWithValue("@groupName", groupName);
                reader = cmd.ExecuteReader();
                List<Game> games = new List<Game>();

                while (reader.Read())
                {
                    Game game = new Game
                    {
                        Id = (int)reader["GameId"],
                        GameName = reader["GameName"] as string,
                        Description = reader["GameDescription"] as string,
                        Year = (int)reader["Year"],
                    };
                    games.Add(game);
                }

                return games;
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public int CountGames()
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT COUNT(*) FROM Games";

                int count = (int)cmd.ExecuteScalar();
                return count;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
            }
        }

        //Query used for allYears page
        public List<Game> GetAllYears()
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT DISTINCT Year FROM Games";

                reader = cmd.ExecuteReader();

                List<Game> games = new List<Game>();

                while (reader.Read())
                {
                    Game game = new Game { Year = (int)reader["Year"] };
                    games.Add(game);
                }

                return games;
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        //Query to be used for Home page to display recently added games (video with clickable link that takes user to that game's game page)
        public List<Game> GetGamesOrderedByMostRecentlyAdded()
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_GetAllGamesOrderedByRecentlyAdded";

                reader = cmd.ExecuteReader();
                List<Game> games = new List<Game>();
                while (reader.Read())
                {
                    Game game = new Game
                    {
                        Id = (int)reader["GameId"],
                        GameName = reader["GameName"] as string,
                        Description = reader["GameDescription"] as string,
                    };

                    games.Add(game);
                }

                return games;
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        //Query to be used for individual game page
        public Game GetGameById(int id)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_GetGameDetailsOne";
                cmd.Parameters.AddWithValue("@GameId", id);

                reader = cmd.ExecuteReader();
                if (!reader.Read())
                    return null;

                Game game = new Game
                {
                    Id = (int)reader["GameId"],
                    GameName = reader["GameName"] as string,
                    Description = reader["GameDescription"] as string,
                    Year = (int)reader["Year"],
                    GroupId = (int)reader["GroupId"],
                    GroupName = reader["GroupName"] as string,
                    ExecutableId = (int)reader["executableId"],
                    ExecutableFilePath = reader["executableFilePath"] as string,
                    HtmlVersionLink = reader["HtmlVersionLink"] as string,
                };

                return game;
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        //Query to be used for Game page to display all game pictures (called in the getGameByID method)
        public List<PictureFile> GetGamePictures(int gameId)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT pictureID, pictureFileName, pictureFilePath FROM PictureFiles WHERE gameID=@Id";
                cmd.Parameters.AddWithValue("@Id", gameId);

                reader = cmd.ExecuteReader();
                List<PictureFile> pictureFiles = new List<PictureFile>();
                while (reader.Read())
                {
                    PictureFile pictureFile = new PictureFile
                    {
                        GameID = gameId,
                        PictureId = (int)reader["pictureID"],
                        PictureFileName = reader["pictureFileName"] as string,
                        PictureFilePath = reader["pictureFilePath"] as string,
                    };

                    pictureFiles.Add(pictureFile);
                }

                return pictureFiles;
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public List<VideoFile> GetGameVideos(int gameId)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT videoID, videoFileName, videoFilePath FROM videoFiles WHERE gameID=@Id";
                cmd.Parameters.AddWithValue("@Id", gameId);

                reader = cmd.ExecuteReader();
                List<VideoFile> videoFiles = new List<VideoFile>();
                while (reader.Read())
                {
                    VideoFile videoFile = new VideoFile
                    {
                        GameID = gameId,
                        VideoId = (int)reader["videoID"],
                        VideoFileName = reader["videoFileName"] as string,
                        VideoFilePath = reader["videoFilePath"] as string,
                    };

                    videoFiles.Add(videoFile);
                }

                return videoFiles;
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        //Query to be used for Games by Year page
        public List<Game> GetGamesByYear(int year)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_GetAllGamesByYear";
                cmd.Parameters.AddWithValue("@GameYear", year);

                reader = cmd.ExecuteReader();
                List<Game> games = new List<Game>();
                while (reader.Read())
                {
                    Game game = new Game
                    {
                        Id = (int)reader["GameId"],
                        GameName = reader["GameName"] as string,
                        Description = reader["GameDescription"] as string,
                        PictureFilePathOne = reader["pictureFilePath"] as string,
                        Year = year
                    };

                    games.Add(game);
                }

                return games;
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        //Query to be used for All Games page
        public List<Game> GetAllGames()
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM Games";

                reader = cmd.ExecuteReader();
                List<Game> games = new List<Game>();
                while (reader.Read())
                {
                    Game game = new Game
                    {
                        Id = (int)reader["GameId"],
                        GameName = reader["GameName"] as string,
                        Description = reader["GameDescription"] as string
                    };

                    games.Add(game);
                }

                return games;
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public List<PictureFile> GetPicturesByGameId(int gameId)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM PictureFiles WHERE gameID=@gameID";
                cmd.Parameters.AddWithValue("@gameID", gameId);

                reader = cmd.ExecuteReader();
                List<PictureFile> pictureFiles = new List<PictureFile>();
                while (reader.Read())
                {
                    PictureFile pictureFile = new PictureFile
                    {
                        GameID = gameId,
                        PictureId = (int)reader["pictureID"],
                        PictureFileName = reader["pictureFileName"] as string,
                        PictureFilePath = reader["pictureFilePath"] as string
                    };

                    pictureFiles.Add(pictureFile);
                }

                return pictureFiles;
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        //Command to delete all game data for one game
        public void DeleteGame(int id, int groupId)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_DeleteGame";
                cmd.Parameters.AddWithValue("@GameId", id);
                cmd.Parameters.AddWithValue("@GroupId", groupId);

                _ = cmd.ExecuteNonQuery();
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
            }
        }

        //SQL commands to be used to AddGame
        public void AddAGame(Game game)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlTransaction transaction = null;
            SqlCommand cmd = null;
            try
            {
                transaction = _sqlConnection.BeginTransaction();

                cmd = _sqlConnection.CreateCommand();
                cmd.Transaction = transaction;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO Games(Year, GameName, GameDescription, GroupId, HtmlVersionLink) OUTPUT INSERTED.GameId VALUES(@Year, @GameName, @GameDescription, @GroupId, @HtmlVersionLink)";
                cmd.Parameters.AddWithValue("@Year", game.Year);
                cmd.Parameters.AddWithValue("@GameName", game.GameName);
                cmd.Parameters.AddWithValue("@GameDescription", game.Description);
                cmd.Parameters.AddWithValue("@GroupId", game.GroupId);
                cmd.Parameters.AddWithValue("@HtmlVersionLink", game.HtmlVersionLink);

                game.Id = (int)cmd.ExecuteScalar();

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();

                foreach (PictureFile pictureFile in game.GamePictures)
                {
                    cmd.CommandText = "usp_AddNewPictureFile";
                    cmd.Parameters.AddWithValue("@pictureFileName", pictureFile.PictureFileName);
                    cmd.Parameters.AddWithValue("@pictureFileSize", pictureFile.PictureFileSize);
                    cmd.Parameters.AddWithValue("@pictureFilePath", "~/PictureFileUpload/" + pictureFile.PictureFileName);
                    cmd.Parameters.AddWithValue("@gameID", game.Id);

                    _ = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }

                VideoFile videoFile = game.GameVideos[0];
                cmd.CommandText = "usp_AddNewVideoFile";
                cmd.Parameters.AddWithValue("@videoFileName", videoFile.VideoFileName);
                cmd.Parameters.AddWithValue("@videoFileSize", videoFile.VideoFileSize);
                cmd.Parameters.AddWithValue("@videoFilePath", "~/VideoFileUpload/" + videoFile.VideoFileName);
                cmd.Parameters.AddWithValue("@gameID", game.Id);

                _ = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                cmd.CommandText = "usp_AddNewExecutableFile";
                cmd.Parameters.AddWithValue("@executableFileName", game.ExecutableFileName);
                cmd.Parameters.AddWithValue("@executableFileSize", game.ExecutableFileSize);
                cmd.Parameters.AddWithValue("@executableFilePath", "~/ExecutableFileUpload/" + game.ExecutableFileName);
                cmd.Parameters.AddWithValue("@gameID", game.Id);

                _ = cmd.ExecuteNonQuery();
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("", ex);
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
            }
        }

        //SQL Commands to update game details
        public void UpdateGame(Game game)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlTransaction transaction = null;
            SqlCommand cmd = null;
            try
            {
                transaction = _sqlConnection.BeginTransaction();

                cmd = _sqlConnection.CreateCommand();
                cmd.Transaction = transaction;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "UPDATE Games SET GameName=@GameName, Year=@Year, GameDescription=@GameDescription, HtmlVersionLink = @HtmlVersionLink WHERE GameId=@GameId";
                cmd.Parameters.AddWithValue("@GameId", game.Id);
                cmd.Parameters.AddWithValue("@GameName", game.GameName);
                cmd.Parameters.AddWithValue("@Year", game.Year);
                cmd.Parameters.AddWithValue("@GameDescription", game.Description);
                cmd.Parameters.AddWithValue("@HtmlVersionLink", game.HtmlVersionLink);

                _ = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                cmd.CommandText = "UPDATE PolyGamesGroups SET GroupName=@GroupName WHERE GroupId=@GroupId";
                cmd.Parameters.AddWithValue("@GroupId", game.GroupId);
                cmd.Parameters.AddWithValue("@GroupName", game.GroupName);

                _ = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

                if (game.GamePictures.Count != 0)
                {
                    cmd.CommandText = "DELETE FROM PictureFiles where gameID=@GameId";
                    cmd.Parameters.AddWithValue("@GameId", game.Id);

                    _ = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (PictureFile pictureFile in game.GamePictures)
                    {
                        cmd.CommandText = "usp_AddNewPictureFile";
                        cmd.Parameters.AddWithValue("@pictureFileName", pictureFile.PictureFileName);
                        cmd.Parameters.AddWithValue("@pictureFileSize", pictureFile.PictureFileSize);
                        cmd.Parameters.AddWithValue("@pictureFilePath", "~/PictureFileUpload/" + pictureFile.PictureFileName);
                        cmd.Parameters.AddWithValue("@gameID", game.Id);

                        _ = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                }

                cmd.CommandType = CommandType.Text;  
                if (game.GameVideos.Count != 0)
                {
                    VideoFile videoFile = game.GameVideos[0];

                    cmd.CommandText = @"
                        UPDATE VideoFiles 
                        SET videoFileName = @videoFileName, videoFileSize = @videoFileSize, videoFilePath = @videoFilePath 
                        WHERE gameID=@GameId";
                    cmd.Parameters.AddWithValue("@videoFileName", videoFile.VideoFileName);
                    cmd.Parameters.AddWithValue("@videoFileSize", videoFile.VideoFileSize);
                    cmd.Parameters.AddWithValue("@videoFilePath", "~/VideoFileUpload/" + videoFile.VideoFileName);
                    cmd.Parameters.AddWithValue("@gameID", game.Id);

                    _ = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }

                if (!string.IsNullOrEmpty(game.ExecutableFileName))
                {
                    cmd.CommandText = @"
                        UPDATE ExecutableFiles 
                        SET executableFileName = @executableFileName, executableFileSize = @executableFileSize, executableFilePath = @executableFilePath 
                        WHERE gameID=@GameId";
                    cmd.Parameters.AddWithValue("@executableFileName", game.ExecutableFileName);
                    cmd.Parameters.AddWithValue("@executableFileSize", game.ExecutableFileSize);
                    cmd.Parameters.AddWithValue("@executableFilePath", "~/ExecutableFileUpload/" + game.ExecutableFileName);
                    cmd.Parameters.AddWithValue("@GameId", game.Id);

                    _ = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }

                foreach(Student member in game.GroupMembers)
                {
                    cmd.CommandText = "UPDATE UserLogin SET Name=@Name WHERE MemberId=@MemberId";
                    cmd.Parameters.AddWithValue("@MemberId", member.MemberId);
                    cmd.Parameters.AddWithValue("@Name", member.StudentName);

                    _ = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }

                foreach (Student member in game.GroupMembers)
                {
                    cmd.CommandText = "UPDATE MemberToGroup SET StudentRole=@StudentRole WHERE MemberId=@MemberId AND GroupId=@GroupId";

                    cmd.Parameters.AddWithValue("@MemberId", member.MemberId);
                    cmd.Parameters.AddWithValue("@GroupId", game.GroupId);
                    cmd.Parameters.AddWithValue("@StudentRole", member.StudentRole);

                    _ = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("", ex);
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public List<Game> GetAllGamesList()
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT Games.GameId ,PolyGamesGroups.GroupId, ExecutableFiles.executableId, PolyGamesGroups.GroupName, Games.GameName, Games.Year, ExecutableFiles.executableFileName, ExecutableFiles.executableFileSize FROM Games INNER JOIN PolyGamesGroups ON Games.GroupId = PolyGamesGroups.GroupId INNER JOIN ExecutableFiles ON Games.GameId = ExecutableFiles.gameID";

                reader = cmd.ExecuteReader();
                List<Game> games = new List<Game>();
                while (reader.Read())
                {
                    Game game = new Game
                    {
                        Id = (int)reader["GameId"],
                        GameName = reader["GameName"] as string,
                        Year = (int)reader["Year"],
                        GroupId = (int)reader["GroupId"],
                        GroupName = reader["GroupName"] as string,
                        ExecutableId = (int)reader["executableId"],
                        ExecutableFileName = reader["executableFileName"] as string,
                        ExecutableFileSize = (int)reader["executableFileSize"]
                    };

                    games.Add(game);
                }

                return games;
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        //// test for returning a signle user
        public List<Game> GetUserDataById(int id)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT MemberToGroup.GroupId, PolyGamesGroups.GroupName, Games.GameName, Games.GameId, MemberToGroup.IsHidden FROM MemberToGroup INNER JOIN PolyGamesGroups ON MemberToGroup.GroupId = PolyGamesGroups.GroupId INNER JOIN Games ON PolyGamesGroups.GroupId = Games.GroupId WHERE MemberToGroup.MemberId = @MemberID";
                cmd.Parameters.AddWithValue("@MemberID", id);

                reader = cmd.ExecuteReader();
                List<Game> games = new List<Game>();
                while (reader.Read())
                {
                    Game game = new Game
                    {
                        Id = (int)reader["GameId"],
                        GameName = reader["GameName"] as string,
                        GroupId = (int)reader["GroupId"],
                        GroupName = reader["GroupName"] as string,
                        IsHidden = (bool)reader["IsHidden"]
                    };

                    games.Add(game);
                }

                return games;
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                }

                _sqlConnection.Dispose();

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}