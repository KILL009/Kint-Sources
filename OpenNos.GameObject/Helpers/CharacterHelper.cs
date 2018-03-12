using OpenNos.Core;
using OpenNos.Domain;
using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenNos.GameObject.Helpers
{
    public class CharacterHelper : Singleton<CharacterHelper>
    {
        #region Members

        private int[,] criticalDist;

        private int[,] criticalDistRate;

        private int[,] criticalHit;

        private int[,] criticalHitRate;

        private int[,] distDef;

        private int[,] distDodge;

        private int[,] distRate;

        private int[,] hitDef;

        private int[,] hitDodge;

        private int[,] hitRate;

        private int[,] magicalDef;

        private int[,] maxDist;

        private int[,] maxHit;

        private int[,] minDist;

        // difference between class
        private int[,] minHit;

        #endregion

        #region Instantiation

        public CharacterHelper()
        {
            LoadSpeedData();
            LoadJobXpData();
            LoadSpxpData();
            LoadHeroXpData();
            LoadXpData();
            LoadHpData();
            LoadMpData();
            LoadStats();
            LoadHpHealth();
            LoadMpHealth();
            LoadHpHealthStand();
            LoadMpHealthStand();
        }

        #endregion

        // STAT DATA

        // same for all class

        #region Properties

        public double[] FirstJobXpData { get; private set; }

        public double[] HeroXpData { get; private set; }

        public int[,] HpData { get; private set; }

        public int[] HpHealth { get; private set; }

        public int[] HpHealthStand { get; private set; }

        public int[,] MpData { get; private set; }

        public int[] MpHealth { get; private set; }

        public int[] MpHealthStand { get; private set; }

        public double[] SecondJobXpData { get; private set; }

        public byte[] SpeedData { get; private set; }

        public double[] SpxpData { get; private set; }

        public double[] XpData { get; private set; }

        #endregion

        #region Methods

        public static float ExperiencePenalty(byte playerLevel, byte monsterLevel)
        {
            var leveldifference = playerLevel - monsterLevel;
            float penalty;

            // penalty calculation
            switch (leveldifference)
            {
                case 6:
                    penalty = 0.9f;
                    break;

                case 7:
                    penalty = 0.7f;
                    break;

                case 8:
                    penalty = 0.5f;
                    break;

                case 9:
                    penalty = 0.3f;
                    break;

                default:
                    if (leveldifference > 9)
                    {
                        penalty = 0.1f;
                    }
                    else if (leveldifference > 18)
                    {
                        penalty = 0.05f;
                    }
                    else
                    {
                        penalty = 1f;
                    }

                    break;
            }

            return penalty;
        }

        public static float GoldPenalty(byte playerLevel, byte monsterLevel)
        {
            var leveldifference = playerLevel - monsterLevel;
            float penalty;

            // penalty calculation
            switch (leveldifference)
            {
                case 5:
                    penalty = 0.9f;
                    break;

                case 6:
                    penalty = 0.7f;
                    break;

                case 7:
                    penalty = 0.5f;
                    break;

                case 8:
                    penalty = 0.3f;
                    break;

                case 9:
                    penalty = 0.2f;
                    break;

                default:
                    if (leveldifference > 9 && leveldifference < 19)
                    {
                        penalty = 0.1f;
                    }
                    else if (leveldifference > 18 && leveldifference < 30)
                    {
                        penalty = 0.05f;
                    }
                    else if (leveldifference > 30)
                    {
                        penalty = 0f;
                    }
                    else
                    {
                        penalty = 1f;
                    }

                    break;
            }

            return penalty;
        }

        public static long LoadFairyXpData(long elementRate)
        {
            if (elementRate < 40)
            {
                return elementRate * elementRate + 50;
            }

            return elementRate * elementRate * 3 + 50;
        }

        public static int LoadFamilyXpData(byte familyLevel)
        {
            switch (familyLevel)
            {
                case 1:
                    return 100000;

                case 2:
                    return 250000;

                case 3:
                    return 370000;

                case 4:
                    return 560000;

                case 5:
                    return 840000;

                case 6:
                    return 1260000;

                case 7:
                    return 1900000;

                case 8:
                    return 2850000;

                case 9:
                    return 3570000;

                case 10:
                    return 3830000;

                case 11:
                    return 4150000;

                case 12:
                    return 4750000;

                case 13:
                    return 5500000;

                case 14:
                    return 6500000;

                case 15:
                    return 7000000;

                case 16:
                    return 8500000;

                case 17:
                    return 9500000;

                case 18:
                    return 10000000;

                case 19:
                    return 17000000;

                default:
                    return 999999999;
            }
        }

        public int MagicalDefence(ClassType @class, byte level) => magicalDef[(byte)@class, level];

        public int MaxDistance(ClassType @class, byte level) => maxDist[(byte)@class, level];

        public int MaxHit(ClassType @class, byte level) => maxHit[(byte)@class, level];

        public int MinDistance(ClassType @class, byte level) => minDist[(byte)@class, level];

        public int MinHit(ClassType @class, byte level) => minHit[(int)@class, level];

        public int RarityPoint(short rarity, short lvl)
        {
            int p;
            switch (rarity)
            {
                case 0:
                    p = 0;
                    break;

                case 1:
                    p = 1;
                    break;

                case 2:
                    p = 2;
                    break;

                case 3:
                    p = 3;
                    break;

                case 4:
                    p = 4;
                    break;

                case 5:
                    p = 5;
                    break;

                case 6:
                    p = 7;
                    break;

                case 7:
                    p = 10;
                    break;

                case 8:
                    p = 15;
                    break;

                default:
                    p = rarity * 2;
                    break;
            }

            return p * (lvl / 5 + 1);
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.LayoutRules", "SA1503:CurlyBracketsMustNotBeOmitted", Justification = "Easier to read")]
        public int SlPoint(short spPoint, short mode)
        {
            try
            {
                var point = 0;
                switch (mode)
                {
                    case 0:
                        if (spPoint <= 10)
                        {
                            point = spPoint;
                        }
                        else if (spPoint <= 28)
                        {
                            point = 10 + (spPoint - 10) / 2;
                        }
                        else if (spPoint <= 88)
                        {
                            point = 19 + (spPoint - 28) / 3;
                        }
                        else if (spPoint <= 168)
                        {
                            point = 39 + (spPoint - 88) / 4;
                        }
                        else if (spPoint <= 268)
                        {
                            point = 59 + (spPoint - 168) / 5;
                        }
                        else if (spPoint <= 334)
                        {
                            point = 79 + (spPoint - 268) / 6;
                        }
                        else if (spPoint <= 383)
                        {
                            point = 90 + (spPoint - 334) / 7;
                        }
                        else if (spPoint <= 391)
                        {
                            point = 97 + (spPoint - 383) / 8;
                        }
                        else if (spPoint <= 400)
                        {
                            point = 98 + (spPoint - 391) / 9;
                        }
                        else if (spPoint <= 410)
                        {
                            point = 99 + (spPoint - 400) / 10;
                        }

                        break;

                    case 2:
                        if (spPoint <= 20)
                        {
                            point = spPoint;
                        }
                        else if (spPoint <= 40)
                        {
                            point = 20 + (spPoint - 20) / 2;
                        }
                        else if (spPoint <= 70)
                        {
                            point = 30 + (spPoint - 40) / 3;
                        }
                        else if (spPoint <= 110)
                        {
                            point = 40 + (spPoint - 70) / 4;
                        }
                        else if (spPoint <= 210)
                        {
                            point = 50 + (spPoint - 110) / 5;
                        }
                        else if (spPoint <= 270)
                        {
                            point = 70 + (spPoint - 210) / 6;
                        }
                        else if (spPoint <= 410)
                        {
                            point = 80 + (spPoint - 270) / 7;
                        }

                        break;

                    case 1:
                        if (spPoint <= 10)
                        {
                            point = spPoint;
                        }
                        else if (spPoint <= 48)
                        {
                            point = 10 + (spPoint - 10) / 2;
                        }
                        else if (spPoint <= 81)
                        {
                            point = 29 + (spPoint - 48) / 3;
                        }
                        else if (spPoint <= 161)
                        {
                            point = 40 + (spPoint - 81) / 4;
                        }
                        else if (spPoint <= 236)
                        {
                            point = 60 + (spPoint - 161) / 5;
                        }
                        else if (spPoint <= 290)
                        {
                            point = 75 + (spPoint - 236) / 6;
                        }
                        else if (spPoint <= 360)
                        {
                            point = 84 + (spPoint - 290) / 7;
                        }
                        else if (spPoint <= 400)
                        {
                            point = 97 + (spPoint - 360) / 8;
                        }
                        else if (spPoint <= 410)
                        {
                            point = 99 + (spPoint - 400) / 10;
                        }

                        break;

                    case 3:
                        if (spPoint <= 10)
                        {
                            point = spPoint;
                        }
                        else if (spPoint <= 50)
                        {
                            point = 10 + (spPoint - 10) / 2;
                        }
                        else if (spPoint <= 110)
                        {
                            point = 30 + (spPoint - 50) / 3;
                        }
                        else if (spPoint <= 150)
                        {
                            point = 50 + (spPoint - 110) / 4;
                        }
                        else if (spPoint <= 200)
                        {
                            point = 60 + (spPoint - 150) / 5;
                        }
                        else if (spPoint <= 260)
                        {
                            point = 70 + (spPoint - 200) / 6;
                        }
                        else if (spPoint <= 330)
                        {
                            point = 80 + (spPoint - 260) / 7;
                        }
                        else if (spPoint <= 410)
                        {
                            point = 90 + (spPoint - 330) / 8;
                        }

                        break;
                }

                return point;
            }
            catch
            {
                return 0;
            }
        }

        public int SpPoint(short spLevel, short upgrade)
        {
            var point = (spLevel - 20) * 3;
            if (spLevel <= 20)
            {
                point = 0;
            }

            switch (upgrade)
            {
                case 1:
                    point += 5;
                    break;

                case 2:
                    point += 10;
                    break;

                case 3:
                    point += 15;
                    break;

                case 4:
                    point += 20;
                    break;

                case 5:
                    point += 28;
                    break;

                case 6:
                    point += 36;
                    break;

                case 7:
                    point += 46;
                    break;

                case 8:
                    point += 56;
                    break;

                case 9:
                    point += 68;
                    break;

                case 10:
                    point += 80;
                    break;

                case 11:
                    point += 95;
                    break;

                case 12:
                    point += 110;
                    break;

                case 13:
                    point += 128;
                    break;

                case 14:
                    point += 148;
                    break;

                case 15:
                    point += 173;
                    break;
            }

            if (upgrade > 15)
            {
                point += 173 + 25 + 5 * (upgrade - 15);
            }

            return point;
        }

        internal int DarkResistance(ClassType @class, byte level) => 0;

        internal int Defence(ClassType @class, byte level) => hitDef[(byte)@class, level];

        /// <summary> Defence rate base stats for Character by Class & Level </summary> <param
        /// name="class"></param> <param name="level"></param> <returns></returns>
        internal int DefenceRate(ClassType @class, byte level) => hitDodge[(byte)@class, level];

        /// <summary> Distance Defence base stats for Character by Class & Level </summary> <param
        /// name="class"></param> <param name="level"></param> <returns></returns>
        internal int DistanceDefence(ClassType @class, byte level) => distDef[(byte)@class, level];

        /// <summary> Distance Defence Rate base stats for Character by Class & Level </summary>
        /// <param name="class"></param> <param name="level"></param> <returns></returns>
        internal int DistanceDefenceRate(ClassType @class, byte level)
        {
            return distDodge[(byte)@class, level];
        }

        /// <summary> Distance Rate base stats for Character by Class & Level </summary> <param
        /// name="class"></param> <param name="level"></param> <returns></returns>
        internal int DistanceRate(ClassType @class, byte level) => distRate[(byte)@class, level];

        internal int DistCritical(ClassType @class, byte level) => criticalDist[(byte)@class, level];

        internal int DistCriticalRate(ClassType @class, byte level)
        {
            return criticalDistRate[(byte)@class, level];
        }

        internal int Element(ClassType @class, byte level) => 0;

        internal int ElementRate(ClassType @class, byte level) => 0;

        internal int FireResistance(ClassType @class, byte level) => 0;

        internal int HitCritical(ClassType @class, byte level) => criticalHit[(byte)@class, level];

        internal int HitCriticalRate(ClassType @class, byte level)
        {
            return criticalHitRate[(byte)@class, level];
        }

        internal int HitRate(ClassType @class, byte level) => hitRate[(byte)@class, level];

        internal int LightResistance(ClassType @class, byte level) => 0;

        internal int WaterResistance(ClassType @class, byte level) => 0;

        private void LoadHeroXpData()
        {
            var index = 1;
            var increment = 118980;
            var increment2 = 9120;
            var increment3 = 360;

            HeroXpData = new double[256];
            HeroXpData[0] = 949560;
            for (int lvl = 1; lvl < HeroXpData.Length; lvl++)
            {
                HeroXpData[lvl] = HeroXpData[lvl - 1] + increment;
                increment2 += increment3;
                increment = increment + increment2;
                index++;
                if (index % 10 == 0)
                {
                    if (index / 10 < 3)
                    {
                        increment3 -= index / 10 * 30;
                    }
                    else
                    {
                        increment3 -= 30;
                    }
                }
            }
        }

        private void LoadHpData()
        {
            HpData = new int[4, 256];

            // Adventurer HP
            for (int i = 1; i < HpData.GetLength(1); i++)
            {
                HpData[(int)ClassType.Adventurer, i] = (int)(1 / 2.0 * i * i + 31 / 2.0 * i + 205);
            }

            // Swordsman HP
            for (int i = 0; i < HpData.GetLength(1); i++)
            {
                var j = 16;
                var hp = 946;
                var inc = 85;
                while (j <= i)
                {
                    if ((j % 5) == 2)
                    {
                        hp += inc / 2;
                        inc += 2;
                    }
                    else
                    {
                        hp += inc;
                        inc += 4;
                    }

                    ++j;
                }

                HpData[(int)ClassType.Swordman, i] = hp;
            }

            // Magician HP
            for (int i = 0; i < HpData.GetLength(1); i++)
            {
                HpData[(int)ClassType.Magician, i] = (int)((i + 15) * (i + 15) + i + 15.0 - 465 + 550);
            }

            // Archer HP
            for (int i = 0; i < HpData.GetLength(1); i++)
            {
                var hp = 680;
                var inc = 35;
                var j = 16;
                while (j <= i)
                {
                    hp += inc;
                    ++inc;
                    if ((j % 10) == 1 || (j % 10) == 5 || (j % 10) == 8)
                    {
                        hp += inc;
                        ++inc;
                    }

                    ++j;
                }

                HpData[(int)ClassType.Archer, i] = hp;
            }
        }

        private void LoadHpHealth()
        {
            HpHealth = new int[4];
            HpHealth[(int)ClassType.Archer] = 60;
            HpHealth[(int)ClassType.Adventurer] = 30;
            HpHealth[(int)ClassType.Swordman] = 90;
            HpHealth[(int)ClassType.Magician] = 30;
        }

        private void LoadHpHealthStand()
        {
            HpHealthStand = new int[4];
            HpHealthStand[(int)ClassType.Archer] = 32;
            HpHealthStand[(int)ClassType.Adventurer] = 25;
            HpHealthStand[(int)ClassType.Swordman] = 26;
            HpHealthStand[(int)ClassType.Magician] = 20;
        }

        private void LoadJobXpData()
        {
            // Load JobData
            FirstJobXpData = new double[21];
            SecondJobXpData = new double[256];
            FirstJobXpData[0] = 2200;
            SecondJobXpData[0] = 17600;
            for (int i = 1; i < FirstJobXpData.Length; i++)
                FirstJobXpData[i] = FirstJobXpData[i - 1] + 700;

            for (int i = 1; i < SecondJobXpData.Length; i++)
            {
                var var2 = 400;
                if (i > 3)
                {
                    var2 = 4500;
                }

                if (i > 40)
                {
                    var2 = 15000;
                }

                SecondJobXpData[i] = SecondJobXpData[i - 1] + var2;
            }
        }

        private void LoadMpData()
        {
            MpData = new int[4, 257];

            // ADVENTURER MP
            MpData[(int)ClassType.Adventurer, 0] = 60;
            var baseAdventurer = 9;
            for (int i = 1; i < MpData.GetLength(1); i += 4)
            {
                MpData[(int)ClassType.Adventurer, i] = MpData[(int)ClassType.Adventurer, i - 1] + baseAdventurer;
                MpData[(int)ClassType.Adventurer, i + 1] = MpData[(int)ClassType.Adventurer, i] + baseAdventurer;
                MpData[(int)ClassType.Adventurer, i + 2] = MpData[(int)ClassType.Adventurer, i + 1] + baseAdventurer;
                baseAdventurer++;
                MpData[(int)ClassType.Adventurer, i + 3] = MpData[(int)ClassType.Adventurer, i + 2] + baseAdventurer;
                baseAdventurer++;
            }

            // SWORDSMAN MP
            for (int i = 1; i < MpData.GetLength(1) - 1; i++)
            {
                MpData[(int)ClassType.Swordman, i] = MpData[(int)ClassType.Adventurer, i];
            }

            // ARCHER MP
            for (int i = 0; i < MpData.GetLength(1) - 1; i++)
            {
                MpData[(int)ClassType.Archer, i] = MpData[(int)ClassType.Adventurer, i + 1];
            }

            // MAGICIAN MP
            for (int i = 0; i < MpData.GetLength(1) - 1; i++)
            {
                MpData[(int)ClassType.Magician, i] = 3 * MpData[(int)ClassType.Adventurer, i];
            }
        }

        private void LoadMpHealth()
        {
            MpHealth = new int[4];
            MpHealth[(int)ClassType.Adventurer] = 10;
            MpHealth[(int)ClassType.Swordman] = 30;
            MpHealth[(int)ClassType.Archer] = 50;
            MpHealth[(int)ClassType.Magician] = 80;
        }

        private void LoadMpHealthStand()
        {
            MpHealthStand = new int[4];
            MpHealthStand[(int)ClassType.Adventurer] = 5;
            MpHealthStand[(int)ClassType.Swordman] = 16;
            MpHealthStand[(int)ClassType.Archer] = 28;
            MpHealthStand[(int)ClassType.Magician] = 40;
        }

        private void LoadSpeedData()
        {
            SpeedData = new byte[4];
            SpeedData[(int)ClassType.Adventurer] = 11;
            SpeedData[(int)ClassType.Swordman] = 11;
            SpeedData[(int)ClassType.Archer] = 12;
            SpeedData[(int)ClassType.Magician] = 10;
        }

        private void LoadSpxpData()
        {
            // Load SpData
            SpxpData = new double[256];
            SpxpData[0] = 15000;
            SpxpData[19] = 218000;
            for (int i = 1; i < 19; i++)
                SpxpData[i] = SpxpData[i - 1] + 10000;

            for (int i = 20; i < SpxpData.Length; i++)
                SpxpData[i] = SpxpData[i - 1] + 6 * (3 * i * (i + 1) + 1);
        }

        // TODO: Change or Verify
        private void LoadStats()
        {
            minHit = new int[4, 256];
            maxHit = new int[4, 256];
            hitRate = new int[4, 256];
            criticalHitRate = new int[4, 256];
            criticalHit = new int[4, 256];
            minDist = new int[4, 256];
            maxDist = new int[4, 256];
            distRate = new int[4, 256];
            criticalDistRate = new int[4, 256];
            criticalDist = new int[4, 256];
            hitDef = new int[4, 256];
            hitDodge = new int[4, 256];
            distDef = new int[4, 256];
            distDodge = new int[4, 256];
            magicalDef = new int[4, 256];

            for (int i = 0; i < 256; i++)
            {
                // ADVENTURER
                minHit[(int)ClassType.Adventurer, i] = i + 9; // approx
                maxHit[(int)ClassType.Adventurer, i] = i + 9; // approx
                hitRate[(int)ClassType.Adventurer, i] = i + 9; // approx
                criticalHitRate[(int)ClassType.Adventurer, i] = 0; // sure
                criticalHit[(int)ClassType.Adventurer, i] = 0; // sure
                minDist[(int)ClassType.Adventurer, i] = i + 9; // approx
                maxDist[(int)ClassType.Adventurer, i] = i + 9; // approx
                distRate[(int)ClassType.Adventurer, i] = (i + 9) * 2; // approx
                criticalDistRate[(int)ClassType.Adventurer, i] = 0; // sure
                criticalDist[(int)ClassType.Adventurer, i] = 0; // sure
                hitDef[(int)ClassType.Adventurer, i] = i + 9 / 2; // approx
                hitDodge[(int)ClassType.Adventurer, i] = i + 9; // approx
                distDef[(int)ClassType.Adventurer, i] = (i + 9) / 2; // approx
                distDodge[(int)ClassType.Adventurer, i] = i + 9; // approx
                magicalDef[(int)ClassType.Adventurer, i] = (i + 9) / 2; // approx

                // SWORDMAN
                criticalHitRate[(int)ClassType.Swordman, i] = 0; // approx
                criticalHit[(int)ClassType.Swordman, i] = 0; // approx
                criticalDist[(int)ClassType.Swordman, i] = 0; // approx
                criticalDistRate[(int)ClassType.Swordman, i] = 0; // approx
                minDist[(int)ClassType.Swordman, i] = i + 12; // approx
                maxDist[(int)ClassType.Swordman, i] = i + 12; // approx
                distRate[(int)ClassType.Swordman, i] = 2 * (i + 12); // approx
                hitDodge[(int)ClassType.Swordman, i] = i + 12; // approx
                distDodge[(int)ClassType.Swordman, i] = i + 12; // approx
                magicalDef[(int)ClassType.Swordman, i] = (i + 9) / 2; // approx
                hitRate[(int)ClassType.Swordman, i] = i + 27; // approx
                hitDef[(int)ClassType.Swordman, i] = i + 2; // approx

                minHit[(int)ClassType.Swordman, i] = 2 * i + 5; // approx Numbers n such that 10n+9 is prime.
                maxHit[(int)ClassType.Swordman, i] = 2 * i + 5; // approx Numbers n such that 10n+9 is prime.
                distDef[(int)ClassType.Swordman, i] = i; // approx

                // MAGICIAN
                hitRate[(int)ClassType.Magician, i] = 0; // sure
                criticalHitRate[(int)ClassType.Magician, i] = 0; // sure
                criticalHit[(int)ClassType.Magician, i] = 0; // sure
                criticalDistRate[(int)ClassType.Magician, i] = 0; // sure
                criticalDist[(int)ClassType.Magician, i] = 0; // sure

                minDist[(int)ClassType.Magician, i] = 14 + i; // approx
                maxDist[(int)ClassType.Magician, i] = 14 + i; // approx
                distRate[(int)ClassType.Magician, i] = (14 + i) * 2; // approx
                hitDef[(int)ClassType.Magician, i] = (i + 11) / 2; // approx
                magicalDef[(int)ClassType.Magician, i] = i + 4; // approx
                hitDodge[(int)ClassType.Magician, i] = 24 + i; // approx
                distDodge[(int)ClassType.Magician, i] = 14 + i; // approx

                minHit[(int)ClassType.Magician, i] = 2 * i + 9; // approx Numbers n such that n^2 is of form x^ 2 + 40y ^ 2 with positive x,y.
                maxHit[(int)ClassType.Magician, i] = 2 * i + 9; // approx Numbers n such that n^2 is of form x^2+40y^2 with positive x,y.
                distDef[(int)ClassType.Magician, i] = 20 + i; // approx

                // ARCHER
                criticalHitRate[(int)ClassType.Archer, i] = 0; // sure
                criticalHit[(int)ClassType.Archer, i] = 0; // sure
                criticalDistRate[(int)ClassType.Archer, i] = 0; // sure
                criticalDist[(int)ClassType.Archer, i] = 0; // sure

                minHit[(int)ClassType.Archer, i] = 9 + i * 3; // approx
                maxHit[(int)ClassType.Archer, i] = 9 + i * 3; // approx
                var add = (i % 2) == 0 ? 2 : 4;
                hitRate[(int)ClassType.Archer, 1] = 41;
                hitRate[(int)ClassType.Archer, i] = hitRate[(int)ClassType.Archer, i] + add; // approx
                minDist[(int)ClassType.Archer, i] = 2 * i; // approx
                maxDist[(int)ClassType.Archer, i] = 2 * i; // approx

                distRate[(int)ClassType.Archer, i] = 20 + 2 * i; // approx
                hitDef[(int)ClassType.Archer, i] = i; // approx
                magicalDef[(int)ClassType.Archer, i] = i + 2; // approx
                hitDodge[(int)ClassType.Archer, i] = 41 + i; // approx
                distDodge[(int)ClassType.Archer, i] = i + 2; // approx
                distDef[(int)ClassType.Archer, i] = i; // approx
            }
        }

        private void LoadXpData()
        {
            // Load XpData
            XpData = new double[256];
            double[] v = new double[256];
            double var = 1;
            v[0] = 540;
            v[1] = 960;
            XpData[0] = 300;
            for (int i = 2; i < v.Length; i++)
                v[i] = v[i - 1] + 420 + 120 * (i - 1);

            for (int i = 1; i < XpData.Length; i++)
            {
                if (i < 79)
                {
                    if (i == 14)
                    {
                        var = 6 / 3d;
                    }
                    else if (i == 39)
                    {
                        var = 19 / 3d;
                    }
                    else if (i == 59)
                    {
                        var = 70 / 3d;
                    }

                    XpData[i] = Convert.ToInt64(XpData[i - 1] + var * v[i - 1]);
                }

                if (i < 79)
                {
                    continue;
                }

                switch (i)
                {
                    case 79:
                        var = 5000;
                        break;

                    case 82:
                        var = 9000;
                        break;

                    case 84:
                        var = 13000;
                        break;
                }

                XpData[i] = Convert.ToInt64(XpData[i - 1] + var * (i + 2) * (i + 2));
            }
        }

        #endregion
    }
}