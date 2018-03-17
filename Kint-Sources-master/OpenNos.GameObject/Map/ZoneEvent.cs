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

using System.Collections.Generic;

namespace OpenNos.GameObject
{
    public class ZoneEvent
    {
        #region Instantiation

        public ZoneEvent()
        {
            Events = new List<EventContainer>();
            Range = 1;
        }

        #endregion

        #region Properties

        public List<EventContainer> Events { get; set; }

        public short Range { get; set; }

        public short X { get; set; }

        public short Y { get; set; }

        #endregion

        #region Methods

        public bool InZone(short positionX, short positionY) => positionX <= X + Range && positionX >= X - Range && positionY <= Y + Range && positionY >= Y - Range;

        #endregion
    }
}