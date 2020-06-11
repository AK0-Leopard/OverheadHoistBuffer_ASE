using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using Nancy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.WebAPI
{
    public class ConnectionInfo : NancyModule
    {
        SCApplication app = null;
        const string restfulContentType = "application/json; charset=utf-8";
        const string urlencodedContentType = "application/x-www-form-urlencoded; charset=utf-8";
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public ConnectionInfo()
        {
            //app = SCApplication.getInstance();
            RegisterConnectionEvent();
            After += ctx => ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");

        }
        private void RegisterConnectionEvent()
        {
            Post["ConnectionInfo/LinkStatusChange"] = (p) =>
            {
                var scApp = SCApplication.getInstance();
                bool isSuccess = true;
                string result = string.Empty;

                string linkStatus = Request.Query.linkstatus.Value ?? Request.Form.linkstatus.Value ?? string.Empty;
                try
                {
                    isSuccess = scApp.ConnectionInfoService.doChangeLinkStatus(linkStatus, out result);
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    result = "Execption happend!";
                    logger.Error(ex, "Execption:");
                }
                var response = (Response)result;
                response.ContentType = restfulContentType;
                return response;
            };

            Post["ConnectionInfo/HostModeChange"] = (p) =>
            {
                var scApp = SCApplication.getInstance();
                bool isSuccess = true;
                string result = string.Empty;

                string host_mode = Request.Query.hostmode.Value ?? Request.Form.hostmode.Value ?? string.Empty;
                try
                {
                    isSuccess = scApp.ConnectionInfoService.doChangeHostMode(host_mode, out result);
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    result = "Execption happend!";
                    logger.Error(ex, "Execption:");
                }
                var response = (Response)result;
                response.ContentType = restfulContentType;
                return response;
            };

            Post["ConnectionInfo/TSCStateChange"] = (p) =>
            {
                var scApp = SCApplication.getInstance();
                bool isSuccess = true;
                string result = string.Empty;

                string tsc_state = Request.Query.tscstate.Value ?? Request.Form.tscstate.Value ?? string.Empty;
                try
                {
                    isSuccess = scApp.ConnectionInfoService.doChangeTSCstate(tsc_state, out result);
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    result = "Execption happend!";
                    logger.Error(ex, "Execption:");
                }
                var response = (Response)result;
                response.ContentType = restfulContentType;
                return response;
            };

            Post["ConnectionInfo/ThisExist"] = (p) =>
            {
                var scApp = SCApplication.getInstance();
                bool isSuccess = true;
                string result = "OK";

                //string tsc_state = Request.Query.tscstate.Value ?? Request.Form.tscstate.Value ?? string.Empty;
                try
                {
                    //isSuccess = scApp.ConnectionInfoService.doChangeTSCstate(tsc_state, out result);
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    result = "OK";
                    logger.Error(ex, "Execption:");
                }
                var response = (Response)result;
                response.ContentType = restfulContentType;
                return response;
            };
        }
    }
}
