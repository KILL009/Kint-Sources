/* Box raid Zenas */
DECLARE @BoxVNum SMALLINT = 302
DECLARE @BoxDesign SMALLINT = 23


DECLARE @Epee150 SMALLINT = 4889
DECLARE @EpeeCeleste SMALLINT = 4958
DECLARE @Arba150 SMALLINT = 4895
DECLARE @ArbaCeleste SMALLINT = 4955
DECLARE @Armure150 SMALLINT = 4883
DECLARE @ArmureCeleste SMALLINT = 4949
DECLARE @Arc150 SMALLINT = 4891
DECLARE @ArcCeleste SMALLINT = 4960
DECLARE @Dague150 SMALLINT = 4897
DECLARE @DagueCeleste SMALLINT = 4957
DECLARE @Tunique150 SMALLINT = 4885
DECLARE @TuniqueCeleste SMALLINT = 4951
DECLARE @Baton150 SMALLINT = 4893
DECLARE @BatonCeleste SMALLINT = 4959
DECLARE @Gun150 SMALLINT = 4899
DECLARE @GunCeleste SMALLINT = 4956
DECLARE @Robe150 SMALLINT = 4887
DECLARE @RobeCeleste SMALLINT = 4950
DECLARE @PotionFULL SMALLINT = 1244
DECLARE @BlueStone SMALLINT = 2395 
DECLARE @DragonSkin SMALLINT = 2511
DECLARE @DragonHeart SMALLINT = 2512
DECLARE @DragonBlood SMALLINT = 2513
DECLARE @AngelWings SMALLINT = 2282
DECLARE @FullMoon SMALLINT = 1030
DECLARE @Lucifer SMALLINT = 4812 
DECLARE @MaruMom SMALLINT = 4809
DECLARE @SRagnar SMALLINT = 4326
DECLARE @Leona SMALLINT = 4166 
DECLARE @Ragnar SMALLINT = 4305
DECLARE @Frigg SMALLINT = 4304
DECLARE @FairyFireZenas SMALLINT = 4705
DECLARE @FairyWaterZenas SMALLINT = 4706
DECLARE @FairyLightZenas SMALLINT = 4707
DECLARE @FairyDarkZenas SMALLINT = 4708
DECLARE @ZenasGloves SMALLINT = 4967
DECLARE @ZenasBoots SMALLINT = 4968

INSERT INTO [opennos].[dbo].[RollGeneratedItem] (
	[OriginalItemDesign], [OriginalItemVNum],
	[Probability], [ItemGeneratedAmount], [ItemGeneratedVNum],
	[IsRareRandom],	[MinimumOriginalItemRare], [MaximumOriginalItemRare],
	[IsSuperReward], [ItemGeneratedUpgrade]
)
VALUES
	(@BoxDesign, @BoxVNum, '17', '1', @Epee150, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @EpeeCeleste, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @Arba150, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @ArbaCeleste, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @Armure150, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @ArmureCeleste, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @Arc150, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @ArcCeleste, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @Dague150, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @DagueCeleste, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @Tunique150, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @TuniqueCeleste, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @Baton150, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @BatonCeleste, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @Gun150, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @GunCeleste, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @Robe150, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @RobeCeleste, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '100', '15', @PotionFULL, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '100', '10', @BlueStone, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '80', '6', @DragonSkin, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '80', '4', @DragonHeart, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '80', '6', @DragonBlood, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '80', '30', @AngelWings, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '80', '30', @FullMoon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '3', '1', @Lucifer, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '3', '1', @MaruMom, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '3', '1', @SRagnar, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '5', '1', @Leona, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '5', '1', @Ragnar, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '5', '1', @Frigg, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @FairyFireZenas, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @FairyWaterZenas, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @FairyLightZenas, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @FairyDarkZenas, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '2', '1', @ZenasGloves, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '2', '1', @ZenasBoots, '0', '0', '0', '0', '0');