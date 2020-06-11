using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using Google.Protobuf;
// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="Program.cs" company="">
//     Copyright ©  2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptControl
{
    /// <summary>
    /// Class Program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            //do nothing

            ID_31_TRANS_REQUEST TransferReq = new ID_31_TRANS_REQUEST
            {
                LoadAdr = "10111",
                ToAdr = "10211",
                CSTID = "CST01"
            };

            //TransferReq.GuideSections.Add("111");
            //TransferReq.GuideSegments.Add("222");


            testProtocol(TransferReq);

            byte[] arrayByte = new byte[TransferReq.CalculateSize()];
            TransferReq.WriteTo(new Google.Protobuf.CodedOutputStream(arrayByte));



            ID_31_TRANS_REQUEST TransferReqTemp = ID_31_TRANS_REQUEST.Parser.ParseFrom(arrayByte);

        }


        static void testProtocol(IMessage message)
        {
            byte[] arrayByte = new byte[message.CalculateSize()];
            message.WriteTo(new Google.Protobuf.CodedOutputStream(arrayByte));
        }
    }
}
