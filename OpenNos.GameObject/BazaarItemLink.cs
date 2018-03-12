using OpenNos.Data;

namespace OpenNos.GameObject
{
    public class BazaarItemLink
    {
        #region Properties

        public BazaarItemDTO BazaarItem { get; set; }

        public ItemInstance Item { get; set; }

        public string Owner { get; set; }

        #endregion
    }
}