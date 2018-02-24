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

using System;

namespace OpenNos.Core.Handling
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
   public sealed class PacketAttribute : Attribute
    {
        #region Instantiation

        //[Obsolete]
        public PacketAttribute(int amount = 1, params string[] header)
        {
            Header = header;
            Amount = amount;
        }

        public PacketAttribute(params string[] header)
        {
            Header = header;
            Amount = 1;
        }

        #endregion

        #region Properties

        public int Amount { get; }

        public string[] Header { get; }

        #endregion
    }
}