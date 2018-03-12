namespace OpenNos.GameObject
{
    public class Gift
    {
        #region Instantiation

        public Gift()
        {
            // do nothing
        }

        public Gift(short vnum, byte amount, short design = 0, bool isRareRandom = false)
        {
            VNum = vnum;
            Amount = amount;
            IsRareRandom = isRareRandom;
            Design = design;
        }

        #endregion

        #region Properties

        public byte Amount { get; set; }

        public short Design { get; set; }

        public bool IsRandomRare { get; set; }

        public bool IsRareRandom { get; set; }

        public short VNum { get; set; }

        #endregion
    }
}