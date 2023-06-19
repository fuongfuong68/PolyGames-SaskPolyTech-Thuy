using PolyGames.Common;
using PolyGames.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PolyGames.DAO
{
    public class GameGroupDAO : IDisposable
    {
        private SqlConnection _sqlConnection;
        private bool disposedValue;

        public GameGroupDAO(SqlConnection sqlConnection)
        {
            _sqlConnection = sqlConnection;
        }

        public GameGroupDAO(SqlConfiguration daoConfiguration) {
            _sqlConnection = new SqlConnection(daoConfiguration.ConnectionString);
        }

        public int CountGroups()
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT COUNT(*) FROM PolyGamesGroups";

                int count = (int)cmd.ExecuteScalar();
                return count;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
            }
        }


        public void AddGroup(Group group)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandText = "INSERT INTO PolyGamesGroups(GroupName) VALUES(@GroupName)";
                cmd.Parameters.AddWithValue("@GroupName", group.GroupName);

                _ = cmd.ExecuteNonQuery();
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
            }
        }

        //Gets a list of group member details - called in the getGameById method
        public List<Student> GetGroupMembers(int groupId)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT MemberToGroup.MemberId, studentRole, MemberToGroup.IsHidden, Name FROM UserLogin inner join MemberToGroup on MemberToGroup.MemberId = UserLogin.MemberId WHERE MemberToGroup.GroupId = @Id";
                cmd.Parameters.AddWithValue("@Id", groupId);

                reader = cmd.ExecuteReader();
                List<Student> students = new List<Student>();
                while (reader.Read())
                {
                    Student student = new Student
                    {
                        MemberId = (int)reader["MemberId"],
                        GroupId = groupId,
                        StudentName = reader["Name"] as string,
                        StudentRole = reader["studentRole"] as string,
                        IsHidden = (bool)reader["IsHidden"],
                    };

                    students.Add(student);
                }

                return students;
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public List<Group> GetGroupByMemberId(int memberId)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "usp_GetGroupNameByMemberId";
                cmd.Parameters.AddWithValue("@MemberId", memberId);

                reader = cmd.ExecuteReader();
                List<Group> groups = new List<Group>();
                while (reader.Read())
                {
                    Group group = new Group
                    {
                        GroupId = (int)reader["GroupId"],
                        GroupName = reader["GroupName"] as string
                    };
                    
                    groups.Add(group);
                }

                return groups;
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public List<Group> GetAllGroups()
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM PolyGamesGroups";

                reader = cmd.ExecuteReader();
                List<Group> groups = new List<Group>();
                while (reader.Read())
                {
                    Group group = new Group
                    {
                        GroupId = (int)reader["GroupId"],
                        GroupName = reader["GroupName"] as string
                    };

                    groups.Add(group);
                }

                return groups;
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public List<MemberToGroup> GetMemberToGroupIds()
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT MemberId, GroupId FROM MemberToGroup";

                reader = cmd.ExecuteReader();
                List<MemberToGroup> members = new List<MemberToGroup>();
                while (reader.Read())
                {
                    MemberToGroup member = new MemberToGroup
                    {
                        MemberId = (int)reader["MemberId"],
                        GroupId = (int)reader["GroupId"]
                    };

                    members.Add(member);
                }

                return members;
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public void DeleteMemberFromGroup(int memberId, int groupId)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM MemberToGroup WHERE MemberId=@MemberId AND GroupId=@GroupId";
                cmd.Parameters.AddWithValue("@MemberId", memberId);
                cmd.Parameters.AddWithValue("@GroupId", groupId);

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
        
        public void DeleteMemberFromAllGroup(int memberId)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM MemberToGroup WHERE MemberId=@MemberId";
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

        public bool IsMemberToGroupExist(int memberId, int groupId)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd .CommandType = CommandType.Text;
                cmd.CommandText = "SELECT COUNT(*) FROM MemberToGroup WHERE MemberId=@MemberId AND GroupId=@GroupId";
                cmd.Parameters.AddWithValue("@MemberId", memberId);
                cmd.Parameters.AddWithValue("@GroupId", groupId);

                int count = (int)cmd.ExecuteScalar();

                return count > 0;
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public void UpdateIsHidden(int memberId, List<Game> games)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "UPDATE MemberToGroup SET IsHidden=@IsHidden WHERE MemberId = @MemberId AND GroupId = @GroupId";

                foreach(Game game in games) {
                    cmd.Parameters.AddWithValue("@IsHidden", game.IsHidden);
                    cmd.Parameters.AddWithValue("@MemberId", memberId);
                    cmd.Parameters.AddWithValue("@GroupId", game.GroupId);

                    _ = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public void InsertMemberToGroup(MemberToGroup memberToGroup)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;

                if (memberToGroup.StudentRole != null)
                {
                    cmd.CommandText = "INSERT INTO MemberToGroup(MemberId, GroupId, StudentRole, IsHidden) VALUES(@MemberId, @GroupId, @StudentRole, @ISHidden)";
                    cmd.Parameters.AddWithValue("@StudentRole", memberToGroup.StudentRole);
                }
                else
                {
                    cmd.CommandText = "INSERT INTO MemberToGroup(MemberId, GroupId, IsHidden) VALUES(@MemberId, @GroupId, @ISHidden)";
                }

                cmd.Parameters.AddWithValue("@MemberId", memberToGroup.MemberId);
                cmd.Parameters.AddWithValue("@GroupId", memberToGroup.GroupId);
                cmd.Parameters.AddWithValue("@ISHidden", false);

                _ = cmd.ExecuteNonQuery();
            }
            finally
            {
                if (cmd != null)
                    cmd.Dispose();
            }
        }

        public void UpdateMemberToGroup(int memberId, int newGroupId, int oldGroupId)
        {
            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();

            SqlCommand cmd = null;
            try
            {
                cmd = _sqlConnection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "UPDATE MemberToGroup SET GroupId = @NewGroupId, IsHidden = @ISHidden WHERE MemberId = @MemberId AND GroupId = @OldGroupId";
                cmd.Parameters.AddWithValue("@MemberID", memberId);
                cmd.Parameters.AddWithValue("@NewGroupID", newGroupId);
                cmd.Parameters.AddWithValue("@OldGroupID", oldGroupId);
                cmd.Parameters.AddWithValue("@ISHidden", false);

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