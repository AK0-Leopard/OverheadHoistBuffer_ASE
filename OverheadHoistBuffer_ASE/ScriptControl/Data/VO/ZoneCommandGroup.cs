// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="AlarmMap.cs" company="">
//     Copyright ©  2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.VO
{
    public class ZoneCommandGroup
    {
        public ZoneCommandGroup(string id, List<string> portIDs, string dir)
        {
            ZoneCommandID = id;
            if (portIDs == null)
                PortIDs = new List<string>();
            else
                PortIDs = portIDs;
            zoneDir = convertZoneDir(dir);
        }

        private ZoneDir convertZoneDir(string dir)
        {
            switch (dir)
            {
                case "(1,0)":
                    return ZoneDir.DIR_1_0;
                case "(0,1)":
                    return ZoneDir.DIR_0_1;
                case "(-1,0)":
                    return ZoneDir.DIR_N1_0;
                case "(0,-1)":
                    return ZoneDir.DIR_0_N1;
                default:
                    throw new ArgumentException($"convertZoneDir,dir:{dir} not exist.");
            }
        }

        public String ZoneCommandID;
        public List<string> PortIDs { get; private set; }
        public ZoneDir zoneDir { get; private set; }

        /// <summary>
        /// 用來代表該Zone在電腦座標中，排序的方向
        /// (1,0) => 代表X為正方向的排序
        /// (0,1) => 代表Y為正方向的排序
        /// (N1,0) => 代表X為負方向的排序
        /// (0,N1) => 代表Y為負方向的排序
        /// </summary>
        public enum ZoneDir
        {
            DIR_N1_0,
            DIR_0_1,
            DIR_1_0,
            DIR_0_N1,

        }
    }
}
