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
    public class PortStationService
    {

        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private SCApplication scApp = null;
        private ReportBLL reportBLL = null;
        private LineBLL lineBLL = null;
        private ALINE line = null;
        public PortStationService()
        {

        }
        public void start(SCApplication _app)
        {
            scApp = _app;
            reportBLL = _app.ReportBLL;
        }

        public bool doUpdatePortStationPriority(string portID, int priority)
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
                            isSuccess = scApp.PortStationBLL.OperateDB.updatePriority(portID, priority);
                            if (isSuccess)
                            {
                                tx.Complete();
                                scApp.PortStationBLL.OperateCatch.updatePriority(portID, priority);
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

        public bool doUpdatePortDefPriority(string portID, int priority)
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
                            isSuccess = scApp.PortDefBLL.updatePriority(portID, priority);
                            if (isSuccess)
                            {
                                tx.Complete();
                                scApp.PortStationBLL.OperateCatch.updatePriority(portID, priority);
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

        public bool doUpdatePortStatus(string portID, E_PORT_STATUS status)
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
                            isSuccess = scApp.PortStationBLL.OperateDB.updatePortStatus(portID, status);
                            if (isSuccess)
                            {
                                tx.Complete();
                                scApp.PortStationBLL.OperateCatch.updatePortStatus(portID, status);
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
        public bool doUpdatePortStationServiceStatus(string portID, int status)
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
                            isSuccess = scApp.PortStationBLL.OperateDB.updateServiceStatus(portID, status);
                            if (isSuccess)
                            {
                                tx.Complete();
                                scApp.PortStationBLL.OperateCatch.updateServiceStatus(portID, status);
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

        public bool doPortTypeChgeCommandByMCSCmdID(string change_port_id, int type)
        {
            var port = scApp.PortStationBLL.OperateDB.get(change_port_id);
            bool is_success = true;
            if ((int)port.PORT_STATUS == SECSConst.PortState_InService)
            {
                scApp.PortStationBLL.OperateDB.updatePortType(port, type);
            }
            return is_success;
        }
    }
}
