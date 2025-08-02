namespace ECommerce.Common.Helpers
{
    public static class Parser
    {
        public static bool? ParseBoolean(this string? text)
        {
            return bool.TryParse(text, out var result) ? result : (bool?)null;
        }
        public static bool ParseBoolean(this string? text, bool valueIfError)
        {
            return bool.TryParse(text, out var result) ? result : valueIfError;
        }
        public static int? ParseInt(this string? text)
        {
            return int.TryParse(text, out var result) ? result : (int?)null;
        }
        public static int ParseInt(this string? text, int valueIfError)
        {
            return int.TryParse(text, out var result) ? result : valueIfError;
        }
        public static double? ParseDouble(this string? text)
        {
            return double.TryParse(text, out var result) ? result : (double?)null;
        }
        public static double ParseDouble(this string? text, double valueIfError)
        {
            return double.TryParse(text, out var result) ? result : valueIfError;
        }
    }
}
