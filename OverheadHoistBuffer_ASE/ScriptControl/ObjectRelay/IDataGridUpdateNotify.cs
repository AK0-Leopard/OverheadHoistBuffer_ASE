﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.ObjectRelay
{
    interface IDataGridUpdateNotify
    {
        void OnBeforeInsert();
        void OnBeforeUpdate();
    }
}
