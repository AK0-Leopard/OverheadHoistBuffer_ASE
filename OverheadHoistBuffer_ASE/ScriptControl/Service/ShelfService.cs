using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.BLL;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using com.mirle.ibg3k0.sc.Data.SECS.ASE;

namespace com.mirle.ibg3k0.sc.Service
{
    public class ShelfService
    {
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private SCApplication scApp = null;
        private ShelfDefBLL shelfDefBLL = null;

        public ShelfService()
        {

        }
        public void start(SCApplication _app)
        {
            scApp = _app;
            shelfDefBLL = _app.ShelfDefBLL;
        }


        public bool doUpdatePriority(string shelf_id, int priority)
        {
            bool isSuccess = true;
            string result = string.Empty;
            try
            {
                if (isSuccess)
                {
                    using (TransactionScope tx = SCUtility.getTransactionScope())
                    {
                        using (DBConnection_EF con = DBConnection_EF.GetUContext())
                        {
                            isSuccess = shelfDefBLL.updatePriority(shelf_id, priority);
                            if (isSuccess)
                            {
                                tx.Complete();
                                //scApp.PortStationBLL.OperateCatch.updatePriority(shelf_id, priority);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error(ex, "Execption:");
            }
            return isSuccess;
        }

        public bool doUpdateEnable(string shelf_id, bool enable)
        {
            bool isSuccess = true;
            string result = string.Empty;
            try
            {
                if (isSuccess)
                {
                    using (TransactionScope tx = SCUtility.getTransactionScope())
                    {
                        using (DBConnection_EF con = DBConnection_EF.GetUContext())
                        {
                            isSuccess = shelfDefBLL.updateEnable(shelf_id, enable);
                            if (isSuccess)
                            {
                                tx.Complete();
                                //scApp.PortStationBLL.OperateCatch.updatePriority(shelf_id, priority);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error(ex, "Execption:");
            }
            return isSuccess;
        }

		internal bool doUpdateState(string shelf_id, string state)
		{
            bool isSuccess = true;
            string result = string.Empty;
            try
            {
                if (isSuccess)
                {
                    using (TransactionScope tx = SCUtility.getTransactionScope())
                    {
                        using (DBConnection_EF con = DBConnection_EF.GetUContext())
                        {
                            isSuccess = shelfDefBLL.updateStatus(shelf_id, state);
                            if (isSuccess)
                            {
                                tx.Complete();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error(ex, "Execption:");
            }
            return isSuccess;
        }

        internal bool doUpdateRemark(string shelf_id, string remark)
        {
            bool isSuccess = true;
            string result = string.Empty;
            try
            {
                if (isSuccess)
                {
                    using (TransactionScope tx = SCUtility.getTransactionScope())
                    {
                        using (DBConnection_EF con = DBConnection_EF.GetUContext())
                        {
                            isSuccess = shelfDefBLL.updateRemark(shelf_id, remark);
                            if (isSuccess)
                            {
                                tx.Complete();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                logger.Error(ex, "Execption:");
            }
            return isSuccess;

        }
    }
}
