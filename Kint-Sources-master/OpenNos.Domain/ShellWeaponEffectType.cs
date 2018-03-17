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

namespace OpenNos.Domain
{
    public enum ShellWeaponEffectType : byte
    {
        DamageImproved = 1,
        PercentageTotalDamage = 2,
        MinorBleeding = 3,
        Bleeding = 4,
        HeavyBleeding = 5,
        Blackout = 6,
        Freeze = 7,
        DeadlyBlackout = 8,
        DamageIncreasedtothePlant = 9,
        DamageIncreasedtotheAnimal = 10,
        DamageIncreasedtotheEnemy = 11,
        DamageIncreasedtotheUnDead = 12,
        DamageincreasedtotheSmallMonster = 13,
        DamageincreasedtotheBigMonster = 14,
        CriticalChance = 15, //Except Staff
        CriticalDamage = 16, //Except Staff
        AntiMagicDisorder = 17, //Only Staff
        IncreasedFireProperties = 18,
        IncreasedWaterProperties = 19,
        IncreasedLightProperties = 20,
        IncreasedDarkProperties = 21,
        IncreasedElementalProperties = 22,
        ReducedMPConsume = 23,
        HPRecoveryForKilling = 24,
        MPRecoveryForKilling = 25,
        SLDamage = 26,
        SLDefence = 27,
        SLElement = 28,
        SLHP = 29,
        SLGlobal = 30,
        GainMoreGold = 31,
        GainMoreXP = 32,
        GainMoreCXP = 33,
        PercentageDamageInPVP = 34,
        ReducesPercentageEnemyDefenceInPVP = 35,
        ReducesEnemyFireResistanceInPVP = 36,
        ReducesEnemyWaterResistanceInPVP = 37,
        ReducesEnemyLightResistanceInPVP = 38,
        ReducesEnemyDarkResistanceInPVP = 39,
        ReducesEnemyAllResistancesInPVP = 40,
        NeverMissInPVP = 41,
        PVPDamageAt15Percent = 42,
        ReducesEnemyMPInPVP = 43,
        InspireFireResistanceWithPercentage = 44,
        InspireWaterResistanceWithPercentage = 45,
        InspireLightResistanceWithPercentage = 46,
        InspireDarkResistanceWithPercentage = 47,
        GainSPForKilling = 48,
        IncreasedPrecision = 49,
        IncreasedFocus = 50
    }
}