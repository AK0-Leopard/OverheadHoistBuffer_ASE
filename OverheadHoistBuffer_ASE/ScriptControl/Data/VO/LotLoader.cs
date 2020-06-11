//*********************************************************************************
//      LotLoader.cs
//*********************************************************************************
// File Name: LotLoader.cs
// Description: Lot Loader類別
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

namespace com.mirle.ibg3k0.sc.Data.VO
{
    /// <summary>
    /// Class LotLoader.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.bcf.Common.PropertyChangedVO" />
    public class LotLoader : PropertyChangedVO
    {
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
        /// Loads the lot.
        /// </summary>
        /// <param name="lot">The lot.</param>
        public virtual void loadLot(ALOT lot) 
        {
            linkLotInfo(lot);
        }

        /// <summary>
        /// Links the lot information.
        /// </summary>
        /// <param name="lot">The lot.</param>
        public void linkLotInfo(ALOT lot)
        {
            //lotItem = lot;
            BCFUtility.setValueToPropety(ref lot, ref lotItem);
            OnPropertyChanged(BCFUtility.getPropertyName(() => this.LotItem));
        }

        /// <summary>
        /// Unloads the lot.
        /// </summary>
        public void unloadLot()
        {
            ALOT lot = new ALOT();
            BCFUtility.setValueToPropety(ref lot, ref lotItem);
            OnPropertyChanged(BCFUtility.getPropertyName(() => this.LotItem));
        }

    }

    /// <summary>
    /// Interface ILotLoader
    /// </summary>
    public interface ILotLoader
    {
        /// <summary>
        /// Loads the lot.
        /// </summary>
        /// <param name="lot">The lot.</param>
        void loadLot(ALOT lot);
        /// <summary>
        /// Unloads the lot.
        /// </summary>
        void unloadLot();
        /// <summary>
        /// Links the lot information.
        /// </summary>
        /// <param name="lot">The lot.</param>
        void linkLotInfo(ALOT lot);
    }
}
