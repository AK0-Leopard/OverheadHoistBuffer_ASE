using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;
using com.mirle.ibg3k0.stc.Data.SecsData;

namespace com.mirle.ibg3k0.sc.Data.SECS
{
    public class S2F50 : SXFY
    {
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_BINARY, Length = 1)]
        public string HCACK;
        [SecsElement(Index = 2)]
        public REPITEM[] REPITEMS;

        public S2F50()
        {
            StreamFunction = "S2F50";
            StreamFunctionName = "Enhanced Remote Command Ack";
            W_Bit = 0;
        }

        public class REPITEM : SXFY
        {
            [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 40)]
            public string CPNAME;
            [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_BINARY, Length = 1)]
            public string CPACK;
        }
    }
}
