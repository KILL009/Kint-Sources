using OpenNos.Core;
using System;

namespace OpenNos.GameObject.Helpers
{
    public class MateHelper : Singleton<MateHelper>
    {
        #region Members

        private static MateHelper instance;

        #endregion

        #region Instantiation

        public MateHelper()
        {
            LoadXPData();
        }

        #endregion

        #region Properties

        public new static MateHelper Instance => instance ?? (instance = new MateHelper());

        public double[] XpData { get; private set; }

        #endregion

        #region Methods

        private void LoadXPData()
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