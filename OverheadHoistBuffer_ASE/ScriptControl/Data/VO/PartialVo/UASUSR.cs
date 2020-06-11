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
    public partial class UASUSR
    {

        public virtual Boolean isDisable()
        {
            if (BCFUtility.isMatche(DISABLE_FLG, SCAppConstants.YES_FLAG))
            {
                return true;
            }
            return false;
        }

        public virtual Boolean isAdmin()
        {
            if (BCFUtility.isMatche(ADMIN_FLG, SCAppConstants.YES_FLAG))
            {
                return true;
            }
            return false;
        }

        public virtual Boolean isPowerUser()
        {
            if (BCFUtility.isMatche(POWER_USER_FLG, SCAppConstants.YES_FLAG))
            {
                return true;
            }
            return isAdmin();
        }
    }

}
