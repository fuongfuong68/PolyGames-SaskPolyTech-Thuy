using PolyGames.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PolyGames.DAO
{
    public class UserDAO : IDisposable
    {
        private SqlConnection _sqlConnection;
        private bool disposedValue;

        public UserDAO(SqlConnection sqlConnection)
        {
            _sqlConnection = sqlConnection;
        }

        public void UpdatePassword(int userId, string newPassword)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "UPDATE UserLogin SET Password = @newPW WHERE MemberID = @userId";
                cmd.Parameters.AddWithValue("@newPW", newPassword);
                cmd.Parameters.AddWithValue("@userId", userId);

                _ = cmd.ExecuteNonQuery();
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public User GetUserById(int id)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT MemberId, Name, Email, Password, IsActive, IsAdmin, RegistrationDate, PasswordResetRequest FROM UserLogin WHERE MemberId=@id";
                cmd.Parameters.AddWithValue("@id", id);

                reader = cmd.ExecuteReader();

                if (!reader.Read())
                    return null;

                User user = new User
                {
                    MemberID = (int)reader["MemberId"],
                    Name = reader["Name"] as string,
                    Email = reader["Email"] as string,
                    IsActive = (bool)reader["IsActive"],
                    IsAdmin = (bool)reader["IsAdmin"],
                    RegistrationDate = (DateTime)reader["RegistrationDate"],
                    PasswordResetRequest = (bool)reader["PasswordResetRequest"]
                };

                return user;
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public User GetUserByEmail(string email)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT MemberId, Name, Email, Password, IsActive, IsAdmin, RegistrationDate, PasswordResetRequest FROM UserLogin WHERE Email=@email";
                cmd.Parameters.AddWithValue("@email", email);

                reader = cmd.ExecuteReader();

                if (!reader.Read())
                    return null;

                User user = new User
                {
                    MemberID = (int)reader["MemberId"],
                    Name = reader["Name"] as string,
                    Email = reader["Email"] as string,
                    Password = reader["Password"] as string,
                    IsActive = (bool)reader["IsActive"],
                    IsAdmin = (bool)reader["IsAdmin"],
                    RegistrationDate = (DateTime)reader["RegistrationDate"],
                    PasswordResetRequest = (bool)reader["PasswordResetRequest"]
                };

                return user;
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public List<User> GetAllUsers()
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT MemberId, Name, Email, Password, IsActive, IsAdmin, RegistrationDate, PasswordResetRequest FROM UserLogin";

                reader = cmd.ExecuteReader();
                List<User> users = new List<User>();
                while (reader.Read())
                {
                    User user = new User
                    {
                        MemberID = (int)reader["MemberId"],
                        Name = reader["Name"] as string,
                        Email = reader["Email"] as string,
                        IsActive = (bool)reader["IsActive"],
                        IsAdmin = (bool)reader["IsAdmin"],
                        RegistrationDate = (DateTime)reader["RegistrationDate"],
                        PasswordResetRequest = (bool)reader["PasswordResetRequest"]
                    };

                    user.RegistrationYear = user.RegistrationDate.Value.Year;
                    users.Add(user);
                }

                return users;
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public List<User> GetUserByFilters(GameFilter gameFilter)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;

                if (gameFilter.GroupId == null && gameFilter.RegistrationYear != null)
                {
                    cmd.CommandText = "SELECT * FROM UserLogin WHERE YEAR(RegistrationDate)=@RegistrationDate";
                    cmd.Parameters.AddWithValue("@RegistrationDate", gameFilter.RegistrationYear);
                }
                else if (gameFilter.GroupId != null && gameFilter.RegistrationYear == null)
                {
                    cmd.CommandText = "SELECT * FROM UserLogin INNER JOIN MemberToGroup ON UserLogin.MemberId = MemberToGroup.MemberId WHERE MemberToGroup.GroupId=@GroupId";
                    cmd.Parameters.AddWithValue("@GroupId", gameFilter.GroupId);
                }
                else if (gameFilter.GroupId != null && gameFilter.RegistrationYear != null)
                {
                    cmd.CommandText = "SELECT * FROM UserLogin INNER JOIN MemberToGroup ON UserLogin.MemberId = MemberToGroup.MemberId WHERE MemberToGroup.GroupId=@GroupId AND YEAR(UserLogin.RegistrationDate)=@RegistrationDate";
                    cmd.Parameters.AddWithValue("@GroupId", gameFilter.GroupId);
                    cmd.Parameters.AddWithValue("@RegistrationDate", gameFilter.RegistrationYear);
                }

                reader = cmd.ExecuteReader();
                List<User> users = new List<User>();
                while (reader.Read())
                {
                    User user = new User
                    {
                        MemberID = (int)reader["MemberId"],
                        Name = reader["Name"] as string,
                        Email = reader["Email"] as string,
                        IsActive = (bool)reader["IsActive"],
                        IsAdmin = (bool)reader["IsAdmin"],
                        RegistrationDate = (DateTime)reader["RegistrationDate"],
                        PasswordResetRequest = (bool)reader["PasswordResetRequest"]
                    };

                    user.RegistrationYear = user.RegistrationDate.Value.Year;
                    users.Add(user);
                }

                return users;
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public bool CheckUserExists(User user)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT COUNT(*) FROM UserLogin WHERE Email = @Email";
                cmd.Parameters.AddWithValue("@Email", user.Email);

                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public void AddUser(NewUserModel newUser)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandText = "INSERT INTO UserLogin (Email, Password, Name, IsAdmin, IsActive, RegistrationDate, PasswordResetRequest) VALUES (@Email, @Password, @Name, @IsAdmin, @IsActive, @RegistrationDate, @PasswordResetRequest)";
                cmd.Parameters.AddWithValue("@Email", newUser.Email);
                cmd.Parameters.AddWithValue("@Password", newUser.Password);
                cmd.Parameters.AddWithValue("@Name", newUser.Name);
                cmd.Parameters.AddWithValue("@IsAdmin", newUser.IsAdmin);
                cmd.Parameters.AddWithValue("@IsActive", newUser.IsActive);
                cmd.Parameters.AddWithValue("@RegistrationDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@PasswordResetRequest", newUser.PasswordResetRequest);

                _ = cmd.ExecuteNonQuery();
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public void UpdateUser(User user)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "UPDATE UserLogin SET Email=@Email, Password=@Password, IsActive=@IsActive, IsAdmin=@IsAdmin, Name=@Name, PasswordResetRequest = @PasswordResetRequest WHERE MemberId = @MemberId";
                cmd.Parameters.AddWithValue("@MemberId", user.MemberID);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                cmd.Parameters.AddWithValue("@IsAdmin", user.IsAdmin);
                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@PasswordResetRequest", user.PasswordResetRequest);

                _ = cmd.ExecuteNonQuery();
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public void DeleteUser(int memberId)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM UserLogin WHERE MemberId=@MemberId";
                cmd.Parameters.AddWithValue("@MemberId", memberId);

                _ = cmd.ExecuteNonQuery();
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public List<int> GetDistinctRegistrationYear()
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT DISTINCT YEAR(RegistrationDate)AS 'RegistrationYear' FROM UserLogin ";

                reader = cmd.ExecuteReader();
                List<int> years = new List<int>();
                while (reader.Read())
                {
                    years.Add((int)reader["RegistrationYear"]);
                }

                return years;
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public int CountUsers()
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT COUNT(*) FROM UserLogin";

                int count = (int)cmd.ExecuteScalar();
                return count;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public void AddAccessCode(string code)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO AccessCodes (Code, ExpirationDate) VALUES (@Code, @ExpirationDate)";
                cmd.Parameters.AddWithValue("@Code", code);
                cmd.Parameters.AddWithValue("@ExpirationDate", DateTime.Now.AddHours(24));
                
                _ = cmd.ExecuteNonQuery();
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public List<AccessCode> GetAllAccessCodes()
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT Code, ExpirationDate FROM AccessCodes";

                reader = cmd.ExecuteReader();
                List<AccessCode> accessCodes = new List<AccessCode>();

                while (reader.Read())
                {
                    AccessCode accessCode = new AccessCode
                    {
                        Code = reader["Code"] as string,
                        IsExpired = DateTime.Now > ((DateTime)reader["ExpirationDate"]),
                        ExpirationDate = (DateTime)reader["ExpirationDate"]
                    };

                    accessCodes.Add(accessCode);
                }

                return accessCodes;
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public AccessCode GetAccessCode(string code)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT Code, ExpirationDate FROM AccessCodes Where Code=@code";
                cmd.Parameters.AddWithValue("@code", code);

                reader = cmd.ExecuteReader();

                if (!reader.Read())
                    return null;

                AccessCode accessCode = new AccessCode
                {
                    Code = reader["Code"] as string,
                    IsExpired = DateTime.Now > ((DateTime)reader["ExpirationDate"]),
                    ExpirationDate = (DateTime)reader["ExpirationDate"]
                };

                return accessCode;

            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public void DeleteExpiredCodes()
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM AccessCodes WHERE ExpirationDate < GETDATE()";

                _ = cmd.ExecuteNonQuery();
            }
            finally
            {
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