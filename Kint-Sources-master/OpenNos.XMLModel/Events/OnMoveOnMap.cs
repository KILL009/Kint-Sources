using OpenNos.XMLModel.Objects;
using System;
using System.Xml.Serialization;

namespace OpenNos.XMLModel.Events
{
    [Serializable]
    public class OnMoveOnMap
    {
        #region Properties

        [XmlElement]
        public GenerateClock GenerateClock { get; set; }

        [XmlElement]
        public OnMapClean OnMapClean { get; set; }

        [XmlElement]
        public object RefreshRaidGoals { get; set; }

        [XmlElement]
        public object RemoveButtonLocker { get; set; }

        [XmlElement]
        public object RemoveMonsterLocker { get; set; }

        [XmlElement]
        public SendMessage SendMessage { get; set; }

        [XmlElement]
        public SendPacket SendPacket { get; set; }

        [XmlElement]
        public SetButtonLockers SetButtonLockers { get; set; }

        [XmlElement]
        public SetMonsterLockers SetMonsterLockers { get; set; }

        [XmlElement]
        public StartClock StartClock { get; set; }

        [XmlElement]
        public StartClock StartMapClock { get; set; }

        [XmlElement]
        public SummonMonster[] SummonMonster { get; set; }

        [XmlElement]
        public Wave[] Wave { get; set; }

        #endregion
    }
}