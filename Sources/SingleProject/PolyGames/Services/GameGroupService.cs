using PolyGames.DAO;
using PolyGames.Models;
using System.Collections.Generic;

namespace PolyGames.Services
{
    public class GameGroupService
    {
        private readonly GameGroupDAO _gameGroupDAO;

        public GameGroupService(GameGroupDAO gameGroupDAO)
        {
            _gameGroupDAO = gameGroupDAO;
        }

        public int CountGroups()
        {
            return _gameGroupDAO.CountGroups();
        }

        public void AddGroup(Group group)
        {
            _gameGroupDAO.AddGroup(group);
        }

        public List<Student> GetGroupMembers(int groupId)
        {
            return _gameGroupDAO.GetGroupMembers(groupId);
        }

        public List<Group> GetGroupByMemberId(int memberId)
        {
            return _gameGroupDAO.GetGroupByMemberId(memberId);
        }

        public List<Group> GetAllGroups()
        {
            return _gameGroupDAO.GetAllGroups();
        }

        public List<MemberToGroup> GetMemberToGroupIds()
        {
            return _gameGroupDAO.GetMemberToGroupIds();
        }

        public void DeleteMemberFromGroup(int memberId, int groupId)
        {
            _gameGroupDAO.DeleteMemberFromGroup(memberId, groupId);
        }

        public void UpdateIsHidden(int memberId, List<Game> games)
        {
            _gameGroupDAO.UpdateIsHidden(memberId, games);
        }

        public void InsertMemberToGroup(MemberToGroup memberToGroup)
        {
            _gameGroupDAO.InsertMemberToGroup(memberToGroup);
        }

        public void UpdateMemberToGroup(int memberId, int newGroupId, int oldGroupId)
        {
            _gameGroupDAO.UpdateMemberToGroup(memberId, newGroupId, oldGroupId);
        }
    }
}