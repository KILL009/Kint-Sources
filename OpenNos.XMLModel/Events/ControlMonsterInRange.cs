using System;
using System.Xml.Serialization;

namespace OpenNos.XMLModel.Events
{
    [Serializable]
    public class ControlMonsterInRange
    {
        #region Properties

        [XmlElement]
        public Move Move { get; set; }

        [XmlAttribute]
        public byte Range { get; set; }

        [XmlAttribute]
        public int VNum { get; set; }

        #endregion
    }
}