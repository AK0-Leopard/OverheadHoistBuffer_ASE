// ***********************************************************************
// Assembly         : BC
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="ConfigSystem.cs" company="Mirle">
//     Copyright ©2014 MIRLE.3K0
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.bc.winform
{
    /// <summary>
    /// Class ConfigSystem. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="System.Configuration.Internal.IInternalConfigSystem" />
    public sealed class ConfigSystem : IInternalConfigSystem
    {
        /// <summary>
        /// The client configuration system
        /// </summary>
        private static IInternalConfigSystem clientConfigSystem;
        /// <summary>
        /// Installs this instance.
        /// </summary>
        public static void Install()
        {
            FieldInfo[] fiStateValues = null;
            Type tInitState = typeof(System.Configuration.ConfigurationManager).GetNestedType("InitState", BindingFlags.NonPublic);
            if (null != tInitState)
            {
                fiStateValues = tInitState.GetFields();
            }
            FieldInfo fiInit = typeof(System.Configuration.ConfigurationManager).GetField("s_initState", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo fiSystem = typeof(System.Configuration.ConfigurationManager).GetField("s_configSystem", BindingFlags.NonPublic | BindingFlags.Static);
            if (fiInit != null && fiSystem != null && null != fiStateValues)
            {
                fiInit.SetValue(null, fiStateValues[1].GetValue(null));
                fiSystem.SetValue(null, null);
            }
            ConfigSystem confSys = new ConfigSystem();
      //      Type configFactoryType = Type.GetType
      //          ("System.Configuration.Internal.InternalConfigSettingsFactory, System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
      //          true);
            
            Type configFactoryType = Type.GetType
                ("System.Configuration.Internal.InternalConfigSettingsFactory, System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
                true);
            IInternalConfigSettingsFactory configSettingsFactory =
                (IInternalConfigSettingsFactory)Activator.CreateInstance(configFactoryType, true);
            configSettingsFactory.SetConfigurationSystem(confSys, false);
       //     Type clientConfigSystemType = Type.GetType
       //         ("System.Configuration.ClientConfigurationSystem, System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", true);
       //     clientConfigSystem = (IInternalConfigSystem)Activator.CreateInstance(clientConfigSystemType, true);
            Type clientConfigSystemType = Type.GetType
                ("System.Configuration.ClientConfigurationSystem, System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", true);
            clientConfigSystem = (IInternalConfigSystem)Activator.CreateInstance(clientConfigSystemType, true);

        }

        /// <summary>
        /// 依據指定的索引鍵傳回組態物件。
        /// </summary>
        /// <param name="configKey">組態索引鍵值。</param>
        /// <returns>組態物件。</returns>
        public object GetSection(string configKey)
        {
            // get the section from the default location (web.config or app.config)
            object section = clientConfigSystem.GetSection(configKey);
            return section;
        }

        /// <summary>
        /// 依據指定的區段名稱重新整理組態系統。
        /// </summary>
        /// <param name="sectionName">組態區段的名稱。</param>
        public void RefreshConfig(string sectionName)
        {
            clientConfigSystem.RefreshConfig(sectionName);
        }

        /// <summary>
        /// 取得值，指出是否支援使用者組態。
        /// </summary>
        /// <value><c>true</c> if [supports user configuration]; otherwise, <c>false</c>.</value>
        public bool SupportsUserConfig
        {
            get { return clientConfigSystem.SupportsUserConfig; }
        }
    }
}
