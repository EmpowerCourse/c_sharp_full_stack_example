using Armoire.Common;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using Armoire.Entities.Mappings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Armoire.Entities
{
    public class NhHelper
    {
        private static ISessionFactory _sessionFactory;
        private ITransaction _transaction;
        private readonly ISettingsService _settingsService;
        public ISession Session { get; private set; }

        public NhHelper(ISettingsService settingsService)
        {
            _settingsService = settingsService;

            InitialiseSessionFactory();

            Session = _sessionFactory.OpenSession();
        }

        public void BeginTransaction()
        {
            _transaction = Session.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                // commit transaction if there is one active
                if (_transaction != null && _transaction.IsActive)
                    _transaction.Commit();
            }
            catch
            {
                // rollback if there was an exception
                if (_transaction != null && _transaction.IsActive)
                    _transaction.Rollback();

                throw;
            }
            finally
            {
                Session.Dispose();
            }
        }

        public void Rollback()
        {
            try
            {
                if (_transaction != null && _transaction.IsActive)
                    _transaction.Rollback();
            }
            finally
            {
                Session.Dispose();
            }
        }

        private void InitialiseSessionFactory()
        {
            // We only want one copy of this
            if (_sessionFactory == null)
            {
                var firstConfig =
                Fluently.Configure()
                .Database
                (
                    MsSqlConfiguration.MsSql2008
                        .UseOuterJoin()
                        .ShowSql()
                        .FormatSql()
                        .ConnectionString(c => c.Is(_settingsService.GetStringValue("Database:ConnectionString"))))
                        .Mappings(m =>
                        {
                            m.FluentMappings
                                .Conventions
                                .Setup(x =>
                                {
                                    x.Add(AutoImport.Never());
                                })
                                .AddFromAssemblyOf<PatronMap>();

                        }
                );

                _sessionFactory =
                    firstConfig
                       .ExposeConfiguration(
                           c => c.SetProperty(
                               "current_session_context_class",
                               _settingsService.GetStringValue("Database:SessionContext")))
                   .BuildSessionFactory();
            }
        }
    }
}
