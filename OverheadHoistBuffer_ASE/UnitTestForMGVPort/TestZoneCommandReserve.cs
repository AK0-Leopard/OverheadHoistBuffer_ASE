using com.mirle.ibg3k0.sc;
using com.mirle.ibg3k0.sc.BLL.Interface;
using com.mirle.ibg3k0.sc.Data.PLC_Functions;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.Module;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace UnitTestForLoopEnhance
{
    public class TestZoneCommandReserve
    {
        private (IPortBLL portBLL, IVehicleBLL vehicleBLL, IZoneCommandBLL zoneCommandBLL, ISectionBLL sectionBLL)
            GetStubObject()
        {
            var portBll = Substitute.For<IPortBLL>();
            var vehicleBll = Substitute.For<IVehicleBLL>();
            var zoneCommandBll = Substitute.For<IZoneCommandBLL>();
            var sectionBLL = Substitute.For<ISectionBLL>();
            return (portBll, vehicleBll, zoneCommandBll, sectionBLL);
        }
        private List<PortPLCInfo> loadFackPortObj()
        {
            return new List<PortPLCInfo>()
            {
                new PortPLCInfo(){ EQ_ID = "B7_OHBLINE3_A01",PortWaitIn = false},
                new PortPLCInfo(){ EQ_ID = "B7_OHBLINE3_A02",PortWaitIn = false},
                new PortPLCInfo(){ EQ_ID = "B7_OHBLINE3_A03",PortWaitIn = false},

            };
        }
        private ACMD_MCS bulidFackCMD_MCS(string cmdID, string fromPort, string toPort)
        {
            return new ACMD_MCS()
            {
                CMD_ID = cmdID,
                HOSTSOURCE = fromPort,
                HOSTDESTINATION = toPort
            };
        }


        [Test]
        public void 車子上報ZoneCommandRequest_該Zone命令0筆_Pass()
        {
            //Arrange
            string vhID = "";
            string zoneCommandID = "";
            List<ACMD_MCS> mcs_cmds = new List<ACMD_MCS>();
            LoopTransferEnhance loopTransferEnhance = new LoopTransferEnhance();

            //Act
            var result = loopTransferEnhance.tryGetZoneCommand(mcs_cmds, vhID, zoneCommandID);

            //Assert
            var assert_result = (false, "", default(ACMD_MCS));
            result.Should().BeEquivalentTo(assert_result);
        }

        [Ignore("困難實作，先跳過")]
        public void 車子上報ZoneCommandRequest_該Zone命令0筆_CV有貨物即將WaitIn()
        {
            //Arrange
            string vhID = "";
            string zoneCommandID = "";

            List<ACMD_MCS> mcs_cmds = new List<ACMD_MCS>();
            LoopTransferEnhance loopTransferEnhance = new LoopTransferEnhance();
            //Act
            var result = loopTransferEnhance.tryGetZoneCommand(mcs_cmds, vhID, zoneCommandID);

            //Assert
            var assert_result = (true, "B7_OHBLINE3_A01", default(ACMD_MCS));
            result.Should().BeEquivalentTo(assert_result);
        }
        private List<PortPLCInfo> loadFackPortObj_Port即將有貨物WaitIn()
        {
            return new List<PortPLCInfo>()
            {
                new PortPLCInfo(){ EQ_ID = "B7_OHBLINE3_A01",PortWaitIn = false,LoadPosition1 = true },
                new PortPLCInfo(){ EQ_ID = "B7_OHBLINE3_A02",PortWaitIn = false},
                new PortPLCInfo(){ EQ_ID = "B7_OHBLINE3_A03",PortWaitIn = false},

            };
        }
        public ZoneCommandGroup getFackZoneCommmandGroup()
        {
            return new ZoneCommandGroup("ZONE1", new List<string>() { "B7_OHBLINE3_A01", "B7_OHBLINE3_A02", "B7_OHBLINE3_A03" });
        }
        [Test]
        public void 車子上報ZoneCommandRequest_該Zone命令1筆_有空車在距離內_要Pass讓給他搬()
        {
            //Arrange
            string vhID = "B7_OHBLINE3_CR1";
            string zoneCommandID = "ZONE1";
            List<ACMD_MCS> mcs_cmds = new List<ACMD_MCS>();
            mcs_cmds.Add(bulidFackCMD_MCS("1", "B7_OHBLINE3_A01", "B7_OHBLINE3-ZONE2"));

            var stub = GetStubObject();
            stub.zoneCommandBLL.getZoneCommandGroup(zoneCommandID).Returns(getFackZoneCommmandGroup());
            stub.vehicleBLL.loadCyclingVhs().Returns(getFackVehicleFor_有空車在距離內_要Pass讓給他搬());
            stub.vehicleBLL.getVehicle(vhID).
                 Returns(new AVEHICLE() { VEHICLE_ID = "B7_OHBLINE3_CR1", CUR_SEC_ID = "30140", ACC_SEC_DIST = 2000 });

            stub.portBLL.loadPortPLCInfoByZoneCommnadID(zoneCommandID).Returns(loadFackPortObj());
            stub.sectionBLL.getSectionByToAdr("12269").Returns(new ASECTION() { SEC_ID = "30140", FROM_ADR_ID = "12268", TO_ADR_ID = "12269", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12268").Returns(new ASECTION() { SEC_ID = "30139", FROM_ADR_ID = "12267", TO_ADR_ID = "12268", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12267").Returns(new ASECTION() { SEC_ID = "30138", FROM_ADR_ID = "12266", TO_ADR_ID = "12267", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30140").Returns(new ASECTION() { SEC_ID = "30140", FROM_ADR_ID = "12268", TO_ADR_ID = "12269", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30139").Returns(new ASECTION() { SEC_ID = "30139", FROM_ADR_ID = "12267", TO_ADR_ID = "12268", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30138").Returns(new ASECTION() { SEC_ID = "30138", FROM_ADR_ID = "12266", TO_ADR_ID = "12267", SEC_DIS = 4000 });

            LoopTransferEnhance loopTransferEnhance = new LoopTransferEnhance();
            loopTransferEnhance.Start(stub.portBLL, stub.zoneCommandBLL, stub.vehicleBLL, stub.sectionBLL);
            //Act
            var result = loopTransferEnhance.tryGetZoneCommand(mcs_cmds, vhID, zoneCommandID);

            //Assert
            var assert_result = (false, "", default(ACMD_MCS));
            result.Should().BeEquivalentTo(assert_result);
        }
        private List<AVEHICLE> getFackVehicleFor_有空車在距離內_要Pass讓給他搬()
        {
            return new List<AVEHICLE>()
            {
                new AVEHICLE(){VEHICLE_ID = "B7_OHBLINE3_CR1", CUR_SEC_ID = "30140",ACC_SEC_DIST = 2100},
                new AVEHICLE(){VEHICLE_ID = "B7_OHBLINE3_CR2", CUR_SEC_ID = "30139",ACC_SEC_DIST = 2000},
                new AVEHICLE(){VEHICLE_ID = "B7_OHBLINE3_CR3", CUR_SEC_ID = ""}
            };
        }



        [Test]
        public void 車子上報ZoneCommandRequest_該Zone命令1筆_有空車在距離外_回復有命令()
        {
            string vhID = "B7_OHBLINE3_CR1";
            string zoneCommandID = "ZONE1";
            List<ACMD_MCS> mcs_cmds = new List<ACMD_MCS>();
            mcs_cmds.Add(bulidFackCMD_MCS("1", "B7_OHBLINE3_A01", "B7_OHBLINE3-ZONE2"));

            var stub = GetStubObject();
            stub.zoneCommandBLL.getZoneCommandGroup(zoneCommandID).Returns(getFackZoneCommmandGroup());
            stub.vehicleBLL.loadCyclingVhs().Returns(getFackVehicleFor_有空車在距離外_要把命令給該VH());
            stub.vehicleBLL.getVehicle(vhID).
                 Returns(new AVEHICLE() { VEHICLE_ID = "B7_OHBLINE3_CR1", CUR_SEC_ID = "30140", ACC_SEC_DIST = 2000 });

            stub.portBLL.loadPortPLCInfoByZoneCommnadID(zoneCommandID).Returns(loadFackPortObj());

            stub.sectionBLL.getSectionByToAdr("12269").Returns(new ASECTION() { SEC_ID = "30140", FROM_ADR_ID = "12268", TO_ADR_ID = "12269", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12268").Returns(new ASECTION() { SEC_ID = "30139", FROM_ADR_ID = "12267", TO_ADR_ID = "12268", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12267").Returns(new ASECTION() { SEC_ID = "30138", FROM_ADR_ID = "12266", TO_ADR_ID = "12267", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30140").Returns(new ASECTION() { SEC_ID = "30140", FROM_ADR_ID = "12268", TO_ADR_ID = "12269", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30139").Returns(new ASECTION() { SEC_ID = "30139", FROM_ADR_ID = "12267", TO_ADR_ID = "12268", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30138").Returns(new ASECTION() { SEC_ID = "30138", FROM_ADR_ID = "12266", TO_ADR_ID = "12267", SEC_DIS = 4000 });

            LoopTransferEnhance loopTransferEnhance = new LoopTransferEnhance();
            loopTransferEnhance.Start(stub.portBLL, stub.zoneCommandBLL, stub.vehicleBLL, stub.sectionBLL);
            //Act
            var result = loopTransferEnhance.tryGetZoneCommand(mcs_cmds, vhID, zoneCommandID);

            //Assert
            var assert_result = (true, "B7_OHBLINE3_A01", mcs_cmds[0]);
            result.Should().BeEquivalentTo(assert_result);
        }
        private List<AVEHICLE> getFackVehicleFor_有空車在距離外_要把命令給該VH()
        {
            return new List<AVEHICLE>()
            {
                new AVEHICLE(){VEHICLE_ID = "B7_OHBLINE3_CR1", CUR_SEC_ID = "30140",ACC_SEC_DIST = 2000},
                new AVEHICLE(){VEHICLE_ID = "B7_OHBLINE3_CR2", CUR_SEC_ID = "30140",ACC_SEC_DIST = 2500},
                new AVEHICLE(){VEHICLE_ID = "B7_OHBLINE3_CR3", CUR_SEC_ID = ""}
            };
        }



        [Ignore("尚未寫測試")]
        public void 車子上報ZoneCommandRequest_該Zone命令多筆_要找出最遠的一筆給他()
        {
            //Arrange
            string vhID = "";
            string zoneCommandID = "";
            List<ACMD_MCS> mcs_cmds = new List<ACMD_MCS>();
            LoopTransferEnhance loopTransferEnhance = new LoopTransferEnhance();

            //Act
            var result = loopTransferEnhance.tryGetZoneCommand(mcs_cmds, vhID, zoneCommandID);

            //Assert
            var assert_result = (false, default(ACMD_MCS));
            result.Should().BeEquivalentTo(assert_result);
        }
    }
}