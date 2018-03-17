using System;
using System.Xml.Serialization;

namespace OpenNos.XMLModel.Events
{
    [Serializable]
    public class OnCharacterDiscoveringMap
    {
        #region Properties

        [XmlElement]
        public GenerateMapClock GenerateMapClock { get; set; }

        [XmlElement]
        public NpcDialog NpcDialog { get; set; }

        [XmlElement]
        public OnMoveOnMap OnMoveOnMap { get; set; }

        [XmlElement]
        public SendMessage SendMessage { get; set; }

        [XmlElement]
        public SendPacket SendPacket { get; set; }

        [XmlElement]
        public SpawnPortal[] SpawnPortal { get; set; }

        [XmlElement]
        public SummonMonster[] SummonMonster { get; set; }

        [XmlElement]
        public SummonNpc[] SummonNpc { get; set; }

        #endregion
    }
}