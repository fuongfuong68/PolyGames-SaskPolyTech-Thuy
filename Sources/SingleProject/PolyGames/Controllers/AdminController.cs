using PolyGames.Common;
using PolyGames.Models;
using PolyGames.Services;
using System.Net;
using System.Web.Mvc;

namespace PolyGames.Controllers
{
    [RoutePrefix("admin")]
    public class AdminController : WebController
    {
        private readonly GameService _gameService;
        private readonly UserService _userService;
        private readonly GameGroupService _gameGroupService;

        public AdminController(PolyGameService polyGameService)
        {
            _gameService = polyGameService.GameService;
            _userService = polyGameService.UserService;
            _gameGroupService = polyGameService.GameGroupService;
        }

        [Route("admin-dashboard")]
        public ActionResult AdminDashboard()
        {
            if(!IsAdmin())
                return RedirectToAction("Login", "Home");

            ViewBag.UserNumber = _userService.CountUsers();
            ViewBag.GameNumber = _gameService.CountGames();
            ViewBag.GroupNumber = _gameGroupService.CountGroups();

            return View();
        }

        [Route("user-manager")]
        public ActionResult LoadUserMgrView(int? editableOn, int? registrationYear, int? groupId, string focusOnElementId)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            if (registrationYear == null && groupId == null)
                ViewBag.Users = _userService.GetAllUsers();
            else
                ViewBag.Users = _userService.GetUserByFilters(new GameFilter
                {
                    RegistrationYear = registrationYear,
                    GroupId = groupId
                });

            ViewBag.EditableOn = editableOn;
            ViewBag.RegistrationYear = registrationYear;
            ViewBag.GroupId = groupId;
            ViewBag.FocusOnElementId = focusOnElementId;
            ViewBag.RegistrationYears = new SelectList(_userService.GetDistinctRegistrationYear());
            ViewBag.GroupList = new SelectList(_gameGroupService.GetAllGroups(), "GroupId", "GroupName");
            
            return View();
        }

        [Route("team-manger")]
        public ActionResult LoadTeamMgrView()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            return View();
        }

        [Route("game-manager")]
        public ActionResult LoadGameMgrView()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            ViewBag.Games = _gameService.GetAllGamesList();

            return View();
        }

        [Route("add-user")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(NewUserModel newUser)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Invalid";
                return RedirectToAction("LoadUserMgrView");
            }

            ExecResult execResult = _userService.AddUser(newUser);
            TempData["Message"] = execResult.Message;

            return RedirectToAction("LoadUserMgrView", "Admin");
        }

        [Route("add-group")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddGroup(Group group)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Failed to create the Group";
                return RedirectToAction("LoadTeamMgrView");
            }

            _gameGroupService.AddGroup(group);

            TempData["Message"] = "Group created";
            return RedirectToAction("LoadTeamMgrView");
        }

        [Route("edit-user/{memberId}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(int memberId, User user, int? registrationYear, int? filteredGroupId, string focusOnElementId)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            _userService.UpdateUser(user);

            TempData["Message"] = "User updated";
            return RedirectToAction("LoadUserMgrView", new { registrationYear = registrationYear, groupId = filteredGroupId, focusOnElementId = focusOnElementId });
        }

        [Route("delete-user/{memberId}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUser(int memberId)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            _userService.DeleteUser(memberId);

            TempData["Message"] = "User account deleted";
            return RedirectToAction("LoadUserMgrView");
        }

        [Route("delete-assign-group")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAssignedGroup(int memberId, int groupId, int? editableOn, int? registrationYear, int? filteredGroupId)
        {
            try
            {
                if (!IsAdmin())
                {
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return Json(new { Status = "UnAuthorized" });
                }   

                _gameGroupService.DeleteMemberFromGroup(memberId, groupId);

                return Json(new
                {
                    Status = "Success",
                    Url = Url.Action("LoadUserMgrView", new { editableOn = editableOn, registrationYear = registrationYear, groupId = filteredGroupId })
                });
            }
            catch
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Status = "Failed" });
            }
        }

        [Route("assign-new-group")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignNewGroup(MemberToGroup input, int? editableOn, int? registrationYear, int? filteredGroupId, string focusOnElementId)
        {
            try
            {
                if (!IsAdmin())
                {
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return Json(new { Status = "UnAuthorized" });
                }

                if (!ModelState.IsValid)
                    return Json(new { Status = "Invalid" });

                _gameGroupService.InsertMemberToGroup(input);
                return Json(new {
                    Status = "Success",
                    Url = Url.Action("LoadUserMgrView", new { editableOn = editableOn, registrationYear = registrationYear, groupId = filteredGroupId, focusOnElementId = focusOnElementId })
                });
            }
            catch
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Status = "Failed" });
            }
        }

        [Route("code-manager")]
        public ActionResult LoadCodeMgrView()
        {
            if(!IsAdmin())
                return RedirectToAction("Login", "Home");

            _userService.DeleteExpiredCodes();
            ViewBag.AccessCodes = _userService.GetAllAccessCodes();

            return View();
        }

        [Route("add-access-code")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAccessCode()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Invalid";
                return RedirectToAction("LoadUserMgrView");
            }

            AccessCode accessCode = _userService.GenerateAccessCode(10);
            TempData["Message"] = $"Access Code: {accessCode.Code} | ExpirationDate: {accessCode.ExpirationDate}";

            return RedirectToAction("LoadCodeMgrView");
        }
    }
}