using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Armoire.Common;

namespace Armoire.Persistence
{
    public class NHibernateRepository<TEntity> : IRepository<TEntity>
        where TEntity : Entity
    {
        private readonly ISession _session;

        public NHibernateRepository(
            ISession session
        )
        {
            _session = session;
        }

        public TEntity Add(TEntity entity)
        {
            _session.Save(entity);
            return entity;
        }

        public void Delete(TEntity entity)
        {
            _session.Delete(entity);
        }

        public TEntity Get(int id)
        {
            return _session.Get<TEntity>(id);
        }

        //public IQueryOver<TEntity, TEntity> GetQueryOver()
        //{
        //    return _session.QueryOver<TEntity>();
        //}

        public IQueryable<TEntity> GetQuery()
        {
            return _session.Query<TEntity>();
        }

        public TEntity Update(TEntity entity)
        {
            _session.SaveOrUpdate(entity);
            return entity;
        }
    }
}
