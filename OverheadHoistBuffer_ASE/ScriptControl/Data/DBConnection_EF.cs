using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.ObjectRelay;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using SqlProviderServices = System.Data.Entity.SqlServer.SqlProviderServices;

namespace com.mirle.ibg3k0.sc.Data
{
    public class DBConnection_EF : OHTC_DevEntities, IDisposable
    {
        //private stackalloc string 
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static Logger logger_EFSql = LogManager.GetLogger("EFSqlObserver");
        private static readonly string KEY_WORD_CONTEXT = "context";
        private static readonly string KEY_WORD_USING_LAYER = "UsingLayer";

        string lockKey = "";
        string currentReleaseKey = "";
        public static DBConnection_EF GetContext(out bool isnew)
        {

            //通过CallContext数据槽，可以实现线程类实例唯一的功能
            DBConnection_EF context = CallContext.GetData(KEY_WORD_CONTEXT) as DBConnection_EF;
            if (context == null)
            {
                context = new DBConnection_EF();
                CallContext.SetData(KEY_WORD_CONTEXT, context);
                isnew = true;
            }
            else
            {
                isnew = false;
            }
            //每次都新建上下文对象，在一次逻辑操作中，无法保证数据的正确性
            //DbContext context = new MyFirstEFEntities();
            return context;
        }

        public static DBConnection_EF GetUContext([CallerMemberName] string callerName = "", [CallerFilePath] string callerFilePath = "")
        {
            string keyWord = string.Concat(callerFilePath, ".", callerName);
            //通过CallContext数据槽，可以实现线程类实例唯一的功能
            DBConnection_EF context = CallContext.GetData(KEY_WORD_CONTEXT) as DBConnection_EF;
            if (context == null)
            {
                
                context = new DBConnection_EF(keyWord);
                CallContext.SetData(KEY_WORD_CONTEXT, context);

                int iUsingLayer = 0;
                CallContext.SetData(KEY_WORD_USING_LAYER, iUsingLayer);
            }
            else
            {
                object using_layer = CallContext.GetData(KEY_WORD_USING_LAYER);
                if (using_layer != null)
                {
                    int iUsingLayer = Convert.ToInt32(using_layer);
                    iUsingLayer++;
                    CallContext.SetData(KEY_WORD_USING_LAYER, iUsingLayer);
                }
            }
            //每次都新建上下文对象，在一次逻辑操作中，无法保证数据的正确性
            //DbContext context = new MyFirstEFEntities();
            return context;
        }

        public static DBConnection_EF GetContext()
        {

            //通过CallContext数据槽，可以实现线程类实例唯一的功能
            //DBConnection_EF context = CallContext.GetData(KEY_WORD_CONTEXT) as DBConnection_EF;
            //if (context == null)
            //{
            //    context = new DBConnection_EF();
            //    CallContext.SetData(KEY_WORD_CONTEXT, context);
            //}
            //else
            //{
            //}
            //每次都新建上下文对象，在一次逻辑操作中，无法保证数据的正确性
            //DbContext context = new MyFirstEFEntities();
            return new DBConnection_EF();
        }

        private DBConnection_EF(string key)
        {
            lockKey = key;
            Database.Log = sql => logger_EFSql.Debug(sql);
        }

        public DBConnection_EF()
        {

            //DBConnection_EF context = CallContext.GetData(KEY_WORD_CONTEXT) as DBConnection_EF;
            //if (context == null)
            //{
            //    context = this;
            //    CallContext.SetData(KEY_WORD_CONTEXT, context);
            //    Database.Log = sql => logger_EFSql.Debug(sql);
            //}
            //else
            //{
            //    throw new Exception("double creat instance DBConnection_EF");
            //}
            Database.Log = sql => logger_EFSql.Debug(sql);
            //Configuration.AutoDetectChangesEnabled = false;
            //Configuration.ValidateOnSaveEnabled = false;
        }


        public void Release([CallerMemberName] string callerName = "", [CallerFilePath] string callerFilePath = "")
        {
            //System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
            //string keyWord = string.Concat(callerFilePath, ".", callerName);
            //currentReleaseKey = keyWord;
        }

        public new void Dispose()
        {

            object using_layer = CallContext.GetData(KEY_WORD_USING_LAYER);
            if (using_layer != null)
            {
                int iUsingLayer = Convert.ToInt32(using_layer);
                if (iUsingLayer <= 0)
                {
                    CallContext.SetData(KEY_WORD_CONTEXT, null);
                    CallContext.SetData(KEY_WORD_USING_LAYER, null);
                    base.Dispose();
                }
                else
                {
                    iUsingLayer--;
                    CallContext.SetData(KEY_WORD_USING_LAYER, iUsingLayer);
                }
            }
            else
            {
                CallContext.SetData(KEY_WORD_CONTEXT, null);
                base.Dispose();
            }

            //if (SCUtility.isMatche(currentReleaseKey, lockKey) || SCUtility.isEmpty(lockKey))
            //{
            //    CallContext.SetData(KEY_WORD_CONTEXT, null);
            //    base.Dispose();
            //}

        }


        // Protected implementation of Dispose pattern.
        //bool disposed = false;
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        CallContext.SetData(KEY_WORD_CONTEXT, null);
        //        //this.Dispose();
        //        // Free any other managed objects here.
        //        //
        //    }
        //}

        /// <summary>
        /// The logger
        /// </summary>
        //private static Logger logger = LogManager.GetCurrentClassLogger();

        DbContextTransaction iTrx = null;

        /// <summary>
        /// Begin a unit of work and return the associated <c>ITransaction</c> object.
        /// </summary>
        public DbContextTransaction BeginTransaction()
        {
            if (iTrx == null
                || iTrx.UnderlyingTransaction.Connection == null)
            {
                iTrx = Database.BeginTransaction();
            }

            return iTrx;
        }

        /// <summary>
        /// Flush the associated <c>ISession</c> and end the unit of work.
        /// </summary>
        public void Commit()
        {
            try
            {
                if (iTrx == null)
                {

                    return;
                }
                else
                {
                    iTrx.Commit();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                throw ex;
            }
            finally
            {
                if (iTrx != null)
                {
                    iTrx.Dispose();
                    iTrx = null;
                }
            }


        }

        /// <summary>
        /// Flush the associated <c>ISession</c> and end the unit of work.
        /// </summary>
        public void Rollback()
        {
            try
            {
                if (iTrx == null)
                {

                    return;
                }
                else
                {
                    iTrx.Rollback();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                throw ex;
            }
            finally
            {
                if (iTrx != null)
                {
                    iTrx.Dispose();
                    iTrx = null;
                }
            }
        }

        /// <summary>
        /// Flush the associated <c>ISession</c> and end the unit of work.
        /// </summary>
        public void Close()
        {
            if (iTrx != null)
            {
                iTrx.Dispose();
                iTrx = null;
                //return;
            }
            this.Dispose();
            //iTrx.Dispose();
            //iTrx = null;
            //Not thing...
        }

        public override int SaveChanges()
        {
            int returnCode = 0;
            //var changedEntities = ChangeTracker.Entries();

            //foreach (var changedEntity in changedEntities)
            //{
            //    if (changedEntity.Entity is IDataGridUpdateNotify)
            //    {
            //        var entity = (IDataGridUpdateNotify)changedEntity.Entity;

            //        switch (changedEntity.State)
            //        {
            //            case EntityState.Added:
            //                entity.OnBeforeInsert();
            //                break;

            //            case EntityState.Modified:
            //                entity.OnBeforeUpdate();
            //                break;

            //        }
            //    }
            //}
            try
            {
                return returnCode = base.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                logger.Error(dbEx, "Exception");
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        logger.Error("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
                throw dbEx;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                throw ex;
            }

            //return returnCode;
        }


    }
    //public class MyConfiguration : DbConfiguration
    //{
    //    public MyConfiguration()
    //    {
    //        SetTransactionHandler(SqlProviderServices.ProviderInvariantName, () => new CommitFailureHandler());
    //        //SetExecutionStrategy(SqlProviderServices.ProviderInvariantName, () => new SqlAzureExecutionStrategy());
    //    }
    //}

}
