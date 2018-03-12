using OpenNos.Core;
using OpenNos.DAL.EF;
using OpenNos.DAL.EF.Entities;
using OpenNos.Data;
using OpenNos.GameObject;
using System;
using System.Configuration;

namespace OpenNos.DAL
{
    public class DAOFactory
    {
        #region Members

        private static readonly bool useMock;
        private static IGenericDAO<Account, AccountDTO> accountDAO;
        private static IGenericDAO<BazaarItem, BazaarItemDTO> bazaarItemDAO;
        private static IGenericDAO<BCard, BCardDTO> bcardDAO;
        private static IGenericDAO<Card, CardDTO> cardDAO;
        private static IGenericDAO<Character, CharacterDTO> characterDAO;
        private static IGenericDAO<CharacterRelation, CharacterRelationDTO> characterRelationDAO;
        private static IGenericDAO<CharacterSkill, CharacterSkillDTO> characterskillDAO;
        private static IGenericDAO<Combo, ComboDTO> comboDAO;
        private static IGenericDAO<Drop, DropDTO> dropDAO;
        private static IGenericDAO<EquipmentOption, EquipmentOptionDTO> equipmentOptionDAO;
        private static IGenericDAO<FamilyCharacter, FamilyCharacterDTO> familycharacterDAO;
        private static IGenericDAO<Family, FamilyDTO> familyDAO;
        private static IGenericDAO<FamilyLog, FamilyLogDTO> familylogDAO;
        private static IGenericDAO<GeneralLog, GeneralLogDTO> generallogDAO;
        private static IGenericDAO<Item, ItemDTO> itemDAO;
        private static ItemInstanceDAO<ItemInstance, ItemInstanceDTO> iteminstanceDAO;
        private static IGenericDAO<LogChat, LogChatDTO> logChatDAO;
        private static IGenericDAO<LogCommands, LogCommandsDTO> logCommandsDAO;
        private static IGenericDAO<Mail, MailDTO> mailDAO;
        private static IGenericDAO<Mall, MallDTO> mallDAO;
        private static IGenericDAO<Map, MapDTO> mapDAO;
        private static IGenericDAO<MapMonster, MapMonsterDTO> mapmonsterDAO;
        private static IGenericDAO<MapNpc, MapNpcDTO> mapnpcDAO;
        private static IGenericDAO<MapType, MapTypeDTO> maptypeDAO;
        private static IGenericDAO<MapTypeMap, MapTypeMapDTO> maptypemapDAO;
        private static IGenericDAO<Mate, MateDTO> mateDAO;
        private static IGenericDAO<MinilandObject, MinilandObjectDTO> minilandobjectDAO;
        private static IGenericDAO<NpcMonster, NpcMonsterDTO> npcmonsterDAO;
        private static IGenericDAO<NpcMonsterSkill, NpcMonsterSkillDTO> npcmonsterskillDAO;
        private static IGenericDAO<PenaltyLog, PenaltyLogDTO> penaltylogDAO;
        private static IGenericDAO<Portal, PortalDTO> portalDAO;
        private static IGenericDAO<QuicklistEntry, QuicklistEntryDTO> quicklistDAO;
        private static IGenericDAO<Recipe, RecipeDTO> recipeDAO;
        private static IGenericDAO<RecipeItem, RecipeItemDTO> recipeitemDAO;
        private static IGenericDAO<Respawn, RespawnDTO> respawnDAO;
        private static IGenericDAO<RespawnMapType, RespawnMapTypeDTO> respawnMapTypeDAO;
        private static IGenericDAO<RollGeneratedItem, RollGeneratedItemDTO> rollGeneratedItemDAO;
        private static IGenericDAO<ScriptedInstance, ScriptedInstanceDTO> scriptedinstanceDAO;
        private static IGenericDAO<Shop, ShopDTO> shopDAO;
        private static IGenericDAO<ShopItem, ShopItemDTO> shopitemDAO;
        private static IGenericDAO<ShopSkill, ShopSkillDTO> shopskillDAO;
        private static IGenericDAO<Skill, SkillDTO> skillDAO;
        private static IGenericDAO<StaticBonus, StaticBonusDTO> staticBonusDAO;
        private static IGenericDAO<StaticBuff, StaticBuffDTO> staticBuffDAO;
        private static IGenericDAO<Teleporter, TeleporterDTO> teleporterDAO;

        #endregion

        #region Instantiation

        static DAOFactory()
        {
            try
            {
                useMock = Convert.ToBoolean(ConfigurationManager.AppSettings["UseMock"]);
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Database Error Server", ex);
            }
        }

        #endregion

        #region Properties

        public static IGenericDAO<Account, AccountDTO> AccountDAO
        {
            get { return accountDAO ?? (accountDAO = new GenericDAO<Account, AccountDTO>()); }
        }

        public static IGenericDAO<BazaarItem, BazaarItemDTO> BazaarItemDAO
        {
            get { return bazaarItemDAO ?? (bazaarItemDAO = new GenericDAO<BazaarItem, BazaarItemDTO>()); }
        }

        public static IGenericDAO<BCard, BCardDTO> BCardDAO
        {
            get { return bcardDAO ?? (bcardDAO = new GenericDAO<BCard, BCardDTO>()); }
        }

        public static IGenericDAO<Card, CardDTO> CardDAO
        {
            get { return cardDAO ?? (cardDAO = new GenericDAO<Card, CardDTO>()); }
        }

        public static IGenericDAO<Character, CharacterDTO> CharacterDAO
        {
            get { return characterDAO ?? (characterDAO = new GenericDAO<Character, CharacterDTO>()); }
        }

        public static IGenericDAO<CharacterRelation, CharacterRelationDTO> CharacterRelationDAO
        {
            get { return characterRelationDAO ?? (characterRelationDAO = new GenericDAO<CharacterRelation, CharacterRelationDTO>()); }
        }

        public static IGenericDAO<CharacterSkill, CharacterSkillDTO> CharacterSkillDAO
        {
            get { return characterskillDAO ?? (characterskillDAO = new GenericDAO<CharacterSkill, CharacterSkillDTO>()); }
        }

        public static IGenericDAO<Combo, ComboDTO> ComboDAO
        {
            get { return comboDAO ?? (comboDAO = new GenericDAO<Combo, ComboDTO>()); }
        }

        public static IGenericDAO<Drop, DropDTO> DropDAO
        {
            get { return dropDAO ?? (dropDAO = new GenericDAO<Drop, DropDTO>()); }
        }

        public static IGenericDAO<EquipmentOption, EquipmentOptionDTO> EquipmentOptionDAO
        {
            get { return equipmentOptionDAO ?? (equipmentOptionDAO = new GenericDAO<EquipmentOption, EquipmentOptionDTO>()); }
        }

        public static IGenericDAO<FamilyCharacter, FamilyCharacterDTO> FamilyCharacterDAO
        {
            get { return familycharacterDAO ?? (familycharacterDAO = new GenericDAO<FamilyCharacter, FamilyCharacterDTO>()); }
        }

        public static IGenericDAO<Family, FamilyDTO> FamilyDAO
        {
            get { return familyDAO ?? (familyDAO = new GenericDAO<Family, FamilyDTO>()); }
        }

        public static IGenericDAO<FamilyLog, FamilyLogDTO> FamilyLogDAO
        {
            get { return familylogDAO ?? (familylogDAO = new GenericDAO<FamilyLog, FamilyLogDTO>()); }
        }

        public static IGenericDAO<GeneralLog, GeneralLogDTO> GeneralLogDAO
        {
            get { return generallogDAO ?? (generallogDAO = new GenericDAO<GeneralLog, GeneralLogDTO>()); }
        }

        public static IGenericDAO<Item, ItemDTO> ItemDAO
        {
            get { return itemDAO ?? (itemDAO = new GenericDAO<Item, ItemDTO>()); }
        }

        public static ItemInstanceDAO<ItemInstance, ItemInstanceDTO> IteminstanceDAO
        {
            get { return iteminstanceDAO ?? (iteminstanceDAO = new ItemInstanceDAO<ItemInstance, ItemInstanceDTO>()); }
        }

        public static IGenericDAO<LogChat, LogChatDTO> LogChatDAO
        {
            get { return logChatDAO ?? (logChatDAO = new GenericDAO<LogChat, LogChatDTO>()); }
        }

        public static IGenericDAO<LogCommands, LogCommandsDTO> LogCommandsDAO
        {
            get { return logCommandsDAO ?? (logCommandsDAO = new GenericDAO<LogCommands, LogCommandsDTO>()); }
        }

        public static IGenericDAO<Mail, MailDTO> MailDAO
        {
            get { return mailDAO ?? (mailDAO = new GenericDAO<Mail, MailDTO>()); }
        }

        public static IGenericDAO<Mall, MallDTO> MallDAO
        {
            get { return mallDAO ?? (mallDAO = new GenericDAO<Mall, MallDTO>()); }
        }

        public static IGenericDAO<Map, MapDTO> MapDAO => mapDAO ?? (mapDAO = new GenericDAO<Map, MapDTO>());

        public static IGenericDAO<MapMonster, MapMonsterDTO> MapMonsterDAO
        {
            get { return mapmonsterDAO ?? (mapmonsterDAO = new GenericDAO<MapMonster, MapMonsterDTO>()); }
        }

        public static IGenericDAO<MapNpc, MapNpcDTO> MapNpcDAO
        {
            get { return mapnpcDAO ?? (mapnpcDAO = new GenericDAO<MapNpc, MapNpcDTO>()); }
        }

        public static IGenericDAO<MapType, MapTypeDTO> MapTypeDAO
        {
            get { return maptypeDAO ?? (maptypeDAO = new GenericDAO<MapType, MapTypeDTO>()); }
        }

        public static IGenericDAO<MapTypeMap, MapTypeMapDTO> MapTypeMapDAO
        {
            get { return maptypemapDAO ?? (maptypemapDAO = new GenericDAO<MapTypeMap, MapTypeMapDTO>()); }
        }

        public static IGenericDAO<Mate, MateDTO> MateDAO
        {
            get { return mateDAO ?? (mateDAO = new GenericDAO<Mate, MateDTO>()); }
        }

        public static IGenericDAO<MinilandObject, MinilandObjectDTO> MinilandObjectDAO
        {
            get { return minilandobjectDAO ?? (minilandobjectDAO = new GenericDAO<MinilandObject, MinilandObjectDTO>()); }
        }

        public static IGenericDAO<NpcMonster, NpcMonsterDTO> NpcMonsterDAO
        {
            get { return npcmonsterDAO ?? (npcmonsterDAO = new GenericDAO<NpcMonster, NpcMonsterDTO>()); }
        }

        public static IGenericDAO<NpcMonsterSkill, NpcMonsterSkillDTO> NpcMonsterSkillDAO
        {
            get { return npcmonsterskillDAO ?? (npcmonsterskillDAO = new GenericDAO<NpcMonsterSkill, NpcMonsterSkillDTO>()); }
        }

        public static IGenericDAO<PenaltyLog, PenaltyLogDTO> PenaltyLogDAO
        {
            get { return penaltylogDAO ?? (penaltylogDAO = new GenericDAO<PenaltyLog, PenaltyLogDTO>()); }
        }

        public static IGenericDAO<Portal, PortalDTO> PortalDAO
        {
            get { return portalDAO ?? (portalDAO = new GenericDAO<Portal, PortalDTO>()); }
        }

        public static IGenericDAO<QuicklistEntry, QuicklistEntryDTO> QuicklistEntryDAO
        {
            get { return quicklistDAO ?? (quicklistDAO = new GenericDAO<QuicklistEntry, QuicklistEntryDTO>()); }
        }

        public static IGenericDAO<Recipe, RecipeDTO> RecipeDAO
        {
            get { return recipeDAO ?? (recipeDAO = new GenericDAO<Recipe, RecipeDTO>()); }
        }

        public static IGenericDAO<RecipeItem, RecipeItemDTO> RecipeItemDAO
        {
            get { return recipeitemDAO ?? (recipeitemDAO = new GenericDAO<RecipeItem, RecipeItemDTO>()); }
        }

        public static IGenericDAO<Respawn, RespawnDTO> RespawnDAO
        {
            get { return respawnDAO ?? (respawnDAO = new GenericDAO<Respawn, RespawnDTO>()); }
        }

        public static IGenericDAO<RespawnMapType, RespawnMapTypeDTO> RespawnMapTypeDAO
        {
            get { return respawnMapTypeDAO ?? (respawnMapTypeDAO = new GenericDAO<RespawnMapType, RespawnMapTypeDTO>()); }
        }

        public static IGenericDAO<RollGeneratedItem, RollGeneratedItemDTO> RollGeneratedItemDAO
        {
            get { return rollGeneratedItemDAO ?? (rollGeneratedItemDAO = new GenericDAO<RollGeneratedItem, RollGeneratedItemDTO>()); }
        }

        public static IGenericDAO<ScriptedInstance, ScriptedInstanceDTO> ScriptedInstanceDAO
        {
            get { return scriptedinstanceDAO ?? (scriptedinstanceDAO = new GenericDAO<ScriptedInstance, ScriptedInstanceDTO>()); }
        }

        public static IGenericDAO<Shop, ShopDTO> ShopDAO
        {
            get { return shopDAO ?? (shopDAO = new GenericDAO<Shop, ShopDTO>()); }
        }

        public static IGenericDAO<ShopItem, ShopItemDTO> ShopItemDAO
        {
            get { return shopitemDAO ?? (shopitemDAO = new GenericDAO<ShopItem, ShopItemDTO>()); }
        }

        public static IGenericDAO<ShopSkill, ShopSkillDTO> ShopSkillDAO
        {
            get { return shopskillDAO ?? (shopskillDAO = new GenericDAO<ShopSkill, ShopSkillDTO>()); }
        }

        public static IGenericDAO<Skill, SkillDTO> SkillDAO
        {
            get { return skillDAO ?? (skillDAO = new GenericDAO<Skill, SkillDTO>()); }
        }

        public static IGenericDAO<StaticBonus, StaticBonusDTO> StaticBonusDAO
        {
            get { return staticBonusDAO ?? (staticBonusDAO = new GenericDAO<StaticBonus, StaticBonusDTO>()); }
        }

        public static IGenericDAO<StaticBuff, StaticBuffDTO> StaticBuffDAO
        {
            get { return staticBuffDAO ?? (staticBuffDAO = new GenericDAO<StaticBuff, StaticBuffDTO>()); }
        }

        public static IGenericDAO<Teleporter, TeleporterDTO> TeleporterDAO
        {
            get { return teleporterDAO ?? (teleporterDAO = new GenericDAO<Teleporter, TeleporterDTO>()); }
        }

        #endregion
    }
}