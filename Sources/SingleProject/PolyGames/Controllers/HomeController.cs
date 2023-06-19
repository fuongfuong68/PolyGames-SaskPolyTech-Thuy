using System.Collections.Generic;
using System.Web.Mvc;
using PolyGames.Models;
using System.IO;
using PolyGames.Common;
using System.Linq;
using PolyGames.Services;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace PolyGames.Controllers
{
    [RoutePrefix("")]
    public class HomeController : WebController
    {
        private readonly UserService _userService;
        private readonly GameService _gameService;
        private readonly GameGroupService _gameGroupService;

        public HomeController(PolyGameService polyGameService)
        {
            _userService = polyGameService.UserService;
            _gameService = polyGameService.GameService;
            _gameGroupService = polyGameService.GameGroupService;
        }

        //Home page
        [Route("")]
        public ActionResult Index()
        {
            ViewBag.Games = _gameService.GetGamesOrderedByMostRecentlyAdded();
            ViewBag.AllTeams = _gameGroupService.GetAllGroups();
            ViewBag.AllGames = _gameService.GetAllGames();

            return View();
        }

        //AllYears.cshtml action
        [Route("all-years")]
        public ActionResult AllYears()
        {
            List<Game> games = _gameService.GetAllYears();
            return View(games);
        }

        //AllGames.cshtml page action
        [Route("all-games")]
        public ActionResult AllGames()
        {
            List<Game> games = _gameService.GetAllGames();
            return View(games);
        }

        //GamesByYear.cshtml page action
        [Route("game-by-year/{year}")]
        public ActionResult GamesByYear(int year)
        {
            List<Game> games = _gameService.GetGamesByYear(year);
            return View(games);
        }

        //Main Game page where you can view and edit game details
        [Route("game/{id}")]
        public ActionResult Game(int id)
        {
            Game game = _gameService.GetGameById(id);
            if (game == null)
                return View("_NotFound");

            User user_fromSession = GetUserSession();
            if (user_fromSession == null)
                ViewBag.IsUserMatch = false;
            else
                ViewBag.IsUserMatch = game.GroupMembers.Any(x => x.MemberId == user_fromSession.MemberID);

            ViewBag.UserSession = user_fromSession;

            return View(game);
        }

        [Route("search")]
        public ActionResult Search(string keyword)
        {
            ViewBag.KeyWord = keyword;
            List<Game> games = _gameService.Search(keyword);

            return View(games);
        }

        //Game.cshtml page - action when user clicks the download link
        [Route("download-game/{id}")]
        public ActionResult DownloadGame(int id)
        {
            Game game = _gameService.GetGameById(id);
            string filePath = Server.MapPath(game.ExecutableFilePath);

            //Saves the executable file to the users Download directory
            return File(filePath, "application", Path.GetFileName(filePath));
        }

        [Route("login")]
        [HttpGet]
        public ActionResult Login()
        {
            if (IsAdmin())
                return RedirectToAction("AdminDashboard", "Admin");

            if (IsUser())
                return RedirectToAction("UserDashboard", "User");

            return View();
        }

        [Route("login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User user)
        {
            if (IsAdmin())
                return RedirectToAction("AdminDashboard", "Admin");

            if (IsUser())
                return RedirectToAction("UserDashboard", "User");

            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Invalid email or password";
                return View(user);
            }

            ExecResult<User> result = _userService.Login(user);
            if (!result.Status)
            {
                ViewBag.Message = result.Message;
                return View(user);
            }

            if (!result.Data.IsActive)
            {
                ViewBag.Message = "Your Account is inactive. Please contact your instructor.";
                return View(user);
            }

            if (result.Data.RegistrationDate.Value.AddHours(24) < System.DateTime.Now && result.Data.PasswordResetRequest)
            {
                ViewBag.Message = "Your account has been inactive for more than 24 hours. Please contact your instructor to reactivate it.";
                return View(user);
            }

            if (result.Data.RegistrationDate.Value.AddHours(24) > System.DateTime.Now && result.Data.PasswordResetRequest)
            {
                TempData["Message"] = "Your account requires a password change. Please contact your instructor for further instruction.";
                return RedirectToAction("NewPassword", new { userId = result.Data.MemberID });
            }

            SetUserSession(result.Data);

            if (result.Data.IsAdmin)
                return RedirectToAction("AdminDashboard", "Admin");

            return RedirectToAction("UserDashboard", "User");
        }

        [Route("new-password")]
        [HttpGet]
        public ActionResult NewPassword(int userId)
        {
            User user = _userService.GetUserById(userId);
            if (user == null)
                return View("_NotFound");

            return View(new NewPasswordModel { UserId = user.MemberID });
        }

        [Route("new-password")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewPassword(NewPasswordModel input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Invalid";
                return View(input);
            }

            ExecResult<User> result = _userService.UpdatePassword(input);
            if (!result.Status)
            {
                ViewBag.Message = result.Message;
                return View(input);
            }

            if (!result.Data.IsActive)
            {
                ViewBag.Message = "Your Account is inactive. Please contact your instructor.";
                return View(input);
            }

            SetUserSession(result.Data);

            if (result.Data.IsAdmin)
                return RedirectToAction("AdminDashboard", "Admin");

            return RedirectToAction("UserDashboard", "User");
        }

        [Route("forgot-password")]
        public ActionResult ForgotPasswordStep1()
        {
            return View();
        }

        [Route("forgot-password")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPasswordStep1([Required] string email)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Invalid";
                return View();
            }

            ExecResult result = _userService.ForgotPasswordStep1(email, Session);
            if (!result.Status)
            {
                ViewBag.Message = result.Message;
                return View();
            }

            TempData["Message"] = result.Message;

            return RedirectToAction("ForgotPasswordStep2", new { email = email });
        }

        [Route("forgot-password-step2")]
        public ActionResult ForgotPasswordStep2(string email)
        {
            return View(new ForgotPasswordModel { Email = email });
        }

        [Route("forgot-password-step2")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPasswordStep2(ForgotPasswordModel input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Invalid";
                return View();
            }

            ExecResult result = _userService.ForgotPasswordStep2(input, Session);
            if (!result.Status)
            {
                ViewBag.Message = result.Message;
                return View();
            }

            TempData["Message"] = result.Message;

            return RedirectToAction("Login");
        }

        [Route("check-verification-code")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckVerificationCode(string code)
        {
            try
            {
                if (code == Session["User_VerificationCode"] as string)
                    return Json(new { Status = "Matched" });

                return Json(new { Status = "NotMatched" });
            }
            catch
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Status = "Failed" });
            }
        }

        [Route("register")]
        public ActionResult Register()
        {
            if (IsAdmin())
                return RedirectToAction("AdminDashboard", "Admin");

            if (IsUser())
                return RedirectToAction("UserDashboard", "User");

            return View();
        }

        [Route("register")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel input)
        {
            if (IsAdmin())
                return RedirectToAction("AdminDashboard", "Admin");

            if (IsUser())
                return RedirectToAction("UserDashboard", "User");

            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Empty form can't be submitted";
                return View(input);
            }

            ExecResult result = _userService.Register(input);
            if (!result.Status)
            {
                ViewBag.Message = result.Message;
                return View(input);
            }

            TempData["Message"] = result.Message;
            return RedirectToAction("Login", "Home");
        }

        [Route("logout")]
        public ActionResult LogOut()
        {
            ClearUserSession();
            TempData["Message"] = "Successfully logged out";

            return RedirectToAction("Login");
        }
    }
}