//*********************************************************************************
//      IRouteGuide.cs
//*********************************************************************************
// File Name: IRouteGuide.cs
// Description: 
//
//(c) Copyright 2014, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
// 2019/06/11    Mark Chou      N/A            M0.01   把StartFromTo路徑計算與相關API移除，避免Memory使用過多問題
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc.RouteKit
{
    public interface IRouteGuide
    {
        List<RouteInfo> getFromToRoutesAddrToAddr(int from_addr, int to_addr);
        List<RouteInfo> getFromToRoutesAddrToAddr(int from_addr, int to_addr, List<string> banSegmentIDList);
        List<RouteInfo> getFromToRoutesSectionToSection(int from_section_id, int to_section_id);
        List<RouteInfo> getFromToRoutesSectionToAdr(int from_section_id, int to_port_addr);
        //List<List<Section>> getStartFromToRoutes(int start_addr, int from_addr, int to_addr);//M0.01
        //List<List<Section>> _TimeWindow(int fromAdr1, int toAdr1, int tofrom2, int toAdr2);
        (List<RouteInfo> stratFromRouteInfoList, List<RouteInfo> fromToRouteInfoList) getStartFromThenFromToRoutesSecToAddrToAddr(int start_sec, int from_addr, int to_addr);
        (List<RouteInfo> stratFromRouteInfoList, List<RouteInfo> fromToRouteInfoList) getStartFromThenFromToRoutesAddrToAddrToAddr(int start_addr, int from_addr, int to_addr);
        void banRouteOneDirect(int from, int to);
        void unbanRouteOneDirect(int from, int to);
        void banRouteTwoDirect(int address_1, int address_2);
        void banRouteTwoDirect(string sectionID);
        void unbanRouteTwoDirect(int address_1, int address_2);
        void unbanRouteTwoDirect(string sectionID);
        int[] getAllBanDirectArray();
        void resetBanRoute();
    }
}
