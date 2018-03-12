using OpenNos.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenNos.DAL.EF
{
    public interface IGenericDAO<TEntity, TDTO> : IMappingBaseDAO where TEntity : class
    {
        #region Methods

        DeleteResult Delete(object entitykey);

        TDTO FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        SaveResult InsertOrUpdate(ref TDTO dto);

        SaveResult InsertOrUpdate(IEnumerable<TDTO> dtos);

        IEnumerable<TDTO> LoadAll();

        IEnumerable<TDTO> Where(Expression<Func<TEntity, bool>> predicate);

        #endregion
    }
}