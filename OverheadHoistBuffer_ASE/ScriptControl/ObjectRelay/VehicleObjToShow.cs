using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.ObjectRelay
{
    public class VehicleObjToShow : INotifyPropertyChanged
    {
        double distance2millimeter = 1000;
        double km2millimeter = 1000000;


        //public static BindingList<VehicleObjToShow> ObjectToShow_list = new BindingList<VehicleObjToShow>();
        AVEHICLE vehicle = null;
        public VehicleObjToShow(AVEHICLE myDatabaseObject, double distance_scale)
        {
            this.vehicle = myDatabaseObject;
            distance2millimeter = distance_scale;
        }
        [DisplayName("Vh ID")]
        public string VEHICLE_ID
        {
            get { return vehicle.VEHICLE_ID; }
            set
            {
                this.vehicle.VEHICLE_ID = value;
                NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.VEHICLE_ID));
            }
        }
        //[DisplayName("Address ID")]
        //public string cUR_ADR_ID
        //{
        //    get { return vehicle.CUR_ADR_ID; }
        //    set
        //    {
        //        this.vehicle.CUR_ADR_ID = value;
        //        NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.cUR_ADR_ID));
        //    }
        //}
        //[DisplayName("Section ID")]
        //public string cUR_SEC_ID
        //{
        //    get { return vehicle.CUR_SEC_ID; }
        //    set
        //    {
        //        this.vehicle.CUR_SEC_ID = value;
        //        NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.cUR_SEC_ID));
        //    }
        //}



        [DisplayName("Mode")]
        public VHModeStatus MODE_STATUS
        {
            get { return vehicle.MODE_STATUS; }
            set
            {
                vehicle.MODE_STATUS = value;
                NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.MODE_STATUS));
            }
        }

        [DisplayName("Action")]
        public VHActionStatus ACT_STATUS
        {
            get { return vehicle.ACT_STATUS; }
            set
            {
                vehicle.ACT_STATUS = value;
                NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.ACT_STATUS));
            }
        }
        [DisplayName("MCS CMD")]
        public string MCS_CMD
        {
            get
            {
                return vehicle.MCS_CMD == null ?
                    string.Empty : vehicle.MCS_CMD.Trim();
            }
            set
            {
                vehicle.MCS_CMD = value;
                NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.MCS_CMD));
            }
        }
        [DisplayName("OHTC CMD")]
        public string OHTC_CMD
        {
            get
            {
                return vehicle.OHTC_CMD == null ?
                    string.Empty : vehicle.OHTC_CMD.Trim();
            }
            set
            {
                vehicle.OHTC_CMD = value;
                NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.OHTC_CMD));
            }
        }
        //[DisplayName("CST ID")]
        //public string cST_ID
        //{
        //    get { return vehicle.CST_ID; }
        //    set
        //    {
        //        vehicle.CST_ID = value;
        //        NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.cST_ID));
        //    }
        //}

        [DisplayName("Block pause")]
        public bool bLOCK_PAUSE2Show
        {
            get
            {
                return vehicle.BLOCK_PAUSE == VhStopSingle.StopSingleOn;
            }
        }
        [Browsable(false)]
        public VhStopSingle BLOCK_PAUSE
        {
            set
            {
                vehicle.BLOCK_PAUSE = value;
                NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.bLOCK_PAUSE2Show));
            }
        }

        [DisplayName("CMD pause")]
        public bool cMD_PAUSE2Show
        {
            get
            {
                return vehicle.CMD_PAUSE == VhStopSingle.StopSingleOn;
            }
        }

        [Browsable(false)]
        public VhStopSingle CMD_PAUSE
        {
            set
            {
                vehicle.CMD_PAUSE = value;
                NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.cMD_PAUSE2Show));
            }
        }

        [DisplayName("OBS pause")]
        public bool oBS_PAUSE2Show
        {
            get
            {
                return vehicle.OBS_PAUSE == VhStopSingle.StopSingleOn;
            }
        }
        [Browsable(false)]
        public VhStopSingle OBS_PAUSE
        {
            set
            {
                vehicle.OBS_PAUSE = value;
                NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.oBS_PAUSE2Show));
            }
        }

        [DisplayName("HID pause")]
        public bool hID_PAUSE2Show
        {
            get
            {
                return vehicle.HID_PAUSE == VhStopSingle.StopSingleOn;
            }
        }
        [Browsable(false)]
        public VhStopSingle HID_PAUSE
        {
            set
            {
                vehicle.HID_PAUSE = value;
                NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.hID_PAUSE2Show));
            }
        }


        [DisplayName("OBS DIST(m)")]
        public double oBS_DIST2Show
        {
            get
            {
                return Math.Round((vehicle.OBS_DIST / distance2millimeter), 2, MidpointRounding.AwayFromZero);
            }
        }

        [Browsable(false)]
        public int OBS_DIST
        {
            get { return (int)(vehicle.OBS_DIST / distance2millimeter); }
            set
            {
                vehicle.OBS_DIST = value;
                NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.oBS_DIST2Show));
            }
        }
        //[DisplayName("Has CST")]
        //public int hAS_CST
        //{
        //    get { return vehicle.HAS_CST; }
        //    set
        //    {
        //        vehicle.HAS_CST = value;
        //        NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.hAS_CST));
        //    }
        //}

        [DisplayName("ODO(km)")]
        public double vEHICLE_ACC_DIST2Show
        {
            get
            {
                return Math.Round((vehicle.VEHICLE_ACC_DIST / km2millimeter), 2, MidpointRounding.AwayFromZero);
            }
        }
        //[DisplayName("ODO(km)")]
        [Browsable(false)]
        public int VEHICLE_ACC_DIST
        {
            get
            {
                return (int)(vehicle.VEHICLE_ACC_DIST / km2millimeter);
            }
            set
            {
                vehicle.VEHICLE_ACC_DIST = value;
                NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.vEHICLE_ACC_DIST2Show));
            }
        }
        //[DisplayName("Maintain ODO")]
        //public int mANT_ACC_DIST
        //{
        //    get { return vehicle.MANT_ACC_DIST; }
        //    set
        //    {
        //        vehicle.MANT_ACC_DIST = value;
        //        NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.mANT_ACC_DIST));
        //    }
        //}
        //[DisplayName("Last Maintain Date")]
        //public DateTime? mANT_DATE
        //{
        //    get { return vehicle.MANT_DATE ?? DateTime.MinValue; }
        //    set
        //    {
        //        vehicle.MANT_DATE = value;
        //        NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.mANT_DATE));
        //    }
        //}
        //[DisplayName("GRIP count")]
        //public int gRIP_COUNT
        //{
        //    get { return vehicle.GRIP_COUNT; }
        //    set
        //    {
        //        vehicle.GRIP_COUNT = value;
        //        NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.gRIP_COUNT));
        //    }
        //}
        //[DisplayName("GRIP Mant count")]
        //public int gRIP_MANT_COUNT
        //{
        //    get { return vehicle.GRIP_MANT_COUNT; }
        //    set
        //    {
        //        vehicle.GRIP_MANT_COUNT = value;
        //        NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.gRIP_MANT_COUNT));
        //    }
        //}
        //[DisplayName("Last GRIP Mant date")]
        //public DateTime? gRIP_MANT_DATE
        //{
        //    get { return vehicle.GRIP_MANT_DATE ?? DateTime.MinValue; }
        //    set
        //    {
        //        vehicle.GRIP_MANT_DATE = value;
        //        NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.gRIP_MANT_DATE));
        //    }
        //}
        //[DisplayName("Mode Adr")]
        //public string nODE_ADR
        //{
        //    get { return vehicle.NODE_ADR; }
        //    set
        //    {
        //        vehicle.NODE_ADR = value;
        //        NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.nODE_ADR));
        //    }
        //}
        [DisplayName("Parking")]
        public bool IS_PARKING
        {
            get { return vehicle.IS_PARKING; }
            set
            {
                vehicle.IS_PARKING = value;
                NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.IS_PARKING));
            }
        }
        [DisplayName("Park time")]
        public DateTime? PARK_TIME
        {
            get { return vehicle.PARK_TIME ?? DateTime.MinValue; }
            set
            {
                vehicle.PARK_TIME = value;
                NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.PARK_TIME));
            }
        }

        [DisplayName("Cycling")]
        public bool IS_CYCLING
        {
            get { return vehicle.IS_CYCLING; }
            set
            {
                vehicle.IS_CYCLING = value;
                NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.IS_CYCLING));
            }
        }
        [DisplayName("Cycling time")]
        public DateTime? CYCLERUN_TIME
        {
            get { return vehicle.CYCLERUN_TIME ?? DateTime.MinValue; }
            set
            {
                vehicle.CYCLERUN_TIME = value;
                NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.CYCLERUN_TIME));
            }
        }

        [DisplayName("Sec DIST(m)")]
        public double ACC_SEC_DIST2Show
        {
            get
            {
                return Math.Round((vehicle.ACC_SEC_DIST / distance2millimeter), 2, MidpointRounding.AwayFromZero);
            }
        }
        [Browsable(false)]
        public double ACC_SEC_DIST
        {
            get { return (vehicle.ACC_SEC_DIST / distance2millimeter); }
            set
            {
                this.vehicle.ACC_SEC_DIST = value;
                NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.ACC_SEC_DIST2Show));
            }
        }
        [DisplayName("UPD_TIME")]
        public DateTime? UPD_TIME
        {
            get { return vehicle.UPD_TIME ?? DateTime.MinValue; }
            set
            {
                vehicle.UPD_TIME = value;
                NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.UPD_TIME));
            }
        }
        //[DisplayName("Cycle Zone ID")]
        //public string cYCLERUN_ID
        //{
        //    get { return vehicle.CYCLERUN_ID; }
        //    set
        //    {
        //        vehicle.CYCLERUN_ID = value;
        //        NotifyPropertyChanged(BCFUtility.getPropertyName(() => this.cYCLERUN_ID));
        //    }
        //}
        //This is to notify the changes made to the object directly and not from the control. This refreshes the datagridview.
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(String propertyName)
        {
            try
            {
                if (PropertyChanged != null)
                {
                    Adapter.Invoke((obj) =>
                    {
                        {
                            PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
                        }
                    }, null);
                }
            }
            catch
            {

            }
        }
    }
}
