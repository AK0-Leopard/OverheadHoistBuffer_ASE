using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc
{
    public partial class ACMD_OHTC
    {
        public static ConcurrentDictionary<string, ACMD_OHTC> CMD_OHTC_InfoList { get; private set; } = new ConcurrentDictionary<string, ACMD_OHTC>();
        public static List<string> loadCmdOhtcListOfCmdID()
        {
            var array_cmd_ohtc = CMD_OHTC_InfoList.ToArray();
            return array_cmd_ohtc.Select(cmd => cmd.Key).ToList();
        }
        public static List<ACMD_OHTC> loadCmdOhtcListOfCmdObj()
        {
            var array_cmd_ohtc = CMD_OHTC_InfoList.ToArray();
            return array_cmd_ohtc.Select(cmd => cmd.Value).ToList();
        }
        public bool IsCMD_MCS()
        {
            return !sc.Common.SCUtility.isEmpty(CMD_ID_MCS);
        }


        public HCMD_OHTC ToHCMD_OHTC()
        {
            return new HCMD_OHTC()
            {
                CMD_ID = this.CMD_ID,
                VH_ID = this.VH_ID,
                CARRIER_ID = this.CARRIER_ID,
                CMD_ID_MCS = this.CMD_ID_MCS,
                CMD_TPYE = this.CMD_TPYE,
                SOURCE = this.SOURCE,
                DESTINATION = this.DESTINATION,
                PRIORITY = this.PRIORITY,
                CMD_START_TIME = this.CMD_START_TIME,
                CMD_END_TIME = this.CMD_END_TIME,
                CMD_STAUS = this.CMD_STAUS,
                CMD_PROGRESS = this.CMD_PROGRESS,
                INTERRUPTED_REASON = this.INTERRUPTED_REASON,
                ESTIMATED_TIME = this.ESTIMATED_TIME,
                ESTIMATED_EXCESS_TIME = this.ESTIMATED_EXCESS_TIME,
                REAL_CMP_TIME = this.REAL_CMP_TIME,
                SOURCE_ADR = this.SOURCE_ADR,
                DESTINATION_ADR = this.DESTINATION_ADR,
                BOX_ID = this.BOX_ID,
                LOT_ID = this.LOT_ID,
                CMD_INSER_TIME =
                this.CMD_INSER_TIME.HasValue ? this.CMD_INSER_TIME.Value : DateTime.Now,
                COMPLETE_STATUS = this.COMPLETE_STATUS
            };
        }

        public override string ToString()
        {
            return $"command id:{Common.SCUtility.Trim(CMD_ID, true)}, mcs cmd id:{Common.SCUtility.Trim(CMD_ID_MCS, true)}, cmd type:{CMD_TPYE}, source:{Common.SCUtility.Trim(SOURCE, true)}, dest:{Common.SCUtility.Trim(DESTINATION, true)}, status:{CMD_STAUS}," +
                   $" start time:{CMD_START_TIME?.ToString(App.SCAppConstants.DateTimeFormat_19)}, end time:{CMD_END_TIME?.ToString(App.SCAppConstants.DateTimeFormat_19)}";
        }

        public bool put(ACMD_OHTC ortherObj)
        {
            bool has_change = false;
            if (!sc.Common.SCUtility.isMatche(CMD_ID, ortherObj.CMD_ID))
            {
                CMD_ID = ortherObj.CMD_ID;
                has_change = true;
            }
            if (!sc.Common.SCUtility.isMatche(VH_ID, ortherObj.VH_ID))
            {
                VH_ID = ortherObj.VH_ID;
                has_change = true;
            }
            if (!sc.Common.SCUtility.isMatche(CARRIER_ID, ortherObj.CARRIER_ID))
            {
                CARRIER_ID = ortherObj.CARRIER_ID;
                has_change = true;
            }
            if (!sc.Common.SCUtility.isMatche(CMD_ID_MCS, ortherObj.CMD_ID_MCS))
            {
                CMD_ID_MCS = ortherObj.CMD_ID_MCS;
                has_change = true;
            }
            if (CMD_TPYE != ortherObj.CMD_TPYE)
            {
                CMD_TPYE = ortherObj.CMD_TPYE;
                has_change = true;
            }
            if (!sc.Common.SCUtility.isMatche(SOURCE, ortherObj.SOURCE))
            {
                SOURCE = ortherObj.SOURCE;
                has_change = true;
            }
            if (!sc.Common.SCUtility.isMatche(DESTINATION, ortherObj.DESTINATION))
            {
                DESTINATION = ortherObj.DESTINATION;
                has_change = true;
            }
            if (PRIORITY != ortherObj.PRIORITY)
            {
                PRIORITY = ortherObj.PRIORITY;
                has_change = true;
            }
            if (CMD_START_TIME != ortherObj.CMD_START_TIME)
            {
                CMD_START_TIME = ortherObj.CMD_START_TIME;
                has_change = true;
            }

            if (CMD_END_TIME != ortherObj.CMD_END_TIME)
            {
                CMD_END_TIME = ortherObj.CMD_END_TIME;
                has_change = true;
            }
            if (CMD_STAUS != ortherObj.CMD_STAUS)
            {
                CMD_STAUS = ortherObj.CMD_STAUS;
                has_change = true;
            }
            if (CMD_PROGRESS != ortherObj.CMD_PROGRESS)
            {
                CMD_PROGRESS = ortherObj.CMD_PROGRESS;
                has_change = true;
            }
            if (INTERRUPTED_REASON != ortherObj.INTERRUPTED_REASON)
            {
                INTERRUPTED_REASON = ortherObj.INTERRUPTED_REASON;
                has_change = true;
            }
            if (ESTIMATED_TIME != ortherObj.ESTIMATED_TIME)
            {
                ESTIMATED_TIME = ortherObj.ESTIMATED_TIME;
                has_change = true;
            }
            if (ESTIMATED_EXCESS_TIME != ortherObj.ESTIMATED_EXCESS_TIME)
            {
                ESTIMATED_EXCESS_TIME = ortherObj.ESTIMATED_EXCESS_TIME;
                has_change = true;
            }
            if (REAL_CMP_TIME != ortherObj.REAL_CMP_TIME)
            {
                REAL_CMP_TIME = ortherObj.REAL_CMP_TIME;
                has_change = true;
            }
            if (!sc.Common.SCUtility.isMatche(SOURCE_ADR, ortherObj.SOURCE_ADR))
            {
                SOURCE_ADR = ortherObj.SOURCE_ADR;
                has_change = true;
            }
            if (!sc.Common.SCUtility.isMatche(DESTINATION_ADR, ortherObj.DESTINATION_ADR))
            {
                DESTINATION_ADR = ortherObj.DESTINATION_ADR;
                has_change = true;
            }
            if (!sc.Common.SCUtility.isMatche(BOX_ID, ortherObj.BOX_ID))
            {
                BOX_ID = ortherObj.BOX_ID;
                has_change = true;
            }
            if (!sc.Common.SCUtility.isMatche(LOT_ID, ortherObj.LOT_ID))
            {
                LOT_ID = ortherObj.LOT_ID;
                has_change = true;
            }
            if (!sc.Common.SCUtility.isMatche(CMD_INSER_TIME, ortherObj.CMD_INSER_TIME))
            {
                CMD_INSER_TIME = ortherObj.CMD_INSER_TIME;
                has_change = true;
            }
            return has_change;
        }
    }
}
