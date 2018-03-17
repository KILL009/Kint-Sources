using System;
using System.Xml.Serialization;

namespace OpenNos.XMLModel.Events
{
    [Serializable]
    public class ChangePortalType
    {
        #region Properties

        [XmlAttribute]
        public int IdOnMap { get; set; }

        [XmlAttribute]
        public sbyte Type { get; set; }

        #endregion
    }
}