using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.SECS;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.Data.VO.Interface;
using com.mirle.ibg3k0.sc.ObjectRelay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc
{
    public partial class BLOCKZONEQUEUE
    {
        public string DisplayMember
        {
            get
            {
                string status_string = "";
                switch (STATUS)
                {
                    case SCAppConstants.BlockQueueState.Request:
                        status_string = "Request";
                        break;
                    case SCAppConstants.BlockQueueState.Blocking:
                        status_string = "Blocking";
                        break;
                    case SCAppConstants.BlockQueueState.Through:
                        status_string = "Through";
                        break;
                }
                return $"{ENTRY_SEC_ID}({status_string})";
            }
        }
    }

}
