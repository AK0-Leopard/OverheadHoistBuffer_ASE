using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using Nancy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Threading;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.Data.ValueDefMapAction;

namespace com.mirle.ibg3k0.sc.WebAPI
{
    public class MTSMTLInfo : NancyModule
    {
        //SCApplication app = null;
        const string restfulContentType = "application/json; charset=utf-8";
        const string urlencodedContentType = "application/x-www-form-urlencoded; charset=utf-8";
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        Timer ThreadTimer = null;
        public MTSMTLInfo()
        {
            //app = SCApplication.getInstance();
            RegisterMTSMTLEvent();
            After += ctx => ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");

        }


        private void RegisterMTSMTLEvent()
        {
            Post["MTSMTLInfo/InterlockRequest"] = (p) =>
            {
                var scApp = SCApplication.getInstance();
                string result = string.Empty;
                bool isSuccess = true;
                string station_id = Request.Query.station_id.Value ?? Request.Form.station_id.Value ?? string.Empty;
                string isSet = Request.Query.priority.Value ?? Request.Form.isSet.Value ?? string.Empty;
                try
                {
                    AEQPT MTLMTS = scApp.getEQObjCacheManager().getEquipmentByEQPTID(station_id);
                    bool setValue = Convert.ToBoolean(isSet);
                    if (MTLMTS != null)
                    {
                        if (MTLMTS.EQPT_ID.StartsWith("MTL"))
                        {
                            MTLMTS = MTLMTS as MaintainLift;
                            MTxValueDefMapActionBase MTLValueDefMapActionBase = MTLMTS.getMapActionByIdentityKey(nameof(MTLValueDefMapActionNew)) as MTxValueDefMapActionBase;
                            isSuccess = MTLValueDefMapActionBase.setOHxC2MTL_CarOutInterlock(setValue);
                        }
                        else if (MTLMTS.EQPT_ID.StartsWith("MTS"))
                        {
                            MTLMTS = MTLMTS as MaintainSpace;
                            MTxValueDefMapActionBase MTSValueDefMapActionBase = MTLMTS.getMapActionByIdentityKey(nameof(MTSValueDefMapActionNew)) as MTxValueDefMapActionBase;
                            isSuccess = MTSValueDefMapActionBase.setOHxC2MTL_CarOutInterlock(setValue);
                        }
                        else
                        {
                            isSuccess = false;
                        }

                        if (isSuccess)
                        {
                            result = "OK";
                        }
                        else
                        {
                            result = "Set interlock failed.";
                        }
                    }
                    else
                    {
                        result = $"Can not find station[{station_id}].";
                    }
                }
                catch (Exception ex)
                {
                    result = "Set interlock failed with exception happened.";
                }

                var response = (Response)result;
                response.ContentType = restfulContentType;
                return response;
            };

        }
    }
}
