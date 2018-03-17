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

namespace OpenNos.GameObject.Mock
{
    public class GameObjectMockHelper
    {
        #region Members

        private static GameObjectMockHelper _instance;

        private long _nextClientId;

        #endregion

        #region Properties

        public static GameObjectMockHelper Instance => _instance ?? (_instance = new GameObjectMockHelper());

        #endregion

        #region Methods

        public long GetNextClientId() => ++_nextClientId;

        #endregion
    }
}