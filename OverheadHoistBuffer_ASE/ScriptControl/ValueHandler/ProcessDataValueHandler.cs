//*********************************************************************************
//      ProcessDataValueHandler.cs
//*********************************************************************************
// File Name: ProcessDataValueHandler.cs
// Description: Type 1 EQ Process Data解析
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.bcf.Data;
using com.mirle.ibg3k0.sc.ConfigHandler;
using com.mirle.ibg3k0.sc.Data.VO;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.ValueHandler
{
    /// <summary>
    /// Class ProcessDataValueHandler.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.sc.ValueHandler.ValueHandlerBase" />
    public class ProcessDataValueHandler : ValueHandlerBase
    {
        /// <summary>
        /// The eqpt
        /// </summary>
        protected AEQPT eqpt;

        //Data
        /// <summary>
        /// Gets the operator_ identifier.
        /// </summary>
        /// <value>The operator_ identifier.</value>
        public string Operator_ID { get; private set; }
        /// <summary>
        /// Gets the lot_ identifier.
        /// </summary>
        /// <value>The lot_ identifier.</value>
        public string Lot_ID { get; private set; }
        /// <summary>
        /// Gets the prod_ identifier.
        /// </summary>
        /// <value>The prod_ identifier.</value>
        public string Prod_ID { get; private set; }
        /// <summary>
        /// Gets the ppid.
        /// </summary>
        /// <value>The ppid.</value>
        public string PPID { get; private set; }
        /// <summary>
        /// Gets the glass_ identifier.
        /// </summary>
        /// <value>The glass_ identifier.</value>
        public string Glass_ID { get; private set; }
        //Data Item
        /// <summary>
        /// Gets the data item list.
        /// </summary>
        /// <value>The data item list.</value>
        public List<KeyValuePair<string, string>> DataItemList { get; private set; }

        /// <summary>
        /// The proc data log
        /// </summary>
        protected Logger procDataLog = null;
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessDataValueHandler"/> class.
        /// </summary>
        /// <param name="vr">The vr.</param>
        /// <param name="eq">The eq.</param>
        /// <param name="operator_id">The operator_id.</param>
        /// <param name="lot_id">The lot_id.</param>
        /// <param name="prod_id">The prod_id.</param>
        /// <param name="ppid">The ppid.</param>
        /// <param name="glass_id">The glass_id.</param>
        public ProcessDataValueHandler(ValueRead vr, AEQPT eq, 
            string operator_id, string lot_id, string prod_id, string ppid, string glass_id)
            : base(vr)
        {
            this.eqpt = eq;
            procDataLog = LogManager.GetLogger(eqpt.EQPT_ID.Trim() + "_ProcData");
            DataItemList = new List<KeyValuePair<string, string>>();
            this.Operator_ID = operator_id;
            this.Lot_ID = lot_id;
            this.Prod_ID = prod_id;
            this.PPID = ppid;
            this.Glass_ID = glass_id;

            init();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void init()
        {
            try
            {
                UInt16[] dataItem = (UInt16[])vr.getText();
                analyzeProcessDataItem(dataItem);
            }
            catch (Exception ex) 
            {
                procDataLog.Warn(string.Format("analyzeProcessDataItem Exception.[Exception:{0}]", ex));
            }
        }

        /// <summary>
        /// Analyzes the process data item.
        /// </summary>
        /// <param name="dataItem">The data item.</param>
        private void analyzeProcessDataItem(UInt16[] dataItem)
        {
            ProcessDataConfigHandler handler = eqpt.getProcessDataConfigHandler();
            if (handler == null)
            {
                return;
            }
            List<ProcessDataType> procDataTypeList = handler.ProcDataTypeList;
            if (procDataTypeList == null)
            {
                return;
            }
            int beginIndex = 0;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine(string.Format("[Name:Lot ID][Value:{0}]", Lot_ID));
            sb.AppendLine(string.Format("[Name:Operator ID][Value:{0}]", Operator_ID));
            sb.AppendLine(string.Format("[Name:PPID][Value:{0}]", PPID));
            sb.AppendLine(string.Format("[Name:Prod ID][Value:{0}]", Prod_ID));
            sb.AppendLine(string.Format("[Name:Glass ID][Value:{0}]", Glass_ID));
            foreach (ProcessDataType dataType in procDataTypeList) 
            {
                string name = dataType.Name;
                //A0.01 Begin
                if (String.Compare(name.Trim(), ProcessDataConfigHandler.SPARE_FORMAT.Trim(), true) == 0) 
                {
                    beginIndex = beginIndex + 1;
                    continue;
                }
                //A0.01 End
                int wordCount = dataType.WordCount;
                int[] dataAry = new int[wordCount];
                string valueStr = string.Empty;
                try
                {
                    Array.Copy(dataItem, beginIndex, dataAry, 0, wordCount);
                    beginIndex = beginIndex + wordCount;
                    if (dataType.ItemType == ProcessDataType.DataItemType.BCD)
                    {
//                        int trussCount = dataType.TrussCount;
                        double multiplier = dataType.Multiplier;
                        bool isContainSign = dataType.IsContainSign;
                        BCD_Common bcd = BCD_Common.toBCD_Common(dataAry, wordCount, multiplier, isContainSign);
                        double realValue = bcd.RealValue;
                        valueStr = Convert.ToString(realValue);
                        sb.AppendLine(string.Format("[Name:{0}][Value:{1}]", name, realValue));
                    }
                    else if (dataType.ItemType == ProcessDataType.DataItemType.ASCII)
                    {
                        string realStr = BCFUtility.convertIntAry2String(dataAry);
                        valueStr = realStr;
                        sb.AppendLine(string.Format("[Name:{0}][Value:{1}]", name, realStr));
                    }
                    else if (dataType.ItemType == ProcessDataType.DataItemType.INT) 
                    {
                        double multiplier = dataType.Multiplier;
                        if (dataType.IsContainSign)
                        {
                            double doubleVal = 0;
                            if (dataAry.Length > 2) 
                            {
                                //3個word以上，使用Int64
                                //64 bit(4 word)
                                Int64 val = (Int64)BCFUtility.convertInt2TextByType(16, typeof(System.Int64), dataAry);
                                doubleVal = val * multiplier;
                            }
                            else if (dataAry.Length > 1)
                            {
                                //2個word，使用Int32
                                //32 bit(2 word)
                                Int32 val = (Int32)BCFUtility.convertInt2TextByType(16, typeof(System.Int32), dataAry);
                                doubleVal = val * multiplier;
                            }
                            else 
                            {
                                //1個word，使用Int16
                                //16 bit(1 word)
                                Int16 val = (Int16)BCFUtility.convertInt2TextByType(16, typeof(System.Int16), dataAry);
                                doubleVal = val * multiplier;
                            }
//                            Int16 val = (Int16)BCFUtility.convertInt2TextByType(16, typeof(System.Int16), dataAry);
//                            double doubleVal = val * multiplier;
                            valueStr = Convert.ToString(doubleVal);
                        }
                        else 
                        {
                            double doubleVal = 0;
                            if (dataAry.Length > 2)
                            {
                                //3個word以上，使用UInt64
                                //64 bit(4 word)
                                UInt64 val = (UInt64)BCFUtility.convertInt2TextByType(16, typeof(System.UInt64), dataAry);
                                doubleVal = val * multiplier;
                            }
                            else if (dataAry.Length > 1)
                            {
                                //2個word，使用UInt32
                                //32 bit(2 word)
                                UInt32 val = (UInt32)BCFUtility.convertInt2TextByType(16, typeof(System.UInt32), dataAry);
                                doubleVal = val * multiplier;
                            }
                            else
                            {
                                //1個word，使用UInt16
                                //16 bit(1 word)
                                UInt16 val = (UInt16)BCFUtility.convertInt2TextByType(16, typeof(System.UInt16), dataAry);
                                doubleVal = val * multiplier;
                            }
//                            UInt16 val = (UInt16)BCFUtility.convertInt2TextByType(16, typeof(System.UInt16), dataAry);
//                            double doubleVal = val * multiplier;
                            valueStr = Convert.ToString(doubleVal);
                        }
                        sb.AppendLine(string.Format("[Name:{0}][Value:{1}]", name, valueStr));
                    }
                }
                catch (Exception ex)
                {
                    sb.AppendLine(string.Format("[Name:{0}][Exception:{1}]", name, ex));
                }
                DataItemList.Add(new KeyValuePair<string, string>(name, valueStr));
            }
            procDataLog.Info(sb.ToString());
        }

    }
}
