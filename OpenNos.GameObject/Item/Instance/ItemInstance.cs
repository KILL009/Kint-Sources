using OpenNos.Data;
using OpenNos.Domain;
using System;

namespace OpenNos.GameObject
{
    public class ItemInstance : ItemInstanceDTO
    {
        #region Members

        private Item item;
        private Random random;

        #endregion

        #region Instantiation

        public ItemInstance()
        {
            random = new Random();
        }

        public ItemInstance(short vNum, byte amount)
        {
            ItemVNum = vNum;
            Amount = amount;
            Type = Item.Type;
            random = new Random();
        }

        #endregion

        #region Properties

        public ClientSession CharacterSession => ServerManager.Instance.GetSessionByCharacterId(CharacterId);

        public bool IsBound
        {
            get
            {
                return BoundCharacterId.HasValue && Item.ItemType != ItemType.Armor && Item.ItemType != ItemType.Weapon;
            }
        }

        public Item Item => item ?? (item = ServerManager.Instance.GetItem(ItemVNum));

        #endregion

        //// TODO: create Interface

        #region Methods

        public ItemInstance DeepCopy() => (ItemInstance)MemberwiseClone();

        public string GenerateFStash() => $"f_stash {GenerateStashPacket()}";

        public string GenerateInventoryAdd()
        {
            switch (Type)
            {
                case InventoryType.Equipment:
                    return $"ivn 0 {Slot}.{ItemVNum}.{Rare}.{(Item.IsColored ? Design : Upgrade)}.0";

                case InventoryType.Main:
                    return $"ivn 1 {Slot}.{ItemVNum}.{Amount}.0";

                case InventoryType.Etc:
                    return $"ivn 2 {Slot}.{ItemVNum}.{Amount}.0";

                case InventoryType.Miniland:
                    return $"ivn 3 {Slot}.{ItemVNum}.{Amount}";

                case InventoryType.Specialist:
                    return $"ivn 6 {Slot}.{ItemVNum}.{Rare}.{Upgrade}.{(this as SpecialistInstance)?.SpStoneUpgrade}";

                case InventoryType.Costume:
                    return $"ivn 7 {Slot}.{ItemVNum}.{Rare}.{Upgrade}.0";
            }

            return string.Empty;
        }

        public string GeneratePStash() => $"pstash {GenerateStashPacket()}";

        public string GenerateStash() => $"stash {GenerateStashPacket()}";

        public string GenerateStashPacket()
        {
            var packet = $"{Slot}.{ItemVNum}.{(byte)Item.Type}";
            switch (Item.Type)
            {
                case InventoryType.Equipment:
                    return packet + $".{Amount}.{Rare}.{Upgrade}";

                case InventoryType.Specialist:
                    var sp = this as SpecialistInstance;
                    return packet + $".{Upgrade}.{sp?.SpStoneUpgrade ?? 0}.0";

                default:
                    return packet + $".{Amount}.0.0";
            }
        }

        public void Save()
        {
        }

        #endregion
    }
}