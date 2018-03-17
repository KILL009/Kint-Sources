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
using OpenNos.DAL.EF;
using OpenNos.DAL.Interface;
using OpenNos.Data;
using System;
using System.Configuration;

namespace OpenNos.DAL
{
    public static class DAOFactory
    {
        #region Members

        private static readonly bool _useMock;

        private static IAccountDAO _accountDAO;
        private static IBazaarItemDAO _bazaarItemDAO;
        private static IBCardDAO _bcardDAO;
        private static ICardDAO _cardDAO;
        private static ICellonOptionDAO _cellonoptionDAO;
        private static ICharacterDAO _characterDAO;
        private static ICharacterRelationDAO _characterRelationDAO;
        private static ICharacterSkillDAO _characterskillDAO;
        private static IComboDAO _comboDAO;
        private static IDropDAO _dropDAO;
        private static IFamilyCharacterDAO _familycharacterDAO;
        private static IFamilyDAO _familyDAO;
        private static IFamilyLogDAO _familylogDAO;
        private static IGeneralLogDAO _generallogDAO;
        
        private static IItemDAO _itemDAO;
        private static IItemInstanceDAO _iteminstanceDAO;
        private static IMailDAO _mailDAO;
        private static IMaintenanceLogDAO _maintenanceLogDAO;
        private static IMapDAO _mapDAO;
        private static IMapMonsterDAO _mapmonsterDAO;
        private static IMapNpcDAO _mapnpcDAO;
        private static IMapTypeDAO _maptypeDAO;
        private static IMapTypeMapDAO _maptypemapDAO;
        private static IMateDAO _mateDAO;
        private static IMinigameLogDAO _minigameLogDAO;
        private static IMinilandObjectDAO _minilandobjectDAO;
        private static INpcMonsterDAO _npcmonsterDAO;
        private static INpcMonsterSkillDAO _npcmonsterskillDAO;
        private static IPenaltyLogDAO _penaltylogDAO;
        private static IPortalDAO _portalDAO;
        private static IQuestDAO _questDAO;
        private static IQuestProgressDAO _questProgressDAO;
        private static IQuicklistEntryDAO _quicklistDAO;
        private static IRecipeDAO _recipeDAO;
        private static IRecipeItemDAO _recipeitemDAO;
        private static IRecipeListDAO _recipeListDAO;
        private static IRespawnDAO _respawnDAO;
        private static IRespawnMapTypeDAO _respawnMapTypeDAO;
        private static IRollGeneratedItemDAO _rollGeneratedItemDAO;
        private static IScriptedInstanceDAO _scriptedinstanceDAO;
        private static IShellEffectDAO _shelleffectDAO;
        private static IShopDAO _shopDAO;
        private static IShopItemDAO _shopitemDAO;
        private static IShopSkillDAO _shopskillDAO;
        private static ISkillDAO _skillDAO;
        private static IStaticBonusDAO _staticBonusDAO;
        private static IStaticBuffDAO _staticBuffDAO;
        private static ITeleporterDAO _teleporterDAO;

        #endregion

        #region Instantiation

        static DAOFactory()
        {
            try
            {
                _useMock = Convert.ToBoolean(ConfigurationManager.AppSettings["UseMock"]);
            }
            catch (Exception ex)
            {
                Logger.Error("Database Error Server", ex);
            }
        }

        #endregion

        #region Properties

        public static IAccountDAO AccountDAO
        {
            get
            {
                if (_accountDAO == null)
                {
                    if (_useMock)
                    {
                        _accountDAO = new Mock.AccountDAO();
                    }
                    else
                    {
                        _accountDAO = new DAO.AccountDAO();
                    }
                }

                return _accountDAO;
            }
        }

        

        public static IBazaarItemDAO BazaarItemDAO
        {
            get
            {
                if (_bazaarItemDAO == null)
                {
                    if (_useMock)
                    {
                        _bazaarItemDAO = new Mock.BazaarItemDAO();
                    }
                    else
                    {
                        _bazaarItemDAO = new DAO.BazaarItemDAO();
                    }
                }

                return _bazaarItemDAO;
            }
        }

        public static IBCardDAO BCardDAO
        {
            get
            {
                if (_bcardDAO == null)
                {
                    if (_useMock)
                    {
                        _bcardDAO = new Mock.BCardDAO();
                    }
                    else
                    {
                        _bcardDAO = new DAO.BCardDAO();
                    }
                }

                return _bcardDAO;
            }
        }

        public static ICardDAO CardDAO
        {
            get
            {
                if (_cardDAO == null)
                {
                    if (_useMock)
                    {
                        _cardDAO = new Mock.CardDAO();
                    }
                    else
                    {
                        _cardDAO = new DAO.CardDAO();
                    }
                }

                return _cardDAO;
            }
        }

        public static ICellonOptionDAO CellonOptionDAO
        {
            get
            {
                if (_cellonoptionDAO == null)
                {
                    if (_useMock)
                    {
                        _cellonoptionDAO = new Mock.CellonOptionDAO();
                    }
                    else
                    {
                        _cellonoptionDAO = new DAO.CellonOptionDAO();
                    }
                }

                return _cellonoptionDAO;
            }
        }

        public static ICharacterDAO CharacterDAO
        {
            get
            {
                if (_characterDAO == null)
                {
                    if (_useMock)
                    {
                        _characterDAO = new Mock.CharacterDAO();
                    }
                    else
                    {
                        _characterDAO = new DAO.CharacterDAO();
                    }
                }

                return _characterDAO;
            }
        }

        public static ICharacterRelationDAO CharacterRelationDAO
        {
            get
            {
                if (_characterRelationDAO == null)
                {
                    if (_useMock)
                    {
                        _characterRelationDAO = new Mock.CharacterRelationDAO();
                    }
                    else
                    {
                        _characterRelationDAO = new DAO.CharacterRelationDAO();
                    }
                }

                return _characterRelationDAO;
            }
        }


        public static ICharacterSkillDAO CharacterSkillDAO
        {
            get
            {
                if (_characterskillDAO == null)
                {
                    if (_useMock)
                    {
                        _characterskillDAO = new Mock.CharacterSkillDAO();
                    }
                    else
                    {
                        _characterskillDAO = new DAO.CharacterSkillDAO();
                    }
                }

                return _characterskillDAO;
            }
        }

        public static IComboDAO ComboDAO
        {
            get
            {
                if (_comboDAO == null)
                {
                    if (_useMock)
                    {
                        _comboDAO = new Mock.ComboDAO();
                    }
                    else
                    {
                        _comboDAO = new DAO.ComboDAO();
                    }
                }

                return _comboDAO;
            }
        }

        public static IDropDAO DropDAO
        {
            get
            {
                if (_dropDAO == null)
                {
                    if (_useMock)
                    {
                        _dropDAO = new Mock.DropDAO();
                    }
                    else
                    {
                        _dropDAO = new DAO.DropDAO();
                    }
                }

                return _dropDAO;
            }
        }

        public static IFamilyCharacterDAO FamilyCharacterDAO
        {
            get
            {
                if (_familycharacterDAO == null)
                {
                    if (_useMock)
                    {
                        _familycharacterDAO = new Mock.FamilyCharacterDAO();
                    }
                    else
                    {
                        _familycharacterDAO = new DAO.FamilyCharacterDAO();
                    }
                }

                return _familycharacterDAO;
            }
        }

        public static IFamilyDAO FamilyDAO
        {
            get
            {
                if (_familyDAO == null)
                {
                    if (_useMock)
                    {
                        _familyDAO = new Mock.FamilyDAO();
                    }
                    else
                    {
                        _familyDAO = new DAO.FamilyDAO();
                    }
                }

                return _familyDAO;
            }
        }

        public static IFamilyLogDAO FamilyLogDAO
        {
            get
            {
                if (_familylogDAO == null)
                {
                    if (_useMock)
                    {
                        _familylogDAO = new Mock.FamilyLogDAO();
                    }
                    else
                    {
                        _familylogDAO = new DAO.FamilyLogDAO();
                    }
                }

                return _familylogDAO;
            }
        }

        public static IGeneralLogDAO GeneralLogDAO
        {
            get
            {
                if (_generallogDAO == null)
                {
                    if (_useMock)
                    {
                        _generallogDAO = new Mock.GeneralLogDAO();
                    }
                    else
                    {
                        _generallogDAO = new DAO.GeneralLogDAO();
                    }
                }

                return _generallogDAO;
            }
        }

        public static IItemDAO ItemDAO
        {
            get
            {
                if (_itemDAO == null)
                {
                    if (_useMock)
                    {
                        _itemDAO = new Mock.ItemDAO();
                    }
                    else
                    {
                        _itemDAO = new DAO.ItemDAO();
                    }
                }

                return _itemDAO;
            }
        }

        public static IItemInstanceDAO IteminstanceDAO
        {
            get
            {
                if (_iteminstanceDAO == null)
                {
                    if (_useMock)
                    {
                        _iteminstanceDAO = new Mock.ItemInstanceDAO();
                    }
                    else
                    {
                        _iteminstanceDAO = new DAO.ItemInstanceDAO();
                    }
                }

                return _iteminstanceDAO;
            }
        }

        public static IMailDAO MailDAO
        {
            get
            {
                if (_mailDAO == null)
                {
                    if (_useMock)
                    {
                        _mailDAO = new Mock.MailDAO();
                    }
                    else
                    {
                        _mailDAO = new DAO.MailDAO();
                    }
                }

                return _mailDAO;
            }
        }

        public static IMaintenanceLogDAO MaintenanceLogDAO
        {
            get
            {
                if (_maintenanceLogDAO == null)
                {
                    if (_useMock)
                    {
                        _maintenanceLogDAO = new Mock.MaintenanceLogDAO();
                    }
                    else
                    {
                        _maintenanceLogDAO = new DAO.MaintenanceLogDAO();
                    }
                }

                return _maintenanceLogDAO;
            }
        }

        public static IMapDAO MapDAO
        {
            get
            {
                if (_mapDAO == null)
                {
                    if (_useMock)
                    {
                        _mapDAO = new Mock.MapDAO();
                    }
                    else
                    {
                        _mapDAO = new DAO.MapDAO();
                    }
                }

                return _mapDAO;
            }
        }

        public static IMapMonsterDAO MapMonsterDAO
        {
            get
            {
                if (_mapmonsterDAO == null)
                {
                    if (_useMock)
                    {
                        _mapmonsterDAO = new Mock.MapMonsterDAO();
                    }
                    else
                    {
                        _mapmonsterDAO = new DAO.MapMonsterDAO();
                    }
                }

                return _mapmonsterDAO;
            }
        }

        public static IMapNpcDAO MapNpcDAO
        {
            get
            {
                if (_mapnpcDAO == null)
                {
                    if (_useMock)
                    {
                        _mapnpcDAO = new Mock.MapNpcDAO();
                    }
                    else
                    {
                        _mapnpcDAO = new DAO.MapNpcDAO();
                    }
                }

                return _mapnpcDAO;
            }
        }

        public static IMapTypeDAO MapTypeDAO
        {
            get
            {
                if (_maptypeDAO == null)
                {
                    if (_useMock)
                    {
                        _maptypeDAO = new Mock.MapTypeDAO();
                    }
                    else
                    {
                        _maptypeDAO = new DAO.MapTypeDAO();
                    }
                }

                return _maptypeDAO;
            }
        }

        public static IMapTypeMapDAO MapTypeMapDAO
        {
            get
            {
                if (_maptypemapDAO == null)
                {
                    if (_useMock)
                    {
                        _maptypemapDAO = new Mock.MapTypeMapDAO();
                    }
                    else
                    {
                        _maptypemapDAO = new DAO.MapTypeMapDAO();
                    }
                }

                return _maptypemapDAO;
            }
        }

        public static IMateDAO MateDAO
        {
            get
            {
                if (_mateDAO == null)
                {
                    if (_useMock)
                    {
                        _mateDAO = new Mock.MateDAO();
                    }
                    else
                    {
                        _mateDAO = new DAO.MateDAO();
                    }
                }

                return _mateDAO;
            }
        }

        public static IMinigameLogDAO MinigameLogDAO
        {
            get
            {
                if (_minigameLogDAO == null)
                {
                    if (_useMock)
                    {
                       // _minigameLogDAO = new Mock.MinigameLogDAO();
                    }
                    else
                    {
                        _minigameLogDAO = new DAO.MinigameLogDAO();
                    }
                }

                return _minigameLogDAO;
            }
        }

        public static IMinilandObjectDAO MinilandObjectDAO
        {
            get
            {
                if (_minilandobjectDAO == null)
                {
                    if (_useMock)
                    {
                        _minilandobjectDAO = new Mock.MinilandObjectDAO();
                    }
                    else
                    {
                        _minilandobjectDAO = new DAO.MinilandObjectDAO();
                    }
                }

                return _minilandobjectDAO;
            }
        }

        public static INpcMonsterDAO NpcMonsterDAO
        {
            get
            {
                if (_npcmonsterDAO == null)
                {
                    if (_useMock)
                    {
                        _npcmonsterDAO = new Mock.NpcMonsterDAO();
                    }
                    else
                    {
                        _npcmonsterDAO = new DAO.NpcMonsterDAO();
                    }
                }

                return _npcmonsterDAO;
            }
        }

        public static INpcMonsterSkillDAO NpcMonsterSkillDAO
        {
            get
            {
                if (_npcmonsterskillDAO == null)
                {
                    if (_useMock)
                    {
                        _npcmonsterskillDAO = new Mock.NpcMonsterSkillDAO();
                    }
                    else
                    {
                        _npcmonsterskillDAO = new DAO.NpcMonsterSkillDAO();
                    }
                }

                return _npcmonsterskillDAO;
            }
        }

        public static IPenaltyLogDAO PenaltyLogDAO
        {
            get
            {
                if (_penaltylogDAO == null)
                {
                    if (_useMock)
                    {
                        _penaltylogDAO = new Mock.PenaltyLogDAO();
                    }
                    else
                    {
                        _penaltylogDAO = new DAO.PenaltyLogDAO();
                    }
                }

                return _penaltylogDAO;
            }
        }

        public static IPortalDAO PortalDAO
        {
            get
            {
                if (_portalDAO == null)
                {
                    if (_useMock)
                    {
                        _portalDAO = new Mock.PortalDAO();
                    }
                    else
                    {
                        _portalDAO = new DAO.PortalDAO();
                    }
                }

                return _portalDAO;
            }
        }

        public static IQuestDAO QuestDAO
        {
            get
            {
                if (_questDAO == null)
                {
                    if (_useMock)
                    {
                        _questDAO = new Mock.QuestDAO();
                    }
                    else
                    {
                        _questDAO = new DAO.QuestDAO();
                    }
                }

                return _questDAO;
            }
        }

        public static IQuestProgressDAO QuestProgressDAO
        {
            get
            {
                if (_questProgressDAO == null)
                {
                    if (_useMock)
                    {
                        _questProgressDAO = new Mock.QuestProgressDAO();
                    }
                    else
                    {
                        _questProgressDAO = new DAO.QuestProgressDAO();
                    }
                }

                return _questProgressDAO;
            }
        }

        public static IQuicklistEntryDAO QuicklistEntryDAO
        {
            get
            {
                if (_quicklistDAO == null)
                {
                    if (_useMock)
                    {
                        _quicklistDAO = new Mock.QuicklistEntryDAO();
                    }
                    else
                    {
                        _quicklistDAO = new DAO.QuicklistEntryDAO();
                    }
                }

                return _quicklistDAO;
            }
        }

        public static IRecipeDAO RecipeDAO
        {
            get
            {
                if (_recipeDAO == null)
                {
                    if (_useMock)
                    {
                        _recipeDAO = new Mock.RecipeDAO();
                    }
                    else
                    {
                        _recipeDAO = new DAO.RecipeDAO();
                    }
                }

                return _recipeDAO;
            }
        }

        public static IRecipeItemDAO RecipeItemDAO
        {
            get
            {
                if (_recipeitemDAO == null)
                {
                    if (_useMock)
                    {
                        _recipeitemDAO = new Mock.RecipeItemDAO();
                    }
                    else
                    {
                        _recipeitemDAO = new DAO.RecipeItemDAO();
                    }
                }

                return _recipeitemDAO;
            }
        }

        public static IRecipeListDAO RecipeListDAO
        {
            get
            {
                if (_recipeListDAO == null)
                {
                    if (_useMock)
                    {
                        _recipeListDAO = new Mock.RecipeListDAO();
                    }
                    else
                    {
                        _recipeListDAO = new DAO.RecipeListDAO();
                    }
                }

                return _recipeListDAO;
            }
        }

        public static IRespawnDAO RespawnDAO
        {
            get
            {
                if (_respawnDAO == null)
                {
                    if (_useMock)
                    {
                        _respawnDAO = new Mock.RespawnDAO();
                    }
                    else
                    {
                        _respawnDAO = new DAO.RespawnDAO();
                    }
                }

                return _respawnDAO;
            }
        }

        public static IRespawnMapTypeDAO RespawnMapTypeDAO
        {
            get
            {
                if (_respawnMapTypeDAO == null)
                {
                    if (_useMock)
                    {
                        _respawnMapTypeDAO = new Mock.RespawnMapTypeDAO();
                    }
                    else
                    {
                        _respawnMapTypeDAO = new DAO.RespawnMapTypeDAO();
                    }
                }

                return _respawnMapTypeDAO;
            }
        }

        public static IRollGeneratedItemDAO RollGeneratedItemDAO
        {
            get
            {
                if (_rollGeneratedItemDAO == null)
                {
                    if (_useMock)
                    {
                        _rollGeneratedItemDAO = new Mock.RollGeneratedItemDAO();
                    }
                    else
                    {
                        _rollGeneratedItemDAO = new DAO.RollGeneratedItemDAO();
                    }
                }

                return _rollGeneratedItemDAO;
            }
        }

        public static IScriptedInstanceDAO ScriptedInstanceDAO
        {
            get
            {
                if (_scriptedinstanceDAO == null)
                {
                    if (_useMock)
                    {
                        _scriptedinstanceDAO = new Mock.ScriptedInstanceDAO();
                    }
                    else
                    {
                        _scriptedinstanceDAO = new DAO.ScriptedInstanceDAO();
                    }
                }

                return _scriptedinstanceDAO;
            }
        }

        public static IShellEffectDAO ShellEffectDAO
        {
            get
            {
                if (_shelleffectDAO == null)
                {
                    if (_useMock)
                    {
                        _shelleffectDAO = new Mock.ShellEffectDAO();
                    }
                    else
                    {
                        _shelleffectDAO = new DAO.ShellEffectDAO();
                    }
                }

                return _shelleffectDAO;
            }
        }

        public static IShopDAO ShopDAO
        {
            get
            {
                if (_shopDAO == null)
                {
                    if (_useMock)
                    {
                        _shopDAO = new Mock.ShopDAO();
                    }
                    else
                    {
                        _shopDAO = new DAO.ShopDAO();
                    }
                }

                return _shopDAO;
            }
        }

        public static IShopItemDAO ShopItemDAO
        {
            get
            {
                if (_shopitemDAO == null)
                {
                    if (_useMock)
                    {
                        _shopitemDAO = new Mock.ShopItemDAO();
                    }
                    else
                    {
                        _shopitemDAO = new DAO.ShopItemDAO();
                    }
                }

                return _shopitemDAO;
            }
        }

        public static IShopSkillDAO ShopSkillDAO
        {
            get
            {
                if (_shopskillDAO == null)
                {
                    if (_useMock)
                    {
                        _shopskillDAO = new Mock.ShopSkillDAO();
                    }
                    else
                    {
                        _shopskillDAO = new DAO.ShopSkillDAO();
                    }
                }

                return _shopskillDAO;
            }
        }

        public static ISkillDAO SkillDAO
        {
            get
            {
                if (_skillDAO == null)
                {
                    if (_useMock)
                    {
                        _skillDAO = new Mock.SkillDAO();
                    }
                    else
                    {
                        _skillDAO = new DAO.SkillDAO();
                    }
                }

                return _skillDAO;
            }
        }

        public static IStaticBonusDAO StaticBonusDAO
        {
            get
            {
                if (_staticBonusDAO == null)
                {
                    if (_useMock)
                    {
                        _staticBonusDAO = new Mock.StaticBonusDAO();
                    }
                    else
                    {
                        _staticBonusDAO = new DAO.StaticBonusDAO();
                    }
                }

                return _staticBonusDAO;
            }
        }

        public static IStaticBuffDAO StaticBuffDAO
        {
            get
            {
                if (_staticBuffDAO == null)
                {
                    if (_useMock)
                    {
                        _staticBuffDAO = new Mock.StaticBuffDAO();
                    }
                    else
                    {
                        _staticBuffDAO = new DAO.StaticBuffDAO();
                    }
                }

                return _staticBuffDAO;
            }
        }

        public static ITeleporterDAO TeleporterDAO
        {
            get
            {
                if (_teleporterDAO == null)
                {
                    if (_useMock)
                    {
                        _teleporterDAO = new Mock.TeleporterDAO();
                    }
                    else
                    {
                        _teleporterDAO = new DAO.TeleporterDAO();
                    }
                }

                return _teleporterDAO;
            }
        }

        #endregion
    }
}