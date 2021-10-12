using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.ObjectRelay
{
    public class ALARMObjToShow
    {
        public VALARM alarm_obj = null;
        public ALARMObjToShow(VALARM alarmObj)
        {
            alarm_obj = alarmObj;
        }
        [Description("Code")]
        public string ALAM_CODE { get { return alarm_obj.ALAM_CODE; } }
        [Description("Device ID")]
        public string EQPT_ID { get { return alarm_obj.EQPT_ID; } }
        [Description("State")]
        public ErrorStatus ALAM_STAT { get { return (ErrorStatus)alarm_obj.ALAM_STAT; } }
        [Description("Level")]
        public E_ALARM_LVL ALAM_LVL { get { return (E_ALARM_LVL)alarm_obj.ALAM_LVL; } }
        [Description("Happend time")]
        public System.DateTime RPT_DATE_TIME
        {
            get
            {
                DateTime dateTime = DateTime.MinValue;
                DateTime.TryParseExact(alarm_obj.RPT_DATE_TIME, App.SCAppConstants.TimestampFormat_19, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);

                return dateTime;
            }
        }
        [Description("Clear time")]
        public Nullable<System.DateTime> CLEAR_DATE_TIME
        {
            get
            {
                bool is_success = DateTime.TryParseExact(alarm_obj.RPT_DATE_TIME, App.SCAppConstants.TimestampFormat_19, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime);
                if (is_success)
                    return dateTime;
                else
                    return null;
            }
        }
        [Description("Description")]
        public string ALAM_DESC { get { return sc.Common.SCUtility.Trim(alarm_obj.ALAM_DESC, true); } }
        [Description("BOX ID")]
        public string BOX_ID { get { return alarm_obj.BOX_ID; } }
        [Description("CST ID")]
        public string CST_ID { get { return alarm_obj.CARRIER_ID; } }
        [Description("LOT ID")]
        public string LOT_ID { get { return alarm_obj.LOT_ID; } }
    }
}
