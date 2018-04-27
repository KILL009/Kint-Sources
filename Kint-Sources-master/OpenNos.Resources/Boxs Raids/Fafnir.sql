/* Box raid Fafnir */
DECLARE @BoxVNum SMALLINT = 302
DECLARE @BoxDesign SMALLINT = 26

DECLARE @EpeeCeleste SMALLINT = 4958
DECLARE @EpeeInfernal SMALLINT = 4964
DECLARE @EpeeFernon SMALLINT = 4981
DECLARE @ArbaCeleste SMALLINT = 4955
DECLARE @ArbaInfernal SMALLINT = 4961
DECLARE @ArbaFernon SMALLINT = 4978
DECLARE @ArmureCeleste SMALLINT = 4949
DECLARE @ArmureInfernal SMALLINT = 4952
DECLARE @ArmureFernon SMALLINT = 4984
DECLARE @ArcCeleste SMALLINT = 4960
DECLARE @ArcInfernal SMALLINT = 4966
DECLARE @ArcFernon SMALLINT = 4983
DECLARE @DagueCeleste SMALLINT = 4957
DECLARE @DagueInfernal SMALLINT = 4963
DECLARE @DagueFernon SMALLINT = 4980
DECLARE @TuniqueCeleste SMALLINT = 4951
DECLARE @TuniqueInfernal SMALLINT = 4954
DECLARE @TuniqueFernon SMALLINT = 4986
DECLARE @BatonCeleste SMALLINT = 4959
DECLARE @BatonInfernal SMALLINT = 4965
DECLARE @BatonFernon SMALLINT = 4982
DECLARE @GunCeleste SMALLINT = 4956
DECLARE @GunInfernal SMALLINT = 4962
DECLARE @GunFernon SMALLINT = 4879
DECLARE @RobeCeleste SMALLINT = 4950
DECLARE @RobeFernon SMALLINT = 4985
DECLARE @RobeInfernal SMALLINT = 4953
DECLARE @PotionFULL SMALLINT = 1244
DECLARE @BlueStone SMALLINT = 2395
DECLARE @DragonSkin SMALLINT = 2511
DECLARE @DragonHeart SMALLINT = 2512
DECLARE @DragonBlood SMALLINT = 2513
DECLARE @AngelWings SMALLINT = 2282
DECLARE @FullMoon SMALLINT = 1030 
DECLARE @Nelia SMALLINT = 4824
DECLARE @Hongbi SMALLINT = 4810
DECLARE @Cheongbi SMALLINT = 4811
DECLARE @Jennifer SMALLINT = 4315
DECLARE @Yertirand SMALLINT = 4330


INSERT INTO [opennos].[dbo].[RollGeneratedItem] (
	[OriginalItemDesign], [OriginalItemVNum],
	[Probability], [ItemGeneratedAmount], [ItemGeneratedVNum],
	[IsRareRandom],	[MinimumOriginalItemRare], [MaximumOriginalItemRare],
	[IsSuperReward], [ItemGeneratedUpgrade]
)
VALUES
	(@BoxDesign, @BoxVNum, '10', '1', @EpeeCeleste, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '10', '1', @EpeeInfernal, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @EpeeFernon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '10', '1', @ArbaCeleste, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '10', '1', @ArbaInfernal, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @ArbaFernon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '10', '1', @ArmureCeleste, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '10', '1', @ArmureInfernal, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @ArmureFernon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '10', '1', @ArcCeleste, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '10', '1', @ArcInfernal, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @ArcFernon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '10', '1', @DagueCeleste, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '10', '1', @DagueInfernal, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @DagueFernon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '10', '1', @TuniqueCeleste, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '10', '1', @TuniqueInfernal, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @TuniqueFernon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '10', '1', @BatonCeleste, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '10', '1', @BatonInfernal, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @BatonFernon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '10', '1', @GunCeleste, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '10', '1', @GunInfernal, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @GunFernon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '10', '1', @RobeCeleste, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '10', '1', @RobeInfernal, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @RobeFernon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '100', '20', @PotionFULL, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '100', '15', @BlueStone, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '80', '7', @DragonSkin, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '80', '5', @DragonHeart, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '80', '7', @DragonBlood, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '80', '35', @AngelWings, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '80', '35', @FullMoon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '3', '1', @Nelia, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '3', '1', @Hongbi, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '3', '1', @Cheongbi, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '5', '1', @Jennifer, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '5', '1', @Yertirand, '0', '0', '0', '0', '0');