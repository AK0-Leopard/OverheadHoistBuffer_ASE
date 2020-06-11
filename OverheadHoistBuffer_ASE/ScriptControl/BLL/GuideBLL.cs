using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using com.mirle.ibg3k0.sc.RouteKit;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.BLL
{
    public class GuideBLL
    {
        SCApplication scApp;
        Logger logger = LogManager.GetCurrentClassLogger();

        public void start(SCApplication _scApp)
        {
            scApp = _scApp;
        }
        public (bool isSuccess, List<string> guideSegmentIds, List<string> guideSectionIds, List<string> guideAddressIds, int totalCost)
            getGuideInfo(string startAddress, string targetAddress, List<string> byPassSectionIDs = null)
        {
            if (SCUtility.isMatche(startAddress, targetAddress))
            {
                return (true, new List<string>(), new List<string>(), new List<string>(), 0);
            }

            bool is_success = false;
            int.TryParse(startAddress, out int i_start_address);
            int.TryParse(targetAddress, out int i_target_address);

            List<RouteInfo> stratFromRouteInfoList = null;
            if (byPassSectionIDs == null || byPassSectionIDs.Count == 0)
            {
                stratFromRouteInfoList = scApp.NewRouteGuide.getFromToRoutesAddrToAddr(i_start_address, i_target_address);
            }
            else
            {
                stratFromRouteInfoList = scApp.NewRouteGuide.getFromToRoutesAddrToAddr(i_start_address, i_target_address, byPassSectionIDs);
            }
            RouteInfo min_stratFromRouteInfo = null;
            if (stratFromRouteInfoList != null && stratFromRouteInfoList.Count > 0)
            {
                min_stratFromRouteInfo = stratFromRouteInfoList.First();
                is_success = true;
            }

            return (is_success, null, min_stratFromRouteInfo.GetSectionIDs(), min_stratFromRouteInfo.GetAddressesIDs(), min_stratFromRouteInfo.total_cost);
        }



        public (bool isSuccess, int distance) IsRoadWalkable(string startAddress, string targetAddress)
        {
            try
            {
                if (SCUtility.isMatche(startAddress, targetAddress))
                    return (true, 0);

                var guide_info = getGuideInfo(startAddress, targetAddress);
                //if ((guide_info.guideAddressIds != null && guide_info.guideAddressIds.Count != 0) &&
                //    ((guide_info.guideSectionIds != null && guide_info.guideSectionIds.Count != 0)))
                if (guide_info.isSuccess)
                {
                    return (true, guide_info.totalCost);
                }
                else
                {
                    return (false, int.MaxValue);
                }
            }
            catch
            {
                return (false, int.MaxValue);
            }
        }
        public int GetDistance(string startAddress, string targetAddress)
        {
            try
            {
                if (SCUtility.isMatche(startAddress, targetAddress))
                    return 0;

                var guide_info = getGuideInfo(startAddress, targetAddress);
                //if ((guide_info.guideAddressIds != null && guide_info.guideAddressIds.Count != 0) &&
                //    ((guide_info.guideSectionIds != null && guide_info.guideSectionIds.Count != 0)))
                if (guide_info.isSuccess)
                {
                    return guide_info.totalCost;
                }
                else
                {
                    return int.MaxValue;
                }
            }
            catch
            {
                return int.MaxValue;
            }
        }

    }

}

