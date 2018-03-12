namespace OpenNos.Core
{
    public static class Extension
    {
        #region Methods

        public static string Truncate(this string str, int length)
        {
            return str.Length > length ? str.Substring(0, length) : str;
        }

        #endregion
    }
}