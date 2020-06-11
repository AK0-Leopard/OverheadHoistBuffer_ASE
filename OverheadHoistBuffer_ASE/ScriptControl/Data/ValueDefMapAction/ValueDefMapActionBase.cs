//*********************************************************************************
//      MESDefaultMapAction.cs
//*********************************************************************************
// File Name: MESDefaultMapAction.cs
// Description: Type 1 Function
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using System;
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using NLog;
using com.mirle.ibg3k0.sc.Data.VO;
using System.Dynamic;
using System.Linq.Expressions;
using KingAOP;
using com.mirle.ibg3k0.sc.Data.TcpIp;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System.Linq;
using com.mirle.ibg3k0.sc.Common;
using System.Collections.Generic;
using com.mirle.iibg3k0.ttc.Common;

namespace com.mirle.ibg3k0.sc.Data.ValueDefMapAction
{
    /// <summary>
    /// Class ValueDefMapActionBase.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Data.ValueDefMapAction.IValueDefMapAction" />
    public class ValueDefMapActionBase : IValueDefMapAction
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The logger_ map actio log
        /// </summary>
        protected static Logger logger_MapActioLog = LogManager.GetLogger("MapActioLog");
        /// <summary>
        /// The sc application
        /// </summary>
        protected SCApplication scApp = null;
        /// <summary>
        /// The BCF application
        /// </summary>
        protected BCFApplication bcfApp = null;
        /// <summary>
        /// The eqpt
        /// </summary>
        protected AVEHICLE eqpt = null;
        //protected AVEHICLE eqpt = null;



        /// <summary>
        /// Initializes a new instance of the <see cref="ValueDefMapActionBase" /> class.
        /// </summary>
        public ValueDefMapActionBase()
        {
            scApp = SCApplication.getInstance();
            bcfApp = scApp.getBCFApplication();
        }

        /// <summary>
        /// Does the test eq.
        /// </summary>
        public void doTestEQ()
        {
            string vh_id = eqpt.VEHICLE_ID;
            logger_MapActioLog.Info("VEHICLE ID:{0}", vh_id);
        }



        /// <summary>
        /// Does the share memory initialize.
        /// </summary>
        /// <param name="runLevel">The run level.</param>
        public virtual void doShareMemoryInit(com.mirle.ibg3k0.bcf.App.BCFAppConstants.RUN_LEVEL runLevel) { }
        /// <summary>
        /// Does the initialize.
        /// </summary>
        public virtual void doInit()
        {
            try
            {
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
            }
        }
        /// <summary>
        /// Uns the register event.
        /// </summary>
        public virtual void unRegisterEvent() { }
        /// <summary>
        /// Sets the context.
        /// </summary>
        /// <param name="baseEQ">The base eq.</param>
        public virtual void setContext(BaseEQObject baseEQ) { }
        /// <summary>
        /// Gets the identity key.
        /// </summary>
        /// <returns>System.String.</returns>
        public virtual string getIdentityKey() { return this.GetType().Name; }
        public virtual void RegisteredTcpIpProcEvent() { }
        public virtual void UnRgisteredProcEvent() { }



        public virtual bool sned_Str1(ID_1_HOST_BASIC_INFO_VERSION_REP sned_gpp, out ID_101_HOST_BASIC_INFO_VERSION_RESPONSE receive_gpp) { receive_gpp = null; return false; }
        public virtual bool sned_Str11(ID_11_BASIC_INFO_REP sned_gpp, out ID_111_BASIC_INFO_RESPONSE receive_gpp) { receive_gpp = null; return true; }
        public virtual bool sned_Str13(ID_13_TAVELLING_DATA_REP send_gpp, out ID_113_TAVELLING_DATA_RESPONSE receive_gpp) { receive_gpp = null; return true; }
        public virtual bool sned_Str15(ID_15_SECTION_DATA_REP send_gpp, out ID_115_SECTION_DATA_RESPONSE receive_gpp) { receive_gpp = null; return true; }
        public virtual bool sned_Str17(ID_17_ADDRESS_DATA_REP send_gpp, out ID_117_ADDRESS_DATA_RESPONSE receive_gpp) { receive_gpp = null; return true; }
        public virtual bool sned_Str19(ID_19_SCALE_DATA_REP send_gpp, out ID_119_SCALE_DATA_RESPONSE receive_gpp) { receive_gpp = null; return true; }
        public virtual bool sned_Str21(ID_21_CONTROL_DATA_REP send_gpp, out ID_121_CONTROL_DATA_RESPONSE receive_gpp) { receive_gpp = null; return true; }
        public virtual bool sned_Str23(ID_23_GUIDE_DATA_REP send_gpp, out ID_123_GUIDE_DATA_RESPONSE receive_gpp) { receive_gpp = null; return true; }

        public virtual bool sned_Str61(ID_61_INDIVIDUAL_UPLOAD_REQ send_gpp, out ID_161_INDIVIDUAL_UPLOAD_RESPONSE receive_gpp) { receive_gpp = null; return true; }
        public virtual bool sned_Str63(ID_63_INDIVIDUAL_CHANGE_REQ send_gpp, out ID_163_INDIVIDUAL_CHANGE_RESPONSE receive_gpp) { receive_gpp = null; return true; }
        public virtual bool sned_Str41(ID_41_MODE_CHANGE_REQ send_gpp, out ID_141_MODE_CHANGE_RESPONSE receive_gpp) { receive_gpp = null; return true; }
        public virtual bool send_Str43(ID_43_STATUS_REQUEST send_gpp, out ID_143_STATUS_RESPONSE receive_gpp) { receive_gpp = null; return true; }
        public virtual bool sned_Str45(ID_45_POWER_OPE_REQ send_gpp, out ID_145_POWER_OPE_RESPONSE receive_gpp) { receive_gpp = null; return true; }
        public virtual bool sned_Str91(ID_91_ALARM_RESET_REQUEST send_gpp, out ID_191_ALARM_RESET_RESPONSE receive_gpp) { receive_gpp = null; return true; }

        public virtual bool send_Str31(ID_31_TRANS_REQUEST send_gpp, out ID_131_TRANS_RESPONSE receive_gpp, out string reason) { reason = string.Empty; receive_gpp = null; return true; }
        public virtual bool send_Str35(ID_35_CARRIER_ID_RENAME_REQUEST sned_gpp, out ID_135_CARRIER_ID_RENAME_RESPONSE receive_gpp) { receive_gpp = null; return true; }
        public virtual bool send_Str37(string cmd_id, CMDCancelType actType) { return true; }
        public virtual bool send_Str39(ID_39_PAUSE_REQUEST sned_gpp, out ID_139_PAUSE_RESPONSE receive_gpp) { receive_gpp = null; return true; }
        public virtual bool send_Str71(ID_71_RANGE_TEACHING_REQUEST sned_gpp, out ID_171_RANGE_TEACHING_RESPONSE receive_gpp) { receive_gpp = null; return true; }

        public virtual bool doDataSysc() { return true; }
        public virtual void doCatchPLCCSTInterfaceLog() { }


        protected virtual void Connection(object sender, TcpIpEventArgs e)
        {
            dynamic service = scApp.VehicleService;
            service.Connection(bcfApp, eqpt);
        }
        protected virtual void Disconnection(object sender, TcpIpEventArgs e)
        {
            dynamic service = scApp.VehicleService;
            service.Disconnection(bcfApp, eqpt);
        }

        public virtual bool snedMessage(WrapperMessage wrapper, bool isReply = false) { return true; }
        public virtual com.mirle.iibg3k0.ttc.Common.TrxTcpIp.ReturnCode snedRecv<TSource2>(WrapperMessage wrapper, out TSource2 stRecv, out string rtnMsg)
        {
            stRecv = default(TSource2);
            rtnMsg = string.Empty;
            return iibg3k0.ttc.Common.TrxTcpIp.ReturnCode.SendDataFail;
        }

        public virtual void PLC_Control_TrunOn() { throw new NotImplementedException(); }
        public virtual void PLC_Control_TrunOff() { throw new NotImplementedException(); }
        public virtual bool setVehicleControlItemForPLC(Boolean[] itmes) { throw new NotImplementedException(); }


    }
}
