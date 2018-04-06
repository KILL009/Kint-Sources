using System;
using System.Xml.Serialization;

namespace OpenNos.XMLModel.Objects
{
    [Serializable]
    public class Item
    {
        #region Properties

        [XmlAttribute]
        public byte Amount { get; set; }

        [XmlAttribute]
        public int Volume { get; set; }

        [XmlAttribute]
        public short Design { get; set; }

        [XmlAttribute]
        public bool IsRandomRare { get; set; }

        [XmlAttribute]
        public short VNum { get; set; }

        #endregion
    }
}