using eProjectSemester3_1.Application.Context;
using eProjectSemester3_1.Application.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace eProjectSemester3_1.Application
{
    public partial class UnitOfWork : IDisposable
    {
        //http://msdn.microsoft.com/en-us/library/bb738523.aspx
        //http://stackoverflow.com/questions/815586/entity-framework-using-transactions-or-savechangesfalse-and-acceptallchanges

        private readonly AppDbContext _context;
        private readonly IDbTransaction _transaction;
        private readonly ObjectContext _objectContext;

        /// <summary>
        /// Constructor
        /// </summary>
        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            // In order to make calls that are overidden in the caching ef-wrapper, we need to use
            // transactions from the connection, rather than TransactionScope. 
            // This results in our call e.g. to commit() being intercepted 
            // by the wrapper so the cache can be adjusted.
            // This won't work with the dbcontext because it handles the connection itself, so we must use the underlying ObjectContext. 
            // http://blogs.msdn.com/b/diego/archive/2012/01/26/exception-from-dbcontext-api-entityconnection-can-only-be-constructed-with-a-closed-dbconnection.aspx
            _objectContext = ((IObjectContextAdapter)_context).ObjectContext;

            // Updating EF timeout taken from
            // http://stackoverflow.com/questions/6232633/entity-framework-timeouts
            //_objectContext.CommandTimeout = 3 * 60; // value in seconds

            if (_objectContext.Connection.State != ConnectionState.Open)
            {
                _objectContext.Connection.Open();
                _transaction = _objectContext.Connection.BeginTransaction();
            }
        }

        public void AutoDetectChangesEnabled(bool option)
        {
            _context.Configuration.AutoDetectChangesEnabled = option;
        }

        public void LazyLoadingEnabled(bool option)
        {
            _context.Configuration.LazyLoadingEnabled = option;
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Commit()
        {
            _context.SaveChanges();
            _transaction.Commit();
        }

        /// <summary>
        /// Commits the transcation and saves changes to the database.. Also clears the long term cache based on the starting cache keys from CacheConstants
        /// </summary>
        /// <param name="cacheStartsWithToClear"></param>
        /// <param name="cacheService"></param>
        public void Commit(List<string> cacheStartsWithToClear, CacheService cacheService)
        {
            Commit();
            cacheService.ClearStartsWith(cacheStartsWithToClear);
        }

        public void Rollback()
        {
            _transaction.Rollback();

            // http://blog.oneunicorn.com/2011/04/03/rejecting-changes-to-entities-in-ef-4-1/

            foreach (var entry in _context.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case System.Data.Entity.EntityState.Modified:
                        entry.State = System.Data.Entity.EntityState.Unchanged;
                        break;
                    case System.Data.Entity.EntityState.Added:
                        entry.State = System.Data.Entity.EntityState.Detached;
                        break;
                    case System.Data.Entity.EntityState.Deleted:
                        // Note - problem with deleted entities:
                        // When an entity is deleted its relationships to other entities are severed. 
                        // This includes setting FKs to null for nullable FKs or marking the FKs as conceptually null (don’t ask!) 
                        // if the FK property is not nullable. You’ll need to reset the FK property values to 
                        // the values that they had previously in order to re-form the relationships. 
                        // This may include FK properties in other entities for relationships where the 
                        // deleted entity is the principal of the relationship–e.g. has the PK 
                        // rather than the FK. I know this is a pain–it would be great if it could be made easier in the future, but for now it is what it is.
                        entry.State = System.Data.Entity.EntityState.Unchanged;
                        break;
                }
            }
        }

        public void Dispose()
        {
            if (_objectContext.Connection.State == ConnectionState.Open)
            {
                _objectContext.Connection.Close();
            }
        }

    }

    public class UnitOfWorkManager
    {
        private bool _isDisposed;
        private readonly AppDbContext _context;

        public UnitOfWorkManager(AppDbContext context)
        {
            _context = context;
            //http://www.entityframeworktutorial.net/code-first/automated-migration-in-code-first.aspx
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AppDbContext, Configuration>("AppDbContent"));
        }

        /// <summary>
        /// Provides an instance of a unit of work. This wrapping in the manager
        /// class helps keep concerns separated
        /// </summary>
        /// <returns></returns>
        public UnitOfWork NewUnitOfWork()
        {
            return new UnitOfWork(_context);
        }

        /// <summary>
        /// Make sure there are no open sessions.
        /// In the web app this will be called when the injected UnitOfWork manager
        /// is disposed at the end of a request.
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _context.Dispose();
                _isDisposed = true;
            }
        }
    }
}