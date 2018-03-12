using OpenNos.Domain;

namespace OpenNos.GameObject
{
    public class MonsterMapItem : MapItem
    {
        #region Instantiation

        public MonsterMapItem(short x, short y, short itemVNum, int amount = 1, long ownerId = -1) : base(x, y)
        {
            ItemVNum = itemVNum;
            if (amount < 100)
            {
                Amount = (byte)amount;
            }

            GoldAmount = amount;
            OwnerId = ownerId;
        }

        #endregion

        #region Properties

        public sealed override byte Amount { get; set; }

        public int GoldAmount { get; private set; }

        public sealed override short ItemVNum { get; set; }

        public long? OwnerId { get; }

        #endregion

        #region Methods

        public override ItemInstance GetItemInstance()
        {
            if (_itemInstance == null && OwnerId != null)
            {
                _itemInstance = Inventory.InstantiateItemInstance(ItemVNum, OwnerId.Value, Amount);
            }

            return _itemInstance;
        }

        public void Rarify(ClientSession session)
        {
            var instance = GetItemInstance();
            if (instance.Item.Type != InventoryType.Equipment || (instance.Item.ItemType != ItemType.Weapon && instance.Item.ItemType != ItemType.Armor))
            {
                return;
            }

            var wearableInstance = instance as WearableInstance;
            wearableInstance?.RarifyItem(session, RarifyMode.Drop, RarifyProtection.None);
        }

        #endregion
    }
}