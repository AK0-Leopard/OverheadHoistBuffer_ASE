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
            //Zone01
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

            //Zone05
            stub.reserveBLL.GetHltMapAddress("12010").Returns((true, 37121, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12011").Returns((true, 36762, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12012").Returns((true, 36241, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12013").Returns((true, 35878, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12014").Returns((true, 35521, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12015").Returns((true, 35163, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12016").Returns((true, 34642, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12017").Returns((true, 34281, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12018").Returns((true, 33922, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12019").Returns((true, 33559, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12020").Returns((true, 33041, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12021").Returns((true, 32679, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12022").Returns((true, 32318, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12023").Returns((true, 31954, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12024").Returns((true, 31437, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12025").Returns((true, 31081, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12026").Returns((true, 30712, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12027").Returns((true, 30364, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12028").Returns((true, 29845, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12029").Returns((true, 29484, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12030").Returns((true, 29124, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12031").Returns((true, 28766, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12032").Returns((true, 28246, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12033").Returns((true, 27885, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12034").Returns((true, 27528, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12035").Returns((true, 27170, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12036").Returns((true, 26647, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12037").Returns((true, 26288, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12038").Returns((true, 25926, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12039").Returns((true, 25572, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12040").Returns((true, 25053, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12041").Returns((true, 24695, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12042").Returns((true, 24333, 2270, false));
            stub.reserveBLL.GetHltMapAddress("12043").Returns((true, 23967, 2270, false));


            //ZC06-T09的點位
            stub.reserveBLL.GetHltMapAddress("15150").Returns((true, 15025, 2270, false));
        }

        private void setSectionData(StubObjectCollection stub)
        {
            //Zone01
            stub.sectionBLL.getSectionByToAdr("12269").Returns(new ASECTION() { SEC_ID = "30140", FROM_ADR_ID = "12268", TO_ADR_ID = "12269", SEC_DIS = 4000 });
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

            stub.sectionBLL.getSection("30140").Returns(new ASECTION() { SEC_ID = "30140", FROM_ADR_ID = "12268", TO_ADR_ID = "12269", SEC_DIS = 4000 });
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

            //Zone05
            stub.sectionBLL.getSectionByToAdr("12010").Returns(new ASECTION() { SEC_ID = "30009", FROM_ADR_ID = "12009", TO_ADR_ID = "12010", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12011").Returns(new ASECTION() { SEC_ID = "30010", FROM_ADR_ID = "12010", TO_ADR_ID = "12011", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12012").Returns(new ASECTION() { SEC_ID = "30011", FROM_ADR_ID = "12011", TO_ADR_ID = "12012", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12013").Returns(new ASECTION() { SEC_ID = "30012", FROM_ADR_ID = "12012", TO_ADR_ID = "12013", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12014").Returns(new ASECTION() { SEC_ID = "30013", FROM_ADR_ID = "12013", TO_ADR_ID = "12014", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12015").Returns(new ASECTION() { SEC_ID = "30014", FROM_ADR_ID = "12014", TO_ADR_ID = "12015", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12016").Returns(new ASECTION() { SEC_ID = "30015", FROM_ADR_ID = "12015", TO_ADR_ID = "12016", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12017").Returns(new ASECTION() { SEC_ID = "30016", FROM_ADR_ID = "12016", TO_ADR_ID = "12017", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12018").Returns(new ASECTION() { SEC_ID = "30017", FROM_ADR_ID = "12017", TO_ADR_ID = "12018", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12019").Returns(new ASECTION() { SEC_ID = "30018", FROM_ADR_ID = "12018", TO_ADR_ID = "12019", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12020").Returns(new ASECTION() { SEC_ID = "30019", FROM_ADR_ID = "12019", TO_ADR_ID = "12020", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12021").Returns(new ASECTION() { SEC_ID = "30020", FROM_ADR_ID = "12020", TO_ADR_ID = "12021", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12022").Returns(new ASECTION() { SEC_ID = "30021", FROM_ADR_ID = "12021", TO_ADR_ID = "12022", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12023").Returns(new ASECTION() { SEC_ID = "30022", FROM_ADR_ID = "12022", TO_ADR_ID = "12023", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12024").Returns(new ASECTION() { SEC_ID = "30023", FROM_ADR_ID = "12023", TO_ADR_ID = "12024", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12025").Returns(new ASECTION() { SEC_ID = "30024", FROM_ADR_ID = "12024", TO_ADR_ID = "12025", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12026").Returns(new ASECTION() { SEC_ID = "30025", FROM_ADR_ID = "12025", TO_ADR_ID = "12026", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12027").Returns(new ASECTION() { SEC_ID = "30026", FROM_ADR_ID = "12026", TO_ADR_ID = "12027", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12028").Returns(new ASECTION() { SEC_ID = "30027", FROM_ADR_ID = "12027", TO_ADR_ID = "12028", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12029").Returns(new ASECTION() { SEC_ID = "30028", FROM_ADR_ID = "12028", TO_ADR_ID = "12029", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12030").Returns(new ASECTION() { SEC_ID = "30029", FROM_ADR_ID = "12029", TO_ADR_ID = "12030", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12031").Returns(new ASECTION() { SEC_ID = "30030", FROM_ADR_ID = "12030", TO_ADR_ID = "12031", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12032").Returns(new ASECTION() { SEC_ID = "30031", FROM_ADR_ID = "12031", TO_ADR_ID = "12032", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12033").Returns(new ASECTION() { SEC_ID = "30032", FROM_ADR_ID = "12032", TO_ADR_ID = "12033", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12034").Returns(new ASECTION() { SEC_ID = "30033", FROM_ADR_ID = "12033", TO_ADR_ID = "12034", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12035").Returns(new ASECTION() { SEC_ID = "30034", FROM_ADR_ID = "12034", TO_ADR_ID = "12035", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12036").Returns(new ASECTION() { SEC_ID = "30035", FROM_ADR_ID = "12035", TO_ADR_ID = "12036", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12037").Returns(new ASECTION() { SEC_ID = "30036", FROM_ADR_ID = "12036", TO_ADR_ID = "12037", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12038").Returns(new ASECTION() { SEC_ID = "30037", FROM_ADR_ID = "12037", TO_ADR_ID = "12038", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12039").Returns(new ASECTION() { SEC_ID = "30038", FROM_ADR_ID = "12038", TO_ADR_ID = "12039", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12040").Returns(new ASECTION() { SEC_ID = "30039", FROM_ADR_ID = "12039", TO_ADR_ID = "12040", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12041").Returns(new ASECTION() { SEC_ID = "30040", FROM_ADR_ID = "12040", TO_ADR_ID = "12041", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12042").Returns(new ASECTION() { SEC_ID = "30041", FROM_ADR_ID = "12041", TO_ADR_ID = "12042", SEC_DIS = 4000 });
            stub.sectionBLL.getSectionByToAdr("12043").Returns(new ASECTION() { SEC_ID = "30042", FROM_ADR_ID = "12042", TO_ADR_ID = "12043", SEC_DIS = 4000 });

            stub.sectionBLL.getSection("30009").Returns(new ASECTION() { SEC_ID = "30009", FROM_ADR_ID = "12009", TO_ADR_ID = "12010", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30010").Returns(new ASECTION() { SEC_ID = "30010", FROM_ADR_ID = "12010", TO_ADR_ID = "12011", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30011").Returns(new ASECTION() { SEC_ID = "30011", FROM_ADR_ID = "12011", TO_ADR_ID = "12012", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30012").Returns(new ASECTION() { SEC_ID = "30012", FROM_ADR_ID = "12012", TO_ADR_ID = "12013", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30013").Returns(new ASECTION() { SEC_ID = "30013", FROM_ADR_ID = "12013", TO_ADR_ID = "12014", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30014").Returns(new ASECTION() { SEC_ID = "30014", FROM_ADR_ID = "12014", TO_ADR_ID = "12015", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30015").Returns(new ASECTION() { SEC_ID = "30015", FROM_ADR_ID = "12015", TO_ADR_ID = "12016", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30016").Returns(new ASECTION() { SEC_ID = "30016", FROM_ADR_ID = "12016", TO_ADR_ID = "12017", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30017").Returns(new ASECTION() { SEC_ID = "30017", FROM_ADR_ID = "12017", TO_ADR_ID = "12018", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30018").Returns(new ASECTION() { SEC_ID = "30018", FROM_ADR_ID = "12018", TO_ADR_ID = "12019", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30019").Returns(new ASECTION() { SEC_ID = "30019", FROM_ADR_ID = "12019", TO_ADR_ID = "12020", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30020").Returns(new ASECTION() { SEC_ID = "30020", FROM_ADR_ID = "12020", TO_ADR_ID = "12021", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30021").Returns(new ASECTION() { SEC_ID = "30021", FROM_ADR_ID = "12021", TO_ADR_ID = "12022", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30022").Returns(new ASECTION() { SEC_ID = "30022", FROM_ADR_ID = "12022", TO_ADR_ID = "12023", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30023").Returns(new ASECTION() { SEC_ID = "30023", FROM_ADR_ID = "12023", TO_ADR_ID = "12024", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30024").Returns(new ASECTION() { SEC_ID = "30024", FROM_ADR_ID = "12024", TO_ADR_ID = "12025", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30025").Returns(new ASECTION() { SEC_ID = "30025", FROM_ADR_ID = "12025", TO_ADR_ID = "12026", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30026").Returns(new ASECTION() { SEC_ID = "30026", FROM_ADR_ID = "12026", TO_ADR_ID = "12027", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30027").Returns(new ASECTION() { SEC_ID = "30027", FROM_ADR_ID = "12027", TO_ADR_ID = "12028", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30028").Returns(new ASECTION() { SEC_ID = "30028", FROM_ADR_ID = "12028", TO_ADR_ID = "12029", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30029").Returns(new ASECTION() { SEC_ID = "30029", FROM_ADR_ID = "12029", TO_ADR_ID = "12030", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30030").Returns(new ASECTION() { SEC_ID = "30030", FROM_ADR_ID = "12030", TO_ADR_ID = "12031", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30031").Returns(new ASECTION() { SEC_ID = "30031", FROM_ADR_ID = "12031", TO_ADR_ID = "12032", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30032").Returns(new ASECTION() { SEC_ID = "30032", FROM_ADR_ID = "12032", TO_ADR_ID = "12033", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30033").Returns(new ASECTION() { SEC_ID = "30033", FROM_ADR_ID = "12033", TO_ADR_ID = "12034", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30034").Returns(new ASECTION() { SEC_ID = "30034", FROM_ADR_ID = "12034", TO_ADR_ID = "12035", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30035").Returns(new ASECTION() { SEC_ID = "30035", FROM_ADR_ID = "12035", TO_ADR_ID = "12036", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30036").Returns(new ASECTION() { SEC_ID = "30036", FROM_ADR_ID = "12036", TO_ADR_ID = "12037", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30037").Returns(new ASECTION() { SEC_ID = "30037", FROM_ADR_ID = "12037", TO_ADR_ID = "12038", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30038").Returns(new ASECTION() { SEC_ID = "30038", FROM_ADR_ID = "12038", TO_ADR_ID = "12039", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30039").Returns(new ASECTION() { SEC_ID = "30039", FROM_ADR_ID = "12039", TO_ADR_ID = "12040", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30040").Returns(new ASECTION() { SEC_ID = "30040", FROM_ADR_ID = "12040", TO_ADR_ID = "12041", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30041").Returns(new ASECTION() { SEC_ID = "30041", FROM_ADR_ID = "12041", TO_ADR_ID = "12042", SEC_DIS = 4000 });
            stub.sectionBLL.getSection("30042").Returns(new ASECTION() { SEC_ID = "30042", FROM_ADR_ID = "12042", TO_ADR_ID = "12043", SEC_DIS = 4000 });
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
            stub.vehicleBLL.loadCyclingAndTransferReadyVhs(stub.cmdBLL).Returns(vhs);

            stub.vehicleBLL.getVehicle("B7_OHBLINE3_CR1").Returns(vh1);
            stub.vehicleBLL.getVehicle("B7_OHBLINE3_CR2").Returns(vh2);
            stub.vehicleBLL.getVehicle("B7_OHBLINE3_CR3").Returns(vh3);
        }

        private void setZoneCommandData(StubObjectCollection stub)
        {
            var zone_command_group_ZC01 = new ZoneCommandGroup("ZC01",
                new List<string>()
                { "B7_OHBLINE3_A01", "B7_OHBLINE3_A02", "B7_OHBLINE3_A03",
                  "108701","108801","108901","109001",
                  "109101","109201","109301","109401",
                  "109501","109601","109701"
                },
                "(1,0)");
            stub.zoneCommandBLL.getZoneCommandGroup("ZC01").Returns(zone_command_group_ZC01);
            stub.zoneCommandBLL.tryGetZoneCommandGroupByPortID("B7_OHBLINE3_A01").
                Returns((true, zone_command_group_ZC01));
            stub.zoneCommandBLL.tryGetZoneCommandGroupByPortID("B7_OHBLINE3_A02").
                Returns((true, zone_command_group_ZC01));
            stub.zoneCommandBLL.tryGetZoneCommandGroupByPortID("109001").
                Returns((true, zone_command_group_ZC01));

            var zone_command_group_n = new ZoneCommandGroup("ZC06",
                new List<string>()
                { "B7_OHBlOOP_T09"
                },
                "(-1,0)");
            stub.zoneCommandBLL.getZoneCommandGroup("ZC06").Returns(zone_command_group_n);
            stub.zoneCommandBLL.tryGetZoneCommandGroupByPortID("B7_OHBlOOP_T09").
                Returns((true, zone_command_group_n));

            var zone_command_group_ZC05 = new ZoneCommandGroup("ZC05",
                new List<string>()
                { "100601", "100701", "100801", "100901", "101001",
                  "101101","101201","101301","101401","101501","101601","101701","101801","101901","102001",
                  "102101","102201","102301","102401","102501","102601","102701","102801","102901","103001",
                  "103101","103201","103301","103401","103501","103601","103701","103801","103901","104001"
                },
                "(-1,0)");
            stub.zoneCommandBLL.getZoneCommandGroup("ZC05").Returns(zone_command_group_ZC05);
            stub.zoneCommandBLL.tryGetZoneCommandGroupByPortID("102501").
                Returns((true, zone_command_group_ZC05));
            stub.zoneCommandBLL.tryGetZoneCommandGroupByPortID("103501").
                Returns((true, zone_command_group_ZC05));
            stub.zoneCommandBLL.tryGetZoneCommandGroupByPortID("102001").
                Returns((true, zone_command_group_ZC05));
            stub.zoneCommandBLL.tryGetZoneCommandGroupByPortID("103301").
                Returns((true, zone_command_group_ZC05));
        }

        private void setPortDefData(StubObjectCollection stub)
        {
            //Zone01
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

            stub.portDefBLL.getPortDef("B7_OHBlOOP_T09").Returns(new PortDef() { PLCPortID = "B7_OHBlOOP_T09", ADR_ID = "15150" });

            stub.portDefBLL.getPortDefByAdrID("12269").Returns(new PortDef() { PLCPortID = "108701", ADR_ID = "12269" });
            stub.portDefBLL.getPortDefByAdrID("12270").Returns(new PortDef() { PLCPortID = "108801", ADR_ID = "12270" });
            stub.portDefBLL.getPortDefByAdrID("12271").Returns(new PortDef() { PLCPortID = "108901", ADR_ID = "12271" });
            stub.portDefBLL.getPortDefByAdrID("12272").Returns(new PortDef() { PLCPortID = "109001", ADR_ID = "12272" });
            stub.portDefBLL.getPortDefByAdrID("12273").Returns(new PortDef() { PLCPortID = "109101", ADR_ID = "12273" });
            stub.portDefBLL.getPortDefByAdrID("12274").Returns(new PortDef() { PLCPortID = "109201", ADR_ID = "12274" });
            stub.portDefBLL.getPortDefByAdrID("12275").Returns(new PortDef() { PLCPortID = "109301", ADR_ID = "12275" });
            stub.portDefBLL.getPortDefByAdrID("12276").Returns(new PortDef() { PLCPortID = "109401", ADR_ID = "12276" });
            stub.portDefBLL.getPortDefByAdrID("12278").Returns(new PortDef() { PLCPortID = "109501", ADR_ID = "12278" });
            stub.portDefBLL.getPortDefByAdrID("12279").Returns(new PortDef() { PLCPortID = "109601", ADR_ID = "12279" });
            stub.portDefBLL.getPortDefByAdrID("12281").Returns(new PortDef() { PLCPortID = "109701", ADR_ID = "12281" });
            stub.portDefBLL.getPortDefByAdrID("13277").Returns(new PortDef() { PLCPortID = "B7_OHBLINE3_A01", ADR_ID = "13277" });
            stub.portDefBLL.getPortDefByAdrID("13280").Returns(new PortDef() { PLCPortID = "B7_OHBLINE3_A02", ADR_ID = "13280" });
            stub.portDefBLL.getPortDefByAdrID("13282").Returns(new PortDef() { PLCPortID = "B7_OHBLINE3_A03", ADR_ID = "13282" });

            //Zone05
            stub.portDefBLL.getPortDef("100601").Returns(new PortDef() { PLCPortID = "100601", ADR_ID = "12009" });
            stub.portDefBLL.getPortDef("100701").Returns(new PortDef() { PLCPortID = "100701", ADR_ID = "12010" });
            stub.portDefBLL.getPortDef("100801").Returns(new PortDef() { PLCPortID = "100801", ADR_ID = "12011" });
            stub.portDefBLL.getPortDef("100901").Returns(new PortDef() { PLCPortID = "100901", ADR_ID = "12012" });
            stub.portDefBLL.getPortDef("101001").Returns(new PortDef() { PLCPortID = "101001", ADR_ID = "12013" });
            stub.portDefBLL.getPortDef("101101").Returns(new PortDef() { PLCPortID = "101101", ADR_ID = "12014" });
            stub.portDefBLL.getPortDef("101201").Returns(new PortDef() { PLCPortID = "101201", ADR_ID = "12015" });
            stub.portDefBLL.getPortDef("101301").Returns(new PortDef() { PLCPortID = "101301", ADR_ID = "12016" });
            stub.portDefBLL.getPortDef("101401").Returns(new PortDef() { PLCPortID = "101401", ADR_ID = "12017" });
            stub.portDefBLL.getPortDef("101501").Returns(new PortDef() { PLCPortID = "101501", ADR_ID = "12018" });
            stub.portDefBLL.getPortDef("101601").Returns(new PortDef() { PLCPortID = "101601", ADR_ID = "12019" });
            stub.portDefBLL.getPortDef("101701").Returns(new PortDef() { PLCPortID = "101701", ADR_ID = "12020" });
            stub.portDefBLL.getPortDef("101801").Returns(new PortDef() { PLCPortID = "101801", ADR_ID = "12021" });
            stub.portDefBLL.getPortDef("101901").Returns(new PortDef() { PLCPortID = "101901", ADR_ID = "12022" });
            stub.portDefBLL.getPortDef("102001").Returns(new PortDef() { PLCPortID = "102001", ADR_ID = "12023" });
            stub.portDefBLL.getPortDef("102101").Returns(new PortDef() { PLCPortID = "102101", ADR_ID = "12024" });
            stub.portDefBLL.getPortDef("102201").Returns(new PortDef() { PLCPortID = "102201", ADR_ID = "12025" });
            stub.portDefBLL.getPortDef("102301").Returns(new PortDef() { PLCPortID = "102301", ADR_ID = "12026" });
            stub.portDefBLL.getPortDef("102401").Returns(new PortDef() { PLCPortID = "102401", ADR_ID = "12027" });
            stub.portDefBLL.getPortDef("102501").Returns(new PortDef() { PLCPortID = "102501", ADR_ID = "12028" });
            stub.portDefBLL.getPortDef("102601").Returns(new PortDef() { PLCPortID = "102601", ADR_ID = "12029" });
            stub.portDefBLL.getPortDef("102701").Returns(new PortDef() { PLCPortID = "102701", ADR_ID = "12030" });
            stub.portDefBLL.getPortDef("102801").Returns(new PortDef() { PLCPortID = "102801", ADR_ID = "12031" });
            stub.portDefBLL.getPortDef("102901").Returns(new PortDef() { PLCPortID = "102901", ADR_ID = "12032" });
            stub.portDefBLL.getPortDef("103001").Returns(new PortDef() { PLCPortID = "103001", ADR_ID = "12033" });
            stub.portDefBLL.getPortDef("103101").Returns(new PortDef() { PLCPortID = "103101", ADR_ID = "12034" });
            stub.portDefBLL.getPortDef("103201").Returns(new PortDef() { PLCPortID = "103201", ADR_ID = "12035" });
            stub.portDefBLL.getPortDef("103301").Returns(new PortDef() { PLCPortID = "103301", ADR_ID = "12036" });
            stub.portDefBLL.getPortDef("103401").Returns(new PortDef() { PLCPortID = "103401", ADR_ID = "12037" });
            stub.portDefBLL.getPortDef("103501").Returns(new PortDef() { PLCPortID = "103501", ADR_ID = "12038" });
            stub.portDefBLL.getPortDef("103601").Returns(new PortDef() { PLCPortID = "103601", ADR_ID = "12039" });
            stub.portDefBLL.getPortDef("103701").Returns(new PortDef() { PLCPortID = "103701", ADR_ID = "12040" });
            stub.portDefBLL.getPortDef("103801").Returns(new PortDef() { PLCPortID = "103801", ADR_ID = "12041" });
            stub.portDefBLL.getPortDef("103901").Returns(new PortDef() { PLCPortID = "103901", ADR_ID = "12042" });
            stub.portDefBLL.getPortDef("104001").Returns(new PortDef() { PLCPortID = "104001", ADR_ID = "12043" });


            stub.portDefBLL.getPortDefByAdrID("12009").Returns(new PortDef() { PLCPortID = "100601", ADR_ID = "12009" });
            stub.portDefBLL.getPortDefByAdrID("12010").Returns(new PortDef() { PLCPortID = "100701", ADR_ID = "12010" });
            stub.portDefBLL.getPortDefByAdrID("12011").Returns(new PortDef() { PLCPortID = "100801", ADR_ID = "12011" });
            stub.portDefBLL.getPortDefByAdrID("12012").Returns(new PortDef() { PLCPortID = "100901", ADR_ID = "12012" });
            stub.portDefBLL.getPortDefByAdrID("12013").Returns(new PortDef() { PLCPortID = "101001", ADR_ID = "12013" });
            stub.portDefBLL.getPortDefByAdrID("12014").Returns(new PortDef() { PLCPortID = "101101", ADR_ID = "12014" });
            stub.portDefBLL.getPortDefByAdrID("12015").Returns(new PortDef() { PLCPortID = "101201", ADR_ID = "12015" });
            stub.portDefBLL.getPortDefByAdrID("12016").Returns(new PortDef() { PLCPortID = "101301", ADR_ID = "12016" });
            stub.portDefBLL.getPortDefByAdrID("12017").Returns(new PortDef() { PLCPortID = "101401", ADR_ID = "12017" });
            stub.portDefBLL.getPortDefByAdrID("12018").Returns(new PortDef() { PLCPortID = "101501", ADR_ID = "12018" });
            stub.portDefBLL.getPortDefByAdrID("12019").Returns(new PortDef() { PLCPortID = "101601", ADR_ID = "12019" });
            stub.portDefBLL.getPortDefByAdrID("12020").Returns(new PortDef() { PLCPortID = "101701", ADR_ID = "12020" });
            stub.portDefBLL.getPortDefByAdrID("12021").Returns(new PortDef() { PLCPortID = "101801", ADR_ID = "12021" });
            stub.portDefBLL.getPortDefByAdrID("12022").Returns(new PortDef() { PLCPortID = "101901", ADR_ID = "12022" });
            stub.portDefBLL.getPortDefByAdrID("12023").Returns(new PortDef() { PLCPortID = "102001", ADR_ID = "12023" });
            stub.portDefBLL.getPortDefByAdrID("12024").Returns(new PortDef() { PLCPortID = "102101", ADR_ID = "12024" });
            stub.portDefBLL.getPortDefByAdrID("12025").Returns(new PortDef() { PLCPortID = "102201", ADR_ID = "12025" });
            stub.portDefBLL.getPortDefByAdrID("12026").Returns(new PortDef() { PLCPortID = "102301", ADR_ID = "12026" });
            stub.portDefBLL.getPortDefByAdrID("12027").Returns(new PortDef() { PLCPortID = "102401", ADR_ID = "12027" });
            stub.portDefBLL.getPortDefByAdrID("12028").Returns(new PortDef() { PLCPortID = "102501", ADR_ID = "12028" });
            stub.portDefBLL.getPortDefByAdrID("12029").Returns(new PortDef() { PLCPortID = "102601", ADR_ID = "12029" });
            stub.portDefBLL.getPortDefByAdrID("12030").Returns(new PortDef() { PLCPortID = "102701", ADR_ID = "12030" });
            stub.portDefBLL.getPortDefByAdrID("12031").Returns(new PortDef() { PLCPortID = "102801", ADR_ID = "12031" });
            stub.portDefBLL.getPortDefByAdrID("12032").Returns(new PortDef() { PLCPortID = "102901", ADR_ID = "12032" });
            stub.portDefBLL.getPortDefByAdrID("12033").Returns(new PortDef() { PLCPortID = "103001", ADR_ID = "12033" });
            stub.portDefBLL.getPortDefByAdrID("12034").Returns(new PortDef() { PLCPortID = "103101", ADR_ID = "12034" });
            stub.portDefBLL.getPortDefByAdrID("12035").Returns(new PortDef() { PLCPortID = "103201", ADR_ID = "12035" });
            stub.portDefBLL.getPortDefByAdrID("12036").Returns(new PortDef() { PLCPortID = "103301", ADR_ID = "12036" });
            stub.portDefBLL.getPortDefByAdrID("12037").Returns(new PortDef() { PLCPortID = "103401", ADR_ID = "12037" });
            stub.portDefBLL.getPortDefByAdrID("12038").Returns(new PortDef() { PLCPortID = "103501", ADR_ID = "12038" });
            stub.portDefBLL.getPortDefByAdrID("12039").Returns(new PortDef() { PLCPortID = "103601", ADR_ID = "12039" });
            stub.portDefBLL.getPortDefByAdrID("12040").Returns(new PortDef() { PLCPortID = "103701", ADR_ID = "12040" });
            stub.portDefBLL.getPortDefByAdrID("12041").Returns(new PortDef() { PLCPortID = "103801", ADR_ID = "12041" });
            stub.portDefBLL.getPortDefByAdrID("12042").Returns(new PortDef() { PLCPortID = "103901", ADR_ID = "12042" });
            stub.portDefBLL.getPortDefByAdrID("12043").Returns(new PortDef() { PLCPortID = "104001", ADR_ID = "12043" });


        }
        private static void setSeqNum(StubObjectCollection stub)
        {
            stub.sequenceBLL.
                getCommandID(com.mirle.ibg3k0.sc.App.SCAppConstants.GenOHxCCommandType.Auto).
                Returns("111");
        }

        private ACMD_MCS bulidFackCMD_MCS(string cmdID, string fromPort, string toPort)
        {
            return new ACMD_MCS()
            {
                CMD_ID = cmdID,
                BOX_ID = "BOXID",
                CARRIER_ID = "CARRIERID",
                LOT_ID = "LOTID",
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
                (stub.zoneCommandBLL, stub.vehicleBLL, stub.sectionBLL, stub.portDefBLL, stub.reserveBLL,
                 stub.transferService, stub.shelfDefBLL, stub.cassetteDataBLL, stub.cmdBLL);

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
                (stub.zoneCommandBLL, stub.vehicleBLL, stub.sectionBLL, stub.portDefBLL, stub.reserveBLL,
                 stub.transferService, stub.shelfDefBLL, stub.cassetteDataBLL, stub.cmdBLL);
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
            stub.vehicleBLL.loadCyclingAndTransferReadyVhs(stub.cmdBLL).
                Where(v => v.VEHICLE_ID == "B7_OHBLINE3_CR2").FirstOrDefault().CUR_SEC_ID = "30141";

            setZoneCommandData(stub);

            LoopTransferEnhance loopTransferEnhance = new LoopTransferEnhance();
            loopTransferEnhance.Start
                (stub.zoneCommandBLL, stub.vehicleBLL, stub.sectionBLL, stub.portDefBLL, stub.reserveBLL,
                 stub.transferService, stub.shelfDefBLL, stub.cassetteDataBLL, stub.cmdBLL);
            //Act
            var result = loopTransferEnhance.tryGetZoneCommand(mcs_cmds, vhID, zoneCommandID);

            //Assert
            var assert_result = (true, "B7_OHBLINE3_A01", mcs_cmds[0]);
            result.Should().BeEquivalentTo(assert_result);
        }


        [Test]
        public void 車子上報ZoneCommandRequest_該Zone命令多筆_要找出最遠的一筆給他_1_0()
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
                (stub.zoneCommandBLL, stub.vehicleBLL, stub.sectionBLL, stub.portDefBLL, stub.reserveBLL,
                 stub.transferService, stub.shelfDefBLL, stub.cassetteDataBLL, stub.cmdBLL);
            //Act
            var result = loopTransferEnhance.tryGetZoneCommand(mcs_cmds, vhID, zoneCommandID);

            //Assert
            var assert_result = (true, "B7_OHBLINE3_A03", cmd_mcs2);
            result.Should().BeEquivalentTo(assert_result);

        }
        [Test]
        public void 車子上報ZoneCommandRequest_該Zone命令多筆_要找出最遠的一筆給他_N1_0()
        {
            //Arrange
            string vhID = "B7_OHBLINE3_CR1";
            string zoneCommandID = "ZC05";
            List<ACMD_MCS> mcs_cmds = new List<ACMD_MCS>();
            var cmd_mcs1 = bulidFackCMD_MCS("1", "103301", "B7_OHBLINE3-ZONE2");
            var cmd_mcs2 = bulidFackCMD_MCS("2", "102001", "B7_OHBLINE3-ZONE2");
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
                (stub.zoneCommandBLL, stub.vehicleBLL, stub.sectionBLL, stub.portDefBLL, stub.reserveBLL,
                 stub.transferService, stub.shelfDefBLL, stub.cassetteDataBLL, stub.cmdBLL);
            //Act
            var result = loopTransferEnhance.tryGetZoneCommand(mcs_cmds, vhID, zoneCommandID);

            //Assert
            var assert_result = (true, "103301", cmd_mcs1);
            result.Should().BeEquivalentTo(assert_result);

        }

        [Ignore("尚未寫好測試條件準備好")]
        public void 再找出對應的車子_並且回復車子有搬送命令後_將該命令改為PreInitial_命產生小命令()
        {
            //Arrange
            var cmd_mcs1 = bulidFackCMD_MCS("1", "B7_OHBLINE3_A01", "B7_OHBLINE3-ZONE2");

            var stub = GetStubObject();
            setSectionData(stub);
            setAddressData(stub);
            setVehicleData(stub);
            setZoneCommandData(stub);
            setPortDefData(stub);
            stub.sequenceBLL.
                getCommandID(com.mirle.ibg3k0.sc.App.SCAppConstants.GenOHxCCommandType.Auto).
                Returns("111");
            LoopTransferEnhance loopTransferEnhance = new LoopTransferEnhance();
            loopTransferEnhance.Start
                (stub.zoneCommandBLL, stub.vehicleBLL, stub.sectionBLL, stub.portDefBLL, stub.reserveBLL,
                 stub.transferService, stub.shelfDefBLL, stub.cassetteDataBLL, stub.cmdBLL);

            var vh = stub.vehicleBLL.getVehicle("B7_OHBLINE3_CR1");
            //Act
            var result = loopTransferEnhance.preAssignMCSCommand(stub.sequenceBLL, vh, cmd_mcs1);

            //Assert
            result.Should().BeTrue();
        }
        [Test]
        public void 針對準備預派給VH的命令_檢查是否有跑過頭_沒有跑過頭_Group方向1_0()
        {
            //Arrange
            var cmd_mcs1 = bulidFackCMD_MCS("1", "B7_OHBLINE3_A01", "B7_OHBLINE3-ZONE2");

            var stub = GetStubObject();
            setSectionData(stub);
            setAddressData(stub);
            setVehicleData(stub);
            setZoneCommandData(stub);
            setPortDefData(stub);
            setSeqNum(stub);
            LoopTransferEnhance loopTransferEnhance = new LoopTransferEnhance();
            loopTransferEnhance.Start
                (stub.zoneCommandBLL, stub.vehicleBLL, stub.sectionBLL, stub.portDefBLL, stub.reserveBLL,
                 stub.transferService, stub.shelfDefBLL, stub.cassetteDataBLL, stub.cmdBLL);
            //變更車子的位置
            var vh2 = stub.vehicleBLL.loadCyclingAndTransferReadyVhs(stub.cmdBLL).
                 Where(v => v.VEHICLE_ID == "B7_OHBLINE3_CR2").FirstOrDefault();
            vh2.X_Axis = 9000;


            //Act
            var result = loopTransferEnhance.IsRunOver(vh2, cmd_mcs1.HOSTSOURCE);

            //Assert
            result.Should().BeFalse();
        }
        [Test]
        public void 針對準備預派給VH的命令_檢查是否有跑過頭_跑過頭_Group方向1_0()
        {
            //Arrange
            var cmd_mcs1 = bulidFackCMD_MCS("1", "B7_OHBLINE3_A01", "B7_OHBLINE3-ZONE2");

            var stub = GetStubObject();
            setSectionData(stub);
            setAddressData(stub);
            setVehicleData(stub);
            setZoneCommandData(stub);
            setPortDefData(stub);
            setSeqNum(stub);
            LoopTransferEnhance loopTransferEnhance = new LoopTransferEnhance();
            loopTransferEnhance.Start
                (stub.zoneCommandBLL, stub.vehicleBLL, stub.sectionBLL, stub.portDefBLL, stub.reserveBLL,
                 stub.transferService, stub.shelfDefBLL, stub.cassetteDataBLL, stub.cmdBLL);
            //變更車子的位置
            var vh2 = stub.vehicleBLL.loadCyclingAndTransferReadyVhs(stub.cmdBLL).
                 Where(v => v.VEHICLE_ID == "B7_OHBLINE3_CR2").FirstOrDefault();
            vh2.X_Axis = 10000;


            //Act
            var result = loopTransferEnhance.IsRunOver(vh2, cmd_mcs1.HOSTSOURCE);

            //Assert
            result.Should().BeTrue();
        }
        [Test]
        public void 針對準備預派給VH的命令_檢查是否有跑過頭_沒有跑過頭_Group方向N1_0()
        {
            //Arrange
            var cmd_mcs1 = bulidFackCMD_MCS("1", "B7_OHBlOOP_T09", "B7_OHBLINE3-ZONE2");

            var stub = GetStubObject();
            setSectionData(stub);
            setAddressData(stub);
            setVehicleData(stub);
            setZoneCommandData(stub);
            setPortDefData(stub);
            setSeqNum(stub);
            LoopTransferEnhance loopTransferEnhance = new LoopTransferEnhance();
            loopTransferEnhance.Start
                (stub.zoneCommandBLL, stub.vehicleBLL, stub.sectionBLL, stub.portDefBLL, stub.reserveBLL,
                 stub.transferService, stub.shelfDefBLL, stub.cassetteDataBLL, stub.cmdBLL);
            //變更車子的位置
            var vh2 = stub.vehicleBLL.loadCyclingAndTransferReadyVhs(stub.cmdBLL).
                 Where(v => v.VEHICLE_ID == "B7_OHBLINE3_CR2").FirstOrDefault();
            vh2.X_Axis = 17000;


            //Act
            var result = loopTransferEnhance.IsRunOver(vh2, cmd_mcs1.HOSTSOURCE);

            //Assert
            result.Should().BeFalse();
        }
        [Test]
        public void 針對準備預派給VH的命令_檢查是否有跑過頭_有跑過頭_Group方向N1_0()
        {
            //Arrange
            var cmd_mcs1 = bulidFackCMD_MCS("1", "B7_OHBlOOP_T09", "B7_OHBLINE3-ZONE2");

            var stub = GetStubObject();
            setSectionData(stub);
            setAddressData(stub);
            setVehicleData(stub);
            setZoneCommandData(stub);
            setPortDefData(stub);
            setSeqNum(stub);
            LoopTransferEnhance loopTransferEnhance = new LoopTransferEnhance();
            loopTransferEnhance.Start
                (stub.zoneCommandBLL, stub.vehicleBLL, stub.sectionBLL, stub.portDefBLL, stub.reserveBLL,
                 stub.transferService, stub.shelfDefBLL, stub.cassetteDataBLL, stub.cmdBLL);
            //變更車子的位置
            var vh2 = stub.vehicleBLL.loadCyclingAndTransferReadyVhs(stub.cmdBLL).
                 Where(v => v.VEHICLE_ID == "B7_OHBLINE3_CR2").FirstOrDefault();
            vh2.X_Axis = 13000;


            //Act
            var result = loopTransferEnhance.IsRunOver(vh2, cmd_mcs1.HOSTSOURCE);

            //Assert
            result.Should().BeTrue();
        }
        [Test]
        public void 車子上報Load_Unload完成後_確認該Zone有無可順途搬送的命令_該Zone有1筆後面沒車尚未超過()
        {
            //Arrange
            string vhID = "B7_OHBLINE3_CR1";
            string zoneCommandID = "ZC01";
            List<ACMD_MCS> mcs_cmds = new List<ACMD_MCS>();
            var cmd_mcs1 = bulidFackCMD_MCS("1", "B7_OHBLINE3_A01", "B7_OHBLINE3-ZONE2");
            mcs_cmds.Add(cmd_mcs1);

            var stub = GetStubObject();
            setSectionData(stub);
            setAddressData(stub);
            setVehicleData(stub);
            setZoneCommandData(stub);
            setPortDefData(stub);
            LoopTransferEnhance loopTransferEnhance = new LoopTransferEnhance();
            loopTransferEnhance.Start
                (stub.zoneCommandBLL, stub.vehicleBLL, stub.sectionBLL, stub.portDefBLL, stub.reserveBLL,
                 stub.transferService, stub.shelfDefBLL, stub.cassetteDataBLL, stub.cmdBLL);

            //變更車子的位置
            var vh2 = stub.vehicleBLL.loadCyclingAndTransferReadyVhs(stub.cmdBLL).
                 Where(v => v.VEHICLE_ID == "B7_OHBLINE3_CR2").FirstOrDefault();
            vh2.CUR_SEC_ID = "30147";
            var vh1 = stub.vehicleBLL.loadCyclingAndTransferReadyVhs(stub.cmdBLL).
                 Where(v => v.VEHICLE_ID == "B7_OHBLINE3_CR1").FirstOrDefault();
            vh1.X_Axis = 6794;
            vh1.CUR_ADR_ID = "12272";
            vh1.CUR_SEC_ID = "30143";

            //Act
            var result = loopTransferEnhance.tryGetZoneCommandWhenCommandComplete(mcs_cmds,new List<ACMD_MCS>(), vhID);

            //Assert
            var assert_result = (true, "B7_OHBLINE3_A01", cmd_mcs1);
            result.Should().BeEquivalentTo(assert_result);
        }
        [Test]
        public void 車子上報Load_Unload完成後_確認該Zone有無可順途搬送的命令_該Zone有1筆後面沒車但已經超過搬送的Port()
        {
            //Arrange
            string vhID = "B7_OHBLINE3_CR1";
            string zoneCommandID = "ZC01";
            List<ACMD_MCS> mcs_cmds = new List<ACMD_MCS>();
            var cmd_mcs1 = bulidFackCMD_MCS("1", "B7_OHBLINE3_A01", "B7_OHBLINE3-ZONE2");
            mcs_cmds.Add(cmd_mcs1);

            var stub = GetStubObject();
            setSectionData(stub);
            setAddressData(stub);
            setVehicleData(stub);
            setZoneCommandData(stub);
            setPortDefData(stub);
            LoopTransferEnhance loopTransferEnhance = new LoopTransferEnhance();
            loopTransferEnhance.Start
                (stub.zoneCommandBLL, stub.vehicleBLL, stub.sectionBLL, stub.portDefBLL, stub.reserveBLL,
                 stub.transferService, stub.shelfDefBLL, stub.cassetteDataBLL, stub.cmdBLL);

            //變更車子的位置
            var vh2 = stub.vehicleBLL.loadCyclingAndTransferReadyVhs(stub.cmdBLL).
                 Where(v => v.VEHICLE_ID == "B7_OHBLINE3_CR2").FirstOrDefault();
            vh2.CUR_SEC_ID = "30139";
            var vh1 = stub.vehicleBLL.loadCyclingAndTransferReadyVhs(stub.cmdBLL).
                 Where(v => v.VEHICLE_ID == "B7_OHBLINE3_CR1").FirstOrDefault();
            vh1.X_Axis = 10280;
            vh1.CUR_ADR_ID = "13280";
            vh1.CUR_SEC_ID = "30151";

            //Act
            var result = loopTransferEnhance.tryGetZoneCommandWhenCommandComplete(mcs_cmds, new List<ACMD_MCS>(), vhID);

            //Assert
            var assert_result = (false, "", default(ACMD_MCS));
            result.Should().BeEquivalentTo(assert_result);
        }
    }
}