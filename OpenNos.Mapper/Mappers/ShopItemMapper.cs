using OpenNos.DAL.EF;
using OpenNos.Data;

namespace OpenNos.Mapper.Mappers
{
    public class ShopItemMapper
    {
        public ShopItemMapper()
        {
        }

        public bool ToShopItemDTO(ShopItem input, ShopItemDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.Color = input.Color;
            output.ItemVNum = input.ItemVNum;
            output.Rare = (sbyte)input.Rare;
            output.ShopId = input.ShopId;
            output.ShopItemId = input.ShopItemId;
            output.Slot = input.Slot;
            output.Type = input.Type;
            output.Upgrade = input.Upgrade;
            return true;
        }

        public bool ToShopItem(ShopItemDTO input, ShopItem output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.Color = input.Color;
            output.ItemVNum = input.ItemVNum;
            output.Rare = (sbyte)input.Rare;
            output.ShopId = input.ShopId;
            output.ShopItemId = input.ShopItemId;
            output.Slot = input.Slot;
            output.Type = input.Type;
            output.Upgrade = input.Upgrade;
            return true;
        }
    }
}

