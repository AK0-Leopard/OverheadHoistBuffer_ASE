using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.WebAPI.Expansion
{
    public static class AVEHICLEExpansion
    {
        public static Mirle.Protos.ReserveModule.VehicleInfo convert2VehicleInfoForReserveModule(this AVEHICLE vh)
        {
            return new Mirle.Protos.ReserveModule.VehicleInfo()
            {
                VehicleId = vh.VEHICLE_ID,
                VehicleX = vh.X_Axis,
                VehicleY = vh.Y_Axis,
                VehicleAngle = 0,
                ForkDirection = Mirle.Protos.ReserveModule.Direction.None,
                SensorDirection = Mirle.Protos.ReserveModule.Direction.None,
                SpeedMmPreSecond = 1
            };
        }
    }
}
