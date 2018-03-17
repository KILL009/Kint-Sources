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

namespace OpenNos.Core
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public sealed class PacketIndexAttribute : Attribute
    {
        #region Instantiation

        /// <summary>
        /// Specify the Index of the packet to parse this property to.
        /// </summary>
        /// <param name="index">The zero based index starting from header (exclusive).</param>
        /// <param name="isReturnPacket">
        /// Adds an # to the Header and replaces Spaces with ^ if set to true.
        /// </param>
        /// <param name="serializeToEnd">
        /// Defines if everything from this index should be serialized into the underlying property
        /// </param>
        /// <param name="removeSeparator">
        /// Removes the separator (.) for List&lt;PacketDefinition&gt; packets.
        /// </param>
        public PacketIndexAttribute(int index, bool isReturnPacket = false, bool serializeToEnd = false, bool removeSeparator = false)
        {
            Index = index;
            IsReturnPacket = isReturnPacket;
            SerializeToEnd = serializeToEnd;
            RemoveSeparator = removeSeparator;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The zero based index starting from the header (exclusive).
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Adds an # to the Header and replaces Spaces with ^
        /// </summary>
        public bool IsReturnPacket { get; set; }

        /// <summary>
        /// Removes the separator (.) for List&lt;PacketDefinition&gt; packets.
        /// </summary>
        public bool RemoveSeparator { get; set; }

        /// <summary>
        /// Defines if everything from this index should be serialized into the underlying property.
        /// </summary>
        public bool SerializeToEnd { get; set; }

        #endregion
    }
}