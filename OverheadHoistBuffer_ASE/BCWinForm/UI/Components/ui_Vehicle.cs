using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.bc.winform.App;
using com.mirle.ibg3k0.sc.Common;
using com.mirle.ibg3k0.bcf.Common;
using System.Threading;
using com.mirle.ibg3k0.sc;

namespace com.mirle.ibg3k0.bc.winform.UI.Components
{
    public partial class ui_Vehicle : UserControl
    {
        const string CHANGE_PROPERTY_NAME_TEXT = "Text";

        string EventHandler = string.Empty;
        private AEQPT eqpt = null;
        private BCApplication bcApp = null;
        public ui_Vehicle()
        {
            InitializeComponent();
        }
        public void start(string eq_id)
        {
            bcApp = BCApplication.getInstance();
            //eqpt = bcApp.SCApplication.getEQObjCacheManager().getEquipmentByEQPTID(eq_id);
            lbl_ID_Value.Text = eq_id;
            initialDataBinding();
        }
        private void initialDataBinding()
        {
            EventHandler = this.Name;
            //eqpt.addEventHandler(EventHandler, BCFUtility.getPropertyName(() => eqpt.VH_State), (s1, e1) => updateVHState());
            //eqpt.addEventHandler(EventHandler, BCFUtility.getPropertyName(() => eqpt.TcpIp_Msg_State), (s1, e1) => updateMessageStage());
        }

        private void updateVHState()
        {
            Adapter.BeginInvoke(new SendOrPostCallback((o1) =>
            {
                //lbl_VHStatus_Value.Text = eqpt.VH_State.ToString();
            }), null);
        }
        private void updateMessageStage()
        {
            Adapter.BeginInvoke(new SendOrPostCallback((o1) =>
            {
                //lbl_MsgStatus_Vaue.Text = eqpt.TcpIp_Msg_State;
            }), null);

        }
    }
}
