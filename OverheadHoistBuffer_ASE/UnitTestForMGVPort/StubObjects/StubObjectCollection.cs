using com.mirle.ibg3k0.sc.BLL.Interface;
using com.mirle.ibg3k0.sc.Service.Interface;
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
        public IVehicleBLL vehicleBLL { get; private set; }
        public IZoneCommandBLL zoneCommandBLL { get; private set; }
        public ISectionBLL sectionBLL { get; private set; }
        public IReserveBLL reserveBLL { get; private set; }
        public IPortDefBLL portDefBLL { get; private set; }
        public ITransferService transferService { get; private set; }
        public IShelfDefBLL shelfDefBLL { get; private set; }
        public ICassetteDataBLL cassetteDataBLL { get; private set; }
        public ICMDBLL cmdBLL { get; private set; }
        public ISequenceBLL sequenceBLL { get; private set; }
        public StubObjectCollection()
        {
            vehicleBLL = Substitute.For<IVehicleBLL>();
            zoneCommandBLL = Substitute.For<IZoneCommandBLL>();
            sectionBLL = Substitute.For<ISectionBLL>();
            reserveBLL = Substitute.For<IReserveBLL>();
            portDefBLL = Substitute.For<IPortDefBLL>();
            transferService = Substitute.For<ITransferService>();
            shelfDefBLL = Substitute.For<IShelfDefBLL>();
            cassetteDataBLL = Substitute.For<ICassetteDataBLL>();
            cmdBLL = Substitute.For<ICMDBLL>();
            sequenceBLL = Substitute.For<ISequenceBLL>();
        }
    }
}
