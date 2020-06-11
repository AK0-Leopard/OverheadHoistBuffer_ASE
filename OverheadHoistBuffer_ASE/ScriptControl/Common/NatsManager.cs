using com.mirle.ibg3k0.sc.App;
using NATS.Client;
using STAN.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Common
{
    public class NatsManager
    {
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        StanConnectionFactory StanConnectionFactory = null;
        SCApplication scApp = null;
        //ConcurrentDictionary<string, IStanConnection> dicConnection = new ConcurrentDictionary<string, IStanConnection>();
        IStanConnection conn = null;
        ConcurrentDictionary<string, IStanSubscription> dicSubscription = new ConcurrentDictionary<string, IStanSubscription>();
        StanOptions cOpts = StanOptions.GetDefaultOptions();
        readonly string DefaultNatsURL = "nats://192.168.39.222:4222";
        readonly string[] servers_port = new string[] { "4222", "4223", "4224" };

        string producID = null;
        string clusterID = null;
        string clientID = null;


        public NatsManager(SCApplication _scApp, string product_id, string cluster_id, string client_id)
        {
            try
            {
                scApp = _scApp;
                producID = product_id;
                clusterID = cluster_id;
                clientID = client_id;

                string nats_server_ip = getHostTableIP("nats.ohxc.mirle.com.tw");
                if (nats_server_ip != null)
                {
                    DefaultNatsURL = $"nats://{nats_server_ip}:4222";
                }

                string[] srevers_url = new string[servers_port.Length];
                for (int i = 0; i < srevers_url.Length; i++)
                {
                    srevers_url[i] = $"nats://{nats_server_ip}:{servers_port[i]}";
                }
                Options natsOptions = null;
#if DEBUG
                cOpts.NatsURL = DefaultNatsURL;
                clusterID = "test-cluster";
#else
                natsOptions = ConnectionFactory.GetDefaultOptions();
                natsOptions.MaxReconnect = Options.ReconnectForever;
                natsOptions.ReconnectWait = 1000;
                natsOptions.NoRandomize = true;
                natsOptions.Servers = srevers_url;
                natsOptions.Name = client_id;
                natsOptions.AllowReconnect = true;
                natsOptions.Timeout = 1000;
                natsOptions.PingInterval = 5000;
                natsOptions.Url = DefaultNatsURL;
                natsOptions.AsyncErrorEventHandler += (sender, args) =>
                {
                    logger.Error($"Server:{args.Conn.ConnectedUrl}{Environment.NewLine},Message:{args.Error}{Environment.NewLine},Subject:{args.Subscription.Subject}");
                    //Console.WriteLine("Error: ");
                    //Console.WriteLine("   Server: " + args.Conn.ConnectedUrl);
                    //Console.WriteLine("   Message: " + args.Error);
                    //Console.WriteLine("   Subject: " + args.Subscription.Subject);
                };

                natsOptions.ServerDiscoveredEventHandler += (sender, args) =>
                {
                    logger.Info($"A new server has joined the cluster:{String.Join(", ", args.Conn.DiscoveredServers)}");
                    //Console.WriteLine("A new server has joined the cluster:");
                    //Console.WriteLine("    " + String.Join(", ", args.Conn.DiscoveredServers));
                };

                natsOptions.ClosedEventHandler += (sender, args) =>
                {
                    logger.Info($"Connection Closed:{Environment.NewLine}Server:{args.Conn.ConnectedUrl}");
                    //Console.WriteLine("Connection Closed: ");
                    //Console.WriteLine("   Server: " + args.Conn.ConnectedUrl);
                };

                natsOptions.DisconnectedEventHandler += (sender, args) =>
                {
                    logger.Info($"Connection Disconnected:{Environment.NewLine}Server:{args.Conn.ConnectedUrl}");
                    //Console.WriteLine("Connection Disconnected: ");
                    //Console.WriteLine("   Server: " + args.Conn.ConnectedUrl);
                };
                natsOptions.ReconnectedEventHandler += (sender, args) =>
                {
                    logger.Info($"Connection Reconnected:{Environment.NewLine}Server:{args.Conn.ConnectedUrl}");
                    //Console.WriteLine("Connection Disconnected: ");
                    //Console.WriteLine("   Server: " + args.Conn.ConnectedUrl);
                };
                IConnection natsConn = null;
                natsConn = new ConnectionFactory().CreateConnection(natsOptions);
                cOpts.NatsConn = natsConn;

#endif


                StanConnectionFactory = new StanConnectionFactory();

                // cOpts.NatsURL = DefaultNatsURL;
                conn = getConnection();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception");
            }
        }

        private string getHostTableIP(string ip)
        {
            string return_ip = null;
            var remoteipAdr = System.Net.Dns.GetHostAddresses(ip);
            if (remoteipAdr != null && remoteipAdr.Count() > 0)
            {
                return_ip = remoteipAdr[0].ToString();
            }

            return return_ip;
        }

        IStanConnection getConnection()
        {
            IStanConnection conn = null;
            try
            {
                conn = StanConnectionFactory.CreateConnection(clusterID, clientID, cOpts);
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Warn(ex, "Creat nats connection fail!");
            }
            return conn;
        }

        public string Publish(string subject, byte[] data, EventHandler<StanAckHandlerArgs> handler)
        {
            if (conn != null && conn.NATSConnection.State == ConnState.CONNECTED)
            {
                subject = $"{producID}_{subject}";
                return conn.Publish(subject, data, handler);
            }
            else
                return null;
        }

        public void Publish(string subject, byte[] data)
        {
            if (conn != null && conn.NATSConnection.State == ConnState.CONNECTED)
            {
                subject = $"{producID}_{subject}";
                conn.Publish(subject, data);
            }
        }
        public void PublishAsync(string subject, byte[] data)
        {
            if (conn != null &&
                conn.NATSConnection.State == ConnState.CONNECTED &&
                scApp.getEQObjCacheManager().getLine().ServiceMode == SCAppConstants.AppServiceMode.Active)
            {
                subject = $"{producID}_{subject}";
                conn.PublishAsync(subject, data);
            }
        }

        public void Subscriber(string subject, EventHandler<StanMsgHandlerArgs> handler, bool in_all = false, bool is_last = false, ulong since_seq_no = 0, DateTime? since_duration = null)
        {
            if (conn == null || conn.NATSConnection.State != ConnState.CONNECTED) return;
            StanSubscriptionOptions sOpts = StanSubscriptionOptions.GetDefaultOptions();
            if (in_all)
            {
                sOpts.DeliverAllAvailable();
            }
            else if (is_last)
            {
                sOpts.StartWithLastReceived();
            }
            else if (since_seq_no != 0)
            {
                sOpts.StartAt(since_seq_no);
            }
            else if (since_duration.HasValue)
            {
                sOpts.StartAt(since_duration.Value);
            }
            subject = $"{producID}_{subject}";
            dicSubscription.GetOrAdd(subject, conn.Subscribe(subject, sOpts, handler));
        }
        public void Unsubscriber(string subject)
        {
            dicSubscription.TryRemove(subject, out IStanSubscription stanSubscription);
        }
        public void close()
        {
            if (dicSubscription.Count > 0)
            {
                foreach (var keyPair in dicSubscription)
                {
                    keyPair.Value.Close();
                }
            }

            conn?.Close();
        }



    }
}
