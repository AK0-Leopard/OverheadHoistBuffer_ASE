using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// 組件的一般資訊是由下列的屬性集控制。
// 變更這些屬性的值即可修改組件的相關
// 資訊。
[assembly: AssemblyTitle("ScriptControl")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("ScriptControl")]
[assembly: AssemblyCopyright("Copyright ©  2020")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// 將 ComVisible 設定為 false 會使得這個組件中的型別
// 對 COM 元件而言為不可見。如果您需要從 COM 存取這個組件中
// 的型別，請在該型別上將 ComVisible 屬性設定為 true。
[assembly: ComVisible(false)]

// 下列 GUID 為專案公開 (Expose) 至 COM 時所要使用的 typelib ID
[assembly: Guid("3d2cfe14-a57f-447f-8474-024dcf26db43")]

// 組件的版本資訊是由下列四項值構成:
//
//      主要版本
//      次要版本 
//      組建編號
//      修訂編號
//
// 您可以指定所有的值，也可以依照以下的方式，使用 '*' 將組建和修訂編號
// 指定為預設值:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.5.1")]
[assembly: AssemblyFileVersion("1.5.1")]

//版號 + Tab + 日期 + Tab + 修改人 + Tab + 敘述
//1.5.1 2020/09/30  SCC+    增加命令狀態為 Canceling、Aborting 進行強制結束功能
//1.5.0 2020/09/28  JasonWu+ 修改PortValueDefMapAction 中對於LoadPostion1 的觸發，要同時判斷是否為Auto mode。
//1.4.9 2020/09/23  JasonWu+ 1.修改判定 port 是否可用之判定邏輯，拿掉 port ready 及 mismatch 訊號。 
//                           2.修改判定是否可轉向，進一步確認是否為實盒及out mode 避免出貨命令被取走。
//                           3.修改原判定退補空盒命令產生時間，由 10 -> 3.5 (因為AGVC 每3秒觸發一次)
//1.4.7 2020/09/13  SCC+     API CheckIsSourceFromAGVStation 內，增加Log
//1.4.6 2020/09/10  SCC*     修正OHT AlarmType
//1.4.5 2020/09/10  SCC*     GetAGV_OutModeInServicePortName 取得可用AGVPort，跳出迴圈
