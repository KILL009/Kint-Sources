using OpenNos.SCS.Communication.ScsServices.Service;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.Master.Library.Data;
using System;
using System.Collections.Generic;

namespace OpenNos.Master.Library.Interface
{
    [ScsService(Version = "1.1.0.0")]
    public interface IAdminToolService
    {
        /// <summary>
        /// Authenticates the AdminTool Client to the Service
        /// </summary>
        /// <param name="username">AccountName of the User</param>
        /// <param name="password">SHA512 Hash of the Users password</param>
        /// <returns>AuthorityType as byte, 0 if access forbidden or wrong credentials</returns>
        byte Authenticate(string username, string password);

        /// <summary>
        /// Adds a penalty to the specified Account and refreshes the PenaltyLog
        /// </summary>
        /// <param name="accountId">Id of the Account</param>
        /// <param name="penaltyType">Type of the Penalty</param>
        /// <param name="dateEnd">Date when the penalty should end</param>
        /// <param name="reason">Reason for the penalty</param>
        void AddPenalty(long accountId, PenaltyType penaltyType, DateTime dateEnd, string reason);

        /// <summary>
        /// Teleports a Character to another one on the same Channel
        /// </summary>
        /// <param name="sourceCharacter">Id of the Character to teleport</param>
        /// <param name="destCharacter">Id of the Destination Character</param>
        void TeleportToPlayer(long sourceCharacter, long destCharacter);

        /// <summary>
        /// Teleports a player to a specified position
        /// </summary>
        /// <param name="sourceCharacter">Id of the Character to teleport</param>
        /// <param name="mapId">Destination MapId</param>
        /// <param name="mapX">Destination X Coordinate</param>
        /// <param name="mapY">Destination Y Coordinate</param>
        void Teleport(long sourceCharacter, short mapId, short mapX, short mapY);

        /// <summary>
        /// Kicks the given Account out of the Game
        /// </summary>
        /// <param name="accountId">Id of the Account</param>
        void Kick(long accountId);

        /// <summary>
        /// Gets informations about the given Character Name
        /// </summary>
        /// <param name="characterName">Name of the Character</param>
        /// <returns>The resulting CharacterDTO object for the Character</returns>
        CharacterDTO GetCharacterDetails(string characterName);

        /// <summary>
        /// Gets informations about the given Account Name
        /// </summary>
        /// <param name="accountName">Name of the Account</param>
        /// <returns>The resulting AccountDTO object for the Account</returns>
        AccountDTO GetAccountDetails(string accountName);

        /// <summary>
        /// Gets all Penalties for the given Account
        /// </summary>
        /// <param name="accountId">Id of the Account</param>
        /// <returns>A list of all PenaltyLogDTO entries</returns>
        List<PenaltyLogDTO> GetPenalties(long accountId);

        /// <summary>
        /// Gets all Characters(existing and deleted) for the given Account
        /// </summary>
        /// <param name="accountId">Id of the Account</param>
        /// <returns>A List of all CharacterDTO objects ever related to the given Account</returns>
        List<CharacterDTO> GetAllCharacters(long accountId);

        /// <summary>
        /// Gets a list of Items the Character has in Bazaar right now
        /// </summary>
        /// <param name="characterId">Id of the Character</param>
        /// <returns>A List of all BazaarItemDTO objects</returns>
        List<BazaarItemDTO> GetAllBazaarItems(long characterId);

        /// <summary>
        /// Gets a list of all Items the Character has in the respective Inventory
        /// </summary>
        /// <param name="characterId">Id of the Character</param>
        /// <param name="type">Type of the Inventory</param>
        /// <returns>A List of all ItemInstanceDTO objects</returns>
        List<ItemInstanceDTO> GetAllInventoryItems(long characterId, InventoryType type);

        /// <summary>
        /// Forces the respective Session to save all data(for accessing it
        /// </summary>
        /// <param name="characterId">Id of the Character</param>
        void SaveCharacter(long characterId);

        /// <summary>
        /// Sends a Admin Message to the given Channel
        /// </summary>
        /// <param name="destChannelId">Id of the Channel, null for all</param>
        /// <param name="message">Message to send</param>
        void Shout(Guid destChannelId, string message);

        /// <summary>
        /// Shuts down the given Channel
        /// </summary>
        /// <param name="channelId">Id of the Channel, null for all</param>
        void Shutdown(Guid channelId);

        /// <summary>
        /// Send an Item using the Mail Service
        /// </summary>
        /// <param name="senderName">Name that should be shown as Sender</param>
        /// <param name="receivingCharacterId">CharacterId of the Receiver</param>
        /// <param name="vnum">VNum of the Item</param>
        /// <param name="amount">Amount of the Item</param>
        /// <param name="rare">Rare of the Item</param>
        /// <param name="Upgrade">Upgrade of the Item</param>
        /// <param name="isNosmall"></param>
        void SendItem(string senderName, long receivingCharacterId, short vnum, byte amount = 1, byte rare = 0, byte Upgrade = 0, bool isNosmall = false);

        /// <summary>
        /// Send specific Packet to Player
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="characterId"></param>
        void SendPacket(string packet, long characterId);

        /// <summary>
        /// Send specific Packet to Map
        /// </summary>
        /// <param name="channelId">Id of the Channel, null for all</param>
        /// <param name="packet"></param>
        /// <param name="mapInstanceId"></param>
        void SendPacketToMap(Guid channelId, string packet, Guid mapInstanceId);

        /// <summary>
        /// Send an Item using the Mail Service to the whole Map 
        /// </summary>
        /// <param name="channelId">Id of the Channel, null for all</param>
        /// <param name="senderName">Name that should be shown as Sender</param>
        /// <param name="mapInstanceId">Id of the MapInstance to send the Item to</param>
        /// <param name="vnum">VNum of the Item</param>
        /// <param name="amount">Amount of the Item</param>
        /// <param name="rare">Rare of the Item</param>
        /// <param name="Upgrade">Upgrade of the Item</param>
        /// <param name="isNosmall"></param>
        void SendItemToMap(Guid channelId, string senderName, Guid mapInstanceId, short vnum, byte amount = 1, byte rare = 0, byte Upgrade = 0, bool isNosmall = false);

        /// <summary>
        /// Summons a NPCMonster on the given Map
        /// </summary>
        /// <param name="channelId">Id of the Channel, null for all</param>
        /// <param name="npcMonsterVnum">VNum of the NPCMonster</param>
        /// <param name="amount">Amount to Summon</param>
        /// <param name="mapInstanceId">Id of the MapInstance to summon the NPCMonster on</param>
        /// <param name="mapX">X Coordinate</param>
        /// <param name="mapY">Y Coordinate</param>
        void SummonNPCMonster(Guid channelId, short npcMonsterVnum, short amount, Guid mapInstanceId, short mapX = 0, short mapY = 0);

        /// <summary>
        /// Get all currently registered MapInstances (for a specific MapId)
        /// </summary>
        /// <param name="worldId">Guid of the Channel</param>
        /// <param name="mapId">Id of the Map</param>
        /// <returns></returns>
        List<MapInstance> GetAllMapInstances(Guid worldId, short mapId = -1);
    }
}