//*********************************************************************************
//      MESDefaultMapAction.cs
//*********************************************************************************
// File Name: MESDefaultMapAction.cs
// Description: 與EAP通訊的劇本
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2019/10/08    Kevin Wei      N/A            A0.01   Bug fix:修正在手動點擊Force finish時，
//                                                     CST ID填成CMD ID的問題。
// 2019/10/09    Kevin Wei      N/A            A0.02   修改Confirm route的return code填入不正確的問題。
// 2019/10/11    Kevin Wei      N/A            A0.03   增加搬送命令狀態是Pre initial 的時候，無法接受Cancel命令。
//**********************************************************************************
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.BLL;
using com.mirle.ibg3k0.sc.Common;
//using com.mirle.ibg3k0.sc.Data.SECS.CSOT;
using com.mirle.ibg3k0.sc.Data.SECSDriver;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.stc.Common;
using com.mirle.ibg3k0.stc.Data.SecsData;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Transactions;
using com.mirle.ibg3k0.sc.Data.SECS.ASE;
using System.Reflection;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.ValueDefMapAction
{
    public class ASEMCSDefaultMapAction : IBSEMDriver, IValueDefMapAction
    {
        const string DEVICE_NAME_MCS = "MCS";
        const string CALL_CONTEXT_KEY_WORD_SERVICE_ID_MCS = "MCS Service";

        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static Logger GlassTrnLogger = LogManager.GetLogger("GlassTransferRpt_EAP");
        protected static Logger logger_MapActionLog = LogManager.GetLogger("MapActioLog");

        private static Logger TransferServiceLogger = LogManager.GetLogger("TransferServiceLogger");

        private ReportBLL reportBLL = null;

        /// <summary>
        /// 僅在測試階段使用
        /// </summary>
        protected bool isOnlineWithMcs = false;

        string log = "";

        public virtual string getIdentityKey()
        {
            return this.GetType().Name;
        }

        public virtual void setContext(BaseEQObject baseEQ)
        {
            this.line = baseEQ as ALINE;
        }
        public virtual void unRegisterEvent()
        {

        }
        public virtual void doShareMemoryInit(BCFAppConstants.RUN_LEVEL runLevel)
        {
            try
            {
                switch (runLevel)
                {
                    case BCFAppConstants.RUN_LEVEL.ZERO:
                        SECSConst.setDicCEIDAndRPTID(scApp.CEIDBLL.loadDicCEIDAndRPTID());
                        SECSConst.setDicRPTIDAndVID(scApp.CEIDBLL.loadDicRPTIDAndVID());
                        break;
                    case BCFAppConstants.RUN_LEVEL.ONE:
                        break;
                    case BCFAppConstants.RUN_LEVEL.TWO:
                        break;
                    case BCFAppConstants.RUN_LEVEL.NINE:
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
            }
        }
        public virtual void doInit()
        {
            string eapSecsAgentName = scApp.EAPSecsAgentName;
            reportBLL = scApp.ReportBLL;

            ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S1F1", S1F1ReceiveAreYouThere);
            ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S1F3", S1F3ReceiveSelectedEquipmentStatusRequest);

            ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S1F13", S1F13ReceiveEstablishCommunicationRequest);
            ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S1F17", S1F17ReceiveRequestOnLine);

            ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S2F13", S2F13ReceiveEquipmentConstantRequest);
            ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S2F15", S2F15ReceiveNewEquipmentConstantSend);
            ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S2F31", S2F31ReceiveDateTimeSetReq);
            ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S2F33", S2F33ReceiveDefineReport);
            ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S2F35", S2F35ReceiveLinkEventReport);
            ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S2F37", S2F37ReceiveEnableDisableEventReport);
            ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S2F41", S2F41ReceiveHostCommand);
            ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S2F49", S2F49ReceiveEnhancedRemoteCommandExtension);

            ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S5F3", S5F3ReceiveEnableDisableAlarm);

            //ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S1F3", S1F3ReceiveSelectedEquipmentStatusRequest);
            //ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S1F13", S1F13ReceiveEstablishCommunicationRequest);
            //ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S1F15", S1F15ReceiveRequestOffLine);
            //ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S1F17", S1F17ReceiveRequestOnLine);

            //ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S2F17", S2F17ReceiveDateAndTimeRequest);
            //ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S2F31", S2F31ReceiveDateTimeSetReq);
            //ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S2F33", S2F33ReceiveDefineReport);
            //ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S2F35", S2F35ReceiveLinkEventReport);
            //ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S2F37", S2F37ReceiveEnableDisableEventReport);
            //ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S2F41", S2F41ReceiveHostCommand);

            //ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S5F3", S5F3ReceiveEnableDisableAlarm);
            //ISECSControl.addSECSReceivedHandler(bcfApp, eapSecsAgentName, "S5F5", S5F5ReceiveListAlarmRequest);

            ISECSControl.addSECSConnectedHandler(bcfApp, eapSecsAgentName, secsConnected);
            ISECSControl.addSECSDisconnectedHandler(bcfApp, eapSecsAgentName, secsDisconnected);
        }
        #region Receive 

        //protected override void S2F17ReceiveDateAndTimeRequest(object sender, SECSEventArgs e)
        //{
        //    try
        //    {
        //        S2F17 s2f17 = ((S2F17)e.secsHandler.Parse<S2F17>(e));
        //        SCUtility.secsActionRecordMsg(scApp, true, s2f17);
        //        if (isProcess(s2f17)) { return; }

        //        S2F18 s2f18 = null;
        //        s2f18 = new S2F18();
        //        s2f18.SystemByte = s2f17.SystemByte;
        //        s2f18.SECSAgentName = scApp.EAPSecsAgentName;
        //        s2f18.TIME = DateTime.Now.ToString(SCAppConstants.TimestampFormat_16);

        //        TrxSECS.ReturnCode rtnCode = ISECSControl.replySECS(bcfApp, s2f18);
        //        SCUtility.secsActionRecordMsg(scApp, false, s2f18);
        //        if (rtnCode != TrxSECS.ReturnCode.Normal)
        //        {
        //            logger.Warn("Reply EQPT S2F18 Error:{0}", rtnCode);
        //        }


        //        ////當收到S2F17如果TSC_State是在NONE, 之後再接續進行Auto Initial
        //        //if (line.TSC_state_machine.State == ALINE.TSCState.NONE)
        //        //    scApp.LineService.TSCStateToPause();


        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}", line.LINE_ID, "S2F17_Receive_Date_Time_Req", ex.ToString());
        //    }
        //}

        protected void S2F13ReceiveEquipmentConstantRequest(object sender, SECSEventArgs e)
        {
            try
            {
                S2F13 s2f13 = ((S2F13)e.secsHandler.Parse<S2F13>(e));
                SCUtility.secsActionRecordMsg(scApp, true, s2f13);
                //if (isProcess(s2f13)) { return; }

                scApp.TransferService.TransferServiceLogger.Info(
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|s2f13:\n" + s2f13.toSECSString());

                S2F14 s2f14 = null;
                s2f14 = new S2F14();

                s2f14.ECVS = new string[s2f13.ECIDS.Length];
                s2f14.SystemByte = s2f13.SystemByte;
                s2f14.SECSAgentName = scApp.EAPSecsAgentName;

                for (int i = 0; i < s2f13.ECIDS.Length; i++)
                {
                    if (s2f13.ECIDS[i] == "61")
                    {
                        s2f14.ECVS[i] = line.LINE_ID;
                    }
                    else
                    {
                        s2f14.ECVS[i] = "0";
                    }
                }

                TrxSECS.ReturnCode rtnCode = ISECSControl.replySECS(bcfApp, s2f14);
                SCUtility.secsActionRecordMsg(scApp, false, s2f14);

                scApp.TransferService.TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "MCS >> OHB|s2f14 SCES_ReturnCode:" + rtnCode);

                if (rtnCode != TrxSECS.ReturnCode.Normal)
                {
                    logger.Warn("Reply EQPT S2F16 Error:{0}", rtnCode);
                }

                //scApp.CEIDBLL.DeleteRptInfoByBatch();

                //if (s2f33.RPTITEMS != null && s2f33.RPTITEMS.Length > 0)
                //    scApp.CEIDBLL.buildReportIDAndVid(s2f33.ToDictionary());



                //SECSConst.setDicRPTIDAndVID(scApp.CEIDBLL.loadDicRPTIDAndVID());

            }
            catch (Exception ex)
            {
                scApp.TransferService.TransferServiceLogger.Error(
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "  S2F13ReceiveEquipmentConstantRequest\n" + ex);

                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}", line.LINE_ID, "S2F17_Receive_Date_Time_Req", ex.ToString());
            }
        }

        protected void S2F15ReceiveNewEquipmentConstantSend(object sender, SECSEventArgs e)
        {
            try
            {
                S2F15 s2f15 = ((S2F15)e.secsHandler.Parse<S2F15>(e));
                SCUtility.secsActionRecordMsg(scApp, true, s2f15);
                //if (isProcess(s2f15)) { return; }

                scApp.TransferService.TransferServiceLogger.Info(
                                DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|s2f15:\n" + s2f15.toSECSString());

                S2F16 s2f16 = null;
                s2f16 = new S2F16();
                s2f16.SystemByte = s2f15.SystemByte;
                s2f16.SECSAgentName = scApp.EAPSecsAgentName;
                s2f16.EAC = "0";

                TrxSECS.ReturnCode rtnCode = ISECSControl.replySECS(bcfApp, s2f16);
                SCUtility.secsActionRecordMsg(scApp, false, s2f16);

                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "MCS >> OHB|s2f16 EAC:" + s2f16.EAC + "   SCES_ReturnCode:" + rtnCode);

                if (rtnCode != TrxSECS.ReturnCode.Normal)
                {
                    logger.Warn("Reply EQPT S2F16 Error:{0}", rtnCode);
                }

                //scApp.CEIDBLL.DeleteRptInfoByBatch();

                //if (s2f33.RPTITEMS != null && s2f33.RPTITEMS.Length > 0)
                //    scApp.CEIDBLL.buildReportIDAndVid(s2f33.ToDictionary());



                //SECSConst.setDicRPTIDAndVID(scApp.CEIDBLL.loadDicRPTIDAndVID());

            }
            catch (Exception ex)
            {
                scApp.TransferService.TransferServiceLogger.Error(
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "  S2F15ReceiveNewEquipmentConstantSend\n" + ex);

                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}", line.LINE_ID, "S2F17_Receive_Date_Time_Req", ex.ToString());
            }
        }
        protected override void S2F33ReceiveDefineReport(object sender, SECSEventArgs e)
        {
            try
            {
                S2F33 s2f33 = ((S2F33)e.secsHandler.Parse<S2F33>(e));
                SCUtility.secsActionRecordMsg(scApp, true, s2f33);
                //if (isProcess(s2f33)) { return; }

                TransferServiceLogger.Info(
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|s2f33:\n" + s2f33.toSECSString());

                S2F34 s2f34 = null;
                s2f34 = new S2F34();
                s2f34.SystemByte = s2f33.SystemByte;
                s2f34.SECSAgentName = scApp.EAPSecsAgentName;
                s2f34.DRACK = "0";

                TrxSECS.ReturnCode rtnCode = ISECSControl.replySECS(bcfApp, s2f34);
                SCUtility.secsActionRecordMsg(scApp, false, s2f34);

                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "MCS >> OHB|s2f34 DRACK:" + s2f34.DRACK + "   SCES_ReturnCode:" + rtnCode);

                if (rtnCode != TrxSECS.ReturnCode.Normal)
                {
                    logger.Warn("Reply EQPT S2F18 Error:{0}", rtnCode);
                }

                if (s2f33.RPTITEMS.Length == 0)
                {
                    scApp.CEIDBLL.DeleteRptInfoByBatch();
                }

                if (s2f33.RPTITEMS != null && s2f33.RPTITEMS.Length > 0)
                {
                    scApp.CEIDBLL.buildReportIDAndVid(s2f33.ToDictionary());
                    SECSConst.setDicRPTIDAndVID(scApp.CEIDBLL.loadDicRPTIDAndVID());
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "  S2F33ReceiveDefineReport\n" + ex);

                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}", line.LINE_ID, "S2F17_Receive_Date_Time_Req", ex.ToString());
            }
        }
        protected override void S2F35ReceiveLinkEventReport(object sender, SECSEventArgs e)
        {
            try
            {
                S2F35 s2f35 = ((S2F35)e.secsHandler.Parse<S2F35>(e));
                SCUtility.secsActionRecordMsg(scApp, true, s2f35);
                //if (isProcess(s2f35)) { return; }

                TransferServiceLogger.Info(
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|s2f35:\n" + s2f35.toSECSString());

                S2F36 s2f36 = null;
                s2f36 = new S2F36();
                s2f36.SystemByte = s2f35.SystemByte;
                s2f36.SECSAgentName = scApp.EAPSecsAgentName;
                s2f36.LRACK = "0";

                TrxSECS.ReturnCode rtnCode = ISECSControl.replySECS(bcfApp, s2f36);
                SCUtility.secsActionRecordMsg(scApp, false, s2f36);

                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "MCS >> OHB|s2f36 LRACK:" + s2f36.LRACK + "   SCES_ReturnCode:" + rtnCode);

                if (rtnCode != TrxSECS.ReturnCode.Normal)
                {
                    logger.Warn("Reply EQPT S2F18 Error:{0}", rtnCode);
                }

                scApp.CEIDBLL.DeleteCEIDInfoByBatch();

                if (s2f35.RPTITEMS != null && s2f35.RPTITEMS.Length > 0)
                    scApp.CEIDBLL.buildCEIDAndReportID(s2f35.ToDictionary());

                SECSConst.setDicCEIDAndRPTID(scApp.CEIDBLL.loadDicCEIDAndRPTID());

            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "  S2F35ReceiveLinkEventReport\n" + ex);

                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}", line.LINE_ID, "S2F17_Receive_Date_Time_Req", ex.ToString());
            }
        }
        protected override void S2F49ReceiveEnhancedRemoteCommandExtension(object sender, SECSEventArgs e)
        {
            try
            {
                if (scApp.getEQObjCacheManager().getLine().ServerPreStop)
                    return;
                string errorMsg = string.Empty;
                //string title = System.Text.ASCIIEncoding.ASCII.GetString((byte[])e.RawData);

                //Modify by Kevin 20200413 S2F49_TRANSFEREXT s2f49 = ((S2F49_TRANSFEREXT)e.secsHandler.Parse<S2F49_TRANSFEREXT>(e));
                //S2F49 s2f49 = ((S2F49)e.secsHandler.Parse<S2F49>(e));//Modify by Kevin 20200413 
                S2F49_TRANSFEREXT s2f49 = ((S2F49_TRANSFEREXT)e.secsHandler.Parse<S2F49_TRANSFEREXT>(e));
                switch (s2f49.RCMD)
                {
                    case "TRANSFEREXT":
                    case "TRANSFER":
                        S2F49_TRANSFEREXT s2f49_transfer = ((S2F49_TRANSFEREXT)e.secsHandler.Parse<S2F49_TRANSFEREXT>(e));
                        SCUtility.secsActionRecordMsg(scApp, true, s2f49_transfer);

                        S2F50 s2f50 = new S2F50();
                        s2f50.SystemByte = s2f49_transfer.SystemByte;
                        s2f50.SECSAgentName = scApp.EAPSecsAgentName;
                        s2f50.HCACK = SECSConst.HCACK_Confirm;

                        string cmdID = s2f49_transfer.REPITEMS.COMMINFO.COMMAINFO.COMMANDIDINFO.CommandID;
                        string priority = s2f49_transfer.REPITEMS.COMMINFO.COMMAINFO.PRIORITY.CPVAL;
                        string cstID = s2f49_transfer.REPITEMS.TRANINFO.CARRINFO.CARRIERIDINFO.CarrierID;
                        string boxID = s2f49_transfer.REPITEMS.TRANINFO.CARRINFO.BOXIDINFO.BoxID;
                        string source = s2f49_transfer.REPITEMS.TRANINFO.CARRINFO.SOUINFO.Source;
                        string dest = s2f49_transfer.REPITEMS.TRANINFO.CARRINFO.DESTINFO.Dest;
                        string lotID = s2f49_transfer.REPITEMS.CSTINFO.CARRINFO.LOTIDINFO.Lot_ID ?? "0";
                        string boxLoc = scApp.CassetteDataBLL.loadCassetteDataByBoxID(boxID)?.Carrier_LOC;
                        string rtnStr = "";
                        bool isFromVh = false;

                        if (string.IsNullOrWhiteSpace(cstID))
                        {
                            cstID = "";
                        }

                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ") +
                            "OHB >> MCS|S2F49:"
                            + "cmdID:" + cmdID
                            + "    priority:" + priority
                            + "    cstID:" + cstID
                            + "    boxID:" + boxID
                            + "    source:" + source
                            + "    dest:" + dest
                            + "    lotID:" + lotID
                            + "    cstLoc:" + boxLoc
                        );

                        SCUtility.RecodeReportInfo(s2f49, cmdID);

                        //檢查搬送命令

                        if (isHostReady())
                        {
                            s2f50.HCACK = scApp.CMDBLL.doCheckMCSCommand(cmdID, priority, cstID, boxID, lotID, ref source, ref dest, out rtnStr, out isFromVh);
                        }
                        else
                        {
                            s2f50.HCACK = SECSConst.HCACK_Not_Able_Execute;
                        }
                        //準備將命令存入資料庫中
                        using (TransactionScope tx = SCUtility.getTransactionScope())
                        {
                            using (DBConnection_EF con = DBConnection_EF.GetUContext())
                            {
                                bool isCreatScuess = true;
                                if (SCUtility.isMatche(SECSConst.HCACK_Confirm, s2f50.HCACK)    //!SCUtility.isMatche(SECSConst.HCACK_Rejected_Already_Requested, s2f50.HCACK
                                    //|| SCUtility.isMatche(SECSConst.HCACK_Confirm_Executed, s2f50.HCACK)
                                   )
                                {
                                    isCreatScuess &= scApp.CMDBLL.doCreatMCSCommand(cmdID, priority, "0", cstID, source, dest, boxID, lotID, boxLoc, s2f50.HCACK, isFromVh);
                                }

                                if (s2f50.HCACK == SECSConst.HCACK_Confirm)
                                {
                                    //isCreatScuess &= scApp.SysExcuteQualityBLL.creatSysExcuteQuality(cmdID, cstID, source, dest);//取消對於SystemQuality的紀錄
                                }

                                if (isCreatScuess)
                                {
                                    //回復MCS檢查結果
                                    TrxSECS.ReturnCode rtnCode = ISECSControl.replySECS(bcfApp, s2f50);
                                    SCUtility.secsActionRecordMsg(scApp, false, s2f50);
                                    SCUtility.RecodeReportInfo(s2f50, cmdID);
                                    if (rtnCode != TrxSECS.ReturnCode.Normal)
                                    {
                                        logger_MapActionLog.Warn("Reply EQPT S2F50) Error:{0}", rtnCode);
                                        TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|S2F50: Reply EQPT S2F50) Error:{0}", rtnCode);
                                        isCreatScuess = false;
                                    }
                                    else
                                    {
                                        TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|S2F50   HCACK:" + s2f50.HCACK);
                                    }
                                }
                                //DB新增交易完成
                                if (isCreatScuess)
                                {
                                    tx.Complete();
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }

                        if (s2f50.HCACK == SECSConst.HCACK_Confirm)
                        {
                            //scApp.CMDBLL.checkMCS_TransferCommand();
                        }
                        else
                        {
                            BCFApplication.onWarningMsg(rtnStr);
                        }
                        break;
                        //case "STAGE":
                        //    S2F49_STAGE s2f49_stage = ((S2F49_STAGE)e.secsHandler.Parse<S2F49_STAGE>(e));

                        //    S2F50 s2f50_stage = new S2F50();
                        //    s2f50_stage.SystemByte = s2f49_stage.SystemByte;
                        //    s2f50_stage.SECSAgentName = scApp.EAPSecsAgentName;
                        //    if (scApp.getEQObjCacheManager().getLine().SCStats == ALINE.TSCState.PAUSING
                        //        || scApp.getEQObjCacheManager().getLine().SCStats == ALINE.TSCState.PAUSED)
                        //    {
                        //        s2f50_stage.HCACK = SECSConst.HCACK_Rejected_Already_Requested;
                        //    }
                        //    else
                        //    {
                        //        s2f50_stage.HCACK = SECSConst.HCACK_Confirm;
                        //    }

                        //    string source_port_id = s2f49_stage.REPITEMS.TRANSFERINFO.CPVALUE.SOURCEPORT_CP.CPVAL_ASCII;
                        //    TrxSECS.ReturnCode rtnCode_stage = ISECSControl.replySECS(bcfApp, s2f50_stage);
                        //    SCUtility.secsActionRecordMsg(scApp, false, s2f50_stage);
                        //    SCUtility.RecodeReportInfo(s2f50_stage);

                        //    //TODO Stage
                        //    //將收下來的Stage命令先放到Redis上
                        //    //等待Timer發現後會將此命令取下來並下命令給車子去執行
                        //    //(此處將再考慮是要透過Timer或是開Thread來監控這件事)
                        //    if (s2f50_stage.HCACK == SECSConst.HCACK_Confirm)
                        //    {
                        //        //暫時取消Stage的流程。
                        //        //var port = scApp.MapBLL.getPortByPortID(source_port_id);
                        //        //AVEHICLE vh_test = scApp.VehicleBLL.findBestSuitableVhStepByStepFromAdr(port.ADR_ID, port.LD_VH_TYPE);
                        //        //scApp.VehicleBLL.callVehicleToMove(vh_test, port.ADR_ID);
                        //    }
                        //    break;
                }
            }
            catch (Exception ex)
            {
                scApp.TransferService.TransferServiceLogger.Error(
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "  S2F49ReceiveEnhancedRemoteCommandExtension\n" + ex);

                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}", line.LINE_ID, "S2F49_Receive_Remote_Command", ex);
            }
        }


        protected override void S2F41ReceiveHostCommand(object sender, SECSEventArgs e)
        {
            try
            {
                string commandHead = System.Text.ASCIIEncoding.ASCII.GetString((byte[])e.RawData);
                SXFY s2f41 = null;
                if (commandHead.Contains("SCAN"))
                {
                    s2f41 = ((S2F41_Scan)e.secsHandler.Parse<S2F41_Scan>(e));
                }
                else if (commandHead.Contains("PRIORITYUPDATE") || commandHead.Contains("PORTTYPECHG"))
                {
                    s2f41 = (S2F41_PriorityUpdate)e.secsHandler.Parse<S2F41_PriorityUpdate>(e);
                }
                else
                {
                    s2f41 = ((S2F41)e.secsHandler.Parse<S2F41>(e));
                }
                SCUtility.secsActionRecordMsg(scApp, true, s2f41);
                //if (!isProcessEAP(s2f37)) { return; }

                S2F42 s2f42 = null;
                s2f42 = new S2F42();
                s2f42.SystemByte = s2f41.SystemByte;
                s2f42.SECSAgentName = scApp.EAPSecsAgentName;
                string mcs_cmd_id = string.Empty;
                //判斷是否執行Cmd
                bool needToResume = false;
                bool needToPause = false;
                bool canCancelCmd = false;
                bool canAbortCmd = false;
                bool canUpdateCmd = false;
                bool canChangCmd = false;
                bool canRetryCmd = false;
                bool canInstallCmd = false;
                bool canRemoveCmd = false;
                bool canDisShelfCmd = false;
                bool canEnShelfCmd = false;
                bool canRenameCmd = false;
                bool canScanCmd = false;
                //CMD裡的資料
                string cancel_abort_cmd_id = string.Empty;
                string update_cmd_id = string.Empty;
                string change_port_id = string.Empty;
                string retry_cmd_id = string.Empty;
                //Install、Remove、Rename
                string carrier_id = string.Empty;
                string box_id = string.Empty;
                string carrier_loc = string.Empty;
                bool has_carrier = false;
                //disable、enable Shelf
                string shelf_id = string.Empty;
                bool shelf_enable = false;
                //================
                string PriorityUpdate = string.Empty;
                int portType = 0;
                log = "";

                switch (s2f41)
                {
                    case S2F41_Scan scan:
                        #region LogSave
                        log += "RCMD:" + scan.RCMD + "  ";
                        log += scan.REPITEMS.CARRIERID.CPNAME + ":";
                        log += scan.REPITEMS.CARRIERID.CPVAL_ASCII + "   ";
                        log += scan.REPITEMS.BOXID.CPNAME + ":";
                        log += scan.REPITEMS.BOXID.CPVAL_ASCII + "   ";
                        log += scan.REPITEMS.CARRIERLOC.CPNAME + ":";
                        log += scan.REPITEMS.CARRIERLOC.CPVAL_ASCII + "   ";
                        #endregion

                        var result_scan = checkHostCommandScan(scan);

                        canScanCmd = result_scan.isOK;
                        carrier_id = result_scan.cstID;
                        box_id = result_scan.boxID;
                        carrier_loc = result_scan.loc;
                        //canCancelCmd = cancel_check_result2.isOK;
                        s2f42.HCACK = result_scan.checkResult;
                        //cancel_abort_cmd_id = cancel_check_result2.cmdID;

                        break;
                    case S2F41_PriorityUpdate priority:
                        #region LogSave
                        log += "RCMD:" + priority.RCMD + "  ";
                        log += priority.REPITEMS.CommandID_CP.CPNAME + ":";
                        log += priority.REPITEMS.CommandID_CP.CPVAL_ASCII + "   ";
                        log += priority.REPITEMS.PRIORITY_CP.CPNAME + ":";
                        log += priority.REPITEMS.PRIORITY_CP.CPVAL_U2 + "   ";
                        #endregion

                        switch (priority.RCMD)
                        {
                            case SECSConst.RCMD_PriorityUpdate:
                                var update_check_result = checkHostCommandPriorityUpdate(priority);
                                canUpdateCmd = update_check_result.isOK;
                                s2f42.HCACK = update_check_result.checkResult;
                                update_cmd_id = update_check_result.cmdID;
                                PriorityUpdate = update_check_result.priority;
                                break;
                            case SECSConst.RCMD_PortTypeChange:
                                var typechg_check_result = checkHostCommandPortTypeChg(priority);
                                s2f42.HCACK = typechg_check_result.checkResult;
                                canChangCmd = typechg_check_result.isOK;
                                change_port_id = typechg_check_result.portID;
                                portType = typechg_check_result.type;
                                break;
                        }
                        break;
                    case S2F41 normal:
                        #region LogSave
                        log += "RCMD:" + normal.RCMD + "  ";

                        foreach (var v in normal.REPITEMS)
                        {
                            log += v.CPNAME + ":";
                            log += v.CPVAL + "   ";
                        }
                        #endregion
                        switch (normal.RCMD)
                        {
                            case SECSConst.RCMD_Resume:
                                if (line.TSC_state_machine.State == ALINE.TSCState.PAUSED || line.TSC_state_machine.State == ALINE.TSCState.PAUSING)
                                {
                                    s2f42.HCACK = SECSConst.HCACK_Confirm_Executed;
                                    needToResume = true;
                                }
                                else
                                {
                                    s2f42.HCACK = SECSConst.HCACK_Not_Able_Execute;
                                    needToResume = false;
                                }
                                break;
                            case SECSConst.RCMD_Cancel:
                                var cancel_check_result = checkHostCommandCancel(s2f41 as S2F41);
                                canCancelCmd = cancel_check_result.isOK;
                                s2f42.HCACK = cancel_check_result.checkResult;
                                cancel_abort_cmd_id = cancel_check_result.cmdID;
                                break;
                            case SECSConst.RCMD_Pause:
                                needToPause = true;
                                if (line.TSC_state_machine.State == ALINE.TSCState.AUTO)
                                {
                                    s2f42.HCACK = SECSConst.HCACK_Confirm_Executed;
                                    needToPause = true;
                                }
                                else
                                {
                                    s2f42.HCACK = SECSConst.HCACK_Not_Able_Execute;
                                    needToResume = false;
                                }
                                break;
                            case SECSConst.RCMD_Scan:
                                //TODO
                                break;
                            case SECSConst.RCMD_Abort:
                                var abort_check_result = checkHostCommandCancel(s2f41 as S2F41);
                                canAbortCmd = abort_check_result.isOK;
                                s2f42.HCACK = abort_check_result.checkResult;
                                cancel_abort_cmd_id = abort_check_result.cmdID;
                                break;
                            case SECSConst.RCMD_Retry:
                                //var abort_check_result = checkHostCommandRetry(s2f41 as S2F41);
                                //canAbortCmd = abort_check_result.isOK;
                                //s2f42.HCACK = abort_check_result.checkResult;
                                //cancel_abort_cmd_id = abort_check_result.cmdID;
                                break;
                            case SECSConst.RCMD_Install:
                                var install_check_result = checkHostCommandInstall(s2f41 as S2F41);
                                canInstallCmd = install_check_result.isOK;
                                s2f42.HCACK = install_check_result.checkResult;
                                has_carrier = install_check_result.hasCarrier;
                                carrier_id = install_check_result.CSTID;
                                box_id = install_check_result.BOXID;
                                carrier_loc = install_check_result.LOCID;
                                break;
                            case SECSConst.RCMD_Remove:
                                var remove_check_result = checkHostCommandRemove(s2f41 as S2F41);
                                canRemoveCmd = remove_check_result.isOK;
                                s2f42.HCACK = remove_check_result.checkResult;
                                has_carrier = remove_check_result.hasCarrier;
                                carrier_id = remove_check_result.CSTID;
                                box_id = remove_check_result.BOXID;
                                break;
                            case SECSConst.RCMD_DisableShelf:
                                var disable_check_result = checkHostCommandDisableShelf(s2f41 as S2F41);
                                canDisShelfCmd = disable_check_result.isOK;
                                s2f42.HCACK = disable_check_result.checkResult;
                                shelf_id = disable_check_result.shelfid;
                                shelf_enable = false;
                                break;
                            case SECSConst.RCMD_EnbleShelf:
                                var enable_check_result = checkHostCommandEnableShelf(s2f41 as S2F41);
                                canDisShelfCmd = enable_check_result.isOK;
                                s2f42.HCACK = enable_check_result.checkResult;
                                shelf_id = enable_check_result.shelfid;
                                shelf_enable = true;
                                break;
                            case SECSConst.RCMD_ReName:
                                var rename_check_result = checkHostCommandRename(s2f41 as S2F41);
                                canRenameCmd = rename_check_result.isOK;
                                s2f42.HCACK = rename_check_result.checkResult;
                                has_carrier = rename_check_result.hasCarrier;
                                carrier_id = rename_check_result.CSTID;
                                box_id = rename_check_result.BOXID;
                                carrier_loc = rename_check_result.LOCID;
                                break;
                            case SECSConst.RCMD_CARRIERLOTIDUPDATE:
                                var CARRIERLOTIDUPDATE_check_result = checkHostCommandRCMD_CARRIERLOTIDUPDATE(s2f41 as S2F41);
                                s2f42.HCACK = CARRIERLOTIDUPDATE_check_result.checkResult;
                                break;
                        }
                        break;
                }

                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|S2F41 " + log);

                if (!isHostReady())
                {
                    s2f42.HCACK = SECSConst.HCACK_Not_Able_Execute;
                };

                TrxSECS.ReturnCode rtnCode = ISECSControl.replySECS(bcfApp, s2f42);
                SCUtility.secsActionRecordMsg(scApp, false, s2f42);
                if (rtnCode != TrxSECS.ReturnCode.Normal)
                {
                    logger.Warn("Reply EQPT S2F18 Error:{0}", rtnCode);
                }
                else
                {
                    TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|S2F42 HCACK:" + s2f42.HCACK);
                }

                if (!isHostReady() || rtnCode != TrxSECS.ReturnCode.Normal)
                {
                    return;
                };

                if (needToResume)
                {
                    reportBLL.ReportTSCAutoInitiated();
                    line.AGVCInitialComplete(reportBLL);
                    line.ResumeToAuto(reportBLL);
                }
                if (needToPause)
                {
                    //line.RequestToPause(reportBLL);
                    scApp.LineService.TSCStateToPause(SECSConst.PAUSE_REASON_MCSRequest);
                }
                if (canCancelCmd)
                {
                    scApp.VehicleService.doCancelOrAbortCommandByMCSCmdID(cancel_abort_cmd_id, ProtocolFormat.OHTMessage.CMDCancelType.CmdCancel);
                }
                if (canAbortCmd)
                {
                    AVEHICLE ohtData = scApp.VehicleBLL.loadAllVehicle().Where(data => (data?.MCS_CMD.Trim() ?? "") == cancel_abort_cmd_id).FirstOrDefault();

                    var cancel_check_result1 = checkHostCommandAbort(s2f41 as S2F41);

                    if (ohtData != null)
                    {
                        scApp.VehicleService.doCancelOrAbortCommandByMCSCmdID(cancel_abort_cmd_id, ProtocolFormat.OHTMessage.CMDCancelType.CmdAbort);
                    }
                    else
                    {
                        scApp.ReportBLL.ReportTransferAbortInitiated(cancel_abort_cmd_id);
                        scApp.ReportBLL.ReportTransferAbortCompleted(cancel_abort_cmd_id);
                        scApp.CMDBLL.updateCMD_MCS_TranStatus(cancel_abort_cmd_id, E_TRAN_STATUS.TransferCompleted);
                    }
                }
                if (canUpdateCmd)
                {
                    scApp.VehicleService.doPriorityUpdateCommandByMCSCmdID(update_cmd_id, PriorityUpdate);
                }
                if (canChangCmd)
                {
                    if (scApp.TransferService.isUnitType(change_port_id, Service.UnitType.AGV))
                    {
                        scApp.TransferService.ReportNowPortType(change_port_id);
                    }
                    else
                    {
                        scApp.TransferService.PortTypeChange(change_port_id, (E_PortType)portType, "S2F41");
                    }
                }
                if (canInstallCmd)
                {
                    scApp.TransferService.OHBC_InsertCassette(carrier_id, box_id, carrier_loc, "S2F41");
                    //scApp.VehicleService.doInstallCommandByMCSCmdID(has_carrier, carrier_id, box_id, carrier_loc);
                }
                if (canRemoveCmd)
                {
                    //scApp.VehicleService.doRemoveCommandByMCSCmdID(has_carrier, carrier_id, box_id);
                    //scApp.ReportBLL.ReportCarrierRemovedCompleted(carrier_id, box_id);
                    scApp.TransferService.DeleteCst(carrier_id, box_id, "S2F41");
                    //scApp.CassetteDataBLL.DeleteCSTbyCstBoxID(carrier_id, box_id);
                }
                if (canDisShelfCmd)
                {
                    scApp.VehicleService.doChgEnableShelfCommand(shelf_id, shelf_enable);
                }
                if (canEnShelfCmd)
                {
                    scApp.VehicleService.doChgEnableShelfCommand(shelf_id, shelf_enable);
                }
                if (canRenameCmd)
                {
                    scApp.VehicleService.doInstallCommandByMCSCmdID(has_carrier, carrier_id, box_id, carrier_loc);
                }
                if (canScanCmd)
                {
                    scApp.TransferService.SetScanCmd(carrier_id, box_id, carrier_loc);
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error("OHB >> MCS|S2F41 " + log + "\n" + ex);
                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}", line.LINE_ID, "S2F17_Receive_Date_Time_Req", ex.ToString());
            }
        }

        private (bool isOK, string checkResult, string cstID, string boxID, string loc) checkHostCommandScan(S2F41_Scan s2F41)
        {
            bool is_ok = false;
            string check_result = SECSConst.HCACK_Param_Invalid;
            //string command_id = string.Empty;
            //var command_id_item = s2F41.REPITEMS.Where(item => SCUtility.isMatche(item.CPNAME, SECSConst.CPNAME_CommandID)).FirstOrDefault();

            string cstID = s2F41.REPITEMS.CARRIERID.CPVAL_ASCII ?? "";
            string boxID = s2F41.REPITEMS.BOXID.CPVAL_ASCII ?? "";
            string loc = s2F41.REPITEMS.CARRIERLOC.CPVAL_ASCII ?? "";

            //優先順序：location > box ID > cassette ID
            CassetteData cstData;
            if (!string.IsNullOrWhiteSpace(loc))
            {
                cstData = scApp.CassetteDataBLL.loadCassetteDataByLoc(loc);

                if (cstData != null)
                {
                    boxID = cstData.BOXID;
                    cstID = cstData.CSTID;
                }
            }
            else if (!string.IsNullOrWhiteSpace(boxID))
            {
                cstData = scApp.CassetteDataBLL.loadCassetteDataByBoxID(boxID);

                if (cstData != null)
                {
                    loc = cstData.Carrier_LOC;
                    cstID = cstData.CSTID;
                }
            }
            else if (!string.IsNullOrWhiteSpace(cstID))
            {
                if(string.IsNullOrWhiteSpace(cstID) == false)
                {
                    cstData = scApp.CassetteDataBLL.loadCassetteDataByCSTID(cstID);

                    if (cstData != null)
                    {
                        loc = cstData.Carrier_LOC;
                        boxID = cstData.BOXID;
                    }
                }
            }
            else //位置對象都沒有，不能判斷要scan誰
            {
                check_result = SECSConst.HCACK_Param_Invalid;
                return (is_ok, check_result, cstID, boxID, loc);
            }
            
            if (scApp.TransferService.isShelfPort(loc))  //SCAN 只能針對儲位
            {
                ShelfDef shelfData = scApp.ShelfDefBLL.loadShelfDataByID(loc);

                if (shelfData != null)
                {
                    if (shelfData.Enable.Trim() == "Y")
                    {
                        is_ok = true;
                        check_result = SECSConst.HCACK_Confirm;
                    }
                    else
                    {
                        check_result = SECSConst.HCACK_Not_Able_Execute;
                    }
                }
            }
            else
            {
                check_result = SECSConst.HCACK_Obj_Not_Exist;
            }

            //var command_id = s2F41.REPITEMS.CARRIERID.CPVAL_ASCII;
            //if (command_id == null)
            //{

            //}
            //else if (command_id != null)
            //{
            ////    command_id = command_id_item.CPVAL;
            //    ACMD_MCS cmd_mcs = scApp.CMDBLL.getCMD_MCSByID(command_id);

            //    if (cmd_mcs != null)
            //    {
            //        //if (cmd_mcs.TRANSFERSTATE < E_TRAN_STATUS.Transferring)
            //        //如果命令在
            //        if (cmd_mcs.COMMANDSTATE < ACMD_MCS.COMMAND_STATUS_BIT_INDEX_LOAD_COMPLETE ||
            //            cmd_mcs.COMMANDSTATE >= ACMD_MCS.COMMAND_STATUS_BIT_INDEX_UNLOAD_ARRIVE)
            //        {
            //            check_result = SECSConst.HCACK_Not_Able_Execute;
            //            is_ok = false;
            //            //string current_status = ACMD_MCS.COMMAND_STATUS_BIT_To_String(cmd_mcs.COMMANDSTATE);
            //            LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
            //               Data: $"Process mcs command [Abort] can't excute, because mcs command id:{command_id} current command status:{cmd_mcs.COMMANDSTATE}",
            //               XID: command_id);
            //        }
            //    }
            //    else
            //    {
            //        check_result = SECSConst.HCACK_Obj_Not_Exist;
            //        is_ok = false;
            //        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
            //           Data: $"Process mcs command [Abort] can't excute, because mcs command id:{command_id} not exist",
            //           XID: command_id);
            //    }
            //}
            //else
            //{
            //    check_result = SECSConst.HCACK_Param_Invalid;
            //    is_ok = false;
            //}
            return (is_ok, check_result, cstID, boxID, loc);
        }

        private (bool isOK, string checkResult, string cmdID) checkHostCommandAbort(S2F41 s2F41)
        {
            bool is_ok = true;
            string check_result = SECSConst.HCACK_Confirm;
            string error_id = string.Empty;
            //var command_id_item = s2F41.REPITEMS.Where(item => SCUtility.isMatche(item.CPNAME, SECSConst.CPNAME_ErrorID)).FirstOrDefault();
            var command_id_item = s2F41.REPITEMS.Where(item => SCUtility.isMatche(item.CPNAME, SECSConst.CPNAME_CommandID)).FirstOrDefault();
            if (command_id_item != null)
            {
                error_id = command_id_item.CPVAL;
                ACMD_MCS cmd_mcs = scApp.CMDBLL.getCMD_MCSByID(error_id);
                if (cmd_mcs == null)
                {
                    check_result = SECSConst.HCACK_Obj_Not_Exist;
                    is_ok = false;
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                       Data: $"Process mcs command [Abort] can't excute, because mcs command id:{error_id} not exist",
                       XID: error_id);
                }
                else
                {
                    if (cmd_mcs.TRANSFERSTATE == E_TRAN_STATUS.Transferring)
                    {
                        check_result = SECSConst.HCACK_Not_Able_Execute;
                        //is_ok = false;
                        //string current_status = ACMD_MCS.COMMAND_STATUS_BIT_To_String(cmd_mcs.COMMANDSTATE);
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                           Data: $"Process mcs command [Abort] can't excute, because mcs command id:{error_id} current command status:{cmd_mcs.COMMANDSTATE}",
                           XID: error_id);
                    }
                }
                //ACMD_MCS cmd_mcs = scApp.CMDBLL.getCMD_MCSByExcuteCmd();
                //if (cmd_mcs != null)
                //{
                //    //if (cmd_mcs.TRANSFERSTATE < E_TRAN_STATUS.Transferring)
                //    //如果命令在
                //    if (cmd_mcs.COMMANDSTATE < ACMD_MCS.COMMAND_STATUS_BIT_INDEX_LOAD_COMPLETE ||
                //        cmd_mcs.COMMANDSTATE >= ACMD_MCS.COMMAND_STATUS_BIT_INDEX_UNLOAD_ARRIVE)
                //    {
                //        check_result = SECSConst.HCACK_Not_Able_Execute;
                //        is_ok = false;
                //        //string current_status = ACMD_MCS.COMMAND_STATUS_BIT_To_String(cmd_mcs.COMMANDSTATE);
                //        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                //           Data: $"Process mcs command [Abort] can't excute, because mcs command id:{error_id} current command status:{cmd_mcs.COMMANDSTATE}",
                //           XID: error_id);
                //    }
                //}
                //else
                //{
                //    check_result = SECSConst.HCACK_Obj_Not_Exist;
                //    is_ok = false;
                //    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                //       Data: $"Process mcs command [Abort] can't excute, because mcs command id:{error_id} not exist",
                //       XID: error_id);
                //}
            }
            else
            {
                check_result = SECSConst.HCACK_Param_Invalid;
                is_ok = false;
            }
            return (is_ok, check_result, error_id);
        }

        private (bool isOK, string checkResult, string cmdID) checkHostCommandCancel(S2F41 s2F41)
        {
            bool is_ok = true;
            string check_result = SECSConst.HCACK_Confirm;
            string command_id = string.Empty;
            var command_id_item = s2F41.REPITEMS.Where(item => SCUtility.isMatche(item.CPNAME, SECSConst.CPNAME_CommandID)).FirstOrDefault();
            if (command_id_item != null)
            {
                command_id = command_id_item.CPVAL;
                ACMD_MCS cmd_mcs = scApp.CMDBLL.getCMD_MCSByID(command_id);

                if (cmd_mcs == null)
                {
                    check_result = SECSConst.HCACK_Obj_Not_Exist;
                    is_ok = false;
                }
            }
            else
            {
                check_result = SECSConst.HCACK_Param_Invalid;
                is_ok = false;
            }
            return (is_ok, check_result, command_id);
        }

        private (bool isOK, string checkResult, string cmdID, string priority) checkHostCommandPriorityUpdate(S2F41_PriorityUpdate s2F41)
        {
            bool is_ok = true;
            string check_result = SECSConst.HCACK_Confirm;
            //string command_id = string.Empty;
            string command_id = s2F41.REPITEMS.CommandID_CP.CPVAL_ASCII;
            string priority = s2F41.REPITEMS.PRIORITY_CP.CPVAL_U2;
            if (command_id != null)
            {
                ACMD_MCS cmd_mcs = scApp.CMDBLL.getCMD_MCSByID(command_id);
                if (cmd_mcs != null)
                {
                    if (cmd_mcs.TRANSFERSTATE >= E_TRAN_STATUS.Transferring)  //A0.03
                    {                                                       //A0.03
                        check_result = SECSConst.HCACK_Not_Able_Execute;    //A0.03
                        is_ok = false;                                      //A0.03
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                           Data: $"Process mcs command [Cancle] can't excute, because mcs command id:{command_id} current transfer status:{cmd_mcs.TRANSFERSTATE},ohtc reply:{check_result}",
                           XID: command_id);
                    }                                                       //A0.03
                    //else if (cmd_mcs.TRANSFERSTATE >= E_TRAN_STATUS.Transferring)
                    else if (cmd_mcs.COMMANDSTATE >= ACMD_MCS.COMMAND_STATUS_BIT_INDEX_LOAD_ARRIVE)
                    {
                        check_result = SECSConst.HCACK_Not_Able_Execute;
                        is_ok = false;
                        string current_status = ACMD_MCS.COMMAND_STATUS_BIT_To_String(cmd_mcs.COMMANDSTATE);
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                           Data: $"Process mcs command [Cancel] can't excute, because mcs command id:{command_id} current command status:{current_status},ohtc reply:{check_result}",
                           XID: command_id);
                    }
                }
                else
                {
                    check_result = SECSConst.HCACK_Obj_Not_Exist;
                    is_ok = false;
                }
            }
            else
            {
                check_result = SECSConst.HCACK_Param_Invalid;
                is_ok = false;
            }
            return (is_ok, check_result, command_id, priority);
        }

        private (bool isOK, string checkResult, string portID, int type) checkHostCommandPortTypeChg(S2F41_PriorityUpdate s2F41)
        {
            bool is_ok = true;
            string check_result = SECSConst.HCACK_Confirm;
            //string command_id = string.Empty;
            string port_id = s2F41.REPITEMS.CommandID_CP.CPVAL_ASCII;
            int port_type = Convert.ToInt32(s2F41.REPITEMS.PRIORITY_CP.CPVAL_U2);
            var port_item = scApp.PortDefBLL.GetPortData(port_id);

            if (port_item == null)
            {
                check_result = SECSConst.HCACK_Obj_Not_Exist;
                is_ok = false;
            }
            else
            {
                if(port_type == 0 | port_type == 1)
                {
                    if ((int)port_item.State == SECSConst.PortState_OutService)
                    {
                        check_result = SECSConst.HCACK_Not_Able_Execute;
                        is_ok = false;
                    }
                    else
                    {
                        if (port_item.PortType == (E_PortType)port_type)    //20/07/16 美微說，流向一樣要回5
                        {
                            check_result = SECSConst.HCACK_Rejected_Already_Requested;
                            is_ok = false;
                            //LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                            //   Data: $"Process mcs command [PortTypeChg] can't excute, because mcs command id:{port_id} current transfer status:{port_item.PortType},ohtc reply:{check_result}",
                            //   XID: port_id);
                        }
                    }
                }
                else
                {
                    check_result = SECSConst.HCACK_Param_Invalid;
                    is_ok = false;
                }
            }

            return (is_ok, check_result, port_id, port_type);
        }

        private (bool isOK, string checkResult, bool hasCarrier, string CSTID, string BOXID, string LOCID) checkHostCommandInstall(S2F41 s2F41)
        {
            bool is_ok = false;
            bool has_carrier = true;
            string check_result = SECSConst.HCACK_Confirm;

            var carrier_id_item = s2F41.REPITEMS.Where(item => SCUtility.isMatche(item.CPNAME, SECSConst.CPNAME_CarrierID)).FirstOrDefault();
            var Box_item = s2F41.REPITEMS.Where(item => SCUtility.isMatche(item.CPNAME, SECSConst.CPNAME_BoxID)).FirstOrDefault();
            var Loc_item = s2F41.REPITEMS.Where(item => SCUtility.isMatche(item.CPNAME, SECSConst.CPNAME_CarrierLoc)).FirstOrDefault();

            string CarrierID = carrier_id_item?.CPVAL ?? "";
            string BoxID = Box_item?.CPVAL ?? "";
            string LocID = Loc_item?.CPVAL ?? "";

            if (string.IsNullOrWhiteSpace(BoxID) || string.IsNullOrWhiteSpace(LocID))
            {
                check_result = SECSConst.HCACK_Param_Invalid;
                is_ok = false;
            }
            else
            {
                if(scApp.TransferService.isLocExist(LocID))
                {
                    is_ok = true;
                }
                else
                {
                    check_result = SECSConst.HCACK_Obj_Not_Exist;
                    is_ok = false;
                }
            }

            return (is_ok, check_result, has_carrier, CarrierID, BoxID, LocID);
        }

        private (bool isOK, string checkResult, bool hasCarrier, string CSTID, string BOXID) checkHostCommandRemove(S2F41 s2F41)
        {
            bool is_ok = true;
            bool has_carrier = true;
            string check_result = SECSConst.HCACK_Confirm;
            var carrier_id_item = s2F41.REPITEMS.Where(item => SCUtility.isMatche(item.CPNAME, SECSConst.CPNAME_CarrierID)).FirstOrDefault();
            var Box_item = s2F41.REPITEMS.Where(item => SCUtility.isMatche(item.CPNAME, SECSConst.CPNAME_BoxID)).FirstOrDefault();
            string CarrierID = "";
            string BoxID = "";

            if (carrier_id_item != null && Box_item != null)
            {
                CarrierID = carrier_id_item.CPVAL?.ToString() ?? "";
                BoxID = Box_item.CPVAL;

                CassetteData cassette = scApp.CassetteDataBLL.loadCassetteDataByCstBoxID(CarrierID, BoxID);

                if (cassette == null)
                {
                    TransferServiceLogger.Info
                    (
                        DateTime.Now.ToString("HH:mm:ss.fff ") +
                        "OHB >> MCS|S2F41 RemoveCST 找不到卡匣 CarrierID:" + CarrierID + " BoxID:" + BoxID
                    );

                    has_carrier = false;
                    //check_result = SECSConst.HCACK_Obj_Not_Exist;
                    check_result = SECSConst.HCACK_Obj_Not_Exist;
                    is_ok = false;
                }
                else
                {
                    has_carrier = true;
                    ACMD_MCS cmdData = scApp.CMDBLL.getCMD_ByBoxID(BoxID);
                    if (cmdData != null)
                    {
                        TransferServiceLogger.Info
                        (
                            DateTime.Now.ToString("HH:mm:ss.fff ") +
                            "OHB >> MCS|S2F41 RemoveCST: 此卡匣已有命令在執行\n"
                            + "cmdID:" + cmdData.CMD_ID
                            + "    來源:" + cmdData.HOSTSOURCE
                            + "    目的:" + cmdData.HOSTDESTINATION
                            + "    CSTID:" + cmdData.CARRIER_ID
                            + "    BOXID:" + cmdData.BOX_ID
                        );

                        has_carrier = false;
                        is_ok = false;
                        check_result = SECSConst.HCACK_Not_Able_Execute;
                    }
                }
            }
            else
            {
                TransferServiceLogger.Info
                (
                    DateTime.Now.ToString("HH:mm:ss.fff ") +
                    "OHB >> MCS|S2F41 RemoveCST carrier_id_item = Null && Box_item = Null"
                );

                check_result = SECSConst.HCACK_Param_Invalid;
                is_ok = false;
            }
            return (is_ok, check_result, has_carrier, CarrierID, BoxID);
        }

        private (bool isOK, string checkResult, string shelfid) checkHostCommandDisableShelf(S2F41 s2F41)
        {
            bool is_ok = true;
            string check_result = SECSConst.HCACK_Confirm;
            var shelf_id_item = s2F41.REPITEMS.Where(item => SCUtility.isMatche(item.CPNAME, SECSConst.CPNAME_ShelfID)).FirstOrDefault();
            string ShelfID = "";
            if (shelf_id_item != null)
            {
                ShelfID = shelf_id_item.CPVAL;

                ShelfDef shelf = scApp.ShelfDefBLL.loadShelfDataByID(ShelfID);
                if (shelf == null)
                {
                    check_result = SECSConst.HCACK_Obj_Not_Exist;
                    is_ok = false;
                }
            }
            else
            {
                check_result = SECSConst.HCACK_Param_Invalid;
                is_ok = false;
            }
            return (is_ok, check_result, ShelfID);
        }

        private (bool isOK, string checkResult, string shelfid) checkHostCommandEnableShelf(S2F41 s2F41)
        {
            bool is_ok = true;
            string check_result = SECSConst.HCACK_Confirm;
            var shelf_id_item = s2F41.REPITEMS.Where(item => SCUtility.isMatche(item.CPNAME, SECSConst.CPNAME_ShelfID)).FirstOrDefault();

            string ShelfID = shelf_id_item.CPVAL;

            if (shelf_id_item != null)
            {
                ShelfDef shelf = scApp.ShelfDefBLL.loadShelfDataByID(ShelfID);
                if (shelf == null)
                {
                    check_result = SECSConst.HCACK_Obj_Not_Exist;
                    is_ok = false;
                }
            }
            else
            {
                check_result = SECSConst.HCACK_Param_Invalid;
                is_ok = false;
            }
            return (is_ok, check_result, ShelfID);
        }

        private (bool isOK, string checkResult, bool hasCarrier, string CSTID, string BOXID, string LOCID) checkHostCommandRename(S2F41 s2F41)
        {
            bool is_ok = true;
            bool has_carrier = true;
            string check_result = SECSConst.HCACK_Confirm;
            var carrier_id_item = s2F41.REPITEMS.Where(item => SCUtility.isMatche(item.CPNAME, SECSConst.CPNAME_CarrierID)).FirstOrDefault();
            var Box_item = s2F41.REPITEMS.Where(item => SCUtility.isMatche(item.CPNAME, SECSConst.CPNAME_BoxID)).FirstOrDefault();
            var Loc_item = s2F41.REPITEMS.Where(item => SCUtility.isMatche(item.CPNAME, SECSConst.CPNAME_CarrierLoc)).FirstOrDefault();
            string CarrierID = carrier_id_item.CPVAL;
            string BoxID = Box_item.CPVAL;
            string LocID = Loc_item.CPVAL;

            if (carrier_id_item != null)
            {
                CassetteData cassette = scApp.CassetteDataBLL.loadCassetteDataByCSTID(CarrierID);
                if (cassette == null)
                {
                    has_carrier = false;
                }
                else
                {
                    has_carrier = true;
                }
            }
            else
            {
                check_result = SECSConst.HCACK_Param_Invalid;
                is_ok = false;
            }
            return (is_ok, check_result, has_carrier, CarrierID, BoxID, LocID);
        }


        private (bool isOK, string checkResult) checkHostCommandConfirmRoute(S2F41 s2F41)
        {
            bool is_ok = true;
            string check_result = SECSConst.HCACK_Confirm;
            var source_item = s2F41.REPITEMS.Where(item => SCUtility.isMatche(item.CPNAME, SECSConst.CPNAME_SourcePort)).FirstOrDefault();
            var dest_item = s2F41.REPITEMS.Where(item => SCUtility.isMatche(item.CPNAME, SECSConst.CPNAME_DestPort)).FirstOrDefault();
            string source_port_id = source_item?.CPVAL;
            string dest_port_id = dest_item?.CPVAL;

            string source_adr_id = "";
            string dest_adr_id = "";
            if (is_ok && !scApp.MapBLL.getAddressID(source_port_id, out source_adr_id))
            {
                check_result = SECSConst.HCACK_Obj_Not_Exist;
                is_ok = false;
            }
            if (!scApp.MapBLL.getAddressID(dest_port_id, out dest_adr_id))
            {
                check_result = SECSConst.HCACK_Obj_Not_Exist;
                is_ok = false;
            }
            if (is_ok)
            {
                var source_adr_sections = scApp.MapBLL.loadSectionByFromAdr(source_adr_id);
                var dest_adr_sections = scApp.MapBLL.loadSectionByFromAdr(dest_adr_id);
                ASEGMENT source_segment = scApp.MapBLL.getSegmentByID(source_adr_sections.First().SEG_NUM);
                ASEGMENT dest_segment = scApp.MapBLL.getSegmentByID(dest_adr_sections.First().SEG_NUM);

                if (source_segment.STATUS == E_SEG_STATUS.Closed || dest_segment.STATUS == E_SEG_STATUS.Closed)
                {
                    //A0.02 check_result = SECSConst.HCACK_Not_Able_Execute;
                    //check_result = SECSConst.HCACK_Enabled_Route_Does_Not_Exist; //A0.02
                }
            }
            return (is_ok, check_result);
        }

        private (bool isOK, string checkResult, string cmdID) checkHostCommandRetry(S2F41 s2F41)
        {
            bool is_ok = true;
            string check_result = SECSConst.HCACK_Confirm;
            string error_id = string.Empty;
            var command_id_item = s2F41.REPITEMS.Where(item => SCUtility.isMatche(item.CPNAME, SECSConst.CPNAME_ErrorID)).FirstOrDefault();
            if (command_id_item != null)
            {
                error_id = command_id_item.CPVAL;
                ACMD_MCS cmd_mcs = scApp.CMDBLL.getCMD_MCSByExcuteCmd();
                if (cmd_mcs != null)
                {
                    //if (cmd_mcs.TRANSFERSTATE < E_TRAN_STATUS.Transferring)
                    //如果命令在
                    if (cmd_mcs.COMMANDSTATE < ACMD_MCS.COMMAND_STATUS_BIT_INDEX_LOAD_COMPLETE ||
                        cmd_mcs.COMMANDSTATE >= ACMD_MCS.COMMAND_STATUS_BIT_INDEX_UNLOAD_ARRIVE)
                    {
                        check_result = SECSConst.HCACK_Not_Able_Execute;
                        is_ok = false;
                        //string current_status = ACMD_MCS.COMMAND_STATUS_BIT_To_String(cmd_mcs.COMMANDSTATE);
                        LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                           Data: $"Process mcs command [Abort] can't excute, because mcs command id:{error_id} current command status:{cmd_mcs.COMMANDSTATE}",
                           XID: error_id);
                    }
                }
                else
                {
                    check_result = SECSConst.HCACK_Obj_Not_Exist;
                    is_ok = false;
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                       Data: $"Process mcs command [Abort] can't excute, because mcs command id:{error_id} not exist",
                       XID: error_id);
                }
            }
            else
            {
                check_result = SECSConst.HCACK_Param_Invalid;
                is_ok = false;
            }
            return (is_ok, check_result, error_id);
        }

        private (bool isOK, string checkResult) checkHostCommandRCMD_CARRIERLOTIDUPDATE(S2F41 s2F41)
        {
            bool is_ok = false;
            string check_result = SECSConst.HCACK_Confirm;
            string cstID = "";
            string lotID = "";

            var cstID_item = s2F41.REPITEMS.Where(item => SCUtility.isMatche(item.CPNAME, SECSConst.CPNAME_CarrierID)).FirstOrDefault();
            var lotID_item = s2F41.REPITEMS.Where(item => SCUtility.isMatche(item.CPNAME, SECSConst.CPNAME_LotID)).FirstOrDefault();
            
            if (cstID_item != null && lotID_item != null)
            {
                cstID = cstID_item.CPVAL?.Trim() ?? "";
                lotID = lotID_item.CPVAL?.Trim() ?? "";

                CassetteData cstData = scApp.CassetteDataBLL.loadCassetteDataByCSTID(cstID);

                if (cstData == null)
                {
                    check_result = SECSConst.HCACK_Obj_Not_Exist;
                }
                else
                {
                    is_ok = scApp.CassetteDataBLL.UpdateCSTID(cstData.Carrier_LOC, cstData.BOXID, cstData.CSTID, lotID.Trim());

                    if(is_ok == false)
                    {

                    }
                }
            }
            else
            {
                check_result = SECSConst.HCACK_Param_Invalid;
            }

            return (is_ok, check_result);
        }

        protected override void S1F3ReceiveSelectedEquipmentStatusRequest(object sender, SECSEventArgs e)
        {
            try
            {
                S1F3 s1f3 = ((S1F3)e.secsHandler.Parse<S1F3>(e));
                SCUtility.secsActionRecordMsg(scApp, true, s1f3);

                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|S1F3 VID: " + string.Join(",", s1f3.SVID));

                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                //          Data: s1f3);
                bool is_monitored_vehicle = false;
                int count = s1f3.SVID.Count();
                S1F4 s1f4 = new S1F4();
                s1f4.SECSAgentName = scApp.EAPSecsAgentName;
                s1f4.SystemByte = s1f3.SystemByte;
                s1f4.SV = new SXFY[count];
                for (int i = 0; i < count; i++)
                {
                    if (s1f3.SVID[i] == SECSConst.VID_Control_State)
                    {
                        line.CurrentStateChecked = true;
                        s1f4.SV[i] = buildControlStateVIDItem();
                    }
                    else if (s1f3.SVID[i] == SECSConst.VID_SC_State)
                    {
                        line.TSCStateChecked = true;
                        s1f4.SV[i] = buildSCStateVIDItem();
                    }
                    else if (s1f3.SVID[i] == SECSConst.VID_Spec_Version)
                    {
                        s1f4.SV[i] = buildSpecVersionVIDItem();
                    }
                    else if (s1f3.SVID[i] == SECSConst.VID_Enhanced_Carriers)
                    {
                        line.EnhancedCarriersChecked = true;
                        s1f4.SV[i] = buildEnhancedCarriersVIDItem();
                    }
                    else if (s1f3.SVID[i] == SECSConst.VID_Enhanced_Transfers)
                    {
                        line.EnhancedTransfersChecked = true;
                        s1f4.SV[i] = buildEnhancedTransfersVIDItem();
                    }
                    else if (s1f3.SVID[i] == SECSConst.VID_Enhanced_Active_Zones)
                    {
                        line.EnhancedVehiclesChecked = true;
                        s1f4.SV[i] = buildZoneDataVIDItem();
                    }
                    else if (s1f3.SVID[i] == SECSConst.VID_Current_Port_States)
                    {
                        line.CurrentPortStateChecked = true;
                        s1f4.SV[i] = buildCurrentPortStatesVIDItem();
                    }
                    else if (s1f3.SVID[i] == SECSConst.VID_CurrEq_Port_Status)
                    {
                        line.CurrentEQPortStateChecked = true;
                        s1f4.SV[i] = buildCurrentEqPortStatusVIDItem();
                    }
                    else if (s1f3.SVID[i] == SECSConst.VID_Port_Types)
                    {
                        line.CurrentPortTypesChecked = true;
                        s1f4.SV[i] = buildCurrentPortTypesVIDItem();
                    }
                    else if (s1f3.SVID[i] == SECSConst.VID_Alarms_Set)
                    {
                        line.AlarmSetChecked = true;
                        s1f4.SV[i] = buildAlarmsSetVIDItem();
                    }
                    else if (s1f3.SVID[i] == SECSConst.VID_Unit_Alarm_List)
                    {
                        line.UnitAlarmStateListChecked = true;
                        s1f4.SV[i] = buildUnitAlarmListVIDItem();
                    }
                    //else if (s1f3.SVID[i] == SECSConst.VID_CurrEq_Port_Status)
                    //{
                    //    line.EnhancedTransfersChecked = true;
                    //    s1f4.SV[i] = buildCurrEqPortStatusVIDItem();
                    //}
                    //else if (s1f3.SVID[i] == SECSConst.VID_Port_Types)
                    //{
                    //    line.EnhancedTransfersChecked = true;
                    //    s1f4.SV[i] = buildPortTypesVIDItem();
                    //}
                    //else if (s1f3.SVID[i] == SECSConst.VID_Alarms_Set)
                    //{
                    //    line.EnhancedTransfersChecked = true;
                    //    s1f4.SV[i] = buildAlarmsSetVIDItem();
                    //}
                    //else if (s1f3.SVID[i] == SECSConst.VID_Unit_Alarm_List)
                    //{
                    //    line.EnhancedTransfersChecked = true;
                    //    s1f4.SV[i] = buildUnitAlarmListVIDItem();
                    //}
                    else
                    {
                        s1f4.SV[i] = new SXFY();
                    }
                }

                if (!is_monitored_vehicle)
                {
                    SCUtility.RecodeReportInfo(s1f3);
                }
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                //          Data: s1f3);
                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|S1F4\n" + string.Join(",", s1f4.SV.Select(x => x.toSECSString().TrimEnd('\r', '\n'))));

                TrxSECS.ReturnCode rtnCode = ISECSControl.replySECS(bcfApp, s1f4);
                SCUtility.secsActionRecordMsg(scApp, false, s1f4);

                if (!is_monitored_vehicle)
                {
                    SCUtility.RecodeReportInfo(s1f4);
                }
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                //          Data: s1f4);
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "  S1F3ReceiveSelectedEquipmentStatusRequest\n" + ex);

                logger.Error("AUOMCSDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}",
                    line.LINE_ID, "S1F3_Receive_Eqpt_Stat_Req", ex.ToString());
            }
        }
        #region Build VIDItem

        private S6F11.RPTINFO.RPTITEM.VIDITEM_107_SV buildZoneDataVIDItem()
        {
            S6F11.RPTINFO.RPTITEM.VIDITEM_107_SV viditem_107 = new S6F11.RPTINFO.RPTITEM.VIDITEM_107_SV();
            List<ZoneDef> zones = scApp.ZoneDefBLL.loadZoneData();
            viditem_107.ZoneData = new S6F11.RPTINFO.RPTITEM.VIDITEM_172_SV[zones.Count];
            for (int i = 0; i < zones.Count; i++)
            {
                viditem_107.ZoneData[i] = new S6F11.RPTINFO.RPTITEM.VIDITEM_172_SV();
                viditem_107.ZoneData[i].ZONE_NAME = zones[i].ZoneName;
                int capacity = scApp.ZoneDefBLL.GetZoneCapacity(zones[i].ZoneID);
                viditem_107.ZoneData[i].ZONE_CAPACITY_OBJ.Zone_Capacity = capacity.ToString();
                int totalsize = scApp.ZoneDefBLL.GetZoneTotalSize(zones[i].ZoneID);
                viditem_107.ZoneData[i].ZONE_TOTAL_SIZE_OBJ.Zone_Total_Size = totalsize.ToString();
                viditem_107.ZoneData[i].ZONE_TYPE_OBJ.Zone_Type = zones[i].ZoneType.ToString();
                viditem_107.ZoneData[i].DISABLE_LOCATIONS_OBJ = new S6F11.RPTINFO.RPTITEM.VIDITEM_888_SV();

                List<ShelfDef> disShelf = scApp.ZoneDefBLL.GetDisShelf(zones[i].ZoneID);

                viditem_107.ZoneData[i].DISABLE_LOCATIONS_OBJ.DISABLE_LOC_OBJ = new S6F11.RPTINFO.RPTITEM.VIDITEM_889_SV[disShelf.Count];

                int disableCnt = viditem_107.ZoneData[i].DISABLE_LOCATIONS_OBJ.DISABLE_LOC_OBJ.Length;

                for (int j = 0; j < disShelf.Count; j++)
                {
                    viditem_107.ZoneData[i].DISABLE_LOCATIONS_OBJ.DISABLE_LOC_OBJ[j] = new S6F11.RPTINFO.RPTITEM.VIDITEM_889_SV();

                    viditem_107.ZoneData[i].DISABLE_LOCATIONS_OBJ.DISABLE_LOC_OBJ[j].CARRIER_LOC_OBJ.CARRIER_LOC = disShelf[j].ShelfID;
                    string cstid = scApp.CassetteDataBLL.loadCassetteDataByShelfID(disShelf[j].ShelfID)?.CSTID ?? "";
                    viditem_107.ZoneData[i].DISABLE_LOCATIONS_OBJ.DISABLE_LOC_OBJ[j].CARRIER_ID_OBJ.CARRIER_ID = cstid;
                }
            }

            return viditem_107;
        }

        private S6F11.RPTINFO.RPTITEM.VIDITEM_06_SV buildControlStateVIDItem()
        {
            string control_state = SCAppConstants.LineHostControlState.convert2MES(line.Host_Control_State);
            S6F11.RPTINFO.RPTITEM.VIDITEM_06_SV viditem_06 = new S6F11.RPTINFO.RPTITEM.VIDITEM_06_SV()
            {
                CONTROLSTATE = control_state
            };
            return viditem_06;
        }
        private S6F11.RPTINFO.RPTITEM.VIDITEM_04_SV buildAlarmsSetVIDItem()
        {
            S6F11.RPTINFO.RPTITEM.VIDITEM_04_SV viditem_04 = new S6F11.RPTINFO.RPTITEM.VIDITEM_04_SV();

            //AlarmsSet，只報會造成設備不能跑貨的狀況(S5F1)，因為會造成此狀況的只有車子的 Alarm ，這邊就濾掉 Port 的異常
            List<ALARM> alarms = scApp.AlarmBLL.loadSetAlarmListByError();  

            viditem_04.ALIDs = new S6F11.RPTINFO.RPTITEM.ALID_DVVAL[alarms.Count];
            //viditem_04.SystemByte = 100;
            for (int i = 0; i < alarms.Count; i++)
            {
                viditem_04.ALIDs[i] = new S6F11.RPTINFO.RPTITEM.ALID_DVVAL();
                viditem_04.ALIDs[i].ALID = alarms[i].ALAM_CODE.Trim();
            }
            return viditem_04;
        }
        private S6F11.RPTINFO.RPTITEM.VIDITEM_118_SV buildCurrentPortStatesVIDItem()
        {
            //List<APORTSTATION> port_station = scApp.getEQObjCacheManager().getALLPortStation();
            string ohbName = scApp.getEQObjCacheManager().getLine().LINE_ID;
            //List<PortDef> port_station = scApp.PortDefBLL.GetOHB_PortData(ohbName); 
            List<PortDef> port_station = scApp.PortDefBLL.GetOHB_CVPortData(ohbName);

            int port_count = port_station.Count;

            string control_state = SCAppConstants.LineHostControlState.convert2MES(line.Host_Control_State);
            S6F11.RPTINFO.RPTITEM.VIDITEM_118_SV viditem_118 = new S6F11.RPTINFO.RPTITEM.VIDITEM_118_SV();
            viditem_118.PORT_INFO_OBJ = new S6F11.RPTINFO.RPTITEM.VIDITEM_354_SV[port_count];
            for (int j = 0; j < port_count; j++)
            {
                viditem_118.PORT_INFO_OBJ[j] = new S6F11.RPTINFO.RPTITEM.VIDITEM_354_SV();
                viditem_118.PORT_INFO_OBJ[j].PORT_ID_OBJ.PORT_ID = port_station[j].PLCPortID;
                viditem_118.PORT_INFO_OBJ[j].PORT_TRANSFTER_STATE_OBJ.PORT_TRANSFER_STATE = ((int)port_station[j].State).ToString();
            }
            return viditem_118;
        }
        private S6F11.RPTINFO.RPTITEM.VIDITEM_114_SV buildSpecVersionVIDItem()
        {
            S6F11.RPTINFO.RPTITEM.VIDITEM_114_SV viditem_114 = new S6F11.RPTINFO.RPTITEM.VIDITEM_114_SV()
            {
                SPEC_VERSION = SCApplication.getMessageString("SYSTEM_VERSION"),
            };
            return viditem_114;
        }
        private S6F11.RPTINFO.RPTITEM.VIDITEM_350_SV buildCurrentEqPortStatusVIDItem()
        {
            S6F11.RPTINFO.RPTITEM.VIDITEM_350_SV viditem_350 = new S6F11.RPTINFO.RPTITEM.VIDITEM_350_SV();
            viditem_350.EQ_PORT_INFO_OBJ = new S6F11.RPTINFO.RPTITEM.VIDITEM_356_SV[0];
            //for(int i = 0;i < viditem_350.EQ_PORT_INFO_OBJ.Length;i++)
            //{
            //    viditem_350.EQ_PORT_INFO_OBJ[i] = new S6F11.RPTINFO.RPTITEM.VIDITEM_356_SV();
            //    viditem_350.EQ_PORT_INFO_OBJ[i].PORT_ID_OBJ.PORT_ID = "123123";
            //    viditem_350.EQ_PORT_INFO_OBJ[i].PORT_TRANSFTER_STATE_OBJ.PORT_TRANSFER_STATE = "1";
            //    viditem_350.EQ_PORT_INFO_OBJ[i].EQ_REQ_SATUS_OBJ.EQ_REQ_STATUS = "1";
            //    viditem_350.EQ_PORT_INFO_OBJ[i].EQ_PRESENCE_STATUS_OBJ.EQ_PRESENCE_STATUS = "1";
            //}
            return viditem_350;
        }
        private S6F11.RPTINFO.RPTITEM.VIDITEM_351_SV buildCurrentPortTypesVIDItem()
        {
            string ohbName = scApp.getEQObjCacheManager().getLine().LINE_ID;
            //List<PortDef> port_station = scApp.PortDefBLL.GetOHB_PortData(ohbName);
            List<PortDef> port_station = scApp.PortDefBLL.GetOHB_CVPortData(ohbName);

            int port_count = port_station.Count;

            string control_state = SCAppConstants.LineHostControlState.convert2MES(line.Host_Control_State);
            S6F11.RPTINFO.RPTITEM.VIDITEM_351_SV viditem_351 = new S6F11.RPTINFO.RPTITEM.VIDITEM_351_SV();
            viditem_351.PORT_TYPE_INFO_OBJ = new S6F11.RPTINFO.RPTITEM.VIDITEM_601_SV[port_count];
            for (int i = 0; i < port_count; i++)
            {
                viditem_351.PORT_TYPE_INFO_OBJ[i] = new S6F11.RPTINFO.RPTITEM.VIDITEM_601_SV();
                viditem_351.PORT_TYPE_INFO_OBJ[i].PORT_ID_OBJ.PORT_ID = port_station[i].PLCPortID;
                viditem_351.PORT_TYPE_INFO_OBJ[i].PORT_UNIT_TYPE_OBJ.PORT_UNIT_TYPE = ((int)port_station[i].PortType).ToString();
            }
            return viditem_351;
        }
        private S6F11.RPTINFO.RPTITEM.VIDITEM_360_SV buildUnitAlarmListVIDItem()
        {
            //List<ALARM> occurred_alarms = scApp.AlarmBLL.getCurrentAlarmsFromRedis();
            //List<ALARM> occurred_error_alarms = occurred_alarms.Where(alarm => alarm.ALAM_LVL == E_ALARM_LVL.Error).ToList();
            
            List<ALARM> alarms = scApp.AlarmBLL.loadSetAlarmListByWarn();

            S6F11.RPTINFO.RPTITEM.VIDITEM_360_SV viditem_360 = new S6F11.RPTINFO.RPTITEM.VIDITEM_360_SV();
            viditem_360.UNIT_ALARM_INFO_OBJ = new S6F11.RPTINFO.RPTITEM.VIDITEM_361_SV[alarms.Count];

            for (int i = 0; i < alarms.Count; i++)
            {
                //AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(occurred_error_alarms[i].EQPT_ID);
                //int current_adr = 0;
                //int.TryParse(vh.CUR_ADR_ID, out current_adr);
                //string uint_id = vh.Real_ID;
                string uint_id = alarms[i].UnitID.ToString().Trim();
                string alarm_id = alarms[i].ALAM_CODE;
                int ialarm_code = 0;
                int.TryParse(alarm_id, out ialarm_code);
                string alarm_code = (ialarm_code < 0 ? ialarm_code * -1 : ialarm_code).ToString();
                string alarm_text = alarms[i].ALAM_DESC;
                string mainte_state = SECSConst.MAINTE_STATE_NotMainteance;

                if (alarms[i].ALAM_LVL == E_ALARM_LVL.Error)
                {
                    mainte_state = SECSConst.MAINTE_STATE_Mainteance;
                }

                viditem_360.UNIT_ALARM_INFO_OBJ[i] = new S6F11.RPTINFO.RPTITEM.VIDITEM_361_SV();
                viditem_360.UNIT_ALARM_INFO_OBJ[i].UNIT_ID_OBJ.UNIT_ID = uint_id;
                viditem_360.UNIT_ALARM_INFO_OBJ[i].ALARM_ID_OBJ.ALARM_ID = alarm_code;
                viditem_360.UNIT_ALARM_INFO_OBJ[i].ALARM_TEXT_OBJ.ALARM_TEXT = alarm_text;
                viditem_360.UNIT_ALARM_INFO_OBJ[i].MAINT_STATE_OBJ.MAINT_STATE = mainte_state;
            }

            return viditem_360;
        }
        private S6F11.RPTINFO.RPTITEM.VIDITEM_51_SV buildEnhancedCarriersVIDItem()
        {
            List<CassetteData> cassettedata = scApp.CassetteDataBLL.loadCassetteData();
            S6F11.RPTINFO.RPTITEM.VIDITEM_51_SV viditem_51 = new S6F11.RPTINFO.RPTITEM.VIDITEM_51_SV();
            viditem_51.ENHANCED_CARRIER_INFO = new S6F11.RPTINFO.RPTITEM.VIDITEM_10_SV[cassettedata.Count];
            for (int i = 0; i < cassettedata.Count; i++)
            {
                viditem_51.ENHANCED_CARRIER_INFO[i] = new S6F11.RPTINFO.RPTITEM.VIDITEM_10_SV();
                viditem_51.ENHANCED_CARRIER_INFO[i].CARRIER_ID_OBJ.CARRIER_ID = cassettedata[i].CSTID;
                viditem_51.ENHANCED_CARRIER_INFO[i].CARRIER_LOC_OBJ.CARRIER_LOC = cassettedata[i].Carrier_LOC;
                viditem_51.ENHANCED_CARRIER_INFO[i].CARRIER_ZONE_NAME_OBJ.CARRIER_ZONE_NAME = scApp.CassetteDataBLL.GetZoneName(cassettedata[i].Carrier_LOC);
                viditem_51.ENHANCED_CARRIER_INFO[i].INSTALL_TIME_OBJ.INSTALLTIME = DateTime.Parse(cassettedata[i].CSTInDT).ToString("yyyyMMddHHmmss");
                viditem_51.ENHANCED_CARRIER_INFO[i].CARRIER_STATE.Carrier_State = ((int)cassettedata[i].CSTState).ToString();
                viditem_51.ENHANCED_CARRIER_INFO[i].BOX_ID_OBJ.BOX_ID = cassettedata[i].BOXID;
            }
            return viditem_51;
        }
        //private S6F11.RPTINFO.RPTITEM.VIDITEM_119_SV buildEnhancedVehiclesVIDItem()
        //{
        //    List<AVEHICLE> vhs = scApp.getEQObjCacheManager().getAllVehicle();
        //    int vhs_count = vhs.Count;
        //    S6F11.RPTINFO.RPTITEM.VIDITEM_119_SV viditem_119 = new S6F11.RPTINFO.RPTITEM.VIDITEM_119_SV();
        //    viditem_119.ENHANCED_VEHICLE_INFO_OBJ = new S6F11.RPTINFO.RPTITEM.VIDITEM_119_SV.ENHANCED_VEHICLE_INFO[vhs_count];
        //    for (int j = 0; j < vhs_count; j++)
        //    {
        //        string adr_or_port_id = "";
        //        scApp.MapBLL.getPortID(vhs[j].CUR_ADR_ID, out adr_or_port_id);
        //        viditem_119.ENHANCED_VEHICLE_INFO_OBJ[j] = new S6F11.RPTINFO.RPTITEM.VIDITEM_119_SV.ENHANCED_VEHICLE_INFO();
        //        viditem_119.ENHANCED_VEHICLE_INFO_OBJ[j].VehicleID = vhs[j].Real_ID;
        //        viditem_119.ENHANCED_VEHICLE_INFO_OBJ[j].VehicleState = ((int)vhs[j].State).ToString();
        //        viditem_119.ENHANCED_VEHICLE_INFO_OBJ[j].VehicleLocation = adr_or_port_id;
        //    }
        //    return viditem_119;
        //}
        private S6F11.RPTINFO.RPTITEM.VIDITEM_73_SV buildSCStateVIDItem()
        {

            string tsc_state = ((int)line.TSC_state_machine.State).ToString();
            if (line.TSC_state_machine.State == ALINE.TSCState.NONE)
            {
                tsc_state = "1";
            }
            else
            {
                tsc_state = ((int)line.TSC_state_machine.State).ToString();

            }
            S6F11.RPTINFO.RPTITEM.VIDITEM_73_SV viditem_73 = new S6F11.RPTINFO.RPTITEM.VIDITEM_73_SV()
            {
                SC_STATE = tsc_state
            };

            return viditem_73;
        }
        //private S6F11.RPTINFO.RPTITEM.VIDITEM_254_SV buildUnitAlarmStatListItem()
        //{
        //    List<ALARM> occurred_alarms = scApp.AlarmBLL.getCurrentAlarmsFromRedis();
        //    List<ALARM> occurred_error_alarms = occurred_alarms.Where(alarm => alarm.ALAM_LVL == E_ALARM_LVL.Error).ToList();

        //    S6F11.RPTINFO.RPTITEM.VIDITEM_254_SV viditem_254 = new S6F11.RPTINFO.RPTITEM.VIDITEM_254_SV();
        //    viditem_254.UNIT_ALARMS_INFO_OBJ = new S6F11.RPTINFO.RPTITEM.VIDITEM_254_SV.UNIT_ALARM_INFO[occurred_error_alarms.Count];
        //    for (int i = 0; i < occurred_error_alarms.Count; i++)
        //    {
        //        AVEHICLE vh = scApp.getEQObjCacheManager().getVehicletByVHID(occurred_error_alarms[i].EQPT_ID);
        //        int current_adr = 0;
        //        int.TryParse(vh.CUR_ADR_ID, out current_adr);
        //        string uint_id = vh.Real_ID;
        //        string vh_current_position = current_adr.ToString();
        //        string vh_next_position = "0";
        //        string alarm_id = occurred_error_alarms[i].ALAM_CODE;
        //        int ialarm_code = 0;
        //        int.TryParse(alarm_id, out ialarm_code);
        //        string alarm_code = (ialarm_code < 0 ? ialarm_code * -1 : ialarm_code).ToString();
        //        string alarm_text = occurred_error_alarms[i].ALAM_DESC;
        //        string vh_communication_state = SECSConst.VEHICLE_COMMUNICATION_STATE_Communicating;
        //        string mainte_state = SECSConst.MAINTE_STATE_Undefined;
        //        viditem_254.UNIT_ALARMS_INFO_OBJ[i] = new S6F11.RPTINFO.RPTITEM.VIDITEM_254_SV.UNIT_ALARM_INFO()
        //        {
        //            UnitID = uint_id,
        //            VehicleCurrentPosition = vh_current_position,
        //            VehicleNextPosition = vh_next_position,
        //            //AlarmID = alarm_id,
        //            AlarmID = alarm_code,
        //            AlarmText = alarm_text,
        //            VehicleCommunicationState = vh_communication_state,
        //            MainteState = mainte_state
        //        };

        //    }

        //    return viditem_254;
        //}
        private S6F11.RPTINFO.RPTITEM.VIDITEM_76_SV buildEnhancedTransfersVIDItem()
        {
            //List<ACMD_MCS> mcs_cmds = scApp.CMDBLL.loadACMD_MCSIsUnfinished();
            List<ACMD_MCS> mcs_cmds = scApp.CMDBLL.LoadCmdData().Where(data => data.CMDTYPE != ACMD_MCS.CmdType.PortTypeChange.ToString()

                                                                      ).ToList();
            int cmd_count = mcs_cmds.Count;
            S6F11.RPTINFO.RPTITEM.VIDITEM_76_SV viditem_76 = new S6F11.RPTINFO.RPTITEM.VIDITEM_76_SV();
            //viditem_76.ENHANCED_TRANSFER_CMD = new S6F11.RPTINFO.RPTITEM.VIDITEM_205_SV[cmd_count];
            viditem_76.ENHANCED_TRANSFER_CMD = new S6F11.RPTINFO.RPTITEM.VIDITEM_13_SV[cmd_count];
            for (int i = 0; i < cmd_count; i++)
            {
                ACMD_MCS mcs_cmd = mcs_cmds[i];
                viditem_76.ENHANCED_TRANSFER_CMD[i] = new S6F11.RPTINFO.RPTITEM.VIDITEM_13_SV();
                string transfer_state = SECSConst.convert2MES(mcs_cmd.TRANSFERSTATE);
                viditem_76.ENHANCED_TRANSFER_CMD[i].TRANSFER_STATE_OBJ.Transfer_State = transfer_state;

                viditem_76.ENHANCED_TRANSFER_CMD[i].COMMAND_INFO_OBJ.COMMAND_ID_OBJ.COMMAND_ID = mcs_cmd.CMD_ID;
                viditem_76.ENHANCED_TRANSFER_CMD[i].COMMAND_INFO_OBJ.PRIORITY_OBJ.PRIORITY = mcs_cmd.PRIORITY.ToString();

                viditem_76.ENHANCED_TRANSFER_CMD[i].TRANSFER_INFO_OBJ.CARRIER_ID_OBJ.CARRIER_ID = mcs_cmd.CARRIER_ID;
                viditem_76.ENHANCED_TRANSFER_CMD[i].TRANSFER_INFO_OBJ.BOX_ID_OBJ.BOX_ID = mcs_cmd.BOX_ID;
                viditem_76.ENHANCED_TRANSFER_CMD[i].TRANSFER_INFO_OBJ.SOURCE_ID_OBJ.SOURCE_ID = mcs_cmd.HOSTSOURCE;
                viditem_76.ENHANCED_TRANSFER_CMD[i].TRANSFER_INFO_OBJ.DESTINATION_ID_OBJ.DESTINATION_ID = mcs_cmd.HOSTDESTINATION;
            }


            //for (int k = 0; k < cmd_count; k++)
            //{
            //    ACMD_MCS mcs_cmd = mcs_cmds[k];
            //    viditem_76.ENHANCED_TRANSFER_COMMAND_INFOS[k] = new S6F11.RPTINFO.RPTITEM.VIDITEM_205_SV();
            //    viditem_76.ENHANCED_TRANSFER_COMMAND_INFOS[k].COMMAND_INFO_OBJ.COMMAND_ID_OBJ.COMMAND_ID = mcs_cmd.CMD_ID;
            //    viditem_76.ENHANCED_TRANSFER_COMMAND_INFOS[k].COMMAND_INFO_OBJ.PRIORITY_OBJ.PRIORITY = mcs_cmd.PRIORITY.ToString();
            //    viditem_76.ENHANCED_TRANSFER_COMMAND_INFOS[k].COMMAND_INFO_OBJ.REPLACE_OBJ.REPLACE = mcs_cmd.REPLACE.ToString();

            //    string transfer_state = SECSConst.convert2MES(mcs_cmd.TRANSFERSTATE);
            //    viditem_76.ENHANCED_TRANSFER_COMMAND_INFOS[k].TRANSFER_STATE_OBJ.TRANSFER_STATE = transfer_state;


            //    viditem_76.ENHANCED_TRANSFER_COMMAND_INFOS[k].TRANSFER_INFO_OBJ = new S6F11.RPTINFO.RPTITEM.VIDITEM_67_SV[1];
            //    viditem_76.ENHANCED_TRANSFER_COMMAND_INFOS[k].TRANSFER_INFO_OBJ[0] = new S6F11.RPTINFO.RPTITEM.VIDITEM_67_SV();
            //    viditem_76.ENHANCED_TRANSFER_COMMAND_INFOS[k].TRANSFER_INFO_OBJ[0].CARRIER_ID = mcs_cmd.CARRIER_ID;
            //    viditem_76.ENHANCED_TRANSFER_COMMAND_INFOS[k].TRANSFER_INFO_OBJ[0].SOURCE_PORT = mcs_cmd.HOSTSOURCE;
            //    viditem_76.ENHANCED_TRANSFER_COMMAND_INFOS[k].TRANSFER_INFO_OBJ[0].DESTINATION_PORT = mcs_cmd.HOSTDESTINATION;

            //}
            return viditem_76;
        }
        //private S6F11.RPTINFO.RPTITEM.VIDITEM_91_SV buildEnhancedCarriersVIDItem()
        //{
        //    List<AVEHICLE> has_carry_vhs = scApp.getEQObjCacheManager().getAllVehicle().Where(vh => vh.HAS_CST == 1).ToList();
        //    int carry_vhs_count = has_carry_vhs.Count;
        //    S6F11.RPTINFO.RPTITEM.VIDITEM_91_SV viditem_91 = new S6F11.RPTINFO.RPTITEM.VIDITEM_91_SV();
        //    viditem_91.ENHANCED_CARRIER_INFOS = new S6F11.RPTINFO.RPTITEM.VIDITEM_75_DVVAL[carry_vhs_count];

        //    for (int k = 0; k < carry_vhs_count; k++)
        //    {
        //        AVEHICLE has_carray_vh = has_carry_vhs[k];
        //        AVIDINFO vid_info = scApp.VIDBLL.getVIDInfo(has_carray_vh.VEHICLE_ID);

        //        viditem_91.ENHANCED_CARRIER_INFOS[k] = new S6F11.RPTINFO.RPTITEM.VIDITEM_75_DVVAL();
        //        viditem_91.ENHANCED_CARRIER_INFOS[k].CARRIER_ID = vid_info.CARRIER_ID.Trim();
        //        viditem_91.ENHANCED_CARRIER_INFOS[k].VRHICLE_ID = has_carray_vh.Real_ID;
        //        viditem_91.ENHANCED_CARRIER_INFOS[k].CARRIER_LOC = vid_info.CARRIER_LOC;
        //        viditem_91.ENHANCED_CARRIER_INFOS[k].INSTALL_TIME = vid_info.CARRIER_INSTALLED_TIME?.ToString(SCAppConstants.TimestampFormat_16);

        //    }
        //    return viditem_91;
        //}

        //private S6F11.RPTINFO.RPTITEM.VIDITEM_360_SV buildLaneCutInfoListVIDItem()
        //{
        //    List<string> nonActiveSeg = scApp.MapBLL.loadNonActiveSegmentNum();

        //    int non_active_seg = nonActiveSeg.Count;
        //    S6F11.RPTINFO.RPTITEM.VIDITEM_360_SV viditem_360 = new S6F11.RPTINFO.RPTITEM.VIDITEM_360_SV();
        //    viditem_360.LANE_CUT_INFO = new S6F11.RPTINFO.RPTITEM.VIDITEM_330_SV[non_active_seg];
        //    for (int k = 0; k < non_active_seg; k++)
        //    {
        //        string non_active_segment_id = nonActiveSeg[k];
        //        List<ASECTION> sections = scApp.MapBLL.loadSectionsBySegmentID(non_active_segment_id);
        //        string start_point = sections.First().FROM_ADR_ID;
        //        string end_point = sections.Last().TO_ADR_ID;
        //        string lane_cut_type = SECSConst.LANECUTTYPE_LaneCutOnHMI;
        //        viditem_360.LANE_CUT_INFO[k] = new S6F11.RPTINFO.RPTITEM.VIDITEM_330_SV();
        //        viditem_360.LANE_CUT_INFO[k].LANE_INFO_OBJ.StartPoint = start_point;
        //        viditem_360.LANE_CUT_INFO[k].LANE_INFO_OBJ.EndPoint = end_point;
        //        viditem_360.LANE_CUT_INFO[k].LANE_CUT_TYPE = lane_cut_type;
        //    }
        //    return viditem_360;
        //}
        //private S6F11.RPTINFO.RPTITEM.VIDITEM_252_SV buildMonitoredVehilceInfo()
        //{
        //    List<string> nonActiveSeg = scApp.MapBLL.loadNonActiveSegmentNum();

        //    int non_active_seg = nonActiveSeg.Count;
        //    S6F11.RPTINFO.RPTITEM.VIDITEM_252_SV viditem_252 = new S6F11.RPTINFO.RPTITEM.VIDITEM_252_SV();
        //    viditem_252.MONITORED_VEHICLE_INFO_OBJ = new S6F11.RPTINFO.RPTITEM.VIDITEM_252_SV.MONITORED_VEHICLE_INFO[14];
        //    List<AVEHICLE> vhs = scApp.VehicleBLL.cache.loadVhs();
        //    for (int i = 0; i < vhs.Count; i++)
        //    {
        //        AVEHICLE vh = vhs[i];
        //        string vh_id = vh.Real_ID;
        //        string cur_postion = SCUtility.isEmpty(vh.CUR_ADR_ID) ? "0" : vh.CUR_ADR_ID;
        //        string distance = SCUtility.isEmpty(cur_postion) ? "0" : vh.VEHICLE_ACC_DIST.ToString();
        //        string next_position = "0";
        //        ASECTION cur_sec = scApp.SectionBLL.cache.GetSection(vh.CUR_SEC_ID);
        //        if (cur_sec != null)
        //        {
        //            next_position = cur_sec.TO_ADR_ID;
        //        }
        //        string vh_operation_state = ((int)Convert2VehicleOperationState(vh)).ToString();
        //        string vh_communication_state = ((int)Convert2VehicleCommunication(vh)).ToString();
        //        string vh_control_state = ((int)Convert2VehicleControlMode(vh)).ToString();
        //        string vh_Jam_state = ((int)Convert2VehicleJamState(vh)).ToString();

        //        S6F11.RPTINFO.RPTITEM.VIDITEM_252_SV.MONITORED_VEHICLE_INFO vh_info =
        //        new S6F11.RPTINFO.RPTITEM.VIDITEM_252_SV.MONITORED_VEHICLE_INFO()
        //        {
        //            VehicleID = vh_id,
        //            VehicleCurrentPosition = cur_postion,
        //            VehicleDistanceFromCurrentPosition = distance,
        //            VehicleCurrentDomain = "",
        //            VehicleNextPosition = next_position,
        //            VehicleOperationState = vh_operation_state,
        //            VehicleCommunicationState = vh_communication_state,
        //            VehicleControlMode = vh_control_state,
        //            VehicleJamState = vh_Jam_state
        //        };
        //        viditem_252.MONITORED_VEHICLE_INFO_OBJ[i] = vh_info;
        //    }
        //    return viditem_252;
        //}

        //private S6F11.RPTINFO.RPTITEM.VIDITEM_360_SV EnhancedCarrierInfo()
        //{
        //    List<string> nonActiveSeg = scApp.MapBLL.loadNonActiveSegmentNum();

        //    int non_active_seg = nonActiveSeg.Count;
        //    S6F11.RPTINFO.RPTITEM.VIDITEM_360_SV viditem_360 = new S6F11.RPTINFO.RPTITEM.VIDITEM_360_SV();
        //    viditem_360.LANE_CUT_INFO = new S6F11.RPTINFO.RPTITEM.VIDITEM_330_SV[non_active_seg];
        //    for (int k = 0; k < non_active_seg; k++)
        //    {
        //        string non_active_segment_id = nonActiveSeg[k];
        //        List<ASECTION> sections = scApp.MapBLL.loadSectionsBySegmentID(non_active_segment_id);
        //        string start_point = sections.First().FROM_ADR_ID;
        //        string end_point = sections.Last().TO_ADR_ID;
        //        string lane_cut_type = SECSConst.LANECUTTYPE_LaneCutOnHMI;
        //        viditem_360.LANE_CUT_INFO[k] = new S6F11.RPTINFO.RPTITEM.VIDITEM_330_SV();
        //        viditem_360.LANE_CUT_INFO[k].LANE_INFO_OBJ.StartPoint = start_point;
        //        viditem_360.LANE_CUT_INFO[k].LANE_INFO_OBJ.EndPoint = end_point;
        //        viditem_360.LANE_CUT_INFO[k].LANE_CUT_TYPE = lane_cut_type;
        //    }
        //    return viditem_360;
        //}
        VehicleOperationState Convert2VehicleOperationState(AVEHICLE vh)
        {
            if (!vh.isTcpIpConnect)
            {
                return VehicleOperationState.Disconnected;
            }
            else if (vh.IsError)
            {
                return VehicleOperationState.Error;
            }
            else if (vh.ACT_STATUS == ProtocolFormat.OHTMessage.VHActionStatus.NoCommand)
            {
                return VehicleOperationState.Stooped;
            }
            else if (vh.ACT_STATUS == ProtocolFormat.OHTMessage.VHActionStatus.Commanding)
            {
                return VehicleOperationState.Operating;
            }
            else
            {
                return VehicleOperationState.Stooped;
            }
        }

        VehicleCommunictionState Convert2VehicleCommunication(AVEHICLE vh)
        {
            if (!vh.isTcpIpConnect)
            {
                return VehicleCommunictionState.Disconnected;
            }
            else
            {
                if (vh.IsCommunication(scApp.getBCFApplication()))
                {
                    return VehicleCommunictionState.Communicating;
                }
                else
                {
                    return VehicleCommunictionState.NoCommunicating;
                }
            }
        }
        VehicleControlMode Convert2VehicleControlMode(AVEHICLE vh)
        {
            if (!vh.isTcpIpConnect) return VehicleControlMode.Manual;
            if (vh.MODE_STATUS >= ProtocolFormat.OHTMessage.VHModeStatus.AutoRemote)
            {
                return VehicleControlMode.Auto;
            }
            else
            {
                return VehicleControlMode.Manual;
            }
        }
        VehicleJamState Convert2VehicleJamState(AVEHICLE vh)
        {
            return vh.IsBlocking ? VehicleJamState.JamExists : VehicleJamState.NoJan;
        }
        enum VehicleOperationState
        {
            Disconnected,
            Operating,
            Stooped,
            Error,
            Detached
        }
        enum VehicleCommunictionState
        {
            Disconnected,
            Communicating,
            NoCommunicating
        }
        enum VehicleControlMode
        {
            Manual,
            Auto
        }
        enum VehicleJamState
        {
            NoJan,
            JamExists,
            Stuck
        }


        #endregion Build VIDItem
        protected override void S1F15ReceiveRequestOffLine(object sender, SECSEventArgs e)
        {
            try
            {
                string msg = string.Empty;
                S1F15 s1f15 = ((S1F15)e.secsHandler.Parse<S1F15>(e));
                SCUtility.secsActionRecordMsg(scApp, true, s1f15);
                //if (isProcess(s1f15)) { return; }

                scApp.TransferService.TransferServiceLogger.Info(
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|s1f15:\n" + s1f15.toSECSString());

                S1F16 s1f16 = new S1F16();
                s1f16.SystemByte = s1f15.SystemByte;
                s1f16.SECSAgentName = scApp.EAPSecsAgentName;

                s1f16.OFLACK = SECSConst.OFLACK_Accepted;

                TrxSECS.ReturnCode rtnCode = ISECSControl.replySECS(bcfApp, s1f16);
                SCUtility.secsActionRecordMsg(scApp, false, s1f16);

                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ")
                        + "MCS >> OHB|s1f16 OFLACK:" + s1f16.OFLACK + "   SCES_ReturnCode:" + rtnCode);

                if (rtnCode != TrxSECS.ReturnCode.Normal)
                {
                    logger.Warn("Reply EQPT S1F18 Error:{0}", rtnCode);
                }

                if (SCUtility.isMatche(s1f16.OFLACK, SECSConst.OFLACK_Accepted))
                {
                    scApp.LineService.OfflineWithHostByHost();
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "  S1F15ReceiveRequestOffLine\n" + ex);

                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}", line.LINE_ID, "S1F17_Receive_OnlineRequest", ex.ToString());
            }
        }


        protected override void S1F17ReceiveRequestOnLine(object sender, SECSEventArgs e)
        {
            try
            {
                string msg = string.Empty;
                S1F17 s1f17 = ((S1F17)e.secsHandler.Parse<S1F17>(e));
                SCUtility.secsActionRecordMsg(scApp, true, s1f17);

                TransferServiceLogger.Info(
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|S1F17:\n" + s1f17.toSECSString());

                S1F18 s1f18 = new S1F18();
                s1f18.SystemByte = s1f17.SystemByte;
                s1f18.SECSAgentName = scApp.EAPSecsAgentName;

                //檢查狀態是否允許連線
                //if (DebugParameter.RejectEAPOnline)
                if (!scApp.LineService.canOnlineWithHost())
                {
                    s1f18.ONLACK = SECSConst.ONLACK_Not_Accepted;
                }
                else if (line.Host_Control_State == SCAppConstants.LineHostControlState.HostControlState.On_Line_Remote)
                {
                    s1f18.ONLACK = SECSConst.ONLACK_Equipment_Already_On_Line;
                    msg = "OHS is online remote ready!!"; //A0.05
                }
                else
                {
                    s1f18.ONLACK = SECSConst.ONLACK_Accepted;
                }

                TrxSECS.ReturnCode rtnCode = ISECSControl.replySECS(bcfApp, s1f18);
                SCUtility.secsActionRecordMsg(scApp, false, s1f18);

                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "MCS >> OHB|S1F18     SCES_ReturnCode:" + rtnCode + "\n" + s1f18.toSECSString());

                if (rtnCode != TrxSECS.ReturnCode.Normal)
                {
                    logger.Warn("Reply EQPT S1F18 Error:{0}", rtnCode);
                }

                if (SCUtility.isMatche(s1f18.ONLACK, SECSConst.ONLACK_Accepted))
                {
                    scApp.LineService.OnlineWithHostByHost();
                }
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "  S1F17ReceiveRequestOnLine\n" + ex);

                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}", line.LINE_ID, "S1F17_Receive_OnlineRequest", ex.ToString());
            }
        }

        //protected override void S5F5ReceiveListAlarmRequest(object sender, SECSEventArgs e)
        //{
        //    try
        //    {
        //        S5F5 s5f5 = ((S5F5)e.secsHandler.Parse<S5F5>(e));
        //        SCUtility.secsActionRecordMsg(scApp, true, s5f5);
        //        if (isProcess(s5f5)) { return; }
        //        string[] alarm_codes = s5f5.ALID?.Split(',');

        //        List<AlarmMap> alarm_maps = scApp.AlarmBLL.loadAlarmMaps();
        //        string[] alarm_ids = scApp.AlarmBLL.getCurrentAlarmsFromRedis().Select(alarm => alarm.ALAM_CODE).ToArray();
        //        S5F6 s5f6 = null;
        //        s5f6 = new S5F6();
        //        s5f6.SystemByte = s5f5.SystemByte;
        //        s5f6.SECSAgentName = scApp.EAPSecsAgentName;
        //        if (alarm_codes == null)//填所有Alarm資料到S5F6
        //        {
        //            s5f6.ALIDS = new S5F6.ALID_1[alarm_maps.Count];
        //            for (int i = 0; i < alarm_maps.Count; i++)
        //            {
        //                string alarm_id = alarm_maps[i].ALARM_ID;
        //                int ialarm_id = 0;
        //                int.TryParse(alarm_id, out ialarm_id);
        //                alarm_id = (ialarm_id < 0 ? ialarm_id * -1 : ialarm_id).ToString();
        //                s5f6.ALIDS[i] = new S5F6.ALID_1();
        //                bool is_set = alarm_ids.Contains(alarm_maps[i].ALARM_ID);
        //                s5f6.ALIDS[i].ALCD = is_set ? "1" : "0";
        //                s5f6.ALIDS[i].ALID = alarm_id;
        //                s5f6.ALIDS[i].ALTX = alarm_maps[i].ALARM_DESC;
        //            }
        //        }
        //        else
        //        {
        //            s5f6.ALIDS = new S5F6.ALID_1[alarm_codes.Length];
        //            for (int i = 0; i < alarm_codes.Length; i++)//填S5F6資料
        //            {
        //                s5f6.ALIDS[i] = new S5F6.ALID_1();
        //                if (string.IsNullOrEmpty(alarm_codes[i]))
        //                {
        //                    continue; //alarm_code空白不用填值
        //                }
        //                else
        //                {
        //                    foreach (AlarmMap a in alarm_maps)
        //                    {
        //                        if (SCUtility.isMatche(a.ALARM_ID, alarm_codes[i]))
        //                        {
        //                            string alarm_id = a.ALARM_ID;
        //                            int ialarm_id = 0;
        //                            int.TryParse(alarm_id, out ialarm_id);
        //                            alarm_id = (ialarm_id < 0 ? ialarm_id * -1 : ialarm_id).ToString();
        //                            bool is_set = alarm_ids.Contains(a.ALARM_ID);
        //                            s5f6.ALIDS[i].ALCD = is_set ? "1" : "0";
        //                            s5f6.ALIDS[i].ALID = alarm_id;
        //                            s5f6.ALIDS[i].ALTX = a.ALARM_DESC;
        //                            break;
        //                        }
        //                        else
        //                        {
        //                            continue;
        //                        }
        //                    }
        //                }
        //            }
        //        }


        //        TrxSECS.ReturnCode rtnCode = ISECSControl.replySECS(bcfApp, s5f6);
        //        SCUtility.secsActionRecordMsg(scApp, false, s5f6);
        //        if (rtnCode != TrxSECS.ReturnCode.Normal)
        //        {
        //            logger.Warn("Reply EQPT S5F6 Error:{0}", rtnCode);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}", line.LINE_ID, "S5F5ReceiveListAlarmRequest", ex.ToString());
        //    }
        //}
        #endregion Receive 

        #region Send
        #region other
        public override bool S1F13SendEstablishCommunicationRequest()
        {
            try
            {
                S1F13 s1f13 = new S1F13();
                s1f13.SECSAgentName = scApp.EAPSecsAgentName;
                s1f13.MDLN = scApp.getEQObjCacheManager().getLine().LINE_ID.Trim();
                s1f13.SOFTREV = SCApplication.getMessageString("SYSTEM_VERSION");

                S1F14 s1f14 = null;
                string rtnMsg = string.Empty;
                SXFY abortSecs = null;
                SCUtility.secsActionRecordMsg(scApp, false, s1f13);

                TrxSECS.ReturnCode rtnCode = ISECSControl.sendRecv<S1F14>(bcfApp, s1f13, out s1f14, out abortSecs, out rtnMsg, null);
                SCUtility.actionRecordMsg(scApp, s1f13.StreamFunction, line.Real_ID, "Establish Communication.", rtnCode.ToString());

                if (rtnCode == TrxSECS.ReturnCode.Normal)
                {
                    SCUtility.secsActionRecordMsg(scApp, true, s1f14);
                    if (SCUtility.isMatche(s1f14.COMMACK, SECSConst.COMMACK_ACK))
                    {
                        line.EstablishComm = true;
                    }
                    else
                    {
                        //當收到MCS的拒絕On 通訊以後，就把SECS的連線直接關閉
                        scApp.LineService.stopHostCommunication();
                    }
                    return true;
                }
                else
                {
                    line.EstablishComm = false;
                    logger.Warn("Send Establish Communication[S1F13] Error!");
                }
            }
            catch (Exception ex)
            {
                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}", line.LINE_ID, " sendS1F13_Establish_Comm", ex.ToString());
            }
            return false;
        }
        public override bool S5F1SendAlarmReport(string alcd, string alid, string altx)
        {
            try
            {
                S5F1 s5f1 = new S5F1()
                {
                    SECSAgentName = scApp.EAPSecsAgentName,
                    ALCD = alcd,
                    ALID = alid,
                    ALTX = altx
                };
                S5F2 s5f2 = null;
                SXFY abortSecs = null;
                String rtnMsg = string.Empty;
                if (isHostReady())
                {
                    TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> MCS|S5F1 SECSAgentName:" + s5f1.SECSAgentName
                                            + " ALCD:" + s5f1.ALCD
                                            + " ALID:" + s5f1.ALID
                                            + " ALTX:" + s5f1.ALTX
                                            );

                    TrxSECS.ReturnCode rtnCode = ISECSControl.sendRecv<S5F2>(bcfApp, s5f1, out s5f2,
                        out abortSecs, out rtnMsg, null);

                    TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|S5F2 ACKC5:" + s5f2.ACKC5);

                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(GEMDriver), Device: DEVICE_NAME_MCS,
                       Data: s5f1);
                    LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(GEMDriver), Device: DEVICE_NAME_MCS,
                       Data: s5f2);
                    SCUtility.actionRecordMsg(scApp, s5f1.StreamFunction, line.Real_ID,
                        "Send Alarm Report.", rtnCode.ToString());
                    if (rtnCode != TrxSECS.ReturnCode.Normal)
                    {
                        logger.Warn("Send Alarm Report[S5F1] Error![rtnCode={0}]", rtnCode);
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception:");
                return false;
            }
        }

        #endregion
        #region S6F11 Report
        public void SendS6F11(string ceid, VIDCollection vids)
        {
            AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(ceid, vids);
            scApp.ReportBLL.insertMCSReport(mcs_queue);
            S6F11SendMessage(mcs_queue);
        }
        public override bool S6F11SendEquiptmentOffLine()
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                Vids.VIDITEM_61_ECV_EqpName.EQPT_NAME = line.LINE_ID;
                SendS6F11(SECSConst.CEID_Equipment_OFF_LINE, Vids);
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }
        public override bool S6F11SendControlStateLocal()
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                //Vids.VIDITEM_61_ECV_EqpName.EQPT_NAME = line.LINE_ID;
                Vids.VIDITEM_06_SV_ControlState.CONTROLSTATE = ((int)line.Host_Control_State).ToString();
                SendS6F11(SECSConst.CEID_Control_Status_Local, Vids);
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }
        public override bool S6F11SendControlStateRemote()
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                Vids.VIDITEM_61_ECV_EqpName.EQPT_NAME = line.LINE_ID;
                SendS6F11(SECSConst.CEID_Control_Status_Remote, Vids);
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }
        public override bool S6F11SendAlarmCleared(ACMD_MCS CMD_MCS, ALARM ALARM, string unitid, string unitstate)
        {
            try
            {
                string cstLoc = scApp.CassetteDataBLL.GetCassetteLocByBoxID(CMD_MCS?.BOX_ID ?? "");
                VIDCollection Vids = new VIDCollection();
                Vids.VIDITEM_58_DVVAL_CommandID.COMMAND_ID = CMD_MCS?.CMD_ID ?? "";
                Vids.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = CMD_MCS?.CARRIER_ID ?? "";
                Vids.VIDITEM_179_DVVAL_BOXID.BOX_ID = CMD_MCS?.BOX_ID ?? "";
                Vids.VIDITEM_56_DVVAL_CarrierLoc.CARRIER_LOC = cstLoc;
                Vids.VIDITEM_81_DVVAL_AlarmID.ALARM_ID = ALARM.ALAM_CODE;
                Vids.VIDITEM_82_DVVAL_AlarmText.ALARM_TEXT = ALARM.ALAM_DESC;
                Vids.VIDITEM_63_DVVAL_ErrorId.ERRORID = ""; //ALARM.ERROR_ID
                Vids.VIDITEM_72_SV_UnitInfo.UNITID = new S6F11.RPTINFO.RPTITEM.VIDITEM_83_DVVAL();
                Vids.VIDITEM_72_SV_UnitInfo.UNITID.UNIT_ID = unitid;
                Vids.VIDITEM_72_SV_UnitInfo.UNITSTATE = new S6F11.RPTINFO.RPTITEM.VIDITEM_74_DVVAL();
                Vids.VIDITEM_72_SV_UnitInfo.UNITSTATE.Unit_State = unitstate;
                SendS6F11(SECSConst.CEID_Alarm_Cleared, Vids);
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }

        public override bool S6F11SendAlarmSet(ACMD_MCS CMD_MCS, ALARM ALARM, string unitid, string unitstate, string RecoveryOption)
        {
            try
            {
                string cstLoc = scApp.CassetteDataBLL.GetCassetteLocByBoxID(CMD_MCS?.BOX_ID ?? "");
                VIDCollection Vids = new VIDCollection();
                Vids.VIDITEM_58_DVVAL_CommandID.COMMAND_ID = CMD_MCS?.CMD_ID ?? "";
                Vids.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = CMD_MCS?.CARRIER_ID ?? "";
                Vids.VIDITEM_179_DVVAL_BOXID.BOX_ID = CMD_MCS?.BOX_ID ?? "";
                Vids.VIDITEM_56_DVVAL_CarrierLoc.CARRIER_LOC = cstLoc;
                Vids.VIDITEM_81_DVVAL_AlarmID.ALARM_ID = ALARM.ALAM_CODE;
                Vids.VIDITEM_82_DVVAL_AlarmText.ALARM_TEXT = ALARM.ALAM_DESC;
                Vids.VIDITEM_63_DVVAL_ErrorId.ERRORID = ""; //ALARM.ERROR_ID
                Vids.VIDITEM_72_SV_UnitInfo.UNITID = new S6F11.RPTINFO.RPTITEM.VIDITEM_83_DVVAL();
                Vids.VIDITEM_72_SV_UnitInfo.UNITID.UNIT_ID = unitid;
                Vids.VIDITEM_72_SV_UnitInfo.UNITSTATE = new S6F11.RPTINFO.RPTITEM.VIDITEM_74_DVVAL();
                Vids.VIDITEM_72_SV_UnitInfo.UNITSTATE.Unit_State = unitstate;
                Vids.VIDITEM_68_DVVAL_RecoeryOption.Recoery_Option = RecoveryOption;
                SendS6F11(SECSConst.CEID_Alarm_Set, Vids);
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }

        public override bool S6F11SendTSCAutoCompleted()
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                Vids.VIDITEM_61_ECV_EqpName.EQPT_NAME = line.LINE_ID;
                SendS6F11(SECSConst.CEID_TSC_Auto_Completed, Vids);
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }

        public override bool S6F11SendTSCAutoInitiated()
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                Vids.VIDITEM_61_ECV_EqpName.EQPT_NAME = line.LINE_ID;
                SendS6F11(SECSConst.CEID_TSC_Auto_Initiated, Vids);
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }
        public override bool S6F11SendTSCPauseCompleted()
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                Vids.VIDITEM_61_ECV_EqpName.EQPT_NAME = line.LINE_ID;
                SendS6F11(SECSConst.CEID_TSC_Pause_Completed, Vids);
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }
        public override bool S6F11SendTSCPaused()
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                Vids.VIDITEM_61_ECV_EqpName.EQPT_NAME = line.LINE_ID;
                SendS6F11(SECSConst.CEID_TSC_Paused, Vids);
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }

        public override bool S6F11SenSCPauseInitiated()
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                Vids.VIDITEM_61_ECV_EqpName.EQPT_NAME = line.LINE_ID;
                SendS6F11(SECSConst.CEID_TSC_Pause_Initiated, Vids);
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }
        public override bool S6F11SendTransferAbortCompleted(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                ACMD_MCS cmd = scApp.CMDBLL.getCMD_MCSByID(cmd_id);
                CassetteData cassette = scApp.CassetteDataBLL.loadCassetteDataByBoxID(cmd.BOX_ID);
                string cstID = cassette?.CSTID ?? "";
                string boxID = cassette?.BOXID ?? "";
                string loc = cassette?.Carrier_LOC ?? "";
                string zonename = scApp.CassetteDataBLL.GetZoneName(loc);

                Vids.VIDITEM_58_DVVAL_CommandID.COMMAND_ID = cmd.CMD_ID;
                Vids.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = cmd.CARRIER_ID;    //20/07/16 美微說 AbortInitiated、AbortCompleted，所帶的 CARRIER_ID、BOX 是填CMD的
                Vids.VIDITEM_56_DVVAL_CarrierLoc.CARRIER_LOC = loc;
                Vids.VIDITEM_370_DVVAL_CarrierZoneName.CARRIER_ZONE_NAME = zonename;
                Vids.VIDITEM_179_DVVAL_BOXID.BOX_ID = cmd.BOX_ID;
                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Transfer_Abort_Completed, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }

        public override bool S6F11SendTransferAbortFailed(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                ACMD_MCS cmd = scApp.CMDBLL.getCMD_MCSByID(cmd_id);
                CassetteData cassette = scApp.CassetteDataBLL.loadCassetteDataByCstBoxID(cmd.CARRIER_ID, cmd.BOX_ID);
                string cstID = cassette?.CSTID ?? "";
                string boxID = cassette?.BOXID ?? "";
                string loc = cassette?.Carrier_LOC ?? "";
                string zonename = scApp.CassetteDataBLL.GetZoneName(loc);

                Vids.VIDITEM_58_DVVAL_CommandID.COMMAND_ID = cmd.CMD_ID;
                Vids.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = cstID;
                Vids.VIDITEM_56_DVVAL_CarrierLoc.CARRIER_LOC = loc;
                Vids.VIDITEM_370_DVVAL_CarrierZoneName.CARRIER_ZONE_NAME = zonename;
                Vids.VIDITEM_179_DVVAL_BOXID.BOX_ID = boxID;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Transfer_Abort_Failed, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }

        public override bool S6F11SendTransferAbortInitiated(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                ACMD_MCS cmd = scApp.CMDBLL.getCMD_MCSByID(cmd_id);

                CassetteData cassette = scApp.CassetteDataBLL.loadCassetteDataByBoxID(cmd.BOX_ID);
                string cstID = cassette?.CSTID ?? "";
                string boxID = cassette?.BOXID ?? "";
                string loc = cassette?.Carrier_LOC ?? "";
                string zonename = scApp.CassetteDataBLL.GetZoneName(loc);

                Vids.VIDITEM_58_DVVAL_CommandID.COMMAND_ID = cmd.CMD_ID;
                Vids.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = cmd.CARRIER_ID;
                Vids.VIDITEM_56_DVVAL_CarrierLoc.CARRIER_LOC = loc;
                Vids.VIDITEM_370_DVVAL_CarrierZoneName.CARRIER_ZONE_NAME = zonename;
                Vids.VIDITEM_179_DVVAL_BOXID.BOX_ID = cmd.BOX_ID;
                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Transfer_Abort_Initiated, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }
        public override bool S6F11SendTransferCancelCompleted(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                ACMD_MCS cmd = scApp.CMDBLL.getCMD_MCSByID(cmd_id);

                CassetteData cassette = scApp.CassetteDataBLL.loadCassetteDataByBoxID(cmd.BOX_ID);
                string cstID = cassette?.CSTID ?? "";
                string boxID = cassette?.BOXID ?? "";
                string loc = cassette?.Carrier_LOC ?? "";
                string zonename = scApp.CassetteDataBLL.GetZoneName(loc);

                Vids.VIDITEM_58_DVVAL_CommandID.COMMAND_ID = cmd.CMD_ID;
                Vids.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = cstID;
                Vids.VIDITEM_56_DVVAL_CarrierLoc.CARRIER_LOC = loc;
                Vids.VIDITEM_370_DVVAL_CarrierZoneName.CARRIER_ZONE_NAME = zonename;
                Vids.VIDITEM_179_DVVAL_BOXID.BOX_ID = boxID;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Transfer_Cancel_Completed, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
                return false;
            }
        }
        public override bool S6F11SendTransferCancelFailed(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                ACMD_MCS cmd = scApp.CMDBLL.getCMD_MCSByID(cmd_id);

                CassetteData cassette = scApp.CassetteDataBLL.loadCassetteDataByBoxID(cmd.BOX_ID);
                string cstID = cassette?.CSTID ?? "";
                string boxID = cassette?.BOXID ?? "";
                string loc = cassette?.Carrier_LOC ?? "";
                string zonename = scApp.CassetteDataBLL.GetZoneName(loc);

                Vids.VIDITEM_58_DVVAL_CommandID.COMMAND_ID = cmd.CMD_ID;
                Vids.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = cstID;
                Vids.VIDITEM_56_DVVAL_CarrierLoc.CARRIER_LOC = loc;
                Vids.VIDITEM_370_DVVAL_CarrierZoneName.CARRIER_ZONE_NAME = zonename;
                Vids.VIDITEM_179_DVVAL_BOXID.BOX_ID = boxID;
                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Transfer_Cancel_Failed, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
                return false;
            }
        }

        public override bool S6F11SendTransferCancelInitial(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                ACMD_MCS cmd = scApp.CMDBLL.getCMD_MCSByID(cmd_id);
                CassetteData cassette = scApp.CassetteDataBLL.loadCassetteDataByBoxID(cmd.BOX_ID);

                string cstID = "";
                string boxID = "";
                string cstLoc = "";
                string zonename = "";

                if (cassette != null)
                {
                    cstID = cassette.CSTID.Trim();
                    boxID = cassette.BOXID.Trim();
                    cstLoc = cassette.Carrier_LOC;
                    zonename = scApp.CassetteDataBLL.GetZoneName(cassette.Carrier_LOC);
                }

                Vids.VIDITEM_58_DVVAL_CommandID.COMMAND_ID = cmd.CMD_ID;
                Vids.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = cstID;
                Vids.VIDITEM_56_DVVAL_CarrierLoc.CARRIER_LOC = cstLoc;
                Vids.VIDITEM_370_DVVAL_CarrierZoneName.CARRIER_ZONE_NAME = zonename;
                Vids.VIDITEM_179_DVVAL_BOXID.BOX_ID = boxID;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Transfer_Cancel_Initiated, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
                return false;
            }
        }

        public override bool S6F11SendTransferCompleted(ACMD_MCS cmd, CassetteData cassette, string result_code, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                //if (!isSend()) return true;
                VIDCollection Vids = new VIDCollection();
                //ACMD_MCS cmd = scApp.CMDBLL.getCMD_MCSByID(cmd_id);
                //CassetteData cassette = scApp.CassetteDataBLL.loadCassetteDataByBoxID(cmd.BOX_ID.Trim());

                if (cassette == null)
                {
                    cassette = scApp.CassetteDataBLL.loadCassetteDataByBoxID(cmd.BOX_ID.Trim());
                }
                
                string zonename = scApp.CassetteDataBLL.GetZoneName(cassette.Carrier_LOC);

                Vids.VIDITEM_58_DVVAL_CommandID.COMMAND_ID = cmd?.CMD_ID ?? "";
                Vids.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = cassette?.CSTID ?? "";
                Vids.VIDITEM_56_DVVAL_CarrierLoc.CARRIER_LOC = cassette?.Carrier_LOC ?? "";
                Vids.VIDITEM_370_DVVAL_CarrierZoneName.CARRIER_ZONE_NAME = zonename;
                Vids.VIDITEM_64_DVVAL_ResultCode.RESULT_CODE = result_code;
                Vids.VIDITEM_179_DVVAL_BOXID.BOX_ID = cassette?.BOXID ?? "";

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Transfer_Completed, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
                return false;
            }
        }

        public override bool S6F11SendTransferInitiated(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                //if (!isSend()) return true;
                VIDCollection Vids = new VIDCollection();
                ACMD_MCS cmd = scApp.CMDBLL.getCMD_MCSByID(cmd_id);
                CassetteData cassette = scApp.CassetteDataBLL.loadCassetteDataByBoxID(cmd.BOX_ID);
                string cmdID = cmd?.CMD_ID ?? "";
                string cstID = cmd?.CARRIER_ID ?? "";
                string cstLoc = cassette?.Carrier_LOC ?? "";
                string zonename = scApp.CassetteDataBLL.GetZoneName(cstLoc);

                Vids.VIDITEM_58_DVVAL_CommandID.COMMAND_ID = cmdID;
                Vids.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = cstID;
                Vids.VIDITEM_56_DVVAL_CarrierLoc.CARRIER_LOC = cstLoc;
                Vids.VIDITEM_370_DVVAL_CarrierZoneName.CARRIER_ZONE_NAME = zonename;
                //Vids.VIDITEM_60_DVVAL_DestPort.DESTINATION_ID = cmd.HOSTDESTINATION;

                if (scApp.TransferService.isShelfPort(cmd.HOSTDESTINATION))
                {
                    Vids.VIDITEM_60_DVVAL_DestPort.DESTINATION_ID = scApp.CassetteDataBLL.GetZoneName(cmd.HOSTDESTINATION);
                }
                else
                {
                    Vids.VIDITEM_60_DVVAL_DestPort.DESTINATION_ID = cmd.HOSTDESTINATION;
                }

                Vids.VIDITEM_179_DVVAL_BOXID.BOX_ID = cmd.BOX_ID;
                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Transfer_Initiated, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    TransferServiceLogger.Error(DateTime.Now.ToString("HH:mm:ss.fff ") + "S6F11SendTransferInitiated    reportQueues = Null");
                    reportQueues.Add(mcs_queue);
                }
                return true;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(DateTime.Now.ToString("HH:mm:ss.fff ") + "S6F11SendTransferInitiated\n" + ex);

                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
                return false;
            }
        }
        public override bool S6F11SendTransferPaused(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                ACMD_MCS cmd = scApp.CMDBLL.getCMD_MCSByID(cmd_id);

                CassetteData cassette = scApp.CassetteDataBLL.loadCassetteDataByBoxID(cmd.BOX_ID);
                string cstID = cassette?.CSTID ?? "";
                string boxID = cassette?.BOXID ?? "";
                string loc = cassette?.Carrier_LOC ?? "";
                string zonename = scApp.CassetteDataBLL.GetZoneName(loc);

                Vids.VIDITEM_58_DVVAL_CommandID.COMMAND_ID = cmd.CMD_ID;
                Vids.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = cstID;
                Vids.VIDITEM_56_DVVAL_CarrierLoc.CARRIER_LOC = loc;
                Vids.VIDITEM_370_DVVAL_CarrierZoneName.CARRIER_ZONE_NAME = zonename;
                Vids.VIDITEM_179_DVVAL_BOXID.BOX_ID = boxID;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Transfer_Pause, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
                return false;
            }
        }

        public override bool S6F11SendTransferResume(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                ACMD_MCS cmd = scApp.CMDBLL.getCMD_MCSByID(cmd_id);

                CassetteData cassette = scApp.CassetteDataBLL.loadCassetteDataByBoxID(cmd.BOX_ID);
                string cstID = cassette?.CSTID ?? "";
                string boxID = cassette?.BOXID ?? "";
                string loc = cassette?.Carrier_LOC ?? "";
                string zonename = scApp.CassetteDataBLL.GetZoneName(loc);

                Vids.VIDITEM_58_DVVAL_CommandID.COMMAND_ID = cmd.CMD_ID;
                Vids.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = cstID;
                Vids.VIDITEM_56_DVVAL_CarrierLoc.CARRIER_LOC = loc;
                Vids.VIDITEM_370_DVVAL_CarrierZoneName.CARRIER_ZONE_NAME = zonename;
                Vids.VIDITEM_179_DVVAL_BOXID.BOX_ID = boxID;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Transfer_Resumed, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
                return false;
            }
        }
        public override bool S6F11SendCarrierTransferring(ACMD_MCS cmd, CassetteData cassette, string ohtName, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                //if (!isSend()) return true;
                VIDCollection Vids = new VIDCollection();
                //ACMD_MCS cmd = scApp.CMDBLL.getCMD_MCSByID(cmd_id);
                //ACMD_OHTC cmd_oht = scApp.CMDBLL.getCMD_OHTCByID(cmd_id);
                //CassetteData cassette = scApp.CassetteDataBLL.loadCassetteDataByBoxID(cmd.BOX_ID);
                //ACMD_OHTC ohtc = scApp.CMD_OHTCDao.get
                string zonename = scApp.CassetteDataBLL.GetZoneName(cassette.Carrier_LOC);

                Vids.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = cassette.CSTID;
                Vids.VIDITEM_56_DVVAL_CarrierLoc.CARRIER_LOC = cassette.Carrier_LOC;
                Vids.VIDITEM_370_DVVAL_CarrierZoneName.CARRIER_ZONE_NAME = zonename;
                Vids.VIDITEM_70_DVVAL_CraneID.Crane_ID = ohtName;
                Vids.VIDITEM_179_DVVAL_BOXID.BOX_ID = cassette.BOXID;
                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Carrier_Transferring, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
                return false;
            }
        }
        public override bool S6F11SendCarrierInstallCompleted(CassetteData cst, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                //if (!isSend()) return true;
                VIDCollection Vids = new VIDCollection();
                //var cassette = scApp.CassetteDataBLL.loadCassetteDataByCSTID(cst_id);
                string zonename = scApp.CassetteDataBLL.GetZoneName(cst.Carrier_LOC);

                Vids.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = cst.CSTID;
                Vids.VIDITEM_56_DVVAL_CarrierLoc.CARRIER_LOC = cst.Carrier_LOC;
                Vids.VIDITEM_370_DVVAL_CarrierZoneName.CARRIER_ZONE_NAME = zonename;
                Vids.VIDITEM_179_DVVAL_BOXID.BOX_ID = cst.BOXID;
                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Carrier_Installed_Completed, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
                return false;
            }
        }
        public override bool S6F11SendCarrierRemovedCompleted(string cst_id, string box_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                //if (!isSend()) return true;
                VIDCollection Vids = new VIDCollection();
                //var aa = scApp.CassetteDataBLL.loadCassetteData();
                var cassette = scApp.CassetteDataBLL.loadCassetteDataByCstBoxID(cst_id, box_id);
                string zonename = scApp.CassetteDataBLL.GetZoneName(cassette.Carrier_LOC);

                Vids.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = cassette.CSTID;
                Vids.VIDITEM_56_DVVAL_CarrierLoc.CARRIER_LOC = cassette.Carrier_LOC;
                Vids.VIDITEM_370_DVVAL_CarrierZoneName.CARRIER_ZONE_NAME = zonename;
                Vids.VIDITEM_179_DVVAL_BOXID.BOX_ID = cassette.BOXID;

                scApp.CassetteDataBLL.DeleteCSTbyCstBoxID(cassette.CSTID, cassette.BOXID);

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Carrier_Removed_Completed, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
                return false;
            }
        }
        public override bool S6F11SendCarrierRemovedFromPort(CassetteData cst, string Handoff_Type, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                //if (!isSend()) return true;
                VIDCollection Vids = new VIDCollection();
                string zonename = scApp.CassetteDataBLL.GetZoneName(cst.Carrier_LOC);

                Vids.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = cst.CSTID;
                Vids.VIDITEM_179_DVVAL_BOXID.BOX_ID = cst.BOXID;
                Vids.VIDITEM_66_DVVAL_HandoffType.Handoff_Type = Handoff_Type;
                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Carrier_Removed_Port, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
                return false;
            }
        }

        public override bool S6F11SendCarrierResumed(string cmd_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                ACMD_MCS cmd = scApp.CMDBLL.getCMD_MCSByID(cmd_id);
                //ACMD_OHTC cmd_oht = scApp.CMDBLL.getCMD_OHTCByID(cmd_id);
                //CassetteData cassette = scApp.CassetteDataBLL.loadCassetteDataByCSTID(cmd.CARRIER_ID);

                CassetteData cassette = scApp.CassetteDataBLL.loadCassetteDataByBoxID(cmd.BOX_ID);
                string cstID = cassette?.CSTID ?? "";
                string boxID = cassette?.BOXID ?? "";
                string loc = cassette?.Carrier_LOC ?? "";
                string zonename = scApp.CassetteDataBLL.GetZoneName(loc);

                Vids.VIDITEM_58_DVVAL_CommandID.COMMAND_ID = cmd.CMD_ID;
                Vids.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = cstID;
                Vids.VIDITEM_56_DVVAL_CarrierLoc.CARRIER_LOC = loc;
                Vids.VIDITEM_370_DVVAL_CarrierZoneName.CARRIER_ZONE_NAME = zonename;

                //Vids.VIDITEM_60_DVVAL_DestPort.DESTINATION_ID = cmd.HOSTDESTINATION;

                if (scApp.ShelfDefBLL.isExist(cmd.HOSTDESTINATION))
                {
                    Vids.VIDITEM_60_DVVAL_DestPort.DESTINATION_ID = zonename;
                }
                else
                {
                    Vids.VIDITEM_60_DVVAL_DestPort.DESTINATION_ID = cmd.HOSTDESTINATION;
                }

                Vids.VIDITEM_70_DVVAL_CraneID.Crane_ID = cmd.CRANE;
                Vids.VIDITEM_179_DVVAL_BOXID.BOX_ID = cassette.BOXID;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Carrier_Resumed, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
                return false;
            }
        }

        public override bool S6F11SendCarrierStored(CassetteData cst, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                //if (!isSend()) return true;
                VIDCollection Vids = new VIDCollection();
                string zonename = scApp.CassetteDataBLL.GetZoneName(cst.Carrier_LOC);

                Vids.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = cst.CSTID;
                Vids.VIDITEM_56_DVVAL_CarrierLoc.CARRIER_LOC = cst.Carrier_LOC;
                Vids.VIDITEM_370_DVVAL_CarrierZoneName.CARRIER_ZONE_NAME = zonename;
                Vids.VIDITEM_179_DVVAL_BOXID.BOX_ID = cst.BOXID;
                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Carrier_Stored, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
                return false;
            }
        }

        public override bool S6F11SendCarrierStoredAlt(ACMD_MCS cmd, CassetteData cassette, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                //ACMD_MCS cmd = scApp.CMDBLL.getCMD_MCSByID(cmd_id);
                //ACMD_OHTC cmd_oht = scApp.CMDBLL.getCMD_OHTCByID(cmd_id);
                //CassetteData cassette = scApp.CassetteDataBLL.loadCassetteDataByCSTID(cmd.CARRIER_ID);
                //ACMD_OHTC ohtc = scApp.CMD_OHTCDao.get
                string zonename = scApp.CassetteDataBLL.GetZoneName(cassette.Carrier_LOC);

                Vids.VIDITEM_58_DVVAL_CommandID.COMMAND_ID = cmd.CMD_ID;
                Vids.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = cassette.CSTID;
                Vids.VIDITEM_56_DVVAL_CarrierLoc.CARRIER_LOC = cassette.Carrier_LOC;
                Vids.VIDITEM_370_DVVAL_CarrierZoneName.CARRIER_ZONE_NAME = zonename;
                Vids.VIDITEM_60_DVVAL_DestPort.DESTINATION_ID = cmd.HOSTDESTINATION;

                //if (scApp.ShelfDefBLL.isExist(cmd.HOSTDESTINATION))
                //{
                //    Vids.VIDITEM_60_DVVAL_DestPort.DESTINATION_ID = zonename;
                //}
                //else
                //{
                //    Vids.VIDITEM_60_DVVAL_DestPort.DESTINATION_ID = cmd.HOSTDESTINATION;
                //}

                Vids.VIDITEM_179_DVVAL_BOXID.BOX_ID = cmd.BOX_ID;
                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Carrier_Stored_Alt, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
                return false;
            }
        }

        public override bool S6F11SendShelfStatusChange(ZoneDef zone, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                //if (!isSend()) return true;
                VIDCollection Vids = new VIDCollection();
                //List<ZoneDef> zones = scApp.ZoneDefBLL.loadZoneData();
                int capacity = scApp.ZoneDefBLL.GetZoneCapacity(zone.ZoneID);
                int totalsize = scApp.ZoneDefBLL.GetZoneTotalSize(zone.ZoneID);

                Vids.VIDITEM_172_SV_ZoneData.ZONE_NAME = zone.ZoneName;
                Vids.VIDITEM_172_SV_ZoneData.ZONE_CAPACITY_OBJ.Zone_Capacity = capacity.ToString();
                Vids.VIDITEM_172_SV_ZoneData.ZONE_TOTAL_SIZE_OBJ.Zone_Total_Size = totalsize.ToString();
                Vids.VIDITEM_172_SV_ZoneData.ZONE_TYPE_OBJ.Zone_Type = zone.ZoneType.ToString();
                Vids.VIDITEM_172_SV_ZoneData.DISABLE_LOCATIONS_OBJ = new S6F11.RPTINFO.RPTITEM.VIDITEM_888_SV();

                List<ShelfDef> disShelf = scApp.ZoneDefBLL.GetDisShelf(zone.ZoneID);
                Vids.VIDITEM_172_SV_ZoneData.DISABLE_LOCATIONS_OBJ.DISABLE_LOC_OBJ
                    = new S6F11.RPTINFO.RPTITEM.VIDITEM_889_SV[disShelf.Count];

                for (int i = 0; i < disShelf.Count; i++)
                {
                    Vids.VIDITEM_172_SV_ZoneData.DISABLE_LOCATIONS_OBJ.DISABLE_LOC_OBJ[i]
                        = new S6F11.RPTINFO.RPTITEM.VIDITEM_889_SV();
                    string cstid = scApp.CassetteDataBLL.loadCassetteDataByShelfID(disShelf[i].ShelfID)?.CSTID ?? "";
                    Vids.VIDITEM_172_SV_ZoneData.DISABLE_LOCATIONS_OBJ.DISABLE_LOC_OBJ[i].CARRIER_LOC_OBJ.CARRIER_LOC = disShelf[i].ShelfID;
                    Vids.VIDITEM_172_SV_ZoneData.DISABLE_LOCATIONS_OBJ.DISABLE_LOC_OBJ[i].CARRIER_ID_OBJ.CARRIER_ID = cstid;
                }

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Shelf_Status_Change, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
                return false;
            }
        }

        public override bool S6F11SendCarrierWaitIn(CassetteData cst, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                //if (!isSend()) return true;
                VIDCollection Vids = new VIDCollection();
                string zonename = scApp.CassetteDataBLL.GetZoneName(cst.Carrier_LOC);

                Vids.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = cst.CSTID;
                Vids.VIDITEM_56_DVVAL_CarrierLoc.CARRIER_LOC = cst.Carrier_LOC;
                Vids.VIDITEM_370_DVVAL_CarrierZoneName.CARRIER_ZONE_NAME = zonename;
                Vids.VIDITEM_179_DVVAL_BOXID.BOX_ID = cst.BOXID;
                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Carrier_Wait_In, Vids);
                if (reportQueues == null)
                {
                    if (S6F11SendMessage(mcs_queue))
                    {
                        scApp.CassetteDataBLL.UpdateCSTState(cst.BOXID, (int)E_CSTState.WaitIn);
                        scApp.TransferService.SetWaitInOutLog(cst, E_CSTState.WaitIn);
                    }
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
                return false;
            }
        }


        public override bool S6F11SendCarrierWaitOut(CassetteData cst, string portType, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                //if (!isSend()) return true;
                VIDCollection Vids = new VIDCollection();
                string zonename = scApp.CassetteDataBLL.GetZoneName(cst.Carrier_LOC);

                Vids.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = cst.CSTID;
                Vids.VIDITEM_56_DVVAL_CarrierLoc.CARRIER_LOC = cst.Carrier_LOC;
                Vids.VIDITEM_370_DVVAL_CarrierZoneName.CARRIER_ZONE_NAME = zonename;
                //if (cst.Stage != 0)
                //{
                //    Vids.VIDITEM_370_DVVAL_CarrierZoneName.CARRIER_ZONE_NAME = zonename.Remove(9);
                //}
                Vids.VIDITEM_179_DVVAL_BOXID.BOX_ID = cst.BOXID;
                Vids.VIDITEM_116_DVVAL_PortType.PORT_TYPE = portType;
                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Carrier_Wait_Out, Vids);
                if (reportQueues == null)
                {
                    if(S6F11SendMessage(mcs_queue))
                    {
                        scApp.CassetteDataBLL.UpdateCSTState(cst.BOXID, (int)E_CSTState.WaitOut);
                        scApp.TransferService.SetWaitInOutLog(cst, E_CSTState.WaitOut);
                    }
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
                return false;
            }
        }

        public override bool S6F11SendUnitAlarmSet(string unitID, string alarmID, string alarmTest, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                Vids.VIDITEM_83_DVVAL_UnitID.UNIT_ID = unitID;
                Vids.VIDITEM_81_DVVAL_AlarmID.ALARM_ID = alarmID;
                Vids.VIDITEM_82_DVVAL_AlarmText.ALARM_TEXT = alarmTest;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Unit_Alarm_Set, Vids);
                scApp.ReportBLL.insertMCSReport(mcs_queue);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;

        }

        public override bool S6F11SendUnitAlarmCleared(string unitID, string alarmID, string alarmTest, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();

                Vids.VIDITEM_83_DVVAL_UnitID.UNIT_ID = unitID;
                Vids.VIDITEM_81_DVVAL_AlarmID.ALARM_ID = alarmID;
                Vids.VIDITEM_82_DVVAL_AlarmText.ALARM_TEXT = alarmTest;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Unit_Alarm_Cleared, Vids);
                scApp.ReportBLL.insertMCSReport(mcs_queue);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }

        public override bool S6F11SendCraneActive(string cmdID, string craneID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                //ACMD_OHTC ohtc = scApp.CMDBLL.getCMD_OHTCByVehicleID(craneID);
                Vids.VIDITEM_58_DVVAL_CommandID.COMMAND_ID = cmdID;   //要上報的是MCS cmd ID
                Vids.VIDITEM_70_DVVAL_CraneID.Crane_ID = craneID;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Crane_Active, Vids);
                scApp.ReportBLL.insertMCSReport(mcs_queue);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }

        public override bool S6F11SendCraneIdle(string craneID, string cmdID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                //ACMD_OHTC ohtc = scApp.CMDBLL.getCMD_OHTCByVehicleID(craneID.Trim());
                Vids.VIDITEM_58_DVVAL_CommandID.COMMAND_ID = cmdID;
                Vids.VIDITEM_70_DVVAL_CraneID.Crane_ID = craneID;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Crane_Idle, Vids);
                scApp.ReportBLL.insertMCSReport(mcs_queue);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }

        public override bool S6F11SendCraneInEscape(string craneID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                Vids.VIDITEM_70_DVVAL_CraneID.Crane_ID = craneID;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Crane_In_Escape, Vids);
                scApp.ReportBLL.insertMCSReport(mcs_queue);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }

        public override bool S6F11SendCraneOutEscape(string craneID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                Vids.VIDITEM_70_DVVAL_CraneID.Crane_ID = craneID;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Crane_Out_Escape, Vids);
                scApp.ReportBLL.insertMCSReport(mcs_queue);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }

        public override bool S6F11SendCraneOutServce(string craneID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                Vids.VIDITEM_70_DVVAL_CraneID.Crane_ID = craneID;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Crane_Out_Servce, Vids);
                scApp.ReportBLL.insertMCSReport(mcs_queue);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }

        public override bool S6F11SendCraneInServce(string craneID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                Vids.VIDITEM_70_DVVAL_CraneID.Crane_ID = craneID;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Crane_In_Servce, Vids);
                scApp.ReportBLL.insertMCSReport(mcs_queue);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }

        public override bool S6F11SendCarrierIDRead(CassetteData cst, string IDreadStatus, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                //CassetteData cst = scApp.CassetteDataBLL.loadCassetteDataByBoxID(BOXID);
                Vids.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = cst.CSTID;
                Vids.VIDITEM_179_DVVAL_BOXID.BOX_ID = cst.BOXID;
                Vids.VIDITEM_56_DVVAL_CarrierLoc.CARRIER_LOC = cst.Carrier_LOC;
                Vids.VIDITEM_67_DVVAL_IDreadStatus.Carrier_ID_Read_Status = IDreadStatus;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Carrier_ID_Read, Vids);
                scApp.ReportBLL.insertMCSReport(mcs_queue);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }

        public override bool S6F11SendZoneCapacityChange(string loc, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();

                ShelfDef targetShelf = scApp.ShelfDefBLL.loadShelfDataByID(loc);
                ZoneDef zone = scApp.ZoneDefBLL.loadZoneDataByID(targetShelf.ZoneID);

                int capacity = scApp.ZoneDefBLL.GetZoneCapacity(zone.ZoneID);
                int totalsize = scApp.ZoneDefBLL.GetZoneTotalSize(zone.ZoneID);

                Vids.VIDITEM_172_SV_ZoneData.ZONE_NAME = zone.ZoneName;
                Vids.VIDITEM_172_SV_ZoneData.ZONE_CAPACITY_OBJ.Zone_Capacity = capacity.ToString();
                Vids.VIDITEM_172_SV_ZoneData.ZONE_TOTAL_SIZE_OBJ.Zone_Total_Size = totalsize.ToString();
                Vids.VIDITEM_172_SV_ZoneData.ZONE_TYPE_OBJ.Zone_Type = zone.ZoneType.ToString();
                Vids.VIDITEM_172_SV_ZoneData.DISABLE_LOCATIONS_OBJ = new S6F11.RPTINFO.RPTITEM.VIDITEM_888_SV();

                List<ShelfDef> disShelf = scApp.ZoneDefBLL.GetDisShelf(zone.ZoneID);
                Vids.VIDITEM_172_SV_ZoneData.DISABLE_LOCATIONS_OBJ.DISABLE_LOC_OBJ
                    = new S6F11.RPTINFO.RPTITEM.VIDITEM_889_SV[disShelf.Count];

                for (int i = 0; i < disShelf.Count; i++)
                {
                    Vids.VIDITEM_172_SV_ZoneData.DISABLE_LOCATIONS_OBJ.DISABLE_LOC_OBJ[i]
                        = new S6F11.RPTINFO.RPTITEM.VIDITEM_889_SV();
                    string cstid = scApp.CassetteDataBLL.loadCassetteDataByShelfID(disShelf[i].ShelfID)?.CSTID ?? "";
                    Vids.VIDITEM_172_SV_ZoneData.DISABLE_LOCATIONS_OBJ.DISABLE_LOC_OBJ[i].CARRIER_LOC_OBJ.CARRIER_LOC = disShelf[i].ShelfID;
                    Vids.VIDITEM_172_SV_ZoneData.DISABLE_LOCATIONS_OBJ.DISABLE_LOC_OBJ[i].CARRIER_ID_OBJ.CARRIER_ID = cstid;
                }

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Zone_Capacity_Change, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
                return false;
            }
        }

        public override bool S6F11SendOperatorInitiatedAction(string cmd_id, string cmd_type, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                ACMD_MCS cmd = scApp.CMDBLL.getCMD_MCSByID(cmd_id);
                ACMD_OHTC cmd_oht = scApp.CMDBLL.getCMD_OHTCByID(cmd_id);

                CassetteData cassette = scApp.CassetteDataBLL.loadCassetteDataByBoxID(cmd.BOX_ID);
                string cstID = cassette?.CSTID ?? "";
                string boxID = cassette?.BOXID ?? "";
                string loc = cassette?.Carrier_LOC ?? "";
                string zonename = scApp.CassetteDataBLL.GetZoneName(loc);

                Vids.VIDITEM_58_DVVAL_CommandID.COMMAND_ID = cmd.CMD_ID;                
                Vids.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = cstID;
                Vids.VIDITEM_56_DVVAL_CarrierLoc.CARRIER_LOC = loc;
                Vids.VIDITEM_370_DVVAL_CarrierZoneName.CARRIER_ZONE_NAME = zonename;

                Vids.VIDITEM_65_DVVAL_SourceID.SOURCE_ID = cmd.HOSTSOURCE;
                Vids.VIDITEM_60_DVVAL_DestPort.DESTINATION_ID = cmd.HOSTDESTINATION;
                Vids.VIDITEM_80_DVVAL_CommandType.COMMAND_TYPE = cmd_type;
                Vids.VIDITEM_179_DVVAL_BOXID.BOX_ID = boxID;

                //if (scApp.TransferService.isShelfPort(cmd.HOSTDESTINATION))
                //{
                //    Vids.VIDITEM_60_DVVAL_DestPort.DESTINATION_ID = scApp.CassetteDataBLL.GetZoneName(cmd.HOSTDESTINATION);
                //}
                //else
                //{
                //    Vids.VIDITEM_60_DVVAL_DestPort.DESTINATION_ID = cmd.HOSTDESTINATION;
                //}

                Vids.VIDITEM_62_DVVAL_Priority.PRIORITY = cmd.PRIORITY_SUM.ToString();
                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Operator_Initiated_Action, Vids);
                if (reportQueues == null)
                {
                    Task.Run(() =>
                    {
                        S6F11SendMessage(mcs_queue);
                    });
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
                return false;
            }
        }



        public override bool S6F11SendPortOutOfService(string port_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                //Vids.VIDITEM_61_ECV_EqpName.EQPT_NAME = line.LINE_ID;
                Vids.VIDITEM_115_DVVAL_PortID.PORT_ID = port_id;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Port_Out_Of_Service, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }


                return true;

            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }
        public override bool S6F11SendPortInService(string port_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                //Vids.VIDITEM_61_ECV_EqpName.EQPT_NAME = line.LINE_ID;
                Vids.VIDITEM_115_DVVAL_PortID.PORT_ID = port_id;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Port_In_Service, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }

        public override bool S6F11SendLoadReq(string port_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                Vids.VIDITEM_115_DVVAL_PortID.PORT_ID = port_id;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Load_Req, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }

        public override bool S6F11SendUnLoadReq(string port_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                Vids.VIDITEM_115_DVVAL_PortID.PORT_ID = port_id;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Unload_Req, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }
        public override bool S6F11SendNoReq(string port_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                Vids.VIDITEM_115_DVVAL_PortID.PORT_ID = port_id;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_No_Req, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }

        public override bool S6F11SendPortTypeInput(string port_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                Vids.VIDITEM_115_DVVAL_PortID.PORT_ID = port_id;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Port_Type_Input, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }

        public override bool S6F11SendPortTypeOutput(string port_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                Vids.VIDITEM_115_DVVAL_PortID.PORT_ID = port_id;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Port_Type_Output, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }

        public override bool S6F11SendPortTypeChanging(string port_id, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                Vids.VIDITEM_115_DVVAL_PortID.PORT_ID = port_id;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Port_Type_Changing, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }

        public override bool S6F11SendCarrierBoxIDRename(string cstID, string boxID, string cstLOC, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                Vids.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = cstID;
                Vids.VIDITEM_179_DVVAL_BOXID.BOX_ID = boxID;
                Vids.VIDITEM_56_DVVAL_CarrierLoc.CARRIER_LOC = cstLOC;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Carrier_Box_ID_Rename, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }

        public override bool S6F11SendEmptyBoxSupply(string ReqCount, string zoneName, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                Vids.VIDITEM_890_DVVAL_RequestCount.REQUEST_COUNT = ReqCount;
                Vids.VIDITEM_370_DVVAL_CarrierZoneName.CARRIER_ZONE_NAME = zoneName;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Empty_Box_Supply, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);

                return false;
            }
            return true;
        }

        public override bool S6F11SendEmptyBoxRecycling(string boxID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                Vids.VIDITEM_179_DVVAL_BOXID.BOX_ID = boxID;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Empty_Box_Recycling, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);
            }
            return true;
        }

        public override bool S6F11SendQueryLotID(string cstID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                Vids.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = cstID;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_QueryLotID, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);

                return false;
            }
            return true;
        }
        public override bool S6F11SendClearBoxMoveReq(string boxID, string portID, List<AMCSREPORTQUEUE> reportQueues = null)
        {
            try
            {
                VIDCollection Vids = new VIDCollection();
                Vids.VIDITEM_115_DVVAL_PortID.PORT_ID = portID;
                Vids.VIDITEM_179_DVVAL_BOXID.BOX_ID = boxID;

                AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_QueryLotID, Vids);
                if (reportQueues == null)
                {
                    S6F11SendMessage(mcs_queue);
                }
                else
                {
                    reportQueues.Add(mcs_queue);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                   Data: ex);

                return false;
            }
            return true;
        }
        public override AMCSREPORTQUEUE S6F11BulibMessage(string ceid, object vidCollection)
        {
            try
            {
                VIDCollection Vids = vidCollection as VIDCollection;
                string ceidOfname = string.Empty;
                SECSConst.CEID_Dictionary.TryGetValue(ceid, out ceidOfname);
                string ceid_name = $"CEID:[{ceidOfname}({ceid})]";
                S6F11 s6f11 = new S6F11()
                {
                    SECSAgentName = scApp.EAPSecsAgentName,
                    DATAID = "0",
                    CEID = ceid,
                    StreamFunctionName = ceid_name
                };
                //List<string> RPTIDs = SECSConst.DicCEIDAndRPTID[ceid];
                List<string> RPTIDs = SECSConst.getDicCEIDAndRPTID(ceid);
                s6f11.INFO.ITEM = new S6F11.RPTINFO.RPTITEM[RPTIDs.Count];

                for (int i = 0; i < RPTIDs.Count; i++)
                {
                    string rpt_id = RPTIDs[i];
                    s6f11.INFO.ITEM[i] = new S6F11.RPTINFO.RPTITEM();
                    List<ARPTID> AVIDs = SECSConst.getDicRPTIDAndVID(rpt_id);
                    List<string> VIDs = AVIDs.OrderBy(avid => avid.ORDER_NUM).Select(avid => avid.VID.Trim()).ToList();
                    s6f11.INFO.ITEM[i].RPTID = rpt_id;
                    s6f11.INFO.ITEM[i].VIDITEM = new SXFY[AVIDs.Count];
                    for (int j = 0; j < AVIDs.Count; j++)
                    {
                        string vid = VIDs[j];
                        SXFY vid_item = null;
                        switch (vid)
                        {
                            case SECSConst.VID_Control_State:
                                vid_item = Vids.VIDITEM_06_SV_ControlState;
                                break;
                            case SECSConst.VID_Enhanced_Carrier_Info:
                                vid_item = Vids.VIDITEM_10_SV_EnhancedCarrierInfo;
                                break;
                            case SECSConst.VID_Command_Info:
                                vid_item = Vids.VIDITEM_11_SV_CommandInfo;
                                break;
                            case SECSConst.VID_Install_Time:
                                vid_item = Vids.VIDITEM_12_DVVAL_InstallTime;
                                break;
                            case SECSConst.VID_Carrier_ID:
                                vid_item = Vids.VIDITEM_54_DVVAL_CarrierID;
                                break;
                            case SECSConst.VID_Carrier_Info:
                                vid_item = Vids.VIDITEM_55_DVVAL_CarrierInfo;
                                break;
                            case SECSConst.VID_Carrier_Loc:
                                vid_item = Vids.VIDITEM_56_DVVAL_CarrierLoc;
                                break;
                            case SECSConst.VID_Command_ID:
                                vid_item = Vids.VIDITEM_58_DVVAL_CommandID;
                                break;
                            case SECSConst.VID_Dest_Port:
                                vid_item = Vids.VIDITEM_60_DVVAL_DestPort;
                                break;
                            case SECSConst.VID_Eqp_Name:
                                vid_item = Vids.VIDITEM_61_ECV_EqpName;
                                break;
                            case SECSConst.VID_Priority:
                                vid_item = Vids.VIDITEM_62_DVVAL_Priority;
                                break;
                            case SECSConst.VID_Replace:
                                vid_item = Vids.VIDITEM_63_DVVAL_ErrorId;
                                break;
                            case SECSConst.VID_Result_Code:
                                vid_item = Vids.VIDITEM_64_DVVAL_ResultCode;
                                break;
                            case SECSConst.VID_Source_ID:
                                vid_item = Vids.VIDITEM_65_DVVAL_SourceID;
                                break;
                            case SECSConst.VID_Handoff_Type:
                                vid_item = Vids.VIDITEM_66_DVVAL_HandoffType;
                                break;
                            case SECSConst.VID_IDread_Status:
                                vid_item = Vids.VIDITEM_67_DVVAL_IDreadStatus;
                                break;
                            case SECSConst.VID_Recoery_Option:
                                vid_item = Vids.VIDITEM_68_DVVAL_RecoeryOption;
                                break;
                            case SECSConst.VID_Crane_ID:
                                vid_item = Vids.VIDITEM_70_DVVAL_CraneID;
                                break;
                            case SECSConst.VID_Unit_Info:
                                vid_item = Vids.VIDITEM_72_SV_UnitInfo;
                                break;
                            case SECSConst.VID_SC_State:
                                vid_item = Vids.VIDITEM_73_DVVAL_SCState;
                                break;
                            case SECSConst.VID_Command_Type:
                                vid_item = Vids.VIDITEM_80_DVVAL_CommandType;
                                break;
                            case SECSConst.VID_Alarm_ID:
                                vid_item = Vids.VIDITEM_81_DVVAL_AlarmID;
                                break;
                            case SECSConst.VID_Alarm_Text:
                                vid_item = Vids.VIDITEM_82_DVVAL_AlarmText;
                                break;
                            case SECSConst.VID_Unit_ID:
                                vid_item = Vids.VIDITEM_83_DVVAL_UnitID;
                                break;
                            case SECSConst.VID_Spec_Version:
                                vid_item = Vids.VIDITEM_114_DVVAL_SpecVersion;
                                break;
                            case SECSConst.VID_Port_ID:
                                vid_item = Vids.VIDITEM_115_DVVAL_PortID;
                                break;
                            case SECSConst.VID_Port_Type:
                                vid_item = Vids.VIDITEM_116_DVVAL_PortType;
                                break;
                            case SECSConst.VID_Zone_Data:
                                vid_item = Vids.VIDITEM_172_SV_ZoneData;
                                break;
                            case SECSConst.VID_BOX_ID:
                                vid_item = Vids.VIDITEM_179_DVVAL_BOXID;
                                break;
                            case SECSConst.VID_Carrier_Zone_Name:
                                vid_item = Vids.VIDITEM_370_DVVAL_CarrierZoneName;
                                break;
                            case SECSConst.VID_Transfer_Info:
                                vid_item = Vids.VIDITEM_720_SV_TransferInfo;
                                break;
                            case SECSConst.VID_Request_Count:
                                vid_item = Vids.VIDITEM_890_DVVAL_RequestCount;
                                break;
                            case SECSConst.VID_Crane_Current_Position:
                                vid_item = Vids.VIDITEM_891_DVVAL_CraneCurrentPosition;
                                break;
                            case SECSConst.VID_Crane_Total_Distance:
                                vid_item = Vids.VIDITEM_892_DVVAL_CraneTotalDistance;
                                break;
                            case SECSConst.VID_Monitored_CraneInfo:
                                vid_item = Vids.VIDITEM_893_SV_MonitoredCraneInfo;
                                break;
                            default:
                                break;
                        }
                        s6f11.INFO.ITEM[i].VIDITEM[j] = vid_item;
                    }
                }

                return BuildMCSReport
                (s6f11,
                  Vids.VIDITEM_58_DVVAL_CommandID.COMMAND_ID
                , Vids.VH_ID
                , Vids.VIDITEM_115_DVVAL_PortID.PORT_ID);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
                return null;
            }
        }
        private AMCSREPORTQUEUE BuildMCSReport(S6F11 sxfy, string cmd_id, string vh_id, string port_id)
        {
            byte[] byteArray = SCUtility.ToByteArray(sxfy);
            DateTime reportTime = DateTime.Now;
            AMCSREPORTQUEUE queue = new AMCSREPORTQUEUE()
            {
                SERIALIZED_SXFY = byteArray,
                INTER_TIME = reportTime,
                REPORT_TIME = reportTime,
                STREAMFUNCTION_NAME = string.Concat(sxfy.StreamFunction, '-', sxfy.StreamFunctionName),
                STREAMFUNCTION_CEID = sxfy.CEID,
                MCS_CMD_ID = cmd_id,
                VEHICLE_ID = vh_id,
                PORT_ID = port_id
            };
            return queue;
        }
        protected override Boolean isSend(SXFY sxfy)
        {
            Boolean result = false;
            try
            {
                if (sxfy is S6F11)
                {
                    S6F11 s6f11 = (sxfy as S6F11);
                    if (s6f11.CEID == SECSConst.CEID_Equipment_OFF_LINE ||
                        s6f11.CEID == SECSConst.CEID_Control_Status_Local ||
                        s6f11.CEID == SECSConst.CEID_Control_Status_Remote)
                    {
                        return true;
                    }
                }
                result = scApp.getEQObjCacheManager().getLine().Host_Control_State == SCAppConstants.LineHostControlState.HostControlState.On_Line_Local ||
                    scApp.getEQObjCacheManager().getLine().Host_Control_State == SCAppConstants.LineHostControlState.HostControlState.On_Line_Remote;
                if (bcf.Common.BCFUtility.isMatche(sxfy.StreamFunction, "S6F11"))
                {
                    S6F11 s6f11 = null;
                    string ceid = (string)sxfy.getField(bcf.Common.BCFUtility.getPropertyName(() => s6f11.CEID));
                    if (!eventBLL.isEnableReport(ceid))
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}",
                    line.LINE_ID, "isSendEAP", ex.ToString());
            }
            return result;
        }
        public override bool S6F11SendMessage(AMCSREPORTQUEUE queue)
        {
            try
            {
                if (!isHostReady()) return false;

                LogHelper.setCallContextKey_ServiceID(CALL_CONTEXT_KEY_WORD_SERVICE_ID_MCS);

                S6F11 s6f11 = (S6F11)SCUtility.ToObject(queue.SERIALIZED_SXFY);

                S6F12 s6f12 = null;
                SXFY abortSecs = null;
                String rtnMsg = string.Empty;

                //if (!isSend(s6f11)) return true;

                SCUtility.RecodeReportInfo(queue.VEHICLE_ID, queue.MCS_CMD_ID, s6f11, s6f11.CEID);
                SCUtility.secsActionRecordMsg(scApp, false, s6f11);
                #region LogSave
                log = "";
                //string[] dataVal;
                foreach (var v in s6f11.INFO.ITEM[0].VIDITEM)
                {
                    if (v != null)
                    {
                        var g = v.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                        string name = g[0].Name;
                        //var gg = v.GetType().GetCustomAttribute(typeof(SecsElement));
                        var value = v.GetType().GetField(g[0].Name).GetValue(v) ?? "null";
                        log = log + name + ":" + value.ToString().Trim() + "    ";
                    }
                }
                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> MCS|S6F11 CEID:" + s6f11.CEID + "  " + log);
                #endregion
                
                TrxSECS.ReturnCode rtnCode = ISECSControl.sendRecv<S6F12>(bcfApp, s6f11, out s6f12,
                    out abortSecs, out rtnMsg, null);

                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|S6F12 ACK6:" + s6f12.ACKC6);

                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                //   Data: s6f11,
                //   VehicleID: queue.VEHICLE_ID,
                //   XID: queue.MCS_CMD_ID);
                SCUtility.secsActionRecordMsg(scApp, false, s6f12);
                SCUtility.actionRecordMsg(scApp, s6f11.StreamFunction, line.Real_ID,
                            "sendS6F11_common.", rtnCode.ToString());
                SCUtility.RecodeReportInfo(queue.VEHICLE_ID, queue.MCS_CMD_ID, s6f12, s6f11.CEID, rtnCode.ToString());
                //LogHelper.Log(logger: logger, LogLevel: LogLevel.Info, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
                //   Data: s6f12,
                //   VehicleID: queue.VEHICLE_ID,
                //   XID: queue.MCS_CMD_ID);

                if (rtnCode != TrxSECS.ReturnCode.Normal)
                {
                    logger_MapActionLog.Warn("Send Transfer Initiated[S6F11] Error![rtnCode={0}]", rtnCode);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                TransferServiceLogger.Error(DateTime.Now.ToString("HH:mm:ss.fff ") + "OHB >> MCS|S6F11\n" + log + "\n" + ex);
                logger.Error(ex, "Exception:");
                return false;
            }
        }

        #endregion Send
        #endregion

        #region VID Info
        private VIDCollection AVIDINFO2VIDCollection(AVIDINFO vid_info)
        {
            if (vid_info == null)
                return null;
            //string carrier_loc = string.Empty;
            //string port_id = string.Empty;
            //scApp.MapBLL.getPortID(vid_info.CARRIER_LOC, out carrier_loc);
            //scApp.MapBLL.getPortID(vid_info.PORT_ID, out port_id);
            var line = scApp.getEQObjCacheManager().getLine();
            VIDCollection vid_collection = new VIDCollection();
            vid_collection.VH_ID = vid_info.EQ_ID;


            AVEHICLE vh = scApp.VehicleBLL.getVehicleByID(vid_info.EQ_ID);
            //VID_01_AlarmID
            //vid_collection.VIDITEM_01_DVVAL_AlarmID.ALARM_ID = vid_info.ALARM_ID;

            //VID_06_Control
            //string control_state = SCAppConstants.LineHostControlState.convert2MES(line.Host_Control_State);
            //vid_collection.VIDITEM_06_SV_ControlState.CONTROLSTATE = control_state;

            ////VID_54_CarrierID
            //vid_collection.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = vid_info.CARRIER_ID;

            //VID_55_CarrierInfo
            vid_collection.VIDITEM_55_DVVAL_CarrierInfo.CARRIER_ID = vid_info.CARRIER_ID;
            //vid_collection.VIDITEM_55_DVVAL_CarrierInfo.VEHICLE_ID = vh.Real_ID;
            vid_collection.VIDITEM_55_DVVAL_CarrierInfo.CARRIER_LOC = vid_info.CARRIER_LOC;

            ////VID_54_CarrierID
            //vid_collection.VIDITEM_56_DVVAL_CarrierLoc.CARRIER_LOC = vid_info.CARRIER_LOC;

            ////VID_57_Command Name
            ////vid_collection.VIDITEM_57_DVVAL_CommandName.COMMAND_NAME = vid_info.COMMAND_ID; //todo 需確認Command Name要填入什麼?

            ////VID_58_CommandID
            //vid_collection.VIDITEM_58_DVVAL_CommandID.COMMAND_ID = vid_info.COMMAND_ID;

            ////VID_59_CommandInfo
            //vid_collection.VIDITEM_59_DVVAL_CommandInfo.COMMAND_ID = vid_info.COMMAND_ID;
            //vid_collection.VIDITEM_59_DVVAL_CommandInfo.PRIORITY = vid_info.PRIORITY.ToString();
            //vid_collection.VIDITEM_59_DVVAL_CommandInfo.REPLACE = vid_info.REPLACE.ToString();//不知道Replace要填什麼 , For Kevin Wei to Confirm

            ////VIDITEM_60_DVVAL_DestPort
            //vid_collection.VIDITEM_60_DVVAL_DestPort.DESTINATION_PORT = vid_info.DESTPORT;

            ////VIDITEM_61_ECV_EqpName
            //vid_collection.VIDITEM_61_ECV_EqpName.EQPT_NAME = line.LINE_ID;

            ////VIDITEM_62_DVVAL_Priority
            //vid_collection.VIDITEM_62_DVVAL_Priority.PRIORITY = vid_info.PRIORITY.ToString();
            ////VIDITEM_63_DVVAL_Replace
            //vid_collection.VIDITEM_63_DVVAL_Replace.REPLACE = vid_info.REPLACE.ToString();

            ////VIDITEM_64_DVVAL_ResultCode
            //vid_collection.VIDITEM_64_DVVAL_ResultCode.RESULT_CODE = vid_info.RESULT_CODE.ToString();

            ////VIDITEM_65_DVVAL_SourcePort
            //vid_collection.VIDITEM_65_DVVAL_SourcePort.SOURCE_PORT = vid_info.SOURCEPORT;

            ////VIDITEM_66_DVVAL_TransferCommand
            //vid_collection.VIDITEM_66_DVVAL_TransferCommand.COMMAND_INFO.COMMAND_ID = vid_info.COMMAND_ID;
            //vid_collection.VIDITEM_66_DVVAL_TransferCommand.COMMAND_INFO.PRIORITY = vid_info.PRIORITY.ToString();
            //vid_collection.VIDITEM_66_DVVAL_TransferCommand.COMMAND_INFO.REPLACE = string.Empty;//不知道Replace要填什麼 , For Kevin Wei to Confirm
            //vid_collection.VIDITEM_66_DVVAL_TransferCommand.TRANSFER_INFOS = new S6F11.RPTINFO.RPTITEM.VIDITEM_67_DVVAL[1];
            //vid_collection.VIDITEM_66_DVVAL_TransferCommand.TRANSFER_INFOS[0] = new S6F11.RPTINFO.RPTITEM.VIDITEM_67_DVVAL();
            //vid_collection.VIDITEM_66_DVVAL_TransferCommand.TRANSFER_INFOS[0].CARRIER_ID = vid_info.CARRIER_ID;
            //vid_collection.VIDITEM_66_DVVAL_TransferCommand.TRANSFER_INFOS[0].SOURCE_PORT = vid_info.SOURCEPORT;
            //vid_collection.VIDITEM_66_DVVAL_TransferCommand.TRANSFER_INFOS[0].DESTINATION_PORT = vid_info.DESTPORT;

            ////VIDITEM_67_DVVAL_TransferInfo
            //vid_collection.VIDITEM_67_DVVAL_TransferInfo.CARRIER_ID = vid_info.CARRIER_ID;
            //vid_collection.VIDITEM_67_DVVAL_TransferInfo.SOURCE_PORT = vid_info.SOURCEPORT;
            //vid_collection.VIDITEM_67_DVVAL_TransferInfo.DESTINATION_PORT = vid_info.DESTPORT;

            ////VIDITEM_68_DVVAL_TransferPort
            //vid_collection.VIDITEM_68_DVVAL_TransferPort.TRANSFER_PORT = vid_info.PORT_ID;

            ////VIDITEM_69_DVVAL_TransferPort
            //vid_collection.VIDITEM_69_DVVAL_TransferPortList.TRANSFER_PORT_OBJ = new S6F11.RPTINFO.RPTITEM.VIDITEM_68_DVVAL[1]; //todo 要確認該欄位要填入什麼
            //vid_collection.VIDITEM_69_DVVAL_TransferPortList.TRANSFER_PORT_OBJ[0] = new S6F11.RPTINFO.RPTITEM.VIDITEM_68_DVVAL();
            //vid_collection.VIDITEM_69_DVVAL_TransferPortList.TRANSFER_PORT_OBJ[0].TRANSFER_PORT = vid_info.PORT_ID;
            ////VIDITEM_70_DVVAL_VehicleID
            //vid_collection.VIDITEM_70_DVVAL_VehicleID.VEHILCE_ID = vh.Real_ID;

            ////VIDITEM_71_DVVAL_VehicleInfo
            //vid_collection.VIDITEM_71_DVVAL_VehicleInfo.VEHICLE_ID = vh.Real_ID;
            //vid_collection.VIDITEM_71_DVVAL_VehicleInfo.VEHICLE_STATE = ((int)vh.State).ToString();

            ////VIDITEM_72_DVVAL_VehicleState
            //vid_collection.VIDITEM_72_DVVAL_VehicleState.VEHICLE_STATE = ((int)vh.State).ToString();

            ////VIDITEM_73_SV_TSCState
            //string tsc_state = ((int)line.TSC_state_machine.State).ToString();
            //vid_collection.VIDITEM_73_SV_TSCState.TSCState = tsc_state;

            ////VIDITEM_74_DVVAL_CommandType
            //vid_collection.VIDITEM_74_DVVAL_CommandType.COMMAND_TYPE = vid_info.COMMAND_TYPE;

            ////VIDITEM_74_DVVAL_CommandType
            //vid_collection.VIDITEM_75_DVVAL_EnhancedCarriInfo.CARRIER_ID = vid_info.CARRIER_ID;
            //vid_collection.VIDITEM_75_DVVAL_EnhancedCarriInfo.VRHICLE_ID = vh.Real_ID;
            //vid_collection.VIDITEM_75_DVVAL_EnhancedCarriInfo.CARRIER_LOC = vid_info.CARRIER_LOC;
            //vid_collection.VIDITEM_75_DVVAL_EnhancedCarriInfo.INSTALL_TIME = vid_info.CARRIER_INSTALLED_TIME?.ToString(SCAppConstants.TimestampFormat_16);

            ////VIDITEM_74_DVVAL_CommandType
            //vid_collection.VIDITEM_78_SV_SourcePort.SOURCE_PORT = vid_info.SOURCEPORT;

            ////VIDITEM_74_DVVAL_CommandType
            //vid_collection.VIDITEM_79_DVVAL_TransferCompleteInfo.TRANCOMPLETEINFO[0].TRANSFER_INFO_OBJ.CARRIER_ID = vid_info.CARRIER_ID;
            //vid_collection.VIDITEM_79_DVVAL_TransferCompleteInfo.TRANCOMPLETEINFO[0].TRANSFER_INFO_OBJ.SOURCE_PORT = vid_info.SOURCEPORT;
            //vid_collection.VIDITEM_79_DVVAL_TransferCompleteInfo.TRANCOMPLETEINFO[0].TRANSFER_INFO_OBJ.DESTINATION_PORT = vid_info.DESTPORT;
            //vid_collection.VIDITEM_79_DVVAL_TransferCompleteInfo.TRANCOMPLETEINFO[0].CARRIER_LOC = vid_info.CARRIER_LOC;

            ////VIDITEM_114_SV_SpecVersion
            //vid_collection.VIDITEM_114_SV_SpecVersion.SPEC_VERSION = "";//todo 要確認要填入什麼值

            ////VIDITEM_115_DVVAL_PortID
            //vid_collection.VIDITEM_115_DVVAL_PortID.PORT_ID = vid_info.PORT_ID;

            ////VIDITEM_117_DVVAL_VehicleLocation
            //vid_collection.VIDITEM_117_DVVAL_VehicleLocation.VEHICLE_LOCATION = vid_info.PORT_ID;

            ////VIDITEM_120_DVVAL_UnitStatusCleable
            //vid_collection.VIDITEM_120_DVVAL_UnitStatusCleable.UNIT_STATUS_CLEABLE = "Y";

            ////VIDITEM_202_DVVAL_TransferState
            ////vid_collection.VIDITEM_202_DVVAL_TransferState.TRANSFER_STATE =vid_info.

            ////VIDITEM_204_DVVAL_InstallTime
            //vid_collection.VIDITEM_204_DVVAL_InstallTime.INSTALL_TIME = vid_info.CARRIER_INSTALLED_TIME?
            //                                                            .ToString(SCAppConstants.TimestampFormat_16);//要填入INSTALL_TIME

            ////VIDITEM_251_DVVAL_VehicleCurrentPosition
            //vid_collection.VIDITEM_251_DVVAL_VehicleCurrentPosition.VEHICLE_CURRENT_POSITION = SCUtility.isEmpty(vh.CUR_ADR_ID) ? "0" : vh.CUR_ADR_ID;

            ////VIDITEM_262_DVVAL_VehicleNextPosition
            //vid_collection.VIDITEM_262_DVVAL_VehicleNextPosition.VEHICLE_NEXT_POSITION = "";//todo 要確認要填入的資料



            return vid_collection;
        }
        #endregion VID Info

        protected override void S2F37ReceiveEnableDisableEventReport(object sender, SECSEventArgs e)
        {
            try
            {
                S2F37 s2f37 = ((S2F37)e.secsHandler.Parse<S2F37>(e));
                SCUtility.secsActionRecordMsg(scApp, true, s2f37);
                //if (!isProcess(s2f37)) { return; }
                Boolean isValid = true;
                //Boolean isEnable = SCUtility.isMatche(s2f37.CEED, SECSConst.CEED_Enable);
                Boolean isEnable = s2f37.CEED[0] == 255;
                //Boolean isEnable = s2f37.CEED == true;

                scApp.TransferService.TransferServiceLogger.Info(
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|s2f37:\n" + s2f37.toSECSString());

                int cnt = s2f37.CEIDS.Length;
                if (cnt == 0)
                {
                    isValid &= scApp.EventBLL.enableAllEventReport(isEnable);
                }
                else
                {
                    //Check Data
                    for (int ix = 0; ix < cnt; ++ix)
                    {
                        string ceid = s2f37.CEIDS[ix].PadLeft(3, '0');
                        Boolean isContain = SECSConst.CEID_ARRAY.Contains(ceid.Trim());
                        if (!isContain)
                        {
                            isValid = false;
                            break;
                        }
                    }
                    if (isValid)
                    {
                        for (int ix = 0; ix < cnt; ++ix)
                        {
                            string ceid = s2f37.CEIDS[ix].PadLeft(3, '0');
                            isValid &= scApp.EventBLL.enableEventReport(ceid, isEnable);
                        }
                    }
                }

                S2F38 s2f38 = null;
                s2f38 = new S2F38()
                {
                    SystemByte = s2f37.SystemByte,
                    SECSAgentName = scApp.EAPSecsAgentName,
                    ERACK = isValid ? SECSConst.ERACK_Accepted : SECSConst.ERACK_Denied_At_least_one_CEID_dose_not_exist
                };

                TrxSECS.ReturnCode rtnCode = ISECSControl.replySECS(bcfApp, s2f38);

                scApp.TransferService.TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "MCS >> OHB|s2f38 ERACK:" + s2f38.ERACK + "   SCES_ReturnCode:" + rtnCode);

                SCUtility.secsActionRecordMsg(scApp, false, s2f38);

                if (rtnCode != TrxSECS.ReturnCode.Normal)
                {
                    logger.Warn("Reply EQPT S2F18 Error:{0}", rtnCode);
                }
            }
            catch (Exception ex)
            {
                scApp.TransferService.TransferServiceLogger.Error(
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "  S2F37ReceiveEnableDisableEventReport\n" + ex);

                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}", line.LINE_ID, "S2F17_Receive_Date_Time_Req", ex.ToString());
            }
        }

        protected override void S2F31ReceiveDateTimeSetReq(object sender, SECSEventArgs e)
        {
            try
            {
                S2F31 s2f31 = ((S2F31)e.secsHandler.Parse<S2F31>(e));

                SCUtility.secsActionRecordMsg(scApp, true, s2f31);
                SCUtility.actionRecordMsg(scApp, s2f31.StreamFunction, line.Real_ID,
                        "Receive Date Time Set Request From MES.", "");
                //if (isProcess(s2f31)) { return; }

                TransferServiceLogger.Info(
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|s2f31:\n" + s2f31.toSECSString());

                S2F32 s2f32 = new S2F32();
                s2f32.SECSAgentName = scApp.EAPSecsAgentName;
                s2f32.SystemByte = s2f31.SystemByte;
                s2f32.TIACK = SECSConst.TIACK_Accepted;

                string timeStr = s2f31.TIME;

                DateTime mesDateTime = DateTime.Now;
                try
                {
                    mesDateTime = DateTime.ParseExact(timeStr.Trim(), SCAppConstants.TimestampFormat_16, CultureInfo.CurrentCulture);
                }
                catch (Exception ex)
                {
                    s2f32.TIACK = SECSConst.TIACK_Error_not_done;

                    TransferServiceLogger.Error(
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "  S2F31ReceiveDateTimeSetReq\n" + ex);
                }

                SCUtility.secsActionRecordMsg(scApp, false, s2f32);
                TrxSECS.ReturnCode rtnCode = ISECSControl.replySECS(bcfApp, s2f32);

                TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "MCS >> OHB|s2f32 TIACK:" + s2f32.TIACK + "   SCES_ReturnCode:" + rtnCode);

                if (!DebugParameter.DisableSyncTime)
                {
                    SCUtility.updateSystemTime(mesDateTime);
                }

                //與設備同步
                PLCSystemInfoMapAction systemTimeMapAction = scApp.getEQObjCacheManager().getPortByPortID("MASTER_PLC")
                    .getMapActionByIdentityKey(typeof(PLCSystemInfoMapAction).Name) as PLCSystemInfoMapAction;
                systemTimeMapAction.PLC_SetSystemTime();
                systemTimeMapAction.PLC_FinishTimeCalibration();
            }
            catch (Exception ex)
            {
                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}",
                    line.LINE_ID, "S2F31_Receive_Date_Time_Set_Req", ex.ToString());
            }
        }



        //public override bool S6F11SendCarrierInstalled(string vhID, string carrierID, string transferPort, List<AMCSREPORTQUEUE> reportQueues = null)
        //{

        //    try
        //    {

        //        VIDCollection Vids = new VIDCollection();
        //        Vids.VIDITEM_61_ECV_EqpName.EQPT_NAME = line.LINE_ID;

        //        //VID_54_CarrierID
        //        Vids.VIDITEM_54_DVVAL_CarrierID.CARRIER_ID = carrierID;

        //        //Vids.VIDITEM_68_DVVAL_TransferPort.TRANSFER_PORT = "";
        //        Vids.VIDITEM_68_DVVAL_TransferPort.TRANSFER_PORT = transferPort;
        //        Vids.VIDITEM_56_DVVAL_CarrierLoc.CARRIER_LOC = vhID;

        //        AMCSREPORTQUEUE mcs_queue = S6F11BulibMessage(SECSConst.CEID_Carrier_Installed, Vids);
        //        if (reportQueues == null)
        //        {
        //            S6F11SendMessage(mcs_queue);
        //        }
        //        else
        //        {
        //            reportQueues.Add(mcs_queue);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(ASEMCSDefaultMapAction), Device: DEVICE_NAME_MCS,
        //           Data: ex);
        //    }
        //    return true;
        //}

    }
}