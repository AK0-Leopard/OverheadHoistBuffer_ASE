// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S2F41.cs" company="">
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

namespace com.mirle.ibg3k0.sc.Data.SECS.ASE
{   
    /// <summary>
    /// CANCEL：Terminate all activity of a specific 
    /// TRANSFEREXT command based on Command ID while the command is in the
    /// QUEUED state. ABORT：Abort the specific 
    /// TRANSFEREXT command based on Command ID while the command is in Transferring state. 
    /// PAUSE：Put the SC into the PAUSING state. 
    /// RESUME：Put the SC into the AUTO state. 
    /// SCAN：To identify a carrier (If STK has on-board BCR/RFID). 
    /// PRIORITYUPDATE：Specify command ID to update the carrier move priority.
    /// PORTTYPECHG：Specify port ID to change the direction INSTALL：Install carrier 
    /// REMOVE：Remove carrier DISABLESHELF：Disable a shelf of the stocker. 
    /// ENABLESHELF：Enable a shelf of the stocker. 
    /// TRANSFEREXT：Move the specified Carrier ID to the specified destination. 
    /// </summary>
    /// <summary>
    /// Class S2F41.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S2F41 : SXFY
    {

        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
        public string RCMD;

        [SecsElement(Index = 2, ListSpreadOut = true)]
        public REPITEM[] REPITEMS;

        /// <summary>
        /// Initializes a new instance of the <see cref="S2F41"/> class.
        /// </summary>
        public S2F41()
        {
            StreamFunction = "S2F41";
            IsBaseType = true;
        }
        public class REPITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
            public string CPNAME;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 64)]
            public string CPVAL;
        }
    }

    public class S2F41_PriorityUpdate : SXFY
    {
        /// <summary>
        /// The RCMD
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
        public string RCMD;

        [SecsElement(Index = 2)]
        public REPITEM REPITEMS;

        /// <summary>
        /// Initializes a new instance of the <see cref="S2F41"/> class.
        /// </summary>
        public S2F41_PriorityUpdate()
        {
            StreamFunction = "S2F41";
            IsBaseType = true;
        }
        public class REPITEM : SXFY
        {
            [SecsElement(Index = 1)]
            public REPITEM_ASCII CommandID_CP;
            [SecsElement(Index = 2)]
            public REPITEM_U2 PRIORITY_CP;
        }
    }

    public class S2F41_Scan : SXFY
    {
        /// <summary>
        /// The RCMD
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 20)]
        public string RCMD;

        [SecsElement(Index = 2)]
        public REPITEM REPITEMS;

        /// <summary>
        /// Initializes a new instance of the <see cref="S2F41"/> class.
        /// </summary>
        public S2F41_Scan()
        {
            StreamFunction = "S2F41";
            IsBaseType = true;
        }
        public class REPITEM : SXFY
        {
            [SecsElement(Index = 1)]
            public REPITEM_ASCII CARRIERID;
            [SecsElement(Index = 2)]
            public REPITEM_ASCII BOXID;
            [SecsElement(Index = 3)]
            public REPITEM_ASCII CARRIERLOC;
        }
    }

    public class REPITEM_ASCII : SXFY
    {
        [SecsElement(Index = 1, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
        public string CPNAME;
        [SecsElement(Index = 2, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
        public string CPVAL_ASCII;
    }

    public class REPITEM_U2 : SXFY
    {
        [SecsElement(Index = 1, ListElementType = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
        public string CPNAME;
        [SecsElement(Index = 2, ListElementType = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
        public string CPVAL_U2;
    }
}
