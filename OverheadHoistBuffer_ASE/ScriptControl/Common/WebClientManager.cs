using NLog;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Http;

namespace com.mirle.ibg3k0.sc.Common
{
    public class WebClientManager
    {

        public enum HTTP_METHOD
        {
            GET,
            POST,
            DELET,
            PUT,
            PATCH
        }

        private static Object _lock = new Object();
        private static WebClientManager manager;
        public static WebClientManager getInstance()
        {
            if (manager == null)
            {
                lock (_lock)
                {
                    if (manager == null)
                    {
                        manager = new WebClientManager();
                    }
                }
            }
            return manager;
        }

        private WebClientManager()
        {
        }


        
        public string GetInfoFromServer(string uri, string[] action_targets, string[] param)
        {
            string p = string.Join("", param);
            return GetInfoFromServer(uri, action_targets, p);
        }
        public string GetInfoFromServer(string uri, string[] action_targets, string param)
        {
            string result = "default";
            string action_target = string.Join("/", action_targets);
            string address = $"{uri}/{action_target}/{param}";
            LogHelper.Log(logger: NLog.LogManager.GetCurrentClassLogger(), LogLevel: LogLevel.Info, Class: nameof(WebClientManager), Device: "OHxC",
               Data:$"send GetInfoFromServer: {address} ");

            // HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create($"{uri}/{action_target}/{param}");
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(address);
            httpWebRequest.Timeout = 5000;
            httpWebRequest.Method = HTTP_METHOD.GET.ToString();
            //指定 request 的 content type
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";

            using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                httpResponse.Close();
            }
            httpWebRequest.Abort();
            return result;
        }

        public string PostInfoToServer(string[] action_targets, HTTP_METHOD methed, byte[] byteArray)
        {
            string result = string.Empty;
            string action_target = string.Join("/", action_targets);
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create($"http://ohxc2.ohxc.mirle.com.tw:3280/{action_target}");
            httpWebRequest.Method = methed.ToString();
            httpWebRequest.ContentLength = byteArray.Length;
            //指定 request 的 content type
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";

            using (Stream reqStream = httpWebRequest.GetRequestStream())
            {
                reqStream.Write(byteArray, 0, byteArray.Length);
                reqStream.Close();
            }
            using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                httpResponse.Close();
            }
            httpWebRequest.Abort();
            return result;
        }

        public void postInfo2Stock(string host_ip, string port_id, string cst_id, string proc_type)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var address = $"http://{host_ip}:9000/api/io/{port_id}/{proc_type}/{cst_id}";
                    LogManager.GetCurrentClassLogger().Trace($"Send To STK:{address}");
                    var httpResponseMessage = client.PostAsync(address, null).Result;
                    LogManager.GetCurrentClassLogger().Trace($"Result:{httpResponseMessage}");
                }
            }
            catch (Exception ex)
            {

            }
        }


    }
}
