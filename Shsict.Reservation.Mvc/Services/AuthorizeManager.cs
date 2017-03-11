using System;
using System.Web;
using Newtonsoft.Json.Linq;
using Shsict.Core;
using Shsict.Core.Utility;
using Shsict.Reservation.Mvc.Entities;
using Shsict.Reservation.Mvc.Models;

namespace Shsict.Reservation.Mvc.Services
{
    public class AuthorizeManager
    {
        private IRepository _repo => new Repository();

        public bool AuthorizeUser(string userId, string deviceId)
        {
            var user = _repo.Single<User>(x => x.UserName == userId);

            if (user != null)
            {
                // 数据库中存在对应的企业号成员
                return SetSession(user.ID);
            }

            // 数据库中不存在对应的企业号成员，需要新增成员并持久化User, UserWeChat

            // 调用Wechat接口，凭userid获取通讯录成员信息
            var client = new WeChatUserClient();
            var result = client.GetUser(userId);

            if (!string.IsNullOrEmpty(result))
            {
                var json = JToken.Parse(result);

                if (json["errcode"] != null && json["errmsg"] != null &&
                    json["errcode"].Value<int>() == 0 && json["errmsg"].Value<string>() == "ok")
                {
                    // 注册企业成员信息
                    user = Register(json, deviceId);

                    if (user != null)
                    {
                        // 设置授权Session
                        return SetSession(user.ID);
                    }
                }
            }

            return false;
        }


        public bool AuthorizeGuest(string openid, string deviceId)
        {
            var user = _repo.Single<User>(x => x.WeChatOpenId == openid);

            if (user != null)
            {
                // 数据库中存在对应的企业号关注者
                return SetSession(user.ID);
            }

            // 数据库中不存在对应的企业号关注者，需要新增成员并持久化User, UserWeChat

            // TODO

            return false;
        }

        public UserDto GetSession()
        {
            if (HttpContext.Current.Session["AuthorizedUser"] != null)
            {
                return HttpContext.Current.Session["AuthorizedUser"] as UserDto;
            }

            // 通过 User.Indentity.Name 获得用户对象
            if (!string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
            {
                var user = _repo.Single<User>(x => x.UserName == HttpContext.Current.User.Identity.Name);

                if (user != null && SetSession(user.ID))
                {
                    return HttpContext.Current.Session["AuthorizedUser"] as UserDto;
                }
            }

            return null;
        }

        private bool SetSession(Guid userGuid)
        {
            var user = _repo.Single<User>(userGuid);

            if (user != null)
            {
                // 更新用户登录与授权时间
                user.LastLoginDate = DateTime.Now;

                _repo.Update(user);

                // 映射到UserDto
                var u = user.MapTo<User, UserDto>();
                u.UserId = user.UserName;

                // 设置授权Session
                HttpContext.Current.Session["AuthorizedUser"] = u;

                return true;
            }

            return false;
        }

        private User Register(JToken json, string deviceId)
        {
            var user = new User();
            var userWeChat = new UserWeChat();

            using (var trans = DapperHelper.MarsConnection.BeginTransaction())
            {
                try
                {
                    // user: { "errcode":0,"errmsg":"ok","userid":"cyrano","name":"陈继麟","department":[17],"position":"技术工程师","mobile":"13818059707",
                    // "gender":"1","avatar":"http:\/\/shp.qpic.cn\/bizmp\/sfQa6NT594TUfQ42suia698Kz8KNY8eNmeogXYCNQicsaicnCMy5I1mfQ\/","status":1,"extattr":{"attrs":[]}}

                    #region 封装 User 实例

                    user.UserName = json["userid"] != null ? json["userid"].Value<string>() : string.Empty;
                    user.Password = Encrypt.GetMd5Hash("shsict");
                    user.EmployeeName = json["name"] != null ? json["name"].Value<string>() : string.Empty;
                    user.EmployeeNo = string.Empty; // TODO
                    user.Department = string.Empty; // TODO
                    user.Team = string.Empty; // TODO
                    user.Position = json["position"] != null ? json["position"].Value<string>() : string.Empty;
                    user.Email = string.Empty; // TODO
                    user.Mobile = json["mobile"] != null ? json["mobile"].Value<string>() : string.Empty;
                    user.IsApproved = json["status"]?.Value<bool>() ?? true;
                    user.LastLoginDate = DateTime.Now;
                    user.CreateDate = DateTime.Now;
                    user.IsActive = true;
                    user.Remark = json.ToString();

                    object key;

                    _repo.Insert(user, out key, trans);

                    #endregion

                    #region 封装 UserWeChat 实例

                    userWeChat.ID = (Guid)key;
                    userWeChat.UserName = json["userid"] != null ? json["userid"].Value<string>() : string.Empty;
                    userWeChat.LastAuthorizeDate = DateTime.Now;
                    userWeChat.Gender = json["gender"]?.Value<short>() ?? -1;
                    userWeChat.Avatar = json["avatar"] != null ? json["avatar"].Value<string>() : string.Empty;
                    userWeChat.DeviceId = deviceId;

                    _repo.Insert(userWeChat, out key, trans);

                    #endregion

                    trans.Commit();

                    return user;
                }
                catch
                {
                    trans.Rollback();

                    return null;
                }
            }
        }
    }
}
