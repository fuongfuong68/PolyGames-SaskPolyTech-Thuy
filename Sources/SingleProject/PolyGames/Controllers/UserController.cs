using PolyGames.Common;
using PolyGames.Models;
using PolyGames.Services;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace PolyGames.Controllers
{
    [RoutePrefix("user")]
    public class UserController : WebController
    {
        private readonly UserService _userService;
        private readonly GameService _gameService;
        private readonly GameGroupService _gameGroupService;

        public UserController(PolyGameService polyGameService)
        {
            _userService = polyGameService.UserService;
            _gameService = polyGameService.GameService;
            _gameGroupService = polyGameService.GameGroupService;
        }

        [Route("user-dashboard")]
        public ActionResult UserDashboard()
        {
            if (!IsUser())
                return RedirectToAction("Login", "Home");

            User user_fromSession = GetUserSession();
            ViewBag.MemberId = user_fromSession.MemberID;
            ViewBag.Games = _gameService.GetUserDataById(user_fromSession.MemberID);

            return View();
        }

        [Route("update-password")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePassword(UpdatePasswordModel input)
        {
            if (!IsUser())
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Invalid";
                return RedirectToAction("UserDashboard");
            }

            ExecResult result = _userService.UpdatePassword(input);
            TempData["Message"] = result.Message;

            return RedirectToAction("UserDashboard");
        }

        [Route("update-is-hidden")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateIsHidden(List<Game> games)
        {
            if (!IsUser())
                return RedirectToAction("Login", "Home");

            _gameGroupService.UpdateIsHidden(GetUserSession().MemberID, games);

            TempData["Message"] = "Saved Successfully";
            return RedirectToAction("UserDashboard");
        }

        [Route("add-game")]
        public ActionResult AddGame()
        {
            if (!IsUser())
                return RedirectToAction("Login", "Home");

            List<Group> groups = _gameGroupService.GetGroupByMemberId(GetUserSession().MemberID);
            if(groups.Count == 0)
                return View("_NotFound");

            return View(new Game { GroupId = groups[0].GroupId, Year = DateTime.Now.Year });
        }

        [Route("add-game")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddGame(Game game)
        {
            if (!IsUser())
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Invalid";
                return View(game);
            }

            ExecResult result = _gameService.AddAGame(game, Server);
            ViewBag.Message = result.Message;

            return View(game);
        }

        [Route("edit-game/{gameId}")]
        public ActionResult EditGame(int gameId)
        {
            if (!IsUser())
                return RedirectToAction("Login", "Home");

            Game game = _gameService.GetGameById(gameId);
            if (game == null)
                return View("_NotFound");

            return View(game);
        }

        [Route("edit-game/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditGame(Game game)
        {
            if (!IsUser())
                return RedirectToAction("Login", "Home");

            if(!ModelState.IsValid)
            {
                ViewBag.Message = "Invalid";
                return View(game);
            }

            ExecResult result = _gameService.UpdateGame(game, Server);
            if (!result.Status)
            {
                ViewBag.Message = result.Message;
                return View(game);
            }

            return RedirectToAction("Game", "Home", new { id =  game.Id });
        }

        //Game.cshtml page - action when user clicks the delete link
        [Route("delete-game/{id}/{groupId}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteGame(int id, int groupId)
        {
            if(!IsUser())
                return RedirectToAction("Login", "Home");

            _gameService.DeleteGame(id, groupId, Server);
            TempData["Message"] = "Game deletion successful";

            return RedirectToAction("UserDashboard");
        }
    }
}