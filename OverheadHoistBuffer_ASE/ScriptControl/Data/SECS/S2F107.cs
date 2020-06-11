// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S2F107.cs" company="">
//     Copyright ©  2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;
using com.mirle.ibg3k0.stc.Data.SecsData;

namespace com.mirle.ibg3k0.sc.Data.SECS
{
    /// <summary>
    /// Sorter Job Command (H -&gt; E)
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S2F107 : SXFY
    {
        /// <summary>
        /// Sorter Job Command
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
        public string SORTERJOBID;
        /// <summary>
        /// a sort lot item
        /// </summary>
        [SecsElement(Index = 2)]
        public SortJobLotItem[] aSortLotItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="S2F107"/> class.
        /// </summary>
        public S2F107() 
        {
            StreamFunction = "S2F107";
            W_Bit = 1;
        }

        /// <summary>
        /// Class SortJobLotItem.
        /// </summary>
        /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
        public class SortJobLotItem : SXFY
        {
            /// <summary>
            /// Sort LOT  Item
            /// </summary>
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string LOTID;
            /// <summary>
            /// The ptid
            /// </summary>
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string PTID;
            /// <summary>
            /// The cstid
            /// </summary>
            [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string CSTID;
            /// <summary>
            /// a sort glass item
            /// </summary>
            [SecsElement(Index = 4)]
            public SortJobGlassItem[] aSortGlassItem;

            /// <summary>
            /// Initializes a new instance of the <see cref="SortJobLotItem"/> class.
            /// </summary>
            public SortJobLotItem()
            {
                StreamFunction = "LOTID Count";
                W_Bit = 0;
            }
        }
        /// <summary>
        /// Class SortJobGlassItem.
        /// </summary>
        /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
                public class SortJobGlassItem : SXFY
        {
            /// <summary>
            /// Sort Glass Item
            /// </summary>
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string GLSID;
            /// <summary>
            /// The fslotno
            /// </summary>
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string FSLOTNO;
            /// <summary>
            /// The tptid
            /// </summary>
            [SecsElement(Index = 3, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string TPTID;
            /// <summary>
            /// The tcstid
            /// </summary>
            [SecsElement(Index = 4, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
            public string TCSTID;
            /// <summary>
            /// The tslotno
            /// </summary>
            [SecsElement(Index = 5, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 3)]
            public string TSLOTNO;
            /// <summary>
            /// The sortturnflag
            /// </summary>
            [SecsElement(Index = 6, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string SORTTURNFLAG;
            /// <summary>
            /// The sortscrapflag
            /// </summary>
            [SecsElement(Index = 7, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 1)]
            public string SORTSCRAPFLAG;


            /// <summary>
            /// Initializes a new instance of the <see cref="SortJobGlassItem"/> class.
            /// </summary>
            public SortJobGlassItem()
            {
                StreamFunction = "Sort Job Glass Item Count";
                W_Bit = 0;
            }
        }
    }
}
