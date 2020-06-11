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
using com.mirle.ibg3k0.bcf.Common;

namespace com.mirle.ibg3k0.bc.winform.UI.Components
{
    public partial class uctlCommStatus : UserControl
    {

        Image Pic_Ball_Rad = null;
        Image Pic_Ball_Green = null;
        BCMainForm form = null;
        string event_id = string.Empty;
        Dictionary<string, CommuncationInfo> dicCommInfo = null;
        Dictionary<string, PictureBox[]> dicCommInfoPicture = null;
        public uctlCommStatus()
        {
            InitializeComponent();
            Pic_Ball_Rad = global::com.mirle.ibg3k0.bc.winform.Properties.Resources.Ball_Red;
            Pic_Ball_Green = global::com.mirle.ibg3k0.bc.winform.Properties.Resources.Ball_Green;
            dicCommInfoPicture = new Dictionary<string, PictureBox[]>();
            event_id = this.Name;
        }
        public void start(BCMainForm _form)
        {
            form = _form;
            dicCommInfo =
                form.BCApp.SCApplication.getEQObjCacheManager().CommonInfo.dicCommunactionInfo;
            tlp_LightBase.RowStyles.Clear();
            tlp_LightBase.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            foreach (KeyValuePair<string, CommuncationInfo> keyValue in dicCommInfo)
            {
                CommuncationInfo info = keyValue.Value;
                creatTlpRow(info);
            }
        }
        private void creatTlpRow(CommuncationInfo info)
        {
            Label name = new Label() { Text = info.Name, Anchor = AnchorStyles.None };
            name.Dock = DockStyle.Fill;
            name.TextAlign = ContentAlignment.MiddleCenter;
            PictureBox pic_comm = new PictureBox();
            pic_comm.Image = info.IsCommunactionSuccess ? Pic_Ball_Green : Pic_Ball_Rad;
            pic_comm.SizeMode = PictureBoxSizeMode.Zoom;
            PictureBox pic_conn = new PictureBox();
            pic_conn.Image = info.IsConnectinoSuccess ? Pic_Ball_Green : Pic_Ball_Rad;
            pic_conn.SizeMode = PictureBoxSizeMode.Zoom;
            tlp_LightBase.Controls.Add(name, 0, tlp_LightBase.RowCount - 1);
            tlp_LightBase.Controls.Add(pic_comm, 1, tlp_LightBase.RowCount - 1);
            tlp_LightBase.Controls.Add(pic_conn, 2, tlp_LightBase.RowCount - 1);
            tlp_LightBase.RowCount++;
            tlp_LightBase.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            dicCommInfoPicture.Add(info.Name, new PictureBox[] { pic_comm, pic_conn });
            //info.addEventHandler(event_id
            //        , BCFUtility.getPropertyName(() => info.IsCommunactionSuccess)
            //        , (s1, e1) =>
            //        {
            //            bool isSuccess = (bool)e1.PropertyValue;
            //            pic_comm.Image = isSuccess ? Pic_Ball_Green : Pic_Ball_Rad;
            //        });
            //info.addEventHandler(event_id
            //        , BCFUtility.getPropertyName(() => info.IsConnectinoSuccess)
            //        , (s1, e1) =>
            //        {
            //            bool isSuccess = (bool)e1.PropertyValue;
            //            pic_conn.Image = isSuccess ? Pic_Ball_Green : Pic_Ball_Rad;
            //        });


        }

        public void refresh()
        {
            foreach (KeyValuePair<string, PictureBox[]> keyValue in dicCommInfoPicture)
            {
                if (!dicCommInfo.ContainsKey(keyValue.Key))
                {
                    continue;
                }
                CommuncationInfo info = dicCommInfo[keyValue.Key];
                PictureBox[] CommAndConnInfo = keyValue.Value;
                CommAndConnInfo[0].Image = info.IsCommunactionSuccess ? Pic_Ball_Green : Pic_Ball_Rad;
                CommAndConnInfo[1].Image = info.IsConnectinoSuccess ? Pic_Ball_Green : Pic_Ball_Rad;
            }
        }


        int right_start_position_x = 25;
        int lest_start_position_x = 350;
        private Point getLableControlLocation(int clumn)
        {
            return new Point(right_start_position_x, 250 * (clumn - 1));
        }
    }
}
