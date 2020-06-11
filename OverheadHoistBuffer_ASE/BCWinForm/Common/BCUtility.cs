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
using com.mirle.ibg3k0.bc.winform.App;
using com.mirle.ibg3k0.bc.winform.UI.UAS;
using com.mirle.ibg3k0.sc;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.sc.Data.VO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.mirle.ibg3k0.bc.winform.Common
{
    /// <summary>
    /// Class BCUtility.
    /// </summary>
    public class BCUtility
    {
        /// <summary>
        /// Does the logout.
        /// </summary>
        /// <param name="bcApp">The bc application.</param>
        /// <returns>Boolean.</returns>
        public static Boolean doLogout(BCApplication bcApp)
        {
            bcApp.logoff();
            return true;
        }

        /// <summary>
        /// Determines whether the specified bc application is login.
        /// </summary>
        /// <param name="bcApp">The bc application.</param>
        /// <returns>Boolean.</returns>
        public static Boolean isLogin(BCApplication bcApp)
        {
            string loginUserID = bcApp.LoginUserID;
            if (SCUtility.isEmpty(loginUserID))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Logins as admin.
        /// </summary>
        /// <param name="bcApp">The bc application.</param>
        /// <returns>Boolean.</returns>
        public static Boolean loginAsAdmin(BCApplication bcApp)
        {
            UASUSR admin = bcApp.SCApplication.UserBLL.getAdminUser();
            if (admin == null)
            {
                return false;
            }
            bcApp.login(admin);
            return true;
        }

        /// <summary>
        /// 單純進行Login動作
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="bcApp">The bc application.</param>
        /// <returns>Boolean.</returns>
        public static Boolean doLogin(System.Windows.Forms.IWin32Window window, BCApplication bcApp)
        {
            string loginUserID = bcApp.LoginUserID;
            Boolean hasAuth = false;
            if (!SCUtility.isEmpty(loginUserID))
            {
                MessageBox.Show(window, BCApplication.getMessageString("Already_Login"),
                    BCApplication.getMessageString("INFO"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            LoginPopupForm loginForm = new LoginPopupForm(BCAppConstants.FUNC_LOGIN);
            System.Windows.Forms.DialogResult result = loginForm.ShowDialog(window);
            loginUserID = loginForm.getLoginUserID();
            string loginPassword = loginForm.getLoginPassword();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                loginForm.Dispose();
            }
            else
            {
                loginForm.Dispose();
                return false;
            }
            Boolean loginSuccess = false;
            if (!SCUtility.isEmpty(loginUserID))
            {
                loginSuccess = bcApp.SCApplication.UserBLL.checkUserPassword(loginUserID, loginPassword);
            }
            if (loginSuccess)
            {
                bcApp.login(loginUserID);
                hasAuth = bcApp.SCApplication.UserBLL.checkUserAuthority(loginUserID, BCAppConstants.FUNC_LOGIN);
            }
            if (!hasAuth)
            {
                MessageBox.Show(window, BCApplication.getMessageString("NO_AUTHORITY"),
                    BCApplication.getMessageString("WARNING"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return hasAuth;
        }
        public static void showMsgBox_Info(IWin32Window _from, string txt)
        {
            MessageBox.Show(_from, txt
                     , BCApplication.getMessageString("INFO")
                    , MessageBoxButtons.OK
                    , MessageBoxIcon.Information);

        }
        /// <summary>
        /// Does the login.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="bcApp">The bc application.</param>
        /// <param name="function_code">The function_code.</param>
        /// <returns>Boolean.</returns>
        public static Boolean doLogin(System.Windows.Forms.IWin32Window window, BCApplication bcApp, string function_code)
        {
            return doLogin(window, bcApp, function_code, false);
        }

        /// <summary>
        /// Does the login.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="bcApp">The bc application.</param>
        /// <param name="function_code">The function_code.</param>
        /// <param name="isForceChack">if set to <c>true</c> [is force chack].</param>
        /// <returns>Boolean.</returns>
        public static Boolean doLogin(System.Windows.Forms.IWin32Window window, BCApplication bcApp, string function_code, bool isForceChack)//A0.01
        {
            //return true;
            string loginUserID = bcApp.LoginUserID;
            Boolean hasAuth = false;
            if (!isForceChack && !SCUtility.isEmpty(loginUserID))
            {
                hasAuth = bcApp.SCApplication.UserBLL.checkUserAuthority(loginUserID, function_code);
            }

            if (hasAuth)
            {
                return true;
            }

            //            LoginPopupForm loginForm = new LoginPopupForm(function_code);
            //如果已經有人登入了，就必須已切換帳號的方式再次登入
            LoginPopupForm loginForm = new LoginPopupForm(function_code, isForceChack ? false : BCUtility.isLogin(bcApp));
            System.Windows.Forms.DialogResult result = loginForm.ShowDialog(window);
            loginUserID = loginForm.getLoginUserID();
            string loginPassword = loginForm.getLoginPassword();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                loginForm.Dispose();
            }
            else
            {
                loginForm.Dispose();
                return false;
            }
            Boolean loginSuccess = false;
            if (!SCUtility.isEmpty(loginUserID))
            {
                loginSuccess = bcApp.SCApplication.UserBLL.checkUserPassword(loginUserID, loginPassword);
            }
            if (loginSuccess)
            {
                bcApp.login(loginUserID);
                hasAuth = bcApp.SCApplication.UserBLL.checkUserAuthority(loginUserID, function_code);
            }

            if (!hasAuth)
            {
                MessageBox.Show(window, BCApplication.getMessageString("NO_AUTHORITY"),
                    BCApplication.getMessageString("WARNING"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return hasAuth;
        }

        public class AuthorityCheck : Attribute
        {
            public string FUNCode { get; set; }

        }



        #region Pixels & RealLength 的轉換
        private static int scale = 0;
        public static void setScale(int _scale, int zoon_Factor)
        {
            //scale = _scale * 100;
            scale = _scale * zoon_Factor;
        }
        public static double RealLengthToPixelsWidthByScale(double length)
        {
            double length_cm = lengthTransferByScale(length, scale);//1cm:10m
            double length_mm = length_cm * Math.Pow(10, -2) * Math.Pow(10, 3);
            return MillimetersToPixelsWidth(length_mm);
        }
        public static double PixelsWidthToRealLengthByScale(double pixel)
        {
            double length_mm = PixelsWidthToMillimeters(pixel);//1cm:10m
            double length_cm = length_mm * Math.Pow(10, 2) * Math.Pow(10, -3);
            return lengthTransfer2RealLengthByScale(length_cm, scale);
        }

        public static double lengthTransferByScale(double length, double scale)
        {
            return length / scale;
        }

        public static double lengthTransfer2RealLengthByScale(double length, double scale)
        {
            return length * scale;
        }


        public static double MillimetersToPixelsWidth(double length) //length是mm，1厘米=10毫米
        {
            System.Windows.Forms.Panel p = new System.Windows.Forms.Panel();
            System.Drawing.Graphics g = System.Drawing.Graphics.FromHwnd(p.Handle);
            IntPtr hdc = g.GetHdc();
            //int width = GetDeviceCaps(hdc, 4);     // HORZRES  物理的寬度
            //int pixels = GetDeviceCaps(hdc, 8);     // BITSPIXEL
            int width = 508;                        // HORZRES  物理的寬度
            int pixels = 1920;                      // BITSPIXEL
            g.ReleaseHdc(hdc);
            return (((double)pixels / (double)width) * (double)length);
        }

        public static double PixelsWidthToMillimeters(double PixelsWidth) //length是毫米，1厘米=10毫米
        {
            System.Windows.Forms.Panel p = new System.Windows.Forms.Panel();
            System.Drawing.Graphics g = System.Drawing.Graphics.FromHwnd(p.Handle);
            IntPtr hdc = g.GetHdc();
            //int width = GetDeviceCaps(hdc, 4);     // HORZRES  物理的寬度
            //int pixels = GetDeviceCaps(hdc, 8);     // BITSPIXEL  解析度
            int width = 508;                        // HORZRES  物理的寬度
            int pixels = 1920;                      // BITSPIXEL   解析度
            g.ReleaseHdc(hdc);

            return (((double)width / (double)pixels) * (double)PixelsWidth);
        }
        #endregion Pixels & RealLength 的轉換

        public static Color ConvStr2Color(string sText)
        {
            Color clrData;
            sText = (sText != null) ? sText.Trim() : sText;
            clrData = Color.FromName(sText);
            if (!clrData.IsKnownColor)
            {
                clrData = Color.FromArgb(int.Parse(sText, NumberStyles.AllowHexSpecifier));
            }

            return (clrData);
        }

        public static void setComboboxDataSource(ComboBox crl_comboBox, string[] data_Source)
        {
            crl_comboBox.DataSource = data_Source;
            if (crl_comboBox.AutoCompleteCustomSource.Count != 0)
            {
                crl_comboBox.AutoCompleteCustomSource.Clear();
            }
            crl_comboBox.AutoCompleteCustomSource.AddRange(data_Source);
            crl_comboBox.AutoCompleteMode = AutoCompleteMode.Suggest;
            crl_comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
        public class DisplayNameLocalizedAttribute : DisplayNameAttribute
        {
            public DisplayNameLocalizedAttribute(Type resourceManagerProvider, string resourceKey)
               : base(LookupResource(resourceManagerProvider, resourceKey))
            {
            }
        }
        internal static string LookupResource(Type resourceManagerProvider, string resourceKey)
        {
            foreach (PropertyInfo staticProperty in resourceManagerProvider.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
            {
                if (staticProperty.PropertyType == typeof(System.Resources.ResourceManager))
                {
                    System.Resources.ResourceManager resourceManager = (System.Resources.ResourceManager)staticProperty.GetValue(null, null);
                    return resourceManager.GetString(resourceKey);
                }
            }

            return resourceKey; // Fallback with the key name
        }
    }
}
