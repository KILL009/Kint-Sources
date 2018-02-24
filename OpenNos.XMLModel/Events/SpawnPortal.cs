using System;
using System.Xml.Serialization;

namespace OpenNos.XMLModel.Events
{
    [Serializable]
    public class SpawnPortal
    {
        #region Properties

        [XmlAttribute]
        public byte IdOnMap { get; set; }

        [XmlElement]
        public OnTraversal OnTraversal { get; set; }

        [XmlAttribute]
        public short PositionX { get; set; }

        [XmlAttribute]
        public short PositionY { get; set; }

        [XmlAttribute]
        public short ToMap { get; set; }

        [XmlAttribute]
        public short ToX { get; set; }

        [XmlAttribute]
        public short ToY { get; set; }

        [XmlAttribute]
        public short Type { get; set; }

        #endregion
    }
}