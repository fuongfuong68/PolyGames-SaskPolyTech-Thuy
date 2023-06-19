using PolyGames.Common;
using PolyGames.DAO;
using PolyGames.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace PolyGames.Services
{
    public class UserService
    {
        private readonly SmtpService _smtpService;

        private readonly UserDAO _userDAO;
        private readonly GameGroupDAO _gameGroupDAO;

        public UserService(UserDAO userDAO, GameGroupDAO gameGroupDAO, SmtpService smtpService)
        {
            _userDAO = userDAO;
            _gameGroupDAO = gameGroupDAO;
            _smtpService = smtpService;
        }

        public ExecResult AddUser(NewUserModel newUser)
        {
            bool checkExists = _userDAO.CheckUserExists(new User { Email = newUser.Email });
            if (checkExists)
                return new ExecResult { Status = false, Message = "User account already exists" };

            newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
            _userDAO.AddUser(newUser);
            User user_fromDb = _userDAO.GetUserByEmail(newUser.Email);

            _gameGroupDAO.InsertMemberToGroup(new MemberToGroup
            {
                MemberId = user_fromDb.MemberID,
                GroupId = newUser.GroupId
            });

            return new ExecResult { Status = true, Message = "Student added succesfully!" };
        }

        public User GetUserById(int id)
        {
            User user = _userDAO.GetUserById(id);
            if (user == null)
                return null;

            user.CurrentTeamName = _gameGroupDAO.GetGroupByMemberId(user.MemberID);

            return user;
        }

        public User GetUserByEmail(string email)
        {
            User user = _userDAO.GetUserByEmail(email);
            if (user == null)
                return null;

            user.CurrentTeamName = _gameGroupDAO.GetGroupByMemberId(user.MemberID);

            return user;
        }

        public List<User> GetAllUsers()
        {
            List<User> users = _userDAO.GetAllUsers();

            users.ForEach((x) => { 
                x.CurrentTeamName = _gameGroupDAO.GetGroupByMemberId(x.MemberID); 
            });

            return users;
        }

        public List<User> GetUserByFilters(GameFilter gameFilter)
        {
            List<User> users = _userDAO.GetUserByFilters(gameFilter);

            users.ForEach((x) => {
                x.CurrentTeamName = _gameGroupDAO.GetGroupByMemberId(x.MemberID);
            });

            return users;
        }

        public List<int> GetDistinctRegistrationYear()
        {
            return _userDAO.GetDistinctRegistrationYear();
        }

        public void UpdateUser(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            _userDAO.UpdateUser(user);
        }

        public ExecResult UpdatePassword(UpdatePasswordModel input)
        {
            User user = _userDAO.GetUserById(input.ID);
            if (user == null)
                return new ExecResult { Status = false, Message = "Not found" };

            if (!BCrypt.Net.BCrypt.Verify(input.OldPassword, user.Password))
                return new ExecResult { Status = false, Message = "Password did not matched" };

            _userDAO.UpdatePassword(input.ID, BCrypt.Net.BCrypt.HashPassword(input.NewPassword));

            return new ExecResult { Status = true, Message = "Password succesfully changed" };
        }

        public ExecResult<User> UpdatePassword(NewPasswordModel input)
        {
            User user = _userDAO.GetUserById(input.UserId);
            if (user == null)
                return new ExecResult<User> { Status = false, Message = "Not found" };

            user.PasswordResetRequest = false;
            user.Password = BCrypt.Net.BCrypt.HashPassword(input.Password);
            _userDAO.UpdateUser(user);

            return new ExecResult<User> { 
                Status = true, 
                Message = "Password succesfully changed", 
                Data = user 
            };
        }

        public void DeleteUser(int memberId)
        {
            _gameGroupDAO.DeleteMemberFromAllGroup(memberId);
            _userDAO.DeleteUser(memberId);
        }

        public ExecResult Register(RegisterModel registerModel)
        {
            bool isExists = _userDAO.CheckUserExists(new User { Email = registerModel.Email });
            if (isExists)
                return new ExecResult { Status = false, Message = "Email already exists" };

            AccessCode accessCode = _userDAO.GetAccessCode(registerModel.Code);
            if (accessCode == null)
                return new ExecResult { Status = false, Message = "Invalid code" };

            if (accessCode.IsExpired)
                return new ExecResult { Status = false, Message = "The code has expired" };

            _userDAO.AddUser(new NewUserModel
            {
                Name = registerModel.Name,
                Email = registerModel.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerModel.Password),
                IsActive = true,
                IsAdmin = false,
            });

            return new ExecResult { Status = true, Message = "Successfully registered" };
        }

        public ExecResult<User> Login(User userLogin)
        {
            User user = _userDAO.GetUserByEmail(userLogin.Email);
            if (user == null)
                return new ExecResult<User> { Status = false, Message = "Not found" };

            if (BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password))
            {
                user.Password = null;
                return new ExecResult<User> { Status = true, Message = "Successfully logged in", Data = user };
            }

            return new ExecResult<User> { Status = false, Message = "Wrong password" };
        }

        public ExecResult ForgotPasswordStep1(string email, HttpSessionStateBase session)
        {
            User user = _userDAO.GetUserByEmail(email);
            if (user == null)
                return new ExecResult { Status = false, Message = "Not found" };

            string verificationCode = RandomAccessCode.GenerateAccessCode(8);
            session["User_VerificationCode"] = verificationCode;

            StringBuilder mailContentBuilder = new StringBuilder();
            mailContentBuilder.Append("<p>I hope this email finds you well. We are reaching out to you to verify your account for enhanced security measures. As part of our ongoing efforts to protect your personal information, we require you to provide a verification code to confirm your identity.</p>");
            mailContentBuilder.Append("<p>Please find below the unique verification code you will need to enter:</p>");
            mailContentBuilder.Append("<br />");
            mailContentBuilder.Append($"<p style=\"text-align: center;\">Verification Code: <span style=\"color: red;\">{verificationCode}</span></p>");

            _smtpService.Send(email, "Reset password", mailContentBuilder.ToString());

            return new ExecResult<string> { 
                Status = true, 
                Message = "We have sent an email containing a verification code to reset your account password. Please enter the correct code to complete the process.", 
                Data = verificationCode 
            };
        }

        public ExecResult ForgotPasswordStep2(ForgotPasswordModel forgotPasswordModel, HttpSessionStateBase session)
        {
            User user = _userDAO.GetUserByEmail(forgotPasswordModel.Email);
            if (user == null)
                return new ExecResult<string> { Status = false, Message = "Not found" };

            if (forgotPasswordModel.VerificationCode != session["User_VerificationCode"] as string)
                return new ExecResult { Status = true, Message = "" };

            _userDAO.UpdatePassword(user.MemberID, BCrypt.Net.BCrypt.HashPassword(forgotPasswordModel.Password));

            return new ExecResult { Status = true, Message = "New password set successfully" };
        }

        public int CountUsers()
        {
            return _userDAO.CountUsers();
        }

        public AccessCode GenerateAccessCode(int length)
        {
            string newCode = RandomAccessCode.GenerateAccessCode(length);
            _userDAO.AddAccessCode(newCode);

            AccessCode accessCode = new AccessCode
            {
                Code = newCode,
                ExpirationDate = DateTime.Now.AddDays(3),
                IsExpired = false,
            };

            return accessCode;
        }

        public List<AccessCode> GetAllAccessCodes()
        {
            return _userDAO.GetAllAccessCodes();
        }

        public AccessCode GetAccessCode(string code)
        {
            return _userDAO.GetAccessCode(code);
        }

        public void DeleteExpiredCodes()
        {
            _userDAO.DeleteExpiredCodes();
        }
    }
}