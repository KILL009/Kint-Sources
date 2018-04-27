/* Box raid Erenia */
DECLARE @BoxVNum SMALLINT = 302
DECLARE @BoxDesign SMALLINT = 24

DECLARE @Epee150 SMALLINT = 4889
DECLARE @EpeeInfernal SMALLINT = 4964
DECLARE @Arba150 SMALLINT = 4895
DECLARE @ArbaInfernal SMALLINT = 4961
DECLARE @Armure150 SMALLINT = 4883
DECLARE @ArmureInfernal SMALLINT = 4952
DECLARE @Arc150 SMALLINT = 4891
DECLARE @ArcInfernal SMALLINT = 4966
DECLARE @Dague150 SMALLINT = 4897
DECLARE @DagueInfernal SMALLINT = 4963
DECLARE @Tunique150 SMALLINT = 4885
DECLARE @TuniqueInfernal SMALLINT = 4954
DECLARE @Baton150 SMALLINT = 4893
DECLARE @BatonInfernal SMALLINT = 4965
DECLARE @Gun150 SMALLINT = 4899
DECLARE @GunInfernal SMALLINT = 4962
DECLARE @Robe150 SMALLINT = 4887
DECLARE @RobeInfernal SMALLINT = 4953
DECLARE @PotionFULL SMALLINT = 1244
DECLARE @BlueStone SMALLINT = 2395
DECLARE @DragonSkin SMALLINT = 2511
DECLARE @DragonHeart SMALLINT = 2512
DECLARE @DragonBlood SMALLINT = 2513
DECLARE @AngelWings SMALLINT = 2282
DECLARE @FullMoon SMALLINT = 1030
DECLARE @Lucifer SMALLINT = 4812 
DECLARE @Maru SMALLINT = 4808
DECLARE @SRagnar SMALLINT = 4326
DECLARE @Leona SMALLINT = 4166 
DECLARE @Ragnar SMALLINT = 4305
DECLARE @Frigg SMALLINT = 4304
DECLARE @FairyFireErenia SMALLINT = 4709
DECLARE @FairyWaterErenia SMALLINT = 4710
DECLARE @FairyLightErenia SMALLINT = 4711
DECLARE @FairyDarkErenia SMALLINT = 4712
DECLARE @EreniaGloves SMALLINT = 4969
DECLARE @EreniaBoots SMALLINT = 4970

INSERT INTO [opennos].[dbo].[RollGeneratedItem] (
	[OriginalItemDesign], [OriginalItemVNum],
	[Probability], [ItemGeneratedAmount], [ItemGeneratedVNum],
	[IsRareRandom],	[MinimumOriginalItemRare], [MaximumOriginalItemRare],
	[IsSuperReward], [ItemGeneratedUpgrade]
)
VALUES
	(@BoxDesign, @BoxVNum, '17', '1', @Epee150, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @EpeeInfernal, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @Arc150, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @ArcInfernal, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @Baton150, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @BatonInfernal, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @Arba150, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @ArbaInfernal, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @Dague150, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @DagueInfernal, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @Gun150, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @GunInfernal, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @Armure150, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @ArmureInfernal, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @Tunique150, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @TuniqueInfernal, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @Robe150, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @RobeInfernal, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '100', '15', @PotionFULL, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '100', '10', @BlueStone, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '80', '6', @DragonSkin, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '80', '4', @DragonHeart, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '80', '6', @DragonBlood, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '80', '30', @AngelWings, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '80', '30', @FullMoon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '3', '1', @Lucifer, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '3', '1', @Maru, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '3', '1', @SRagnar, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '5', '1', @Leona, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '5', '1', @Ragnar, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '5', '1', @Frigg, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @FairyFireErenia, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @FairyWaterErenia, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @FairyLightErenia, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @FairyDarkErenia, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '2', '1', @EreniaGloves, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '2', '1', @EreniaBoots, '0', '0', '0', '0', '0');