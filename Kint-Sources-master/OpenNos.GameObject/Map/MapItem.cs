/*
 * This file is part of the OpenNos Emulator Project. See AUTHORS file for Copyright information
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 */

using OpenNos.GameObject.Helpers;
using System;

namespace OpenNos.GameObject
{
    public abstract class MapItem
    {
        #region Members

        protected ItemInstance _itemInstance;

        private readonly object _lockObject = new object();
        private long _transportId;

        #endregion

        #region Instantiation

        protected MapItem(short x, short y)
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

        public long TransportId
        {
            get
            {
                lock (_lockObject)
                {
                    if (_transportId == 0)
                    {
                        _transportId = TransportFactory.Instance.GenerateTransportId();
                    }
                    return _transportId;
                }
            }

            private set
            {
                if (value != _transportId)
                {
                    _transportId = value;
                }
            }
        }

        #endregion

        #region Methods

        public string GenerateIn() => StaticPacketHelper.In(Domain.UserType.Object, ItemVNum, TransportId, PositionX, PositionY, this is MonsterMapItem monsterMapItem && monsterMapItem.GoldAmount > 1 ? monsterMapItem.GoldAmount : Amount, 0, 0, 0, 0, false);

        public abstract ItemInstance GetItemInstance();

        #endregion
    }
}