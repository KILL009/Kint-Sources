namespace OpenNos.Core
{
    public class Singleton<T> where T : class, new()
    {
        #region Members

        private static T instance;

        #endregion

        #region Properties

        public static T Instance => instance ?? (instance = new T());

        #endregion
    }
}