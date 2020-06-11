using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.BLL;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.Data.SECS;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using com.mirle.iibg3k0.ttc.Common;
using Google.Protobuf;
using KingAOP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace com.mirle.ibg3k0.sc.Service
{
    public class BlockControlService
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        MapBLL mapBLL = null;
        SectionBLL sectionBLL = null;

        VehicleService vehicleService = null;
        public BlockControlService()
        {

        }
        public void start(SCApplication app)
        {
            mapBLL = app.MapBLL;
            sectionBLL = app.SectionBLL;
            vehicleService = app.VehicleService;

            RegisterReleaseAddressOfKeySection();

        }

        /// <summary>
        /// 用來註冊可能可以釋放Block的Section路段
        /// 當觸發的Leave Section，就會使用To Address來確認是否為某的Block的釋放點
        /// 當觸發的Entry Section，就會使用From Address來確認是否為某的Block的釋放點
        /// </summary>
        private void RegisterReleaseAddressOfKeySection()
        {
            List<ABLOCKZONEMASTER> block_zone_masters = mapBLL.loadAllBlockZoneMaster();
            List<ASECTION> from_adr_of_sections = new List<ASECTION>();
            List<ASECTION> to_adr_of_sections = new List<ASECTION>();
            foreach (var block_zone_master in block_zone_masters)
            {
                if (!SCUtility.isEmpty(block_zone_master.LEAVE_ADR_ID_1))
                {
                    from_adr_of_sections.AddRange(sectionBLL.cache.GetSectionsByFromAddress(block_zone_master.LEAVE_ADR_ID_1));
                    to_adr_of_sections.AddRange(sectionBLL.cache.GetSectionsByToAddress(block_zone_master.LEAVE_ADR_ID_1));
                }
                if (!SCUtility.isEmpty(block_zone_master.LEAVE_ADR_ID_2))
                {
                    from_adr_of_sections.AddRange(sectionBLL.cache.GetSectionsByFromAddress(block_zone_master.LEAVE_ADR_ID_2));
                    to_adr_of_sections.AddRange(sectionBLL.cache.GetSectionsByToAddress(block_zone_master.LEAVE_ADR_ID_2));
                }
            }
            from_adr_of_sections = from_adr_of_sections.Distinct().ToList();
            foreach (ASECTION from_adr_of_section in from_adr_of_sections)
            {
                from_adr_of_section.VehicleEntry += From_adr_of_section_VehicleEntry;
            }
            to_adr_of_sections = to_adr_of_sections.Distinct().ToList();
            foreach (ASECTION to_adr_of_section in to_adr_of_sections)
            {
                to_adr_of_section.VehicleLeave += To_adr_of_section_VehicleLeave;
            }

        }
        private void To_adr_of_section_VehicleLeave(object sender, string e)
        {
            try
            {
                string vh_id = e;
                ASECTION leave_section = sender as ASECTION;
                string leave_section_of_to_adr_id = leave_section.TO_ADR_ID;
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(BlockControlService), Device: VehicleService.DEVICE_NAME_OHx,
                   Data: $"Start try force release block by leave section:{leave_section.SEC_ID} of to adr id:{leave_section_of_to_adr_id}",
                   VehicleID: vh_id);
                forceReleaseBlock(vh_id, leave_section_of_to_adr_id);
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn,
                   Class: nameof(BlockControlService), Device: VehicleService.DEVICE_NAME_OHx,
                   Data: ex,
                   VehicleID: e);
            }
        }
        private void From_adr_of_section_VehicleEntry(object sender, string e)
        {
            try
            {
                string vh_id = e;
                ASECTION entry_section = sender as ASECTION;
                string entry_section_of_from_adr_id = entry_section.FROM_ADR_ID;
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Debug, Class: nameof(BlockControlService), Device: VehicleService.DEVICE_NAME_OHx,
                   Data: $"Start try force release block by entry section:{entry_section.SEC_ID} of from adr id:{entry_section_of_from_adr_id}",
                   VehicleID: vh_id);
                forceReleaseBlock(vh_id, entry_section_of_from_adr_id);
            }
            catch (Exception ex)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn,
                   Class: nameof(BlockControlService), Device: VehicleService.DEVICE_NAME_OHx,
                   Data: ex,
                   VehicleID: e);
            }
        }
        private void forceReleaseBlock(string vhID, string checkAddress)
        {
            var release_result = vehicleService.doBlockRelease(vhID, checkAddress, false);
            if (release_result.hasRelease)
            {
                LogHelper.Log(logger: logger, LogLevel: LogLevel.Warn, Class: nameof(BlockControlService), Device: VehicleService.DEVICE_NAME_OHx,
                   Data: $"Process block force release by ohxc, release address id:{checkAddress}, " +
                         $"release entry section id:{release_result.releaseBlockMaster.ENTRY_SEC_ID}",
                   VehicleID: vhID);
            }
        }

    }
}
