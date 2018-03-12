namespace OpenNos.GameObject
{
    public class MapDesignObject : MinilandObjectDTO
    {
        #region Members

        public ItemInstance ItemInstance;

        #endregion

        #region Methods

        public string GenerateEffect(bool removed)
        {
            return $"eff_g  {ItemInstance.Item?.EffectValue ?? ItemInstance.Design} {MapX.ToString("00")}{MapY.ToString("00")} {MapX} {MapY} {(removed ? 1 : 0)}";
        }

        public string GenerateMapDesignObject(bool deleted)
        {
            return $"mlobj {(deleted ? 0 : 1)} {ItemInstance.Slot} {MapX} {MapY} {ItemInstance.Item.Width} {ItemInstance.Item.Height} 0 {ItemInstance.DurabilityPoint} 0 {(ItemInstance.Item.IsWarehouse ? 1 : 0)}";
        }

        #endregion
    }
}