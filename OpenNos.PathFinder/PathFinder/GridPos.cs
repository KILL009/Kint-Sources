namespace OpenNos.PathFinder
{
    public class GridPos
    {
        #region Instantiation

        public GridPos(short x, short y)
        {
            X = x;
            Y = y;
        }

        public GridPos()
        {
        }

        #endregion

        #region Properties

        public byte Value { get; set; }

        public short X { get; set; }

        public short Y { get; set; }

        #endregion

        #region Methods

        public bool IsArenaStairs() => Value > 0;

        public bool IsWalkable() => Value == 0 || Value == 2 || Value >= 16 && Value <= 19;

        #endregion
    }
}