using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Shsict.Core;
using Shsict.Core.Dapper;
using Shsict.Core.Extension;

namespace Shsict.Reservation.Tests
{
    [TestClass]
    public class JsonTest
    {
        [TestMethod]
        public void JToken_GetValue_Test()
        {
            //{ "UserId":"cyrano","DeviceId":"3cc38f93c7d87eec0103c06feca4779f"}
            var result = new { UserId = "cyrano", DeviceId = "3cc38f93c7d87eec0103c06feca4779f" }.ToJson();

            var json = JToken.Parse(result);

            if (json["UserId"] != null && json["DeviceId"] != null)
            {
                var userid = json["UserId"].Value<string>();
                var deviceId = json["DeviceId"].Value<string>();

                Assert.AreEqual(userid, "cyrano");
                Assert.AreEqual(deviceId, "3cc38f93c7d87eec0103c06feca4779f");
            }
        }

        [TestMethod]
        [Ignore]
        public void JArray_GetValue_Test()
        {
            // { "UserId":"xudanfu1015","DeviceId":"c90af29b945abf19b1a3cace63ac9d45"}
            // user: { "errcode":0,"errmsg":"ok","userid":"xudanfu1015","name":"徐旦复","department":[34],"position":"系统开发主任","mobile":"13482045112",
            // "gender":"1","avatar":"http:\/\/shp.qpic.cn\/bizmp\/sfQa6NT594Qm6CnIQicHUTLTDCib0QlmdrlfI3GIsLxknRhBYc7JFb2Q\/","status":1,
            // "extattr":{"attrs":[{"name":"班组","value":"信息技术组"},{"name":"出生年月","value":"198509"},{"name":"政治面貌","value":"中共党员"},{"name":"工号","value":"1015"},{"name":"座机","value":""},{"name":"英文名","value":"xudanfu"}]}}

            var sql = "SELECT ConfigValue FROM Shsict_Config WHERE(ConfigSystem = 'Reservation') AND(ConfigKey = 'JsonDebug')";

            using (IDapperHelper dapper = DapperHelper.GetInstance())
            {
                var result = dapper.ExecuteScalar(sql).ToString();

                var json = JToken.Parse(result);

                var errcode = json["errcode"].Value<int>();
                var errmsg = json["errmsg"].Value<string>();
                var userid = json["userid"].Value<string>();
                var name = json["name"].Value<string>();
                var department = json["department"].Value<JArray>();
                var position = json["position"].Value<string>();
                var mobile = json["mobile"].Value<string>();
                var gender = json["gender"].Value<string>();
                var avatar = json["avatar"].Value<string>();
                var status = json["status"].Value<int>();
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

                Assert.AreEqual(errcode, 0);
                Assert.AreEqual(errmsg, "ok");
                Assert.AreEqual(userid, "xudanfu1015");
                Assert.AreEqual(name, "徐旦复");
                Assert.AreEqual(department[0].Value<int>(), 34);
                Assert.AreEqual(position, "系统开发主任");
                Assert.AreEqual(mobile, "13482045112");
                Assert.AreEqual(gender, "1");
                Assert.AreEqual(avatar, "http://shp.qpic.cn/bizmp/sfQa6NT594Qm6CnIQicHUTLTDCib0QlmdrlfI3GIsLxknRhBYc7JFb2Q/");
                Assert.AreEqual(status, 1);

                Assert.IsTrue(userdict.Count > 0);
                Assert.AreEqual(userdict["班组"], "信息技术组");
                Assert.AreEqual(userdict["出生年月"], "198509");
                Assert.AreEqual(userdict["政治面貌"], "中共党员");
                Assert.AreEqual(userdict["工号"], "1015");
                Assert.AreEqual(userdict["座机"], "");
                Assert.AreEqual(userdict["英文名"], "xudanfu");
            }
        }
    }
}
