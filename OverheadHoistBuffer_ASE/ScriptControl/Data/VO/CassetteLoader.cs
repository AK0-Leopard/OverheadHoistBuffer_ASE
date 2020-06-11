//*********************************************************************************
//      CassetteLoader.cs
//*********************************************************************************
// File Name: CassetteLoader.cs
// Description: Cassette Loader類別
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
using com.mirle.ibg3k0.sc.App;

namespace com.mirle.ibg3k0.sc.Data.VO
{
    /// <summary>
    /// Class CassetteLoader.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Common.PropertyChangedVO" />
    /// <seealso cref="com.mirle.ibg3k0.sc.Data.VO.ISheetList" />
    public class CassetteLoader : PropertyChangedVO, ISheetList
    {
        /// <summary>
        /// The cassette item
        /// </summary>
        private ACASSETTE cassetteItem = new ACASSETTE();
        /// <summary>
        /// Gets the cassette item.
        /// </summary>
        /// <value>The cassette item.</value>
        public virtual ACASSETTE CassetteItem
        {
            get { return cassetteItem; }
        }
        /// <summary>
        /// The lot item
        /// </summary>
        private ALOT lotItem = new ALOT();
        /// <summary>
        /// Gets the lot item.
        /// </summary>
        /// <value>The lot item.</value>
        public virtual ALOT LotItem
        {
            get { return lotItem; }
        }
        /// <summary>
        /// The sheet list
        /// </summary>
        private SheetList sheetList = new SheetList();
        /// <summary>
        /// Gets the sheet list.
        /// </summary>
        /// <value>The sheet list.</value>
        public virtual SheetList SheetList
        {
            get { return sheetList; }
        }

        /// <summary>
        /// Loads the cassette.
        /// </summary>
        /// <param name="cassette">The cassette.</param>
        public void loadCassette(ACASSETTE cassette)
        {
            BCFUtility.setValueToPropety(ref cassette, ref cassetteItem);
            OnPropertyChanged(BCFUtility.getPropertyName(() => this.CassetteItem));
        }

        /// <summary>
        /// Links the lot information.
        /// </summary>
        /// <param name="lot">The lot.</param>
        public void linkLotInfo(ALOT lot)
        {
            BCFUtility.setValueToPropety(ref lot, ref lotItem);
            OnPropertyChanged(BCFUtility.getPropertyName(() => this.LotItem));
        }

        /// <summary>
        /// Clears the lot information.
        /// </summary>
        public void clearLotInfo()
        {
            ALOT lot = new ALOT();
            BCFUtility.setValueToPropety(ref lot, ref lotItem);
            OnPropertyChanged(BCFUtility.getPropertyName(() => this.LotItem));
        }

        /// <summary>
        /// Unloads the cassette.
        /// </summary>
        public void unloadCassette()
        {

        }

        /// <summary>
        /// Clears all sheet.
        /// </summary>
        public void clearAllSheet()
        {
            sheetList.clearAllSheet();
            OnPropertyChanged(BCFUtility.getPropertyName(() => this.SheetList));
        }

        /// <summary>
        /// Removes the sheet by identifier.
        /// </summary>
        /// <param name="sheet_id">The sheet_id.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool removeSheetByID(string sheet_id)
        {
            Boolean rtnCode = sheetList.removeSheetByID(sheet_id);
            OnPropertyChanged(BCFUtility.getPropertyName(() => this.SheetList));
            return rtnCode;
        }

        /// <summary>
        /// Adds the sheets.
        /// </summary>
        /// <param name="sheets">The sheets.</param>
        public void addSheets(List<ASHEET> sheets)
        {
            sheetList.addSheets(sheets);
            OnPropertyChanged(BCFUtility.getPropertyName(() => this.SheetList));
        }

        /// <summary>
        /// Adds the sheet.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        public void addSheet(ASHEET sheet)
        {
            sheetList.addSheet(sheet);
            OnPropertyChanged(BCFUtility.getPropertyName(() => this.SheetList));
        }

    }

    /// <summary>
    /// Interface ICassetteLoader
    /// </summary>
    public interface ICassetteLoader
    {
        /// <summary>
        /// Loads the cassette.
        /// </summary>
        /// <param name="cassette">The cassette.</param>
        void loadCassette(ACASSETTE cassette);
        /// <summary>
        /// Unloads the cassette.
        /// </summary>
        void unloadCassette();
    }

}
