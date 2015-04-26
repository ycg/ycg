namespace Ycg.Extension
{
    public static class StringExtension
    {
        public static bool IsNull(this string value)
        {
            return value == null;
        }

        public static bool IsEmpty(this string value)
        {
            if (value.IsNull()) return true;
            return value.Length == 0;
        }
    }
}