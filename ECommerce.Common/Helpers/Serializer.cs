using Newtonsoft.Json;

namespace ECommerce.Common.Helpers
{
    public static class Serializer
    {
        private static readonly JsonSerializerSettings IgnoreNullJsonSerializerSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        private static readonly JsonSerializerSettings IncludeNullJsonSerializerSettings = new()
        {

            NullValueHandling = NullValueHandling.Include
        };

        public static string SerializeObject<T>(T objectContent, bool ignoreNull)
        {
            return JsonConvert.SerializeObject(objectContent, Formatting.None, ignoreNull ?
            IgnoreNullJsonSerializerSettings : IncludeNullJsonSerializerSettings);
        }
    }
}
