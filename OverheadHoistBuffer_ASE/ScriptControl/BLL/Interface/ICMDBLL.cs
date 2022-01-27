using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using com.mirle.ibg3k0.sc.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.BLL.Interface
{
    public interface ICMDBLL
    {
        bool hasExcuteCMDMCSByDestPort(string portID);
        bool isCMD_OHTCWillSending(string vhID);
        bool updateCMD_MCS_TranStatus(string cMD_ID, E_TRAN_STATUS transferring);
        bool creatCommand_OHTC(ACMD_OHTC cmd_ohtc);
        bool updateCMD_MCS_Dest(string cMD_ID, string hOSTDESTINATION);
    }
}
