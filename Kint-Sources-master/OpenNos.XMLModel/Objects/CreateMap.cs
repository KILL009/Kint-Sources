using OpenNos.XMLModel.Events;
using System;
using System.Xml.Serialization;

namespace OpenNos.XMLModel.Objects
{
    [Serializable]
    public class CreateMap
    {
        #region Properties

        [XmlElement]
        public GenerateClock GenerateClock { get; set; }

        [XmlAttribute]
        public byte IndexX { get; set; }

        [XmlAttribute]
        public byte IndexY { get; set; }

        [XmlAttribute]
        public int Map { get; set; }

        [XmlElement]
        public OnAreaEntry[] OnAreaEntry { get; set; }

        [XmlElement]
        public OnCharacterDiscoveringMap OnCharacterDiscoveringMap { get; set; }

        [XmlElement]
        public OnLockerOpen OnLockerOpen { get; set; }

        [XmlElement]
        public OnMoveOnMap[] OnMoveOnMap { get; set; }

        [XmlElement]
        public SetButtonLockers SetButtonLockers { get; set; }

        [XmlElement]
        public SetMonsterLockers SetMonsterLockers { get; set; }

        [XmlElement]
        public SpawnButton[] SpawnButton { get; set; }

        [XmlElement]
        public SpawnPortal[] SpawnPortal { get; set; }

        [XmlElement]
        public object StartClock { get; set; }

        [XmlElement]
        public SummonMonster[] SummonMonster { get; set; }

        [XmlElement]
        public SummonNpc[] SummonNpc { get; set; }

        [XmlAttribute]
        public short VNum { get; set; }

        #endregion
    }
}