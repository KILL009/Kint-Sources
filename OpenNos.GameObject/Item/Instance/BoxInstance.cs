using OpenNos.Data;
using System;

namespace OpenNos.GameObject
{
    public class BoxInstance : SpecialistInstance, IBoxInstance
    {
        #region Members

        private Random random;

        #endregion

        #region Instantiation

        public BoxInstance()
        {
            random = new Random();
        }

        public BoxInstance(Guid id)
        {
            Id = id;
            random = new Random();
        }

        #endregion

        #region Properties

        public short HoldingVNum { get; set; }

        #endregion
    }
}