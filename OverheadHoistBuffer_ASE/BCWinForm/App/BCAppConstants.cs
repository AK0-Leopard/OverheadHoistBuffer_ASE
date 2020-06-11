//*********************************************************************************
//      BCAppConstants.cs
//*********************************************************************************
// File Name: BCAppConstants.cs
// Description: Type 1 Function
//
//(c) Copyright 2015, MIRLE Automation Corporation
//
// Date          Author         Request No.    Tag     Description
// ------------- -------------  -------------  ------  -----------------------------
//**********************************************************************************
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.bc.winform.App
{
    /// <summary>
    /// Class BCAppConstants.
    /// </summary>
    public class BCAppConstants
    {
        /****************************** UAS *******************************/
        public const string FUNC_LOGIN = "FUNC_LOGIN";                        //Login
        public const string FUNC_USER_MANAGEMENT = "FUNC_USER_MANAGEMENT";    //User Management
        //A0.01 public const string FUNC_PPID_EDIT = "FUNC_PPID_EDIT";                //PPID/Recipe Edit
        //A0.01 public const string FUNC_ECID_EDIT = "FUNC_ECID_EDIT";                //ECID Edit
        //A0.01 public const string FUNC_ROBOT_CMD_RESET = "FUNC_ROBOT_CMD_RESET";    //Robot Command Reset Button
        //A0.01 public const string FUNC_DEBUG_FORM = "FUNC_DEBUG_FORM";              //Debug Form
        //A0.01 public const string FUNC_GLASS_SORT_FORM = "FUNC_GLASS_SORT_FORM";    //Glass Sort Form
        public const string FUNC_EXCUTE_SEMI_AUTO_PROC = "FUNC_EXCUTE_SEMI_AUTO_PROC";    //Excute Semi-Auto Process Button
        public const string FUNC_CONNECTION_MANAGEMENT = "FUNC_CONNECTION_MANAGEMENT";    //Start / Stop Connection
        public const string FUNC_CLOSE_MASTER_PC = "FUNC_CLOSE_MASTER_PC";    //Close Master PC
        //A0.01 public const string FUNC_EAP_CONTROL_MODE = "FUNC_EAP_CONTROL_MODE";  //EAP CONTROL MODE 
        public const string FUNC_HOST_MODE_CHG = "FUNC_HOST_MODE_CHG";    //Host Mode Change  //A0.01
        public const string FUNC_COMMAND_MAIN = "FUNC_COMMAND_MAIN";    //Command Maintenance  //A0.01
        public const string FUNC_CST_REMOVE = "FUNC_CST_REMOVE";    //Cassette Remove  //A0.01
        public const string FUNC_ALARM_MAIN = "FUNC_ALARM_MAIN";    //Alarm Maintenance  //A0.01
        public const string FUNC_CST_SIZE_MAIN = "FUNC_CST_SIZE_MAIN";    //Cassette Size Maintenance  //A0.01
        public const string FUNC_GLASS_DATA_MAIN = "FUNC_GLASS_DATA_MAIN";    //Glass Data Maintenance  //A0.01
        public const string FUNC_EQPTPARAMETER = "FUNC_EQPTPARAMETER";    //EqptParameterForm  //A0.02
        public const string FUNC_AUTOLOGOUTTIME = "FUNC_AUTOLOGOUTTIME";    //AutoLogoutTimeForm  //A0.03
        public const string FUNC_BUTTONCONTROL = "FUNC_BUTTONCONTROL";    //Button Control  //A0.04
        public const string FUNC_BUTTONCONTROL_CIMON = "FUNC_BUTTONCONTROL_CIMON";                        //Button Control (CIM On)

        /******************************************************************/
        public const string LOGIN_USER_DEFAULT = "";

        public static Color CLR_MAP_VHSTS_NONE_FORE = Color.Black;
        public static Color CLR_MAP_VHSTS_NONE_BACK = Color.White;
        public static Color CLR_MAP_VHSTS_MANUAL_FORE = Color.White;
        public static Color CLR_MAP_VHSTS_MANUAL_BACK = Color.Blue;
        public static Color CLR_MAP_VHSTS_AUTOREMOTE_FORE = Color.Black;
        public static Color CLR_MAP_VHSTS_AUTOREMOTE_BACK = Color.Lime;
        public static Color CLR_MAP_VHSTS_AUTOLOCAL_FORE = Color.Black;
        public static Color CLR_MAP_VHSTS_AUTOLOCAL_BACK = Color.Pink;
        public static Color CLR_MAP_VHSTS_TRANSFER_FORE = Color.Black;
        public static Color CLR_MAP_VHSTS_TRANSFER_BACK = Color.Cyan;
        public static Color CLR_MAP_VHSTS_MOVE2CLEARPARK_FORE = Color.Black;
        public static Color CLR_MAP_VHSTS_MOVE2CLEARPARK_BACK = Color.Cyan;
        public static Color CLR_MAP_VHSTS_MOVE2DIRTY_FORE = Color.Black;
        public static Color CLR_MAP_VHSTS_MOVE2DIRTY_BACK = Color.Cyan;
        public static Color CLR_MAP_VHSTS_MOVE2MTLPORT_FORE = Color.Black;
        public static Color CLR_MAP_VHSTS_MOVEONAUTOLOCAL_BACK = Color.Pink;
        public static Color CLR_MAP_VHSTS_BLOCK_FORE = Color.Black;
        public static Color CLR_MAP_VHSTS_BLOCK_BACK = Color.Yellow;
        public static Color CLR_MAP_VHSTS_PAUSE_FORE = Color.Black;
        public static Color CLR_MAP_VHSTS_PAUSE_BACK = Color.Violet;
        public static Color CLR_MAP_VHSTS_OBSTACLE_FORE = Color.Black;
        public static Color CLR_MAP_VHSTS_OBSTACLE_BACK = Color.Orange;
        public static Color CLR_MAP_VHSTS_HID_PAUSE_FORE = Color.Black;
        public static Color CLR_MAP_VHSTS_HID_PAUSE_BACK = Color.Purple;
        public static Color CLR_MAP_VHSTS_ALARM_FORE = Color.White;
        public static Color CLR_MAP_VHSTS_ALARM_BACK = Color.Red;
        public static Color CLR_MAP_VHSTS_DISCONNECT_FORE = Color.Black;
        public static Color CLR_MAP_VHSTS_DISCONNECT_BACK = Color.Silver;

        public static Color CLR_MAP_VHCST_OFF = Color.White;
        public static Color CLR_MAP_VHCST_ON = Color.Lime;
        public static Color CLR_MAP_PORTSTS_NOLINK = Color.Silver;
        public static Color CLR_MAP_PORTSTS_OFFLINE = Color.White;
        public static Color CLR_MAP_PORTSTS_ONLINE = Color.Lime;

        public static Color CLR_MAP_ADDRESS_DUFAULT = Color.Gainsboro;
        public static Color CLR_MAP_ADDRESS_START = Color.Violet;
        public static Color CLR_MAP_ADDRESS_FROM = Color.Lime;
        public static Color CLR_MAP_ADDRESS_TO = Color.Red;



        #region Section Through Color 
        //public static Color SEC_THROUGH_COLOR_LV1 = Common.BCUtility.ConvStr2Color("99FFFF");
        //public static Color SEC_THROUGH_COLOR_LV2 = Common.BCUtility.ConvStr2Color("0098FF");
        //public static Color SEC_THROUGH_COLOR_LV3 = Common.BCUtility.ConvStr2Color("0166FF");
        //public static Color SEC_THROUGH_COLOR_LV4 = Common.BCUtility.ConvStr2Color("329900");
        //public static Color SEC_THROUGH_COLOR_LV5 = Common.BCUtility.ConvStr2Color("33FF00");
        //public static Color SEC_THROUGH_COLOR_LV6 = Common.BCUtility.ConvStr2Color("FFFF00");
        //public static Color SEC_THROUGH_COLOR_LV7 = Common.BCUtility.ConvStr2Color("FFCC00");
        //public static Color SEC_THROUGH_COLOR_LV8 = Common.BCUtility.ConvStr2Color("FC0000");

        public static Color SEC_THROUGH_COLOR_LV1 = Color.FromArgb(51, 255, 0);
        public static Color SEC_THROUGH_COLOR_LV2 = Color.FromArgb(255, 255, 0);
        public static Color SEC_THROUGH_COLOR_LV3 = Color.FromArgb(255, 204, 0);
        public static Color SEC_THROUGH_COLOR_LV4 = Color.FromArgb(254, 153, 0);
        public static Color SEC_THROUGH_COLOR_LV5 = Color.FromArgb(255, 0, 0);
        public static Color SEC_THROUGH_COLOR_LV6 = Color.FromArgb(204, 0, 1);
        public static Color SEC_THROUGH_COLOR_LV7 = Color.FromArgb(153, 0, 0);
        public static Color SEC_THROUGH_COLOR_LV8 = Color.FromArgb(153, 0, 152);
        public static Color SEC_THROUGH_COLOR_LV9 = Color.FromArgb(203, 0, 204);
        public static Color SEC_THROUGH_COLOR_LV10 = Color.FromArgb(255, 0, 254);
        #endregion Section Through Color 

        #region Section Through Times 
        public const int SEC_THROUGH_TIMES_LV1 = 13;
        public const int SEC_THROUGH_TIMES_LV2 = 26;
        public const int SEC_THROUGH_TIMES_LV3 = 39;
        public const int SEC_THROUGH_TIMES_LV4 = 52;
        public const int SEC_THROUGH_TIMES_LV5 = 65;
        public const int SEC_THROUGH_TIMES_LV6 = 78;
        public const int SEC_THROUGH_TIMES_LV7 = 91;
        public const int SEC_THROUGH_TIMES_LV8 = 104;
        public const int SEC_THROUGH_TIMES_LV9 = 117;
        #endregion Section Through Times 

        public static int NONE = -1;




    }
}
