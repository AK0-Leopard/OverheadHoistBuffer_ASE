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
    public partial class ANODE : BaseEQObject, IAlarmHisList
    {
        private AlarmHisList alarmHisList = new AlarmHisList();
        List<AEQPT> SubEqptList = null;
        public string SegmentLocation
        {
            get
            {
                string segment_location = "";
                if (SubEqptList != null && SubEqptList.Count > 0)
                {
                    if (SubEqptList[0].Type == SCAppConstants.EqptType.OHCV)
                    {
                        segment_location = (SubEqptList[0] as OHCV).SegmentLocation;
                    }
                }
                return segment_location;
            }
        }

        public List<AEQPT> getSubEqptList()
        {
            return SubEqptList;
        }

        private bool is_Alive;
        public bool Is_Alive
        {
            get { return is_Alive; }
            set
            {
                if (is_Alive != value)
                {
                    is_Alive = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.Is_Alive));
                }
            }
        }
        private bool safetyCheckComplete;
        public bool SafetyCheckComplete
        {
            get { return safetyCheckComplete; }
            set
            {
                if (safetyCheckComplete != value)
                {
                    safetyCheckComplete = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.SafetyCheckComplete));
                }
            }
        }
        private bool doorClosed;
        public bool DoorClosed
        {
            get { return doorClosed; }
            set
            {
                if (doorClosed != value)
                {
                    doorClosed = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.DoorClosed));
                }
            }
        }
        public override void doShareMemoryInit(BCFAppConstants.RUN_LEVEL runLevel)
        {
            foreach (IValueDefMapAction action in valueDefMapActionDic.Values)
            {
                action.doShareMemoryInit(runLevel);
            }
            //對sub eqpt進行初始化
            List<AEQPT> subEqptList = SCApplication.getInstance().getEQObjCacheManager().getEuipmentListByNode(NODE_ID);
            SubEqptList = subEqptList;
            if (subEqptList != null)
            {
                foreach (AEQPT eqpt in subEqptList)
                {
                    eqpt.doShareMemoryInit(runLevel);
                }
            }
            List<AVEHICLE> subVhList = SCApplication.getInstance().getEQObjCacheManager().getAllVehicle();
            if (subVhList != null)
            {
                foreach (AVEHICLE vh in subVhList)
                {
                    vh.doShareMemoryInit(runLevel);
                }
            }
        }

        public virtual void resetAlarmHis(List<ALARM> AlarmHisList)
        {
            alarmHisList.resetAlarmHis(AlarmHisList);
        }



    }

}
