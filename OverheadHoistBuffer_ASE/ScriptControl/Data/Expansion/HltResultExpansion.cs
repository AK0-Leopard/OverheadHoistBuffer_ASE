using Mirle.Protos.ReserveModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.Data.Expansion
{
    public static class ResultExpansion
    {
        public static Mirle.Hlts.Utils.HltResult convert2HltResult(this Result hltResult)
        {
            return new Mirle.Hlts.Utils.HltResult()
            {
                OK = hltResult.Ok,
                VehicleID = string.IsNullOrWhiteSpace(hltResult.VehicleId) ? "" : hltResult.VehicleId,
                SectionID = string.IsNullOrWhiteSpace(hltResult.SectionId) ? "" : hltResult.SectionId,
                Description = string.IsNullOrWhiteSpace(hltResult.Description) ? "" : hltResult.Description
            };

        }
    }
}
