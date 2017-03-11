using System;
using System.Text;
using System.Web;
using Shsict.Reservation.Mvc.Entities;

namespace Shsict.Reservation.Mvc.Services
{
    public class WeChatAuthClient : WeChatApiClient
    {
        public string GetOAuthUrl(string redirectUri, ScopeType scope, string state)
        {
            if (!ConfigGlobal.WeChatActive) { return null; }

            //https://open.weixin.qq.com/connect/oauth2/authorize?appid=CORPID&redirect_uri=REDIRECT_URI&response_type=code&scope=SCOPE&state=STATE#wechat_redirect

            var serverUrl = "https://open.weixin.qq.com/connect/oauth2/";

            return
                $"{serverUrl}authorize?appid={AppKey}&redirect_uri={HttpUtility.UrlEncode(redirectUri, Encoding.UTF8)}&response_type=code&scope={scope}&state={HttpUtility.UrlEncode(state, Encoding.UTF8)}#wechat_redirect";
        }

        public string GetUserInfo(string code)
        {
            if (!ConfigGlobal.WeChatActive) { return null; }

            //https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token=ACCESS_TOKEN&code=CODE

            var uri = $"{ServiceUrl}user/getuserinfo?access_token={AccessToken}&code={code}";

            var responseResult = ApiGet(uri);

            if (string.IsNullOrEmpty(responseResult))
            {
                throw new Exception("WeChatAuthClient.GetUserInfo() responseResult is null");
            }

            return responseResult;
        }
    }
}