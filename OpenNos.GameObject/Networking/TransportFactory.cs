namespace OpenNos.GameObject
{
    public class TransportFactory
    {
        #region Members

        private static TransportFactory instance;
        private long lastTransportId = 100000;

        #endregion

        #region Instantiation

        private TransportFactory()
        {
            // do nothing
        }

        #endregion

        #region Properties

        public static TransportFactory Instance => instance ?? (instance = new TransportFactory());

        #endregion

        #region Methods

        public long GenerateTransportId()
        {
            lastTransportId++;

            if (lastTransportId >= long.MaxValue)
            {
                lastTransportId = 0;
            }

            return lastTransportId;
        }

        #endregion
    }
}