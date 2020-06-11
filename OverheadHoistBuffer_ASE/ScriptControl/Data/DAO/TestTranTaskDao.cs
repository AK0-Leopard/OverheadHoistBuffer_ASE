using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace com.mirle.ibg3k0.sc.Data.DAO
{
    public class TestTranTaskDao
    {

        public TestTranTaskDao()
        {
        }

        public List<TranTask> loadTransferTasks_ACycle(BLL.PortStationBLL portStationBLL, DataTable TranCmdPeriodicDataTable)
        {
            List<TranTask> lstTranTask = new List<TranTask>();
            var query = TranCmdPeriodicDataTable.AsEnumerable();
            foreach (DataRow row in query)
            {
                for (int i = 0; i < row.ItemArray.Count(); i++)
                {
                    if (i == 0)
                        continue; //跳過第一行
                    string cellData = row[i].ToString();
                    double icellData = 0;
                    if (double.TryParse(cellData, out icellData))
                    {
                        icellData = Math.Round(icellData, 0);
                    }
                    if (icellData != 0)
                    {
                        string source_port = row[0].ToString();
                        string destination_port = row.Table.Columns[i].ColumnName;
                        if (Common.SCUtility.isMatche(source_port, destination_port))
                            continue;
                        string car_type = string.Empty;
                        var source_port_station = portStationBLL.OperateCatch.getPortStation(source_port);
                        var target_port_station = portStationBLL.OperateCatch.getPortStation(destination_port);
                        if (source_port_station != null)
                        {
                            if (source_port_station.LD_VH_TYPE == E_VH_TYPE.Clean ||
                                target_port_station == null)
                                car_type = ((int)source_port_station.LD_VH_TYPE).ToString();
                        }
                        lstTranTask.Add(new TranTask()
                        {
                            SourcePort = source_port,
                            DestinationPort = destination_port,
                            EachPeriodicCount = icellData.ToString(),
                            CarType = car_type
                        });
                    }
                }
            }
            return lstTranTask;
        }

        public List<TranTask> loadTransferTasks_24Hour(DataTable TranCmdPeriodicDataTable)
        {
            List<TranTask> lstTranTask = new List<TranTask>();

            var query = TranCmdPeriodicDataTable.AsEnumerable();
            foreach (DataRow row in query)
            {
                string source = row["FROM"].ToString();
                string destination = row["TO"].ToString();
                if (string.IsNullOrWhiteSpace(source) ||
                    string.IsNullOrWhiteSpace(destination))
                    continue;
                string sMin = row["MIN"].ToString();
                string CarType = row["CARTYPE"].ToString();
                Double iMin = 0;
                Double.TryParse(sMin, out iMin);
                iMin = Math.Round(iMin, 0, MidpointRounding.AwayFromZero);
                lstTranTask.Add(new TranTask()
                {
                    Min = (int)iMin,
                    SourcePort = source,
                    DestinationPort = destination,
                    CarType = CarType
                });

            }
            return lstTranTask;
        }
        //public List<TranTask> loadTransferTasks_245Hour()
        //{
        //    List<TranTask> lstTranTask = new List<TranTask>();
        //    Dictionary<int, List<TranTask>> dicTemp = null;
        //    var query = from row in TranCmdTaskDataSet.Tables[1].AsEnumerable()
        //                group row by row.Field<string>("MIN");
        //    dicTemp = query.ToDictionary(item => item.Key, item => new TranTask { SourcePort = item. });

        //    var query = TranCmdTaskDataSet.Tables[1].AsEnumerable();

        //    foreach (DataRow row in query)
        //    {
        //        lstTranTask.Add(new TranTask()
        //        {
        //            SourcePort = row[2].ToString(),
        //            DestinationPort = row[3].ToString()
        //        });
        //    }
        //    return lstTranTask;
        //}

    }

    public class TranTask
    {
        private int min;
        public int Min { get { return min; } set { min = value; } }
        private string sourceport;
        public string SourcePort { get { return sourceport; } set { sourceport = value; } }
        private string destinationport;
        public string DestinationPort { get { return destinationport; } set { destinationport = value; } }
        private string eachperiodiccount;
        public string EachPeriodicCount { get { return eachperiodiccount; } set { eachperiodiccount = value; } }
        private string cartype;
        public string CarType { get { return cartype; } set { cartype = value; } }
    }

}
