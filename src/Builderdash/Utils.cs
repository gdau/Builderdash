using System;
using System.Web.Script.Serialization;

namespace Builderdash
{
    public static class Utils
    {
        public static string ToJson(this object input)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(input);
        }

        public static T FromJson<T>(this string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                var serializer = new JavaScriptSerializer();
                try
                {
                    return serializer.Deserialize<T>(value);
                }
                catch (ArgumentException)
                {
                    return default(T);
                }
            }

            return default(T);
        }
    }
}