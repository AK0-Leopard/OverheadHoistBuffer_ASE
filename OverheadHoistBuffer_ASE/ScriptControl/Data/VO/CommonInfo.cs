//*********************************************************************************
//      CommonInfo.cs
//*********************************************************************************
// File Name: CommonInfo.cs
// Description: CCommonInfo類別
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using com.mirle.ibg3k0.bcf.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using System.Data;
using System.ComponentModel;
using com.mirle.ibg3k0.sc.ObjectRelay;

namespace com.mirle.ibg3k0.sc.Data.VO
{
    /// <summary>
    /// Class CommonInfo.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Common.PropertyChangedVO" />
    /// <seealso cref="com.mirle.ibg3k0.sc.Data.VO.IAlarmHisList" />
    /// <seealso cref="com.mirle.ibg3k0.sc.Data.VO.IInProcLotList" />
    public class CommonInfo : PropertyChangedVO, IAlarmHisList, IInProcLotList
    {
        #region MPC Tip Message
        /// <summary>
        /// The ma x_ mp c_ ti p_ ms g_ count
        /// </summary>
        private readonly static int MAX_MPC_TIP_MSG_COUNT = 50;
        /// <summary>
        /// The MPC tip MSG list
        /// </summary>
        private List<MPCTipMessage> mpcTipMsgList = new List<MPCTipMessage>();
        /// <summary>
        /// Gets the MPC tip MSG list.
        /// </summary>
        /// <value>The MPC tip MSG list.</value>
        public List<MPCTipMessage> MPCTipMsgList
        {
            get
            {
                return mpcTipMsgList;
            }
        }
        /// <summary>
        /// The MPC tip MSG data table
        /// </summary>
        private DataTable mpcTipMsgDataTable = null;
        /// <summary>
        /// Initializes the MPC tip MSG data table.
        /// </summary>
        private void initMPCTipMsgDataTable()
        {
            mpcTipMsgDataTable = new DataTable("MPCTipMSG");
            mpcTipMsgDataTable.Columns.Add("Time", typeof(string));
            mpcTipMsgDataTable.Columns.Add("MsgSource", typeof(string));
            mpcTipMsgDataTable.Columns.Add("MsgLevel", typeof(string));
            mpcTipMsgDataTable.Columns.Add("Msg", typeof(string));
        }

        /// <summary>
        /// Adds the MPC tip MSG.
        /// </summary>
        /// <param name="mpcTipMsg">The MPC tip MSG.</param>
        public void addMPCTipMsg(MPCTipMessage mpcTipMsg)
        {
            lock (mpcTipMsgList)
            {
                if (mpcTipMsgList.Count > MAX_MPC_TIP_MSG_COUNT)
                {
                    mpcTipMsgList.RemoveAt(mpcTipMsgList.Count - 1);
                }
                mpcTipMsgList.Insert(0, mpcTipMsg);
                OnPropertyChanged(BCFUtility.getPropertyName(() => this.MPCTipMsgList));
            }
        }
        #endregion

        /// <summary>
        /// The alarm his list
        /// </summary>
        private AlarmHisList alarmHisList = new AlarmHisList();
        /// <summary>
        /// Gets the alarm his list.
        /// </summary>
        /// <value>The alarm his list.</value>
        public AlarmHisList AlarmHisList { get { return alarmHisList; } }

        /// <summary>
        /// The in proc lot list
        /// </summary>
        private InProcLotList inProcLotList = new InProcLotList();
        /// <summary>
        /// Gets the in proc lot list.
        /// </summary>
        /// <value>The in proc lot list.</value>
        public InProcLotList InProcLotList { get { return inProcLotList; } }

        public BindingList<VehicleObjToShow> ObjectToShow_list = new BindingList<VehicleObjToShow>();

        /// <summary>
        /// Resets the lot list.
        /// </summary>
        /// <param name="lotList">The lot list.</param>
        public void resetLotList(List<ALOT> lotList)
        {
            inProcLotList.resetLotList(lotList);
        }

        /// <summary>
        /// Adds the lot.
        /// </summary>
        /// <param name="lot">The lot.</param>
        public void addLot(ALOT lot)
        {
            InProcLotList.addLot(lot);
        }

        /// <summary>
        /// Removes the lot by identifier.
        /// </summary>
        /// <param name="lot_id">The lot_id.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool removeLotByID(string lot_id)
        {
            return InProcLotList.removeLotByID(lot_id);
        }

        /// <summary>
        /// Upadtes the lot.
        /// </summary>
        /// <param name="inputLot">The input lot.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool upadteLot(ALOT inputLot)
        {
            return InProcLotList.upadteLot(inputLot);
        }

        /// <summary>
        /// Resets the alarm his.
        /// </summary>
        /// <param name="alarmList">The alarm list.</param>
        public void resetAlarmHis(List<ALARM> alarmList)
        {
            alarmHisList.resetAlarmHis(alarmList);
        }

        /// <summary>
        /// The secs_msg
        /// </summary>
        private string secs_msg;
        /// <summary>
        /// Gets or sets the sec s_ MSG.
        /// </summary>
        /// <value>The sec s_ MSG.</value>
        public virtual string SECS_Msg
        {
            get { return secs_msg; }
            set
            {
                if (!BCFUtility.isMatche(secs_msg, value))
                {
                    secs_msg = value;
                    OnPropertyChanged(BCFUtility.getPropertyName(() => this.SECS_Msg));
                }
            }
        }

        public Dictionary<string, CommuncationInfo> dicCommunactionInfo = null;
    }


    /// <summary>
    /// Class MPCTipMessage.
    /// </summary>
    //public class MPCTipMessage
    //{
    //    //訊息時間
    //    /// <summary>
    //    /// Gets or sets the time.
    //    /// </summary>
    //    /// <value>The time.</value>
    //    public String Time { set; get; }

    //    //訊息等級
    //    /// <summary>
    //    /// Gets or sets the MSG level.
    //    /// </summary>
    //    /// <value>The MSG level.</value>
    //    public String MsgLevel { get; set; }
    //    //訊息
    //    /// <summary>
    //    /// Gets or sets the MSG.
    //    /// </summary>
    //    /// <value>The MSG.</value>
    //    public String Msg { get; set; }

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="MPCTipMessage"/> class.
    //    /// </summary>
    //    public MPCTipMessage()
    //    {
    //        Time = BCFUtility.formatDateTime(DateTime.Now, SCAppConstants.DateTimeFormat_22);
    //    }
    //}
    public class MPCTipMessage
    {
        //訊息時間
        public String Time { set; get; }
        //訊息等級
        public ProtocolFormat.OHTMessage.MsgLevel MsgLevel { get; set; }
        //訊息
        public String Msg { get; set; }
        //訊息
        public String XID { get; set; }
        public bool IsConfirm { get; set; }
        public MPCTipMessage()
        {
            Time = BCFUtility.formatDateTime(DateTime.Now, SCAppConstants.DateTimeFormat_22);
        }



    }
}
