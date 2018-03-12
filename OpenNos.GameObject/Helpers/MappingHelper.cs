using OpenNos.Core;
using System.Collections.Generic;

namespace OpenNos.GameObject
{
    public class MappingHelper : Singleton<MappingHelper>
    {
        #region Members

        public Dictionary<int, int> GuriItemEffects = new Dictionary<int, int>
        {
            {859, 1343},
            {860, 1344},
            {861, 1344},
            {875, 1558},
            {876, 1559},
            {877, 1560},
            {878, 1560},
            {879, 1561},
            {880, 1561}
        };

        #endregion
    }
}