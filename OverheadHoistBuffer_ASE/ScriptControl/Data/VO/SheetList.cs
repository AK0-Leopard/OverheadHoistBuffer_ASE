//*********************************************************************************
//      SheetList.cs
//*********************************************************************************
// File Name: SheetList.cs
// Description: Sheet List類別
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc.Common;
using NLog;

namespace com.mirle.ibg3k0.sc.Data.VO
{
    /// <summary>
    /// Class SheetList.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Common.PropertyChangedVO" />
    public class SheetList : PropertyChangedVO
    {
        /// <summary>
        /// The sheet record list
        /// </summary>
        private List<ASHEET> sheetRecordList = new List<ASHEET>();
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets the sheet record list.
        /// </summary>
        /// <value>The sheet record list.</value>
        public virtual List<ASHEET> SheetRecordList 
        {
            get 
            {
                //return sheetRecordList.OrderBy(sr => sr.Lot_ID).ToList();
                return sheetRecordList.ToList();
            }
        }

        /// <summary>
        /// Adds the sheet.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        public virtual void addSheet(ASHEET sheet) 
        {
            sheetRecordList.Add(sheet);
            OnPropertyChanged(BCFUtility.getPropertyName(() => this.SheetRecordList));
        }

        /// <summary>
        /// Adds the sheets.
        /// </summary>
        /// <param name="sheets">The sheets.</param>
        public virtual void addSheets(List<ASHEET> sheets) 
        {
            sheetRecordList.AddRange(sheets);
            OnPropertyChanged(BCFUtility.getPropertyName(() => this.SheetRecordList));
        }

        /// <summary>
        /// Removes the sheet by identifier.
        /// </summary>
        /// <param name="sheet_id">The sheet_id.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool removeSheetByID(string sheet_id) 
        {
            ASHEET sheet = sheetRecordList.Where(s => s.SHT_ID.Trim() == sheet_id.Trim()).SingleOrDefault();
            if (sheet == null) 
            {
                return false;
            }
            sheetRecordList.Remove(sheet);
            OnPropertyChanged(BCFUtility.getPropertyName(() => this.SheetRecordList));
            return true;
        }

        /// <summary>
        /// Clears all sheet.
        /// </summary>
        public virtual void clearAllSheet() 
        {
            sheetRecordList.Clear();
            OnPropertyChanged(BCFUtility.getPropertyName(() => this.SheetRecordList));
        }

        /// <summary>
        /// Determines whether this instance is empty.
        /// </summary>
        /// <returns><c>true</c> if this instance is empty; otherwise, <c>false</c>.</returns>
        public virtual bool isEmpty() 
        {
            if (sheetRecordList.Count == 0) 
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the sheet by sheet identifier.
        /// </summary>
        /// <param name="sheetID">The sheet identifier.</param>
        /// <returns>Sheet.</returns>
        public virtual ASHEET getSheetBySheetID(String sheetID)
        {
            return sheetRecordList.Where(p => SCUtility.Trim(p.SHT_ID) == SCUtility.Trim(sheetID)).SingleOrDefault();
        }

        /// <summary>
        /// Determines whether [has sheet by sheet identifier] [the specified sheet identifier].
        /// </summary>
        /// <param name="sheetID">The sheet identifier.</param>
        /// <returns><c>true</c> if [has sheet by sheet identifier] [the specified sheet identifier]; otherwise, <c>false</c>.</returns>
        public virtual bool hasSheetBySheetID(String sheetID)
        {
            try
            {
                if (getSheetBySheetID(sheetID) != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex) 
            {
                logger.Error(ex, "Exception:");
                return false;
            }
        }

    }

    /// <summary>
    /// Interface ISheetList
    /// </summary>
    public interface ISheetList
    {
        /// <summary>
        /// Adds the sheet.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        void addSheet(ASHEET sheet);
        /// <summary>
        /// Adds the sheets.
        /// </summary>
        /// <param name="sheets">The sheets.</param>
        void addSheets(List<ASHEET> sheets);
        /// <summary>
        /// Removes the sheet by identifier.
        /// </summary>
        /// <param name="sheet_id">The sheet_id.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool removeSheetByID(string sheet_id);
        /// <summary>
        /// Clears all sheet.
        /// </summary>
        void clearAllSheet();
    }
}
