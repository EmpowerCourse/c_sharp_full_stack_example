using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Armoire.Common
{
    public interface IRepository<TEntity>
        where TEntity : Entity
    {
        TEntity Add(TEntity entity);
        void Delete(TEntity entity);
        TEntity Get(int id);
        TEntity Update(TEntity entity);
        // IQueryOver<TEntity, TEntity> GetQueryOver();
        IQueryable<TEntity> GetQuery();
    }
}
