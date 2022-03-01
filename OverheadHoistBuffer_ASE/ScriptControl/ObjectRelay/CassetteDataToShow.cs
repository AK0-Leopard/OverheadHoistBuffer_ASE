using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.ObjectRelay
{
    public class CassetteDataToShow
    {
        public CassetteData cstDataObj = null;
        public BLL.Interface.IZoneCommandBLL zoneCommandBLL = null;
        public Service.Interface.ITransferService transferService = null;
        public CassetteDataToShow(CassetteData _cstDataObj, BLL.Interface.IZoneCommandBLL _zoneCommandBLL, Service.Interface.ITransferService _transferService)
        {
            cstDataObj = _cstDataObj;
            zoneCommandBLL = _zoneCommandBLL;
            transferService = _transferService;
        }
        public string ALAM_CODE { get { return cstDataObj.StockerID; } }
        public string CSTID { get { return cstDataObj.CSTID; } }
        public string BOXID { get { return cstDataObj.BOXID; } }
        public string Carrier_LOC { get { return cstDataObj.Carrier_LOC; } }
        public decimal Stage { get { return cstDataObj.Stage; } }
        public E_CSTState CSTState { get { return cstDataObj.CSTState; } }
        public string LotID { get { return cstDataObj.LotID; } }
        public string EmptyCST { get { return cstDataObj.EmptyCST; } }
        public string CSTType { get { return cstDataObj.CSTType; } }
        public string CSTInDT { get { return cstDataObj.CSTInDT; } }
        public string StoreDT { get { return cstDataObj.StoreDT; } }
        public string WaitOutOPDT { get { return cstDataObj.WaitOutOPDT; } }
        public string WaitOutLPDT { get { return cstDataObj.WaitOutLPDT; } }
        public string TrnDT { get { return cstDataObj.TrnDT; } }
        public string ReadStatus { get { return cstDataObj.ReadStatus; } }

        public string ZCID
        {
            get
            {
                var get_result = zoneCommandBLL.tryGetZoneCommandGroupByPortID(Carrier_LOC);
                if (!get_result.hasFind)
                {
                    return "";
                }
                return get_result.zoneCommandGroup.ZoneCommandID;
            }
        }

        public string Zone
        {
            get
            {
                var get_result = transferService.tryGetShelfZoneName(Carrier_LOC);
                if (!get_result.isExist)
                {
                    return "";
                }
                return get_result.zoneName;
            }
        }


    }
}
