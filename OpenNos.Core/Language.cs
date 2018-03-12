using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace OpenNos.Core
{
    public class Language
    {
        #region Members

        private static Language instance;
        private ResourceManager manager;
        private CultureInfo resourceCulture;

        #endregion

        #region Instantiation

        private Language()
        {
            resourceCulture = new CultureInfo(ConfigurationManager.AppSettings["Language"]);
            if (Assembly.GetEntryAssembly() != null)
            {
                manager = new ResourceManager(Assembly.GetEntryAssembly().GetName().Name + ".Resource.LocalizedResources", Assembly.GetEntryAssembly());
            }
        }

        #endregion

        #region Properties

        public static Language Instance => instance ?? (instance = new Language());

        #endregion

        #region Methods

        public string GetMessageFromKey(string message)
        {
            var resourceMessage = manager != null && message != null ? manager.GetString(message, resourceCulture) : string.Empty;

            return !string.IsNullOrEmpty(resourceMessage) ? resourceMessage : $"#<{message}>";
        }

        #endregion
    }
}