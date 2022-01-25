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
using UnitTestForLoopEnhance.StubObjects;
using System.Linq;

namespace UnitTestForLoopEnhance
{
    public class TestZoneCommandReserve
    {
        private StubObjectCollection GetStubObject()
        {
            return new StubObjectCollection();
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

        private void setSectionData(StubObjectCollection stub)
        {
            stub.sectionBLL.getSectionByToAdr("12270").Returns(new ASECTION() { SEC_ID = "30141", FROM_ADR_ID = "12269", TO_ADR_ID = "12270", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12271").Returns(new ASECTION() { SEC_ID = "30142", FROM_ADR_ID = "12270", TO_ADR_ID = "12271", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12272").Returns(new ASECTION() { SEC_ID = "30143", FROM_ADR_ID = "12271", TO_ADR_ID = "12272", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12273").Returns(new ASECTION() { SEC_ID = "30144", FROM_ADR_ID = "12272", TO_ADR_ID = "12273", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12274").Returns(new ASECTION() { SEC_ID = "30145", FROM_ADR_ID = "12273", TO_ADR_ID = "12274", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12275").Returns(new ASECTION() { SEC_ID = "30146", FROM_ADR_ID = "12274", TO_ADR_ID = "12275", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12276").Returns(new ASECTION() { SEC_ID = "30147", FROM_ADR_ID = "12275", TO_ADR_ID = "12276", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12277").Returns(new ASECTION() { SEC_ID = "30148", FROM_ADR_ID = "12276", TO_ADR_ID = "13277", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12278").Returns(new ASECTION() { SEC_ID = "30149", FROM_ADR_ID = "13277", TO_ADR_ID = "12278", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("13280").Returns(new ASECTION() { SEC_ID = "30150", FROM_ADR_ID = "12278", TO_ADR_ID = "13280", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12279").Returns(new ASECTION() { SEC_ID = "30151", FROM_ADR_ID = "13280", TO_ADR_ID = "12279", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12281").Returns(new ASECTION() { SEC_ID = "30152", FROM_ADR_ID = "12279", TO_ADR_ID = "12281", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("13282").Returns(new ASECTION() { SEC_ID = "30153", FROM_ADR_ID = "12281", TO_ADR_ID = "13282", SEC_DIS = 4000 });

            stub.sectionBLL.getSection("30141").Returns(new ASECTION() { SEC_ID = "30141", FROM_ADR_ID = "12269", TO_ADR_ID = "12270", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30142").Returns(new ASECTION() { SEC_ID = "30142", FROM_ADR_ID = "12270", TO_ADR_ID = "12271", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30143").Returns(new ASECTION() { SEC_ID = "30143", FROM_ADR_ID = "12271", TO_ADR_ID = "12272", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30144").Returns(new ASECTION() { SEC_ID = "30144", FROM_ADR_ID = "12272", TO_ADR_ID = "12273", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30145").Returns(new ASECTION() { SEC_ID = "30145", FROM_ADR_ID = "12273", TO_ADR_ID = "12274", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30146").Returns(new ASECTION() { SEC_ID = "30146", FROM_ADR_ID = "12274", TO_ADR_ID = "12275", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30147").Returns(new ASECTION() { SEC_ID = "30147", FROM_ADR_ID = "12275", TO_ADR_ID = "12276", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30148").Returns(new ASECTION() { SEC_ID = "30148", FROM_ADR_ID = "12276", TO_ADR_ID = "13277", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30149").Returns(new ASECTION() { SEC_ID = "30149", FROM_ADR_ID = "13277", TO_ADR_ID = "12278", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30150").Returns(new ASECTION() { SEC_ID = "30150", FROM_ADR_ID = "12278", TO_ADR_ID = "13280", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30151").Returns(new ASECTION() { SEC_ID = "30151", FROM_ADR_ID = "13280", TO_ADR_ID = "12279", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30152").Returns(new ASECTION() { SEC_ID = "30152", FROM_ADR_ID = "12279", TO_ADR_ID = "12281", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30153").Returns(new ASECTION() { SEC_ID = "30153", FROM_ADR_ID = "12281", TO_ADR_ID = "13282", SEC_DIS = 4000 });
        }

        private void setVehicleData(StubObjectCollection stub)
        {
            var vh1 = new AVEHICLE() { VEHICLE_ID = "B7_OHBLINE3_CR1", CUR_SEC_ID = "30145", ACC_SEC_DIST = 2000 };
            var vh2 = new AVEHICLE() { VEHICLE_ID = "B7_OHBLINE3_CR2", CUR_SEC_ID = "30143", ACC_SEC_DIST = 2000 };
            var vh3 = new AVEHICLE() { VEHICLE_ID = "B7_OHBLINE3_CR3", CUR_SEC_ID = "", ACC_SEC_DIST = 2000 };
            var vhs = new List<AVEHICLE>()
            {
                vh1,
                vh2,
                vh3
            };
            stub.vehicleBLL.loadCyclingVhs().Returns(vhs);

            stub.vehicleBLL.getVehicle("B7_OHBLINE3_CR1").Returns(vh1);
            stub.vehicleBLL.getVehicle("B7_OHBLINE3_CR2").Returns(vh2);
            stub.vehicleBLL.getVehicle("B7_OHBLINE3_CR3").Returns(vh3);
        }

        private void setZoneCommandData(StubObjectCollection stub)
        {
            stub.zoneCommandBLL.getZoneCommandGroup("ZC01").
                Returns(new ZoneCommandGroup("ZC01", new List<string>()
                { "B7_OHBLINE3_A01", "B7_OHBLINE3_A02", "B7_OHBLINE3_A03",
                  "108701","108801","108901","109001",
                  "109101","109201","109301","109401",
                  "109501","109601","109701"
                }));
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
            setSectionData(stub);
            setVehicleData(stub);
            setZoneCommandData(stub);

            //stub.zoneCommandBLL.getZoneCommandGroup(zoneCommandID).Returns(getFackZoneCommmandGroup());
            //stub.vehicleBLL.loadCyclingVhs().Returns(getFackVehicleFor_有空車在距離內_要Pass讓給他搬());
            //stub.vehicleBLL.getVehicle(vhID).
            //     Returns(new AVEHICLE() { VEHICLE_ID = "B7_OHBLINE3_CR1", CUR_SEC_ID = "30140", ACC_SEC_DIST = 2000 });

            //stub.portBLL.loadPortPLCInfoByZoneCommnadID(zoneCommandID).Returns(loadFackPortObj());
            //stub.sectionBLL.getSectionByToAdr("12269").Returns(new ASECTION() { SEC_ID = "30140", FROM_ADR_ID = "12268", TO_ADR_ID = "12269", SEC_DIS = 4000 });
            //stub.sectionBLL.getSectionByToAdr("12268").Returns(new ASECTION() { SEC_ID = "30139", FROM_ADR_ID = "12267", TO_ADR_ID = "12268", SEC_DIS = 4000 });
            //stub.sectionBLL.getSectionByToAdr("12267").Returns(new ASECTION() { SEC_ID = "30138", FROM_ADR_ID = "12266", TO_ADR_ID = "12267", SEC_DIS = 4000 });
            //stub.sectionBLL.getSection("30140").Returns(new ASECTION() { SEC_ID = "30140", FROM_ADR_ID = "12268", TO_ADR_ID = "12269", SEC_DIS = 4000 });
            //stub.sectionBLL.getSection("30139").Returns(new ASECTION() { SEC_ID = "30139", FROM_ADR_ID = "12267", TO_ADR_ID = "12268", SEC_DIS = 4000 });
            //stub.sectionBLL.getSection("30138").Returns(new ASECTION() { SEC_ID = "30138", FROM_ADR_ID = "12266", TO_ADR_ID = "12267", SEC_DIS = 4000 });

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
            //Arrange
            string vhID = "B7_OHBLINE3_CR1";
            string zoneCommandID = "ZONE1";
            List<ACMD_MCS> mcs_cmds = new List<ACMD_MCS>();
            mcs_cmds.Add(bulidFackCMD_MCS("1", "B7_OHBLINE3_A01", "B7_OHBLINE3-ZONE2"));

            var stub = GetStubObject();
            setSectionData(stub);

            setVehicleData(stub);
            //變更車子的位置
            stub.vehicleBLL.loadCyclingVhs().
                Where(v => v.VEHICLE_ID == "B7_OHBLINE3_CR2").FirstOrDefault().CUR_SEC_ID = "30141";

            setZoneCommandData(stub);

            LoopTransferEnhance loopTransferEnhance = new LoopTransferEnhance();
            loopTransferEnhance.Start(stub.portBLL, stub.zoneCommandBLL, stub.vehicleBLL, stub.sectionBLL);
            //Act
            var result = loopTransferEnhance.tryGetZoneCommand(mcs_cmds, vhID, zoneCommandID);

            //Assert
            var assert_result = (true, "B7_OHBLINE3_A01", mcs_cmds[0]);
            result.Should().BeEquivalentTo(assert_result);
        }


        [Ignore("尚未寫測試")]
        public void 車子上報ZoneCommandRequest_該Zone命令多筆_要找出最遠的一筆給他()
        {
            //Arrange
            string vhID = "";
            string zoneCommandID = "";
            List<ACMD_MCS> mcs_cmds = new List<ACMD_MCS>();
            mcs_cmds.Add(bulidFackCMD_MCS("1", "B7_OHBLINE3_A01", "B7_OHBLINE3-ZONE2"));
            mcs_cmds.Add(bulidFackCMD_MCS("2", "B7_OHBLINE3_A02", "B7_OHBLINE3-ZONE2"));

            LoopTransferEnhance loopTransferEnhance = new LoopTransferEnhance();

            //Act
            var result = loopTransferEnhance.tryGetZoneCommand(mcs_cmds, vhID, zoneCommandID);

            //Assert
            var assert_result = (false, default(ACMD_MCS));
            result.Should().BeEquivalentTo(assert_result);
        }
    }
}