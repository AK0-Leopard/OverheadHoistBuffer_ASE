using com.mirle.ibg3k0.bc.winform.Properties;
using System.Windows.Forms;

namespace com.mirle.ibg3k0.bc.winform.i18n
{
    public class MsgHelper
    {
        public static string getMsg(string msgKey)
        {
            if (msgKey.Contains("[[") && msgKey.Contains("]]")) return msgKey;
            var str = Resources.ResourceManager.GetString(msgKey);
            if (str != null)
            {
                return str;
            }
            else
            {
                return "[[" + msgKey + "]]";
            }
        }


        public static void setTooltips(ToolTip tt, Control c, string defaultMsg, string msgKey, string[] args)
        {
            string msg = "";
            var str = Resources.ResourceManager.GetString(msgKey);
            if (str != null)
            {
                msg = str;
            }
            else
            {
                msg = "[" + defaultMsg + "]";
            }

            if (args != null && args.Length > 0)
            {
                tt.SetToolTip(c, string.Format(msg, args));

            }
            else
            {
                tt.SetToolTip(c, msg);

            }
        }



    }
}
