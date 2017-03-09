using System;
using System.Web;
using System.Web.Caching;
using Newtonsoft.Json.Linq;
using Shsict.Core;
using Shsict.Reservation.Mvc.Entities;

namespace Shsict.Reservation.Mvc.Services
{
    public class WeChatApiClient : RestClient
    {
        protected WeChatApiClient()
        {
            ServiceUrl = ConfigGlobal.WeChatServiceUrl;
            AppKey = ConfigGlobal.WeChatAppKey;
            CryptographicKey = ConfigGlobal.WeChatAppSecret;
        }

        #region Members and Properties

        protected string AccessToken { get; set; }

        #endregion

        protected void Init()
        {
            if (!ConfigGlobal.WeChatActive) { return; }

            var context = HttpContext.Current;

            // Get access_token by using System.Web.Caching
            if (context?.Cache["AccessToken"] != null)
            {
                AccessToken = context.Cache["AccessToken"].ToString();
            }
            else
            {
                // Get access token
                // http请求方式: GET
                // https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid=id&corpsecret=secrect
                // {"access_toke{ "access_token": "accesstoken000001", "expires_in": 7200}

                var uri = $"{ServiceUrl}gettoken?corpid={AppKey}&corpsecret={CryptographicKey}";

                var responseResult = ApiGet(uri);

                if (string.IsNullOrEmpty(responseResult))
                {
                    throw new Exception("WeChatApiClient.Init() responseResult is null");
                }

                var json = JToken.Parse(responseResult);

                if (json["access_token"] != null && json["expires_in"] != null)
                {
                    // Set access_token by using System.Web.Caching
                    AccessToken =
                        AddItemToCache("AccessToken", json["access_token"], json["expires_in"].Value<double>())
                            .ToString();
                }
            }
        }

        private object AddItemToCache(string key, object value, double expires)
        {
            if (HttpContext.Current != null && HttpContext.Current.Cache[key] == null)
            {
                HttpContext.Current.Cache.Add(key, value, null, DateTime.Now.AddSeconds(expires), TimeSpan.Zero,
                    CacheItemPriority.High, null);
            }

            return value;
        }
    }

    public enum ScopeType
    {
        snsapi_base,
        snsapi_userinfo
    }
}