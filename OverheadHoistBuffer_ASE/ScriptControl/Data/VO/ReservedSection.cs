﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.VO
{
    public class ReservedSection
    {
        public ReservedSection(string vehicleID,string sectionID)
        {
            VehicleID = vehicleID;
            SectionID = sectionID;
        }
        public string VehicleID { get; private set; }
        public string SectionID { get; private set; }
    }
}
