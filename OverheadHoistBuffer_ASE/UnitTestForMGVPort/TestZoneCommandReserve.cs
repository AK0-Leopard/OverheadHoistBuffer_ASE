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

        private void setAddressData(StubObjectCollection stub)
        {
            stub.reserveBLL.GetHltMapAddress("12269").Returns((true, 6794, 0, false));
            stub.reserveBLL.GetHltMapAddress("12270").Returns((true, 7155, 0, false));
            stub.reserveBLL.GetHltMapAddress("12271").Returns((true, 7515, 0, false));
            stub.reserveBLL.GetHltMapAddress("12272").Returns((true, 8037, 0, false));
            stub.reserveBLL.GetHltMapAddress("12273").Returns((true, 8391, 0, false));
            stub.reserveBLL.GetHltMapAddress("12274").Returns((true, 8753, 0, false));
            stub.reserveBLL.GetHltMapAddress("12275").Returns((true, 9110, 0, false));
            stub.reserveBLL.GetHltMapAddress("12276").Returns((true, 9633, 0, false));
            stub.reserveBLL.GetHltMapAddress("13277").Returns((true, 9862, 0, false));
            stub.reserveBLL.GetHltMapAddress("12278").Returns((true, 9992, 0, false));
            stub.reserveBLL.GetHltMapAddress("12279").Returns((true, 10352, 0, false));
            stub.reserveBLL.GetHltMapAddress("13280").Returns((true, 10280, 0, false));
            stub.reserveBLL.GetHltMapAddress("12281").Returns((true, 10710, 0, false));
            stub.reserveBLL.GetHltMapAddress("13282").Returns((true, 10697, 0, false));
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
                Returns(new ZoneCommandGroup("ZC01",
                new List<string>()
                { "B7_OHBLINE3_A01", "B7_OHBLINE3_A02", "B7_OHBLINE3_A03",
                  "108701","108801","108901","109001",
                  "109101","109201","109301","109401",
                  "109501","109601","109701"
                },
                "(1,0)"));
        }

        private void setPortDefData(StubObjectCollection stub)
        {
            stub.portDefBLL.getPortDef("108701").Returns(new PortDef() { PLCPortID = "108701", ADR_ID = "12269" });
            stub.portDefBLL.getPortDef("108801").Returns(new PortDef() { PLCPortID = "108801", ADR_ID = "12270" });
            stub.portDefBLL.getPortDef("108901").Returns(new PortDef() { PLCPortID = "108901", ADR_ID = "12271" });
            stub.portDefBLL.getPortDef("109001").Returns(new PortDef() { PLCPortID = "109001", ADR_ID = "12272" });
            stub.portDefBLL.getPortDef("109101").Returns(new PortDef() { PLCPortID = "109101", ADR_ID = "12273" });
            stub.portDefBLL.getPortDef("109201").Returns(new PortDef() { PLCPortID = "109201", ADR_ID = "12274" });
            stub.portDefBLL.getPortDef("109301").Returns(new PortDef() { PLCPortID = "109301", ADR_ID = "12275" });
            stub.portDefBLL.getPortDef("109401").Returns(new PortDef() { PLCPortID = "109401", ADR_ID = "12276" });
            stub.portDefBLL.getPortDef("109501").Returns(new PortDef() { PLCPortID = "109501", ADR_ID = "12278" });
            stub.portDefBLL.getPortDef("109601").Returns(new PortDef() { PLCPortID = "109601", ADR_ID = "12279" });
            stub.portDefBLL.getPortDef("109701").Returns(new PortDef() { PLCPortID = "109701", ADR_ID = "12281" });
            stub.portDefBLL.getPortDef("B7_OHBLINE3_A01").Returns(new PortDef() { PLCPortID = "B7_OHBLINE3_A01", ADR_ID = "13277" });
            stub.portDefBLL.getPortDef("B7_OHBLINE3_A02").Returns(new PortDef() { PLCPortID = "B7_OHBLINE3_A02", ADR_ID = "13280" });
            stub.portDefBLL.getPortDef("B7_OHBLINE3_A03").Returns(new PortDef() { PLCPortID = "B7_OHBLINE3_A03", ADR_ID = "13282" });
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
            string vhID = "B7_OHBLINE3_CR1";
            string zoneCommandID = "ZC01";
            List<ACMD_MCS> mcs_cmds = new List<ACMD_MCS>();
            mcs_cmds.Add(bulidFackCMD_MCS("1", "B7_OHBLINE3_A04", "B7_OHBLINE3-ZONE2"));
            var stub = GetStubObject();
            setSectionData(stub);
            setVehicleData(stub);
            setZoneCommandData(stub);

            LoopTransferEnhance loopTransferEnhance = new LoopTransferEnhance();
            loopTransferEnhance.Start
                (stub.portBLL, stub.zoneCommandBLL, stub.vehicleBLL, stub.sectionBLL, stub.portDefBLL, stub.reserveBLL);

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
        [Test]
        public void 車子上報ZoneCommandRequest_該Zone命令1筆_有空車在距離內_要Pass讓給他搬()
        {
            //Arrange
            string vhID = "B7_OHBLINE3_CR1";
            string zoneCommandID = "ZC01";
            List<ACMD_MCS> mcs_cmds = new List<ACMD_MCS>();
            mcs_cmds.Add(bulidFackCMD_MCS("1", "B7_OHBLINE3_A01", "B7_OHBLINE3-ZONE2"));

            var stub = GetStubObject();
            setSectionData(stub);
            setVehicleData(stub);
            setZoneCommandData(stub);

            LoopTransferEnhance loopTransferEnhance = new LoopTransferEnhance();
            loopTransferEnhance.Start
                (stub.portBLL, stub.zoneCommandBLL, stub.vehicleBLL, stub.sectionBLL, stub.portDefBLL, stub.reserveBLL);
            //Act
            var result = loopTransferEnhance.tryGetZoneCommand(mcs_cmds, vhID, zoneCommandID);

            //Assert
            var assert_result = (false, "", default(ACMD_MCS));
            result.Should().BeEquivalentTo(assert_result);
        }


        [Test]
        public void 車子上報ZoneCommandRequest_該Zone命令1筆_有空車在距離外_回復有命令()
        {
            //Arrange
            string vhID = "B7_OHBLINE3_CR1";
            string zoneCommandID = "ZC01";
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
            loopTransferEnhance.Start
                (stub.portBLL, stub.zoneCommandBLL, stub.vehicleBLL, stub.sectionBLL, stub.portDefBLL, stub.reserveBLL);
            //Act
            var result = loopTransferEnhance.tryGetZoneCommand(mcs_cmds, vhID, zoneCommandID);

            //Assert
            var assert_result = (true, "B7_OHBLINE3_A01", mcs_cmds[0]);
            result.Should().BeEquivalentTo(assert_result);
        }


        [Test]
        public void 車子上報ZoneCommandRequest_該Zone命令多筆_要找出最遠的一筆給他()
        {
            //Arrange
            string vhID = "B7_OHBLINE3_CR1";
            string zoneCommandID = "ZC01";
            List<ACMD_MCS> mcs_cmds = new List<ACMD_MCS>();
            var cmd_mcs1 = bulidFackCMD_MCS("1", "B7_OHBLINE3_A01", "B7_OHBLINE3-ZONE2");
            var cmd_mcs2 = bulidFackCMD_MCS("2", "B7_OHBLINE3_A03", "B7_OHBLINE3-ZONE2");
            mcs_cmds.Add(cmd_mcs1);
            mcs_cmds.Add(cmd_mcs2);

            var stub = GetStubObject();
            setSectionData(stub);
            setAddressData(stub);
            setVehicleData(stub);
            setZoneCommandData(stub);
            setPortDefData(stub);
            LoopTransferEnhance loopTransferEnhance = new LoopTransferEnhance();
            loopTransferEnhance.Start
                (stub.portBLL, stub.zoneCommandBLL, stub.vehicleBLL, stub.sectionBLL, stub.portDefBLL, stub.reserveBLL);
            //Act
            var result = loopTransferEnhance.tryGetZoneCommand(mcs_cmds, vhID, zoneCommandID);

            //Assert
            var assert_result = (true, "B7_OHBLINE3_A03", cmd_mcs2);
            result.Should().BeEquivalentTo(assert_result);

        }
    }
}