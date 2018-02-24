using System;

namespace OpenNos.GameObject.Helpers
{
    public class MateHelper
    {
        #region Instantiation

        #endregion

        #region Members

        private MateHelper()
        {
            LoadXpData();
            LoadPrimaryMpData();
            LoadSecondaryMpData();
            LoadHpData();
            LoadStats();
        }

        #endregion

        #region Properties

        public double[] HpData { get; private set; }

        // Race == 0
        public double[] PrimaryMpData { get; private set; }

        // Race == 2
        public double[] SecondaryMpData { get; private set; }

        public int[,] DamageData { get; private set; }

        public int[,] HitRateData { get; private set; }

        public int[,] MeleeDefenseData { get; private set; }

        public int[,] RangeDefenseData { get; private set; }

        public int[,] MagicDefenseData { get; private set; }

        public int[,] MeleeDefenseDodgeData { get; private set; }

        public int[,] RangeDefenseDodgeData { get; private set; }

        public double[] XpData { get; private set; }

        #endregion

        #region Methods

        private void LoadPrimaryMpData()
        {
            PrimaryMpData = new double[256];
            PrimaryMpData[0] = 10;
            PrimaryMpData[1] = 10;
            PrimaryMpData[2] = 15;

            int basup = 5;
            byte count = 0;
            bool isStable = true;
            bool isDouble = false;

            for (int i = 3; i < PrimaryMpData.Length; i++)
            {
                if (i % 10 == 1)
                {
                    PrimaryMpData[i] += PrimaryMpData[i - 1] + (basup * 2);
                    continue;
                }
                if (!isStable)
                {
                    basup++;
                    count++;

                    if (count == 2)
                    {
                        if (isDouble)
                        { isDouble = false; }
                        else
                        { isStable = true; isDouble = true; count = 0; }
                    }

                    if (count == 4)
                    { isStable = true; count = 0; }
                }
                else
                {
                    count++;
                    if (count == 2)
                    { isStable = false; count = 0; }
                }
                PrimaryMpData[i] = PrimaryMpData[i - (i % 10 == 2 ? 2 : 1)] + basup;
            }
        }

        private void LoadSecondaryMpData()
        {
            SecondaryMpData = new double[256];
            SecondaryMpData[0] = 60;
            SecondaryMpData[1] = 60;
            SecondaryMpData[2] = 78;

            int basup = 18;
            bool boostup = false;

            for (int i = 3; i < SecondaryMpData.Length; i++)
            {
                if (i % 10 == 1)
                {
                    SecondaryMpData[i] += SecondaryMpData[i - 1] + i + 10;
                    continue;
                }

                if (boostup)
                { basup += 3; boostup = false; }
                else
                { basup++; boostup = true; }

                SecondaryMpData[i] = SecondaryMpData[i - (i % 10 == 2 ? 2 : 1)] + basup;
            }
        }

        private void LoadHpData()
        {
            HpData = new double[256];
            int baseHp = 138;
            int hpBaseUp = 18;
            for (int i = 0; i < HpData.Length; i++)
            {
                HpData[i] = baseHp;
                hpBaseUp++;
                baseHp += hpBaseUp;

                if (i == 37)
                {
                    baseHp = 1765;
                    hpBaseUp = 65;
                }
                if (i < 41)
                {
                    continue;
                }
                if (((99 - i) % 8) == 0)
                {
                    hpBaseUp++;
                }
            }
        }

        private void LoadStats()
        {
            DamageData = new int[4, 256];
            HitRateData = new int[4, 256];
            MeleeDefenseData = new int[4, 256];
            MeleeDefenseDodgeData = new int[4, 256];
            RangeDefenseData = new int[4, 256];
            RangeDefenseDodgeData = new int[4, 256];
            MagicDefenseData = new int[4, 256];

            for (int i = 0; i < 256; i++)
            {
                // Default(0)
                DamageData[0, i] = i + 9; // approx
                HitRateData[0, i] = i + 9; // approx
                MeleeDefenseData[0, i] = i + (9 / 2); // approx
                MeleeDefenseDodgeData[0, i] = i + 9; // approx
                RangeDefenseData[0, i] = (i + 9) / 2; // approx
                RangeDefenseDodgeData[0, i] = i + 9; // approx
                MagicDefenseData[0, i] = (i + 9) / 2; // approx

                // Melee-Type Premium Pets
                MeleeDefenseDodgeData[1, i] = i + 12; // approx
                RangeDefenseDodgeData[1, i] = i + 12; // approx
                MagicDefenseData[1, i] = (i + 9) / 2; // approx
                HitRateData[1, i] = i + 27; // approx
                MeleeDefenseData[1, i] = i + 2; // approx

                DamageData[1, i] = (2 * i) + 5; // approx Numbers n such that 10n+9 is prime.
                RangeDefenseData[1, i] = i; // approx

                // Magic-Type Premium Pets
                HitRateData[2, i] = 0; // sure
                MeleeDefenseData[2, i] = (i + 11) / 2; // approx
                MagicDefenseData[2, i] = i + 4; // approx
                MeleeDefenseDodgeData[2, i] = 24 + i; // approx
                RangeDefenseDodgeData[2, i] = 14 + i; // approx
                DamageData[2, i] = (2 * i) + 9; // approx Numbers n such that n^2 is of form x^ 2 + 40y ^ 2 with positive x,y.
                RangeDefenseData[2, i] = 20 + i; // approx

                // Range-Type Premium Pets
                DamageData[3, i] = 9 + (i * 3); // approx
                HitRateData[3, 1] = 41 + (i % 2 == 0 ? 2 : 4);
                MeleeDefenseData[3, i] = i; // approx
                MagicDefenseData[3, i] = i + 2; // approx
                MeleeDefenseDodgeData[3, i] = 41 + i; // approx
                RangeDefenseDodgeData[3, i] = i + 2; // approx
                RangeDefenseData[3, i] = i; // approx
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
            {
                v[i] = v[i - 1] + 420 + (120 * (i - 1));
            }
            for (int i = 1; i < XpData.Length; i++)
            {
                if (i < 79)
                {
                    switch (i)
                    {
                        case 14:
                            var = 6 / 3d;
                            break;
                        case 39:
                            var = 19 / 3d;
                            break;
                        case 59:
                            var = 70 / 3d;
                            break;
                    }
                    XpData[i] = Convert.ToInt64(XpData[i - 1] + (var * v[i - 1]));
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
                XpData[i] = Convert.ToInt64(XpData[i - 1] + (var * (i + 2) * (i + 2)));
            }
        }

        #endregion

        private static MateHelper _instance;

        public static MateHelper Instance => _instance ?? (_instance = new MateHelper());
    }
}