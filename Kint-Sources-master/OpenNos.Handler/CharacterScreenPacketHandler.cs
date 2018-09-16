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
using OpenNos.Core.Handling;
using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Data.Enums;
using OpenNos.GameObject.Networking;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.Master.Library.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;

namespace OpenNos.Handler
{
    public class CharacterScreenPacketHandler : IPacketHandler
    {
        #region Instantiation

        public CharacterScreenPacketHandler(ClientSession session) => Session = session;

        #endregion

        #region Properties

        private ClientSession Session { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Char_NEW character creation character
        /// </summary>
        /// <param name="characterCreatePacket"></param>
        public void CreateCharacter(CharacterCreatePacket characterCreatePacket)
        {
            if (Session.HasCurrentMapInstance)
            {
                return;
            }

            // TODO: Hold Account Information in Authorized object
            long accountId = Session.Account.AccountId;
            Logger.LogUserEvent("CREATECHARACTER", Session.GenerateIdentity(), $"[CreateCharacter]Name: {characterCreatePacket.Name} Slot: {characterCreatePacket.Slot} Gender: {characterCreatePacket.Gender} HairStyle: {characterCreatePacket.HairStyle} HairColor: {characterCreatePacket.HairColor}");
            if (characterCreatePacket.Slot <= 3 && DAOFactory.CharacterDAO.LoadBySlot(accountId, characterCreatePacket.Slot) == null && characterCreatePacket.Name.Length > 3 && characterCreatePacket.Name.Length < 15)
            {
                Regex rg = new Regex(@"^[A-Za-z0-9_äÄöÖüÜß~*<>°+-.!_-Ð™¤£±†‡×ßø^\S]+$");
                if (rg.Matches(characterCreatePacket.Name).Count == 1)
                {
                    if (DAOFactory.CharacterDAO.LoadByName(characterCreatePacket.Name) == null)
                    {
                        if (characterCreatePacket.Slot > 3)
                        {
                            return;
                        }
                        CharacterDTO newCharacter = new CharacterDTO
                        {
                            Class = (byte)ClassType.Adventurer,
                            Gender = characterCreatePacket.Gender,
                            HairColor = characterCreatePacket.HairColor,
                            HairStyle = characterCreatePacket.HairStyle,
                            Hp = 221,
                            JobLevel = 1,
                            Level = 1,
                            MapId = 1,
                            MapX = 78,
                            MapY = 81,
                            Mp = 221,
                            MaxMateCount = 10,
                            SpPoint = 10000,
                            SpAdditionPoint = 0,
                            Name = characterCreatePacket.Name,
                            Slot = characterCreatePacket.Slot,
                            AccountId = accountId,
                            MinilandMessage = "Welcome",
                            State = CharacterState.Active,
                            MinilandPoint=2000
                        };

                        SaveResult insertResult = DAOFactory.CharacterDAO.InsertOrUpdate(ref newCharacter);
                        CharacterSkillDTO sk1 = new CharacterSkillDTO { CharacterId = newCharacter.CharacterId, SkillVNum = 200 };
                        CharacterSkillDTO sk2 = new CharacterSkillDTO { CharacterId = newCharacter.CharacterId, SkillVNum = 201 };
                        CharacterSkillDTO sk3 = new CharacterSkillDTO { CharacterId = newCharacter.CharacterId, SkillVNum = 209 };
                        QuicklistEntryDTO qlst1 = new QuicklistEntryDTO
                        {
                            CharacterId = newCharacter.CharacterId,
                            Type = 1,
                            Slot = 1,
                            Pos = 1
                        };
                        QuicklistEntryDTO qlst2 = new QuicklistEntryDTO
                        {
                            CharacterId = newCharacter.CharacterId,
                            Q2 = 1,
                            Slot = 2
                        };
                        QuicklistEntryDTO qlst3 = new QuicklistEntryDTO
                        {
                            CharacterId = newCharacter.CharacterId,
                            Q2 = 8,
                            Type = 1,
                            Slot = 1,
                            Pos = 16
                        };
                        QuicklistEntryDTO qlst4 = new QuicklistEntryDTO
                        {
                            CharacterId = newCharacter.CharacterId,
                            Q2 = 9,
                            Type = 1,
                            Slot = 3,
                            Pos = 1
                        };
                        DAOFactory.QuicklistEntryDAO.InsertOrUpdate(qlst1);
                        DAOFactory.QuicklistEntryDAO.InsertOrUpdate(qlst2);
                        DAOFactory.QuicklistEntryDAO.InsertOrUpdate(qlst3);
                        DAOFactory.QuicklistEntryDAO.InsertOrUpdate(qlst4);
                        DAOFactory.CharacterSkillDAO.InsertOrUpdate(sk1);
                        DAOFactory.CharacterSkillDAO.InsertOrUpdate(sk2);
                        DAOFactory.CharacterSkillDAO.InsertOrUpdate(sk3);

                        Inventory startupInventory = new Inventory(new Character(newCharacter));
                        startupInventory.AddNewToInventory(1, 1, InventoryType.Wear);
                        startupInventory.AddNewToInventory(8, 1, InventoryType.Wear);
                        startupInventory.AddNewToInventory(12, 1, InventoryType.Wear);
                        startupInventory.AddNewToInventory(2024, 10, InventoryType.Etc);
                        startupInventory.AddNewToInventory(2081, 1, InventoryType.Etc);
                        startupInventory.AddNewToInventory(1907, 1, InventoryType.Main);
                        startupInventory.ForEach(i => DAOFactory.IteminstanceDAO.InsertOrUpdate(i));

                        LoadCharacters(characterCreatePacket.OriginalContent);
                    }
                    else
                    {
                        Session.SendPacketFormat($"info {Language.Instance.GetMessageFromKey("ALREADY_TAKEN")}");
                    }
                }
                else
                {
                    Session.SendPacketFormat($"info {Language.Instance.GetMessageFromKey("INVALID_CHARNAME")}");
                }
            }
        }

        /// <summary>
        /// Char_DEL packet
        /// </summary>
        /// <param name="characterDeletePacket"></param>
        public void DeleteCharacter(CharacterDeletePacket characterDeletePacket)
        {
            if (Session.HasCurrentMapInstance)
            {
                return;
            }
            Logger.LogUserEvent("DELETECHARACTER", Session.GenerateIdentity(), $"[DeleteCharacter]Name: {characterDeletePacket.Slot}");
            AccountDTO account = DAOFactory.AccountDAO.LoadById(Session.Account.AccountId);
            if (account == null)
            {
                return;
            }
            if (characterDeletePacket.Password != null)
            {
                if (account.Password.ToLower() == CryptographyBase.Sha512(characterDeletePacket.Password))
                {
                    CharacterDTO character = DAOFactory.CharacterDAO.LoadBySlot(account.AccountId, characterDeletePacket.Slot);
                    if (character == null)
                    {
                        return;
                    }
                    DAOFactory.GeneralLogDAO.SetCharIdNull(Convert.ToInt64(character.CharacterId));
                    DAOFactory.CharacterDAO.DeleteByPrimaryKey(account.AccountId, characterDeletePacket.Slot);
                    LoadCharacters(string.Empty);
                }
                else
                {
                    Session.SendPacket($"info {Language.Instance.GetMessageFromKey("BAD_PASSWORD")}");
                }
            }
        }

        /// <summary>
        /// Load Characters, this is the Entrypoint for the Client, Wait for 3 Packets.
        /// </summary>
        /// <param name="packet"></param>
        [Packet(3, "OpenNos.EntryPoint")]
        public void LoadCharacters(string packet)
        {
            string[] loginPacketParts = packet.Split(' ');
            bool isCrossServerLogin = false;

            // Load account by given SessionId
            if (Session.Account == null)
            {
                bool hasRegisteredAccountLogin = true;
                AccountDTO account = null;
                if (loginPacketParts.Length > 4)
                {
                    if (loginPacketParts.Length > 7 && loginPacketParts[4] == "DAC" && loginPacketParts[8] == "CrossServerAuthenticate")
                    {
                        isCrossServerLogin = true;
                        account = DAOFactory.AccountDAO.LoadByName(loginPacketParts[5]);
                    }
                    else
                    {
                        account = DAOFactory.AccountDAO.LoadByName(loginPacketParts[4]);
                    }
                }
                try
                {
                    if (account != null)
                    {
                        if (isCrossServerLogin)
                        {
                            hasRegisteredAccountLogin = CommunicationServiceClient.Instance.IsCrossServerLoginPermitted(account.AccountId, Session.SessionId);
                        }
                        else
                        {
                            hasRegisteredAccountLogin = CommunicationServiceClient.Instance.IsLoginPermitted(account.AccountId, Session.SessionId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("MS Communication Failed.", ex);
                    Session.Disconnect();
                    return;
                }
                if (loginPacketParts.Length > 4 && hasRegisteredAccountLogin)
                {
                    if (account != null)
                    {
                        if (account.Password.ToLower().Equals(CryptographyBase.Sha512(loginPacketParts[6])) || isCrossServerLogin)
                        {
                            Account accountobject = new Account
                            {
                                AccountId = account.AccountId,
                                Name = account.Name,
                                Password = account.Password.ToLower(),
                                Authority = account.Authority                              
                            };
                            
                            Session.InitializeAccount(accountobject, isCrossServerLogin);
                        }
                        else
                        {
                            Logger.Debug($"Client {Session.ClientId} forced Disconnection, invalid Password.");
                            Session.Disconnect();
                            return;
                        }
                    }
                    else
                    {
                        Logger.Debug($"Client {Session.ClientId} forced Disconnection, invalid AccountName.");
                        Session.Disconnect();
                        return;
                    }
                }
                else
                {
                    Logger.Debug($"Client {Session.ClientId} forced Disconnection, login has not been registered or Account is already logged in.");
                    Session.Disconnect();
                    return;
                }
            }
            if (isCrossServerLogin)
            {
                if (byte.TryParse(loginPacketParts[6], out byte slot))
                {
                    SelectCharacter(new SelectPacket() { Slot = slot });
                }
            }
            else
            {
                // TODO: Wrap Database access up to GO
                IEnumerable<CharacterDTO> characters = DAOFactory.CharacterDAO.LoadAllCharactersByAccount(Session.Account.AccountId);
                Logger.Info(string.Format(Language.Instance.GetMessageFromKey("ACCOUNT_ARRIVED"), Session.SessionId));

                // load characterlist packet for each character in CharacterDTO
                Session.SendPacket("clist_start 0");
                foreach (CharacterDTO character in characters)
                {
                    IEnumerable<ItemInstanceDTO> inventory = DAOFactory.IteminstanceDAO.LoadByType(character.CharacterId, InventoryType.Wear);

                    ItemInstance[] equipment = new ItemInstance[16];
                    foreach (ItemInstanceDTO equipmentEntry in inventory)
                    {
                        // explicit load of iteminstance
                        ItemInstance currentInstance = new ItemInstance(equipmentEntry);
                        if (currentInstance != null)
                        {
                            equipment[(short)currentInstance.Item.EquipmentSlot] = currentInstance;
                        }
                    }
                    string petlist = string.Empty;
                    List<MateDTO> mates = DAOFactory.MateDAO.LoadByCharacterId(character.CharacterId).ToList();
                    for (int i = 0; i < 26; i++)
                    {
                        //0.2105.1102.319.0.632.0.333.0.318.0.317.0.9.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1.-1
                        petlist += $"{(i != 0 ? "." : string.Empty)}{(mates.Count > i ? $"{mates[i].Skin}.{mates[i].NpcMonsterVNum}" : "-1")}";
                    }

                    // 1 1 before long string of -1.-1 = act completion
                    Session.SendPacket($"clist {character.Slot} {character.Name} 0 {(byte)character.Gender} {(byte)character.HairStyle} {(byte)character.HairColor} 0 {(byte)character.Class} {character.Level} {character.HeroLevel} {character.prestigeLevel} {equipment[(byte)EquipmentType.Hat]?.ItemVNum ?? -1}.{equipment[(byte)EquipmentType.Armor]?.ItemVNum ?? -1}.{equipment[(byte)EquipmentType.WeaponSkin]?.ItemVNum ?? (equipment[(byte)EquipmentType.MainWeapon]?.ItemVNum ?? -1)}.{equipment[(byte)EquipmentType.SecondaryWeapon]?.ItemVNum ?? -1}.{equipment[(byte)EquipmentType.Mask]?.ItemVNum ?? -1}.{equipment[(byte)EquipmentType.Fairy]?.ItemVNum ?? -1}.{equipment[(byte)EquipmentType.CostumeSuit]?.ItemVNum ?? -1}.{equipment[(byte)EquipmentType.CostumeHat]?.ItemVNum ?? -1} {character.JobLevel}  1 1 {petlist} {(equipment[(byte)EquipmentType.Hat]?.Item.IsColored == true ? equipment[(byte)EquipmentType.Hat].Design : 0)} 0");
                }
                Session.SendPacket("clist_end");
            }
        }

        /// <summary>
        /// select packet
        /// </summary>
        /// <param name="selectPacket"></param>
        public void SelectCharacter(SelectPacket selectPacket)
        {
            try
            {
                Character character = new Character(DAOFactory.CharacterDAO.LoadBySlot(Session.Account.AccountId, selectPacket.Slot));
                if (Session?.Account != null && !Session.HasSelectedCharacter && character != null)
                {
                    character.Initialize();
                    if (Session.Account.Authority > AuthorityType.User)
                    {
                        character.Invisible = true;
                        character.InvisibleGm = true;
                    }
                    character.GeneralLogs = new ThreadSafeGenericList<GeneralLogDTO>();
                    character.GeneralLogs.AddRange(DAOFactory.GeneralLogDAO.LoadByAccount(Session.Account.AccountId).Where(s => s.CharacterId == character.CharacterId).ToList());
                    character.MapInstanceId = ServerManager.GetBaseMapInstanceIdByMapId(character.MapId);
                    character.PositionX = character.MapX;
                    character.PositionY = character.MapY;
                    character.Authority = Session.Account.Authority;
                    Session.SetCharacter(character);
                    if (!Session.Character.GeneralLogs.Any(s => s.Timestamp == DateTime.Now && s.LogData == "World" && s.LogType == "Connection"))
                    {
                        Session.Character.SpAdditionPoint += Session.Character.SpPoint;
                        Session.Character.SpPoint = 10000;
                    }
                    if (Session.Character.Hp > Session.Character.HPLoad())
                    {
                        Session.Character.Hp = (int)Session.Character.HPLoad();
                    }
                    if (Session.Character.Mp > Session.Character.MPLoad())
                    {
                        Session.Character.Mp = (int)Session.Character.MPLoad();
                    }
                    Session.Character.Respawns = DAOFactory.RespawnDAO.LoadByCharacter(Session.Character.CharacterId).ToList();
                    Session.Character.StaticBonusList = DAOFactory.StaticBonusDAO.LoadByCharacterId(Session.Character.CharacterId).ToList();
                    Session.Character.LoadInventory();
                    Session.Character.LoadQuicklists();
                    Session.Character.GenerateMiniland();
                    DAOFactory.MateDAO.LoadByCharacterId(Session.Character.CharacterId).ToList().ForEach(s =>
                    {
                        Mate mate = new Mate(s)
                        {
                            Owner = Session.Character
                        };
                        mate.GenerateMateTransportId();
                        mate.Monster = ServerManager.GetNpc(s.NpcMonsterVNum);
                        Session.Character.Mates.Add(mate);
                    });
                    Observable.Interval(TimeSpan.FromMilliseconds(300)).Subscribe(x => Session.Character.CharacterLife());
                    Session.Character.GeneralLogs.Add(new GeneralLogDTO { AccountId = Session.Account.AccountId, CharacterId = Session.Character.CharacterId, IpAddress = Session.IpAddress, LogData = "World", LogType = "Connection", Timestamp = DateTime.Now });
                    Session.SendPacket("OK");

                    // Inform everyone about connected character
                    CommunicationServiceClient.Instance.ConnectCharacter(ServerManager.Instance.WorldId, character.CharacterId);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Select character failed.", ex);
            }
        }

        #endregion
    }
}