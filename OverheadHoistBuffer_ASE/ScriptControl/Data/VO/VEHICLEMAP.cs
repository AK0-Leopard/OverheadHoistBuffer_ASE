﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.VO
{
    public enum ServiceSide
    {
        None,
        North,
        South
    }

    public class VEHICLEMAP
    {

        public string ID;
        public string REAL_ID;
        public int Num;
        public ServiceSide Side;
    }
}
