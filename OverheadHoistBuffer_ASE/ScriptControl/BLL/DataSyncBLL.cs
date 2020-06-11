using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data;
using com.mirle.ibg3k0.sc.Data.DAO;
using com.mirle.ibg3k0.sc.Data.DAO.EntityFramework;
using com.mirle.ibg3k0.sc.Data.SECS;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace com.mirle.ibg3k0.sc.BLL
{
    public class DataSyncBLL
    {
        ViewSectionDao VSection100Dao = null;
        SegmentDao SegmentDao = null;
        AddressDataDao AddressDataDao = null;
        ScaleBaseDataDao ScaleBaseDataDao = null;
        ControlDataDao ControlDataDao = null;
        VehicleControlDao VehicleControlDao = null;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public DataSyncBLL()
        {

        }
        public void start(SCApplication app)
        {
            VSection100Dao = app.VSection100Dao;
            AddressDataDao = app.AddressDataDao;
            ScaleBaseDataDao = app.ScaleBaseDataDao;
            ControlDataDao = app.ControlDataDao;
            VehicleControlDao = app.VehicleControlDao;
        }

        public void start(ViewSectionDao viewSectionDao, SegmentDao segmentDao, AddressDataDao addressDataDao, ScaleBaseDataDao scaleBaseDataDao,
                          ControlDataDao controlDataDao, VehicleControlDao vehicleControlDao)
        {
            VSection100Dao = viewSectionDao;
            AddressDataDao = addressDataDao;
            ScaleBaseDataDao = scaleBaseDataDao;
            ControlDataDao = controlDataDao;
            VehicleControlDao = vehicleControlDao;
            SegmentDao = segmentDao;
        }

        #region Address
        public List<AADDRESS_DATA> loadAllReleaseAddress_Data()
        {
            List<AADDRESS_DATA> adrs = null;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                adrs = AddressDataDao.loadAllReleaseData(con);
            }
            return adrs;
        }
        public void updateAddressData(string vh_id, string adr_id, int resolution)
        {
            AADDRESS_DATA adr_data = null;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                adr_data = AddressDataDao.getAddressData(con, vh_id, adr_id);
                adr_data.LOACTION = resolution;
                AddressDataDao.update(con, adr_data);
            }
        }

        public const string COMMON_ADDRESS_DATA_INDEX = "OHx";
        public List<AADDRESS_DATA> loadReleaseADDRESS_DATAs(string vh_id)
        {
            List<AADDRESS_DATA> adrs = null;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                adrs = AddressDataDao.loadReleaseDataByVhID(con, vh_id);
            }
            return adrs;
        }
        #endregion Address
        #region section
        public List<VSECTION_100> loadReleaseVSections()
        {
            List<VSECTION_100> vSecs = null;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                vSecs = VSection100Dao.loadReleaseData(con);
            }
            return vSecs;
        }

        public int getCount_ReleaseVSections()
        {
            int count = 0;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                count = VSection100Dao.getReleaseDataCount(con);
            }
            return count;
        }

        public void resetSectionTeachingResult()
        {

        }
        #endregion section


        public SCALE_BASE_DATA getReleaseSCALE_BASE_DATA()
        {
            SCALE_BASE_DATA data = null;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                data = ScaleBaseDataDao.getReleaseData(con);
            }
            return data;
        }

        public CONTROL_DATA getReleaseCONTROL_DATA()
        {
            CONTROL_DATA data = null;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                data = ControlDataDao.getReleaseData(con);
            }
            return data;
        }
        public AVEHICLE_CONTROL_100 getReleaseVehicleControlData_100(string vh_id)
        {
            AVEHICLE_CONTROL_100 data = null;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                data = VehicleControlDao.getReleaseDataByVhID(con, vh_id);
            }
            return data;
        }
        public List<AVEHICLE_CONTROL_100> loadAllReleaseVehicleControlData_100()
        {
            List<AVEHICLE_CONTROL_100> datas = null;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                datas = VehicleControlDao.loadAllReleaseData(con);
            }
            return datas;
        }
        #region Segment Data
        public List<ASEGMENT> loadAllSegments()
        {
            List<ASEGMENT> data = null;
            using (DBConnection_EF con = DBConnection_EF.GetUContext())
            {
                data = SegmentDao.loadAllSegments(con);
            }
            return data;
        }
        #endregion Segment Data


    }
}
