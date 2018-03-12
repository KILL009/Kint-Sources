using OpenNos.Core;
using OpenNos.DAL.EF.DB;
using System;
using System.Data;
using System.Data.Common;

namespace OpenNos.DAL.EF.Helpers
{
    public static class DataAccessHelper
    {
        #region Members

        private static OpenNosContext context;

        #endregion

        #region Properties

        private static OpenNosContext Context => context ?? (context = CreateContext());

        #endregion

        #region Methods

        /// <summary>
        /// Begins and returns a new transaction. Be sure to commit/rollback/dispose this transaction
        /// or use it in an using-clause.
        /// </summary>
        /// <returns>A new transaction.</returns>
        public static DbTransaction BeginTransaction()
        {
            // an open connection is needed for a transaction
            if (Context.Database.Connection.State == ConnectionState.Broken ||
                Context.Database.Connection.State == ConnectionState.Closed)
            {
                Context.Database.Connection.Open();
            }

            // begin and return new transaction
            return Context.Database.Connection.BeginTransaction();
        }

        /// <summary>
        /// Creates new instance of database context.
        /// </summary>
        public static OpenNosContext CreateContext() => new OpenNosContext();

        /// <summary>
        /// Disposes the current instance of database context.
        /// </summary>
        public static void DisposeContext()
        {
            if (context != null)
            {
                context.Dispose();
                context = null;
            }
        }

        public static bool Initialize()
        {
            using (OpenNosContext context = CreateContext())
            {
                try
                {
                    context.Database.Initialize(true);
                    context.Database.Connection.Open();
                    Logger.Log.Info(Language.Instance.GetMessageFromKey("DATABASE_INITIALIZED"));
                }
                catch (Exception ex)
                {
                    Logger.Log.Error("Database Error", ex);
                    Logger.Log.Error(Language.Instance.GetMessageFromKey("DATABASE_NOT_UPTODATE"));
                    return false;
                }

                return true;
            }
        }

        #endregion
    }
}