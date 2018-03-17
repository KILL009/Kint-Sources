using OpenNos.XMLModel.Objects;
using System;

namespace OpenNos.XMLModel.Models.ScriptedInstance
{
    [Serializable]
    public class Globals
    {
        #region Properties

        public Item[] DrawItems { get; set; }

        public Item[] GiftItems { get; set; }

        public Gold Gold { get; set; }

        public Id Id { get; set; }

        public Label Label { get; set; }

        public Name Name { get; set; }

        public Level LevelMaximum { get; set; }

        public Level LevelMinimum { get; set; }

        public Lives Lives { get; set; }

        public Reputation Reputation { get; set; }

        public Item[] RequiredItems { get; set; }

        public Item[] SpecialItems { get; set; }

        public StartPosition StartX { get; set; }

        public StartPosition StartY { get; set; }

        #endregion
    }
}