using OpenNos.Data;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace OpenNos.GameObject
{
    public class Skill : SkillDTO
    {
        #region Instantiation

        public Skill()
        {
            Combos = new List<ComboDTO>();
            BCards = new ConcurrentBag<BCard>();
        }

        #endregion

        #region Properties

        public ConcurrentBag<BCard> BCards { get; set; }

        public List<ComboDTO> Combos { get; set; }

        #endregion

        #region Methods

        public override void Initialize()
        {
            // no custom stuff done for Skill
        }

        #endregion
    }
}