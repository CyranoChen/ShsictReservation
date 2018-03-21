using System.Web.Script.Serialization;

namespace Shsict.Core.Extension
{
    public static class JsonExtensions
    {
        public static string ToJson(this object obj)
        {
            var jsonSerializer = new JavaScriptSerializer();

            if (obj != null)
            {
                return jsonSerializer.Serialize(obj);
            }
            else
            {
                return null;
            }
        }
    }
}