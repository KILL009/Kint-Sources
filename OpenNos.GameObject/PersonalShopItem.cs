namespace OpenNos.GameObject
{
    public class PersonalShopItem
    {
        #region Properties

        public ItemInstance ItemInstance { get; set; }

        public long Price { get; set; }

        public byte SellAmount { get; set; }

        public short ShopSlot { get; set; }

        #endregion
    }
}