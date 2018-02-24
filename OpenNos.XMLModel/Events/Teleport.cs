using System;
using System.Xml.Serialization;

namespace OpenNos.XMLModel.Events
{
    [Serializable]
    public class Teleport
    {
        #region Properties

        [XmlAttribute]
        public short DestinationX { get; set; }

        [XmlAttribute]
        public short DestinationY { get; set; }

        [XmlAttribute]
        public short PositionX { get; set; }

        [XmlAttribute]
        public short PositionY { get; set; }

        #endregion
    }
}