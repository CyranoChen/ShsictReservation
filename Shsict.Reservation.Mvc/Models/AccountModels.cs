using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Web;
using Newtonsoft.Json.Linq;
using Shsict.Core;
using Shsict.Reservation.Mvc.Entities;
using Shsict.Reservation.Mvc.Services;

namespace Shsict.Reservation.Mvc.Models
{
    public class UserDto : User
    {
        public static User Single(object providerUserKey)
        {
            Contract.Requires(providerUserKey != null);

            IRepository repo = new Repository();

            return repo.Single<User>(providerUserKey);
        }

        public static User GetSession()
        {
            if (HttpContext.Current.Session["AuthorizedUser"] != null)
            {
                return HttpContext.Current.Session["AuthorizedUser"] as User;
            }
            // TODO
            //if (!string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
            //{
            //    // Get username from User.Indentity.Name
            //    var membership = MembershipDto.Single(HttpContext.Current.User.Identity.Name);

            //    if (membership == null)
            //    {
            //        return null;
            //    }

            //    SetSession(membership.ID);
            //    return HttpContext.Current.Session["AuthorizedUser"] as User;
            //}

            return null;
        }

        public static void SetSession(object providerUserKey, bool isAnonymous = false)
        {
            var user = Single(providerUserKey);

            // update user lastActivityDate & isAnonymous
            IRepository repo = new Repository();

            // TODO
            //user.LastActivityDate = DateTime.Now;
            //user.IsAnonymous = isAnonymous;

            repo.Update(user);

            // set user session
            HttpContext.Current.Session["AuthorizedUser"] = user;
        }
    }

    public class UserWeChatDto
    {
        public static UserWeChat Authorize(Guid userGuid, string accessToken, double expiresIn, string refreshToken,
            string openId, ScopeType scope, bool anonymous = false)
        {
            using (var trans = DapperHelper.MarsConnection.BeginTransaction())
            {
                try
                {
                    IRepository repo = new Repository();

                    // 保存微信用户
                    var user = anonymous ? repo.Single<User>(userGuid) : UserDto.GetSession();

                    if (user != null && user.ID == userGuid)
                    {
                        var u = new UserWeChat();

                        if (repo.Any<UserWeChat>(userGuid))
                        {
                            u = repo.Single<UserWeChat>(userGuid);
                        }

                        u.ID = userGuid;
                        u.UserName = user.UserName;
                        u.LastAuthorizeDate = DateTime.Now;

                        u.AccessToken = accessToken;
                        u.AccessTokenExpiredDate = DateTime.Now.AddSeconds(expiresIn);
                        u.RefreshToken = refreshToken;
                        u.RefreshTokenExpiredDate = DateTime.Now.AddDays(30);

                        u.Gender = 0;

                        if (u.Province == null) u.Province = string.Empty;
                        if (u.City == null) u.City = string.Empty;
                        if (u.Country == null) u.Country = string.Empty;
                        if (u.HeadImgUrl == null) u.HeadImgUrl = string.Empty;
                        if (u.Privilege == null) u.Privilege = string.Empty;
                        if (u.UnionID == null) u.UnionID = string.Empty;

                        repo.Save(u, trans);

                        // 更新普通用户
                        user.WeChatOpenID = openId;

                        // 按scope，获取微信用户详情
                        if (scope.Equals(ScopeType.snsapi_userinfo))
                        {
                            // TODO
                            var result = new WeChatAuthClient().GetUserInfo(openId);

                            if (!string.IsNullOrEmpty(result))
                            {
                                var json = JToken.Parse(result);

                                user.WeChatNickName = json["nickname"].Value<string>();

                                u.Gender = json["sex"].Value<short>();
                                u.Province = json["province"].Value<string>();
                                u.City = json["city"].Value<string>();
                                u.Country = json["country"].Value<string>();
                                u.HeadImgUrl = json["headimgurl"].Value<string>();
                                u.Privilege = json["privilege"].Value<JArray>().ToString();
                                u.UnionID = json["unionid"] != null ? json["unionid"].Value<string>() : string.Empty;

                                repo.Update(u, trans);
                            }
                        }

                        // 更新user的openId, nickname
                        repo.Update(user, trans);

                        trans.Commit();

                        return u;
                    }

                    return null;
                }
                catch
                {
                    trans.Rollback();

                    throw;
                }
            }
        }
    }

    public class MyAvatarDto
    {
        public IEnumerable<UserDto> Avatars { get; set; }
    }

    public class UserProfileDto
    {
        [Required(ErrorMessage = "请填写{0}")]
        [StringLength(20, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "真实姓名")]
        public string RealName { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "手机")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [DataType(DataType.EmailAddress, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "邮箱")]
        public string Email { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "请填写{0}")]
        [DataType(DataType.Password, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "当前密码")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [StringLength(100, ErrorMessage = "{0}长度至少需要{2}位", MinimumLength = 6)]
        [DataType(DataType.Password, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "新密码与确认密码不一致")]
        [DataType(DataType.Password, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "确认密码")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "请填写{0}")]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "记住密码")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "请填写{0}")]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [Display(Name = "邮箱")]
        [DataType(DataType.EmailAddress, ErrorMessage = "请正确填写{0}")]
        public string Email { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [StringLength(100, ErrorMessage = "{0}长度至少需要{2}位", MinimumLength = 7)]
        [DataType(DataType.Password, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "新密码与确认密码不一致")]
        [DataType(DataType.Password, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "密码确认")]
        public string ConfirmPassword { get; set; }
    }
}