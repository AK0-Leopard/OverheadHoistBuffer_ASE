using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Common
{
    public static class AVEHICLEExpansion
    {
        public static Mirle.Hlts.Utils.HltVehicle convert2HltVehicle(this AVEHICLE vh)
        {
            var hlt_vh = new Mirle.Hlts.Utils.HltVehicle(vh.VEHICLE_ID, vh.X_Axis, vh.Y_Axis,
                                                         angle: 0, speedMmPerSecond: 1,
                                                         sensorDirection: Mirle.Hlts.Utils.HltDirection.None,
                                                         forkDirection: Mirle.Hlts.Utils.HltDirection.None);
            return hlt_vh;
        }
    }
}
