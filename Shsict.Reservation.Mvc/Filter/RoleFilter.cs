using System.Web.Mvc;
using Shsict.Reservation.Mvc.Entities;
using Shsict.Reservation.Mvc.Services;

namespace Shsict.Reservation.Mvc.Filter
{
    public class AdminRoleAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var authorizedUser = new AuthorizeManager().GetSession();

            if (authorizedUser.Role.Equals(UserRoleEnum.Admin))
            {
                base.OnActionExecuting(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectResult("~/Account/Login");
            }
        }
    }

    public class CanteenRoleAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var authorizedUser = new AuthorizeManager().GetSession();

            if (authorizedUser.Role.Equals(UserRoleEnum.Canteen) || 
                authorizedUser.Role.Equals(UserRoleEnum.Admin))
            {
                base.OnActionExecuting(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectResult("~/Account/Login");
            }
        }
    }

    public class ManagerRoleAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var authorizedUser = new AuthorizeManager().GetSession();

            if (authorizedUser.Role.Equals(UserRoleEnum.Manager) ||
                authorizedUser.Role.Equals(UserRoleEnum.Admin))
            {
                base.OnActionExecuting(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectResult("~/Account/Login");
            }
        }
    }
}