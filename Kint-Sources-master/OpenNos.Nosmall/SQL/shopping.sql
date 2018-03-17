/*
Navicat SQL Server Data Transfer

Source Server         : localhost
Source Server Version : 140000
Source Host           : localhost:1433
Source Database       : opennos
Source Schema         : dbo

Target Server Type    : SQL Server
Target Server Version : 140000
File Encoding         : 65001

Date: 2018-01-03 22:51:49
*/


-- ----------------------------
-- Table structure for shopping
-- ----------------------------
DROP TABLE [dbo].[shopping]
GO
CREATE TABLE [dbo].[shopping] (
[Id] int NOT NULL ,
[Name] nvarchar(255) NOT NULL ,
[Price] int NOT NULL ,
[Photo] nvarchar(255) NOT NULL ,
[VNum] int NOT NULL ,
[Category] int NOT NULL ,
[Description] nvarchar(255) NOT NULL 
)


GO

-- ----------------------------
-- Records of shopping
-- ----------------------------
INSERT INTO [dbo].[shopping] ([Id], [Name], [Price], [Photo], [VNum], [Category], [Description]) VALUES (N'1', N'SP6E + 15', N'4600', N'https://img00.deviantart.net/97f7/i/2012/139/7/1/nostale_fanart____halloween_by_rya_konsata-d50bhnl.jpg', N'4497', N'1', N'//CECI EST UN FAUX PRODUIT')
GO
GO

-- ----------------------------
-- Indexes structure for table shopping
-- ----------------------------

-- ----------------------------
-- Primary Key structure for table shopping
-- ----------------------------
ALTER TABLE [dbo].[shopping] ADD PRIMARY KEY ([Id])
GO

