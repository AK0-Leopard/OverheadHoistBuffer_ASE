//*********************************************************************************
//      SECSConnectionConfigHandler.cs
//*********************************************************************************
// File Name: TcpIpConnectionConfigHandler.cs
// Description: 解析App.config TcpIp Connection特定設定內容
//
//(c) Copyright 2017, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace com.mirle.ibg3k0.sc.ConfigHandler
{
    /// <summary>
    /// Class SECSConnectionConfigHandler.
    /// </summary>
    /// <seealso cref="System.Configuration.IConfigurationSectionHandler" />
    public class DataCollectionConfigHandler : IConfigurationSectionHandler
    {
        /// <summary>
        /// 建立組態區段處理常式。
        /// </summary>
        /// <param name="parent">父物件。</param>
        /// <param name="configContext">組態內容物件。</param>
        /// <param name="section">區段 XML 節點。</param>
        /// <returns>已建立的區段處理常式物件。</returns>
        /// <exception cref="ConfigurationErrorsException">
        /// [Connection] Attribute required: Name
        /// or
        /// [Connection] Attribute required: DeviceID
        /// or
        /// [Connection] Attribute required: LocalIP
        /// or
        /// [Connection] Attribute required: LocalPort
        /// or
        /// [Connection] Attribute required: RemoteIP
        /// or
        /// [Connection] Attribute required: RemotePort
        /// or
        /// [Connection] Attribute required: T3Timeout
        /// or
        /// [Connection] Attribute required: T5Timeout
        /// or
        /// [Connection] Attribute required: T6Timeout
        /// or
        /// [Connection] Attribute required: T7Timeout
        /// or
        /// [Connection] Attribute required: T8Timeout
        /// or
        /// [Connection] Attribute required: RetryCount
        /// or
        /// [Connection] Attribute required: TXLogPath
        /// or
        /// [Connection] Attribute required: ConnectMode
        /// </exception>
        /// <exception cref="System.Exception">[Connection] Attribute Format Error: ConnectMode</exception>
        public virtual object Create(object parent, object configContext, XmlNode section)
        {
            List<DataCollectionConfigSection> tcpipConfigSectionList = new List<DataCollectionConfigSection>();
            if (!section.Name.Equals("DataCollectionSetting"))
            {
                return null;
            }

            string method_name = string.Empty;
            string ip = string.Empty;
            int port = 0;
            string item = string.Empty;
            int period = 0;
            bool is_report = false;
            string item_name = string.Empty;
            List<DataCollectionConfigSection> sections = new List<DataCollectionConfigSection>();
            foreach (XmlNode methodNode in section.ChildNodes)
            {
                try
                {
                    method_name = methodNode.Attributes["Name"].Value.Trim();
                }
                catch (Exception)
                {
                    throw new ConfigurationErrorsException("[Name] Attribute required: Name");
                }
                try
                {
                    ip = methodNode.Attributes["IP"].Value.Trim();
                }
                catch (Exception)
                {
                    throw new ConfigurationErrorsException("[IP] Attribute required: Name");
                }
                try
                {
                    port = Convert.ToInt32(methodNode.Attributes["Port"].Value.Trim());
                }
                catch (Exception)
                {
                    throw new ConfigurationErrorsException("[Port] Attribute required: Name");
                }

                List<DataCollectionConfigItem> items = new List<DataCollectionConfigItem>();
                foreach (XmlNode ItemNode in methodNode.ChildNodes)
                {
                    try
                    {
                        item_name = ItemNode.Attributes["Name"].Value.Trim();
                    }
                    catch (Exception)
                    {
                        throw new ConfigurationErrorsException("[Name] Attribute required: Name");
                    }
                    try
                    {
                        period = Convert.ToInt32(ItemNode.Attributes["Period"].Value.Trim());
                    }
                    catch (Exception)
                    {
                        throw new ConfigurationErrorsException("[Period] Attribute required: Name");
                    }
                    try
                    {
                        is_report = ItemNode.Attributes["IsReport"].Value.Trim() == "Y" ? true : false;
                    }
                    catch (Exception)
                    {
                        throw new ConfigurationErrorsException("[IsReport] Attribute required: Name");
                    }
                    items.Add(new DataCollectionConfigItem(item_name, period, is_report));
                }
                DataCollectionConfigSection datacollection_section = new DataCollectionConfigSection(method_name,ip,port, items);
                sections.Add(datacollection_section);
            }
            return new DataCollectionConfigSections(sections);
        }
    }

    /// <summary>
    /// Class SECSConfigSections.
    /// </summary>
    public class DataCollectionConfigSections
    {
        public List<DataCollectionConfigSection> DataCollectionConfigSectionList { get; private set; }
        public DataCollectionConfigSections(List<DataCollectionConfigSection> _DataCollectionSectionList)
        {

            this.DataCollectionConfigSectionList = _DataCollectionSectionList;
        }
    }

    /// <summary>
    /// Class DataCollectionConfigSection.
    /// </summary>
    public class DataCollectionConfigSection
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the Period
        /// </summary>
        /// <value>The local ip.</value>
        public string IP { get; private set; }
        /// <summary>
        /// Gets the IsReport
        /// </summary>
        /// <value>The local port.</value>
        public int Port { get; private set; }

        public List<DataCollectionConfigItem> DataCollectionItemList { get; private set; }

        public DataCollectionConfigSection(string name, string ip, int port, List<DataCollectionConfigItem> itemList)
        {
            this.Name = name;
            this.IP = ip;
            this.Port = port;
            this.DataCollectionItemList = itemList;
        }

    }

    /// <summary>
    /// Class DataCollectionConfigSection.
    /// </summary>
    public class DataCollectionConfigItem
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the Period
        /// </summary>
        /// <value>The local ip.</value>
        public int Period { get; private set; }
        /// <summary>
        /// Gets the IsReport
        /// </summary>
        /// <value>The local port.</value>
        public bool IsReport { get; private set; }



        public DataCollectionConfigItem(string name, int period, bool is_report)
        {
            this.Name = name;
            this.Period = period;
            this.IsReport = is_report;
        }

    }

}
