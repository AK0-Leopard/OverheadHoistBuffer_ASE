using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.bcf.Data.TimerAction;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.BLL;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.SECS;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.stc.Common;
using com.mirle.ibg3k0.stc.Data.SecsData;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.SECSDriver
{
    public abstract class GEMDriver
    {
        const string DEVICE_NAME_MCS = "MCS";
        protected SCApplication scApp = null;
        protected BCFApplication bcfApp = null;
        protected ALINE line = null;
        protected EventBLL eventBLL = null;
        private Logger logger = LogManager.GetCurrentClassLogger();
        public GEMDriver()
        {
            scApp = SCApplication.getInstance();
            eventBLL = scApp.EventBLL;
            bcfApp = scApp.getBCFApplication();
        }
        #region Receive
        protected virtual void S1F1ReceiveAreYouThere(object sender, SECSEventArgs e)
        {
            try
            {
                S1F1 s1f1 = ((S1F1)e.secsHandler.Parse<S1F1>(e));
                SCUtility.secsActionRecordMsg(scApp, true, s1f1);
                SCUtility.actionRecordMsg(scApp, s1f1.StreamFunction, line.Real_ID,
                        "Receive Are You There From MES.", "");

                scApp.TransferService.TransferServiceLogger.Info(
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|s1f1:\n" + s1f1.toSECSString());

                //if (!isProcess(s1f1)) { return; }
                S1F2 s1f2 = new S1F2()
                {
                    SECSAgentName = scApp.EAPSecsAgentName,
                    SystemByte = s1f1.SystemByte,
                    MDLN = bcfApp.BC_ID,
                    SOFTREV = SCApplication.getMessageString("SYSTEM_VERSION")
                };
                SCUtility.secsActionRecordMsg(scApp, false, s1f2);
                TrxSECS.ReturnCode rtnCode = ISECSControl.replySECS(bcfApp, s1f2);
                SCUtility.actionRecordMsg(scApp, s1f1.StreamFunction, line.Real_ID,
                        "Reply Are You There To MES.", rtnCode.ToString());

                scApp.TransferService.TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "MCS >> OHB|s1f2 MDLN:" + s1f2.MDLN + "   SCES_ReturnCode:" + rtnCode);

                if (rtnCode != TrxSECS.ReturnCode.Normal)
                {
                    logger.Warn("Reply EAP S1F2 Error:{0}", rtnCode);
                }
            }
            catch (Exception ex)
            {
                scApp.TransferService.TransferServiceLogger.Error(
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "  S1F1ReceiveAreYouThere\n" + ex);

                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}",
                    line.LINE_ID, "S1F1_Receive_AreYouThere", ex.ToString());
            }
        }
        protected virtual void S1F3ReceiveSelectedEquipmentStatusRequest(object sender, SECSEventArgs e)
        {
            try
            {
                S1F3 s1f3 = ((S1F3)e.secsHandler.Parse<S1F3>(e));
                SCUtility.secsActionRecordMsg(scApp, true, s1f3);
                int count = s1f3.SVID.Count();
                S1F4 s1f4 = new S1F4();
                s1f4.SECSAgentName = scApp.EAPSecsAgentName;
                s1f4.SystemByte = s1f3.SystemByte;
                s1f4.SV = new SXFY[count];
                for (int i = 0; i < count; i++)
                {
                    if (s1f3.SVID[i] == SECSConst.VID_AlarmsSet)
                    {
                        //TODO Set Alarm List
                        s1f4.SV[i] = new S6F11.RPTINFO.RPTITEM.VIDITEM_04();


                    }
                    else if (s1f3.SVID[i] == SECSConst.VID_ControlState)
                    {
                        string control_state = SCAppConstants.LineHostControlState.convert2MES(line.Host_Control_State);
                        s1f4.SV[i] = new S6F11.RPTINFO.RPTITEM.VIDITEM_06()
                        {
                            CONTROLSTATE = control_state
                        };
                    }
                    else if (s1f3.SVID[i] == SECSConst.VID_ActiveVehicles)
                    {
                        List<AVEHICLE> vhs = scApp.getEQObjCacheManager().getAllVehicle();
                        int vhs_count = vhs.Count;
                        S6F11.RPTINFO.RPTITEM.VIDITEM_71[] VEHICLEINFOs = new S6F11.RPTINFO.RPTITEM.VIDITEM_71[vhs_count];
                        for (int j = 0; j < vhs_count; j++)
                        {
                            VEHICLEINFOs[j] = new S6F11.RPTINFO.RPTITEM.VIDITEM_71()
                            {
                                VHINFO = new S6F11.RPTINFO.RPTITEM.VIDITEM_71.VEHICLEINFO()
                                {
                                    VEHICLE_ID = vhs[j].VEHICLE_ID,
                                    VEHICLE_STATE = "2"
                                }
                            };
                        }
                        s1f4.SV[i] = new S6F11.RPTINFO.RPTITEM.VIDITEM_53()
                        {
                            VEHICLEINFO = VEHICLEINFOs
                        };
                    }
                    else if (s1f3.SVID[i] == SECSConst.VID_SCState)
                    {
                        //string sc_state = SCAppConstants.LineSCState.convert2MES(line.SCStats);
                        //s1f4.SV[i] = new S6F11.RPTINFO.RPTITEM.VIDITEM_73()
                        //{
                        //    SCSTATE = sc_state
                        //};
                    }
                    else if (s1f3.SVID[i] == SECSConst.VID_EnhancedTransfers)
                    {
                        List<ACMD_MCS> mcs_cmds = scApp.CMDBLL.loadACMD_MCSIsUnfinished();
                        int cmd_count = mcs_cmds.Count;
                        S6F11.RPTINFO.RPTITEM.VIDITEM_13[] EnhancedTransferCmds = new S6F11.RPTINFO.RPTITEM.VIDITEM_13[cmd_count];
                        for (int k = 0; k < cmd_count; k++)
                        {
                            ACMD_MCS mcs_cmd = mcs_cmds[k];
                            string transfer_state = SCAppConstants.TransferState.convert2MES(mcs_cmd.TRANSFERSTATE);
                            EnhancedTransferCmds[k] = new S6F11.RPTINFO.RPTITEM.VIDITEM_13();
                            EnhancedTransferCmds[k].TRANSFER_STATE.TRANSFER_STATE = transfer_state;

                            EnhancedTransferCmds[k].COMMAND_INFO.COMMAND_ID.COMMAND_ID = mcs_cmd.CMD_ID;
                            EnhancedTransferCmds[k].COMMAND_INFO.PRIORITY.PRIORITY = mcs_cmd.PRIORITY.ToString();

                            EnhancedTransferCmds[k].TRANSFER_INFO.CARRIER_ID = mcs_cmd.CARRIER_ID;
                            EnhancedTransferCmds[k].TRANSFER_INFO.SOURCE_PORT = mcs_cmd.HOSTSOURCE;
                            EnhancedTransferCmds[k].TRANSFER_INFO.DESTINATION_PORT = mcs_cmd.HOSTDESTINATION;
                        }

                        s1f4.SV[i] = new S6F11.RPTINFO.RPTITEM.VIDITEM_76()
                        {
                            EnhancedTransferCmd = EnhancedTransferCmds
                        };
                    }
                    else if (s1f3.SVID[i] == SECSConst.VID_Spec_Version)
                    {
                        s1f4.SV[i] = new S6F11.RPTINFO.RPTITEM.VIDITEM_114()
                        {
                            SPEC_VERSION = string.Empty // TODO fill in
                        };
                    }
                    else if (s1f3.SVID[i] == SECSConst.VID_Enhanced_Carriers)
                    {
                        List<AVEHICLE> has_carry_vhs = scApp.getEQObjCacheManager().getAllVehicle().Where(vh => vh.HAS_CST == 1).ToList();
                        int carry_vhs_count = has_carry_vhs.Count;
                        S6F11.RPTINFO.RPTITEM.VIDITEM_10[] carrier_info = new S6F11.RPTINFO.RPTITEM.VIDITEM_10[carry_vhs_count];
                        for (int j = 0; j < carry_vhs_count; j++)
                        {
                            carrier_info[j] = new S6F11.RPTINFO.RPTITEM.VIDITEM_10();
                            carrier_info[j].CARRIER_ID_OBJ.CARRIER_ID = has_carry_vhs[j].CST_ID.Trim();
                            carrier_info[j].CARRIER_LOC_OBJ.CARRIER_LOC = has_carry_vhs[j].VEHICLE_ID;
                            carrier_info[j].INSTALL_TIME_OBJ.INSTALLTIME = string.Empty; //TODO 要填入進入的時間
                            carrier_info[j].CARRIER_ZONE_NAME = string.Empty;
                            carrier_info[j].CARRIER_STATE = string.Empty;
                        }


                        s1f4.SV[i] = new S6F11.RPTINFO.RPTITEM.VIDITEM_51()
                        {
                            ENHANCED_CARRIER_INFO = carrier_info
                        };
                    }
                    else if (s1f3.SVID[i] == SECSConst.VID_Current_Port_States)
                    {
                        List<APORTSTATION> port_station = scApp.MapBLL.loadAllPort();
                        int port_count = port_station.Count;
                        var vid_118 = new S6F11.RPTINFO.RPTITEM.VIDITEM_118();
                        vid_118.PORT_INFO = new S6F11.RPTINFO.RPTITEM.VIDITEM_354[port_count];
                        for (int j = 0; j < port_count; j++)
                        {
                            vid_118.PORT_INFO[j] = new S6F11.RPTINFO.RPTITEM.VIDITEM_354();
                            vid_118.PORT_INFO[j].PORT_ID.PORT_ID = port_station[j].PORT_ID;
                            vid_118.PORT_INFO[j].PORT_TRANSFTER_STATE.PORT_TRANSFER_STATE =
                                ((int)port_station[j].PORT_STATUS).ToString();
                        }
                        s1f4.SV[i] = vid_118;

                    }
                    else
                    {
                        s1f4.SV[i] = new S6F11.RPTINFO.RPTITEM.VIDITEM_04();
                    }
                }
                TrxSECS.ReturnCode rtnCode = ISECSControl.replySECS(bcfApp, s1f4);
                SCUtility.secsActionRecordMsg(scApp, false, s1f4);
            }
            catch (Exception ex)
            {
                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}",
                    line.LINE_ID, "S1F3_Receive_Eqpt_Stat_Req", ex.ToString());
            }
        }
        protected virtual void S1F13ReceiveEstablishCommunicationRequest(object sender, SECSEventArgs e)
        {
            try
            {
                S1F13 s1f13 = ((S1F13)e.secsHandler.Parse<S1F13>(e));
                SCUtility.secsActionRecordMsg(scApp, true, s1f13);
                SCUtility.actionRecordMsg(scApp, s1f13.StreamFunction, line.Real_ID,
                        "Receive Establish Communication From MES.", "");

                scApp.TransferService.TransferServiceLogger.Info(
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|s1f13:\n" + s1f13.toSECSString());

                //if (!isProcessEAP(s1f13)) { return; }
                S1F14 s1f14 = new S1F14();
                s1f14.SECSAgentName = scApp.EAPSecsAgentName;
                s1f14.SystemByte = s1f13.SystemByte;
                s1f14.COMMACK = "0";
                s1f14.VERSION_INFO = new string[2]
                { "OHTC",
                  SCAppConstants.getMainFormVersion("") };

                SCUtility.secsActionRecordMsg(scApp, false, s1f14);
                TrxSECS.ReturnCode rtnCode = ISECSControl.replySECS(bcfApp, s1f14);
                SCUtility.actionRecordMsg(scApp, s1f13.StreamFunction, line.Real_ID,
                        "Reply Establish Communication To MES.", rtnCode.ToString());

                scApp.TransferService.TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "MCS >> OHB|s1f14 COMMACK:" + s1f14.COMMACK + "   SCES_ReturnCode:" + rtnCode);

                if (rtnCode != TrxSECS.ReturnCode.Normal)
                {
                    logger.Warn("Reply EAP S1F14 Error:{0}", rtnCode);
                }
                logger.Debug("s1f13Receive ok!");
                line.EstablishComm = true;
            }
            catch (Exception ex)
            {
                scApp.TransferService.TransferServiceLogger.Error(
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "  S1F13ReceiveEstablishCommunicationRequest \n" + ex);

                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}",
                    line.LINE_ID, "s1f13_Receive_EstablishCommunication", ex.ToString());
            }
        }
        protected virtual void S1F15ReceiveRequestOffLine(object sender, SECSEventArgs e)
        {
            throw new NotImplementedException();
        }
        protected virtual void S1F17ReceiveRequestOnLine(object sender, SECSEventArgs e)
        {
            try
            {
                string msg = string.Empty;
                S1F17 s1f17 = ((S1F17)e.secsHandler.Parse<S1F17>(e));
                SCUtility.secsActionRecordMsg(scApp, true, s1f17);

                //if (!isProcess(s1f17)) { return; }

                S1F18 s1f18 = new S1F18();
                s1f18.SystemByte = s1f17.SystemByte;
                s1f18.SECSAgentName = scApp.EAPSecsAgentName;


                //檢查狀態是否允許連線
                if (DebugParameter.RejectEAPOnline)
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
                if (rtnCode != TrxSECS.ReturnCode.Normal)
                {
                    logger.Warn("Reply EQPT S1F18 Error:{0}", rtnCode);
                }

                if (SCUtility.isMatche(s1f18.ONLACK, SECSConst.ONLACK_Accepted))
                {
                    line.Host_Control_State = SCAppConstants.LineHostControlState.HostControlState.On_Line_Remote;
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}", line.LINE_ID, "S1F17_Receive_OnlineRequest", ex.ToString());
            }
        }
        protected virtual void S2F15ReceiveNewEquiptment(object sender, SECSEventArgs e)
        {
            try
            {
                S2F15 s2f15 = ((S2F15)e.secsHandler.Parse<S2F15>(e));
                SCUtility.secsActionRecordMsg(scApp, true, s2f15);
                SCUtility.actionRecordMsg(scApp, s2f15.StreamFunction, line.Real_ID,
                        "Receive New EQPT Constant Data From MES.", "");
                //if (!isProcess(s2f15)) { return; }

                S2F16 s2f16 = new S2F16();
                s2f16.SECSAgentName = scApp.EAPSecsAgentName;
                s2f16.SystemByte = s2f15.SystemByte;
                s2f16.EAC = "0";

                SCUtility.secsActionRecordMsg(scApp, false, s2f16);
                TrxSECS.ReturnCode rtnCode = ISECSControl.replySECS(bcfApp, s2f16);
                SCUtility.actionRecordMsg(scApp, s2f16.StreamFunction, line.Real_ID,
                        "Reply OK To MES.", rtnCode.ToString());
            }
            catch (Exception ex)
            {
                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}",
                    line.LINE_ID, "S2F15_Receive_New_EQConstants", ex.ToString());
            }
        }
        protected virtual void S2F17ReceiveDateAndTimeRequest(object sender, SECSEventArgs e)
        {
            try
            {
                S2F17 s2f17 = ((S2F17)e.secsHandler.Parse<S2F17>(e));
                SCUtility.secsActionRecordMsg(scApp, true, s2f17);
                //if (!isProcess(s2f17)) { return; }

                S2F18 s2f18 = null;
                s2f18 = new S2F18();
                s2f18.SystemByte = s2f17.SystemByte;
                s2f18.SECSAgentName = scApp.EAPSecsAgentName;
                s2f18.TIME = DateTime.Now.ToString("yyyyMMddHHmmss");

                TrxSECS.ReturnCode rtnCode = ISECSControl.replySECS(bcfApp, s2f18);
                SCUtility.secsActionRecordMsg(scApp, false, s2f18);
                if (rtnCode != TrxSECS.ReturnCode.Normal)
                {
                    logger.Warn("Reply EQPT S2F18 Error:{0}", rtnCode);
                }
            }
            catch (Exception ex)
            {
                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}", line.LINE_ID, "S2F17_Receive_Date_Time_Req", ex.ToString());
            }
        }
        protected virtual void S2F31ReceiveDateTimeSetReq(object sender, SECSEventArgs e)
        {
            try
            {
                S2F31 s2f31 = ((S2F31)e.secsHandler.Parse<S2F31>(e));

                SCUtility.secsActionRecordMsg(scApp, true, s2f31);
                SCUtility.actionRecordMsg(scApp, s2f31.StreamFunction, line.Real_ID,
                        "Receive Date Time Set Request From MES.", "");
                //if (!isProcess(s2f31)) { return; }

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
                catch (Exception dtEx)
                {
                    s2f32.TIACK = SECSConst.TIACK_Error_not_done;
                }

                SCUtility.secsActionRecordMsg(scApp, false, s2f32);
                ISECSControl.replySECS(bcfApp, s2f32);

                if (!DebugParameter.DisableSyncTime)
                {
                    SCUtility.updateSystemTime(mesDateTime);
                }

                //TODO 與設備同步
            }
            catch (Exception ex)
            {
                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}",
                    line.LINE_ID, "S2F31_Receive_Date_Time_Set_Req", ex.ToString());
            }
        }
        protected virtual void S2F33ReceiveDefineReport(object sender, SECSEventArgs e)
        {
            try
            {
                S2F33 s2f33 = ((S2F33)e.secsHandler.Parse<S2F33>(e));
                SCUtility.secsActionRecordMsg(scApp, true, s2f33);
                //if (!isProcess(s2f33)) { return; }

                S2F34 s2f34 = null;
                s2f34 = new S2F34();
                s2f34.SystemByte = s2f33.SystemByte;
                s2f34.SECSAgentName = scApp.EAPSecsAgentName;
                s2f34.DRACK = "0";


                TrxSECS.ReturnCode rtnCode = ISECSControl.replySECS(bcfApp, s2f34);
                SCUtility.secsActionRecordMsg(scApp, false, s2f34);


                if (rtnCode != TrxSECS.ReturnCode.Normal)
                {
                    logger.Warn("Reply EQPT S2F18 Error:{0}", rtnCode);
                }

                scApp.CEIDBLL.DeleteRptInfoByBatch();

                if (s2f33.RPTITEMS != null && s2f33.RPTITEMS.Length > 0)
                    scApp.CEIDBLL.buildRptsFromMCS(s2f33.RPTITEMS);



                SECSConst.setDicRPTIDAndVID(scApp.CEIDBLL.loadDicRPTIDAndVID());

            }
            catch (Exception ex)
            {
                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}", line.LINE_ID, "S2F17_Receive_Date_Time_Req", ex.ToString());
            }
        }
        protected virtual void S2F35ReceiveLinkEventReport(object sender, SECSEventArgs e)
        {
            try
            {
                S2F35 s2f35 = ((S2F35)e.secsHandler.Parse<S2F35>(e));
                SCUtility.secsActionRecordMsg(scApp, true, s2f35);
                //if (!isProcess(s2f35)) { return; }


                S2F36 s2f36 = null;
                s2f36 = new S2F36();
                s2f36.SystemByte = s2f35.SystemByte;
                s2f36.SECSAgentName = scApp.EAPSecsAgentName;
                s2f36.LRACK = "0";

                TrxSECS.ReturnCode rtnCode = ISECSControl.replySECS(bcfApp, s2f36);
                SCUtility.secsActionRecordMsg(scApp, false, s2f36);
                if (rtnCode != TrxSECS.ReturnCode.Normal)
                {
                    logger.Warn("Reply EQPT S2F18 Error:{0}", rtnCode);
                }

                scApp.CEIDBLL.DeleteCEIDInfoByBatch();

                if (s2f35.RPTITEMS != null && s2f35.RPTITEMS.Length > 0)
                    scApp.CEIDBLL.buildCEIDsFromMCS(s2f35.RPTITEMS);

                SECSConst.setDicCEIDAndRPTID(scApp.CEIDBLL.loadDicCEIDAndRPTID());

            }
            catch (Exception ex)
            {
                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}", line.LINE_ID, "S2F17_Receive_Date_Time_Req", ex.ToString());
            }
        }
        protected virtual void S2F37ReceiveEnableDisableEventReport(object sender, SECSEventArgs e)
        {
            try
            {
                S2F37 s2f37 = ((S2F37)e.secsHandler.Parse<S2F37>(e));
                SCUtility.secsActionRecordMsg(scApp, true, s2f37);
                //if (!isProcess(s2f37)) { return; }
                Boolean isValid = true;
                //Boolean isEnable = SCUtility.isMatche(s2f37.CEED, SECSConst.CEED_Enable);
                Boolean isEnable = s2f37.CEED[0] == 255;

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

                S2F38 s2f18 = null;
                s2f18 = new S2F38()
                {
                    SystemByte = s2f37.SystemByte,
                    SECSAgentName = scApp.EAPSecsAgentName,
                    ERACK = isValid ? SECSConst.ERACK_Accepted : SECSConst.ERACK_Denied_At_least_one_CEID_dose_not_exist
                };

                TrxSECS.ReturnCode rtnCode = ISECSControl.replySECS(bcfApp, s2f18);
                SCUtility.secsActionRecordMsg(scApp, false, s2f18);
                if (rtnCode != TrxSECS.ReturnCode.Normal)
                {
                    logger.Warn("Reply EQPT S2F18 Error:{0}", rtnCode);
                }
            }
            catch (Exception ex)
            {
                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}", line.LINE_ID, "S2F17_Receive_Date_Time_Req", ex.ToString());
            }
        }
        protected virtual void S5F3ReceiveEnableDisableAlarm(object sender, SECSEventArgs e)
        {
            try
            {
                bool isSuccess = true;
                S5F3 s5f3 = ((S5F3)e.secsHandler.Parse<S5F3>(e));
                SCUtility.secsActionRecordMsg(scApp, true, s5f3);
                //if (isProcess(s5f3)) { return; }
                Boolean isEnable = SCUtility.isMatche(s5f3.ALED, SECSConst.ALED_Enable);
                string alarm_code = s5f3.ALID;

                scApp.TransferService.TransferServiceLogger.Info(
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS >> OHB|s5f3:\n" + s5f3.toSECSString());

                isSuccess = scApp.AlarmBLL.enableAlarmReport(alarm_code, isEnable);

                S5F4 s5f4 = null;
                s5f4 = new S5F4();
                s5f4.SystemByte = s5f3.SystemByte;
                s5f4.SECSAgentName = scApp.EAPSecsAgentName;
                s5f4.ACKC5 = isSuccess ? SECSConst.ACKC5_Accepted : SECSConst.ACKC5_Not_Accepted;

                TrxSECS.ReturnCode rtnCode = ISECSControl.replySECS(bcfApp, s5f4);
                SCUtility.secsActionRecordMsg(scApp, false, s5f4);

                scApp.TransferService.TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ")
                    + "MCS >> OHB|s5f4 ACKC5:" + s5f4.ACKC5 + "   SCES_ReturnCode:" + rtnCode);

                if (rtnCode != TrxSECS.ReturnCode.Normal)
                {
                    logger.Warn("Reply EQPT S2F18 Error:{0}", rtnCode);
                }
            }
            catch (Exception ex)
            {
                scApp.TransferService.TransferServiceLogger.Error(
                    DateTime.Now.ToString("HH:mm:ss.fff ") + "  S5F3ReceiveEnableDisableAlarm\n" + ex);

                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}", line.LINE_ID, "S2F17_Receive_Date_Time_Req", ex.ToString());
            }
        }

        protected virtual void S5F5ReceiveListAlarmRequest(object sender, SECSEventArgs e)
        {
            logger.Warn("GEMDriver has Warn [Line Name:{0}],[Error method:{1}],[Warn Message:{2}", line.LINE_ID, "S5F5ReceiveListAlarmRequest", "Not implemented");
        }


        protected virtual Boolean isProcess(SXFY sxfy)
        {
            Boolean isProcess = false;
            string streamFunction = sxfy.StreamFunction;
            if (line.Host_Control_State == SCAppConstants.LineHostControlState.HostControlState.EQ_Off_line)
            {
                if (sxfy is S1F17)
                {
                    isProcess = true;
                }
                else if (sxfy is S2F41)
                {
                    string rcmd = (sxfy as S2F41).RCMD;
                }
                else
                {
                    isProcess = false;
                }
            }
            else
            {
                isProcess = true;
            }
            //if (!isProcess)
            //{
            //    S1F0 sxf0 = new S1F0()
            //    {
            //        SECSAgentName = scApp.EAPSecsAgentName,
            //        StreamFunction = sxfy.getAbortFunctionName(),
            //        SystemByte = sxfy.SystemByte
            //    };
            //    SCUtility.secsActionRecordMsg(scApp, false, sxf0);
            //    TrxSECS.ReturnCode rtnCode = ISECSControl.replySECS(bcfApp, sxf0);
            //    SCUtility.actionRecordMsg(scApp, sxf0.StreamFunction, line.Real_ID,
            //                "Reply Abort To MES.", rtnCode.ToString());
            //}
            return isProcess;
        }
        #endregion Receive

        #region Send
        public virtual bool S1F1SendAreYouThere()
        {
            try
            {
                S1F1 s1f1 = new S1F1()
                {
                    SECSAgentName = scApp.EAPSecsAgentName
                };
                S1F2 s1f2 = null;
                string rtnMsg = string.Empty;
                SXFY abortSecs = null;
                //SCUtility.secsActionRecordMsg(scApp, false, s1f1);
                TrxSECS.ReturnCode rtnCode = ISECSControl.sendRecv<S1F2>(bcfApp, s1f1, out s1f2, out abortSecs, out rtnMsg, null);
                SCUtility.actionRecordMsg(scApp, s1f1.StreamFunction, line.Real_ID,
                                "Send Are You There To MES.", rtnCode.ToString());
                if (rtnCode == TrxSECS.ReturnCode.Normal)
                {
                    //SCUtility.secsActionRecordMsg(scApp, false, s1f2);
                    return true;
                }
                else if (rtnCode == TrxSECS.ReturnCode.Abort)
                {
                    SCUtility.secsActionRecordMsg(scApp, false, abortSecs);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exection:");
            }
            return false;
        }
        public virtual bool S5F1SendAlarmReport(string alcd, string alid, string altx)
        {
            throw new NotImplementedException();

        }

        public virtual bool S1F13SendEstablishCommunicationRequest()
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
                    line.EstablishComm = true;
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
        public virtual bool S2F17SendDateAndTimeRequest()
        {
            try
            {
                S2F17 s2f17 = new S2F17();
                s2f17.SECSAgentName = scApp.EAPSecsAgentName;

                S2F18 s2f18 = null;
                string rtnMsg = string.Empty;
                SXFY abortSecs = null;
                SCUtility.secsActionRecordMsg(scApp, false, s2f17);

                TrxSECS.ReturnCode rtnCode = ISECSControl.sendRecv<S2F18>(bcfApp, s2f17, out s2f18, out abortSecs, out rtnMsg, null);
                SCUtility.actionRecordMsg(scApp, s2f17.StreamFunction, line.Real_ID, "Date Time Request.", rtnCode.ToString());

                if (rtnCode == TrxSECS.ReturnCode.Normal)
                {
                    SCUtility.secsActionRecordMsg(scApp, true, s2f18);
                    string timeStr = s2f18.TIME;
                    DateTime mesDateTime = DateTime.Now;
                    try
                    {
                        mesDateTime = DateTime.ParseExact(timeStr.Trim(), SCAppConstants.TimestampFormat_16, CultureInfo.CurrentCulture);
                    }
                    catch (Exception dtEx)
                    {
                        logger.Error(dtEx, String.Format("Receive Date Time Set Request From MES. Format Error![Date Time:{0}]",
                            timeStr));
                    }

                    if (!DebugParameter.DisableSyncTime)
                    {
                        SCUtility.updateSystemTime(mesDateTime);
                    }
                    //todo 跟其他設備同步
                    return true;
                }
                else
                    logger.Warn("Send Date Time Request[S2F17] Error!");
            }
            catch (Exception ex)
            {
                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}", line.LINE_ID, "sendS2F17_DateTimeReq", ex.ToString());
            }

            return false;
        }

        public abstract bool S6F11SendEquiptmentOffLine();
        public abstract bool S6F11SendControlStateLocal();
        public abstract bool S6F11SendControlStateRemote();
        //public abstract bool S6F11SendAlarmCleared();
        //public abstract bool S6F11SendAlarmSet();



        public abstract AMCSREPORTQUEUE S6F11BulibMessage(string ceid, object Vids);
        public abstract bool S6F11SendMessage(AMCSREPORTQUEUE queue);


        /// <summary>
        /// 僅在測試階段使用
        /// </summary>
        protected bool isOnlineWithMcs = false;
        protected virtual Boolean isHostReady()
        {
            Boolean result = false;
            try
            {
                //return true;    //之後註解要拿掉
                //SCAppConstants.LineHostControlState.HostControlState s = scApp.getEQObjCacheManager().getLine().Host_Control_State;
                result =
                    (scApp.getEQObjCacheManager().getLine().Host_Control_State == SCAppConstants.LineHostControlState.HostControlState.On_Line_Local
                    || scApp.getEQObjCacheManager().getLine().Host_Control_State == SCAppConstants.LineHostControlState.HostControlState.On_Line_Remote
                    ) && line.Secs_Link_Stat == SCAppConstants.LinkStatus.LinkOK;
            }
            catch (Exception ex)
            {
                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}",
                    line.LINE_ID, "isSendEAP", ex.ToString());
            }
            return result;
        }
        protected virtual Boolean isSend(SXFY sxfy)
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
                //if (bcf.Common.BCFUtility.isMatche(sxfy.StreamFunction, "S6F11"))
                //{
                //    S6F11 s6f11 = null;
                //    string ceid = (string)sxfy.getField(bcf.Common.BCFUtility.getPropertyName(() => s6f11.CEID));
                //    if (!eventBLL.isEnableReport(ceid))
                //    {
                //        return false;
                //    }
                //}
            }
            catch (Exception ex)
            {
                logger.Error("MESDefaultMapAction has Error[Line Name:{0}],[Error method:{1}],[Error Message:{2}",
                    line.LINE_ID, "isSendEAP", ex.ToString());
            }
            return result;
        }
        #endregion Send


        #region Connected / Disconnection
        protected void secsConnected(object sender, SECSEventArgs e)
        {
            if (line.Secs_Link_Stat == SCAppConstants.LinkStatus.LinkOK) return;
            Dictionary<string, CommuncationInfo> dicCommunactionInfo =
                scApp.getEQObjCacheManager().CommonInfo.dicCommunactionInfo;
            if (dicCommunactionInfo.ContainsKey("MCS"))
            {
                dicCommunactionInfo["MCS"].IsConnectinoSuccess = true;
            }
            line.Secs_Link_Stat = SCAppConstants.LinkStatus.LinkOK;
            scApp.TransferService.TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS 連線");

            isOnlineWithMcs = true;
            line.connInfoUpdate_Connection();
            SCUtility.RecodeConnectionInfo
                ("MCS",
                SCAppConstants.RecodeConnectionInfo_Type.Connection.ToString(),
                line.StopWatch_mcsDisconnectionTime.Elapsed.TotalSeconds);

            ITimerAction timer = scApp.getBCFApplication().getTimerAction("SECSHeartBeat");
            if (timer != null && !timer.IsStarted)
            {
                timer.start();
            }

            initialWithMCS();

        }

        const int ESTABLISH_COMMUNICATION_MONITORING_TIME = 4000;
        private void initialWithMCS()
        {
            bool isSuccess = true;
            do
            {
                isSuccess = S1F13SendEstablishCommunicationRequest();
                System.Threading.SpinWait.SpinUntil(() => false, ESTABLISH_COMMUNICATION_MONITORING_TIME);
            }
            while (!isSuccess);
            //if (line.Host_Control_State != SCAppConstants.LineHostControlState.HostControlState.EQ_Off_line)
            //    S6F11SendEquiptmentOffLine();
            //S1F1
            //S1F1SendAreYouThere();
            //S2F17
            //S2F17SendDateAndTimeRequest();
            //...
        }

        protected void secsDisconnected(object sender, SECSEventArgs e)
        {
            if (line.Secs_Link_Stat == SCAppConstants.LinkStatus.LinkFail) return;
            //not implement
            Dictionary<string, CommuncationInfo> dicCommunactionInfo =
                scApp.getEQObjCacheManager().CommonInfo.dicCommunactionInfo;
            if (dicCommunactionInfo.ContainsKey("MCS"))
            {
                dicCommunactionInfo["MCS"].IsConnectinoSuccess = false;
            }
            isOnlineWithMcs = false;
            line.Secs_Link_Stat = SCAppConstants.LinkStatus.LinkFail;

            Task.Run(() => scApp.LineService.OfflineWithHostByOp());

            scApp.TransferService.TransferServiceLogger.Info(DateTime.Now.ToString("HH:mm:ss.fff ") + "MCS 斷線");

            line.connInfoUpdate_Disconnection();

            SCUtility.RecodeConnectionInfo
                ("MCS",
                SCAppConstants.RecodeConnectionInfo_Type.Disconnection.ToString(),
                line.StopWatch_mcsConnectionTime.Elapsed.TotalSeconds);
        }
        #endregion Connected / Disconnection
    }
}
