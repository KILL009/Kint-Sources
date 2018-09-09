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

using OpenNos.Core;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using System;
using System.Collections.Generic;
using OpenNos.GameObject.Networking;
using System.Linq;

namespace OpenNos.GameObject
{
    public class ItemInstance : ItemInstanceDTO
    {
        #region Members

        private Item _item;
        private long _transportId;
        private List<CellonOptionDTO> _cellonOptions;
        private List<ShellEffectDTO> _shellEffects;
        private int i1;

        #endregion

        #region Instantiation

        public ItemInstance()
        {

        }

        public ItemInstance(short vNum, byte amount)
        {
            ItemVNum = vNum;
            Amount = amount;
            Type = Item.Type;
        }

        public ItemInstance(ItemInstanceDTO input)
        {
            Ammo = input.Ammo;
            Amount = input.Amount;
            BoundCharacterId = input.BoundCharacterId;
            Cellon = input.Cellon;
            CharacterId = input.CharacterId;
            CloseDefence = input.CloseDefence;
            Concentrate = input.Concentrate;
            CriticalDodge = input.CriticalDodge;
            CriticalLuckRate = input.CriticalLuckRate;
            CriticalRate = input.CriticalRate;
            DamageMaximum = input.DamageMaximum;
            DamageMinimum = input.DamageMinimum;
            DarkElement = input.DarkElement;
            DarkResistance = input.DarkResistance;
            DefenceDodge = input.DefenceDodge;
            Design = input.Design;
            DistanceDefence = input.DistanceDefence;
            DistanceDefenceDodge = input.DistanceDefenceDodge;
            DurabilityPoint = input.DurabilityPoint;
            ElementRate = input.ElementRate;
            EquipmentSerialId = input.EquipmentSerialId;
            FireElement = input.FireElement;
            FireResistance = input.FireResistance;
            HitRate = input.HitRate;
            HoldingVNum = input.HoldingVNum;
            HP = input.HP;
            Id = input.Id;
            IsEmpty = input.IsEmpty;
            IsFixed = input.IsFixed;
            ItemDeleteTime = input.ItemDeleteTime;
            ItemVNum = input.ItemVNum;
            LightElement = input.LightElement;
            LightResistance = input.LightResistance;
            MagicDefence = input.MagicDefence;
            MaxElementRate = input.MaxElementRate;
            MP = input.MP;
            Rare = input.Rare;
            SlDamage = input.SlDamage;
            SlDefence = input.SlDefence;
            SlElement = input.SlElement;
            SlHP = input.SlHP;
            Slot = input.Slot;
            SpDamage = input.SpDamage;
            SpDark = input.SpDark;
            SpDefence = input.SpDefence;
            SpElement = input.SpElement;
            SpFire = input.SpFire;
            SpHP = input.SpHP;
            SpLevel = input.SpLevel;
            SpLight = input.SpLight;
            SpStoneUpgrade = input.SpStoneUpgrade;
            SpWater = input.SpWater;
            Type = input.Type;
            Upgrade = input.Upgrade;
            WaterElement = input.WaterElement;
            WaterResistance = input.WaterResistance;
            XP = input.XP;
        }

        #endregion

        #region Properties

        public bool IsBound => BoundCharacterId.HasValue;

        public Item Item => _item ?? (_item = ServerManager.GetItem(ItemVNum));

        public long TransportId
        {
            get
            {
                if (_transportId == 0)
                {
                    // create transportId thru factory
                    _transportId = TransportFactory.Instance.GenerateTransportId();
                }

                return _transportId;
            }
        }

        public List<CellonOptionDTO> CellonOptions => _cellonOptions ?? (_cellonOptions = DAOFactory.CellonOptionDAO.GetOptionsByWearableInstanceId(EquipmentSerialId == Guid.Empty ? EquipmentSerialId = Guid.NewGuid() : EquipmentSerialId).ToList());

        public List<ShellEffectDTO> ShellEffects => _shellEffects ?? (_shellEffects = DAOFactory.ShellEffectDAO.LoadByEquipmentSerialId(EquipmentSerialId == Guid.Empty ? EquipmentSerialId = Guid.NewGuid() : EquipmentSerialId).ToList());

        public object Value { get; internal set; }


        #endregion

        //// TODO: create Interface

        #region Methods

        public ItemInstance DeepCopy() => (ItemInstance)MemberwiseClone();

        public string GenerateFStash() => $"f_stash {GenerateStashPacket()}";

        public string GenerateInventoryAdd()
        {
            switch (Type)
            {
                case InventoryType.Equipment:
                    return $"ivn 0 {Slot}.{ItemVNum}.{Rare}.{(Item.IsColored ? Design : Upgrade)}.{SpStoneUpgrade}";

                case InventoryType.Main:
                    return $"ivn 1 {Slot}.{ItemVNum}.{Amount}.0";

                case InventoryType.Etc:
                    return $"ivn 2 {Slot}.{ItemVNum}.{Amount}.0";

                case InventoryType.Miniland:
                    return $"ivn 3 {Slot}.{ItemVNum}.{Amount}";

                case InventoryType.Specialist:
                    return $"ivn 6 {Slot}.{ItemVNum}.{Rare}.{Upgrade}.{SpStoneUpgrade}";

                case InventoryType.Costume:
                    return $"ivn 7 {Slot}.{ItemVNum}.{Rare}.{Upgrade}.0";
            }
            return string.Empty;
        }

        public string GeneratePStash() => $"pstash {GenerateStashPacket()}";

        public string GenerateStash() => $"stash {GenerateStashPacket()}";

        public string GenerateStashPacket()
        {
            string packet = $"{Slot}.{ItemVNum}.{(byte)Item.Type}";
            switch (Item.Type)
            {
                case InventoryType.Equipment:
                    return packet + $".{Amount}.{Rare}.{Upgrade}";

                case InventoryType.Specialist:
                    return packet + $".{Upgrade}.{SpStoneUpgrade}.0";

                default:
                    return packet + $".{Amount}.0.0";
            }
        }

        public string GeneratePslInfo()
        {
            // 1235.3 1237.4 1239.5 <= skills SkillVNum.Grade
            return $"pslinfo {Item.VNum} {Item.Element} {Item.ElementRate} {Item.LevelJobMinimum} {Item.Speed} {Item.FireResistance} {Item.WaterResistance} {Item.LightResistance} {Item.DarkResistance} 0.0 0.0 0.0";
        }

        public string GenerateSlInfo()
        {
            int freepoint = CharacterHelper.SPPoint(SpLevel, Upgrade) - SlDamage - SlHP - SlElement - SlDefence;

            int slElement = CharacterHelper.SlPoint(SlElement, 2);
            int slHp = CharacterHelper.SlPoint(SlHP, 3);
            int slDefence = CharacterHelper.SlPoint(SlDefence, 1);
            int slHit = CharacterHelper.SlPoint(SlDamage, 0);

            string skill = string.Empty;
            List<CharacterSkill> skillsSp = new List<CharacterSkill>();
            foreach (Skill ski in ServerManager.GetAllSkill().Where(ski => ski.Class == Item.Morph + 31 && ski.LevelMinimum <= SpLevel))
            {
                skillsSp.Add(new CharacterSkill
                {
                    SkillVNum = ski.SkillVNum,
                    CharacterId = CharacterId
                });
            }
            byte spdestroyed = 0;
            if (Rare == -2)
            {
                spdestroyed = 1;
            }
            if (skillsSp.Count == 0)
            {
                skill = "-1";
            }
            short firstskillvnum = skillsSp[0].SkillVNum;

            for (int i = 1; i < 11; i++)
            {
                if (skillsSp.Count >= i + 1 && skillsSp[i].SkillVNum <= firstskillvnum + 10)
                {
                    skill += $"{skillsSp[i].SkillVNum}.";
                }
            }

            // 10 9 8 '0 0 0 0'<- bonusdamage bonusarmor bonuselement bonushpmp its after upgrade and
            // 3 first values are not important
            skill = skill.TrimEnd('.');
            return $"slinfo {(Type == InventoryType.Wear || Type == InventoryType.Specialist || Type == InventoryType.Equipment ? "0" : "2")} {ItemVNum} {Item.Morph} {SpLevel} {Item.LevelJobMinimum} {Item.ReputationMinimum} 0 0 0 0 0 0 0 {Item.SpType} {Item.FireResistance} {Item.WaterResistance} {Item.LightResistance} {Item.DarkResistance} {XP} {CharacterHelper.SPXPData[SpLevel == 0 ? 0 : SpLevel - 1]} {skill} {TransportId} {freepoint} {slHit} {slDefence} {slElement} {slHp} {Upgrade} 0 0 {spdestroyed} 0 0 0 0 {SpStoneUpgrade} {SpDamage} {SpDefence} {SpElement} {SpHP} {SpFire} {SpWater} {SpLight} {SpDark}";
        }

        public void PerfectSP(ClientSession session)
        {

            do {
                short[] upsuccess = { 50, 40, 30, 20, 10 };

                int[] goldprice = { 5000, 10000, 20000, 50000, 100000 };
                byte[] stoneprice = { 1, 2, 3, 4, 5 };
                short stonevnum;
                byte upmode = 1;

                switch (Item.Morph)
                {
                    case 2:
                    case 6:
                    case 9:
                    case 12:
                        stonevnum = 2514;
                        break;

                    case 3:
                    case 4:
                    case 14:
                        stonevnum = 2515;
                        break;

                    case 5:
                    case 11:
                    case 15:
                        stonevnum = 2516;
                        break;

                    case 10:
                    case 13:
                    case 7:
                        stonevnum = 2517;
                        break;

                    case 17:
                    case 18:
                    case 19:
                        stonevnum = 2518;
                        break;

                    case 20:
                    case 21:
                    case 22:
                        stonevnum = 2519;
                        break;

                    case 23:
                    case 24:
                    case 25:
                        stonevnum = 2520;
                        break;

                    case 26:
                    case 27:
                    case 28:
                        stonevnum = 2521;
                        break;

                    default:
                        return;

               
            }
                if (SpStoneUpgrade > 99)
                {
                    return;
                }
                if (SpStoneUpgrade > 80)
                {
                    upmode = 5;
                }
                else if (SpStoneUpgrade > 60)
                {
                    upmode = 4;
                }
                else if (SpStoneUpgrade > 40)
                {
                    upmode = 3;
                }
                else if (SpStoneUpgrade > 20)
                {
                    upmode = 2;
                }

                if (IsFixed)
                {
                    return;
                }
                if (session.Character.Gold < goldprice[upmode - 1])
                {
                    return;
                }
                if (session.Character.Inventory.CountItem(stonevnum) < stoneprice[upmode - 1])
                {
                    return;
                }
           
            ItemInstance specialist = session.Character.Inventory.GetItemInstanceById(Id);

                int rnd = ServerManager.RandomNumber();
                if (rnd < upsuccess[upmode - 1])
                {
                    byte type = (byte)ServerManager.RandomNumber(0, 16), count = 1;
                    if (upmode == 4)
                    {
                        count = 2;
                    }
                    if (upmode == 5)
                    {
                        count = (byte)ServerManager.RandomNumber(3, 6);
                    }

                    count = (byte)(count * ServerManager.Instance.PerfectionRate);

                    session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3009), session.Character.MapX, session.Character.MapY);

                    if (type < 3)
                    {
                        specialist.SpDamage += count;
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_ATTACK"), count), 12));
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_ATTACK"), count), 0));
                    }
                    else if (type < 6)
                    {
                        specialist.SpDefence += count;
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_DEFENSE"), count), 12));
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_DEFENSE"), count), 0));
                    }
                    else if (type < 9)
                    {
                        specialist.SpElement += count;
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_ELEMENT"), count), 12));
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_ELEMENT"), count), 0));
                    }
                    else if (type < 12)
                    {
                        specialist.SpHP += count;
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_HPMP"), count), 12));
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_HPMP"), count), 0));
                    }
                    else if (type == 12)
                    {
                        specialist.SpFire += count;
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_FIRE"), count), 12));
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_FIRE"), count), 0));
                    }
                    else if (type == 13)
                    {
                        specialist.SpWater += count;
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_WATER"), count), 12));
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_WATER"), count), 0));
                    }
                    else if (type == 14)
                    {
                        specialist.SpLight += count;
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_LIGHT"), count), 12));
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_LIGHT"), count), 0));
                    }
                    else if (type == 15)
                    {
                        specialist.SpDark += count;
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_SHADOW"), count), 12));
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("PERFECTSP_SUCCESS"), Language.Instance.GetMessageFromKey("PERFECTSP_SHADOW"), count), 0));
                    }
                    specialist.SpStoneUpgrade++;
                
            }
                else
                {
                session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("PERFECTSP_FAILURE"), 11));
                session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("PERFECTSP_FAILURE"), 0));
            }
            session.SendPacket(specialist.GenerateInventoryAdd());
            session.Character.Gold -= goldprice[upmode - 1];
            session.SendPacket(session.Character.GenerateGold());
            session.Character.Inventory.RemoveItemAmount(stonevnum, stoneprice[upmode - 1]);
            session.SendPacket("shop_end 1");

            } while (i1 == 20);

        }

        public void UpgradeSp(ClientSession sesion, UpgradeProtection protect)
        {
            if (Upgrade >= 20)
            {
                return;
            }

            byte[] upfail = { 20, 25, 30, 40, 50, 60, 65, 70, 75, 80, 90, 93, 95, 97, 99 };
            byte[] destroy = { 0, 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 70 };

            int[] goldprice = { 200000, 200000, 200000, 200000, 200000, 500000, 500000, 500000, 500000, 500000, 1000000, 1000000, 1000000, 1000000, 1000000 };
            byte[] feather = { 3, 5, 8, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 70 };
            byte[] fullmoon = { 1, 3, 5, 7, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30 };
            byte[] soul = { 2, 4, 6, 8, 10, 1, 2, 3, 4, 5, 1, 2, 3, 4, 5 };
            const short featherVnum = 2282;
            const short fullmoonVnum = 1030;
            const short greenSoulVnum = 2283;
            const short redSoulVnum = 2284;
            const short blueSoulVnum = 2285;
            const short dragonSkinVnum = 2511;
            const short dragonBloodVnum = 2512;
            const short dragonHeartVnum = 2513;
            const short blueScrollVnum = 1363;
            const short redScrollVnum = 1364;

            if (!sesion.HasCurrentMapInstance)
            {
                return;
            }
            if (sesion.Character.Inventory.CountItem(fullmoonVnum) < fullmoon[Upgrade])
            {
                sesion.SendPacket(sesion.Character.GenerateSay(Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(fullmoonVnum).Name, fullmoon[Upgrade])), 10));
                return;
            }
            if (sesion.Character.Inventory.CountItem(featherVnum) < feather[Upgrade])
            {
                sesion.SendPacket(sesion.Character.GenerateSay(Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(featherVnum).Name, feather[Upgrade])), 10));
                return;
            }
            if (sesion.Character.Gold < goldprice[Upgrade])
            {
                sesion.SendPacket(sesion.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                return;
            }

            short itemToRemove = 2283;

            if (Upgrade < 5)
            {
                if (SpLevel > 20)
                {
                    if (Item.Morph <= 16)
                    {
                        if (sesion.Character.Inventory.CountItem(greenSoulVnum) < soul[Upgrade])
                        {
                            sesion.SendPacket(sesion.Character.GenerateSay(Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(greenSoulVnum).Name, soul[Upgrade])), 10));
                            return;
                        }
                        if (protect == UpgradeProtection.Protected)
                        {
                            if (sesion.Character.Inventory.CountItem(blueScrollVnum) < 1)
                            {
                                sesion.SendPacket(sesion.Character.GenerateSay(Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(blueScrollVnum).Name, 1)), 10));
                                return;
                            }
                            sesion.Character.Inventory.RemoveItemAmount(blueScrollVnum);
                            sesion.SendPacket("shop_end 2");
                        }
                    }
                    else
                    {
                        if (sesion.Character.Inventory.CountItem(dragonSkinVnum) < soul[Upgrade])
                        {
                            sesion.SendPacket(sesion.Character.GenerateSay(Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(dragonSkinVnum).Name, soul[Upgrade])), 10));
                            return;
                        }
                        if (protect == UpgradeProtection.Protected)
                        {
                            if (sesion.Character.Inventory.CountItem(blueScrollVnum) < 1)
                            {
                                sesion.SendPacket(sesion.Character.GenerateSay(Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(blueScrollVnum).Name, 1)), 10));
                                return;
                            }
                            sesion.Character.Inventory.RemoveItemAmount(blueScrollVnum);
                            sesion.SendPacket("shop_end 2");
                        }
                        itemToRemove = dragonSkinVnum;
                    }
                }
                else
                {
                    sesion.SendPacket(sesion.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("LVL_REQUIRED"), 21), 11));
                    return;
                }
            }
            else if (Upgrade < 10)
            {
                if (SpLevel > 40)
                {
                    if (Item.Morph <= 16)
                    {
                        if (sesion.Character.Inventory.CountItem(redSoulVnum) < soul[Upgrade])
                        {
                            sesion.SendPacket(sesion.Character.GenerateSay(Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(redSoulVnum).Name, soul[Upgrade])), 10));
                            return;
                        }
                        if (protect == UpgradeProtection.Protected)
                        {
                            if (sesion.Character.Inventory.CountItem(blueScrollVnum) < 1)
                            {
                                sesion.SendPacket(sesion.Character.GenerateSay(Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(blueScrollVnum).Name, 1)), 10));
                                return;
                            }
                            sesion.Character.Inventory.RemoveItemAmount(blueScrollVnum);
                            sesion.SendPacket("shop_end 2");
                        }
                        itemToRemove = redSoulVnum;
                    }
                    else
                    {
                        if (sesion.Character.Inventory.CountItem(dragonBloodVnum) < soul[Upgrade])
                        {
                            sesion.SendPacket(sesion.Character.GenerateSay(Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(dragonBloodVnum).Name, soul[Upgrade])), 10));
                            return;
                        }
                        if (protect == UpgradeProtection.Protected)
                        {
                            if (sesion.Character.Inventory.CountItem(blueScrollVnum) < 1)
                            {
                                sesion.SendPacket(sesion.Character.GenerateSay(Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(blueScrollVnum).Name, 1)), 10));
                                return;
                            }
                            sesion.Character.Inventory.RemoveItemAmount(blueScrollVnum);
                            sesion.SendPacket("shop_end 2");
                        }
                        itemToRemove = dragonBloodVnum;
                    }
                }
                else
                {
                    sesion.SendPacket(sesion.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("LVL_REQUIRED"), 41), 11));
                    return;
                }
            }
            else if (Upgrade < 15)
            {
                if (SpLevel > 50)
                {
                    if (Item.Morph <= 16)
                    {
                        if (sesion.Character.Inventory.CountItem(blueSoulVnum) < soul[Upgrade])
                        {
                            sesion.SendPacket(sesion.Character.GenerateSay(Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(blueSoulVnum).Name, soul[Upgrade])), 10));
                            return;
                        }
                        if (protect == UpgradeProtection.Protected)
                        {
                            if (sesion.Character.Inventory.CountItem(redScrollVnum) < 1)
                            {
                                return;
                            }
                            sesion.Character.Inventory.RemoveItemAmount(redScrollVnum);
                            sesion.SendPacket("shop_end 2");
                        }
                        itemToRemove = blueSoulVnum;
                    }
                    else
                    {
                        if (sesion.Character.Inventory.CountItem(dragonHeartVnum) < soul[Upgrade])
                        {
                            return;
                        }
                        if (protect == UpgradeProtection.Protected)
                        {
                            if (sesion.Character.Inventory.CountItem(redScrollVnum) < 1)
                            {
                                sesion.SendPacket(sesion.Character.GenerateSay(Language.Instance.GetMessageFromKey(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(redScrollVnum).Name, 1)), 10));
                                return;
                            }
                            sesion.Character.Inventory.RemoveItemAmount(redScrollVnum);
                            sesion.SendPacket("shop_end 2");
                        }
                        itemToRemove = dragonHeartVnum;
                    }
                }
                else
                {
                    sesion.SendPacket(sesion.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("LVL_REQUIRED"), 51), 11));
                    return;
                }
            }

            sesion.Character.Gold -= goldprice[Upgrade];

            // remove feather and fullmoon before upgrading
            sesion.Character.Inventory.RemoveItemAmount(featherVnum, feather[Upgrade]);
            sesion.Character.Inventory.RemoveItemAmount(fullmoonVnum, fullmoon[Upgrade]);

            ItemInstance wearable = sesion.Character.Inventory.GetItemInstanceById(Id);
            ItemInstance inventory = sesion.Character.Inventory.GetItemInstanceById(Id);
            int rnd = ServerManager.RandomNumber();
            if (rnd < destroy[Upgrade])
            {
                if (protect == UpgradeProtection.Protected)
                {
                    sesion.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, sesion.Character.CharacterId, 3004), sesion.Character.MapX, sesion.Character.MapY);
                    sesion.SendPacket(sesion.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADESP_FAILED_SAVED"), 11));
                    sesion.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADESP_FAILED_SAVED"), 0));
                }
                else
                {
                    sesion.Character.Inventory.RemoveItemAmount(itemToRemove, soul[Upgrade]);
                    wearable.Rare = -2;
                    sesion.SendPacket(sesion.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADESP_DESTROYED"), 11));
                    sesion.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADESP_DESTROYED"), 0));
                    sesion.SendPacket(wearable.GenerateInventoryAdd());
                }
            }
            else if (rnd < upfail[Upgrade])
            {
                if (protect == UpgradeProtection.Protected)
                {
                    sesion.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, sesion.Character.CharacterId, 3004), sesion.Character.MapX, sesion.Character.MapY);
                }
                else
                {
                    sesion.Character.Inventory.RemoveItemAmount(itemToRemove, soul[Upgrade]);
                }
                sesion.SendPacket(sesion.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADESP_FAILED"), 11));
                sesion.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADESP_FAILED"), 0));
            }
            else
            {
                if (protect == UpgradeProtection.Protected)
                {
                    sesion.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, sesion.Character.CharacterId, 3004), sesion.Character.MapX, sesion.Character.MapY);
                }
                sesion.Character.Inventory.RemoveItemAmount(itemToRemove, soul[Upgrade]);
                sesion.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, sesion.Character.CharacterId, 3009), sesion.Character.MapX, sesion.Character.MapY);
                sesion.SendPacket(sesion.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADESP_SUCCESS"), 12));
                sesion.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADESP_SUCCESS"), 0));
                wearable.Upgrade++;
                if (wearable.Upgrade > 8)
                {
                    sesion.Character.Family?.InsertFamilyLog(FamilyLogType.ItemUpgraded, sesion.Character.Name, itemVNum: wearable.ItemVNum, upgrade: wearable.Upgrade);
                }
                sesion.SendPacket(wearable.GenerateInventoryAdd());
            }
            sesion.SendPacket(sesion.Character.GenerateGold());
            sesion.SendPacket(sesion.Character.GenerateEq());
            sesion.SendPacket("shop_end 1");
        }

        public string GenerateEInfo()
        {
            EquipmentType equipmentslot = Item.EquipmentSlot;
            ItemType itemType = Item.ItemType;
            byte itemClass = Item.Class;
            byte subtype = Item.ItemSubType;
            DateTime test = ItemDeleteTime ?? DateTime.Now;
            long time = ItemDeleteTime != null ? (long)test.Subtract(DateTime.Now).TotalSeconds : 0;
            long seconds = IsBound ? time : Item.ItemValidTime;
            if (seconds < 0)
            {
                seconds = 0;
            }
            switch (itemType)
            {
                case ItemType.Weapon:
                    switch (equipmentslot)
                    {
                        case EquipmentType.MainWeapon:
                            return $"e_info {(itemClass == 4 ? 1 : itemClass == 8 ? 5 : 0)} {ItemVNum} {Rare} {Upgrade} {(IsFixed ? 1 : 0)} {Item.LevelMinimum} {Item.DamageMinimum + DamageMinimum} {Item.DamageMaximum + DamageMaximum} {Item.HitRate + HitRate} {Item.CriticalLuckRate + CriticalLuckRate} {Item.CriticalRate + CriticalRate} {Ammo} {Item.MaximumAmmo} {Item.Price} -1 {Rare} {BoundCharacterId ?? 0} {ShellEffects.Count} {ShellEffects.Aggregate(string.Empty, (result, effect) => result += $"{(byte)effect.EffectLevel}.{effect.Effect}.{(byte)effect.Value} ")}"; // Shell Rare, CharacterId, ShellEffectCount, ShellEffects

                        case EquipmentType.SecondaryWeapon:
                            return $"e_info {(itemClass <= 2 ? 1 : 0)} {ItemVNum} {Rare} {Upgrade} {(IsFixed ? 1 : 0)} {Item.LevelMinimum} {Item.DamageMinimum + DamageMinimum} {Item.DamageMaximum + DamageMaximum} {Item.HitRate + HitRate} {Item.CriticalLuckRate + CriticalLuckRate} {Item.CriticalRate + CriticalRate} {Ammo} {Item.MaximumAmmo} {Item.Price} -1 {Rare} {BoundCharacterId ?? 0} {ShellEffects.Count} {ShellEffects.Aggregate(string.Empty, (result, effect) => result += $"{(byte)effect.EffectLevel}.{effect.Effect}.{(byte)effect.Value} ")}";
                    }
                    break;

                case ItemType.Armor:
                    return $"e_info 2 {ItemVNum} {Rare} {Upgrade} {(IsFixed ? 1 : 0)} {Item.LevelMinimum} {Item.CloseDefence + CloseDefence} {Item.DistanceDefence + DistanceDefence} {Item.MagicDefence + MagicDefence} {Item.DefenceDodge + DefenceDodge} {Item.Price} -1 {Rare} {BoundCharacterId ?? 0} {ShellEffects.Count} {ShellEffects.Aggregate(string.Empty, (result, effect) => result += $"{(byte)effect.EffectLevel}.{effect.Effect}.{(byte)effect.Value} ")}";

                case ItemType.Fashion:
                    switch (equipmentslot)
                    {
                        case EquipmentType.CostumeHat:
                            return $"e_info 3 {ItemVNum} {Item.LevelMinimum} {Item.CloseDefence + CloseDefence} {Item.DistanceDefence + DistanceDefence} {Item.MagicDefence + MagicDefence} {Item.DefenceDodge + DefenceDodge} {Item.FireResistance + FireResistance} {Item.WaterResistance + WaterResistance} {Item.LightResistance + LightResistance} {Item.DarkResistance + DarkResistance} {Item.Price} {(Item.ItemValidTime == 0 ? -1 : 0)} 2 {(Item.ItemValidTime == 0 ? -1 : seconds / 3600)}";

                        case EquipmentType.CostumeSuit:
                            return $"e_info 2 {ItemVNum} {Rare} {Upgrade} {(IsFixed ? 1 : 0)} {Item.LevelMinimum} {Item.CloseDefence + CloseDefence} {Item.DistanceDefence + DistanceDefence} {Item.MagicDefence + MagicDefence} {Item.DefenceDodge + DefenceDodge} {Item.Price} {(Item.ItemValidTime == 0 ? -1 : 0)} 1 {(Item.ItemValidTime == 0 ? -1 : seconds / 3600)}"; // 1 = IsCosmetic -1 = no shells

                        default:
                            return $"e_info 3 {ItemVNum} {Item.LevelMinimum} {Item.CloseDefence + CloseDefence} {Item.DistanceDefence + DistanceDefence} {Item.MagicDefence + MagicDefence} {Item.DefenceDodge + DefenceDodge} {Item.FireResistance + FireResistance} {Item.WaterResistance + WaterResistance} {Item.LightResistance + LightResistance} {Item.DarkResistance + DarkResistance} {Item.Price} {Upgrade} 0 -1"; // after Item.Price theres TimesConnected {(Item.ItemValidTime == 0 ? -1 : Item.ItemValidTime / (3600))}
                    }

                case ItemType.Jewelery:
                    switch (equipmentslot)
                    {
                        case EquipmentType.Amulet:
                            return $"e_info 4 {ItemVNum} {Item.LevelMinimum} {seconds * 10} 0 0 {Item.Price}";

                        case EquipmentType.Fairy:
                            return $"e_info 4 {ItemVNum} {Item.Element} {ElementRate + Item.ElementRate} 0 0 0 0 0"; // last IsNosmall

                        default:
                            string cellon = "";
                            foreach (CellonOptionDTO option in CellonOptions)
                            {
                                cellon += $" {(byte)option.Type} {option.Level} {option.Value}";
                            }
                            return $"e_info 4 {ItemVNum} {Item.LevelMinimum} {Item.MaxCellonLvl} {Item.MaxCellon} {CellonOptions.Count} {Item.Price}{cellon}";
                    }
                case ItemType.Specialist:
                    return $"e_info 8 {ItemVNum}";

                case ItemType.Box:
                    switch (subtype)
                    {
                        case 0:
                            return HoldingVNum == 0 ?
                                $"e_info 7 {ItemVNum} 0" : $"e_info 7 {ItemVNum} 1 {HoldingVNum} {SpLevel} {XP} 100 {SpDamage} {SpDefence}";

                        case 2:
                            Item spitem = ServerManager.GetItem(HoldingVNum);
                            return HoldingVNum == 0 ?
                                $"e_info 7 {ItemVNum} 0" :
                                $"e_info 7 {ItemVNum} 1 {HoldingVNum} {SpLevel} {XP} {CharacterHelper.SPXPData[SpLevel == 0 ? 0 : SpLevel - 1]} {Upgrade} {CharacterHelper.SlPoint(SlDamage, 0)} {CharacterHelper.SlPoint(SlDefence, 1)} {CharacterHelper.SlPoint(SlElement, 2)} {CharacterHelper.SlPoint(SlHP, 3)} {CharacterHelper.SPPoint(SpLevel, Upgrade) - SlDamage - SlHP - SlElement - SlDefence} {SpStoneUpgrade} {spitem.FireResistance} {spitem.WaterResistance} {spitem.LightResistance} {spitem.DarkResistance} {SpDamage} {SpDefence} {SpElement} {SpHP} {SpFire} {SpWater} {SpLight} {SpDark}";

                        case 4:
                            return HoldingVNum == 0 ?
                                $"e_info 11 {ItemVNum} 0" :
                                $"e_info 11 {ItemVNum} 1 {HoldingVNum}";

                        case 5:
                            Item fairyitem = ServerManager.GetItem(HoldingVNum);
                            return HoldingVNum == 0 ?
                                $"e_info 12 {ItemVNum} 0" :
                                $"e_info 12 {ItemVNum} 1 {HoldingVNum} {ElementRate + fairyitem.ElementRate}";

                        default:
                            return $"e_info 8 {ItemVNum} {Design} {Rare}";
                    }

                case ItemType.Shell:
                    return $"e_info 9 {ItemVNum} {Upgrade} {Rare} {Item.Price} {ShellEffects.Count} {ShellEffects.Aggregate(string.Empty, (result, effect) => result += $"{(byte)effect.EffectLevel}.{effect.Effect}.{(byte)effect.Value} ")}"; // 0 = Number of effects
            }
            return string.Empty;
        }

        public void OptionItem(ClientSession session, short cellonVNum)
        {
            if (EquipmentSerialId == Guid.Empty)
            {
                EquipmentSerialId = Guid.NewGuid();
            }
            if (session.Character.Inventory.CountItem(cellonVNum) > 0 && Item.MaxCellon > CellonOptions.Count)
            {
                byte dataIndex = 0;
                int goldAmount = 0;
                switch (cellonVNum)
                {
                    case 1017:
                        dataIndex = 0;
                        goldAmount = 700;
                        break;

                    case 1018:
                        dataIndex = 1;
                        goldAmount = 1400;
                        break;

                    case 1019:
                        dataIndex = 2;
                        goldAmount = 3000;
                        break;

                    case 1020:
                        dataIndex = 3;
                        goldAmount = 5000;
                        break;

                    case 1021:
                        dataIndex = 4;
                        goldAmount = 10000;
                        break;

                    case 1022:
                        dataIndex = 5;
                        goldAmount = 20000;
                        break;

                    case 1023:
                        dataIndex = 6;
                        goldAmount = 32000;
                        break;

                    case 1024:
                        dataIndex = 7;
                        goldAmount = 58000;
                        break;

                    case 1025:
                        dataIndex = 8;
                        goldAmount = 95000;
                        break;

                    case 1026:
                        return; //no data known, not implemented in the client at all right now

                        //dataIndex = 9;
                        //break;
                }

                if (Item.MaxCellonLvl > dataIndex && session.Character.Gold >= goldAmount)
                {
                    short[][] minimumData = new short[][] {
                    new short[] { 30, 50, 5, 8, 0, 0 },                 //lv1
                    new short[] { 120, 150, 14, 16, 0, 0 },             //lv2
                    new short[] { 220, 280, 22, 28, 0, 0 },             //lv3
                    new short[] { 330, 350, 30, 38, 0, 0 },             //lv4
                    new short[] { 430, 450, 40, 50, 0, 0 },             //lv5
                    new short[] { 600, 600, 55, 65, 1, 1 },             //lv6
                    new short[] { 800, 800, 75, 75, 8, 11 },            //lv7
                    new short[] { 1000, 1000, 100, 100, 13, 21 },       //lv8
                    new short[] { 1100, 1100, 110, 110, 14, 22 },       //lv9
                    new short[] { 0, 0, 0, 0, 0, 0 }                    //lv10 (NOT EXISTING!)
                };
                    short[][] maximumData = new short[][] {
                    new short[] { 100, 150, 10, 15, 0, 0 },          //lv1
                    new short[] { 200, 250, 20, 25, 0, 0 },          //lv1
                    new short[] { 300, 330, 28, 35, 0, 0 },          //lv1
                    new short[] { 400, 420, 38, 45, 0, 0 },          //lv1
                    new short[] { 550, 550, 50, 60, 0, 0 },          //lv1
                    new short[] { 750, 750, 70, 80, 7, 10 },         //lv1
                    new short[] { 1000, 1000, 90,90, 12, 20 },      //lv1
                    new short[] { 1300, 1300, 120, 120, 17, 35 },    //lv1
                    new short[] { 1500, 1500, 135, 135, 21, 45 },    //lv1
                    new short[] { 0, 0, 0, 0, 0, 0 }            //lv10 (NOT EXISTING!)
                };

                    short[] generateOption()
                    {
                        byte option = 0;
                        if (dataIndex < 5)
                        {
                            option = (byte)ServerManager.RandomNumber(0, 4);
                        }
                        else
                        {
                            option = (byte)ServerManager.RandomNumber(0, 6);
                        }

                        if (CellonOptions.Any(s => s.Type == (CellonOptionType)option))
                        {
                            return new short[] { -1, -1 };
                        }

                        return new short[] { option, (short)ServerManager.RandomNumber(minimumData[dataIndex][option], maximumData[dataIndex][option] + 1) };
                    }

                    short[] value = generateOption();
                    Logger.LogUserEvent("OPTION", session.GenerateIdentity(), $"[OptionItem]Serial: {EquipmentSerialId} Type: {value[0]} Value: {value[1]}");
                    if (value[0] != -1)
                    {
                        CellonOptionDTO cellonOptionDTO = new CellonOptionDTO()
                        {
                            EquipmentSerialId = EquipmentSerialId,
                            Level = (byte)(dataIndex + 1),
                            Type = (CellonOptionType)value[0],
                            Value = value[1]
                        };

                        CellonOptions.Add(cellonOptionDTO);

                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("OPTION_SUCCESS"), Rare), 12));
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("OPTION_SUCCESS"), Rare), 0));
                        session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 3009), session.Character.PositionX, session.Character.PositionY);
                        session.SendPacket("shop_end 1");
                    }
                    else
                    {
                        session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("OPTION_FAIL"), Rare), 12));
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("OPTION_FAIL"), Rare), 0));
                        session.SendPacket("shop_end 1");
                    }
                    session.Character.Inventory.RemoveItemAmount(cellonVNum);
                    session.Character.Gold -= goldAmount;
                    session.SendPacket(session.Character.GenerateGold());
                }
            }

            foreach (CellonOptionDTO effect in CellonOptions)
            {
                effect.EquipmentSerialId = EquipmentSerialId;
                effect.CellonOptionId = DAOFactory.CellonOptionDAO.InsertOrUpdate(effect).CellonOptionId;
            }
        }

        public void RarifyItem(ClientSession session, RarifyMode mode, RarifyProtection protection, bool isCommand = false, byte forceRare = 0)
        {
            byte[] rare = { 80, 70, 60, 40, 30, 15, 10, 5, 3, 2, 1 };
            const short goldprice = 500;
            const double reducedpricefactor = 0.5;
            const double reducedchancefactor = 1.1;
            const byte cella = 5;
            const int cellaVnum = 1014;
            const int scrollVnum = 1218;
            double rnd;

            if (session?.HasCurrentMapInstance == false)
            {
                return;
            }
            if (mode != RarifyMode.Drop || Item.ItemType == ItemType.Shell)
            {
                rare[0] = 0;
                rare[1] = 0;
                rare[2] = 0;
                rnd = ServerManager.RandomNumber(0, 80);
            }
            else
            {
                rnd = ServerManager.RandomNumber(0, 1000) / 10D;
            }
            if (protection == RarifyProtection.RedAmulet)
            {
                for (byte i = 0; i < rare.Length; i++)
                {
                    rare[i] = (byte)(rare[i] * reducedchancefactor);
                }
            }
            if (session != null)
            {
                switch (mode)
                {
                    case RarifyMode.Free:
                        break;

                    case RarifyMode.Reduced:
                        if (session.Character.Gold < (long)(goldprice * reducedpricefactor))
                        {
                            return;
                        }
                        if (session.Character.Inventory.CountItem(cellaVnum) < cella * reducedpricefactor)
                        {
                            return;
                        }
                        session.Character.Inventory.RemoveItemAmount(cellaVnum, (int)(cella * reducedpricefactor));
                        session.Character.Gold -= (long)(goldprice * reducedpricefactor);
                        session.SendPacket(session.Character.GenerateGold());
                        break;

                    case RarifyMode.Normal:
                        if (session.Character.Gold < goldprice)
                        {
                            return;
                        }
                        if (session.Character.Inventory.CountItem(cellaVnum) < cella)
                        {
                            return;
                        }
                        if (protection == RarifyProtection.Scroll && !isCommand
                            && session.Character.Inventory.CountItem(scrollVnum) < 1)
                        {
                            return;
                        }

                        if (protection == RarifyProtection.Scroll && !isCommand)
                        {
                            session.Character.Inventory.RemoveItemAmount(scrollVnum);
                            session.SendPacket("shop_end 2");
                        }
                        session.Character.Gold -= goldprice;
                        session.Character.Inventory.RemoveItemAmount(cellaVnum, cella);
                        session.SendPacket(session.Character.GenerateGold());
                        break;

                    case RarifyMode.Drop:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                }
            }

            void rarify(sbyte rarity)
            {
                Rare = rarity;
                if (mode != RarifyMode.Drop)
                {
                    Logger.LogUserEvent("GAMBLE", session.GenerateIdentity(), $"[RarifyItem]Protection: {protection.ToString()} IIId: {Id} ItemVnum: {ItemVNum} Result: Success");

                    session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("RARIFY_SUCCESS"), Rare), 12));
                    session.SendPacket(UserInterfaceHelper.GenerateMsg(string.Format(Language.Instance.GetMessageFromKey("RARIFY_SUCCESS"), Rare), 0));
                    session.CurrentMapInstance?.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, CharacterId, 3009), session.Character.PositionX, session.Character.PositionY);
                    session.SendPacket("shop_end 1");
                }
                SetRarityPoint();
            }

            if (forceRare != 0)
            {
                rarify((sbyte)forceRare);
                return;
            }

            if (rnd < rare[10] && Item.IsHeroic && protection == RarifyProtection.Scroll && !(protection == RarifyProtection.Scroll && Rare >= 8))
            {
                rarify(8);
            }
            if (rnd < rare[9] && !(protection == RarifyProtection.Scroll && Rare >= 7))
            {
                rarify(7);
            }
            else if (rnd < rare[8] && !(protection == RarifyProtection.Scroll && Rare >= 6))
            {
                rarify(6);
            }
            else if (rnd < rare[7] && !(protection == RarifyProtection.Scroll && Rare >= 5))
            {
                rarify(5);
            }
            else if (rnd < rare[6] && !(protection == RarifyProtection.Scroll && Rare >= 4))
            {
                rarify(4);
            }
            else if (rnd < rare[5] && !(protection == RarifyProtection.Scroll && Rare >= 3))
            {
                rarify(3);
            }
            else if (rnd < rare[4] && !(protection == RarifyProtection.Scroll && Rare >= 2))
            {
                rarify(2);
            }
            else if (rnd < rare[3] && !(protection == RarifyProtection.Scroll && Rare >= 1))
            {
                rarify(1);
            }
            else if (rnd < rare[2] && !(protection == RarifyProtection.Scroll && Rare >= 0))
            {
                rarify(0);
            }
            else if (rnd < rare[1] && !(protection == RarifyProtection.Scroll && Rare >= -1))
            {
                rarify(-1);
            }
            else if (rnd < rare[0] && !(protection == RarifyProtection.Scroll && Rare >= -2))
            {
                rarify(-2);
            }
            else if (mode != RarifyMode.Drop && session != null)
            {
                if (protection == RarifyProtection.None)
                {
                    Logger.LogUserEvent("GAMBLE", session.GenerateIdentity(), $"[RarifyItem]Protection: {protection.ToString()} IIId: {Id} ItemVnum: {ItemVNum} Result: Destroyed");

                    session.Character.DeleteItemByItemInstanceId(Id);
                    session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("RARIFY_FAILED"), 11));
                    session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("RARIFY_FAILED"), 0));
                }
                else
                {
                    Logger.LogUserEvent("GAMBLE", session.GenerateIdentity(), $"[RarifyItem]Protection: {protection.ToString()} IIId: {Id} ItemVnum: {ItemVNum} Result: Fail");

                    session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("RARIFY_FAILED_ITEM_SAVED"), 11));
                    session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("RARIFY_FAILED_ITEM_SAVED"), 0));
                    session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3004), session.Character.MapX, session.Character.MapY);
                }
            }
            if (mode != RarifyMode.Drop && session != null)
            {
                ItemInstance inventory = session.Character.Inventory.GetItemInstanceById(Id);
                if (inventory != null)
                {
                    session.SendPacket(inventory.GenerateInventoryAdd());
                }
            }
        }

        public void SetRarityPoint()
        {
            switch (Item.EquipmentSlot)
            {
                case EquipmentType.MainWeapon:
                case EquipmentType.SecondaryWeapon:
                    {
                        int point = CharacterHelper.RarityPoint(Rare, Item.IsHeroic ? (short)(95 + Item.LevelMinimum) : Item.LevelMinimum);
                        Concentrate = 0;
                        HitRate = 0;
                        DamageMinimum = 0;
                        DamageMaximum = 0;
                        if (Rare >= 0)
                        {
                            for (int i = 0; i < point; i++)
                            {
                                int rndn = ServerManager.RandomNumber(0, 3);
                                if (rndn == 0)
                                {
                                    Concentrate++;
                                    HitRate++;
                                }
                                else
                                {
                                    DamageMinimum++;
                                    DamageMaximum++;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i > Rare * 10; i--)
                            {
                                DamageMinimum--;
                                DamageMaximum--;
                            }
                        }
                    }
                    break;

                case EquipmentType.Armor:
                    {
                        int point = CharacterHelper.RarityPoint(Rare, Item.IsHeroic ? (short)(95 + Item.LevelMinimum) : Item.LevelMinimum);
                        DefenceDodge = 0;
                        DistanceDefenceDodge = 0;
                        DistanceDefence = 0;
                        MagicDefence = 0;
                        CloseDefence = 0;
                        if (Rare >= 0)
                        {
                            for (int i = 0; i < point; i++)
                            {
                                int rndn = ServerManager.RandomNumber(0, 3);
                                if (rndn == 0)
                                {
                                    DefenceDodge++;
                                    DistanceDefenceDodge++;
                                }
                                else
                                {
                                    DistanceDefence++;
                                    MagicDefence++;
                                    CloseDefence++;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i > Rare * 10; i--)
                            {
                                DistanceDefence--;
                                MagicDefence--;
                                CloseDefence--;
                            }
                        }
                    }
                    break;
            }
        }

        public void SetShellEffects()
        {
            byte CNormCount = 0;
            byte BNormCount = 0;
            byte ANormCount = 0;
            byte SNormCount = 0;
            byte CBonusMax = 0;
            byte BBonusMax = 0;
            byte ABonusMax = 0;
            byte SBonusMax = 0;
            byte CPVPMax = 0;
            byte BPVPMax = 0;
            byte APVPMax = 0;
            byte SPVPMax = 0;

            byte ShellType = 0;
            bool IsWeapon = true;

            switch (ItemVNum)
            {
                case 589:
                case 590:
                case 591:
                case 592:
                case 593:
                case 594:
                case 595:
                case 596:
                case 597:
                case 598:
                    ShellType = 0;
                    break;

                case 565:
                case 566:
                case 567:
                    ShellType = 1;
                    break;

                case 568:
                case 569:
                case 570:
                    ShellType = 2;
                    break;

                case 571:
                case 572:
                case 573:
                    ShellType = 3;
                    break;

                case 574:
                case 575:
                case 576:
                    ShellType = 4;
                    break;

                case 599:
                case 656:
                case 657:
                case 658:
                case 659:
                case 660:
                case 661:
                case 662:
                case 663:
                case 664:
                    ShellType = 0;
                    IsWeapon = false;
                    break;

                case 577:
                case 578:
                case 579:
                    ShellType = 1;
                    IsWeapon = false;
                    break;

                case 580:
                case 581:
                case 582:
                    ShellType = 2;
                    IsWeapon = false;
                    break;

                case 583:
                case 584:
                case 585:
                    ShellType = 3;
                    IsWeapon = false;
                    break;

                case 586:
                case 587:
                case 588:
                    ShellType = 4;
                    IsWeapon = false;
                    break;
            }

            switch (Rare)
            {
                case 1:
                    switch (ShellType)
                    {
                        case 0:
                            CNormCount = 1;
                            BNormCount = 0;
                            ANormCount = 0;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 1:
                            CNormCount = 1;
                            BNormCount = 0;
                            ANormCount = 0;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 2:
                            CNormCount = 1;
                            BNormCount = 0;
                            ANormCount = 0;
                            SNormCount = 0;
                            CBonusMax = 1;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 3:
                            CNormCount = 1;
                            BNormCount = 0;
                            ANormCount = 0;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 1;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 4:
                            CNormCount = 1;
                            BNormCount = 0;
                            ANormCount = 0;
                            SNormCount = 0;
                            CBonusMax = 1;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 1;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;
                    }
                    break;

                case 2:
                    switch (ShellType)
                    {
                        case 0:
                            CNormCount = 2;
                            BNormCount = 0;
                            ANormCount = 0;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 1:
                            CNormCount = 2;
                            BNormCount = 0;
                            ANormCount = 0;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 2:
                            CNormCount = 1;
                            BNormCount = 0;
                            ANormCount = 0;
                            SNormCount = 0;
                            CBonusMax = 1;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 3:
                            CNormCount = 1;
                            BNormCount = 0;
                            ANormCount = 0;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 2;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 4:
                            CNormCount = 2;
                            BNormCount = 0;
                            ANormCount = 0;
                            SNormCount = 0;
                            CBonusMax = 1;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 1;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;
                    }
                    break;

                case 3:
                    switch (ShellType)
                    {
                        case 0:
                            CNormCount = 2;
                            BNormCount = 1;
                            ANormCount = 0;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 1:
                            CNormCount = 2;
                            BNormCount = 1;
                            ANormCount = 0;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 2:
                            CNormCount = 1;
                            BNormCount = 1;
                            ANormCount = 0;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 1;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 3:
                            CNormCount = 1;
                            BNormCount = 1;
                            ANormCount = 0;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 1;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 4:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 0;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 1;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;
                    }
                    break;

                case 4:
                    switch (ShellType)
                    {
                        case 0:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 0;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 1:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 0;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 2:
                            CNormCount = 1;
                            BNormCount = 1;
                            ANormCount = 0;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 1;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 3:
                            CNormCount = 1;
                            BNormCount = 1;
                            ANormCount = 0;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 2;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 4:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 0;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 1;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 1;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;
                    }
                    break;

                case 5:
                    switch (ShellType)
                    {
                        case 0:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 1;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 1:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 1;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 2:
                            CNormCount = 1;
                            BNormCount = 1;
                            ANormCount = 1;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 1;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 3:
                            CNormCount = 1;
                            BNormCount = 1;
                            ANormCount = 1;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 2;
                            APVPMax = 1;
                            SPVPMax = 0;
                            break;

                        case 4:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 1;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 1;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 1;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;
                    }
                    break;

                case 6:
                    switch (ShellType)
                    {
                        case 0:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 2;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 1:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 2;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 2:
                            CNormCount = 1;
                            BNormCount = 1;
                            ANormCount = 2;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 1;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 3:
                            CNormCount = 1;
                            BNormCount = 1;
                            ANormCount = 2;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 2;
                            APVPMax = 2;
                            SPVPMax = 0;
                            break;

                        case 4:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 2;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 1;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 1;
                            APVPMax = 1;
                            SPVPMax = 0;
                            break;
                    }
                    break;

                case 7:
                    switch (ShellType)
                    {
                        case 0:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 2;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 1:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 2;
                            SNormCount = 1;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 2:
                            CNormCount = 1;
                            BNormCount = 1;
                            ANormCount = 2;
                            SNormCount = 1;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 1;
                            CPVPMax = 0;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 3:
                            CNormCount = 1;
                            BNormCount = 1;
                            ANormCount = 2;
                            SNormCount = 1;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 2;
                            APVPMax = 2;
                            SPVPMax = 2;
                            break;

                        case 4:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 2;
                            SNormCount = 1;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 1;
                            CPVPMax = 0;
                            BPVPMax = 1;
                            APVPMax = 1;
                            SPVPMax = 1;
                            break;
                    }
                    break;

                case 8:
                    switch (ShellType)
                    {
                        case 0:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 2;
                            SNormCount = 0;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 1:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 2;
                            SNormCount = 2;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 2:
                            CNormCount = 1;
                            BNormCount = 1;
                            ANormCount = 2;
                            SNormCount = 2;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 2;
                            CPVPMax = 0;
                            BPVPMax = 0;
                            APVPMax = 0;
                            SPVPMax = 0;
                            break;

                        case 3:
                            CNormCount = 1;
                            BNormCount = 1;
                            ANormCount = 2;
                            SNormCount = 2;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 0;
                            CPVPMax = 0;
                            BPVPMax = 2;
                            APVPMax = 2;
                            SPVPMax = 3;
                            break;

                        case 4:
                            CNormCount = 2;
                            BNormCount = 2;
                            ANormCount = 2;
                            SNormCount = 2;
                            CBonusMax = 0;
                            BBonusMax = 0;
                            ABonusMax = 0;
                            SBonusMax = 2;
                            CPVPMax = 0;
                            BPVPMax = 1;
                            APVPMax = 1;
                            SPVPMax = 2;
                            break;
                    }
                    break;
            }

            List<ShellEffectDTO> effectsList = new List<ShellEffectDTO>();

            if (!IsWeapon && SPVPMax == 3)
            {
                SPVPMax = 2;
            }
            if (EquipmentSerialId == Guid.Empty)
            {
                EquipmentSerialId = Guid.NewGuid();
            }
            short CalculateEffect(short maximum)
            {
                if (maximum == 0)
                {
                    return 1;
                }
                else
                {
                    double multiplier = 0;
                    switch (Rare)
                    {
                        case 0:
                        case 1:
                        case 2:
                            multiplier = 0.6;
                            break;

                        case 3:
                            multiplier = 0.65;
                            break;

                        case 4:
                            multiplier = 0.75;
                            break;

                        case 5:
                            multiplier = 0.85;
                            break;

                        case 6:
                            multiplier = 0.95;
                            break;

                        case 7:
                        case 8:
                            multiplier = 1;
                            break;
                    }

                    short min = (short)((maximum / 80D) * (Upgrade - 20) * multiplier);
                    short max = (short)((maximum / 80D) * Upgrade * multiplier);
                    if (min == 0)
                    {
                        min = 1;
                    }
                    if (max <= min)
                    {
                        max = (short)(min + 1);
                    }
                    return (short)ServerManager.RandomNumber(min, max);
                }
            }

            void AddEffect(ShellEffectLevelType levelType)
            {
                int i = 0;
                while (i < 20)
                {
                    i++;
                    switch (levelType)
                    {
                        case ShellEffectLevelType.CNormal:
                            {
                                byte[] effects = new byte[]
                                {
                                    (byte)ShellWeaponEffectType.DamageImproved,
                                    (byte)ShellWeaponEffectType.DamageIncreasedtothePlant,
                                    (byte)ShellWeaponEffectType.DamageIncreasedtotheAnimal,
                                    (byte)ShellWeaponEffectType.DamageIncreasedtotheEnemy,
                                    (byte)ShellWeaponEffectType.CriticalChance,
                                    (byte)ShellWeaponEffectType.CriticalDamage,
                                    (byte)ShellWeaponEffectType.AntiMagicDisorder,
                                    (byte)ShellWeaponEffectType.ReducedMPConsume,
                                    (byte)ShellWeaponEffectType.Blackout,
                                    (byte)ShellWeaponEffectType.MinorBleeding,
                                };
                                short[] maximum = new short[] { 80, 8, 8, 8, 10, 50, 0, 10, 4, 4 };

                                if (!IsWeapon)
                                {
                                    effects = new byte[]
                                    {
                                        (byte)ShellArmorEffectType.CloseDefence,
                                        (byte)ShellArmorEffectType.DistanceDefence,
                                        (byte)ShellArmorEffectType.MagicDefence,
                                        (byte)ShellArmorEffectType.ReducedCritChanceRecive,
                                        (byte)ShellArmorEffectType.ReducedStun,
                                        (byte)ShellArmorEffectType.ReducedMinorBleeding,
                                        (byte)ShellArmorEffectType.ReducedShock,
                                        (byte)ShellArmorEffectType.ReducedPoisonParalysis,
                                        (byte)ShellArmorEffectType.ReducedBlind,
                                        (byte)ShellArmorEffectType.RecoveryHPOnRest,
                                        (byte)ShellArmorEffectType.RecoveryMPOnRest
                                    };
                                    maximum = new short[] { 55, 55, 55, 8, 30, 30, 45, 15, 30, 48, 48 };
                                }

                                int position = ServerManager.RandomNumber(0, effects.Length);
                                byte effect = effects[position];
                                short value = CalculateEffect(maximum[position]);

                                if (effectsList.Any(s => s.Effect == effect))
                                {
                                    continue;
                                }

                                effectsList.Add(new ShellEffectDTO() { EffectLevel = ShellEffectLevelType.CNormal, Effect = effect, Value = value, EquipmentSerialId = EquipmentSerialId });
                                return;
                            }
                        case ShellEffectLevelType.BNormal:
                            {
                                byte[] effects = new byte[]
                                {
                                    (byte)ShellWeaponEffectType.DamageImproved,
                                    (byte)ShellWeaponEffectType.DamageIncreasedtothePlant,
                                    (byte)ShellWeaponEffectType.DamageIncreasedtotheAnimal,
                                    (byte)ShellWeaponEffectType.DamageIncreasedtotheEnemy,
                                    (byte)ShellWeaponEffectType.DamageIncreasedtotheUnDead,
                                    (byte)ShellWeaponEffectType.DamageincreasedtotheSmallMonster,
                                    (byte)ShellWeaponEffectType.CriticalChance,
                                    (byte)ShellWeaponEffectType.CriticalDamage,
                                    (byte)ShellWeaponEffectType.ReducedMPConsume,
                                    (byte)ShellWeaponEffectType.Freeze,
                                    (byte)ShellWeaponEffectType.Bleeding,
                                    (byte)ShellWeaponEffectType.IncreasedFireProperties,
                                    (byte)ShellWeaponEffectType.IncreasedWaterProperties,
                                    (byte)ShellWeaponEffectType.IncreasedLightProperties,
                                    (byte)ShellWeaponEffectType.IncreasedDarkProperties,
                                    (byte)ShellWeaponEffectType.SLDamage,
                                    (byte)ShellWeaponEffectType.SLDefence,
                                    (byte)ShellWeaponEffectType.SLElement,
                                    (byte)ShellWeaponEffectType.SLHP,
                                    (byte)ShellWeaponEffectType.HPRecoveryForKilling,
                                    (byte)ShellWeaponEffectType.MPRecoveryForKilling
                                };
                                short[] maximum = new short[] { 120, 16, 16, 16, 8, 8, 16, 75, 20, 4, 4, 61, 61, 61, 61, 10, 10, 10, 10, 150, 150 };

                                if (!IsWeapon)
                                {
                                    effects = new byte[]
                                    {
                                        (byte)ShellArmorEffectType.CloseDefence,
                                        (byte)ShellArmorEffectType.DistanceDefence,
                                        (byte)ShellArmorEffectType.MagicDefence,
                                        (byte)ShellArmorEffectType.ReducedCritChanceRecive,
                                        (byte)ShellArmorEffectType.ReducedBlind,
                                        (byte)ShellArmorEffectType.RecoveryHPOnRest,
                                        (byte)ShellArmorEffectType.RecoveryMPOnRest,
                                        (byte)ShellArmorEffectType.ReducedBleedingAndMinorBleeding,
                                        (byte)ShellArmorEffectType.ReducedArmorDeBuff,
                                        (byte)ShellArmorEffectType.ReducedFreeze,
                                        (byte)ShellArmorEffectType.ReducedParalysis,
                                        (byte)ShellArmorEffectType.IncreasedFireResistence,
                                        (byte)ShellArmorEffectType.IncreasedWaterResistence,
                                        (byte)ShellArmorEffectType.IncreasedLightResistence,
                                        (byte)ShellArmorEffectType.IncreasedDarkResistence
                                    };
                                    maximum = new short[] { 95, 95, 95, 13, 40, 85, 85, 27, 42, 38, 27, 8, 8, 8, 8 };
                                }

                                int position = ServerManager.RandomNumber(0, effects.Length);
                                byte effect = effects[position];
                                short value = CalculateEffect(maximum[position]);

                                if (effectsList.Any(s => s.Effect == effect))
                                {
                                    continue;
                                }

                                effectsList.Add(new ShellEffectDTO() { EffectLevel = ShellEffectLevelType.BNormal, Effect = effect, Value = value, EquipmentSerialId = EquipmentSerialId });
                                return;
                            }
                        case ShellEffectLevelType.ANormal:
                            {
                                byte[] effects = new byte[]
                                {
                                    (byte)ShellWeaponEffectType.DamageImproved,
                                    (byte)ShellWeaponEffectType.DamageIncreasedtotheUnDead,
                                    (byte)ShellWeaponEffectType.DamageincreasedtotheSmallMonster,
                                    (byte)ShellWeaponEffectType.HeavyBleeding,
                                    (byte)ShellWeaponEffectType.IncreasedFireProperties,
                                    (byte)ShellWeaponEffectType.IncreasedWaterProperties,
                                    (byte)ShellWeaponEffectType.IncreasedLightProperties,
                                    (byte)ShellWeaponEffectType.IncreasedDarkProperties,
                                    (byte)ShellWeaponEffectType.SLDamage,
                                    (byte)ShellWeaponEffectType.SLDefence,
                                    (byte)ShellWeaponEffectType.SLElement,
                                    (byte)ShellWeaponEffectType.SLHP,
                                    (byte)ShellWeaponEffectType.HPRecoveryForKilling,
                                    (byte)ShellWeaponEffectType.MPRecoveryForKilling
                                };
                                short[] maximum = new short[] { 160, 16, 16, 1, 125, 125, 125, 125, 15, 15, 15, 15, 175, 175 };

                                if (!IsWeapon)
                                {
                                    effects = new byte[]
                                    {
                                        (byte)ShellArmorEffectType.CloseDefence,
                                        (byte)ShellArmorEffectType.DistanceDefence,
                                        (byte)ShellArmorEffectType.MagicDefence,
                                        (byte)ShellArmorEffectType.ReducedFreeze,
                                        (byte)ShellArmorEffectType.ReducedParalysis,
                                        (byte)ShellArmorEffectType.ReducedAllStun,
                                        (byte)ShellArmorEffectType.ReducedAllBleedingType,
                                        (byte)ShellArmorEffectType.RecoveryHP,
                                        (byte)ShellArmorEffectType.RecoveryMP,
                                        (byte)ShellArmorEffectType.IncreasedFireResistence,
                                        (byte)ShellArmorEffectType.IncreasedWaterResistence,
                                        (byte)ShellArmorEffectType.IncreasedLightResistence,
                                        (byte)ShellArmorEffectType.IncreasedDarkResistence
                                    };
                                    maximum = new short[] { 160, 160, 160, 43, 35, 40, 40, 80, 80, 16, 16, 16, 16 };
                                }

                                int position = ServerManager.RandomNumber(0, effects.Length);
                                byte effect = effects[position];
                                short value = CalculateEffect(maximum[position]);

                                if (effectsList.Any(s => s.Effect == effect))
                                {
                                    continue;
                                }

                                effectsList.Add(new ShellEffectDTO() { EffectLevel = ShellEffectLevelType.ANormal, Effect = effect, Value = value, EquipmentSerialId = EquipmentSerialId });
                                return;
                            }
                        case ShellEffectLevelType.SNormal:
                            {
                                byte[] effects = new byte[]
                                {
                                    (byte)ShellWeaponEffectType.PercentageTotalDamage,
                                    (byte)ShellWeaponEffectType.DamageincreasedtotheBigMonster,
                                    (byte)ShellWeaponEffectType.IncreasedElementalProperties,
                                    (byte)ShellWeaponEffectType.SLGlobal,
                                };
                                short[] maximum = new short[] { 20, 25, 140, 9 };

                                if (!IsWeapon)
                                {
                                    effects = new byte[]
                                    {
                                        (byte)ShellArmorEffectType.PercentageTotalDefence,
                                        (byte)ShellArmorEffectType.ReducedAllNegativeEffect,
                                        (byte)ShellArmorEffectType.IncreasedAllResistence,
                                        (byte)ShellArmorEffectType.RecoveryHPInDefence
                                    };
                                    maximum = new short[] { 20, 33, 22, 56 };
                                }

                                int position = ServerManager.RandomNumber(0, effects.Length);
                                byte effect = effects[position];
                                short value = CalculateEffect(maximum[position]);

                                if (effectsList.Any(s => s.Effect == effect))
                                {
                                    continue;
                                }

                                effectsList.Add(new ShellEffectDTO() { EffectLevel = ShellEffectLevelType.SNormal, Effect = effect, Value = value, EquipmentSerialId = EquipmentSerialId });
                                return;
                            }
                        case ShellEffectLevelType.CBonus:
                            {
                                byte[] effects = new byte[]
                                {
                                    (byte)ShellWeaponEffectType.GainMoreGold,
                                    (byte)ShellWeaponEffectType.GainMoreXP,
                                    (byte)ShellWeaponEffectType.GainMoreCXP
                                };
                                short[] maximum = new short[] { 7, 4, 4 };

                                if (!IsWeapon)
                                {
                                    effects = new byte[]
                                    {
                                        (byte)ShellArmorEffectType.ReducedPrideLoss,
                                        (byte)ShellArmorEffectType.ReducedProductionPointConsumed
                                    };
                                    maximum = new short[] { 45, 43 };
                                }

                                int position = ServerManager.RandomNumber(0, effects.Length);
                                byte effect = effects[position];
                                short value = CalculateEffect(maximum[position]);

                                if (effectsList.Any(s => s.Effect == effect))
                                {
                                    continue;
                                }

                                effectsList.Add(new ShellEffectDTO() { EffectLevel = ShellEffectLevelType.CBonus, Effect = effect, Value = value, EquipmentSerialId = EquipmentSerialId });
                                return;
                            }
                        case ShellEffectLevelType.BBonus:
                            {
                                byte[] effects = new byte[]
                                {
                                    (byte)ShellWeaponEffectType.GainMoreGold,
                                    (byte)ShellWeaponEffectType.GainMoreXP,
                                    (byte)ShellWeaponEffectType.GainMoreCXP
                                };
                                short[] maximum = new short[] { 13, 6, 6 };

                                if (!IsWeapon)
                                {
                                    effects = new byte[]
                                    {
                                        (byte)ShellArmorEffectType.ReducedProductionPointConsumed,
                                        (byte)ShellArmorEffectType.IncreasedProductionPossibility,
                                        (byte)ShellArmorEffectType.IncreasedRecoveryItemSpeed
                                    };
                                    maximum = new short[] { 56, 47, 21 };
                                }

                                int position = ServerManager.RandomNumber(0, effects.Length);
                                byte effect = effects[position];
                                short value = CalculateEffect(maximum[position]);

                                if (effectsList.Any(s => s.Effect == effect))
                                {
                                    continue;
                                }

                                effectsList.Add(new ShellEffectDTO() { EffectLevel = ShellEffectLevelType.BBonus, Effect = effect, Value = value, EquipmentSerialId = EquipmentSerialId });
                                return;
                            }
                        case ShellEffectLevelType.ABonus:
                            {
                                byte[] effects = new byte[]
                                {
                                    (byte)ShellWeaponEffectType.GainMoreGold,
                                    (byte)ShellWeaponEffectType.GainMoreXP,
                                    (byte)ShellWeaponEffectType.GainMoreCXP
                                };
                                short[] maximum = new short[] { 28, 12, 12 };

                                if (!IsWeapon)
                                {
                                    effects = new byte[]
                                    {
                                        (byte)ShellArmorEffectType.ReducedProductionPointConsumed,
                                        (byte)ShellArmorEffectType.IncreasedProductionPossibility,
                                        (byte)ShellArmorEffectType.IncreasedRecoveryItemSpeed
                                    };
                                    maximum = new short[] { 60, 60, 46 };
                                }

                                int position = ServerManager.RandomNumber(0, effects.Length);
                                byte effect = effects[position];
                                short value = CalculateEffect(maximum[position]);

                                if (effectsList.Any(s => s.Effect == effect))
                                {
                                    continue;
                                }

                                effectsList.Add(new ShellEffectDTO() { EffectLevel = ShellEffectLevelType.ABonus, Effect = effect, Value = value, EquipmentSerialId = EquipmentSerialId });
                                return;
                            }
                        case ShellEffectLevelType.SBonus:
                            {
                                byte[] effects = new byte[]
                                {
                                    (byte)ShellWeaponEffectType.GainMoreGold,
                                    (byte)ShellWeaponEffectType.GainMoreXP,
                                    (byte)ShellWeaponEffectType.GainMoreCXP
                                };
                                short[] maximum = new short[] { 40, 18, 18 };

                                if (!IsWeapon)
                                {
                                    effects = new byte[]
                                    {
                                        (byte)ShellArmorEffectType.ReducedProductionPointConsumed,
                                        (byte)ShellArmorEffectType.IncreasedProductionPossibility,
                                        (byte)ShellArmorEffectType.IncreasedRecoveryItemSpeed
                                    };
                                    maximum = new short[] { 60, 75, 55 };
                                }

                                int position = ServerManager.RandomNumber(0, effects.Length);
                                byte effect = effects[position];
                                short value = CalculateEffect(maximum[position]);

                                if (effectsList.Any(s => s.Effect == effect))
                                {
                                    continue;
                                }

                                effectsList.Add(new ShellEffectDTO() { EffectLevel = ShellEffectLevelType.SBonus, Effect = effect, Value = value, EquipmentSerialId = EquipmentSerialId });
                                return;
                            }
                        case ShellEffectLevelType.CPVP:
                            {
                                byte[] effects = new byte[]
                                {
                                    (byte)ShellWeaponEffectType.PercentageDamageInPVP,
                                    (byte)ShellWeaponEffectType.ReducesPercentageEnemyDefenceInPVP,
                                    (byte)ShellWeaponEffectType.PVPDamageAt15Percent,
                                    (byte)ShellWeaponEffectType.ReducesEnemyMPInPVP,
                                };
                                short[] maximum = new short[] { 8, 8, 54, 12 };

                                if (!IsWeapon)
                                {
                                    effects = new byte[]
                                    {
                                        (byte)ShellArmorEffectType.PercentageAllPVPDefence,
                                        (byte)ShellArmorEffectType.CloseDefenceDodgeInPVP,
                                        (byte)ShellArmorEffectType.DistanceDefenceDodgeInPVP,
                                        (byte)ShellArmorEffectType.IgnoreMagicDamage
                                    };
                                    maximum = new short[] { 9, 4, 4, 4 };
                                }

                                int position = ServerManager.RandomNumber(0, effects.Length);
                                byte effect = effects[position];
                                short value = CalculateEffect(maximum[position]);

                                if (effectsList.Any(s => s.Effect == effect))
                                {
                                    continue;
                                }

                                effectsList.Add(new ShellEffectDTO() { EffectLevel = ShellEffectLevelType.CPVP, Effect = effect, Value = value, EquipmentSerialId = EquipmentSerialId });
                                return;
                            }
                        case ShellEffectLevelType.BPVP:
                            {
                                byte[] effects = new byte[]
                                {
                                    (byte)ShellWeaponEffectType.PercentageDamageInPVP,
                                    (byte)ShellWeaponEffectType.ReducesPercentageEnemyDefenceInPVP,
                                    (byte)ShellWeaponEffectType.ReducesEnemyMPInPVP,
                                    (byte)ShellWeaponEffectType.ReducesEnemyFireResistanceInPVP,
                                    (byte)ShellWeaponEffectType.ReducesEnemyWaterResistanceInPVP,
                                    (byte)ShellWeaponEffectType.ReducesEnemyLightResistanceInPVP,
                                    (byte)ShellWeaponEffectType.ReducesEnemyDarkResistanceInPVP
                                };
                                short[] maximum = new short[] { 12, 12, 20, 6, 6, 6, 6 };

                                if (!IsWeapon)
                                {
                                    effects = new byte[]
                                    {
                                        (byte)ShellArmorEffectType.PercentageAllPVPDefence,
                                        (byte)ShellArmorEffectType.CloseDefenceDodgeInPVP,
                                        (byte)ShellArmorEffectType.DistanceDefenceDodgeInPVP,
                                        (byte)ShellArmorEffectType.IgnoreMagicDamage
                                    };
                                    maximum = new short[] { 11, 6, 6, 6 };
                                }

                                int position = ServerManager.RandomNumber(0, effects.Length);
                                byte effect = effects[position];
                                short value = CalculateEffect(maximum[position]);

                                if (effectsList.Any(s => s.Effect == effect))
                                {
                                    continue;
                                }

                                effectsList.Add(new ShellEffectDTO() { EffectLevel = ShellEffectLevelType.BPVP, Effect = effect, Value = value, EquipmentSerialId = EquipmentSerialId });
                                return;
                            }
                        case ShellEffectLevelType.APVP:
                            {
                                byte[] effects = new byte[]
                                {
                                    (byte)ShellWeaponEffectType.PercentageDamageInPVP,
                                    (byte)ShellWeaponEffectType.ReducesPercentageEnemyDefenceInPVP,
                                    (byte)ShellWeaponEffectType.ReducesEnemyMPInPVP,
                                    (byte)ShellWeaponEffectType.ReducesEnemyFireResistanceInPVP,
                                    (byte)ShellWeaponEffectType.ReducesEnemyWaterResistanceInPVP,
                                    (byte)ShellWeaponEffectType.ReducesEnemyLightResistanceInPVP,
                                    (byte)ShellWeaponEffectType.ReducesEnemyDarkResistanceInPVP
                                };
                                short[] maximum = new short[] { 17, 17, 42, 15, 15, 15, 15 };

                                if (!IsWeapon)
                                {
                                    effects = new byte[]
                                    {
                                        (byte)ShellArmorEffectType.PercentageAllPVPDefence,
                                        (byte)ShellArmorEffectType.CloseDefenceDodgeInPVP,
                                        (byte)ShellArmorEffectType.DistanceDefenceDodgeInPVP,
                                        (byte)ShellArmorEffectType.IgnoreMagicDamage,
                                        (byte)ShellArmorEffectType.ProtectMPInPVP
                                    };
                                    maximum = new short[] { 20, 12, 12, 12, 0 };
                                }

                                int position = ServerManager.RandomNumber(0, effects.Length);
                                byte effect = effects[position];
                                short value = CalculateEffect(maximum[position]);

                                if (effectsList.Any(s => s.Effect == effect))
                                {
                                    continue;
                                }

                                effectsList.Add(new ShellEffectDTO() { EffectLevel = ShellEffectLevelType.APVP, Effect = effect, Value = value, EquipmentSerialId = EquipmentSerialId });
                                return;
                            }
                        case ShellEffectLevelType.SPVP:
                            {
                                byte[] effects = new byte[]
                                {
                                    (byte)ShellWeaponEffectType.PercentageDamageInPVP,
                                    (byte)ShellWeaponEffectType.ReducesPercentageEnemyDefenceInPVP,
                                    (byte)ShellWeaponEffectType.ReducesEnemyAllResistancesInPVP
                                };
                                short[] maximum = new short[] { 35, 35, 17 };

                                if (!IsWeapon)
                                {
                                    effects = new byte[]
                                    {
                                        (byte)ShellArmorEffectType.PercentageAllPVPDefence,
                                        (byte)ShellArmorEffectType.DodgeAllAttacksInPVP
                                    };
                                    maximum = new short[] { 32, 16 };
                                }

                                int position = ServerManager.RandomNumber(0, effects.Length);
                                byte effect = effects[position];
                                short value = CalculateEffect(maximum[position]);

                                if (effectsList.Any(s => s.Effect == effect))
                                {
                                    continue;
                                }

                                effectsList.Add(new ShellEffectDTO() { EffectLevel = ShellEffectLevelType.SPVP, Effect = effect, Value = value, EquipmentSerialId = EquipmentSerialId });
                                return;
                            }
                    }
                }
            }

            for (int i = 0; i < CNormCount; i++)
            {
                AddEffect(ShellEffectLevelType.CNormal);
            }

            for (int i = 0; i < BNormCount; i++)
            {
                AddEffect(ShellEffectLevelType.BNormal);
            }

            for (int i = 0; i < ANormCount; i++)
            {
                AddEffect(ShellEffectLevelType.ANormal);
            }

            for (int i = 0; i < SNormCount; i++)
            {
                AddEffect(ShellEffectLevelType.SNormal);
            }

            for (int i = 0; i < CBonusMax; i++)
            {
                AddEffect(ShellEffectLevelType.CBonus);
            }

            for (int i = 0; i < BBonusMax; i++)
            {
                AddEffect(ShellEffectLevelType.BBonus);
            }

            for (int i = 0; i < ABonusMax; i++)
            {
                AddEffect(ShellEffectLevelType.ABonus);
            }

            for (int i = 0; i < SBonusMax; i++)
            {
                AddEffect(ShellEffectLevelType.SBonus);
            }

            for (int i = 0; i < SPVPMax; i++)
            {
                AddEffect(ShellEffectLevelType.SPVP);
            }

            for (int i = 0; i < APVPMax; i++)
            {
                AddEffect(ShellEffectLevelType.APVP);
            }

            for (int i = 0; i < BPVPMax; i++)
            {
                AddEffect(ShellEffectLevelType.BPVP);
            }

            for (int i = 0; i < CPVPMax; i++)
            {
                AddEffect(ShellEffectLevelType.CPVP);
            }

            ShellEffects.Clear();
            ShellEffects.AddRange(effectsList);

            DAOFactory.ShellEffectDAO.InsertOrUpdateFromList(ShellEffects, EquipmentSerialId);
        }

        public void Sum(ClientSession session, ItemInstance itemToSum)
        {
            if (!session.HasCurrentMapInstance)
            {
                return;
            }
            if (Upgrade < 10)
            {
                short[] upsuccess = { 100, 100, 85, 70, 50, 20 };
                int[] goldprice = { 1500, 3000, 6000, 12000, 24000, 48000 };
                short[] sand = { 5, 10, 15, 20, 25, 30 };
                const int sandVnum = 1027;
                if (Upgrade + itemToSum.Upgrade < 10 && ((itemToSum.Item.EquipmentSlot == EquipmentType.Gloves && Item.EquipmentSlot == EquipmentType.Gloves) || (Item.EquipmentSlot == EquipmentType.Boots && itemToSum.Item.EquipmentSlot == EquipmentType.Boots)))
                {
                    if (session.Character.Gold < goldprice[Upgrade])
                    {
                        return;
                    }
                    if (session.Character.Inventory.CountItem(sandVnum) < sand[Upgrade])
                    {
                        return;
                    }
                    session.Character.Inventory.RemoveItemAmount(sandVnum, (byte)sand[Upgrade]);
                    session.Character.Gold -= goldprice[Upgrade];

                    int rnd = ServerManager.RandomNumber();
                    if (rnd < upsuccess[Upgrade + itemToSum.Upgrade])
                    {
                        Logger.LogUserEvent("SUM_ITEM", session.GenerateIdentity(), $"[SumItem]ItemId {Id} ItemToSumId: {itemToSum.Id} Upgrade: {Upgrade} ItemToSumUpgrade: {itemToSum.Upgrade} Result: Success");

                        Upgrade += (byte)(itemToSum.Upgrade + 1);
                        DarkResistance += (short)(itemToSum.DarkResistance + itemToSum.Item.DarkResistance);
                        LightResistance += (short)(itemToSum.LightResistance + itemToSum.Item.LightResistance);
                        WaterResistance += (short)(itemToSum.WaterResistance + itemToSum.Item.WaterResistance);
                        FireResistance += (short)(itemToSum.FireResistance + itemToSum.Item.FireResistance);
                        session.Character.DeleteItemByItemInstanceId(itemToSum.Id);
                        session.SendPacket($"pdti 10 {ItemVNum} 1 27 {Upgrade} 0");
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SUM_SUCCESS"), 0));
                        session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("SUM_SUCCESS"), 12));
                        session.SendPacket(UserInterfaceHelper.GenerateGuri(19, 1, session.Character.CharacterId, 1324));
                        session.SendPacket(GenerateInventoryAdd());
                    }
                    else
                    {
                        Logger.LogUserEvent("SUM_ITEM", session.GenerateIdentity(), $"[SumItem]ItemId {Id} ItemToSumId: {itemToSum.Id} Upgrade: {Upgrade} ItemToSumUpgrade: {itemToSum.Upgrade} Result: Fail");

                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("SUM_FAILED"), 0));
                        session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("SUM_FAILED"), 11));
                        session.SendPacket(UserInterfaceHelper.GenerateGuri(19, 1, session.Character.CharacterId, 1332));
                        session.Character.DeleteItemByItemInstanceId(itemToSum.Id);
                        session.Character.DeleteItemByItemInstanceId(Id);
                    }
                    session.CurrentMapInstance?.Broadcast(UserInterfaceHelper.GenerateGuri(6, 1, session.Character.CharacterId), session.Character.MapX, session.Character.MapY);
                    session.SendPacket(session.Character.GenerateGold());
                    session.SendPacket("shop_end 1");
                }
            }
        }

        public void UpgradeItem(ClientSession session, UpgradeMode mode, UpgradeProtection protection, bool isCommand = false)
        {
            if (!session.HasCurrentMapInstance)
            {
                return;
            }
            if (Upgrade < 20)
            {
                byte[] upfail;
                byte[] upfix;
                int[] goldprice;
                short[] cella;
                byte[] gem;

                if (Rare >= 8)
                {
                    upfix = new byte[] { 50, 40, 70, 65, 80, 90, 95, 97, 98, 99 };
                    upfail = new byte[] { 50, 40, 60, 50, 60, 70, 75, 77, 83, 89 };

                    goldprice = new[] { 5000, 15000, 30000, 100000, 300000, 800000, 1500000, 4000000, 7000000, 10000000 };
                    cella = new short[] { 40, 100, 160, 240, 320, 440, 560, 760, 960, 1200 };
                    gem = new byte[] { 2, 2, 4, 4, 6, 2, 2, 4, 4, 6 };
                }
                else
                {
                    upfix = new byte[] { 0, 0, 10, 15, 20, 20, 20, 20, 15, 14 };
                    upfail = new byte[] { 0, 0, 0, 5, 20, 40, 60, 70, 80, 85 };

                    goldprice = new[] { 500, 1500, 3000, 10000, 30000, 80000, 150000, 400000, 700000, 1000000 };
                    cella = new short[] { 20, 50, 80, 120, 160, 220, 280, 380, 480, 600 };
                    gem = new byte[] { 1, 1, 2, 2, 3, 1, 1, 2, 2, 3 };
                }

                const short cellaVnum = 1014;
                const short gemVnum = 1015;
                const short gemFullVnum = 1016;
                const double reducedpricefactor = 0.5;
                const short normalScrollVnum = 1218;
                const short goldScrollVnum = 5369;

                if (IsFixed)
                {
                    session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("ITEM_IS_FIXED"), 10));
                    session.SendPacket("shop_end 1");
                    return;
                }
                switch (mode)
                {
                    case UpgradeMode.Free:
                        break;

                    case UpgradeMode.Reduced:
                        if (session.Character.Gold < (long)(goldprice[Upgrade] * reducedpricefactor))
                        {
                            session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                            return;
                        }
                        if (session.Character.Inventory.CountItem(cellaVnum) < cella[Upgrade] * reducedpricefactor)
                        {
                            session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(cellaVnum).Name, cella[Upgrade] * reducedpricefactor), 10));
                            return;
                        }
                        if (protection == UpgradeProtection.Protected && !isCommand && session.Character.Inventory.CountItem(goldScrollVnum) < 1)
                        {
                            session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(goldScrollVnum).Name, cella[Upgrade] * reducedpricefactor), 10));
                            return;
                        }
                        if (Upgrade < 5)
                        {
                            if (session.Character.Inventory.CountItem(gemVnum) < gem[Upgrade])
                            {
                                session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(gemVnum).Name, gem[Upgrade]), 10));
                                return;
                            }
                            session.Character.Inventory.RemoveItemAmount(gemVnum, gem[Upgrade]);
                        }
                        else
                        {
                            if (session.Character.Inventory.CountItem(gemFullVnum) < gem[Upgrade])
                            {
                                session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(gemFullVnum).Name, gem[Upgrade]), 10));
                                return;
                            }
                            session.Character.Inventory.RemoveItemAmount(gemFullVnum, gem[Upgrade]);
                        }
                        if (protection == UpgradeProtection.Protected && !isCommand)
                        {
                            session.Character.Inventory.RemoveItemAmount(goldScrollVnum);
                            session.SendPacket("shop_end 2");
                        }
                        session.Character.Gold -= (long)(goldprice[Upgrade] * reducedpricefactor);
                        session.Character.Inventory.RemoveItemAmount(cellaVnum, (short)(cella[Upgrade] * reducedpricefactor));
                        session.SendPacket(session.Character.GenerateGold());
                        break;

                    case UpgradeMode.Normal:
                        if (session.Character.Inventory.CountItem(cellaVnum) < cella[Upgrade])
                        {
                            return;
                        }
                        if (session.Character.Gold < goldprice[Upgrade])
                        {
                            session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("NOT_ENOUGH_MONEY"), 10));
                            return;
                        }
                        if (protection == UpgradeProtection.Protected && !isCommand && session.Character.Inventory.CountItem(normalScrollVnum) < 1)
                        {
                            session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(normalScrollVnum).Name, 1), 10));
                            return;
                        }
                        if (Upgrade < 5)
                        {
                            if (session.Character.Inventory.CountItem(gemVnum) < gem[Upgrade])
                            {
                                session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(gemVnum).Name, gem[Upgrade]), 10));
                                return;
                            }
                            session.Character.Inventory.RemoveItemAmount(gemVnum, gem[Upgrade]);
                        }
                        else
                        {
                            if (session.Character.Inventory.CountItem(gemFullVnum) < gem[Upgrade])
                            {
                                session.SendPacket(session.Character.GenerateSay(string.Format(Language.Instance.GetMessageFromKey("NOT_ENOUGH_ITEMS"), ServerManager.GetItem(gemFullVnum).Name, gem[Upgrade]), 10));
                                return;
                            }
                            session.Character.Inventory.RemoveItemAmount(gemFullVnum, gem[Upgrade]);
                        }
                        if (protection == UpgradeProtection.Protected && !isCommand)
                        {
                            session.Character.Inventory.RemoveItemAmount(normalScrollVnum);
                            session.SendPacket("shop_end 2");
                        }
                        session.Character.Inventory.RemoveItemAmount(cellaVnum, cella[Upgrade]);
                        session.Character.Gold -= goldprice[Upgrade];
                        session.SendPacket(session.Character.GenerateGold());
                        break;
                }
                ItemInstance wearable = session.Character.Inventory.GetItemInstanceById(Id);
                int rnd = ServerManager.RandomNumber();
                if (Rare == 8)
                {
                    if (rnd < upfail[Upgrade])
                    {
                        Logger.LogUserEvent("UPGRADE_ITEM", session.GenerateIdentity(), $"[UpgradeItem]ItemType: {wearable.Item.ItemType} Protection: {protection.ToString()} IIId: {Id} Upgrade: {wearable.Upgrade} Result: Fail");

                        if (protection == UpgradeProtection.None)
                        {
                            session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_FAILED"), 11));
                            session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADE_FAILED"), 0));
                            session.Character.DeleteItemByItemInstanceId(Id);
                        }
                        else
                        {
                            session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3004), session.Character.MapX, session.Character.MapY);
                            session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("SCROLL_PROTECT_USED"), 11));
                            session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADE_FAILED_ITEM_SAVED"), 0));
                        }
                    }
                    else if (rnd < upfix[Upgrade])
                    {
                        Logger.LogUserEvent("UPGRADE_ITEM", session.GenerateIdentity(), $"[UpgradeItem]ItemType: {wearable.Item.ItemType} Protection: {protection.ToString()} IIId: {Id} Upgrade: {wearable.Upgrade} Result: Fixed");

                        session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3004), session.Character.MapX, session.Character.MapY);
                        wearable.IsFixed = true;
                        session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_FIXED"), 11));
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADE_FIXED"), 0));
                    }
                    else
                    {
                        Logger.LogUserEvent("UPGRADE_ITEM", session.GenerateIdentity(), $"[UpgradeItem]ItemType: {wearable.Item.ItemType} Protection: {protection.ToString()} IIId: {Id} Upgrade: {wearable.Upgrade} Result: Success");

                        session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3009), session.Character.MapX, session.Character.MapY);
                        session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_SUCCESS"), 12));
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADE_SUCCESS"), 0));
                        wearable.Upgrade++;
                        if (wearable.Upgrade > 4)
                        {
                            session.Character.Family?.InsertFamilyLog(FamilyLogType.ItemUpgraded, session.Character.Name, itemVNum: wearable.ItemVNum, upgrade: wearable.Upgrade);
                        }
                        session.SendPacket(wearable.GenerateInventoryAdd());
                    }
                }
                else
                {
                    if (rnd < upfix[Upgrade])
                    {
                        Logger.LogUserEvent("UPGRADE_ITEM", session.GenerateIdentity(), $"[UpgradeItem]ItemType: {wearable.Item.ItemType} Protection: {protection.ToString()} IIId: {Id} Upgrade: {wearable.Upgrade} Result: Fixed");

                        session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3004), session.Character.MapX, session.Character.MapY);
                        wearable.IsFixed = true;
                        session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_FIXED"), 11));
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADE_FIXED"), 0));
                    }
                    else if (rnd < upfail[Upgrade] + upfix[Upgrade])
                    {
                        Logger.LogUserEvent("UPGRADE_ITEM", session.GenerateIdentity(), $"[UpgradeItem]ItemType: {wearable.Item.ItemType} Protection: {protection.ToString()} IIId: {Id} Upgrade: {wearable.Upgrade} Result: Fail");

                        if (protection == UpgradeProtection.None)
                        {
                            session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_FAILED"), 11));
                            session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADE_FAILED"), 0));
                            session.Character.DeleteItemByItemInstanceId(Id);
                        }
                        else
                        {
                            session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3004), session.Character.MapX, session.Character.MapY);
                            session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("SCROLL_PROTECT_USED"), 11));
                            session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADE_FAILED_ITEM_SAVED"), 0));
                        }
                    }
                    else
                    {
                        Logger.LogUserEvent("UPGRADE_ITEM", session.GenerateIdentity(), $"[UpgradeItem]ItemType: {wearable.Item.ItemType} Protection: {protection.ToString()} IIId: {Id} Upgrade: {wearable.Upgrade} Result: Success");

                        session.CurrentMapInstance.Broadcast(StaticPacketHelper.GenerateEff(UserType.Player, session.Character.CharacterId, 3009), session.Character.MapX, session.Character.MapY);
                        session.SendPacket(session.Character.GenerateSay(Language.Instance.GetMessageFromKey("UPGRADE_SUCCESS"), 12));
                        session.SendPacket(UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("UPGRADE_SUCCESS"), 0));
                        wearable.Upgrade++;
                        if (wearable.Upgrade > 4)
                        {
                            session.Character.Family?.InsertFamilyLog(FamilyLogType.ItemUpgraded, session.Character.Name, itemVNum: wearable.ItemVNum, upgrade: wearable.Upgrade);
                        }
                        session.SendPacket(wearable.GenerateInventoryAdd());
                    }
                }
                session.SendPacket("shop_end 1");
            }
        }

        #endregion
    }
}