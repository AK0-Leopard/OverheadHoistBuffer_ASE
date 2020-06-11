using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Forms;
using System.Xml;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
namespace com.mirle.ibg3k0.sc.MQTT
{
    public class MQTTControl
    {
        IPAddress MQTTIPaddress = null;
        string MQTTPort = null;
        public MqttClient client = null;
        public MQTTControl()
        {
            getMQTTConfig();
            client = new MqttClient(MQTTIPaddress);
            string clientId = Guid.NewGuid().ToString();
            client.Connect(clientId);
        }
        ~MQTTControl()
        {
            client.Disconnect();
        }
        void getMQTTConfig()
        {
            string path = Directory.GetCurrentDirectory()+@"\Config\MQTTSetting.config";
            
            XmlReader reader;
            try
            {
                reader = XmlReader.Create(path);
            }
            catch (Exception)
            {
                MessageBox.Show("Read MQTT config fail.");
                return;
            }
            // Begin to parase
            try
            {
                reader.ReadToFollowing("MQTT");
                MQTTIPaddress = IPAddress.Parse(reader.GetAttribute("IPaddress"));
                //MQTTPort = reader.GetAttribute("Port");
            }
            catch (Exception)
            {
                MessageBox.Show("Read Language File Failed.");
                return;
            }
            return;
        }
        /// <summary>
        /// 推播一則訊息至MQTT Server
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="msg"></param>
        public void MQTTPub(string topic, string msg)
        {
            string strValue = Convert.ToString(msg);

            client.Publish(topic, Encoding.UTF8.GetBytes(strValue));
        }
        /// <summary>
        /// 向MQTT Server訂閱某一topic的訊息
        /// </summary>
        /// <param name="topic"></param>
        public void MQTTSub(string topic,byte QoS)
        {

            // subscribe the topic with QoS 2 
            client.Subscribe(new string[] { topic }, new byte[] {QoS});
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

        }
        static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string msg = System.Text.Encoding.UTF8.GetString(e.Message, 0, e.Message.Length);
            // handle message received 
        }
    }
}
