using System;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json.Linq;
using Shsict.Core;
using Shsict.Core.Dapper;
using Shsict.Core.Utility;
using Shsict.Reservation.Mvc.Entities;
using Shsict.Reservation.Mvc.Models;
using Shsict.Reservation.Mvc.Services;

namespace Shsict.Reservation.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IRepository _repo = new Repository();

        // GET: Account
        [Authorize]
        public ActionResult Index()
        {
            var model = new AuthorizeManager().GetSession();

            return View(model);
        }


        // POST: /Account
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(UserDto model)
        {
            var authorizedUser = new AuthorizeManager().GetSession();

            if (ModelState.IsValid && authorizedUser.ID == model.ID)
            {
                try
                {
                    var user = _repo.Single<User>(model.ID);

                    // 设置model中的UserID
                    model.UserId = user.UserName;

                    user.EmployeeName = model.EmployeeName;
                    user.EmployeeNo = model.EmployeeNo;
                    user.Department = model.Department;
                    user.Position = model.Position;
                    user.Mobile = model.Mobile;
                    user.IsActive = true;

                    // 更新用户信息
                    _repo.Update(user);

                    // 更新session中的授权用户
                    var auth = new AuthorizeManager();

                    if (auth.SetSession(user.ID))
                    {
                        ModelState.AddModelError("Success", "保存成功");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Warn", ex.Message);
                }
            }

            return View(model);
        }


        // POST: /Account
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Password(AccountModels.PasswordDto model)
        {
            var authorizedUser = new AuthorizeManager().GetSession();

            if (ModelState.IsValid)
            {
                try
                {
                    var user = _repo.Single<User>(authorizedUser.ID);

                    if (user != null && user.Password == Encrypt.GetMd5Hash(model.OldPassword))
                    {
                        user.Password = Encrypt.GetMd5Hash(model.NewPassword);

                        // 更新用户信息
                        _repo.Update(user);

                        ModelState.AddModelError("Success", "保存成功");

                        return RedirectToAction("Logout", "Account");
                    }
                    else
                    {
                        ModelState.AddModelError("Error", "用户旧密码填写不正确");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Warn", ex.Message);
                }
            }

            return RedirectToAction("Index", "Account");
        }


        // GET: Account/Login

        public ActionResult Login(bool weChatRedirect = true, string returnUrl = null)
        {
            if (weChatRedirect)
            {
                if (ConfigGlobal.WeChatActive && BrowserInfo.IsWeChatClient())
                {
                    // 自动跳转微信认证机制
                    return RedirectToAction("WeChatLogin", "Account");
                }
            }

            ViewBag.ReturnUrl = returnUrl;

            // 直接打开登录界面
            return View();
        }


        // POST: /Account/Login

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(AccountModels.LoginDto model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var auth = new AuthorizeManager();

                if (auth.AuthorizeEmployee(model.UserId, model.Password))
                {
                    var u = auth.GetSession();

                    // Sign in
                    FormsAuthentication.SetAuthCookie(u.UserId, !string.IsNullOrEmpty(model.RememberMe));

                    // 食堂管理员和系统管理员跳后台管理界面
                    if (u.Role >= UserRoleEnum.Canteen)
                    {
                        if (Url.IsLocalUrl(returnUrl)) { return Redirect(returnUrl); }

                        return RedirectToAction("Index", "Home");
                    }

                    return RedirectToAction("Index", "Reservation");
                }
                else
                {
                    ModelState.AddModelError("Warn", "用户名或密码不正确");
                }
            }

            return View(model);
        }

        //
        // GET: /Account/Logout

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            return RedirectToAction("Login", "Account", new { weChatRedirect = false });
        }

        // 
        // GET: /Account/WeChatLogin

        public ActionResult WeChatLogin()
        {
            // 开启微信认证，并通过微信内部浏览器访问时，跳转微信OAuth认证接口
            if (HttpContext.Request.Url != null && ConfigGlobal.WeChatActive && BrowserInfo.IsWeChatClient())
            {
                var client = new WeChatAuthClient();

                var authUri = client.GetOAuthUrl($"http://{HttpContext.Request.Url.Authority}/Account/WeChatAuth",
                    ScopeType.snsapi_base, "ShsictReservation");

                if (!string.IsNullOrEmpty(authUri))
                {
                    return Redirect(authUri);
                }
            }

            return RedirectToAction("Login", "Account", new { weChatRedirect = false });
        }

        // 
        // GET: /Account/WeChatAuth

        public ActionResult WeChatAuth(string code, string state)
        {
            if (!string.IsNullOrEmpty(code) &&
                !string.IsNullOrEmpty(state) && state.Equals("ShsictReservation"))
            {
                // 获取微信授权access_token
                var client = new WeChatAuthClient();
                var result = client.GetUserInfo(code);

                if (!string.IsNullOrEmpty(result))
                {
                    var json = JToken.Parse(result);

                    if (json["UserId"] != null && json["DeviceId"] != null)
                    {
                        // 企业成员授权时返回 {"UserId":"cyrano","DeviceId":"3cc38f93c7d87eec0103c06feca4779f"}
                        var userid = json["UserId"].Value<string>();
                        var deviceId = json["DeviceId"].Value<string>();

                        // 授权当前企业号成员
                        var auth = new AuthorizeManager();

                        if (auth.AuthorizeUser(userid, deviceId))
                        {
                            // 设置Cookie
                            FormsAuthentication.SetAuthCookie(userid, true);

                            // 授权成功跳转订餐界面
                            return RedirectToAction("Index", "Reservation");
                        }
                    }
                    else if (json["OpenId"] != null && json["DeviceId"] != null)
                    {
                        // 非企业成员授权时返回 
                        var openId = json["OpenId"].Value<string>();
                        var deviceId = json["DeviceId"].Value<string>();

                        // 授权非企业号成员的关注者
                        var auth = new AuthorizeManager();

                        if (auth.AuthorizeGuest(openId, deviceId))
                        {
                            // 设置Cookie
                            FormsAuthentication.SetAuthCookie(openId, true);

                            // 授权成功跳转用户信息页（补充订餐必要信息）
                            return RedirectToAction("Index", "Account");
                        }
                    }

                    // 其他情况下均跳转登录界面
                }
            }

            // 授权失败跳转登录界面
            return RedirectToAction("Login", "Account", new { weChatRedirect = false });
        }


        #region 用户认证状态码

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "此用户名已存在";

                case MembershipCreateStatus.DuplicateEmail:
                    return "此邮箱已被其他用户注册使用";

                case MembershipCreateStatus.InvalidPassword:
                    return "此密码不符合密码规则";

                case MembershipCreateStatus.InvalidEmail:
                    return "此邮箱地址无效";

                case MembershipCreateStatus.InvalidAnswer:
                    return "此密码问题答案无效";

                case MembershipCreateStatus.InvalidQuestion:
                    return "此密码问题无效";

                case MembershipCreateStatus.InvalidUserName:
                    return "此用户名无效";

                case MembershipCreateStatus.ProviderError:
                    return "注册过程出错，请联系管理员";

                case MembershipCreateStatus.UserRejected:
                    return "此申请被取消";

                default:
                    return "发生未知错误，请联系管理员";
            }
        }

        #endregion


        // Get: Account/Debug

        //public ActionResult Debug(string code, string state)
        //{
        //    var authClient = new WeChatAuthClient();

        //    ViewBag.Code = code;
        //    ViewBag.State = state;

        //    ViewBag.Result = authClient.GetUserInfo(code);

        //    if (!string.IsNullOrEmpty(ViewBag.Result))
        //    {
        //        JToken json = JToken.Parse(ViewBag.Result.ToString());

        //        if (json["UserId"] != null && json["DeviceId"] != null)
        //        {
        //            // 企业成员授权时返回 {"UserId":"cyrano","DeviceId":"3cc38f93c7d87eec0103c06feca4779f"}
        //            var userid = json["UserId"].Value<string>();
        //            var deviceId = json["DeviceId"].Value<string>();

        //            var userclient = new WeChatUserClient();

        //            ViewBag.User = userclient.GetUser(userid);
        //        }
        //    }

        //    ViewBag.AccessToken = HttpContext?.Cache["AccessToken"].ToString();

        //    return View();
        //}


        // Get: Account/DebugRegister

        //public ActionResult DebugRegister()
        //{
        //    var result = new { UserId = "cyrano", DeviceId = "3cc38f93c7d87eec0103c06feca4779f" }.ToJson();

        //    var json = JToken.Parse(result);

        //    if (json["UserId"] != null && json["DeviceId"] != null)
        //    {
        //        // 企业成员授权时返回 {"UserId":"cyrano","DeviceId":"3cc38f93c7d87eec0103c06feca4779f"}
        //        var userid = json["UserId"].Value<string>();
        //        var deviceId = json["DeviceId"].Value<string>();

        //        // 授权当前企业号成员
        //        var auth = new AuthorizeManager();

        //        if (auth.AuthorizeUser(userid, deviceId))
        //        {
        //            // 设置Cookie
        //            FormsAuthentication.SetAuthCookie(userid, true);

        //            // 授权成功跳转订餐界面
        //            return RedirectToAction("Index", "Reservation");
        //        }
        //    }

        //    // 授权失败跳转登录界面
        //    return RedirectToAction("Login", "Account", new { weChatRedirect = false });
        //}
    }
}