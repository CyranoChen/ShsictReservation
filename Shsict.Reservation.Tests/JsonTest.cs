using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Shsict.Core;

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
    }
}
