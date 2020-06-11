using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.SECS;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.Data.VO.Interface;
using com.mirle.ibg3k0.sc.ObjectRelay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc
{
    public partial class HASHEET
    {
       
        public HASHEET(ASHEET sht, String TimeStamp)
        {
            T_STAMP = TimeStamp;
            SHT_ID = sht.SHT_ID;
            JOB_NO = sht.JOB_NO;
            LOT_ID = sht.LOT_ID;
            SHT_STAT = sht.SHT_STAT;
            PROD_ID = sht.PROD_ID;
            SOURCE_CST_ID = sht.SOURCE_CST_ID;
            SOURCE_SLOT_NO = sht.SOURCE_SLOT_NO;
            SOURCE_PORT_NO = sht.SOURCE_PORT_NO;
            TARGET_CST_ID = sht.TARGET_CST_ID;
            TARGET_SLOT_NO = sht.TARGET_SLOT_NO;
            TARGET_PORT_NO = sht.TARGET_PORT_NO;
            PROC_FLAG = sht.PROC_FLAG;
            SHT_JUDGE = sht.SHT_JUDGE;
            SHT_GRADE = sht.SHT_GRADE;
            PROC_INFO = sht.PROC_INFO;
            PPID = sht.PPID;
            CRATE_ID = sht.CRATE_ID;
            TAKE_OUT_STAT = sht.TAKE_OUT_STAT;
            NODE_ID = sht.NODE_ID;
            CST_ID = sht.CST_ID;
            SLOT_NO = sht.SLOT_NO;
            TAKE_OUT_TIME = sht.TAKE_OUT_TIME;
            SCRAP_CODE = sht.SCRAP_CODE;
            REASON_CODE = sht.REASON_CODE;
        }
        public HASHEET(ASHEET sht)
            : this(sht, BCFUtility.formatDateTime(DateTime.Now, SCAppConstants.TimestampFormat_19))
        {
               
        }
    }

}
