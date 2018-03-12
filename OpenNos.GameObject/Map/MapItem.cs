using OpenNos.Data;
using System;
using System.Collections.Generic;

namespace OpenNos.GameObject
{
    public abstract class MapItem
    {
        #region Members

        public List<EquipmentOptionDTO> EquipmentOptions;
        protected ItemInstance _itemInstance;
        private long transportId;

        #endregion

        #region Instantiation

        public MapItem(short x, short y)
        {
            PositionX = x;
            PositionY = y;
            CreatedDate = DateTime.Now;
            TransportId = 0;
        }

        #endregion

        #region Properties

        public abstract byte Amount { get; set; }

        public DateTime CreatedDate { get; set; }

        public abstract short ItemVNum { get; set; }

        public short PositionX { get; set; }

        public short PositionY { get; set; }

        public virtual long TransportId
        {
            get
            {
                if (transportId == 0)
                {
                    // create transportId thru factory
                    // TODO: Review has some problems, aka. issue corresponding to
                    //       weird/multiple/missplaced drops
                    transportId = TransportFactory.Instance.GenerateTransportId();
                }

                return transportId;
            }

            set
            {
                if (value != transportId)
                {
                    transportId = value;
                }
            }
        }

        #endregion

        #region Methods

        public string GenerateIn()
        {
            return $"in 9 {ItemVNum} {TransportId} {PositionX} {PositionY} {(this is MonsterMapItem && ((MonsterMapItem)this).GoldAmount > 1 ? ((MonsterMapItem)this).GoldAmount : Amount)} 0 0 -1";
        }

        public string GenerateOut(long id) => $"out 9 {id}";

        public abstract ItemInstance GetItemInstance();

        #endregion
    }
}