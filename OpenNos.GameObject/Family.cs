using OpenNos.DAL;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.Master.Library.Client;
using OpenNos.Master.Library.Data;
using System;
using System.Collections.Generic;

namespace OpenNos.GameObject
{
    public class Family : FamilyDTO
    {
        #region Instantiation

        public Family()
        {
            FamilyCharacters = new List<FamilyCharacter>();
        }

        #endregion

        #region Properties

        public MapInstance Act4Raid { get; set; }

        public MapInstance Act4RaidBossMap { get; set; }

        public Act4RaidType Act4RaidType { get; set; }

        public List<FamilyCharacter> FamilyCharacters { get; set; }

        public List<FamilyLogDTO> FamilyLogs { get; set; }

        public MapInstance LandOfDeath { get; set; }

        public Inventory Warehouse { get; set; }

        #endregion

        #region Methods

        public override void Initialize()
        {
            // do nothing
        }

        public static Family FromDTO(FamilyDTO familyDTO)
        {
            Family family = new Family();
            family.FamilyExperience = familyDTO.FamilyExperience;
            family.FamilyHeadGender = familyDTO.FamilyHeadGender;
            family.FamilyId = familyDTO.FamilyId;
            family.FamilyLevel = familyDTO.FamilyLevel;
            family.FamilyMessage = familyDTO.FamilyMessage;
            family.ManagerAuthorityType = familyDTO.ManagerAuthorityType;
            family.ManagerCanGetHistory = familyDTO.ManagerCanGetHistory;
            family.ManagerCanInvite = familyDTO.ManagerCanInvite;
            family.ManagerCanNotice = familyDTO.ManagerCanNotice;
            family.ManagerCanShout = familyDTO.ManagerCanShout;
            family.MaxSize = familyDTO.MaxSize;
            family.MemberAuthorityType = familyDTO.MemberAuthorityType;
            family.MemberCanGetHistory = familyDTO.MemberCanGetHistory;
            family.Name = familyDTO.Name;
            family.WarehouseSize = familyDTO.WarehouseSize;
            return family;
        }

        public void InsertFamilyLog(FamilyLogType logtype, string characterName = "", string characterName2 = "", string rainBowFamily = "", string message = "", byte level = 0, int experience = 0, int itemVNum = 0, byte upgrade = 0, int raidType = 0, FamilyAuthority authority = FamilyAuthority.Head, int righttype = 0, int rightvalue = 0)
        {
            var value = string.Empty;
            switch (logtype)
            {
                case FamilyLogType.DailyMessage:
                    value = $"{characterName}|{message}";
                    break;

                case FamilyLogType.FamilyXP:
                    value = $"{characterName}|{experience}";
                    break;

                case FamilyLogType.LevelUp:
                    value = $"{characterName}|{level}";
                    break;

                case FamilyLogType.RaidWon:
                    value = raidType.ToString();
                    break;

                case FamilyLogType.ItemUpgraded:
                    value = $"{characterName}|{itemVNum}|{upgrade}";
                    break;

                case FamilyLogType.UserManaged:
                    value = $"{characterName}|{characterName2}";
                    break;

                case FamilyLogType.FamilyLevelUp:
                    value = level.ToString();
                    break;

                case FamilyLogType.AuthorityChanged:
                    value = $"{characterName}|{(byte)authority}|{characterName2}";
                    break;

                case FamilyLogType.FamilyManaged:
                    value = characterName;
                    break;

                case FamilyLogType.RainbowBattle:
                    value = rainBowFamily;
                    break;

                case FamilyLogType.RightChanged:
                    value = $"{characterName}|{(byte)authority}|{righttype}|{rightvalue}";
                    break;

                case FamilyLogType.WareHouseAdded:
                case FamilyLogType.WareHouseRemoved:
                    value = $"{characterName}|{message}";
                    break;
            }

            var log = new FamilyLogDTO
            {
                FamilyId = FamilyId,
                FamilyLogData = value,
                FamilyLogType = logtype,
                Timestamp = DateTime.Now
            };
            DAOFactory.FamilyLogDAO.InsertOrUpdate(ref log);
            ServerManager.Instance.FamilyRefresh(FamilyId);
            CommunicationServiceClient.Instance.SendMessageToCharacter(new SCSCharacterMessage()
            {
                DestinationCharacterId = FamilyId,
                SourceCharacterId = 0,
                SourceWorldId = ServerManager.Instance.WorldId,
                Message = "fhis_stc",
                Type = MessageType.Family
            });
        }

        internal Family DeepCopy()
        {
            var clonedCharacter = (Family)MemberwiseClone();
            return clonedCharacter;
        }

        #endregion
    }
}