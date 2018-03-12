using System;

namespace OpenNos.Core.Networking.Communication.ScsServices.Service
{
    /// <summary>
    /// Base class for all services that is serviced by IScsServiceApplication. A class must be
    /// derived from ScsService to serve as a SCS service.
    /// </summary>
    public abstract class ScsService
    {
        #region Members

        /// <summary>
        /// The current client for a thread that called service method.
        /// </summary>
        [ThreadStatic]
        private static IScsServiceClient currentClient;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current client which called this service method.
        /// </summary>
        /// <remarks>
        /// This property is thread-safe, if returns correct client when called in a service method
        /// if the method is called by SCS system, else throws exception.
        /// </remarks>
        public IScsServiceClient CurrentClient
        {
            get
            {
                return GetCurrentClient();
            }

            set
            {
                currentClient = value;
            }
        }

        #endregion

        #region Methods

        private IScsServiceClient GetCurrentClient()
        {
            if (currentClient != null)
            {
                return currentClient;
            }

            throw new ArgumentNullException("Client channel can not be obtained. CurrentClient property must be called by the thread which runs the service method.");
        }

        #endregion
    }
}