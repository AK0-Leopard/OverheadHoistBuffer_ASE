using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using com.mirle.ibg3k0.sc.Data.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.BLL.Interface
{
    public interface IVehicleBLL
    {

        AVEHICLE getVehicle(string vhID);
        List<AVEHICLE> loadVehicleBySecID(string secID);
        List<AVEHICLE> loadCyclingAndTransferReadyVhs(ICMDBLL cmdBLL);


    }
}
