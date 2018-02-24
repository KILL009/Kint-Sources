using System;
using System.Xml.Serialization;

namespace OpenNos.XMLModel.Events
{
    [Serializable]
    public class Roam
    {
        #region Properties

        [XmlAttribute]
        public short FirstX { get; set; }

        [XmlAttribute]
        public short FirstY { get; set; }

        [XmlAttribute]
        public short SecondaryX { get; set; }

        [XmlAttribute]
        public short SecondaryY { get; set; }

        #endregion
    }
}