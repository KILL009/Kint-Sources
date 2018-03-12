namespace OpenNos.Core
{
    public class SessionFactory
    {
        #region Members

        private static SessionFactory instance;
        private int sessionCounter;

        #endregion

        #region Instantiation

        private SessionFactory()
        {
        }

        #endregion

        #region Properties

        public static SessionFactory Instance => instance ?? (instance = new SessionFactory());

        #endregion

        #region Methods

        public int GenerateSessionId()
        {
            sessionCounter += 2;
            return sessionCounter;
        }

        #endregion
    }
}