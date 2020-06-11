//*********************************************************************************
//      InProcLotList.cs
//*********************************************************************************
// File Name: InProcLotList.cs
// Description: 
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2014/04/10    Miles Chen     N/A            N/A     Initial Release
//
//**********************************************************************************
using com.mirle.ibg3k0.bcf.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.VO
{
    /// <summary>
    /// Class InProcLotList.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Common.PropertyChangedVO" />
    public class InProcLotList : PropertyChangedVO
    {
        /// <summary>
        /// The lot list
        /// </summary>
        private List<ALOT> lotList = new List<ALOT>();
        /// <summary>
        /// Gets the lot list.
        /// </summary>
        /// <value>The lot list.</value>
        public virtual List<ALOT> LotList { get { return lotList; } }

        /// <summary>
        /// The lot lock
        /// </summary>
        private Object lotLock = new Object();
        /// <summary>
        /// Resets the lot list.
        /// </summary>
        /// <param name="dbLotList">The database lot list.</param>
        public virtual void resetLotList(List<ALOT> dbLotList)
        {
            lock (lotLock)
            {
                lotList.Clear();
                lotList.AddRange(dbLotList);
                OnPropertyChanged(BCFUtility.getPropertyName(() => this.LotList));
            }
        }

        /// <summary>
        /// Adds the lot.
        /// </summary>
        /// <param name="lot">The lot.</param>
        public virtual void addLot(ALOT lot)
        {
            lotList.Add(lot);
            OnPropertyChanged(BCFUtility.getPropertyName(() => this.lotList));
        }

        /// <summary>
        /// Removes the lot by identifier.
        /// </summary>
        /// <param name="lot_id">The lot_id.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool removeLotByID(string lot_id)
        {
            ALOT lot = lotList.Where(l => l.LOT_ID.Trim() == lot_id.Trim()).SingleOrDefault();
            if (lot == null)
            {
                return false;
            }
            lotList.Remove(lot);
            OnPropertyChanged(BCFUtility.getPropertyName(() => this.lotList));
            return true;
        }

        /// <summary>
        /// Upadtes the lot.
        /// </summary>
        /// <param name="inputLot">The input lot.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool upadteLot(ALOT inputLot)
        {
            ALOT lot = lotList.Where(l => l.LOT_ID.Trim() == inputLot.LOT_ID.Trim()).SingleOrDefault();
            if (lot == null)
            {
                return false;
            }
            lotList.Remove(lot);
            lotList.Add(inputLot);
            OnPropertyChanged(BCFUtility.getPropertyName(() => this.lotList));
            return true;
        }

    }

    /// <summary>
    /// Interface IInProcLotList
    /// </summary>
    public interface IInProcLotList
    {
        /// <summary>
        /// Adds the lot.
        /// </summary>
        /// <param name="lot">The lot.</param>
        void addLot(ALOT lot);
        /// <summary>
        /// Removes the lot by identifier.
        /// </summary>
        /// <param name="lot_id">The lot_id.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool removeLotByID(string lot_id);
        /// <summary>
        /// Upadtes the lot.
        /// </summary>
        /// <param name="inputLot">The input lot.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool upadteLot(ALOT inputLot);
        /// <summary>
        /// Resets the lot list.
        /// </summary>
        /// <param name="lotList">The lot list.</param>
        void resetLotList(List<ALOT> lotList);
    }
}
