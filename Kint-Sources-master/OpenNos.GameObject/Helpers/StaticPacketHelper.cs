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
using OpenNos.Domain;
using OpenNos.GameObject.Packets.ServerPackets;

namespace OpenNos.GameObject.Helpers
{
    public static class StaticPacketHelper
    {
        #region Methods

        public static string Cancel(byte type = 0, long callerId = 0) => $"cancel {type} {callerId}";

        public static string CastOnTarget(UserType type, long callerId, byte secondaryType, long targetId, short castAnimation, short castEffect, short skillVNum) => $"ct {(byte)type} {callerId} {secondaryType} {targetId} {castAnimation} {castEffect} {skillVNum}";

        public static EffectPacket GenerateEff(UserType effectType, long callerId, int effectId)
        {
            return new EffectPacket
            {
                EffectType = effectType,
                CallerId = callerId,
                EffectId = effectId
            };
        }

        public static string In(UserType type, short callerVNum, long callerId, short mapX, short mapY, int direction, int currentHp, int currentMp, short dialog, InRespawnType respawnType, bool isSitting)
        {
            switch (type)
            {
                case UserType.Npc:
                case UserType.Monster:
                    return $"in {(byte)type} {callerVNum} {callerId} {mapX} {mapY} {direction} {currentHp} {currentMp} {dialog} 0 0 -1 {(byte)respawnType} {(isSitting ? 1 : 0)} -1 - 0 -1 0 0 0 0 0 0 0 0";

                case UserType.Object:
                    return $"in 9 {callerVNum} {callerId} {mapX} {mapY} {direction} 0 0 -1";

                default:
                    return string.Empty;
            }
        }

        public static MovePacket Move(UserType type, long callerId, short positionX, short positionY, byte speed)
        {
            return new MovePacket
            {
                MoveType = type,
                CallerId = callerId,
                PositionX = positionX,
                PositionY = positionY,
                Speed = speed
            };
        }

        public static string Out(UserType type, long callerId) => $"out {(byte)type} {callerId}";

        public static string Say(byte type, long callerId, byte secondaryType, string message) => $"say {type} {callerId} {secondaryType} {message}";

        public static string SkillReset(int castId) => $"sr {castId}";

        public static string SkillResetWithCoolDown(int castId, int coolDown) => $"sr -10 {castId} {coolDown}";

        public static string SkillUsed(UserType type, long callerId, byte secondaryType, long targetId, short skillVNum, short cooldown, short attackAnimation, short skillEffect, short x, short y, bool isAlive, int health, int damage, int hitmode, byte skillType) => $"su {(byte)type} {callerId} {secondaryType} {targetId} {skillVNum} {cooldown} {attackAnimation} {skillEffect} {x} {y} {(isAlive ? 1 : 0)} {health} {damage} {hitmode} {skillType}";

        internal static string GenerateEff(UserType player, object characterId, int v)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}