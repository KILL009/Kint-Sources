using OpenNos.Data;
using System.Collections.Generic;

namespace OpenNos.GameObject
{
    public class Card : CardDTO
    {
        #region Properties

        public List<BCard> BCards { get; set; }

        #endregion

        #region Methods

        public override void Initialize()
        {
        }

        #endregion
    }
}