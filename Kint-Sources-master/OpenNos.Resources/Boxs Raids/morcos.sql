/* Box raid Morcos A4 - R3 mini */
DECLARE @BoxId SMALLINT = 185

DECLARE @RPVPWeap SMALLINT = 571
DECLARE @RPVPCost SMALLINT = 583
DECLARE @RPerfWeap SMALLINT = 574
DECLARE @RPerfCost SMALLINT = 586
DECLARE @RSpéWeap SMALLINT = 568
DECLARE @RSpéCost SMALLINT = 580
DECLARE @Eraser SMALLINT = 1430
DECLARE @RainbowPearl SMALLINT = 1429
DECLARE @GloveFire70 SMALLINT = 381
DECLARE @ShoeFire70 SMALLINT = 389
DECLARE @AngelWings SMALLINT = 1353
DECLARE @FullMoon SMALLINT = 776
DECLARE @BlueStone SMALLINT = 2395
DECLARE @LittleRuby SMALLINT = 2514
DECLARE @Ruby SMALLINT = 2518

INSERT INTO [opennos].[dbo].[RollGeneratedItem] (
	[OriginalItemDesign], [OriginalItemVNum],
	[Probability], [ItemGeneratedAmount], [ItemGeneratedVNum],
	[IsRareRandom],	[MinimumOriginalItemRare], [MaximumOriginalItemRare],
	[IsSuperReward], [ItemGeneratedUpgrade]
)
VALUES
	('0', @BoxId, '1', '1', @RPVPWeap, '0', '0', '0', '0', '0'),
	('0', @BoxId, '1', '1', @RPVPCost, '0', '0', '0', '0', '0'),
	('0', @BoxId, '1', '1', @RPerfWeap, '0', '0', '0', '0', '0'),
	('0', @BoxId, '1', '1', @RPerfCost, '0', '0', '0', '0', '0'),
	('0', @BoxId, '1', '1', @RSpéWeap, '0', '0', '0', '0', '0'),
	('0', @BoxId, '1', '1', @RSpéCost, '0', '0', '0', '0', '0'),
	('0', @BoxId, '1', '1', @Eraser, '0', '0', '0', '0', '0'),
	('0', @BoxId, '1', '20', @RainbowPearl, '0', '0', '0', '0', '0'),
	('0', @BoxId, '1', '1', @GloveFire70, '0', '0', '0', '0', '0'),
	('0', @BoxId, '1', '1', @ShoeFire70, '0', '0', '0', '0', '0'),
	('0', @BoxId, '1', '20', @AngelWings, '0', '0', '0', '0', '0'),
	('0', @BoxId, '1', '20', @FullMoon, '0', '0', '0', '0', '0'),
	('0', @BoxId, '1', '10', @BlueStone, '0', '0', '0', '0', '0'),
	('0', @BoxId, '1', '2', @LittleRuby, '0', '0', '0', '0', '0'),
	('0', @BoxId, '1', '2', @Ruby, '0', '0', '0', '0', '0')
