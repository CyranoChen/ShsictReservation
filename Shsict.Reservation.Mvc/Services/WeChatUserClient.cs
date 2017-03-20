using System;
using System.Text;
using System.Web;
using Shsict.Reservation.Mvc.Entities;

namespace Shsict.Reservation.Mvc.Services
{
    public class WeChatUserClient : WeChatApiClient
    {
        public string GetUser(string userid)
        {
            if (!ConfigGlobal.WeChatActive) { return null; }

            //https://qyapi.weixin.qq.com/cgi-bin/user/get?access_token=ACCESS_TOKEN&userid=USERID

            var uri = $"{ServiceUrl}user/get?access_token={AccessToken}&userid={userid}";

            var responseResult = ApiGet(uri);

            if (string.IsNullOrEmpty(responseResult))
            {
                throw new Exception("WeChatUserClient.GetUser() responseResult is null");
            }

            return responseResult;
        }

        public string GetDepartmentList(int departmentid)
        {
            if (!ConfigGlobal.WeChatActive) { return null; }
            
            //https://qyapi.weixin.qq.com/cgi-bin/department/list?access_token=ACCESS_TOKEN&id=ID

            var uri = $"{ServiceUrl}department/list?access_token={AccessToken}&userid={departmentid}";

            var responseResult = ApiGet(uri);

            if (string.IsNullOrEmpty(responseResult))
            {
                throw new Exception("WeChatUserClient.GetDepartmentList() responseResult is null");
            }

            return responseResult;
        }
    }
}