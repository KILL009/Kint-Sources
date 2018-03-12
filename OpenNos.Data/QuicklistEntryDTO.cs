namespace OpenNos.Data
{
    public class QuicklistEntryDTO : SynchronizableBaseDTO
    {
        #region Properties

        public long CharacterId { get; set; }

        public short Morph { get; set; }

        public short Pos { get; set; }

        public short Q1 { get; set; }

        public short Q2 { get; set; }

        public short Slot { get; set; }

        public short Type { get; set; }

        #endregion
    }
}