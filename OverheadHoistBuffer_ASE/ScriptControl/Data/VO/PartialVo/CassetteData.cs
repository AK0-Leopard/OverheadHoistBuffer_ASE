using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc
{
    public partial class CassetteData
    {
        public enum OHCV_STAGE
        {
            OHTtoPort = 0,  //入料進行中
            OP = 1,
            BP1,
            BP2,
            BP3,
            BP4,
            BP5,
            LP,
        }

        public CassetteData Clone()
        {
            return (CassetteData)this.MemberwiseClone();
        }

        public bool hasCommandExcute(BLL.CMDBLL cmdBLL)
        {
            bool has_cmd_excute = cmdBLL.hasExcuteCMDByBoxID(BOXID);
            return has_cmd_excute;
        }

        public string  getZoneID(sc.Service.TransferService transferService)
        {
            if (!transferService.isShelfPort(Carrier_LOC))
            {
                return "";
            }
            string zone_id = transferService.getShelfZoneID(Carrier_LOC);
            return zone_id;
        }
    }
}
