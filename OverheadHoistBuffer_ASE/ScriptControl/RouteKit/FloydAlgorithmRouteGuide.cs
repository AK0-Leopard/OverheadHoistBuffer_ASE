//*********************************************************************************
//      FloydAlgorithmRouteGuide.cs
//*********************************************************************************
// File Name: FloydAlgorithmRouteGuide.cs
// Description: 
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2019/06/11    Mark Chou      N/A            M0.01   把StartFromTo路徑計算與相關API移除，避免Memory使用過多問題
// 2019/06/11    Mark Chou      N/A            M0.02   優化修改計算Intercost的程式碼。
// 2019/07/11    Mark Chou      N/A            M0.03   移除Section To Section、Section To Node、Node To Section的路徑計算，節省運算時間與記憶體。
// 2019/08/04    Kevin Wei      N/A            M0.04   修正計算路徑的方式，改換成平行處理來加快運算。
// 2019/08/04    Kevin Wei      N/A            M0.05   修改權重的比例，由原本的0.7改為0.1
// 2019/12/19    Mark Chou      N/A            M0.06   加入Dijkstra演算法，執行階段才進行路徑計算。
// 2019/12/23    Mark Chou      N/A            M0.07   Dijkstra路徑計算，加入可單次禁止路徑計算。
//**********************************************************************************
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.RouteKit
{
    public class FloydAlgorithmRouteGuide : IRouteGuide
    {
        int n;//節點數量
        int max_path_keep_num = 100;
        int[,] graph;
        string algorithm;
        readonly double move_cost_forward = 0;
        readonly double move_cost_reverse = 0;

        int[][] cost_matrix = null;
        public TimeWindow timewindow;
        List<InterCost> interCostsList = null;
        List<Section> sectionList = null;
        PathInfos[][] pathInfo_matrix = null;
        //M0.03 PathInfos[][] pathInfo_forSecToSec_matrix = null;
        //M0.03 PathInfos[][] pathInfo_forSecToNode_matrix = null;
        //M0.03 PathInfos[][] pathInfo_forNodeToSec_matrix = null;
        Dictionary<int, int> addressIndexDic;
        Dictionary<int, int> IndexAddressDic;
        Dictionary<int, int> sectionIndexDic;
        Dictionary<int, int> nonNodeAddressBelongSectonDic;

        List<List<int>> banPatternList = null;//設定被Ban的Section會紀錄在該list裡面
        List<List<int>> banEndPatternList = null;//由於AGV物理條件上的限制，某些Section不能作為路徑的結尾，這些Section會紀錄在這個List裡面。
        List<List<int>> alternativePathConfigList = null;
        string BCID = string.Empty;



        public FloydAlgorithmRouteGuide(List<ASECTION> sections,
            double moveCostForward, double moveCostReverse, string bcID, string algorithm)
        {
            this.algorithm = algorithm;
            move_cost_forward = moveCostForward;
            move_cost_reverse = moveCostReverse;
            BCID = bcID;
            banEndPatternList = new List<List<int>>();
            banPatternList = new List<List<int>>();
            alternativePathConfigList = new List<List<int>>();
            List<int> alternativePathConfig_1 = new List<int>();
            List<int> alternativePathConfig_2 = new List<int>();
            List<int> alternativePathConfig_3 = new List<int>();
            List<int> alternativePathConfig_4 = new List<int>();
            List<int> alternativePathConfig_5 = new List<int>();
            List<int> alternativePathConfig_6 = new List<int>();
            List<int> alternativePathConfig_7 = new List<int>();
            List<int> alternativePathConfig_8 = new List<int>();
            List<int> alternativePathConfig_9 = new List<int>();
            List<int> alternativePathConfig_10 = new List<int>();

            Build(sections, moveCostForward, moveCostReverse);
        }

        private void Build(List<ASECTION> sections, double moveCostForward, double moveCostReverse)
        {
            sectionList = readDataForSectionList(sections, moveCostForward, moveCostReverse);
            initialIndexDic(sectionList);
            n = addressIndexDic.Count;
            createInterCostList(sectionList);
            PresetBanEndPattern(sectionList);
            if (SCApplication.getMessageString("SHORTEST_PATH_ALGORITHM").Trim() == "FLOYD") //M0.06
            {
                fillCostMatrix(sectionList);
                //建立直接相連的路徑資訊
                EstablishInterconnectedPaths();
                initialPathTableByFloydAlgorithm();
                timewindow = new TimeWindow(this); //初始化TimeWindow物件，帶入FloydAlgorithmRouteGuide物件為參數，使TimeWindow 可以用FloydAlgorithmRouteGuide的東西
            }
            else
            {
                initGraph();//M0.06
            }
        }
        private List<Section> readDataForSectionList(List<ASECTION> sections, double moveCostForward, double moveCostReverse)
        {
            List<Section> sectionList = new List<Section>();
            foreach (ASECTION sec in sections)
            {
                string section_id = sec.SEC_ID.Trim();
                int address_1 = int.Parse(sec.FROM_ADR_ID);
                int address_2 = int.Parse(sec.TO_ADR_ID);

                double section_dis = sec.SEC_DIS;
                int moveCost_1 = sec.SEC_COST_From2To;
                int moveCost_2 = sec.SEC_COST_To2From;

                double movecostF_weight = moveCostForward;
                double movecostR_weight = moveCostReverse;

                if (sec.SEC_DIR == E_RAIL_DIR.F)
                {
                    moveCost_1 = moveCost_1 + (int)(section_dis * movecostF_weight);
                    moveCost_2 = moveCost_2 + (int)(section_dis * movecostR_weight);
                }
                else
                {
                    moveCost_1 = moveCost_1 + (int)(section_dis * movecostR_weight);
                    moveCost_2 = moveCost_2 + (int)(section_dis * movecostF_weight);
                }
                string changeSec_1 = SCUtility.Trim(sec.ADR1_CHG_SEC_ID_1);
                int interCost_1 = sec.ADR1_CHG_SEC_COST_1;
                string changeSec_2 = SCUtility.Trim(sec.ADR1_CHG_SEC_ID_2);
                int interCost_2 = sec.ADR1_CHG_SEC_COST_2;
                string changeSec_3 = SCUtility.Trim(sec.ADR1_CHG_SEC_ID_3);
                int interCost_3 = sec.ADR1_CHG_SEC_COST_3;
                string changeSec_4 = SCUtility.Trim(sec.ADR2_CHG_SEC_ID_1);
                int interCost_4 = sec.ADR2_CHG_SEC_COST_1;
                string changeSec_5 = SCUtility.Trim(sec.ADR2_CHG_SEC_ID_2);
                int interCost_5 = sec.ADR2_CHG_SEC_COST_2;
                string changeSec_6 = SCUtility.Trim(sec.ADR2_CHG_SEC_ID_3);
                int interCost_6 = sec.ADR2_CHG_SEC_COST_3;
                bool isBanEnd_From2To = sec.ISBANEND_From2To;
                bool isBanEnd_To2From = sec.ISBANEND_To2From;
                int direct = (int)sec.SEC_DIR;

                Section section = new Section(section_id, address_1, address_2, moveCost_1, moveCost_2,
                    changeSec_1, interCost_1, changeSec_2, interCost_2, changeSec_3, interCost_3,
                    changeSec_4, interCost_4, changeSec_5, interCost_5, changeSec_6, interCost_6,
                    isBanEnd_From2To, isBanEnd_To2From, direct);
                sectionList.Add(section);

            }
            return sectionList;
        }


        private void initGraph()//M0.06
        {
            graph = new int[n, n];

            foreach (Section s in sectionList)
            {
                int index_1 = addressIndexDic[s.address_1];
                int index_2 = addressIndexDic[s.address_2];
                graph[index_1, index_2] = s.moveCost_1;
                graph[index_2, index_1] = s.moveCost_2;
            }
        }

        #region Application Interface

        public List<List<Section>> _TimeWindow(int fromAdr1, int toAdr1, int tofrom2, int toAdr2)
        {
            int index_from1 = addressIndexDic[fromAdr1];
            int index_to1 = addressIndexDic[toAdr1];
            int index_from2 = addressIndexDic[tofrom2];
            int index_to2 = addressIndexDic[toAdr2];

            PathInfos pathInfos_norma1 = pathInfo_matrix[index_from1][index_to1];
            PathInfos pathInfos_norma2 = pathInfo_matrix[index_from2][index_to2];

            if (pathInfos_norma1 == null)
            {
                throw new Exception($"Can't find path for command:[{fromAdr1},{toAdr1},Please Reselect it.");
            }
            if (pathInfos_norma2 == null)
            {
                throw new Exception($"Can't find path for command:[{tofrom2},{toAdr2},Please Reselect it.");
            }
            List<PathInfos> InfosList = new List<PathInfos>();
            int bestCost;
            List<PathInfo> bestCombination;
            List<int> overlapCostList;

            InfosList.Add(pathInfos_norma1);
            InfosList.Add(pathInfos_norma2);

            findLeastOverlapCostCombination_Ver2(InfosList, out bestCost, out bestCombination, out overlapCostList);

            List<List<Section>> routeList = new List<List<Section>>();
            foreach (PathInfo p in bestCombination)
            {
                List<Section> route = new List<Section>();
                for (int i = 0; i < p.path.Count - 1; i++)
                {
                    Section section = getSectionByTwoNode(p.path[i], p.path[i + 1]);
                    route.Add(section);
                }
                routeList.Add(route);
            }
            return routeList;
        }

        public List<RouteInfo> getFromToRoutesSectionToSection(int from_section_id, int to_section_id)
        {
            List<RouteInfo> routeList = new List<RouteInfo>();
            //M0.03 start
            //int index_from = sectionIndexDic[from_section_id];
            //int index_to = sectionIndexDic[to_section_id];
            //foreach (PathInfo p in pathInfo_forSecToSec_matrix[index_from][index_to].getNotBannedPathInfoList(banPatternList))
            //{
            //    List<Section> route = new List<Section>();
            //    for (int i = 0; i < p.path.Count - 1; i++)
            //    {
            //        Section section = getSectionByTwoNode(p.path[i], p.path[i + 1]);
            //        route.Add(section);
            //    }
            //    routeList.Add(new RouteInfo(route, p.path, p.total_cost));
            //}
            //M0.03 end
            return routeList;
        }

        public List<RouteInfo> getFromToRoutesSectionToAdr(int from_section_id, int to_addr)
        {
            List<RouteInfo> routeList = new List<RouteInfo>();
            //M0.03 start
            //if (isNonNodeAddress(to_addr))//目的地是非Node Address
            //{
            //    int to_section_id = nonNodeAddressBelongSectonDic[to_addr];
            //    if (from_section_id == to_section_id)
            //    {
            //        List<Section> route = new List<Section>();
            //        Section section = getSectionByID(from_section_id);
            //        route.Add(section);
            //        List<int> tempPath = new List<int>();
            //        tempPath.Add(section.address_1);
            //        tempPath.Add(section.address_2);
            //        tempPath[tempPath.Count - 1] = to_addr;//將最後一個Address換成ControlAddress
            //        routeList.Add(new RouteInfo(route, tempPath, 0));
            //        return routeList;
            //    }
            //    int index_from = sectionIndexDic[from_section_id];
            //    int index_to = sectionIndexDic[nonNodeAddressBelongSectonDic[to_addr]];
            //    bool isAddressCloseToNode = false;
            //    int closeToNodeAddress = 0;
            //    if (addressCloseToNodeDic.ContainsKey(to_addr))//查表找出Port Address太靠近的節點
            //    {
            //        isAddressCloseToNode = true;
            //        closeToNodeAddress = addressCloseToNodeDic[to_addr];
            //    }
            //    foreach (PathInfo p in pathInfo_forSecToSec_matrix[index_from][index_to].getNotBannedPathInfoList(banPatternList))
            //    {
            //        if (isAddressCloseToNode)
            //        {
            //            bool isBanned = false;
            //            foreach (List<int> banEndPattern in banEndPatternList)
            //            {
            //                if (banEndPattern[1] == closeToNodeAddress)
            //                {
            //                    if (p.path[p.path.Count - 3] == banEndPattern[0] && p.path[p.path.Count - 2] == banEndPattern[1])//如果路徑的倒數第三、第二節點符合BanEndPattern，則該路徑不可走
            //                    {
            //                        isBanned = true;
            //                        break;
            //                    }
            //                }
            //            }
            //            if (isBanned) continue;
            //        }
            //        List<Section> route = new List<Section>();
            //        for (int i = 0; i < p.path.Count - 1; i++)
            //        {
            //            Section section = getSectionByTwoNode(p.path[i], p.path[i + 1]);
            //            route.Add(section);
            //        }
            //        List<int> tempPath = p.path.ToList();

            //        tempPath[tempPath.Count - 1] = to_addr;//將最後一個Address換成ControlAddress

            //        routeList.Add(new RouteInfo(route, tempPath, p.total_cost));
            //    }
            //}
            //else//目的地是一般Node Address
            //{
            //    int index_from = sectionIndexDic[from_section_id];
            //    int index_to = addressIndexDic[to_addr];
            //    foreach (PathInfo p in pathInfo_forSecToNode_matrix[index_from][index_to].getNotBannedPathInfoList(banPatternList))
            //    {
            //        List<Section> route = new List<Section>();
            //        for (int i = 0; i < p.path.Count - 1; i++)
            //        {
            //            Section section = getSectionByTwoNode(p.path[i], p.path[i + 1]);
            //            route.Add(section);
            //        }
            //        routeList.Add(new RouteInfo(route, p.path, p.total_cost));
            //    }
            //}
            //M0.03 end
            return routeList;
        }


        public List<RouteInfo> getFromToRoutesAddrToAddr(int from_addr, int to_addr)//M0.06
        {

            if (algorithm.Trim() == "FLOYD")//M0.06
            {
                #region floyd
                List<RouteInfo> routeList = new List<RouteInfo>();
                int index_from = addressIndexDic[from_addr];
                int index_to = addressIndexDic[to_addr];
                foreach (PathInfo p in pathInfo_matrix[index_from][index_to].getNotBannedPathInfoList(banPatternList))
                {
                    List<Section> route = new List<Section>();
                    for (int i = 0; i < p.path.Count - 1; i++)
                    {
                        Section section = getSectionByTwoNode(p.path[i], p.path[i + 1]);
                        route.Add(section);
                    }
                    routeList.Add(new RouteInfo(route, p.path, p.total_cost));
                }
                return routeList;
                #endregion floyd
            }
            else
            {
                #region Dijkstra
                int[,] localGraph = new int[n, n];
                for (int i = 0; i < n; i++)//複製一個Cost圖出來
                {
                    for (int j = 0; j < n; j++)
                    {
                        localGraph[i, j] = graph[i, j];
                    }
                }
                foreach (List<int> banPattern in banPatternList)//Ban掉的路徑輸入進計算Cost的圖
                {
                    int ban_index_from = addressIndexDic[banPattern[0]];
                    int ban_index_to = addressIndexDic[banPattern[1]];
                    localGraph[ban_index_from, ban_index_to] = 0;//Cost 0代表無法到達
                }
                //Cost圖準備完成

                PathInfo pathInfo = new PathInfo();
                bool isFirstTime = true;
                do
                {
                    if (!isFirstTime)
                    {
                        int ban_index_from = addressIndexDic[pathInfo.path[pathInfo.path.Count - 2]];
                        int ban_index_to = addressIndexDic[pathInfo.path[pathInfo.path.Count - 1]];
                        localGraph[ban_index_from, ban_index_to] = 0;//Cost 0代表無法到達
                        pathInfo = new PathInfo();
                    }
                    else
                    {
                        isFirstTime = false;
                    }
                    int[] dist;
                    int[] previous;
                    int index_from = addressIndexDic[from_addr];
                    int index_to = addressIndexDic[to_addr];
                    dijkstra(localGraph, index_from, index_to, out dist, out previous);
                    //Dijkstra演算結束
                    if (dist[index_to] == int.MaxValue)//表示無法到達
                    {
                        break;
                    }
                    pathInfo.total_cost = dist[index_to];

                    int index = index_to;
                    while (previous[index] != index_from)
                    {
                        pathInfo.path.Add(IndexAddressDic[index]);
                        index = previous[index];
                    }
                    pathInfo.path.Add(IndexAddressDic[index]);
                    pathInfo.path.Add(IndexAddressDic[previous[index]]);
                    pathInfo.path.Reverse();
                }
                while (pathInfo.isPathEndBanned(banEndPatternList));

                List<RouteInfo> routeList = new List<RouteInfo>();
                if (pathInfo.path.Count != 0)
                {
                    List<Section> route = new List<Section>();
                    for (int i = 0; i < pathInfo.path.Count - 1; i++)
                    {
                        Section section = getSectionByTwoNode(pathInfo.path[i], pathInfo.path[i + 1]);
                        route.Add(section);
                    }
                    routeList.Add(new RouteInfo(route, pathInfo.path, pathInfo.total_cost));
                }
                else
                {
                    //do nothing
                }
                return routeList;
                #endregion Dijkstra
            }
        }


        public List<RouteInfo> getFromToRoutesAddrToAddr(int from_addr, int to_addr, List<string> banSegmentIDList)//M0.07
        {
            if (algorithm.Trim() == "FLOYD")//M0.06
            {
                #region floyd
                List<RouteInfo> routeList = new List<RouteInfo>();
                int index_from = addressIndexDic[from_addr];
                int index_to = addressIndexDic[to_addr];
                foreach (PathInfo p in pathInfo_matrix[index_from][index_to].getNotBannedPathInfoList(banPatternList))
                {
                    List<Section> route = new List<Section>();
                    for (int i = 0; i < p.path.Count - 1; i++)
                    {
                        Section section = getSectionByTwoNode(p.path[i], p.path[i + 1]);
                        route.Add(section);
                    }
                    routeList.Add(new RouteInfo(route, p.path, p.total_cost));
                }
                return routeList;
                #endregion floyd
            }
            else
            {
                #region Dijkstra
                int[,] localGraph = new int[n, n];
                for (int i = 0; i < n; i++)//複製一個Cost圖出來
                {
                    for (int j = 0; j < n; j++)
                    {
                        localGraph[i, j] = graph[i, j];
                    }
                }
                #region 建立新的banPatternList
                List<List<int>> _banPatternList = new List<List<int>>(banPatternList);
                foreach (string segmentID in banSegmentIDList)
                {
                    if (!int.TryParse(segmentID, out int sec_id)) continue;
                    Section section = getSectionByID(sec_id);
                    if (section != null)
                    {
                        List<int> banPattern_1 = new List<int>();
                        banPattern_1.Add(section.address_1);
                        banPattern_1.Add(section.address_2);

                        List<int> banPattern_2 = new List<int>();
                        banPattern_2.Add(section.address_2);
                        banPattern_2.Add(section.address_1);
                        bool isExist_1 = false;
                        foreach (List<int> p in _banPatternList)
                        {
                            if (p[0] == banPattern_1[0] && p[1] == banPattern_1[1])
                            {
                                isExist_1 = true;
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        if (!isExist_1)
                        {
                            _banPatternList.Add(banPattern_1);
                        }

                        bool isExist_2 = false;
                        foreach (List<int> p in _banPatternList)
                        {
                            if (p[0] == banPattern_2[0] && p[1] == banPattern_2[1])
                            {
                                isExist_2 = true;
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        if (!isExist_2)
                        {
                            _banPatternList.Add(banPattern_2);
                        }
                    }
                }
                #endregion 建立新的banPatternList

                foreach (List<int> banPattern in _banPatternList)//Ban掉的路徑輸入進計算Cost的圖
                {
                    int ban_index_from = addressIndexDic[banPattern[0]];
                    int ban_index_to = addressIndexDic[banPattern[1]];
                    localGraph[ban_index_from, ban_index_to] = 0;//Cost 0代表無法到達
                }
                //Cost圖準備完成

                PathInfo pathInfo = new PathInfo();
                bool isFirstTime = true;
                do
                {
                    if (!isFirstTime)
                    {
                        int ban_index_from = addressIndexDic[pathInfo.path[pathInfo.path.Count - 2]];
                        int ban_index_to = addressIndexDic[pathInfo.path[pathInfo.path.Count - 1]];
                        localGraph[ban_index_from, ban_index_to] = 0;//Cost 0代表無法到達
                        pathInfo = new PathInfo();
                    }
                    else
                    {
                        isFirstTime = false;
                    }
                    int[] dist;
                    int[] previous;
                    int index_from = addressIndexDic[from_addr];
                    int index_to = addressIndexDic[to_addr];
                    dijkstra(localGraph, index_from, index_to, out dist, out previous);
                    //Dijkstra演算結束
                    if (dist[index_to] == int.MaxValue)//表示無法到達
                    {
                        break;
                    }
                    pathInfo.total_cost = dist[index_to];

                    int index = index_to;
                    while (previous[index] != index_from)
                    {
                        pathInfo.path.Add(IndexAddressDic[index]);
                        index = previous[index];
                    }
                    pathInfo.path.Add(IndexAddressDic[index]);
                    pathInfo.path.Add(IndexAddressDic[previous[index]]);
                    pathInfo.path.Reverse();
                }
                while (pathInfo.isPathEndBanned(banEndPatternList));

                List<RouteInfo> routeList = new List<RouteInfo>();
                if (pathInfo.path.Count != 0)
                {
                    List<Section> route = new List<Section>();
                    for (int i = 0; i < pathInfo.path.Count - 1; i++)
                    {
                        Section section = getSectionByTwoNode(pathInfo.path[i], pathInfo.path[i + 1]);
                        route.Add(section);
                    }
                    routeList.Add(new RouteInfo(route, pathInfo.path, pathInfo.total_cost));
                }
                else
                {
                    //do nothing
                }
                return routeList;
                #endregion Dijkstra
            }
        }
        public void dijkstra(int[,] graph, int index_from, int index_to, out int[] dist, out int[] previous)
        {
            dist = new int[n];
            previous = new int[n];
            bool[] sptSet = new bool[n];

            for (int i = 0; i < n; i++)
            {
                dist[i] = int.MaxValue;
                sptSet[i] = false;
            }
            dist[index_from] = 0;
            for (int count = 0; count < n - 1; count++)
            {
                int u = minDistance(dist, sptSet);
                if (u == index_to) break;//已經找到目的地，不用再找了。
                sptSet[u] = true;
                for (int v = 0; v < n; v++)
                {
                    int intercostFirstPoint = previous[u];
                    int intercostSecondPoint = u;
                    int intercostThirdPoint = v;
                    int inter_cost = 0;
                    InterCost interCost = interCostsList.Find((InterCost c) => c.firstPoint == intercostFirstPoint && c.secondPoint == intercostSecondPoint && c.thirdPoint == intercostThirdPoint);
                    if (interCost == null)
                    {
                        inter_cost = 0;
                    }
                    else
                    {
                        inter_cost = interCost.cost;
                    }
                    if (!sptSet[v] && graph[u, v] != 0 && dist[u] != int.MaxValue && dist[u] + graph[u, v] + inter_cost < dist[v])
                    {
                        previous[v] = u;
                        dist[v] = dist[u] + graph[u, v] + inter_cost;
                    }
                }
            }
        }

        int minDistance(int[] dist,
                bool[] sptSet)
        {
            // Initialize min value 
            int min = int.MaxValue, min_index = -1;

            for (int v = 0; v < n; v++)
                if (sptSet[v] == false && dist[v] <= min)
                {
                    min = dist[v];
                    min_index = v;
                }

            return min_index;
        }

        public (List<RouteInfo> stratFromRouteInfoList, List<RouteInfo> fromToRouteInfoList) getStartFromThenFromToRoutesSecToAddrToAddr(int start_sec, int from_addr, int to_addr)
        {
            List<RouteInfo> startFromRouteList = getFromToRoutesSectionToAdr(start_sec, from_addr);
            List<RouteInfo> fromToRouteList = getFromToRoutesAddrToAddr(from_addr, to_addr);
            return (startFromRouteList, fromToRouteList);
        }

        public (List<RouteInfo> stratFromRouteInfoList, List<RouteInfo> fromToRouteInfoList) getStartFromThenFromToRoutesAddrToAddrToAddr(int start_addr, int from_addr, int to_addr)
        {
            List<RouteInfo> startFromRouteList = getFromToRoutesAddrToAddr(start_addr, from_addr);
            List<RouteInfo> fromToRouteList = getFromToRoutesAddrToAddr(from_addr, to_addr);
            return (startFromRouteList, fromToRouteList);
        }
        /////////////////////////////

        public void banRouteOneDirect(int from, int to)
        {
            List<int> banPattern = new List<int>();
            banPattern.Add(from);
            banPattern.Add(to);
            bool isExist = false;
            foreach (List<int> p in banPatternList)
            {
                if (p[0] == banPattern[0] && p[1] == banPattern[1])
                {
                    isExist = true;
                    break;
                }
                else
                {
                    continue;
                }
            }
            if (!isExist)
            {
                banPatternList.Add(banPattern);
            }
        }

        public void unbanRouteOneDirect(int from, int to)
        {
            List<int> banPattern = new List<int>();
            banPattern.Add(from);
            banPattern.Add(to);
            bool isExist = false;
            int index = 0;
            foreach (List<int> p in banPatternList)
            {
                if (p[0] == banPattern[0] && p[1] == banPattern[1])
                {
                    isExist = true;
                    break;
                }
                else
                {
                    index++;
                    continue;
                }
            }
            if (isExist)
            {
                banPatternList.RemoveAt(index);
            }
        }
        public void banRouteTwoDirect(string sectionID)
        {
            if (!int.TryParse(sectionID, out int sec_id)) return;
            Section section = getSectionByID(sec_id);
            if (section != null)
                banRouteTwoDirect(section.address_1, section.address_2);
        }
        public void banRouteTwoDirect(int address_1, int address_2)
        {
            List<int> banPattern_1 = new List<int>();
            banPattern_1.Add(address_1);
            banPattern_1.Add(address_2);

            List<int> banPattern_2 = new List<int>();
            banPattern_2.Add(address_2);
            banPattern_2.Add(address_1);
            bool isExist_1 = false;
            foreach (List<int> p in banPatternList)
            {
                if (p[0] == banPattern_1[0] && p[1] == banPattern_1[1])
                {
                    isExist_1 = true;
                    break;
                }
                else
                {
                    continue;
                }
            }
            if (!isExist_1)
            {
                banPatternList.Add(banPattern_1);
            }

            bool isExist_2 = false;
            foreach (List<int> p in banPatternList)
            {
                if (p[0] == banPattern_2[0] && p[1] == banPattern_2[1])
                {
                    isExist_2 = true;
                    break;
                }
                else
                {
                    continue;
                }
            }
            if (!isExist_2)
            {
                banPatternList.Add(banPattern_2);
            }
        }
        public void unbanRouteTwoDirect(string sectionID)
        {
            if (!int.TryParse(sectionID, out int sec_id)) return;
            Section section = getSectionByID(sec_id);
            unbanRouteTwoDirect(section.address_1, section.address_2);
        }
        public void unbanRouteTwoDirect(int address_1, int address_2)
        {
            List<int> banPattern_1 = new List<int>();
            banPattern_1.Add(address_1);
            banPattern_1.Add(address_2);

            List<int> banPattern_2 = new List<int>();
            banPattern_2.Add(address_2);
            banPattern_2.Add(address_1);
            bool isExist_1 = false;
            int index_1 = 0;
            foreach (List<int> p in banPatternList)
            {
                if (p[0] == banPattern_1[0] && p[1] == banPattern_1[1])
                {
                    isExist_1 = true;
                    break;
                }
                else
                {
                    index_1++;
                    continue;
                }
            }
            if (isExist_1)
            {
                banPatternList.RemoveAt(index_1);
            }

            bool isExist_2 = false;
            int index_2 = 0;
            foreach (List<int> p in banPatternList)
            {
                if (p[0] == banPattern_2[0] && p[1] == banPattern_2[1])
                {
                    isExist_2 = true;
                    break;
                }
                else
                {
                    index_2++;
                    continue;
                }
            }
            if (isExist_2)
            {
                banPatternList.RemoveAt(index_2);
            }
        }

        public int[] getAllBanDirectArray()
        {
            int arrayLength = (banPatternList.Count) * 2;
            int[] banDirectArray = new int[arrayLength];
            int index = 0;
            foreach (List<int> pattern in banPatternList)
            {
                banDirectArray[index++] = pattern[0];
                banDirectArray[index++] = pattern[1];
            }
            return banDirectArray;
        }
        public void resetBanRoute()
        {
            banPatternList.Clear();
        }



        #endregion Application Interface

        public List<PathInfo> getFromToPathInfoSectionToSection(int from_section_id, int to_section_id)
        {
            List<PathInfo> pathInfoList = new List<PathInfo>();
            //M0.03 start
            //int index_from = sectionIndexDic[from_section_id];
            //int index_to = sectionIndexDic[to_section_id];
            //foreach (PathInfo p in pathInfo_forSecToSec_matrix[index_from][index_to].getNotBannedPathInfoList(banPatternList))
            //{
            //    pathInfoList.Add(p);
            //}
            //M0.03 end
            return pathInfoList;
        }

        public List<PathInfo> getFromToPathInfoListSectionToAdr(int from_section_id, int to_addr)
        {
            List<PathInfo> pathInfoList = new List<PathInfo>();

            //if (isNonNodeAddress(to_addr))//目的地是非Node Address
            //{
            int index_from = sectionIndexDic[from_section_id];
            int index_to = sectionIndexDic[nonNodeAddressBelongSectonDic[to_addr]];
            bool isAddressCloseToNode = false;
            int closeToNodeAddress = 0;
            //M0.03 start
            //if (addressCloseToNodeDic.ContainsKey(to_addr))//查表找出Port Address太靠近的節點
            //{
            //    isAddressCloseToNode = true;
            //    closeToNodeAddress = addressCloseToNodeDic[to_addr];
            //}
            //foreach (PathInfo p in pathInfo_forSecToSec_matrix[index_from][index_to].getNotBannedPathInfoList(banPatternList))
            //{
            //    if (isAddressCloseToNode)
            //    {
            //        bool isBanned = false;
            //        foreach (List<int> banEndPattern in banEndPatternList)
            //        {
            //            if (banEndPattern[1] == closeToNodeAddress)
            //            {
            //                if (p.path[p.path.Count - 3] == banEndPattern[0] && p.path[p.path.Count - 2] == banEndPattern[1])//如果路徑的倒數第三、第二節點符合BanEndPattern，則該路徑不可走
            //                {
            //                    isBanned = true;
            //                    break;
            //                }
            //            }
            //        }
            //        if (isBanned) continue;
            //    }
            //    pathInfoList.Add(p);
            //}
            //M0.03 end
            return pathInfoList;
            //}
            //else//目的地是一般Node Address
            //{
            //    //int index_from = sectionIndexDic[from_section_id];
            //    //int index_to = addressIndexDic[to_addr];
            //    //return pathInfo_forSecToNode_matrix[index_from][index_to].getNotBannedPathInfoList(banPatternList);
            //}

        }

        public List<PathInfo> getFromToPathInfoListAddrToAddr(int from_addr, int to_addr)
        {
            List<PathInfo> pathInfoList = new List<PathInfo>();
            //bool fromAddrIsNonNodeAddr = isNonNodeAddress(from_addr);
            //bool toAddrIsNonNodeAddr = isNonNodeAddress(to_addr);

            //if (!fromAddrIsNonNodeAddr && !toAddrIsNonNodeAddr)//Node Address to Node Address
            //{
            int index_from = addressIndexDic[from_addr];
            int index_to = addressIndexDic[to_addr];
            return pathInfo_matrix[index_from][index_to].getNotBannedPathInfoList(banPatternList);
            //}
            //else if (!fromAddrIsNonNodeAddr && toAddrIsNonNodeAddr)//Node Address to NonNode Address
            //{
            //    int index_from = addressIndexDic[from_addr];
            //    int index_to = sectionIndexDic[nonNodeAddressBelongSectonDic[to_addr]];
            //    bool isPortCloseToNode = false;
            //    int closeToNodeAddress = 0;
            //    if (addressCloseToNodeDic.ContainsKey(to_addr))//查表找出Port Node太靠近的節點
            //    {
            //        isPortCloseToNode = true;
            //        closeToNodeAddress = addressCloseToNodeDic[to_addr];
            //    }
            //    foreach (PathInfo p in pathInfo_forNodeToSec_matrix[index_from][index_to].getNotBannedPathInfoList(banPatternList))
            //    {
            //        if (isPortCloseToNode)
            //        {
            //            bool isBanned = false;
            //            foreach (List<int> banEndPattern in banEndPatternList)
            //            {
            //                if (banEndPattern[1] == closeToNodeAddress)
            //                {
            //                    if (p.path[p.path.Count - 3] == banEndPattern[0] && p.path[p.path.Count - 2] == banEndPattern[1])//如果路徑的倒數第三、第二節點符合BanEndPattern，則該路徑不可走
            //                    {
            //                        isBanned = true;
            //                        break;
            //                    }
            //                }
            //            }
            //            if (isBanned) continue;
            //        }
            //        pathInfoList.Add(p);
            //    }
            //    return pathInfoList;
            //}
            //else if (fromAddrIsNonNodeAddr && !toAddrIsNonNodeAddr)//NonNode Address to Node Address
            //{
            //    int from_sec_id = nonNodeAddressBelongSectonDic[from_addr];
            //    pathInfoList = getFromToPathInfoListSectionToAdr(from_sec_id, to_addr);
            //    return pathInfoList;
            //}
            //else//NonNode Address to NonNode Address
            //{
            //    int from_sec_id = nonNodeAddressBelongSectonDic[from_addr];
            //    pathInfoList = getFromToPathInfoListSectionToAdr(from_sec_id, to_addr);
            //    return pathInfoList;
            //}
        }

        //private bool isNonNodeAddress(int address)
        //{
        //    if (address > 2000 && (address.ToString().StartsWith("20") || address.ToString().StartsWith("30")))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        private bool isPortAddress(int address)
        {
            if (address > 3000 && address.ToString().StartsWith("30"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Section getSectionByTwoNode(int node1, int node2)
        {
            foreach (Section s in sectionList)
            {
                if ((s.address_1 == node1 && s.address_2 == node2)
                 || (s.address_1 == node2 && s.address_2 == node1))
                {
                    return s;
                }
                else
                {
                    continue;
                }
            }
            return null;
        }
        private Section getSectionByID(int ID)
        {
            return sectionList.Where(sec => sec.isection_id == ID).SingleOrDefault();
        }

        private void initialPathTableByFloydAlgorithm()
        {
            #region Floyd演算法
            //Floyd演算法
            for (int k = 0; k < n; k++)
            {
                Console.WriteLine($"K:{k}");
                for (int i = 0; i < n; i++)
                {
                    //Console.WriteLine($"i:{i}");
                    Parallel.For(0, n, j =>             //M0,04
                    {
                        //M0,04 for (int j = 0; j < n; j++)
                        //M0,04 {

                        int inter_cost = 0;
                        //M0.02 start
                        int intercostFirstPoint = i;
                        int intercostSecondPoint = k;
                        int intercostThirdPoint = j;
                        InterCost interCost = interCostsList.Find((InterCost c) => c.firstPoint == intercostFirstPoint && c.secondPoint == intercostSecondPoint && c.thirdPoint == intercostThirdPoint);
                        if (interCost == null)
                        {
                            inter_cost = 0;
                        }
                        else
                        {
                            inter_cost = interCost.cost;
                        }
                        //M0.02 end
                        int new_cost = cost_matrix[i][k] + inter_cost + cost_matrix[k][j];
                        #region 紀錄路徑資訊
                        if (new_cost < 999999 && new_cost != 0)//不能到達與自己加自己不加入路徑資訊
                        {
                            if (i != k && k != j)//包含自己到自己的路徑不計算
                            {
                                int frontPathCout = pathInfo_matrix[i][k].getPathInfotoList().Count;
                                int backPathCout = pathInfo_matrix[k][j].getPathInfotoList().Count;

                                foreach (PathInfo frontPath in pathInfo_matrix[i][k].getPathInfotoList())
                                {
                                    foreach (PathInfo backPath in pathInfo_matrix[k][j].getPathInfotoList())
                                    {
                                        int _inter_cost = 0;
                                        //M0.02 start
                                        int _intercostFirstPoint;
                                        int _intercostSecondPoint;
                                        int _intercostThirdPoint;
                                        _intercostFirstPoint = addressIndexDic[frontPath.path[frontPath.path.Count - 2]];
                                        _intercostSecondPoint = k;
                                        _intercostThirdPoint = addressIndexDic[backPath.path[1]];
                                        InterCost __interCost = interCostsList.Find((InterCost c) => c.firstPoint == _intercostFirstPoint && c.secondPoint == _intercostSecondPoint && c.thirdPoint == _intercostThirdPoint);
                                        if (__interCost == null)
                                        {
                                            _inter_cost = 0;
                                        }
                                        else
                                        {
                                            _inter_cost = __interCost.cost;
                                        }
                                        //M0.02 end

                                        PathInfo pathInfo = new PathInfo();
                                        pathInfo.total_cost = frontPath.total_cost + _inter_cost + backPath.total_cost;
                                        List<int> tempCostdetail = frontPath.costDetail.ToList();
                                        tempCostdetail.RemoveAt(tempCostdetail.Count - 1);//前路徑CostDetail去掉結尾(被intercost置換了)
                                        pathInfo.costDetail.AddRange(tempCostdetail);
                                        pathInfo.costDetail.Add(_inter_cost);
                                        pathInfo.costDetail.AddRange(backPath.costDetail);
                                        int count = 0;
                                        pathInfo.costAccumulation.Add(0);
                                        foreach (int c in pathInfo.costDetail)
                                        {
                                            count += c;
                                            pathInfo.costAccumulation.Add(count);
                                        }
                                        pathInfo.path.AddRange(frontPath.path);
                                        List<int> temppath = backPath.path.ToList();
                                        temppath.RemoveAt(0);//後路徑去掉開頭(前路徑結尾等於後路徑開頭)
                                        pathInfo.path.AddRange(temppath);
                                        if (!pathInfo.isPathLoop() && !pathInfo.isPathBanned(banPatternList))//避免新產生的路徑有迴圈
                                        {
                                            if (pathInfo_matrix[i][j].getPathInfotoList().Contains(pathInfo))
                                            {
                                                //break;
                                            }
                                            else
                                            {
                                                pathInfo_matrix[i][j].addtoInfotoListLimited(pathInfo);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        #endregion 紀錄路徑資訊

                        if (cost_matrix[i][j] > new_cost)
                        {
                            cost_matrix[i][j] = new_cost;
                        }
                        //M0,04 }
                        //M0,04 if (j != 0 && j % 5 == 0)
                        //M0,04 {
                        //M0,04     System.Threading.Thread.Sleep(100);
                        //M0,04 }

                    });

                }
            }
            #endregion Floyd演算法

            #region 建立SecionToSection路徑
            int sectionTotalCount = sectionList.Count;
            //M0.03 start
            //pathInfo_forSecToSec_matrix = new PathInfos[sectionTotalCount][];
            //for (int i = 0; i < sectionTotalCount; i++)
            //{
            //    pathInfo_forSecToSec_matrix[i] = new PathInfos[sectionTotalCount];
            //    for (int j = 0; j < sectionTotalCount; j++)
            //    {
            //        pathInfo_forSecToSec_matrix[i][j] = new PathInfos(max_path_keep_num);
            //    }
            //}
            //for (int i = 0; i < sectionTotalCount; i++)
            //{
            //    for (int j = 0; j < sectionTotalCount; j++)
            //    {
            //        if (i == j) continue;//包含自己到自己的路徑不計算


            //        int startAddress_1 = sectionList[i].address_1;
            //        int startAddress_2 = sectionList[i].address_2;
            //        int destinationAddress_1 = sectionList[j].address_1;
            //        int destinationAddress_2 = sectionList[j].address_2;
            //        int direct = sectionDirDic[int.Parse(sectionList[j].section_id)];

            //        if (direct == 1)
            //        {
            //            if (startAddress_1 != destinationAddress_1)
            //            {
            //                foreach (PathInfo pi in pathInfo_matrix[addressIndexDic[startAddress_1]][addressIndexDic[destinationAddress_1]].getPathInfotoList())
            //                {
            //                    if (pi.path.Contains(startAddress_2) || pi.path.Contains(destinationAddress_2)) continue;//通過另一個起點或終點的路徑不予加入
            //                    PathInfo secPathInfo = new PathInfo();
            //                    secPathInfo.path.Add(startAddress_2);
            //                    secPathInfo.path.AddRange(pi.path);
            //                    secPathInfo.path.Add(destinationAddress_2);

            //                    int front_inter_cost = 0;
            //                    int back_inter_cost = 0;
            //                    //M0.02 start
            //                    int intercostFirstPoint = addressIndexDic[secPathInfo.path[0]];
            //                    int intercostSecondPoint = addressIndexDic[secPathInfo.path[1]];
            //                    int intercostThirdPoint = addressIndexDic[secPathInfo.path[2]];
            //                    InterCost interCost = interCostsList.Find((InterCost c) => c.firstPoint == intercostFirstPoint && c.secondPoint == intercostSecondPoint && c.thirdPoint == intercostThirdPoint);
            //                    if (interCost == null)
            //                    {
            //                        front_inter_cost = 0;
            //                    }
            //                    else
            //                    {
            //                        front_inter_cost = interCost.cost;
            //                    }

            //                    intercostFirstPoint = addressIndexDic[secPathInfo.path[secPathInfo.path.Count - 3]];
            //                    intercostSecondPoint = addressIndexDic[secPathInfo.path[secPathInfo.path.Count - 2]];
            //                    intercostThirdPoint = addressIndexDic[secPathInfo.path[secPathInfo.path.Count - 1]];
            //                    InterCost _interCost = interCostsList.Find((InterCost c) => c.firstPoint == intercostFirstPoint && c.secondPoint == intercostSecondPoint && c.thirdPoint == intercostThirdPoint);
            //                    if (_interCost == null)
            //                    {
            //                        back_inter_cost = 0;
            //                    }
            //                    else
            //                    {
            //                        back_inter_cost = _interCost.cost;
            //                    }
            //                    //M0.02 end
            //                    secPathInfo.total_cost = cost_matrix[addressIndexDic[startAddress_2]][addressIndexDic[startAddress_1]] + front_inter_cost
            //                        + pi.total_cost + back_inter_cost + cost_matrix[addressIndexDic[destinationAddress_1]][addressIndexDic[destinationAddress_2]];

            //                    secPathInfo.costDetail.Add(cost_matrix[addressIndexDic[startAddress_2]][addressIndexDic[startAddress_1]]);
            //                    secPathInfo.costDetail.Add(front_inter_cost);
            //                    List<int> tempCostDetail = pi.costDetail.ToList();
            //                    tempCostDetail.RemoveAt(tempCostDetail.Count - 1);
            //                    secPathInfo.costDetail.AddRange(tempCostDetail);
            //                    secPathInfo.costDetail.Add(back_inter_cost);
            //                    secPathInfo.costDetail.Add(cost_matrix[addressIndexDic[destinationAddress_1]][addressIndexDic[destinationAddress_2]]);
            //                    secPathInfo.costDetail.Add(0);
            //                    int count = 0;
            //                    secPathInfo.costAccumulation.Add(0);
            //                    foreach (int c in secPathInfo.costDetail)
            //                    {
            //                        count += c;
            //                        secPathInfo.costAccumulation.Add(count);
            //                    }
            //                    pathInfo_forSecToSec_matrix[i][j].addtoInfotoListUnlimited(secPathInfo);
            //                }
            //            }
            //            else
            //            {
            //                PathInfo secPathInfo = new PathInfo();
            //                secPathInfo.path.Add(startAddress_2);
            //                secPathInfo.path.Add(startAddress_1);
            //                secPathInfo.path.Add(destinationAddress_2);

            //                int inter_cost = 0;
            //                //M0.02 start
            //                int intercostFirstPoint = addressIndexDic[secPathInfo.path[0]];
            //                int intercostSecondPoint = addressIndexDic[secPathInfo.path[1]];
            //                int intercostThirdPoint = addressIndexDic[secPathInfo.path[2]];
            //                InterCost interCost = interCostsList.Find((InterCost c) => c.firstPoint == intercostFirstPoint && c.secondPoint == intercostSecondPoint && c.thirdPoint == intercostThirdPoint);
            //                if (interCost == null)
            //                {
            //                    inter_cost = 0;
            //                }
            //                else
            //                {
            //                    inter_cost = interCost.cost;
            //                }
            //                //M0.02 end
            //                secPathInfo.total_cost = cost_matrix[addressIndexDic[startAddress_2]][addressIndexDic[startAddress_1]]
            //                    + inter_cost + cost_matrix[addressIndexDic[destinationAddress_1]][addressIndexDic[destinationAddress_2]];

            //                secPathInfo.costDetail.Add(cost_matrix[addressIndexDic[startAddress_2]][addressIndexDic[startAddress_1]]);
            //                secPathInfo.costDetail.Add(inter_cost);
            //                secPathInfo.costDetail.Add(cost_matrix[addressIndexDic[destinationAddress_1]][addressIndexDic[destinationAddress_2]]);
            //                secPathInfo.costDetail.Add(0);
            //                int count = 0;
            //                secPathInfo.costAccumulation.Add(0);
            //                foreach (int c in secPathInfo.costDetail)
            //                {
            //                    count += c;
            //                    secPathInfo.costAccumulation.Add(count);
            //                }
            //                pathInfo_forSecToSec_matrix[i][j].addtoInfotoListUnlimited(secPathInfo);
            //            }
            //            if (startAddress_2 != destinationAddress_1)
            //            {
            //                foreach (PathInfo pi in pathInfo_matrix[addressIndexDic[startAddress_2]][addressIndexDic[destinationAddress_1]].getPathInfotoList())
            //                {
            //                    if (pi.path.Contains(startAddress_1) || pi.path.Contains(destinationAddress_2)) continue;//通過另一個起點或終點的路徑不予加入
            //                    PathInfo secPathInfo = new PathInfo();
            //                    secPathInfo.path.Add(startAddress_1);
            //                    secPathInfo.path.AddRange(pi.path);
            //                    secPathInfo.path.Add(destinationAddress_2);

            //                    int front_inter_cost = 0;
            //                    int back_inter_cost = 0;
            //                    //M0.02 start
            //                    int intercostFirstPoint = addressIndexDic[secPathInfo.path[0]];
            //                    int intercostSecondPoint = addressIndexDic[secPathInfo.path[1]];
            //                    int intercostThirdPoint = addressIndexDic[secPathInfo.path[2]];
            //                    InterCost interCost = interCostsList.Find((InterCost c) => c.firstPoint == intercostFirstPoint && c.secondPoint == intercostSecondPoint && c.thirdPoint == intercostThirdPoint);
            //                    if (interCost == null)
            //                    {
            //                        front_inter_cost = 0;
            //                    }
            //                    else
            //                    {
            //                        front_inter_cost = interCost.cost;
            //                    }

            //                    intercostFirstPoint = addressIndexDic[secPathInfo.path[secPathInfo.path.Count - 3]];
            //                    intercostSecondPoint = addressIndexDic[secPathInfo.path[secPathInfo.path.Count - 2]];
            //                    intercostThirdPoint = addressIndexDic[secPathInfo.path[secPathInfo.path.Count - 1]];
            //                    InterCost _interCost = interCostsList.Find((InterCost c) => c.firstPoint == intercostFirstPoint && c.secondPoint == intercostSecondPoint && c.thirdPoint == intercostThirdPoint);
            //                    if (_interCost == null)
            //                    {
            //                        back_inter_cost = 0;
            //                    }
            //                    else
            //                    {
            //                        back_inter_cost = _interCost.cost;
            //                    }
            //                    //M0.02 end

            //                    secPathInfo.total_cost = cost_matrix[addressIndexDic[startAddress_1]][addressIndexDic[startAddress_2]] + front_inter_cost
            //                        + pi.total_cost + back_inter_cost + cost_matrix[addressIndexDic[destinationAddress_1]][addressIndexDic[destinationAddress_2]];

            //                    secPathInfo.costDetail.Add(cost_matrix[addressIndexDic[startAddress_1]][addressIndexDic[startAddress_2]]);
            //                    secPathInfo.costDetail.Add(front_inter_cost);
            //                    List<int> tempCostDetail = pi.costDetail.ToList();
            //                    tempCostDetail.RemoveAt(tempCostDetail.Count - 1);
            //                    secPathInfo.costDetail.AddRange(tempCostDetail);
            //                    secPathInfo.costDetail.Add(back_inter_cost);
            //                    secPathInfo.costDetail.Add(cost_matrix[addressIndexDic[destinationAddress_1]][addressIndexDic[destinationAddress_2]]);
            //                    secPathInfo.costDetail.Add(0);
            //                    int count = 0;
            //                    secPathInfo.costAccumulation.Add(0);
            //                    foreach (int c in secPathInfo.costDetail)
            //                    {
            //                        count += c;
            //                        secPathInfo.costAccumulation.Add(count);
            //                    }
            //                    pathInfo_forSecToSec_matrix[i][j].addtoInfotoListUnlimited(secPathInfo);
            //                }
            //            }
            //            else
            //            {
            //                PathInfo secPathInfo = new PathInfo();
            //                secPathInfo.path.Add(startAddress_1);
            //                secPathInfo.path.Add(startAddress_2);
            //                secPathInfo.path.Add(destinationAddress_2);

            //                int inter_cost = 0;
            //                //M0.02 start
            //                int intercostFirstPoint = addressIndexDic[secPathInfo.path[0]];
            //                int intercostSecondPoint = addressIndexDic[secPathInfo.path[1]];
            //                int intercostThirdPoint = addressIndexDic[secPathInfo.path[2]];
            //                InterCost interCost = interCostsList.Find((InterCost c) => c.firstPoint == intercostFirstPoint && c.secondPoint == intercostSecondPoint && c.thirdPoint == intercostThirdPoint);
            //                if (interCost == null)
            //                {
            //                    inter_cost = 0;
            //                }
            //                else
            //                {
            //                    inter_cost = interCost.cost;
            //                }
            //                //M0.02 end

            //                secPathInfo.total_cost = cost_matrix[addressIndexDic[startAddress_1]][addressIndexDic[startAddress_2]]
            //                    + inter_cost + cost_matrix[addressIndexDic[destinationAddress_1]][addressIndexDic[destinationAddress_2]];

            //                secPathInfo.costDetail.Add(cost_matrix[addressIndexDic[startAddress_1]][addressIndexDic[startAddress_2]]);
            //                secPathInfo.costDetail.Add(inter_cost);
            //                secPathInfo.costDetail.Add(cost_matrix[addressIndexDic[destinationAddress_1]][addressIndexDic[destinationAddress_2]]);
            //                secPathInfo.costDetail.Add(0);
            //                int count = 0;
            //                secPathInfo.costAccumulation.Add(0);
            //                foreach (int c in secPathInfo.costDetail)
            //                {
            //                    count += c;
            //                    secPathInfo.costAccumulation.Add(count);
            //                }
            //                pathInfo_forSecToSec_matrix[i][j].addtoInfotoListUnlimited(secPathInfo);
            //            }
            //        }
            //        if (direct == 2)
            //        {
            //            if (startAddress_1 != destinationAddress_2)
            //            {
            //                foreach (PathInfo pi in pathInfo_matrix[addressIndexDic[startAddress_1]][addressIndexDic[destinationAddress_2]].getPathInfotoList())
            //                {
            //                    if (pi.path.Contains(startAddress_2) || pi.path.Contains(destinationAddress_1)) continue;//通過另一個起點或終點的路徑不予加入
            //                    PathInfo secPathInfo = new PathInfo();
            //                    secPathInfo.path.Add(startAddress_2);
            //                    secPathInfo.path.AddRange(pi.path);
            //                    secPathInfo.path.Add(destinationAddress_1);

            //                    int front_inter_cost = 0;
            //                    int back_inter_cost = 0;
            //                    //M0.02 start
            //                    int intercostFirstPoint = addressIndexDic[secPathInfo.path[0]];
            //                    int intercostSecondPoint = addressIndexDic[secPathInfo.path[1]];
            //                    int intercostThirdPoint = addressIndexDic[secPathInfo.path[2]];
            //                    InterCost interCost = interCostsList.Find((InterCost c) => c.firstPoint == intercostFirstPoint && c.secondPoint == intercostSecondPoint && c.thirdPoint == intercostThirdPoint);
            //                    if (interCost == null)
            //                    {
            //                        front_inter_cost = 0;
            //                    }
            //                    else
            //                    {
            //                        front_inter_cost = interCost.cost;
            //                    }

            //                    intercostFirstPoint = addressIndexDic[secPathInfo.path[secPathInfo.path.Count - 3]];
            //                    intercostSecondPoint = addressIndexDic[secPathInfo.path[secPathInfo.path.Count - 2]];
            //                    intercostThirdPoint = addressIndexDic[secPathInfo.path[secPathInfo.path.Count - 1]];
            //                    InterCost _interCost = interCostsList.Find((InterCost c) => c.firstPoint == intercostFirstPoint && c.secondPoint == intercostSecondPoint && c.thirdPoint == intercostThirdPoint);
            //                    if (_interCost == null)
            //                    {
            //                        back_inter_cost = 0;
            //                    }
            //                    else
            //                    {
            //                        back_inter_cost = _interCost.cost;
            //                    }

            //                    //M0.02 end
            //                    secPathInfo.total_cost = cost_matrix[addressIndexDic[startAddress_2]][addressIndexDic[startAddress_1]] + front_inter_cost
            //                        + pi.total_cost + back_inter_cost + cost_matrix[addressIndexDic[destinationAddress_2]][addressIndexDic[destinationAddress_1]];

            //                    secPathInfo.costDetail.Add(cost_matrix[addressIndexDic[startAddress_2]][addressIndexDic[startAddress_1]]);
            //                    secPathInfo.costDetail.Add(front_inter_cost);
            //                    List<int> tempCostDetail = pi.costDetail.ToList();
            //                    tempCostDetail.RemoveAt(tempCostDetail.Count - 1);
            //                    secPathInfo.costDetail.AddRange(tempCostDetail);
            //                    secPathInfo.costDetail.Add(back_inter_cost);
            //                    secPathInfo.costDetail.Add(cost_matrix[addressIndexDic[destinationAddress_2]][addressIndexDic[destinationAddress_1]]);
            //                    secPathInfo.costDetail.Add(0);
            //                    int count = 0;
            //                    secPathInfo.costAccumulation.Add(0);
            //                    foreach (int c in secPathInfo.costDetail)
            //                    {
            //                        count += c;
            //                        secPathInfo.costAccumulation.Add(count);
            //                    }
            //                    pathInfo_forSecToSec_matrix[i][j].addtoInfotoListUnlimited(secPathInfo);
            //                }
            //            }
            //            else
            //            {

            //                PathInfo secPathInfo = new PathInfo();
            //                secPathInfo.path.Add(startAddress_2);
            //                secPathInfo.path.Add(startAddress_1);
            //                secPathInfo.path.Add(destinationAddress_1);

            //                int inter_cost = 0;
            //                //M0.02 start
            //                int intercostFirstPoint = addressIndexDic[secPathInfo.path[0]];
            //                int intercostSecondPoint = addressIndexDic[secPathInfo.path[1]];
            //                int intercostThirdPoint = addressIndexDic[secPathInfo.path[2]];
            //                InterCost interCost = interCostsList.Find((InterCost c) => c.firstPoint == intercostFirstPoint && c.secondPoint == intercostSecondPoint && c.thirdPoint == intercostThirdPoint);
            //                if (interCost == null)
            //                {
            //                    inter_cost = 0;
            //                }
            //                else
            //                {
            //                    inter_cost = interCost.cost;
            //                }
            //                //M0.02 end
            //                secPathInfo.total_cost = cost_matrix[addressIndexDic[startAddress_2]][addressIndexDic[startAddress_1]]
            //                    + inter_cost + cost_matrix[addressIndexDic[destinationAddress_2]][addressIndexDic[destinationAddress_1]];

            //                secPathInfo.costDetail.Add(cost_matrix[addressIndexDic[startAddress_2]][addressIndexDic[startAddress_1]]);
            //                secPathInfo.costDetail.Add(inter_cost);
            //                secPathInfo.costDetail.Add(cost_matrix[addressIndexDic[destinationAddress_2]][addressIndexDic[destinationAddress_1]]);
            //                secPathInfo.costDetail.Add(0);
            //                int count = 0;
            //                secPathInfo.costAccumulation.Add(0);
            //                foreach (int c in secPathInfo.costDetail)
            //                {
            //                    count += c;
            //                    secPathInfo.costAccumulation.Add(count);
            //                }
            //                pathInfo_forSecToSec_matrix[i][j].addtoInfotoListUnlimited(secPathInfo);
            //            }

            //            if (startAddress_2 != destinationAddress_2)
            //            {
            //                foreach (PathInfo pi in pathInfo_matrix[addressIndexDic[startAddress_2]][addressIndexDic[destinationAddress_2]].getPathInfotoList())
            //                {
            //                    if (pi.path.Contains(startAddress_1) || pi.path.Contains(destinationAddress_1)) continue;//通過另一個起點或終點的路徑不予加入
            //                    PathInfo secPathInfo = new PathInfo();
            //                    secPathInfo.path.Add(startAddress_1);
            //                    secPathInfo.path.AddRange(pi.path);
            //                    secPathInfo.path.Add(destinationAddress_1);

            //                    int front_inter_cost = 0;
            //                    int back_inter_cost = 0;
            //                    //M0.02 start
            //                    int intercostFirstPoint = addressIndexDic[secPathInfo.path[0]];
            //                    int intercostSecondPoint = addressIndexDic[secPathInfo.path[1]];
            //                    int intercostThirdPoint = addressIndexDic[secPathInfo.path[2]];
            //                    InterCost interCost = interCostsList.Find((InterCost c) => c.firstPoint == intercostFirstPoint && c.secondPoint == intercostSecondPoint && c.thirdPoint == intercostThirdPoint);
            //                    if (interCost == null)
            //                    {
            //                        front_inter_cost = 0;
            //                    }
            //                    else
            //                    {
            //                        front_inter_cost = interCost.cost;
            //                    }

            //                    intercostFirstPoint = addressIndexDic[secPathInfo.path[secPathInfo.path.Count - 3]];
            //                    intercostSecondPoint = addressIndexDic[secPathInfo.path[secPathInfo.path.Count - 2]];
            //                    intercostThirdPoint = addressIndexDic[secPathInfo.path[secPathInfo.path.Count - 1]];
            //                    InterCost _interCost = interCostsList.Find((InterCost c) => c.firstPoint == intercostFirstPoint && c.secondPoint == intercostSecondPoint && c.thirdPoint == intercostThirdPoint);
            //                    if (_interCost == null)
            //                    {
            //                        back_inter_cost = 0;
            //                    }
            //                    else
            //                    {
            //                        back_inter_cost = _interCost.cost;
            //                    }

            //                    //M0.02 end
            //                    secPathInfo.total_cost = cost_matrix[addressIndexDic[startAddress_1]][addressIndexDic[startAddress_2]] + front_inter_cost
            //                        + pi.total_cost + back_inter_cost + cost_matrix[addressIndexDic[destinationAddress_2]][addressIndexDic[destinationAddress_1]];

            //                    secPathInfo.costDetail.Add(cost_matrix[addressIndexDic[startAddress_1]][addressIndexDic[startAddress_2]]);
            //                    secPathInfo.costDetail.Add(front_inter_cost);
            //                    List<int> tempCostDetail = pi.costDetail.ToList();
            //                    tempCostDetail.RemoveAt(tempCostDetail.Count - 1);
            //                    secPathInfo.costDetail.AddRange(tempCostDetail);
            //                    secPathInfo.costDetail.Add(back_inter_cost);
            //                    secPathInfo.costDetail.Add(cost_matrix[addressIndexDic[destinationAddress_2]][addressIndexDic[destinationAddress_1]]);
            //                    secPathInfo.costDetail.Add(0);
            //                    int count = 0;
            //                    secPathInfo.costAccumulation.Add(0);
            //                    foreach (int c in secPathInfo.costDetail)
            //                    {
            //                        count += c;
            //                        secPathInfo.costAccumulation.Add(count);
            //                    }
            //                    pathInfo_forSecToSec_matrix[i][j].addtoInfotoListUnlimited(secPathInfo);
            //                }
            //            }
            //            else
            //            {
            //                PathInfo secPathInfo = new PathInfo();
            //                secPathInfo.path.Add(startAddress_1);
            //                secPathInfo.path.Add(startAddress_2);
            //                secPathInfo.path.Add(destinationAddress_1);

            //                int inter_cost = 0;
            //                //M0.02 start
            //                int intercostFirstPoint = addressIndexDic[secPathInfo.path[0]];
            //                int intercostSecondPoint = addressIndexDic[secPathInfo.path[1]];
            //                int intercostThirdPoint = addressIndexDic[secPathInfo.path[2]];
            //                InterCost interCost = interCostsList.Find((InterCost c) => c.firstPoint == intercostFirstPoint && c.secondPoint == intercostSecondPoint && c.thirdPoint == intercostThirdPoint);
            //                if (interCost == null)
            //                {
            //                    inter_cost = 0;
            //                }
            //                else
            //                {
            //                    inter_cost = interCost.cost;
            //                }
            //                //M0.02 end
            //                secPathInfo.total_cost = cost_matrix[addressIndexDic[startAddress_1]][addressIndexDic[startAddress_2]]
            //                    + inter_cost + cost_matrix[addressIndexDic[destinationAddress_2]][addressIndexDic[destinationAddress_1]];

            //                secPathInfo.costDetail.Add(cost_matrix[addressIndexDic[startAddress_1]][addressIndexDic[startAddress_2]]);
            //                secPathInfo.costDetail.Add(inter_cost);
            //                secPathInfo.costDetail.Add(cost_matrix[addressIndexDic[destinationAddress_2]][addressIndexDic[destinationAddress_1]]);
            //                secPathInfo.costDetail.Add(0);
            //                int count = 0;
            //                secPathInfo.costAccumulation.Add(0);
            //                foreach (int c in secPathInfo.costDetail)
            //                {
            //                    count += c;
            //                    secPathInfo.costAccumulation.Add(count);
            //                }
            //                pathInfo_forSecToSec_matrix[i][j].addtoInfotoListUnlimited(secPathInfo);
            //            }
            //        }
            //        //if (pathInfo_forSecToSec_matrix[i][j].getPathInfotoList().Count == 0)
            //        //{

            //        //}

            //        //if (pathInfo_forSecToSec_matrix[i][j].getPathInfotoList().Count < hahacount)
            //        //{

            //        //    hahacount = pathInfo_forSecToSec_matrix[i][j].getPathInfotoList().Count;
            //        //}
            //    }
            //}
            //M0.03 end

            #endregion 建立SecionToSection路徑

            #region 建立NodeToSection路徑
            //M0.03 start
            //pathInfo_forNodeToSec_matrix = new PathInfos[n][];
            //for (int i = 0; i < n; i++)
            //{
            //    pathInfo_forNodeToSec_matrix[i] = new PathInfos[sectionTotalCount];
            //    for (int j = 0; j < sectionTotalCount; j++)
            //    {
            //        pathInfo_forNodeToSec_matrix[i][j] = new PathInfos(max_path_keep_num);
            //    }
            //}
            //for (int i = 0; i < n; i++)
            //{
            //    for (int j = 0; j < sectionTotalCount; j++)
            //    {
            //        int startIndex = i;
            //        //int startAddress = IndexAddressDic[i];
            //        int destinationAddress_1 = sectionList[j].address_1;
            //        int destinationAddress_2 = sectionList[j].address_2;
            //        int destinationIndex_1 = addressIndexDic[destinationAddress_1];
            //        int destinationIndex_2 = addressIndexDic[destinationAddress_2];
            //        int direct = sectionDirDic[int.Parse(sectionList[j].section_id)];

            //        if (direct == 1)
            //        {
            //            if (startIndex != destinationIndex_1)
            //            {
            //                foreach (PathInfo pi in pathInfo_matrix[startIndex][destinationIndex_1].getPathInfotoList())
            //                {
            //                    if (pi.path.Contains(destinationAddress_2)) continue;//通過另一個終點的路徑不予加入
            //                    PathInfo secPathInfo = new PathInfo();
            //                    secPathInfo.path.AddRange(pi.path);
            //                    secPathInfo.path.Add(destinationAddress_2);

            //                    int back_inter_cost = 0;
            //                    //M0.02 start
            //                    int intercostFirstPoint = addressIndexDic[secPathInfo.path[secPathInfo.path.Count - 3]];
            //                    int intercostSecondPoint = addressIndexDic[secPathInfo.path[secPathInfo.path.Count - 2]];
            //                    int intercostThirdPoint = addressIndexDic[secPathInfo.path[secPathInfo.path.Count - 1]];
            //                    InterCost interCost = interCostsList.Find((InterCost c) => c.firstPoint == intercostFirstPoint && c.secondPoint == intercostSecondPoint && c.thirdPoint == intercostThirdPoint);
            //                    if (interCost == null)
            //                    {
            //                        back_inter_cost = 0;
            //                    }
            //                    else
            //                    {
            //                        back_inter_cost = interCost.cost;
            //                    }

            //                    //M0.02 end
            //                    secPathInfo.total_cost = pi.total_cost + back_inter_cost + cost_matrix[destinationIndex_1][destinationIndex_2];
            //                    List<int> tempCostDetail = pi.costDetail.ToList();
            //                    tempCostDetail.RemoveAt(tempCostDetail.Count - 1);
            //                    secPathInfo.costDetail.AddRange(tempCostDetail);
            //                    secPathInfo.costDetail.Add(back_inter_cost);
            //                    secPathInfo.costDetail.Add(cost_matrix[destinationIndex_1][destinationIndex_2]);
            //                    secPathInfo.costDetail.Add(0);
            //                    int count = 0;
            //                    secPathInfo.costAccumulation.Add(0);
            //                    foreach (int c in secPathInfo.costDetail)
            //                    {
            //                        count += c;
            //                        secPathInfo.costAccumulation.Add(count);
            //                    }
            //                    pathInfo_forNodeToSec_matrix[i][j].addtoInfotoListUnlimited(secPathInfo);
            //                }
            //            }
            //            else
            //            {
            //                PathInfo secPathInfo = new PathInfo();
            //                secPathInfo.path.Add(destinationAddress_1);
            //                secPathInfo.path.Add(destinationAddress_2);

            //                secPathInfo.total_cost = cost_matrix[destinationIndex_1][destinationIndex_2];
            //                secPathInfo.costDetail.Add(cost_matrix[destinationIndex_1][destinationIndex_2]);
            //                secPathInfo.costDetail.Add(0);
            //                int count = 0;
            //                secPathInfo.costAccumulation.Add(0);
            //                foreach (int c in secPathInfo.costDetail)
            //                {
            //                    count += c;
            //                    secPathInfo.costAccumulation.Add(count);
            //                }
            //                pathInfo_forNodeToSec_matrix[i][j].addtoInfotoListUnlimited(secPathInfo);
            //            }
            //        }
            //        if (direct == 2)
            //        {
            //            if (startIndex != destinationIndex_2)
            //            {
            //                foreach (PathInfo pi in pathInfo_matrix[startIndex][destinationIndex_2].getPathInfotoList())
            //                {
            //                    if (pi.path.Contains(destinationAddress_1)) continue;//通過另一個起點或終點的路徑不予加入
            //                    PathInfo secPathInfo = new PathInfo();
            //                    secPathInfo.path.AddRange(pi.path);
            //                    secPathInfo.path.Add(destinationAddress_1);

            //                    int back_inter_cost = 0;
            //                    //M0.02 start
            //                    int intercostFirstPoint = addressIndexDic[secPathInfo.path[secPathInfo.path.Count - 3]];
            //                    int intercostSecondPoint = addressIndexDic[secPathInfo.path[secPathInfo.path.Count - 2]];
            //                    int intercostThirdPoint = addressIndexDic[secPathInfo.path[secPathInfo.path.Count - 1]];
            //                    InterCost interCost = interCostsList.Find((InterCost c) => c.firstPoint == intercostFirstPoint && c.secondPoint == intercostSecondPoint && c.thirdPoint == intercostThirdPoint);
            //                    if (interCost == null)
            //                    {
            //                        back_inter_cost = 0;
            //                    }
            //                    else
            //                    {
            //                        back_inter_cost = interCost.cost;
            //                    }
            //                    //M0.02 end
            //                    secPathInfo.total_cost = pi.total_cost + back_inter_cost + cost_matrix[destinationIndex_2][destinationIndex_1];
            //                    List<int> tempCostDetail = pi.costDetail.ToList();
            //                    tempCostDetail.RemoveAt(tempCostDetail.Count - 1);
            //                    secPathInfo.costDetail.AddRange(tempCostDetail);
            //                    secPathInfo.costDetail.Add(back_inter_cost);
            //                    secPathInfo.costDetail.Add(cost_matrix[destinationIndex_2][destinationIndex_1]);
            //                    secPathInfo.costDetail.Add(0);
            //                    int count = 0;
            //                    secPathInfo.costAccumulation.Add(0);
            //                    foreach (int c in secPathInfo.costDetail)
            //                    {
            //                        count += c;
            //                        secPathInfo.costAccumulation.Add(count);
            //                    }
            //                    pathInfo_forNodeToSec_matrix[i][j].addtoInfotoListUnlimited(secPathInfo);
            //                }
            //            }
            //            else
            //            {

            //                PathInfo secPathInfo = new PathInfo();
            //                secPathInfo.path.Add(destinationAddress_2);
            //                secPathInfo.path.Add(destinationAddress_1);

            //                secPathInfo.total_cost = cost_matrix[destinationIndex_2][destinationIndex_1];
            //                secPathInfo.costDetail.Add(cost_matrix[destinationIndex_2][destinationIndex_1]);
            //                secPathInfo.costDetail.Add(0);
            //                int count = 0;
            //                secPathInfo.costAccumulation.Add(0);
            //                foreach (int c in secPathInfo.costDetail)
            //                {
            //                    count += c;
            //                    secPathInfo.costAccumulation.Add(count);
            //                }
            //                pathInfo_forNodeToSec_matrix[i][j].addtoInfotoListUnlimited(secPathInfo);
            //            }
            //        }
            //    }
            //}
            //M0.03 end

            #endregion 建立NodeToSection路徑

            #region 去除結尾是橫移路徑
            //int lowcount = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    List<PathInfo> pathInfos = pathInfo_matrix[i][j].getPathInfotoList().ToList();

                    for (int x = 0; x < pathInfos.Count; x++)
                    {
                        if (pathInfos[x].isPathEndBanned(banEndPatternList))
                        {
                            pathInfo_matrix[i][j].removePathInfo(pathInfos[x]);
                        }
                    }
                }
            }
            #endregion 去除結尾是橫移路徑

            #region 加入替代道路

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (pathInfo_matrix[i][j].getPathInfotoList().Count == 0) continue;
                    PathInfo best_pathInfo = pathInfo_matrix[i][j].getPathInfotoList().First();
                    for (int x = 0; x < alternativePathConfigList.Count; x++)//所有的替代道路都看一次
                    {
                        for (int p = 0; p + 1 < best_pathInfo.path.Count; p++)//找最佳路徑有沒有替代道路的Pattern
                        {
                            bool findMainSection = false;
                            List<int> alternativePath = null;
                            if (best_pathInfo.path[p + 0] == alternativePathConfigList[x][0] && best_pathInfo.path[p + 1] == alternativePathConfigList[x][1])
                            {
                                findMainSection = true;//找到替代道路出現在路徑中
                                alternativePath = new List<int>();
                                alternativePath.Add(alternativePathConfigList[x][2]);
                                alternativePath.Add(alternativePathConfigList[x][3]);
                            }
                            else if (best_pathInfo.path[p + 0] == alternativePathConfigList[x][2] && best_pathInfo.path[p + 1] == alternativePathConfigList[x][3])
                            {
                                findMainSection = true;//找到替代道路出現在路徑中
                                alternativePath = new List<int>();
                                alternativePath.Add(alternativePathConfigList[x][0]);
                                alternativePath.Add(alternativePathConfigList[x][1]);
                            }
                            else
                            {
                                continue;
                            }
                            if (findMainSection)
                            {
                                PathInfo frontPath = null;
                                PathInfo backPath = null;
                                bool frontPathEmtryIsWrong = false;
                                bool backPathEmtryIsWrong = false;
                                //新加入的替代路徑產生方法是找 原始路徑起始點到替代道路起點的最佳路徑(前路徑)+替代道路+替代道路終點到原始路徑終點(後路徑)
                                //但要注意前路徑不可含替代道路終點，後路徑也不可含替代道路起點。如果有的話就找次佳，還是不行就找次次佳路徑...都沒有就放棄在這輪加入替代路徑
                                if (pathInfo_matrix[i][addressIndexDic[alternativePath[0]]].getPathInfotoList().Count != 0)
                                {
                                    foreach (PathInfo pi in pathInfo_matrix[i][addressIndexDic[alternativePath[0]]].getPathInfotoList())
                                    {
                                        if (pi.path.Contains(alternativePath[1]))
                                        {
                                            frontPathEmtryIsWrong = true;
                                            continue;
                                        }
                                        else
                                        {
                                            frontPath = pi;
                                            frontPathEmtryIsWrong = false;
                                            break;
                                        }
                                    }
                                    //frontPath = pathInfo_matrix[i][addressIndexDic[alternativePath[0]]].getPathInfotoList().First();
                                }
                                else
                                {
                                    //do nothing
                                }

                                if (frontPathEmtryIsWrong)
                                {
                                    continue;
                                }

                                if (pathInfo_matrix[addressIndexDic[alternativePath[1]]][j].getPathInfotoList().Count != 0)
                                {
                                    foreach (PathInfo pi in pathInfo_matrix[addressIndexDic[alternativePath[1]]][j].getPathInfotoList())
                                    {
                                        if (pi.path.Contains(alternativePath[0]))
                                        {
                                            backPathEmtryIsWrong = true;
                                            continue;
                                        }
                                        else
                                        {
                                            backPath = pi;
                                            backPathEmtryIsWrong = false;
                                            break;
                                        }
                                    }
                                    //backPath = pathInfo_matrix[addressIndexDic[alternativePath[1]]][j].getPathInfotoList().First();
                                }
                                else
                                {
                                    //do nothing
                                }

                                if (backPathEmtryIsWrong)
                                {
                                    continue;
                                }

                                int _inter_cos_front = 0;
                                if (frontPath != null)
                                {
                                    //M0.02 start
                                    int intercostFirstPoint = addressIndexDic[frontPath.path[frontPath.path.Count - 2]];
                                    int intercostSecondPoint = addressIndexDic[alternativePath[0]];
                                    int intercostThirdPoint = addressIndexDic[alternativePath[1]];
                                    InterCost interCost = interCostsList.Find((InterCost c) => c.firstPoint == intercostFirstPoint && c.secondPoint == intercostSecondPoint && c.thirdPoint == intercostThirdPoint);
                                    if (interCost == null)
                                    {
                                        _inter_cos_front = 0;
                                    }
                                    else
                                    {
                                        _inter_cos_front = interCost.cost;
                                    }
                                    //M0.02 end
                                }

                                int _inter_cos_back = 0;
                                if (backPath != null)
                                {
                                    //M0.02 start
                                    int intercostFirstPoint = addressIndexDic[alternativePath[0]];
                                    int intercostSecondPoint = addressIndexDic[alternativePath[1]];
                                    int intercostThirdPoint = addressIndexDic[backPath.path[1]];
                                    InterCost interCost = interCostsList.Find((InterCost c) => c.firstPoint == intercostFirstPoint && c.secondPoint == intercostSecondPoint && c.thirdPoint == intercostThirdPoint);
                                    if (interCost == null)
                                    {
                                        _inter_cos_back = 0;
                                    }
                                    else
                                    {
                                        _inter_cos_back = interCost.cost;
                                    }
                                    //M0.02 end
                                }



                                PathInfo pathInfo = new PathInfo();
                                int alternativePathCost = cost_matrix[addressIndexDic[alternativePath[0]]][addressIndexDic[alternativePath[1]]];
                                if (frontPath != null) pathInfo.total_cost += frontPath.total_cost;
                                pathInfo.total_cost += (_inter_cos_front + alternativePathCost + _inter_cos_back);
                                if (backPath != null) pathInfo.total_cost += backPath.total_cost;


                                List<int> tempCostdetail = new List<int>();
                                if (frontPath != null)
                                {
                                    tempCostdetail = frontPath.costDetail.ToList();
                                    tempCostdetail.RemoveAt(tempCostdetail.Count - 1);//前路徑CostDetail去掉結尾(被intercost置換了)
                                }
                                pathInfo.costDetail.AddRange(tempCostdetail);
                                if (frontPath != null)
                                {
                                    pathInfo.costDetail.Add(_inter_cos_front);
                                }
                                pathInfo.costDetail.Add(alternativePathCost);
                                pathInfo.costDetail.Add(_inter_cos_back);
                                if (backPath != null)
                                {
                                    pathInfo.costDetail.AddRange(backPath.costDetail);
                                }
                                int count = 0;
                                pathInfo.costAccumulation.Add(0);
                                foreach (int c in pathInfo.costDetail)
                                {
                                    count += c;
                                    pathInfo.costAccumulation.Add(count);
                                }
                                if (frontPath != null)
                                {
                                    pathInfo.path.AddRange(frontPath.path);
                                }
                                else
                                {
                                    pathInfo.path.Add(alternativePath[0]);
                                }
                                if (backPath != null)
                                {
                                    List<int> temppath = backPath.path.ToList();
                                    pathInfo.path.AddRange(temppath);
                                }
                                else
                                {
                                    pathInfo.path.Add(alternativePath[1]);
                                }

                                if (!pathInfo.isPathLoop() && !pathInfo.isPathEndBanned(banEndPatternList) && !pathInfo.isPathBanned(banPatternList))
                                {
                                    if (pathInfo_matrix[i][j].getPathInfotoList().Contains(pathInfo))
                                    {
                                        //break;
                                    }
                                    else
                                    {
                                        pathInfo_matrix[i][j].addtoInfotoListUnlimited(pathInfo);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion 加入替代道路

            #region 建立SecionToNode路徑
            //M0.03 start
            //pathInfo_forSecToNode_matrix = new PathInfos[sectionTotalCount][];
            //for (int i = 0; i < sectionTotalCount; i++)
            //{
            //    pathInfo_forSecToNode_matrix[i] = new PathInfos[n];
            //    for (int j = 0; j < n; j++)
            //    {
            //        pathInfo_forSecToNode_matrix[i][j] = new PathInfos(max_path_keep_num);
            //    }
            //}
            //for (int i = 0; i < sectionTotalCount; i++)
            //{
            //    for (int j = 0; j < n; j++)
            //    {

            //        Section section = sectionList[i];
            //        int statr_address_1 = section.address_1;
            //        int statr_address_2 = section.address_2;
            //        int start_index_1 = addressIndexDic[statr_address_1];
            //        int start_index_2 = addressIndexDic[statr_address_2];
            //        PathInfos pathInfos_1 = pathInfo_matrix[start_index_1][j];
            //        PathInfos pathInfos_2 = pathInfo_matrix[start_index_2][j];
            //        if (start_index_1 == j)//section與node直接相接
            //        {
            //            PathInfo new_pathInfo = new PathInfo();
            //            new_pathInfo.path.Add(statr_address_2);
            //            new_pathInfo.path.Add(statr_address_1);
            //            new_pathInfo.total_cost = cost_matrix[addressIndexDic[new_pathInfo.path[0]]][addressIndexDic[new_pathInfo.path[1]]];
            //            new_pathInfo.costDetail.Add(cost_matrix[addressIndexDic[new_pathInfo.path[0]]][addressIndexDic[new_pathInfo.path[1]]]);
            //            new_pathInfo.costDetail.Add(0);
            //            int count = 0;
            //            new_pathInfo.costAccumulation.Add(0);
            //            foreach (int c in new_pathInfo.costDetail)
            //            {
            //                count += c;
            //                new_pathInfo.costAccumulation.Add(count);
            //            }

            //            pathInfo_forSecToNode_matrix[i][j].addtoInfotoListUnlimited(new_pathInfo);

            //        }
            //        else
            //        {
            //            foreach (PathInfo pi in pathInfos_1.getPathInfotoList())
            //            {
            //                if (pi.path.Contains(statr_address_2)) continue;
            //                PathInfo new_pathInfo = new PathInfo();
            //                new_pathInfo.path.Add(statr_address_2);
            //                new_pathInfo.path.AddRange(pi.path);
            //                int inter_cost = 0;
            //                //M0.02 start
            //                int intercostFirstPoint = addressIndexDic[new_pathInfo.path[0]];
            //                int intercostSecondPoint = addressIndexDic[new_pathInfo.path[1]];
            //                int intercostThirdPoint = addressIndexDic[new_pathInfo.path[2]];
            //                InterCost interCost = interCostsList.Find((InterCost c) => c.firstPoint == intercostFirstPoint && c.secondPoint == intercostSecondPoint && c.thirdPoint == intercostThirdPoint);
            //                if (interCost == null)
            //                {
            //                    inter_cost = 0;
            //                }
            //                else
            //                {
            //                    inter_cost = interCost.cost;
            //                }
            //                //M0.02 end
            //                new_pathInfo.total_cost = cost_matrix[addressIndexDic[new_pathInfo.path[0]]][addressIndexDic[new_pathInfo.path[1]]] + inter_cost + pi.total_cost;

            //                new_pathInfo.costDetail.Add(cost_matrix[addressIndexDic[new_pathInfo.path[0]]][addressIndexDic[new_pathInfo.path[1]]]);
            //                new_pathInfo.costDetail.Add(inter_cost);
            //                new_pathInfo.costDetail.AddRange(pi.costDetail);
            //                int count = 0;
            //                new_pathInfo.costAccumulation.Add(0);
            //                foreach (int c in new_pathInfo.costDetail)
            //                {
            //                    count += c;
            //                    new_pathInfo.costAccumulation.Add(count);
            //                }
            //                if (!new_pathInfo.isPathLoop())
            //                {
            //                    pathInfo_forSecToNode_matrix[i][j].addtoInfotoListUnlimited(new_pathInfo);
            //                }
            //            }
            //        }
            //        if (start_index_2 == j)//section與node直接相接
            //        {
            //            PathInfo new_pathInfo = new PathInfo();
            //            new_pathInfo.path.Add(statr_address_1);
            //            new_pathInfo.path.Add(statr_address_2);
            //            new_pathInfo.total_cost = cost_matrix[addressIndexDic[new_pathInfo.path[0]]][addressIndexDic[new_pathInfo.path[1]]];
            //            new_pathInfo.costDetail.Add(cost_matrix[addressIndexDic[new_pathInfo.path[0]]][addressIndexDic[new_pathInfo.path[1]]]);
            //            new_pathInfo.costDetail.Add(0);
            //            int count = 0;
            //            new_pathInfo.costAccumulation.Add(0);
            //            foreach (int c in new_pathInfo.costDetail)
            //            {
            //                count += c;
            //                new_pathInfo.costAccumulation.Add(count);
            //            }
            //            pathInfo_forSecToNode_matrix[i][j].addtoInfotoListUnlimited(new_pathInfo);
            //        }
            //        else
            //        {
            //            foreach (PathInfo pi in pathInfos_2.getPathInfotoList())
            //            {
            //                if (pi.path.Contains(statr_address_1)) continue;
            //                PathInfo new_pathInfo = new PathInfo();
            //                new_pathInfo.path.Add(statr_address_1);
            //                new_pathInfo.path.AddRange(pi.path);
            //                int inter_cost = 0;
            //                //M0.02 start
            //                int intercostFirstPoint = addressIndexDic[new_pathInfo.path[0]];
            //                int intercostSecondPoint = addressIndexDic[new_pathInfo.path[1]];
            //                int intercostThirdPoint = addressIndexDic[new_pathInfo.path[2]];
            //                InterCost interCost = interCostsList.Find((InterCost c) => c.firstPoint == intercostFirstPoint && c.secondPoint == intercostSecondPoint && c.thirdPoint == intercostThirdPoint);
            //                if (interCost == null)
            //                {
            //                    inter_cost = 0;
            //                }
            //                else
            //                {
            //                    inter_cost = interCost.cost;
            //                }
            //                //M0.02 end
            //                new_pathInfo.total_cost = cost_matrix[addressIndexDic[new_pathInfo.path[0]]][addressIndexDic[new_pathInfo.path[1]]] + inter_cost + pi.total_cost;

            //                new_pathInfo.costDetail.Add(cost_matrix[addressIndexDic[new_pathInfo.path[0]]][addressIndexDic[new_pathInfo.path[1]]]);
            //                new_pathInfo.costDetail.Add(inter_cost);
            //                new_pathInfo.costDetail.AddRange(pi.costDetail);
            //                int count = 0;
            //                new_pathInfo.costAccumulation.Add(0);
            //                foreach (int c in new_pathInfo.costDetail)
            //                {
            //                    count += c;
            //                    new_pathInfo.costAccumulation.Add(count);
            //                }
            //                if (!new_pathInfo.isPathLoop())
            //                {
            //                    pathInfo_forSecToNode_matrix[i][j].addtoInfotoListUnlimited(new_pathInfo);
            //                }
            //            }
            //        }
            //    }
            //}
            //M0.03 end

            #endregion 建立SecionToNode路徑




            #region SectionToSection路徑加入替代道路
            //M0.03 start
            //for (int i = 0; i < sectionTotalCount; i++)
            //{
            //    for (int j = 0; j < sectionTotalCount; j++)
            //    {
            //        if (pathInfo_forSecToSec_matrix[i][j].getPathInfotoList().Count == 0) continue;
            //        PathInfo best_pathInfo = pathInfo_forSecToSec_matrix[i][j].getPathInfotoList().First();
            //        for (int x = 0; x < alternativePathConfigList.Count; x++)
            //        {
            //            for (int p = 0; p + 1 < best_pathInfo.path.Count; p++)
            //            {
            //                bool findMainSection = false;
            //                List<int> alternativePath = null;
            //                if (best_pathInfo.path[p + 0] == alternativePathConfigList[x][0] && best_pathInfo.path[p + 1] == alternativePathConfigList[x][1])
            //                {
            //                    findMainSection = true;
            //                    alternativePath = new List<int>();
            //                    alternativePath.Add(alternativePathConfigList[x][2]);
            //                    alternativePath.Add(alternativePathConfigList[x][3]);
            //                }
            //                else if (best_pathInfo.path[p + 0] == alternativePathConfigList[x][2] && best_pathInfo.path[p + 1] == alternativePathConfigList[x][3])
            //                {
            //                    findMainSection = true;
            //                    alternativePath = new List<int>();
            //                    alternativePath.Add(alternativePathConfigList[x][0]);
            //                    alternativePath.Add(alternativePathConfigList[x][1]);
            //                }
            //                else
            //                {
            //                    continue;
            //                }
            //                if (findMainSection)
            //                {
            //                    PathInfo frontPath = null;
            //                    PathInfo backPath = null;
            //                    bool frontPathEmtryIsWrong = false;
            //                    bool backPathEmtryIsWrong = false;

            //                    if (pathInfo_forSecToNode_matrix[i][addressIndexDic[alternativePath[0]]].getPathInfotoList().Count != 0)
            //                    {
            //                        foreach (PathInfo pi in pathInfo_forSecToNode_matrix[i][addressIndexDic[alternativePath[0]]].getPathInfotoList())
            //                        {
            //                            if (pi.path.Contains(alternativePath[1]))
            //                            {
            //                                frontPathEmtryIsWrong = true;
            //                                continue;
            //                            }
            //                            else
            //                            {
            //                                frontPath = pi;
            //                                frontPathEmtryIsWrong = false;
            //                                break;
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        //do nothing
            //                    }

            //                    if (frontPathEmtryIsWrong)
            //                    {
            //                        continue;
            //                    }
            //                    if (pathInfo_forNodeToSec_matrix[addressIndexDic[alternativePath[1]]][j].getPathInfotoList().Count != 0)
            //                    {
            //                        foreach (PathInfo pi in pathInfo_forNodeToSec_matrix[addressIndexDic[alternativePath[1]]][j].getPathInfotoList())
            //                        {
            //                            if (pi.path.Contains(alternativePath[0]))
            //                            {
            //                                backPathEmtryIsWrong = true;
            //                                continue;
            //                            }
            //                            else
            //                            {
            //                                backPath = pi;
            //                                backPathEmtryIsWrong = false;
            //                                break;
            //                            }
            //                        }
            //                    }
            //                    else
            //                    {
            //                        //do nothing
            //                    }

            //                    if (backPathEmtryIsWrong)
            //                    {
            //                        continue;
            //                    }

            //                    int _inter_cos_front = 0;
            //                    if (frontPath != null)
            //                    {
            //                        //M0.02 start
            //                        int intercostFirstPoint = addressIndexDic[frontPath.path[frontPath.path.Count - 2]];
            //                        int intercostSecondPoint = addressIndexDic[alternativePath[0]];
            //                        int intercostThirdPoint = addressIndexDic[alternativePath[1]];
            //                        InterCost interCost = interCostsList.Find((InterCost c) => c.firstPoint == intercostFirstPoint && c.secondPoint == intercostSecondPoint && c.thirdPoint == intercostThirdPoint);
            //                        if (interCost == null)
            //                        {
            //                            _inter_cos_front = 0;
            //                        }
            //                        else
            //                        {
            //                            _inter_cos_front = interCost.cost;
            //                        }
            //                        //M0.02 end
            //                    }

            //                    int _inter_cos_back = 0;
            //                    if (backPath != null)
            //                    {
            //                        //M0.02 start
            //                        int intercostFirstPoint = addressIndexDic[alternativePath[0]];
            //                        int intercostSecondPoint = addressIndexDic[alternativePath[1]];
            //                        int intercostThirdPoint = addressIndexDic[backPath.path[1]];
            //                        InterCost interCost = interCostsList.Find((InterCost c) => c.firstPoint == intercostFirstPoint && c.secondPoint == intercostSecondPoint && c.thirdPoint == intercostThirdPoint);
            //                        if (interCost == null)
            //                        {
            //                            _inter_cos_back = 0;
            //                        }
            //                        else
            //                        {
            //                            _inter_cos_back = interCost.cost;
            //                        }
            //                        //M0.02 end
            //                    }



            //                    PathInfo pathInfo = new PathInfo();
            //                    int alternativePathCost = cost_matrix[addressIndexDic[alternativePath[0]]][addressIndexDic[alternativePath[1]]];
            //                    if (frontPath != null) pathInfo.total_cost += frontPath.total_cost;
            //                    pathInfo.total_cost += (_inter_cos_front + alternativePathCost + _inter_cos_back);
            //                    if (backPath != null) pathInfo.total_cost += backPath.total_cost;


            //                    List<int> tempCostdetail = new List<int>();
            //                    if (frontPath != null)
            //                    {
            //                        tempCostdetail = frontPath.costDetail.ToList();
            //                        tempCostdetail.RemoveAt(tempCostdetail.Count - 1);//前路徑CostDetail去掉結尾(被intercost置換了)
            //                    }
            //                    pathInfo.costDetail.AddRange(tempCostdetail);
            //                    if (frontPath != null)
            //                    {
            //                        pathInfo.costDetail.Add(_inter_cos_front);
            //                    }
            //                    pathInfo.costDetail.Add(alternativePathCost);
            //                    pathInfo.costDetail.Add(_inter_cos_back);
            //                    if (backPath != null)
            //                    {
            //                        pathInfo.costDetail.AddRange(backPath.costDetail);
            //                    }
            //                    int count = 0;
            //                    pathInfo.costAccumulation.Add(0);
            //                    foreach (int c in pathInfo.costDetail)
            //                    {
            //                        count += c;
            //                        pathInfo.costAccumulation.Add(count);
            //                    }
            //                    if (frontPath != null)
            //                    {
            //                        pathInfo.path.AddRange(frontPath.path);
            //                    }
            //                    else
            //                    {
            //                        pathInfo.path.Add(alternativePath[0]);
            //                    }
            //                    if (backPath != null)
            //                    {
            //                        List<int> temppath = backPath.path.ToList();
            //                        pathInfo.path.AddRange(temppath);
            //                    }
            //                    else
            //                    {
            //                        pathInfo.path.Add(alternativePath[1]);
            //                    }

            //                    if (!pathInfo.isPathLoop())
            //                    {
            //                        if (pathInfo_forSecToSec_matrix[i][j].getPathInfotoList().Contains(pathInfo))
            //                        {
            //                            //break;
            //                        }
            //                        else
            //                        {
            //                            pathInfo_forSecToSec_matrix[i][j].addtoInfotoListUnlimited(pathInfo);
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //M0.03 end

            #endregion SectionToSection路徑加入替代道路

            //M0.01 Start
            //#region 紀錄StartFromTo路徑資訊
            //startFromToPathInfosList = new List<StartFromToPathInfos>();
            //for (int i = 0; i < n; i++)
            //{
            //    for (int j = 0; j < n; j++)
            //    {
            //        if (i == j) continue;//起點不能跟中繼點相同
            //        LinkedList<PathInfo> startFromPathInfos = pathInfo_matrix[i][j].getPathInfotoList();
            //        //StartFromToPathInfos startFromToPathInfos = new StartFromToPathInfos()

            //        for (int k = 0; k < n; k++)
            //        {
            //            if (j == k) continue;//中繼點不能跟終點相同

            //            LinkedList<PathInfo> fromToPathInfos = pathInfo_matrix[j][k].getPathInfotoList();
            //            List<int> pattern = new List<int>();
            //            pattern.Add(i);
            //            pattern.Add(j);
            //            pattern.Add(k);
            //            StartFromToPathInfos startFromToPathInfos = new StartFromToPathInfos(pattern, max_path_keep_num);

            //            foreach (PathInfo sf in startFromPathInfos)
            //            {
            //                foreach (PathInfo ft in fromToPathInfos)
            //                {
            //                    if (ft.costDetail.Count == 0)
            //                    {

            //                    }
            //                    StartFromToPathInfo startFromToPathInfo = new StartFromToPathInfo(sf, ft, stay_cost);
            //                    if (!startFromToPathInfo.isPathBanned(banPatternList))
            //                    {
            //                        startFromToPathInfos.addtoInfotoList(startFromToPathInfo);
            //                    }
            //                }
            //            }
            //            startFromToPathInfosList.Add(startFromToPathInfos);
            //        }
            //    }
            //}
            //#endregion 紀錄StartFromTo路徑資訊
            //M0.01 End

        }
        private void EstablishInterconnectedPaths()//根據Section List建立直接相連的路徑
        {
            pathInfo_matrix = new PathInfos[n][];
            for (int i = 0; i < n; i++)
            {
                pathInfo_matrix[i] = new PathInfos[n];
                for (int j = 0; j < n; j++)
                {
                    pathInfo_matrix[i][j] = new PathInfos(max_path_keep_num);
                }
            }
            foreach (Section s in sectionList)
            {
                PathInfo pathInfo_1 = new PathInfo();
                pathInfo_1.total_cost = s.moveCost_1;
                pathInfo_1.costDetail.Add(s.moveCost_1);
                pathInfo_1.costDetail.Add(0);
                int count = 0;
                pathInfo_1.costAccumulation.Add(0);
                foreach (int c in pathInfo_1.costDetail)
                {
                    count += c;
                    pathInfo_1.costAccumulation.Add(count);
                }
                int index_1 = addressIndexDic[s.address_1];
                int index_2 = addressIndexDic[s.address_2];
                pathInfo_1.path = new List<int>();
                pathInfo_1.path.Add(s.address_1);
                pathInfo_1.path.Add(s.address_2);
                pathInfo_matrix[index_1][index_2].addtoInfotoListLimited(pathInfo_1);

                PathInfo pathInfo_2 = new PathInfo();
                pathInfo_2.total_cost = s.moveCost_2;
                pathInfo_2.costDetail.Add(s.moveCost_2);
                pathInfo_2.costDetail.Add(0);
                count = 0;
                pathInfo_2.costAccumulation.Add(0);
                foreach (int c in pathInfo_2.costDetail)
                {
                    count += c;
                    pathInfo_2.costAccumulation.Add(count);
                }
                pathInfo_2.path = new List<int>();
                pathInfo_2.path.Add(s.address_2);
                pathInfo_2.path.Add(s.address_1);
                pathInfo_matrix[index_2][index_1].addtoInfotoListLimited(pathInfo_2);
            }
        }
        /// <summary>
        /// 預先確認要將那些路徑Ban
        /// 1.禁止直接橫移到Port的路徑
        /// </summary>
        private void PresetBanEndPattern(List<Section> sections)
        {
            banEndPatternList.Clear();
            foreach (var sec in sections)
            {
                if (sec.isBanEnd_From2To)
                {
                    List<int> banEndPattern = new List<int>();
                    banEndPattern.Add(sec.address_1);
                    banEndPattern.Add(sec.address_2);

                    banEndPatternList.Add(banEndPattern);
                }
                if (sec.isBanEnd_To2From)
                {
                    List<int> banEndPattern = new List<int>();
                    banEndPattern.Add(sec.address_2);
                    banEndPattern.Add(sec.address_1);
                    banEndPatternList.Add(banEndPattern);
                }
            }
        }

        private void DecisionCostWeight(out double movecostF_weight, out double movecostR_weight)
        {
            switch (BCID)
            {
                //case App.SCAppConstants.WorkVersion.VERSION_NAME_AUO_CAAGV100_Beta:
                //    movecostF_weight = 0.1;
                //    movecostR_weight = 10;
                //    break;
                case App.SCAppConstants.WorkVersion.VERSION_NAME_ASE:
                    movecostF_weight = 1;
                    movecostR_weight = 1;
                    break;
                default:
                    movecostF_weight = 0.7;
                    movecostR_weight = 1;
                    break;
            }
        }

        private bool isByPass(int adr1, int adr2)
        {
            int[] bypass_adr = new[] { 48048, 48047, 48050, 48053, 48054, 48057, 48058 };
            if (bypass_adr.Contains(adr1)) return true;
            if (bypass_adr.Contains(adr2)) return true;
            return false;

        }
        private void initialIndexDic(List<Section> sectionList)
        {
            SortedSet<int> addressSet = new SortedSet<int>();
            sectionIndexDic = new Dictionary<int, int>();
            for (int i = 0; i < sectionList.Count; i++)
            {
                int section_id = Convert.ToInt32(sectionList[i].section_id);
                sectionIndexDic.Add(section_id, i);
                addressSet.Add(sectionList[i].address_1);
                addressSet.Add(sectionList[i].address_2);
            }
            addressIndexDic = new Dictionary<int, int>();
            IndexAddressDic = new Dictionary<int, int>();
            int index = 0;
            foreach (int address in addressSet)
            {
                addressIndexDic.Add(address, index);
                IndexAddressDic.Add(index, address);
                index++;
            }

        }


        private void fillCostMatrix(List<Section> sectionList)
        {
            cost_matrix = new int[n][];
            for (int i = 0; i < n; i++)
            {
                cost_matrix[i] = new int[n];
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    cost_matrix[i][j] = 999999;
                }
            }
            foreach (Section s in sectionList)
            {
                int index_1 = addressIndexDic[s.address_1];
                int index_2 = addressIndexDic[s.address_2];

                cost_matrix[index_1][index_2] = s.moveCost_1;
                cost_matrix[index_2][index_1] = s.moveCost_2;
            }
        }
        private void createInterCostList(List<Section> sectionList)
        {
            interCostsList = new List<InterCost>();
            foreach (Section s in sectionList)
            {
                if (s.changeSec_1 != null)
                {
                    for (int i = 0; i < sectionList.Count; i++)
                    {

                        if (sectionList[i].section_id == s.changeSec_1.changeSec)
                        {
                            int last_address;
                            int index_1 = addressIndexDic[s.address_2];
                            int index_2 = addressIndexDic[s.address_1];
                            if (sectionList[i].address_1 == s.address_1)
                            {
                                last_address = sectionList[i].address_2;
                            }
                            else
                            {
                                last_address = sectionList[i].address_1;
                            }
                            int index_3 = addressIndexDic[last_address];
                            InterCost interCost = new InterCost(s.changeSec_1.interCost, index_1, index_2, index_3);
                            interCostsList.Add(interCost);
                            break;
                        }
                    }
                }
                if (s.changeSec_2 != null)
                {
                    for (int i = 0; i < sectionList.Count; i++)
                    {
                        if (sectionList[i].section_id == s.changeSec_2.changeSec)
                        {
                            int last_address;
                            int index_1 = addressIndexDic[s.address_2];
                            int index_2 = addressIndexDic[s.address_1];
                            if (sectionList[i].address_1 == s.address_1)
                            {
                                last_address = sectionList[i].address_2;
                            }
                            else
                            {
                                last_address = sectionList[i].address_1;
                            }
                            int index_3 = addressIndexDic[last_address];
                            InterCost interCost = new InterCost(s.changeSec_2.interCost, index_1, index_2, index_3);
                            interCostsList.Add(interCost);
                            break;
                        }
                    }
                }
                if (s.changeSec_3 != null)
                {
                    for (int i = 0; i < sectionList.Count; i++)
                    {
                        if (sectionList[i].section_id == s.changeSec_3.changeSec)
                        {
                            int last_address;
                            int index_1 = addressIndexDic[s.address_2];
                            int index_2 = addressIndexDic[s.address_1];
                            if (sectionList[i].address_1 == s.address_1)
                            {
                                last_address = sectionList[i].address_2;
                            }
                            else
                            {
                                last_address = sectionList[i].address_1;
                            }
                            int index_3 = addressIndexDic[last_address];
                            InterCost interCost = new InterCost(s.changeSec_3.interCost, index_1, index_2, index_3);
                            interCostsList.Add(interCost);
                            break;
                        }
                    }
                }
                if (s.changeSec_4 != null)
                {
                    for (int i = 0; i < sectionList.Count; i++)
                    {
                        if (sectionList[i].section_id == s.changeSec_4.changeSec)
                        {
                            int last_address;
                            int index_1 = addressIndexDic[s.address_1];
                            int index_2 = addressIndexDic[s.address_2];
                            if (sectionList[i].address_1 == s.address_2)
                            {
                                last_address = sectionList[i].address_2;
                            }
                            else
                            {
                                last_address = sectionList[i].address_1;
                            }
                            int index_3 = addressIndexDic[last_address];
                            InterCost interCost = new InterCost(s.changeSec_4.interCost, index_1, index_2, index_3);
                            interCostsList.Add(interCost);
                            break;
                        }
                    }
                }
                if (s.changeSec_5 != null)
                {
                    for (int i = 0; i < sectionList.Count; i++)
                    {
                        if (sectionList[i].section_id == s.changeSec_5.changeSec)
                        {
                            int last_address;
                            int index_1 = addressIndexDic[s.address_1];
                            int index_2 = addressIndexDic[s.address_2];
                            if (sectionList[i].address_1 == s.address_2)
                            {
                                last_address = sectionList[i].address_2;
                            }
                            else
                            {
                                last_address = sectionList[i].address_1;
                            }
                            int index_3 = addressIndexDic[last_address];
                            InterCost interCost = new InterCost(s.changeSec_5.interCost, index_1, index_2, index_3);
                            interCostsList.Add(interCost);
                            break;
                        }
                    }
                }
                if (s.changeSec_6 != null)
                {
                    for (int i = 0; i < sectionList.Count; i++)
                    {
                        if (sectionList[i].section_id == s.changeSec_6.changeSec)
                        {
                            int last_address;
                            int index_1 = addressIndexDic[s.address_1];
                            int index_2 = addressIndexDic[s.address_2];
                            if (sectionList[i].address_1 == s.address_2)
                            {
                                last_address = sectionList[i].address_2;
                            }
                            else
                            {
                                last_address = sectionList[i].address_1;
                            }
                            int index_3 = addressIndexDic[last_address];
                            InterCost interCost = new InterCost(s.changeSec_6.interCost, index_1, index_2, index_3);
                            interCostsList.Add(interCost);
                            break;
                        }
                    }
                }
            }
        }

        private bool findLeastOverlapCostCombination(List<PathInfos> pathInfos, out int overlapCost, out List<PathInfo> chosenPaths, out List<int> overlapCostList)
        {
            overlapCost = 999999;
            chosenPaths = null;
            overlapCostList = new List<int>();

            List<PathInfo> currentPaths = new List<PathInfo>();
            List<List<int>> combinationList = new List<List<int>>();
            combinationList = getCombinationList(pathInfos);

            for (int i = 0; i < combinationList.Count; i++)
            {
                for (int j = 0; j < combinationList[i].Count; j++)
                {
                    PathInfo pathinfo = pathInfos[j].getNotBannedPathInfoList(banPatternList).ToList()[combinationList[i][j]];
                    currentPaths.Add(pathinfo);
                }
                int temp = culculateOverlapCost(currentPaths);
                overlapCostList.Add(temp);
                if (temp < overlapCost)
                {
                    overlapCost = temp;
                    chosenPaths = currentPaths.ToList();
                }
                currentPaths.Clear();
            }
            overlapCostList.Sort();
            return true;
        }
        private bool findLeastOverlapCostCombination_Ver2(List<PathInfos> pathInfos, out int overlapCost, out List<PathInfo> chosenPaths, out List<int> overlapCostList)
        {
            overlapCost = 999999;
            chosenPaths = null;
            overlapCostList = new List<int>();

            List<PathInfo> currentPaths = new List<PathInfo>();
            List<List<int>> combinationList = new List<List<int>>();
            List<int> bestCombination = null;
            int round = 1;
            while (round != pathInfos.Count)
            {
                overlapCost = 999999;
                combinationList = getCombinationList(pathInfos, bestCombination);

                for (int i = 0; i < combinationList.Count; i++)
                {
                    for (int j = 0; j < combinationList[i].Count; j++)
                    {
                        PathInfo pathinfo = pathInfos[j].getNotBannedPathInfoList(banPatternList)[combinationList[i][j]];
                        //PathInfo pathinfo = pathInfos[j].getPathInfotoList().ToList()[combinationList[i][j]];
                        currentPaths.Add(pathinfo);
                    }
                    int temp = culculateOverlapCost(currentPaths);
                    overlapCostList.Add(temp);
                    if (temp < overlapCost)
                    {
                        bestCombination = combinationList[i];
                        overlapCost = temp;
                        chosenPaths = currentPaths.ToList();
                    }
                    currentPaths.Clear();
                }
                round++;
            }
            overlapCostList.Sort();
            return true;
        }

        private bool findLeastOverlapCostCombination_Ver2(List<TimeWindowPathIfos> pathInfos, out int overlapCost, out List<PathInfo> chosenPaths, out List<int> overlapCostList)
        {
            overlapCost = 999999;
            chosenPaths = null;
            overlapCostList = new List<int>();

            List<PathInfo> currentPaths = new List<PathInfo>();
            List<List<int>> combinationList = new List<List<int>>();
            List<int> bestCombination = null;
            int round = 1;
            while (round != pathInfos.Count)
            {
                overlapCost = 999999;
                combinationList = getCombinationList(pathInfos, bestCombination);

                for (int i = 0; i < combinationList.Count; i++)
                {
                    for (int j = 0; j < combinationList[i].Count; j++)
                    {
                        PathInfo pathinfo = pathInfos[j].pathInfos[combinationList[i][j]];
                        //PathInfo pathinfo = pathInfos[j].getPathInfotoList().ToList()[combinationList[i][j]];
                        currentPaths.Add(pathinfo);
                    }
                    int temp = culculateOverlapCost(currentPaths);
                    overlapCostList.Add(temp);
                    if (temp < overlapCost)
                    {
                        bestCombination = combinationList[i];
                        overlapCost = temp;
                        chosenPaths = currentPaths.ToList();
                    }
                    currentPaths.Clear();
                }
                round++;
            }
            overlapCostList.Sort();
            return true;
        }
        private List<List<int>> getCombinationList(List<PathInfos> pathInfos, List<int> pre_combination)
        {
            List<List<int>> combinationList = new List<List<int>>();

            if (pre_combination != null)
            {
                int index = pre_combination.Count;
                for (int i = 0; i < pathInfos[index].getNotBannedPathInfoList(banPatternList).Count; i++)
                {
                    List<int> combination = pre_combination.ToList();
                    combination.Add(i);
                    combinationList.Add(combination);
                }
            }
            else
            {
                for (int i = 0; i < pathInfos[1].getNotBannedPathInfoList(banPatternList).Count; i++)
                {
                    List<int> combination = new List<int>();
                    combination.Add(0);
                    combination.Add(i);
                    combinationList.Add(combination);
                }
            }
            return combinationList;
        }

        private List<List<int>> getCombinationList(List<TimeWindowPathIfos> pathInfos, List<int> pre_combination)
        {
            List<List<int>> combinationList = new List<List<int>>();

            if (pre_combination != null)
            {
                int index = pre_combination.Count;
                for (int i = 0; i < pathInfos[index].pathInfos.Count; i++)
                {
                    List<int> combination = pre_combination.ToList();
                    combination.Add(i);
                    combinationList.Add(combination);
                }
            }
            else
            {
                for (int i = 0; i < pathInfos[1].pathInfos.Count; i++)
                {
                    List<int> combination = new List<int>();
                    combination.Add(0);
                    combination.Add(i);
                    combinationList.Add(combination);
                }
            }
            return combinationList;
        }

        private int culculateOverlapCost(List<PathInfo> pathinfos)
        {
            int cost = 0;
            List<ActionInfos> actionInfosList = new List<ActionInfos>();
            #region 準備action資料
            for (int i = 0; i < pathinfos.Count; i++)
            {
                int index = 0;
                for (int j = 0; j < pathinfos[i].path.Count; j++)
                {
                    if (j != pathinfos[i].path.Count - 1)
                    {
                        List<int> move = new List<int>();
                        move.Add(pathinfos[i].path[j]);
                        move.Add(pathinfos[i].path[j + 1]);
                        ActionInfo actionInfo = new ActionInfo(pathinfos[i].costAccumulation[index], pathinfos[i].costDetail[index]);
                        index++;

                        //for(int x = 0;x<actionInfosList.Count)
                        bool actions_exist = false;
                        foreach (ActionInfos acts in actionInfosList)
                        {
                            if (acts.isMatch(move))
                            {
                                acts.actionInfoList.Add(actionInfo);
                                actions_exist = true;
                                break;
                            }
                        }
                        if (!actions_exist)
                        {
                            ActionInfos actionInfos = new ActionInfos(move);
                            actionInfos.actionInfoList.Add(actionInfo);
                            actionInfosList.Add(actionInfos);
                        }

                        //if (j != pathinfos[i].path.Count - 2)
                        //{
                        List<int> stay = new List<int>();
                        stay.Add(pathinfos[i].path[j + 1]);
                        ActionInfo stayInfo = new ActionInfo(pathinfos[i].costAccumulation[index], pathinfos[i].costDetail[index]);
                        index++;
                        bool moves_exist = false;
                        foreach (ActionInfos acts in actionInfosList)
                        {
                            if (acts.isMatch(stay))
                            {
                                acts.actionInfoList.Add(stayInfo);
                                moves_exist = true;
                                break;
                            }
                        }
                        if (!moves_exist)
                        {
                            ActionInfos actionInfos = new ActionInfos(stay);
                            actionInfos.actionInfoList.Add(stayInfo);
                            actionInfosList.Add(actionInfos);
                        }
                        //}

                    }

                }
            }
            #endregion 準備action資料
            foreach (ActionInfos actinfos in actionInfosList)
            {
                for (int i = 0; i < actinfos.actionInfoList.Count; i++)
                {
                    for (int j = i + 1; j < actinfos.actionInfoList.Count; j++)
                    {
                        if (actinfos.actionInfoList[i].start_time == actinfos.actionInfoList[j].start_time)
                        {
                            if (actinfos.actionInfoList[i].end_time < actinfos.actionInfoList[j].end_time)
                            {
                                cost += actinfos.actionInfoList[i].cost;
                                continue;
                            }
                            else if (actinfos.actionInfoList[i].end_time > actinfos.actionInfoList[j].end_time)
                            {
                                cost += actinfos.actionInfoList[j].cost;
                                continue;
                            }
                            else
                            {
                                cost += actinfos.actionInfoList[i].cost;
                                continue;
                            }
                        }
                        else if (actinfos.actionInfoList[i].end_time == actinfos.actionInfoList[j].end_time)
                        {
                            if (actinfos.actionInfoList[i].start_time < actinfos.actionInfoList[j].start_time)
                            {
                                cost += actinfos.actionInfoList[j].cost;
                                continue;
                            }
                            else if (actinfos.actionInfoList[i].start_time > actinfos.actionInfoList[j].start_time)
                            {
                                cost += actinfos.actionInfoList[i].cost;
                                continue;
                            }
                            else
                            {
                                cost += actinfos.actionInfoList[i].cost;
                                continue;
                            }
                        }
                        else
                        {
                            if (actinfos.actionInfoList[i].start_time < actinfos.actionInfoList[j].start_time)
                            {

                                if (actinfos.actionInfoList[i].end_time < actinfos.actionInfoList[j].start_time)
                                {
                                    continue;
                                }
                                else if (actinfos.actionInfoList[i].end_time > actinfos.actionInfoList[j].start_time)
                                {
                                    if (actinfos.actionInfoList[i].end_time < actinfos.actionInfoList[j].end_time)
                                    {
                                        cost += (actinfos.actionInfoList[i].end_time - actinfos.actionInfoList[j].start_time);
                                        continue;
                                    }
                                    else
                                    {
                                        cost += actinfos.actionInfoList[j].cost;
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else if (actinfos.actionInfoList[i].start_time > actinfos.actionInfoList[j].start_time)
                            {

                                if (actinfos.actionInfoList[j].end_time < actinfos.actionInfoList[i].start_time)
                                {
                                    continue;
                                }
                                else if (actinfos.actionInfoList[j].end_time > actinfos.actionInfoList[i].start_time)
                                {
                                    if (actinfos.actionInfoList[j].end_time < actinfos.actionInfoList[i].end_time)
                                    {
                                        cost += (actinfos.actionInfoList[j].end_time - actinfos.actionInfoList[i].start_time);
                                        continue;
                                    }
                                    else
                                    {
                                        cost += actinfos.actionInfoList[i].cost;
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                continue;
                            }

                        }
                    }
                }
            }

            return cost;
        }
        private List<List<int>> getCombinationList(List<PathInfos> pathInfos)
        {
            List<List<int>> combinationList = null;

            if (pathInfos.Count > 1)
            {
                List<PathInfos> _pathInfos = pathInfos.ToList();
                _pathInfos.RemoveAt(_pathInfos.Count - 1);
                List<List<int>> oldCombinationList = getCombinationList(_pathInfos);
                combinationList = new List<List<int>>();

                foreach (List<int> com in oldCombinationList)
                {
                    for (int i = 0; i < pathInfos[pathInfos.Count - 1].getNotBannedPathInfoList(banPatternList).Count; i++)
                    {
                        List<int> _com = com.ToList();
                        _com.Add(i);
                        combinationList.Add(_com);
                    }
                }
            }
            else
            {
                combinationList = new List<List<int>>();
                for (int i = 0; i < pathInfos[0].getNotBannedPathInfoList(banPatternList).Count; i++)
                {
                    List<int> combination = new List<int>();
                    combination.Add(i);
                    combinationList.Add(combination);
                }
            }
            return combinationList;
        }

        public class TimeWindow
        {
            private List<TimeWindowPathIfos> pathInfosList = new List<TimeWindowPathIfos>();//存放待執行計算的命令路徑列表的List
            private FloydAlgorithmRouteGuide floydAlgorithm;
            public List<TimeWindowResult> computeResult = new List<TimeWindowResult>();//存放TimeWindow計算出來的最佳組合結果。

            public TimeWindow(FloydAlgorithmRouteGuide floydAlgorithm)
            {
                this.floydAlgorithm = floydAlgorithm;
            }

            //加入一筆待計算的命令訊息至TimeWindow中，vh_id為執行該命令的車輛ID，curr_sec_id為目前車輛所在的位置的Section ID，path為命令的路徑訊息(應為一連串的Section ID)
            public void addCMDInfoCurrIsSection(string vh_id, int curr_sec_id, int to_address, List<int> path)
            {
                //int index_from = floydAlgorithm.sectionIndexDic[curr_sec_id];
                //int index_to = floydAlgorithm.addressIndexDic[to_address];
                List<PathInfo> pathInfos = floydAlgorithm.getFromToPathInfoListSectionToAdr(curr_sec_id, to_address);
                TimeWindowPathIfos timeWindowPathIfos = new TimeWindowPathIfos(vh_id, path, pathInfos);
                pathInfosList.Add(timeWindowPathIfos);
            }
            //加入一筆待計算的命令訊息至TimeWindow中，類似addCMDInfoCurrIsSection方法，僅有車輛所在位置是用Address代入。
            public void addCMDInfoCurrIsAddress(string vh_id, int curr_address, int to_address, List<int> path)
            {
                //int index_from = floydAlgorithm.addressIndexDic[curr_address];
                //int index_to = floydAlgorithm.addressIndexDic[to_address];
                List<PathInfo> pathInfos = floydAlgorithm.getFromToPathInfoListAddrToAddr(curr_address, to_address);
                TimeWindowPathIfos timeWindowPathIfos = new TimeWindowPathIfos(vh_id, path, pathInfos);
                pathInfosList.Add(timeWindowPathIfos);
            }
            //清除目前已經加入TimeWindow的命令訊息
            public void clearAllCMDInfo()
            {
                pathInfosList.Clear();
                computeResult.Clear();
            }
            //計算TimeWindow
            public List<RouteInfo> Compute()
            {
                computeResult.Clear();
                if (pathInfosList.Count == 0) return null;

                int bestCost;
                List<PathInfo> bestCombination;
                List<int> overlapCostList;


                floydAlgorithm.findLeastOverlapCostCombination_Ver2(pathInfosList, out bestCost, out bestCombination, out overlapCostList);
                List<RouteInfo> routeList = new List<RouteInfo>();

                for (int i = 0; i < bestCombination.Count; i++)
                {
                    List<Section> sectionList = new List<Section>();
                    routeList.Add(new RouteInfo(sectionList, bestCombination[i].path, bestCombination[i].total_cost));

                    for (int j = 0; j < bestCombination[i].path.Count - 1; j++)
                    {
                        Section section = floydAlgorithm.getSectionByTwoNode(bestCombination[i].path[j], bestCombination[i].path[j + 1]);
                        sectionList.Add(section);
                    }
                    List<int> sectionIDList = new List<int>();
                    foreach (Section s in sectionList)
                    {
                        sectionIDList.Add(int.Parse(s.section_id));
                    }
                    TimeWindowResult tr = new TimeWindowResult(pathInfosList[i].vehicle_id, pathInfosList[i].original_pathinfo, sectionIDList);
                    computeResult.Add(tr);
                }

                return routeList;


            }
        }
        public class TimeWindowResult//用於TimeWindow計算後的單筆命令結果
        {
            public string vehicle_id;//車輛ID
            public List<int> original_pathinfo;//該車輛在計算Timewindow前的原先規畫路徑
            public List<int> new_pathInfo;//計算Timewindow後該車輛的最佳路徑
            public bool isChangePath;//該路徑跟原先規畫路徑是否有變更
            public TimeWindowResult(string vehicle_id, List<int> original_pathinfo, List<int> new_pathInfo)
            {
                this.vehicle_id = vehicle_id;
                this.original_pathinfo = original_pathinfo;
                this.new_pathInfo = new_pathInfo;
                isChangePath = true;
                if (original_pathinfo == null || new_pathInfo == null)
                {
                    isChangePath = false;
                    return;
                }
                else
                {
                    int startSecID = new_pathInfo[0];
                    for (int i = 0; i < original_pathinfo.Count - new_pathInfo.Count + 1; i++)
                    {
                        if (original_pathinfo[i] == startSecID)
                        {
                            List<int> tempPathInfo = original_pathinfo.GetRange(i, original_pathinfo.Count - i);
                            if (tempPathInfo.SequenceEqual(new_pathInfo))
                            {
                                isChangePath = false;
                                return;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
        }

        class TimeWindowPathIfos//用於TimeWindow計算時會代入的單筆路徑資訊
        {
            public string vehicle_id;//車輛ID
            public List<int> original_pathinfo;//該車輛在計算Timewindow前的原先規畫路徑
            public List<PathInfo> pathInfos;//車輛當前位置到終點的可能路徑集合
            public TimeWindowPathIfos(string vehicle_id, List<int> original_pathinfo, List<PathInfo> pathInfos)
            {
                this.vehicle_id = vehicle_id;
                this.original_pathinfo = original_pathinfo;
                this.pathInfos = pathInfos;
            }
        }

    }

    class ActionInfos
    {
        public List<ActionInfo> actionInfoList;

        public List<int> pattern;//用於比對是否為同類的StartFromTo路徑

        public ActionInfos(List<int> _pattern)
        {
            this.pattern = _pattern;
            actionInfoList = new List<ActionInfo>();
        }

        public bool isMatch(List<int> _pattern)
        {
            if (this.pattern.SequenceEqual(_pattern))
            {
                return true;
            }
            _pattern.Reverse();//方向相反同樣需要占用路徑
            if (this.pattern.SequenceEqual(_pattern))
            {
                return true;
            }
            return false;
        }
    }

    class ActionInfo
    {
        public int cost;
        public int start_time;
        public int end_time;
        public ActionInfo(int start_time, int cost)
        {
            this.cost = cost;
            this.start_time = start_time;
            end_time = start_time + cost;
        }
    }


    public class PathInfo : IEquatable<PathInfo>//單點到單點的路徑資訊
    {
        public int total_cost;
        public List<int> costDetail;
        public List<int> path;
        public int startDelayTime;
        public List<int> costAccumulation;

        public PathInfo()
        {

            costDetail = new List<int>();
            costAccumulation = new List<int>();
            path = new List<int>();
        }

        public PathInfo(int total_cost, List<int> path)
        {
            this.total_cost = total_cost;
            this.path = path;
        }

        public bool isPathLoop()
        {
            if (path.Count < 3)//少於3個節點不可能loop
            {
                return false;
            }
            else
            {
                List<int> temp = this.path.ToList();
                //temp.RemoveAt(0);
                while (temp.Count > 0)
                {
                    int x = temp[0];
                    temp.RemoveAt(0);
                    if (temp.Contains(x))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool isPathBanned(List<List<int>> banPatternList)
        {

            for (int i = 0; i < banPatternList.Count; i++)
            {
                for (int j = 0; j + banPatternList[i].Count - 1 < path.Count; j++)
                {
                    bool findBannedPattern = true;
                    for (int k = 0; k < banPatternList[i].Count(); k++)
                    {
                        if (path[j + k] == banPatternList[i][k])
                        {
                            continue;
                        }
                        else
                        {
                            findBannedPattern = false;
                            break;
                        }
                    }
                    if (findBannedPattern)
                    {
                        return true;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return false;
        }

        public bool isPathEndBanned(List<List<int>> banPatternList)//某些特定的路徑結尾是不能使用的
        {

            for (int i = 0; i < banPatternList.Count; i++)
            {

                bool findBannedPattern = true;
                for (int k = 0; k < banPatternList[i].Count(); k++)
                {
                    if (path[(path.Count - banPatternList[i].Count()) + k] == banPatternList[i][k])
                    {
                        continue;
                    }
                    else
                    {
                        findBannedPattern = false;
                        break;
                    }
                }
                if (findBannedPattern)
                {
                    return true;
                }
                else
                {
                    continue;
                }

            }
            return false;
        }

        public bool Equals(PathInfo other)
        {
            if (this.total_cost != other.total_cost)
            {
                return false;
            }
            if (!this.path.SequenceEqual(other.path))
            {
                return false;
            }
            return true;
        }
    }

    class PathInfos : ICloneable
    {
        protected int max_path_keep_num;
        protected LinkedList<PathInfo> pathInfoList;
        public PathInfos(int max_path_keep_num)
        {
            this.max_path_keep_num = max_path_keep_num;
            pathInfoList = new LinkedList<PathInfo>();
        }
        public LinkedList<PathInfo> getPathInfotoList()
        {
            return pathInfoList;
        }
        public List<PathInfo> getNotBannedPathInfoList(List<List<int>> banpatternList)
        {
            List<PathInfo> temps = pathInfoList.ToList();
            List<PathInfo> pathInfos = new List<PathInfo>();
            foreach (PathInfo pi in temps)
            {
                if (!pi.isPathBanned(banpatternList))
                {
                    pathInfos.Add(pi);
                }
            }
            return pathInfos;
        }
        public void addtoInfotoListLimited(PathInfo pathInfo)
        {
            bool insert_already = false;
            foreach (PathInfo info in pathInfoList)//把路徑放進linkedList裡(有排序)
            {
                if (pathInfo.total_cost < info.total_cost)
                {
                    LinkedListNode<PathInfo> node = pathInfoList.Find(info);
                    pathInfoList.AddBefore(node, pathInfo);
                    insert_already = true;
                    if (pathInfoList.Count > max_path_keep_num)
                    {
                        pathInfoList.RemoveLast();
                    }
                    break;
                }
            }
            if (insert_already == false)//沒有路徑比新路徑慢，加到最後面
            {
                if (pathInfoList.Count < max_path_keep_num)
                {
                    pathInfoList.AddLast(pathInfo);
                }
            }
        }

        public void addtoInfotoListUnlimited(PathInfo pathInfo)
        {
            bool insert_already = false;
            foreach (PathInfo info in pathInfoList)//把路徑放進linkedList裡(有排序)
            {
                if (pathInfo.total_cost < info.total_cost)
                {
                    LinkedListNode<PathInfo> node = pathInfoList.Find(info);
                    pathInfoList.AddBefore(node, pathInfo);
                    insert_already = true;
                    break;
                }
            }
            if (insert_already == false)//沒有路徑比新路徑慢，加到最後面
            {
                pathInfoList.AddLast(pathInfo);
            }
        }

        public void removePathInfo(PathInfo pathInfo)
        {
            pathInfoList.Remove(pathInfo);
        }

        public void addStartDelayTime(int delay)
        {
            foreach (PathInfo p in pathInfoList)
            {
                p.startDelayTime += delay;
                for (int i = 0; i < p.costAccumulation.Count; i++)
                {
                    p.costAccumulation[i] += delay;
                }
            }
        }

        public object Clone()
        {
            PathInfos pathInfos = new PathInfos(this.max_path_keep_num);
            foreach (PathInfo p in pathInfoList)
            {
                PathInfo pathInfo = new PathInfo();
                pathInfo.total_cost = p.total_cost;
                pathInfo.startDelayTime = p.startDelayTime;

                for (int i = 0; i < p.costDetail.Count; i++)
                {
                    pathInfo.costDetail.Add(p.costDetail[i]);
                }
                for (int i = 0; i < p.costAccumulation.Count; i++)
                {
                    pathInfo.costAccumulation.Add(p.costAccumulation[i]);
                }
                for (int i = 0; i < p.path.Count; i++)
                {
                    pathInfo.path.Add(p.path[i]);
                }
                pathInfos.pathInfoList.AddLast(pathInfo);
            }
            return pathInfos;
        }
    }


    class StartFromToPathInfo : PathInfo, IEquatable<StartFromToPathInfo>//包含StartFromTo的路徑資訊
    {
        PathInfo startFromPathInfo;//起點到中繼點的路徑資訊
        PathInfo fromToPathInfo;//中繼點到終點的路徑資訊


        public StartFromToPathInfo(PathInfo startFromPathInfo, PathInfo fromToPathInfo, int stay_cost)
        {
            this.startFromPathInfo = startFromPathInfo;
            this.fromToPathInfo = fromToPathInfo;
            total_cost = startFromPathInfo.total_cost + stay_cost + fromToPathInfo.total_cost + stay_cost;
            costDetail = new List<int>();
            List<int> tempCostDetail = startFromPathInfo.costDetail.ToList();
            tempCostDetail.RemoveAt(tempCostDetail.Count - 1);//被stayAtFromCost取代
            costDetail.AddRange(tempCostDetail);
            costDetail.Add(stay_cost);
            tempCostDetail = fromToPathInfo.costDetail.ToList();
            tempCostDetail.RemoveAt(tempCostDetail.Count - 1);//被stayAtFromCost取代
            costDetail.AddRange(tempCostDetail);
            costDetail.Add(stay_cost);

            int count = 0;
            costAccumulation = new List<int>();
            costAccumulation.Add(0);
            foreach (int c in costDetail)
            {
                count += c;
                costAccumulation.Add(count);
            }
            path = new List<int>();
            path.AddRange(startFromPathInfo.path);
            List<int> temppath = fromToPathInfo.path.ToList();
            temppath.RemoveAt(0);//後路徑去掉開頭(前路徑結尾等於後路徑開頭)
            path.AddRange(temppath);

        }
        public bool Equals(StartFromToPathInfo other)
        {
            if (this.total_cost != other.total_cost)
            {
                return false;
            }
            if (!this.path.SequenceEqual(other.path))
            {
                return false;
            }
            return true;
        }
    }

    //class InterCost : IEquatable<InterCost>//因為載具可能會需要轉向，所花費的時間稱之為intercost
    class InterCost //因為載具可能會需要轉向，所花費的時間稱之為intercost
    {
        public int cost;
        public int firstPoint;
        public int secondPoint;
        public int thirdPoint;
        //public List<int> pattern;

        public InterCost()
        {
            //pattern = new List<int>();
        }

        //public InterCost(int cost, List<int> pattern)
        //{
        //    this.cost = cost;
        //    this.pattern = pattern;
        //}

        public InterCost(int cost, int firstPoint, int secondPoint, int thirdPoint)
        {
            this.cost = cost;
            //pattern = new List<int>();
            //pattern.Add(a);
            //pattern.Add(b);
            //pattern.Add(c);
            this.firstPoint = firstPoint;
            this.secondPoint = secondPoint;
            this.thirdPoint = thirdPoint;
        }


        //public bool isMatch(List<int> _pattern)
        //{
        //    if (_pattern.Count < 3)//少於3個點必定有誤
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        if (!this.pattern.SequenceEqual(_pattern))
        //        {
        //            return false;
        //        }
        //        return true;
        //    }
        //}

        //public bool Equals(InterCost other)
        //{
        //    if (this.cost != other.cost)
        //    {
        //        return false;
        //    }
        //    if (!this.pattern.SequenceEqual(other.pattern))
        //    {
        //        return false;
        //    }
        //    return true;
        //}
    }

    public class Section //section都是雙向的
    {
        public int isection_id;
        public string section_id;
        public int address_1;
        public int address_2;
        public int moveCost_1;
        public int moveCost_2;
        public bool isBanEnd_From2To;
        public bool isBanEnd_To2From;
        public int direct;//1是順向,2是逆向

        public changeSection changeSec_1;
        public changeSection changeSec_2;
        public changeSection changeSec_3;

        public changeSection changeSec_4;
        public changeSection changeSec_5;
        public changeSection changeSec_6;

        public Section(string section_id, int address_1, int address_2, int moveCost_1, int moveCost_2,
            string changeSec_1, int interCost_1, string changeSec_2, int interCost_2,
            string changeSec_3, int interCost_3, string changeSec_4, int interCost_4,
            string changeSec_5, int interCost_5, string changeSec_6, int interCost_6,
            bool _isBanEnd_From2To, bool _isBanEnd_To2From, int direct)
        {
            this.section_id = section_id;
            int.TryParse(this.section_id, out isection_id);
            this.address_1 = address_1;
            this.address_2 = address_2;
            this.moveCost_1 = moveCost_1;
            this.moveCost_2 = moveCost_2;
            if (!string.IsNullOrEmpty(changeSec_1))
            {
                this.changeSec_1 = new changeSection(changeSec_1, interCost_1);
            }
            if (!string.IsNullOrEmpty(changeSec_2))
            {
                this.changeSec_2 = new changeSection(changeSec_2, interCost_2);
            }
            if (!string.IsNullOrEmpty(changeSec_3))
            {
                this.changeSec_3 = new changeSection(changeSec_3, interCost_3);
            }
            if (!string.IsNullOrEmpty(changeSec_4))
            {
                this.changeSec_4 = new changeSection(changeSec_4, interCost_4);
            }
            if (!string.IsNullOrEmpty(changeSec_5))
            {
                this.changeSec_5 = new changeSection(changeSec_5, interCost_5);
            }
            if (!string.IsNullOrEmpty(changeSec_6))
            {
                this.changeSec_6 = new changeSection(changeSec_6, interCost_6);
            }

            isBanEnd_From2To = _isBanEnd_From2To;
            isBanEnd_To2From = _isBanEnd_To2From;
            this.direct = direct;
            //this.changeSec_1 = changeSec_1;
            //this.interCost_1 = interCost_1;
            //this.changeSec_2 = changeSec_2;
            //this.interCost_2 = interCost_2;
            //this.changeSec_3 = changeSec_3;
            //this.interCost_3 = interCost_3;
        }
        public class changeSection
        {
            public string changeSec;
            public int interCost;
            public changeSection(string changeSec, int interCost)
            {
                this.changeSec = changeSec;
                this.interCost = interCost;
            }
        }
    }

    public class RouteInfo
    {
        public RouteInfo(List<Section> _sections, List<int> _addresses, int _total_cost)
        {
            sections = _sections;
            addresses = _addresses;
            total_cost = _total_cost;
        }
        public RouteInfo(List<Section> _sections, int _total_cost)
        {
            sections = _sections;
            total_cost = _total_cost;
        }
        public List<Section> sections;
        public List<int> addresses;

        public int total_cost;
        public List<string> GetSectionIDs()
        {
            return sections.Select(sec => sec?.section_id).ToList();
        }
        public static (List<string> guideSections, List<string> guideAddresses) GetSectionInfos(BLL.SegmentBLL segmentBLL, List<string> guideSegmentIds, List<string> guideAddressIds)
        {
            List<string> section_ids = new List<string>();
            List<string> address_ids = new List<string>();

            for (int i = 0; i < guideSegmentIds.Count; i++)
            {
                string seg_id = guideSegmentIds[i];
                //List<ASECTION> on_segment_sections = sectionBLL.cache.GetSectionBySegmentID(seg_id);
                //List<ASECTION> on_segment_sections_by_order = getSectionByOrder(on_segment_sections, sec_obj.FromAddress);
                List<ASECTION> on_segment_sections = segmentBLL.cache.GetSegment(seg_id).Sections;
                List<ASECTION> sections_by_order = new List<ASECTION>();
                List<string> addresses_by_order = new List<string>();
                string start_adr_on_segment = guideAddressIds[i];
                string end_adr_on_segment = guideAddressIds[(i) + 1];

                var firstSecAndDir = findFirstSectionIndexAndDir(on_segment_sections, start_adr_on_segment, end_adr_on_segment);
                if (firstSecAndDir.dir == E_RAIL_DIR.F)
                {
                    for (int j = firstSecAndDir.firstSectionIndex; j < on_segment_sections.Count; j++)
                    {
                        ASECTION secTemp = on_segment_sections[j];
                        addresses_by_order.Add(secTemp.FROM_ADR_ID);
                        sections_by_order.Add(secTemp);
                        if (SCUtility.isMatche(on_segment_sections[j].TO_ADR_ID, end_adr_on_segment))
                        {
                            break;
                        }
                    }
                }
                else
                {
                    for (int j = firstSecAndDir.firstSectionIndex; j >= 0; j--)
                    {
                        ASECTION secTemp = on_segment_sections[j];
                        addresses_by_order.Add(secTemp.TO_ADR_ID);
                        sections_by_order.Add(secTemp);
                        if (SCUtility.isMatche(on_segment_sections[j].FROM_ADR_ID, end_adr_on_segment))
                        {
                            break;
                        }
                    }
                }
                section_ids.AddRange(sections_by_order.Select(sec1 => sec1.SEC_ID.Trim()));
                address_ids.AddRange(addresses_by_order);
            }
            address_ids.Add(guideAddressIds.Last());
            return (section_ids, address_ids);
        }

        private List<ASECTION> getSectionByOrder(List<ASECTION> sections, string from_adr)
        {
            if (sections.Count == 1) return sections;
            List<ASECTION> order_sections = new List<ASECTION>();
            string next_from_adr = from_adr;
            for (int i = 0; i < sections.Count; i++)
            {
                ASECTION section = sections.Where(s => s.FROM_ADR_ID.Trim() == next_from_adr.Trim()).SingleOrDefault();
                order_sections.Add(section);
                next_from_adr = section.TO_ADR_ID;
            }
            return order_sections;
        }
        static private (int firstSectionIndex, E_RAIL_DIR dir) findFirstSectionIndexAndDir(List<ASECTION> on_segment_sections_by_order, string firstAdr, string secondAdr)
        {
            E_RAIL_DIR dir = E_RAIL_DIR.F;
            int first_adr_of_section_index = 0;
            for (int i = 0; i < on_segment_sections_by_order.Count; i++)
            {
                if (SCUtility.isMatche(on_segment_sections_by_order[i].FROM_ADR_ID, firstAdr))
                {
                    first_adr_of_section_index = i;
                    for (int j = i; j < on_segment_sections_by_order.Count; j++)
                    {
                        if (SCUtility.isMatche(on_segment_sections_by_order[j].TO_ADR_ID, secondAdr))
                        {
                            dir = E_RAIL_DIR.F;
                            return (first_adr_of_section_index, dir);
                        }
                    }
                }
            }
            for (int i = on_segment_sections_by_order.Count - 1; i >= 0; i--)
            {
                if (SCUtility.isMatche(on_segment_sections_by_order[i].TO_ADR_ID, firstAdr))
                {
                    first_adr_of_section_index = i;
                    for (int j = i; j >= 0; j--)
                    {
                        if (SCUtility.isMatche(on_segment_sections_by_order[j].FROM_ADR_ID, secondAdr))
                        {
                            dir = E_RAIL_DIR.R;
                            return (first_adr_of_section_index, dir);
                        }
                    }
                }
            }
            throw new Exception("fun:findFirstSectionIndexAndDir ,not find.");
        }

        public List<string> GetAddressesIDs()
        {
            return addresses.Select(adr => adr.ToString("0000")).ToList();
        }


    }


    class GFG
    {
        // A utility function to find the 
        // vertex with minimum distance 
        // value, from the set of vertices 
        // not yet included in shortest 
        // path tree 
        static int V = 9;






    }
}
