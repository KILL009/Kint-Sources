using System;
using System.Xml.Serialization;

namespace OpenNos.XMLModel.Events
{
    [Serializable]
    public class OnMapClean
    {
        #region Properties

        [XmlElement]
        public ChangePortalType[] ChangePortalType { get; set; }

        [XmlElement]
        public NpcDialog NpcDialog { get; set; }

        [XmlElement]
        public object RefreshMapItems { get; set; }

        [XmlElement]
        public SendMessage SendMessage { get; set; }

        [XmlElement]
        public SendPacket SendPacket { get; set; }

        #endregion
    }
}