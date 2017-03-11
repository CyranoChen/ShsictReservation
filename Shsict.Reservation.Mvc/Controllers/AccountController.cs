using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Newtonsoft.Json.Linq;
using Shsict.Reservation.Mvc.Entities;
using Shsict.Core.Utility;
using Shsict.Reservation.Mvc.Models;
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

        // Get: Account

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

        // 
        // GET: /Account/WeChatLogin

        public ActionResult WeChatLogin()
        {
            if (HttpContext.Request.Url != null)
            {
                var client = new WeChatAuthClient();

                var authUri = client.GetOAuthUrl($"http://{HttpContext.Request.Url.Authority}/Account/WeChatAuth", ScopeType.snsapi_base, "ShsictReservation");

                if (!string.IsNullOrEmpty(authUri))
                {
                    return Redirect(authUri);
                }
            }

            return RedirectToAction("Index", "Account");
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

                        // TODO Autherize

                    }
                    else if (json["OpenId"] != null && json["DeviceId"] != null)
                    {
                        // 非企业成员授权时返回 
                        var openId = json["OpenId"].Value<string>();
                        var deviceId = json["DeviceId"].Value<string>();

                        // TODO Autherize

                    }
                    else if (json["errcode"] != null && json["errmsg"] != null)
                    {
                        // TODO Exception
                    }
                }

                ViewBag.Result = result;


                return View();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}