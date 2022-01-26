using com.mirle.ibg3k0.sc.BLL.Interface;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestForLoopEnhance.StubObjects
{
    public class StubObjectCollection
    {
        public IPortBLL portBLL { get; private set; }
        public IVehicleBLL vehicleBLL { get; private set; }
        public IZoneCommandBLL zoneCommandBLL { get; private set; }
        public ISectionBLL sectionBLL { get; private set; }
        public IReserveBLL reserveBLL { get; private set; }
        public IPortDefBLL portDefBLL { get; private set; }
        public StubObjectCollection()
        {
            portBLL = Substitute.For<IPortBLL>();
            vehicleBLL = Substitute.For<IVehicleBLL>();
            zoneCommandBLL = Substitute.For<IZoneCommandBLL>();
            sectionBLL = Substitute.For<ISectionBLL>();
            reserveBLL = Substitute.For<IReserveBLL>();
            portDefBLL = Substitute.For<IPortDefBLL>();
        }
    }
}
