using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using Common.Logging;
using StackExchange.Redis;
using STAN.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Common
{
    public class CheckSystemEventHandler
    {
        const string REDIS_KEY_CHECK_SYSTEM_EXIST_FLAG = "CHECK_SYSTEM_EXIST";
        RedisCacheManager redisCacheManager = null;
        NatsManager natsManager = null;
        SCApplication app = null;
        public CheckSystemEventHandler()
        {

        }
        public void Start(SCApplication _app)
        {
            app = _app;
            redisCacheManager = app.getRedisCacheManager();
            //redisCacheManager.SubscriptionEvent(SCAppConstants.REDIS_EVENT_KEY, RedisEventHandler);

            natsManager = app.getNatsManager();
            natsManager.Subscriber(SCAppConstants.REDIS_EVENT_KEY, AMS_EVENT_HANDLER);
        }

        public void CheckCheckSystemIsExist()
        {
            bool isExist = redisCacheManager.KeyExists(REDIS_KEY_CHECK_SYSTEM_EXIST_FLAG);
            app.getEQObjCacheManager().getLine().DetectionSystemExist =
                isExist ? SCAppConstants.ExistStatus.Exist : SCAppConstants.ExistStatus.NoExist;
        }

        private long ADVANCE_NOTICE_OBSTRUCT_SyncPoint = 0;
        private void AMS_EVENT_HANDLER(object sender, StanMsgHandlerArgs e)
        {
            var bytes = e.Message.Data;
            string sillegalInfoValue = System.Text.Encoding.Default.GetString(bytes);
            string[] eventInfo = sillegalInfoValue.Split(',');
            string vh_id = eventInfo[0];
            string illegal_code = eventInfo[1];
            string time = eventInfo[2];
            string messageValue = eventInfo[3];

            switch (illegal_code)
            {
                case SCAppConstants.REDIS_EVENT_CODE_ILLEGAL_ENTRY_BLOCK_ZONE:
                    string[] Illegal_Entry_Section = getMessageList(messageValue);
                    List<BLOCKZONEQUEUE> queues = app.MapBLL.loadNonReleaseBlockQueueBySecIds(Illegal_Entry_Section.ToList());
                    foreach (BLOCKZONEQUEUE queue in queues)
                    {
                        //app.VehicleService.PAUSE_REQ
                        //    (queue.CAR_ID.Trim(),
                        //    ProtocolFormat.OHTMessage.PauseEvent.Pause,
                        //    ProtocolFormat.OHTMessage.PauseType.OhxC);
                    }
                    //     bcf.App.BCFApplication.onWarningMsg($"{vh_id} illegal entry block zone:{messageValue}");
                    break;
                case SCAppConstants.REDIS_EVENT_CODE_ADVANCE_NOTICE_OBSTRUCT_VH:


                    break;
                case SCAppConstants.REDIS_EVENT_CODE_VEHICLE_IDEL_WARNING:
                    string[] idel_warning_info = getMessageList(messageValue);
                    string current_section = idel_warning_info[0];
                    string section_last_update_time = idel_warning_info[1];
                    VHActionStatus action_status;
                    Enum.TryParse(idel_warning_info[2], out action_status);

                    bool isWarning = bool.Parse(idel_warning_info[3]);
                    if (isWarning)
                    {
                        if (action_status != VHActionStatus.NoCommand)
                        {
                            AVEHICLE vh = app.VehicleBLL.getVehicleByID(vh_id);

                            app.VehicleBLL.DoIdleVehicleHandle_InAction(vh.HAS_CST == 1 ?
                                                                        VhLoadCarrierStatus.Exist : VhLoadCarrierStatus.NotExist);
                        }
                        else
                        {
                            //TODO 要加入回Home點的動作
                            //app.VehicleBLL.DoIdleVehicleHandle_NoAction(vh_id);
                        }
                        bcf.App.BCFApplication.onWarningMsg
                            ($"{vh_id} is idle for a long time,current sec id [{current_section}].");
                    }
                    else
                    {
                        bcf.App.BCFApplication.onInfoMsg
                            ($"{vh_id} resume the move.");
                    }
                    break;
                case SCAppConstants.REDIS_EVENT_CODE_VEHICLE_LOADUNLOAD_TOO_LONG_WARNING:
                    string[] loadunload_toolong_info = getMessageList(messageValue);
                    string mcs_cmd = loadunload_toolong_info[0];
                    string current_sec_id = loadunload_toolong_info[1];
                    string recent_tran_event = loadunload_toolong_info[1];
                    bcf.App.BCFApplication.onWarningMsg
                        ($"{vh_id} is {recent_tran_event} for a long time,current sec id [{current_sec_id}].");
                    break;
                case SCAppConstants.REDIS_EVENT_CODE_EARTHQUAKE_ON:
                    string[] earthquake_info = getMessageList(messageValue);
                    bool isHappend = bool.Parse(earthquake_info[0]);
                    ALINE line = app.getEQObjCacheManager().getLine();
                    line.IsEarthquakeHappend = isHappend;
                    if (isHappend)
                    {
                        bcf.App.BCFApplication.onErrorMsg("An earthquake has occurred !!!");
                        app.VehicleService.PauseAllVehicleByOHxCPause();
                    }
                    else
                    {
                        app.VehicleService.ResumeAllVehicleByOhxCPause();
                    }
                    break;
                case SCAppConstants.REDIS_EVENT_CODE_ADVANCE_NOTICE_OBSTRUCTED_VH:
                    string[] obstructed_vh_info = getMessageList(messageValue);
                    string obstructed_vh = vh_id;
                    string obstruct_vh = obstructed_vh_info[0];
                    app.VehicleService.OHxCPauseRequest(obstructed_vh, PauseEvent.Pause, SCAppConstants.OHxCPauseType.Obstacle);
                    break;
                case SCAppConstants.REDIS_EVENT_CODE_NOTICE_OBSTRUCTED_VH_CONTINUE:
                    string _obstructed_vh = vh_id;
                    app.VehicleService.OHxCPauseRequest(_obstructed_vh, PauseEvent.Continue, SCAppConstants.OHxCPauseType.Obstacle);
                    break;
                case SCAppConstants.REDIS_EVENT_CODE_NOTICE_THE_VH_NEEDS_TO_CHANGE_THE_PATH:
                    string need_change_the_path_vh = vh_id;
                    app.VehicleService.VhicleChangeThePath(vh_id, true);
                    break;
                default:
                    break;
            }
        }


        private static string[] getMessageList(string messageValue)
        {
            string[] values;
            if (messageValue.Contains("-"))
            {
                values = messageValue.Split('-');
            }
            else
            {
                values = new string[1];
                values[0] = messageValue;
            }

            return values;
        }
    }
}
