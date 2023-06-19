using PolyGames.Common;
using PolyGames.DAO;
using System;
using System.Data.SqlClient;

namespace PolyGames.Services
{
    public class PolyGameService : IDisposable
    {
        private readonly SqlConnection _sqlConnection;
        private readonly UploadConfiguration _uploadConfiguration;
        private readonly ContentTypeManager _contentTypeManager;
        private readonly FileNameGenerator _fileNameGenerator;

        private readonly SmtpService _smtpService;

        private readonly GameDAO _gameDAO;
        private readonly UserDAO _userDAO;
        private readonly GameGroupDAO _gameGroupDAO;
        private bool disposedValue;

        public PolyGameService(SqlConfiguration sqlConfiguration, UploadConfiguration uploadConfiguration, ContentTypeManager contentTypeManager, FileNameGenerator fileNameGenerator, SmtpService smtpService)
        {
            _sqlConnection = new SqlConnection() { ConnectionString = sqlConfiguration.ConnectionString };
            _uploadConfiguration = uploadConfiguration;
            _contentTypeManager = contentTypeManager;
            _fileNameGenerator = fileNameGenerator;

            _smtpService = smtpService;

            _gameDAO = new GameDAO(_sqlConnection);
            _userDAO = new UserDAO(_sqlConnection);
            _gameGroupDAO = new GameGroupDAO(_sqlConnection);
        }

        public GameService GameService => new GameService(_gameDAO, _gameGroupDAO, _uploadConfiguration, _contentTypeManager, _fileNameGenerator);
        public UserService UserService => new UserService(_userDAO, _gameGroupDAO, _smtpService);
        public GameGroupService GameGroupService => new GameGroupService(_gameGroupDAO);

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