/* Box raid Laurena */
DECLARE @BoxVNum SMALLINT = 302
DECLARE @BoxDesign SMALLINT = 20

DECLARE @Epee110 SMALLINT = 4866
DECLARE @Epee130 SMALLINT = 4888
DECLARE @Arba110 SMALLINT = 4863
DECLARE @Arba130 SMALLINT = 4894
DECLARE @Armure110 SMALLINT = 4854
DECLARE @Armure130 SMALLINT = 4882
DECLARE @Arc110 SMALLINT = 4868
DECLARE @Arc130 SMALLINT = 4890
DECLARE @Dague110 SMALLINT = 4865
DECLARE @Dague130 SMALLINT = 4896
DECLARE @Tunique110 SMALLINT = 4856
DECLARE @Tunique130 SMALLINT = 4884
DECLARE @Baton110 SMALLINT = 4867
DECLARE @Baton130 SMALLINT = 4892
DECLARE @Gun110 SMALLINT = 4864
DECLARE @Gun130 SMALLINT = 4898
DECLARE @Robe110 SMALLINT = 4855
DECLARE @Robe130 SMALLINT = 4886
DECLARE @PotionFULL SMALLINT = 1244
DECLARE @BlueStone SMALLINT = 2395
DECLARE @DragonSkin SMALLINT = 2511
DECLARE @DragonHeart SMALLINT = 2512
DECLARE @DragonBlood SMALLINT = 2513
DECLARE @AngelWings SMALLINT = 2282
DECLARE @FullMoon SMALLINT = 1030
DECLARE @LaurenaHat SMALLINT = 4699
DECLARE @CowGirl SMALLINT = 4817
DECLARE @Fiona SMALLINT = 4818
DECLARE @Lucie SMALLINT = 4815
DECLARE @Laurena SMALLINT = 4813
DECLARE @Eliza SMALLINT = 4820
DECLARE @Graham SMALLINT = 4977
DECLARE @Sakura SMALLINT = 335
DECLARE @Motifere SMALLINT = 4493
DECLARE @DemonHunter SMALLINT = 4492
DECLARE @Devin SMALLINT = 4491
DECLARE @Renegat SMALLINT = 4489
DECLARE @AngeVengeur SMALLINT = 4488
DECLARE @Archimage SMALLINT = 4487

INSERT INTO [opennos].[dbo].[RollGeneratedItem] (
	[OriginalItemDesign], [OriginalItemVNum],
	[Probability], [ItemGeneratedAmount], [ItemGeneratedVNum],
	[IsRareRandom],	[MinimumOriginalItemRare], [MaximumOriginalItemRare],
	[IsSuperReward], [ItemGeneratedUpgrade]
)
VALUES
	(@BoxDesign, @BoxVNum, '17', '1', @Epee110, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '13', '1', @Epee130, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @Arba110, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '13', '1', @Arba130, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @Armure110, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '13', '1', @Armure130, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @Arc110, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '13', '1', @Arc130, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @Dague110, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '13', '1', @Dague130, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @Tunique110, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '13', '1', @Tunique130, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @Baton110, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '13', '1', @Baton130, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @Gun110, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '13', '1', @Gun130, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @Robe110, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '13', '1', @Robe130, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '100', '7', @PotionFULL, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '100', '5', @BlueStone, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '80', '5', @DragonSkin, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '80', '3', @DragonHeart, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '80', '5', @DragonBlood, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '80', '15', @AngelWings, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '80', '15', @FullMoon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @LaurenaHat, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '3', '1', @CowGirl, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '3', '1', @Fiona, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '3', '1', @Lucie, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '3', '1', @Laurena, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '3', '1', @Eliza, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '5', '1', @Graham, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '5', '1', @Sakura, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '5', '1', @Motifere, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '5', '1', @DemonHunter, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '5', '1', @Devin, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '5', '1', @Renegat, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '5', '1', @AngeVengeur, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '5', '1', @Archimage, '0', '0', '0', '0', '0');