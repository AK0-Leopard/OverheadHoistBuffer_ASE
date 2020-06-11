// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S1F4.cs" company="">
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

namespace com.mirle.ibg3k0.sc.Data.SECS.CSOT
{
    /// <summary>
    /// Class S1F4.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S1F4 : SXFY
    {
        [SecsElement(Index = 1, ListSpreadOut = true)]
        public SXFY[] SV;


        //public class VID006 : SXFY
        //{
        //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
        //    public string SCSTAT;
        //}
        //public class VID073 : SXFY
        //{
        //    [SecsElement(Index = 1, ListSpreadOut = true, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER, Length = 1)]
        //    public string SCSTAT;
        //}

        //public class VID053 : SXFY
        //{
        //    [SecsElement(Index = 1, ListSpreadOut = true)]
        //    public VID071[] VEHICLEINFO;
        //}


        //public class VID071 : SXFY
        //{
        //    [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII,
        //       Length = 32)]
        //    public string VEHICLE_ID;
        //    [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER,
        //       Length = 1)]
        //    public string VEHICLE_STATE;
        //}

        //public class VID074 : SXFY
        //{
        //    [SecsElement(Index = 1, ListSpreadOut = true)]
        //    public PORTINFO[] PORTINFOS;

        //    public class PORTINFO : SXFY
        //    {
        //        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII,
        //           Length = 64)]
        //        public string PORTID;
        //        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_2_BYTE_UNSIGNED_INTEGER,
        //           Length = 1)]
        //        public string PORTTRANSFERSTATE;
        //    }
        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="S1F4"/> class.
        /// </summary>
        public S1F4()
        {
            StreamFunction = "S1F4";
            W_Bit = 0;
        }
    }
}
