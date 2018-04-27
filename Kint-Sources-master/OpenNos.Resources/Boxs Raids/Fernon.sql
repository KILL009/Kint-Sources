/* Box raid Fernon */
DECLARE @BoxVNum SMALLINT = 302
DECLARE @BoxDesign SMALLINT = 25

DECLARE @EpeeFernon SMALLINT = 4981
DECLARE @ArbaFernon SMALLINT = 4978
DECLARE @ArmureFernon SMALLINT = 4984
DECLARE @ArcFernon SMALLINT = 4983
DECLARE @DagueFernon SMALLINT = 4980
DECLARE @TuniqueFernon SMALLINT = 4986
DECLARE @BatonFernon SMALLINT = 4982
DECLARE @GunFernon SMALLINT = 4879
DECLARE @RobeFernon SMALLINT = 4985
DECLARE @PotionFULL SMALLINT = 1244
DECLARE @BlueStone SMALLINT = 2395
DECLARE @DragonSkin SMALLINT = 2511
DECLARE @DragonHeart SMALLINT = 2512
DECLARE @DragonBlood SMALLINT = 2513
DECLARE @CollierLaurena SMALLINT = 4835
DECLARE @AnneauLaurena SMALLINT = 4836
DECLARE @BraceletLaurena SMALLINT = 4837
DECLARE @AngelWings SMALLINT = 2282
DECLARE @FullMoon SMALLINT = 1030
DECLARE @Aegir SMALLINT = 4800
DECLARE @Foxy SMALLINT = 4807
DECLARE @Hongbi SMALLINT = 4810
DECLARE @Cheongbi SMALLINT = 4811
DECLARE @Arlequin SMALLINT = 4823
DECLARE @Erdimine SMALLINT = 4306 
DECLARE @FairyFireFernon SMALLINT = 4713
DECLARE @FairyWaterFernon SMALLINT = 4714
DECLARE @FairyLightFernon SMALLINT = 4715
DECLARE @FairyDarkFernon SMALLINT = 4716
DECLARE @FernonBoots SMALLINT = 4839

INSERT INTO [opennos].[dbo].[RollGeneratedItem] (
	[OriginalItemDesign], [OriginalItemVNum],
	[Probability], [ItemGeneratedAmount], [ItemGeneratedVNum],
	[IsRareRandom],	[MinimumOriginalItemRare], [MaximumOriginalItemRare],
	[IsSuperReward], [ItemGeneratedUpgrade]
)
VALUES
	(@BoxDesign, @BoxVNum, '17', '1', @EpeeFernon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @ArbaFernon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @ArmureFernon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @ArcFernon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @DagueFernon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @TuniqueFernon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @BatonFernon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @GunFernon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '17', '1', @RobeFernon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '100', '25', @PotionFULL, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '100', '20', @BlueStone, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '80', '8', @DragonSkin, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '80', '6', @DragonHeart, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '80', '8', @DragonBlood, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @CollierLaurena, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @AnneauLaurena, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @BraceletLaurena, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '80', '50', @AngelWings, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '80', '50', @FullMoon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '3', '1', @Aegir, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '3', '1', @Foxy, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '3', '1', @Hongbi, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '3', '1', @Cheongbi, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '3', '1', @Arlequin, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '5', '1', @Erdimine, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @FairyFireFernon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @FairyWaterFernon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @FairyLightFernon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '1', '1', @FairyDarkFernon, '0', '0', '0', '0', '0'),
	(@BoxDesign, @BoxVNum, '2', '1', @FernonBoots, '0', '0', '0', '0', '0');
	