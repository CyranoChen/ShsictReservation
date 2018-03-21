using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using Shsict.Core;
using Shsict.Core.Dapper;
using Shsict.Core.Extension;
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
            var userWeChat = _repo.Single<UserWeChat>(x => x.UserName == userId);

            // 数据库中存在对应的企业号成员 且 7天内已更新过微信用户信息
            if (user != null && userWeChat?.LastAuthorizeDate >= DateTime.Today.AddDays(-7))
            {
                return SetSession(user.ID);
            }

            // 数据库中不存在对应的企业号成员，需要新增成员并持久化User, UserWeChat
            // 如数据库中存在，则同步成员信息，并更新时间戳
            user = SyncUserWithWeChat(userId, deviceId);

            // 设置授权Session
            return user != null && SetSession(user.ID);
        }

        public bool AuthorizeEmployee(string userId, string password)
        {
            using (IDapperHelper dapper = DapperHelper.GetInstance())
            {
                var trans = dapper.BeginTransaction();

                try
                {
                    var user = _repo.Query<User>(x => x.Password == Encrypt.GetMd5Hash(password)).Find(x =>
                        x.IsActive && (x.UserName.Equals(userId, StringComparison.OrdinalIgnoreCase) ||
                                       x.EmployeeNo.Equals(userId, StringComparison.OrdinalIgnoreCase)));

                    if (user != null)
                    {
                        user.LastLoginDate = DateTime.Now;

                        _repo.Update(user);

                        trans.Commit();

                        return SetSession(user.ID);
                    }
                }
                catch
                {
                    trans.Rollback();
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

        public bool SetSession(Guid userGuid)
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

                var userWeChat = _repo.Single<UserWeChat>(userGuid);

                if (userWeChat != null)
                {
                    u = userWeChat.MapTo(u);

                    //u.Avatar = userWeChat.Avatar;
                    //u.Gender = userWeChat.Gender;
                }

                // 设置授权Session
                HttpContext.Current.Session["AuthorizedUser"] = u;

                return true;
            }

            return false;
        }

        public User SyncUserWithWeChat(string userId, string deviceId = null)
        {
            // 调用Wechat接口，凭userid获取通讯录成员信息
            var client = new WeChatUserClient();
            var result = client.GetUser(userId);

            if (!string.IsNullOrEmpty(result))
            {
                var json = JToken.Parse(result);

                if (json["errcode"] != null && json["errmsg"] != null &&
                    json["errcode"].Value<int>() == 0 && json["errmsg"].Value<string>() == "ok")
                {
                    var user = new User
                    {
                        Password = Encrypt.GetMd5Hash("shsict"),
                        Email = string.Empty,
                        Role = UserRoleEnum.Employee,
                        CreateDate = DateTime.Now,
                        IsActive = true
                    };

                    var userWeChat = new UserWeChat();

                    // user: { "errcode":0,"errmsg":"ok","userid":"cyrano","name":"陈继麟","department":[17],"position":"技术工程师","mobile":"13818059707",
                    // "gender":"1","avatar":"http:\/\/shp.qpic.cn\/bizmp\/sfQa6NT594TUfQ42suia698Kz8KNY8eNmeogXYCNQicsaicnCMy5I1mfQ\/","status":1,"extattr":{"attrs":[]}}

                    // { "UserId":"xudanfu1015","DeviceId":"c90af29b945abf19b1a3cace63ac9d45"}
                    // user: { "errcode":0,"errmsg":"ok","userid":"xudanfu1015","name":"徐旦复","department":[34],"position":"系统开发主任","mobile":"13482045112",
                    // "gender":"1","avatar":"http:\/\/shp.qpic.cn\/bizmp\/sfQa6NT594Qm6CnIQicHUTLTDCib0QlmdrlfI3GIsLxknRhBYc7JFb2Q\/","status":1,
                    // "extattr":{"attrs":[{"name":"班组","value":"信息技术组"},{"name":"出生年月","value":"198509"},{"name":"政治面貌","value":"中共党员"},{"name":"工号","value":"1015"},{"name":"座机","value":""},{"name":"英文名","value":"xudanfu"}]}}

                    // 获得微信用户的扩展属性
                    var extattr = json["extattr"].Value<JToken>();
                    var attrs = extattr?["attrs"].Value<JArray>();

                    var userdict = new Dictionary<string, string>();

                    if (attrs?.Count > 0)
                    {
                        foreach (var kvp in attrs)
                        {
                            userdict.Add(kvp["name"].Value<string>(), kvp["value"].Value<string>());
                        }
                    }

                    // 通过userid，从数据库中获取对应用户
                    if (json["userid"] != null && _repo.Any<User>(x => x.UserName == json["userid"].Value<string>()))
                    {
                        user = _repo.Single<User>(x => x.UserName == json["userid"].Value<string>());
                    }

                    using (IDapperHelper dapper = DapperHelper.GetInstance())
                    {
                        var trans = dapper.BeginTransaction();

                        try
                        {
                            #region 封装 User 实例

                            user.UserName = json["userid"] != null ? json["userid"].Value<string>() : string.Empty;
                            user.EmployeeName = json["name"] != null ? json["name"].Value<string>() : string.Empty;
                            user.EmployeeNo = userdict.ContainsKey("工号") && userdict["工号"] != null ? userdict["工号"] : string.Empty;

                            if (json["department"]?.Value<JArray>() != null && json["department"].Value<JArray>().Count > 0)
                            {
                                user.Department = GetDepartment(json["department"].Value<JArray>()[0].Value<int>());
                            }
                            else
                            {
                                user.Department = string.Empty;
                            }

                            user.Team = userdict.ContainsKey("班组") && userdict["班组"] != null ? userdict["班组"] : string.Empty;
                            user.Position = json["position"] != null ? json["position"].Value<string>() : string.Empty;
                            user.Mobile = json["mobile"] != null ? json["mobile"].Value<string>() : string.Empty;
                            user.IsApproved = json["status"]?.Value<bool>() ?? true;
                            user.LastLoginDate = DateTime.Now;

                            user.Remark = json.ToString();

                            _repo.Save(user, out var key);

                            #endregion

                            #region 封装 UserWeChat 实例

                            userWeChat.ID = (Guid)key;
                            userWeChat.UserName = json["userid"] != null ? json["userid"].Value<string>() : string.Empty;
                            userWeChat.LastAuthorizeDate = DateTime.Now;
                            userWeChat.Gender = json["gender"]?.Value<short>() ?? -1;
                            userWeChat.Avatar = json["avatar"] != null ? json["avatar"].Value<string>() : string.Empty;

                            if (!string.IsNullOrEmpty(deviceId))
                            { userWeChat.DeviceId = deviceId; }

                            _repo.Save(userWeChat, out key);

                            #endregion

                            trans.Commit();

                            return user;
                        }
                        catch
                        {
                            trans.Rollback();
                        }
                    }
                }
            }

            return null;
        }

        private string GetDepartment(int id)
        {
            if (ConfigGlobal.WeChatActive && id > 0)
            {
                var client = new WeChatUserClient();

                var result = client.GetDepartmentList(id);

                // { "errcode": 0, "errmsg": "ok", "department": [ { "id": 2, "name": "广州研发中心", "parentid": 1, "order": 10 },
                //{ "id": 3, "name": "邮箱产品部",  "parentid": 2, "order": 40 } ]}

                if (!string.IsNullOrEmpty(result))
                {
                    var json = JToken.Parse(result);

                    if (json["errcode"] != null && json["errmsg"] != null &&
                        json["errcode"].Value<int>() == 0 && json["errmsg"].Value<string>() == "ok" &&
                        json["department"] != null)
                    {
                        var departments = json["department"].Value<JArray>();

                        if (departments != null && departments.Count > 0)
                        {
                            var d = departments.FirstOrDefault(x => x["id"].Value<int>() == id);

                            if (d != null)
                            {
                                return d["name"].Value<string>();
                            }
                        }
                    }
                }
            }

            return string.Empty;
        }
    }
}
