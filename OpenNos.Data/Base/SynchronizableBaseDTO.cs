using System;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    public abstract class SynchronizableBaseDTO : MappingBaseDTO
    {
        #region Instantiation

        public SynchronizableBaseDTO()
        {
            Id = Guid.NewGuid(); //make unique
        }

        #endregion

        #region Properties

        [Key]
        public Guid Id { get; set; }

        #endregion

        #region Methods

        public override bool Equals(object obj) => ((SynchronizableBaseDTO)obj).Id == Id;

        public override int GetHashCode() => Id.GetHashCode();

        #endregion
    }
}