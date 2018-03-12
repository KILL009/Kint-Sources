using System.IO;
using System.Reflection;

namespace OpenNos.Core
{
    public class Application
    {
        #region Methods

        public static string AppPath(bool backSlash = true)
        {
            var text = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            if (backSlash)
            {
                text += @"\";
            }

            return text;
        }

        #endregion
    }
}