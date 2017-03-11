using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json.Linq;
using Shsict.Core;
using Shsict.Core.Utility;
using Shsict.Reservation.Mvc.Entities;
using Shsict.Reservation.Mvc.Services;

namespace Shsict.Reservation.Mvc.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account

        public ActionResult Index()
        {
            return View();
        }

        // GET: Account/Login

        public ActionResult Login()
        {
            return View();
        }

        // GET: Account/Detail

        public ActionResult Detail()
        {
            return View();
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

            return RedirectToAction("Login", "Account");
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
                            return RedirectToAction("Detail", "Account");
                        }
                    }

                    // 其他情况下均跳转登录界面
                }
            }

            // 授权失败跳转登录界面
            return RedirectToAction("Login", "Account");
        }


        // Get: Account/Debug

        public ActionResult Debug(string code, string state)
        {
            var authClient = new WeChatAuthClient();

            ViewBag.Code = code;
            ViewBag.State = state;

            ViewBag.Result = authClient.GetUserInfo(code);

            if (!string.IsNullOrEmpty(ViewBag.Result))
            {
                JToken json = JToken.Parse(ViewBag.Result.ToString());

                if (json["UserId"] != null && json["DeviceId"] != null)
                {
                    // 企业成员授权时返回 {"UserId":"cyrano","DeviceId":"3cc38f93c7d87eec0103c06feca4779f"}
                    var userid = json["UserId"].Value<string>();
                    var deviceId = json["DeviceId"].Value<string>();

                    var userclient = new WeChatUserClient();

                    ViewBag.User = userclient.GetUser(userid);
                }
            }

            ViewBag.AccessToken = HttpContext?.Cache["AccessToken"].ToString();

            return View();
        }


        // Get: Account/DebugRegister

        public ActionResult DebugRegister()
        {
            var result = new { UserId = "cyrano", DeviceId = "3cc38f93c7d87eec0103c06feca4779f" }.ToJson();

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

            // 授权失败跳转登录界面
            return RedirectToAction("Login", "Account");
        }
    }
}