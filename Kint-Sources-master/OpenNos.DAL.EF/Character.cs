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

using OpenNos.DAL.EF.Entities;
using OpenNos.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.DAL.EF
{
    public class Character
    {
        #region Instantiation

        public Character()
        {
            CharacterSkill = new HashSet<CharacterSkill>();
            CharacterRelation1 = new HashSet<CharacterRelation>();
            CharacterRelation2 = new HashSet<CharacterRelation>();
            StaticBonus = new HashSet<StaticBonus>();
            StaticBuff = new HashSet<StaticBuff>();
            BazaarItem = new HashSet<BazaarItem>();
            Inventory = new HashSet<ItemInstance>();
            QuestProgress = new HashSet<QuestProgress>();
            QuicklistEntry = new HashSet<QuicklistEntry>();
            Respawn = new HashSet<Respawn>();
            GeneralLog = new HashSet<GeneralLog>();
            Mail = new HashSet<Mail>();
            Mail1 = new HashSet<Mail>();
            MinilandObject = new HashSet<MinilandObject>();
            Mate = new HashSet<Mate>();
            MinigameLog = new HashSet<MinigameLog>();
        }

        #endregion

        #region Properties

        public virtual Account Account { get; set; }

        public long AccountId { get; set; }

        public int Act4Dead { get; set; }

        public int Act4Kill { get; set; }

        public int Act4Points { get; set; }

        public int ArenaWinner { get; set; }

        public virtual ICollection<BazaarItem> BazaarItem { get; set; }

        [MaxLength(255)]
        public string Biography { get; set; }

        public bool BuffBlocked { get; set; }

        public long CharacterId { get; set; }

        public virtual ICollection<CharacterRelation> CharacterRelation1 { get; set; }

        public virtual ICollection<CharacterRelation> CharacterRelation2 { get; set; }

        public virtual ICollection<CharacterSkill> CharacterSkill { get; set; }

        public byte Class { get; set; }

        public short Compliment { get; set; }

        public float Dignity { get; set; }

        public bool EmoticonsBlocked { get; set; }

        public bool ExchangeBlocked { get; set; }

        public byte Faction { get; set; }

        public virtual ICollection<FamilyCharacter> FamilyCharacter { get; set; }

        public bool FamilyRequestBlocked { get; set; }

        public bool FriendRequestBlocked { get; set; }

        public GenderType Gender { get; set; }

        public virtual ICollection<GeneralLog> GeneralLog { get; set; }

        public long Gold { get; set; }

        public long GoldBank { get; set; }

        public bool GroupRequestBlocked { get; set; }

        public HairColorType HairColor { get; set; }

        public HairStyleType HairStyle { get; set; }

        public bool HeroChatBlocked { get; set; }

        public int HeroLevel { get; set; }

        public int prestigeLevel { get; set; }

        public long HeroXp { get; set; }

        public long PrestigeXp { get; set; }

        public int Hp { get; set; }

        public bool HpBlocked { get; set; }

        public virtual ICollection<ItemInstance> Inventory { get; set; }

        public byte JobLevel { get; set; }

        public long JobLevelXp { get; set; }

        public DateTime LastLogin { get; set; }

        public long LastFamilyLeave { get; set; }

        public int Level { get; set; }

        public long LevelXp { get; set; }

        public virtual ICollection<Mail> Mail { get; set; }

        public virtual ICollection<Mail> Mail1 { get; set; }

        public virtual Map Map { get; set; }

        public short MapId { get; set; }

        public short MapX { get; set; }

        public short MapY { get; set; }

        public int MasterPoints { get; set; }

        public int MasterTicket { get; set; }

        public virtual ICollection<Mate> Mate { get; set; }

        public byte MaxMateCount { get; set; }

        public virtual ICollection<MinigameLog> MinigameLog { get; set; }

        public bool MinilandInviteBlocked { get; set; }

        [MaxLength(255)]
        public string MinilandMessage { get; set; }

        public virtual ICollection<MinilandObject> MinilandObject { get; set; }

        public short MinilandPoint { get; set; }

        public MinilandState MinilandState { get; set; }

        public bool MouseAimLock { get; set; }

        public int Mp { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        public virtual ICollection<QuestProgress> QuestProgress { get; set; }

        public bool QuickGetUp { get; set; }

        public virtual ICollection<QuicklistEntry> QuicklistEntry { get; set; }

        public long RagePoint { get; set; }

        public long Reputation { get; set; }

        public virtual ICollection<Respawn> Respawn { get; set; }

        public byte Slot { get; set; }

        public int SpAdditionPoint { get; set; }

        public int SpPoint { get; set; }

        public byte State { get; set; }

        public virtual ICollection<StaticBonus> StaticBonus { get; set; }

        public virtual ICollection<StaticBuff> StaticBuff { get; set; }

        public int TalentLose { get; set; }

        public int TalentSurrender { get; set; }

        public int TalentWin { get; set; }

        public bool WhisperBlocked { get; set; }

        public int NosheatDollar { get; set; }


        #endregion
    }
}