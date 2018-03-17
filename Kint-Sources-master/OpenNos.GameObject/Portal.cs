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

using OpenNos.Data;
using System;
using OpenNos.GameObject.Networking;
using System.Collections.Generic;

namespace OpenNos.GameObject
{
    public class Portal : PortalDTO
    {
        #region Members

        private Guid _destinationMapInstanceId;

        private Guid _sourceMapInstanceId;

        #endregion

        #region Instantiation

        public Portal() => OnTraversalEvents = new List<EventContainer>();

        public Portal(PortalDTO input)
        {
            OnTraversalEvents = new List<EventContainer>();
            DestinationMapId = input.DestinationMapId;
            DestinationX = input.DestinationX;
            DestinationY = input.DestinationY;
            IsDisabled = input.IsDisabled;
            PortalId = input.PortalId;
            SourceMapId = input.SourceMapId;
            SourceX = input.SourceX;
            SourceY = input.SourceY;
            Type = input.Type;
        }

        #endregion

        #region Properties

        public Guid DestinationMapInstanceId
        {
            get
            {
                if (_destinationMapInstanceId == default && DestinationMapId != -1)
                {
                    _destinationMapInstanceId = ServerManager.GetBaseMapInstanceIdByMapId(DestinationMapId);
                }
                return _destinationMapInstanceId;
            }
            set => _destinationMapInstanceId = value;
        }

        public List<EventContainer> OnTraversalEvents { get; set; }

        public Guid SourceMapInstanceId
        {
            get
            {
                if (_sourceMapInstanceId == default)
                {
                    _sourceMapInstanceId = ServerManager.GetBaseMapInstanceIdByMapId(SourceMapId);
                }
                return _sourceMapInstanceId;
            }
            set => _sourceMapInstanceId = value;
        }

        #endregion

        #region Methods

        public string GenerateGp() => $"gp {SourceX} {SourceY} {ServerManager.GetMapInstance(DestinationMapInstanceId)?.Map.MapId ?? 0} {Type} {PortalId} {(IsDisabled ? 1 : 0)}";

        #endregion
    }
}