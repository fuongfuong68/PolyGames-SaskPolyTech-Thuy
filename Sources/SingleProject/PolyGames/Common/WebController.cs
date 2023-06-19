using PolyGames.Models;
using System.Web.Mvc;

namespace PolyGames.Common
{
    public class WebController : Controller
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.HttpContext.IsDebuggingEnabled)
            {
                filterContext.ExceptionHandled = true;
                ViewResult viewResult = new ViewResult();
                viewResult.ViewName = "_Error";
                filterContext.Result = viewResult;
            }
        }

        protected void SetUserSession(User user)
        {
            Session["LoginSession"] = user;
        }

        protected void ClearUserSession()
        {
            Session["LoginSession"] = null;
        }

        protected User GetUserSession()
        {
            if (Session["LoginSession"] == null)
                return null;

            return Session["LoginSession"] as User;
        }

        protected bool IsAdmin()
        {
            User user_fromSession = GetUserSession();
            if (user_fromSession == null)
                return false;

            return user_fromSession.IsAdmin;
        }

        protected bool IsUser()
        {
            User user_fromSession = GetUserSession();
            if (user_fromSession == null)
                return false;

            return !user_fromSession.IsAdmin;
        }
    }
}