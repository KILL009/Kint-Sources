using System;

namespace OpenNos.Core.Handling
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class PacketAttribute : Attribute
    {
        #region Instantiation

        [Obsolete]
        public PacketAttribute(string header, int amount = 1)
        {
            Header = header;
            Amount = amount;
        }

        #endregion

        #region Properties

        public int Amount { get; }

        public string Header { get; }

        #endregion
    }
}