
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 02/10/2018 11:31:21
-- Generated from EDMX file: D:\Sorce all Opennos\NosTale4All\OpenNos.Nosmall\Models\Login.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [opennos];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_dbo_Character_dbo_Account_AccountId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Character] DROP CONSTRAINT [FK_dbo_Character_dbo_Account_AccountId];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Account]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Account];
GO
IF OBJECT_ID(N'[dbo].[Character]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Character];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Account'
CREATE TABLE [dbo].[Account] (
    [AccountId] bigint IDENTITY(1,1) NOT NULL,
    [Authority] smallint  NOT NULL,
    [Email] nvarchar(255)  NULL,
    [Name] nvarchar(255)  NULL,
    [Password] varchar(255)  NULL,
    [RegistrationIP] nvarchar(45)  NULL,
    [VerificationToken] nvarchar(32)  NULL,
    [ReferrerId] bigint  NOT NULL,
    [BankGold] bigint  NOT NULL,
    [NosDollar] int  NOT NULL
);
GO

-- Creating table 'Character'
CREATE TABLE [dbo].[Character] (
    [CharacterId] bigint IDENTITY(1,1) NOT NULL,
    [AccountId] bigint  NOT NULL,
    [Act4Dead] int  NOT NULL,
    [Act4Kill] int  NOT NULL,
    [Act4Points] int  NOT NULL,
    [ArenaWinner] int  NOT NULL,
    [Biography] nvarchar(255)  NULL,
    [BuffBlocked] bit  NOT NULL,
    [Class] tinyint  NOT NULL,
    [Compliment] smallint  NOT NULL,
    [Dignity] real  NOT NULL,
    [EmoticonsBlocked] bit  NOT NULL,
    [ExchangeBlocked] bit  NOT NULL,
    [Faction] tinyint  NOT NULL,
    [FamilyRequestBlocked] bit  NOT NULL,
    [FriendRequestBlocked] bit  NOT NULL,
    [Gender] tinyint  NOT NULL,
    [Gold] bigint  NOT NULL,
    [GroupRequestBlocked] bit  NOT NULL,
    [HairColor] tinyint  NOT NULL,
    [HairStyle] tinyint  NOT NULL,
    [HeroChatBlocked] bit  NOT NULL,
    [HeroLevel] tinyint  NOT NULL,
    [HeroXp] bigint  NOT NULL,
    [Hp] int  NOT NULL,
    [HpBlocked] bit  NOT NULL,
    [JobLevel] tinyint  NOT NULL,
    [JobLevelXp] bigint  NOT NULL,
    [Level] tinyint  NOT NULL,
    [LevelXp] bigint  NOT NULL,
    [MapId] smallint  NOT NULL,
    [MapX] smallint  NOT NULL,
    [MapY] smallint  NOT NULL,
    [MasterPoints] int  NOT NULL,
    [MasterTicket] int  NOT NULL,
    [MinilandInviteBlocked] bit  NOT NULL,
    [MouseAimLock] bit  NOT NULL,
    [Mp] int  NOT NULL,
    [Name] varchar(255)  NULL,
    [QuickGetUp] bit  NOT NULL,
    [RagePoint] bigint  NOT NULL,
    [Slot] tinyint  NOT NULL,
    [SpAdditionPoint] int  NOT NULL,
    [SpPoint] int  NOT NULL,
    [State] tinyint  NOT NULL,
    [TalentLose] int  NOT NULL,
    [TalentSurrender] int  NOT NULL,
    [TalentWin] int  NOT NULL,
    [WhisperBlocked] bit  NOT NULL,
    [MinilandState] tinyint  NOT NULL,
    [MinilandMessage] nvarchar(255)  NULL,
    [MinilandPoint] smallint  NOT NULL,
    [MaxMateCount] tinyint  NOT NULL,
    [Reputation] bigint  NOT NULL,
    [GoldBank] bigint  NOT NULL,
    [SkyLandOr] int  NULL,
    [BankGold] bigint  NOT NULL,
    [LastFamilyLeave] bigint  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [AccountId] in table 'Account'
ALTER TABLE [dbo].[Account]
ADD CONSTRAINT [PK_Account]
    PRIMARY KEY CLUSTERED ([AccountId] ASC);
GO

-- Creating primary key on [CharacterId] in table 'Character'
ALTER TABLE [dbo].[Character]
ADD CONSTRAINT [PK_Character]
    PRIMARY KEY CLUSTERED ([CharacterId] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [AccountId] in table 'Character'
ALTER TABLE [dbo].[Character]
ADD CONSTRAINT [FK_dbo_Character_dbo_Account_AccountId]
    FOREIGN KEY ([AccountId])
    REFERENCES [dbo].[Account]
        ([AccountId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_Character_dbo_Account_AccountId'
CREATE INDEX [IX_FK_dbo_Character_dbo_Account_AccountId]
ON [dbo].[Character]
    ([AccountId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------