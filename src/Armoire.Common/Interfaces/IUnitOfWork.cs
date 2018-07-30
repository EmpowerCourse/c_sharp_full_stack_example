using System;
using System.Collections.Generic;
using System.Text;

namespace Armoire.Common
{
    public interface IUnitOfWork : IDisposable
    {
        bool IsInTransaction();
        void Begin();
        void Commit();
        void Rollback();
    }
}
