using System.Collections.Generic;

namespace OpenNos.GameObject.Event
{
    public class TalentArenaBattle
    {
        #region Properties

        public byte Calls { get; set; }

        public List<long> CharacterOrder { get; set; }

        public byte GroupLevel { get; set; }

        public List<long> KilledCharacters { get; set; }

        public MapInstance MapInstance { get; set; }

        public byte Side { get; set; }

        #endregion
    }
}