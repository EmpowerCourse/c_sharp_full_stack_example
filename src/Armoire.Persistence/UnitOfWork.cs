using Armoire.Common;
using NHibernate;
using System;

namespace Armoire.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ISession _session;
        private ITransaction _transaction = null;

        public UnitOfWork(ISession session)
        {
            _session = session;
        }

        private UnitOfWork() { }

        public bool IsInTransaction()
        {
            return _transaction != null;
        }

        public void Begin()
        {
            if (_transaction == null)
            {
                _transaction = _session.BeginTransaction();
            }
            else
            {
                throw new InvalidOperationException("Transaction already in progress");
            }
        }

        public void Commit()
        {
            if (_transaction != null)
            {
                _session.Flush();
                _transaction.Commit();
                _transaction.Dispose();
                _transaction = null;
            }
            else
            {
                throw new InvalidOperationException("No transaction in progress");
            }
        }

        public void Rollback()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction.Dispose();
                _transaction = null;
            }
            else
            {
                throw new InvalidOperationException("No transaction in progress");
            }
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                if (_transaction.IsActive && !(_transaction.WasCommitted || _transaction.WasRolledBack) && _session.IsOpen && _session.IsConnected)
                {
                    _transaction.Rollback();
                }
                _transaction.Dispose();
            }

            _session.Dispose();
        }
    }
}
