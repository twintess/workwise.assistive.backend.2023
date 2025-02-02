namespace utils.mrds.net
{
    public static class DbUtils
    {
        public static string CheckString(string? str)
        {
            try
            {
                return string.IsNullOrEmpty(str) ? string.Empty : str;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}