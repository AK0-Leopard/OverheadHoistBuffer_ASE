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
using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Controller;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc;

namespace com.mirle.ibg3k0.bc.winform.UI.Components
{
    public partial class uctlTcpIpAgentStatus : UserControl
    {
        Image Pic_Ball_Rad = null;
        Image Pic_Ball_Green = null;
        BCMainForm form = null;
        string event_id = string.Empty;
        Dictionary<string, object[]> dicTcpIpAgentInfo = null;
        public uctlTcpIpAgentStatus()
        {
            InitializeComponent();
            Pic_Ball_Rad = global::com.mirle.ibg3k0.bc.winform.Properties.Resources.Ball_Red;
            Pic_Ball_Green = global::com.mirle.ibg3k0.bc.winform.Properties.Resources.Ball_Green;
            dicTcpIpAgentInfo = new Dictionary<string, object[]>();
            event_id = this.Name;
        }
        public void start(BCMainForm _form)
        {
            form = _form;
            //List<Equipment> lstEq = form.BCApp.SCApplication.getEQObjCacheManager().getAllEquipment();
            List<AVEHICLE> lstEq = form.BCApp.SCApplication.getEQObjCacheManager().getAllVehicle();
            tlp_InfoBase.RowStyles.Clear();
            tlp_InfoBase.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));


            foreach (AVEHICLE eq in lstEq)
            {
                //if (eq.CommunicationType != BCFAppConstants.COMMUNICATION_TYPE_TCPIP)
                //{
                //    continue;
                //}
                creatTlpRow(eq);
            }

        }
        private void creatTlpRow(AVEHICLE eq)
        {
            Label name = getLabel(eq.VEHICLE_ID);
            PictureBox pic_comm = getPictureBox();
            PictureBox pic_conn = getPictureBox();
            Label conn_time = getLabel("");
            Label disConn_time = getLabel("");
            Label disConnTimes = getLabel("");
            Label lostPackets = getLabel("");
            CheckBox is_listening = getCheckBox(eq);

            tlp_InfoBase.Controls.Add(name, 0, tlp_InfoBase.RowCount - 1);
            tlp_InfoBase.Controls.Add(pic_conn, 1, tlp_InfoBase.RowCount - 1);
            tlp_InfoBase.Controls.Add(pic_comm, 2, tlp_InfoBase.RowCount - 1);
            tlp_InfoBase.Controls.Add(conn_time, 3, tlp_InfoBase.RowCount - 1);
            tlp_InfoBase.Controls.Add(disConn_time, 4, tlp_InfoBase.RowCount - 1);
            tlp_InfoBase.Controls.Add(disConnTimes, 5, tlp_InfoBase.RowCount - 1);
            tlp_InfoBase.Controls.Add(lostPackets, 6, tlp_InfoBase.RowCount - 1);
            tlp_InfoBase.Controls.Add(is_listening, 7, tlp_InfoBase.RowCount - 1);

            tlp_InfoBase.RowCount++;
            tlp_InfoBase.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            dicTcpIpAgentInfo.Add(eq.VEHICLE_ID, new object[] { pic_conn, pic_comm, conn_time, disConn_time, disConnTimes, lostPackets, is_listening });

        }

        private static CheckBox getCheckBox(AVEHICLE eq)
        {
            CheckBox check_box = new CheckBox();
            check_box.CheckAlign = ContentAlignment.MiddleCenter;
            return check_box;
        }


        private static Label getLabel(string Name)
        {
            Label name = new Label() { Text = Name, Anchor = AnchorStyles.None };
            name.Dock = DockStyle.Fill;
            name.TextAlign = ContentAlignment.MiddleCenter;
            return name;
        }

        private PictureBox getPictureBox()
        {
            PictureBox pic_comm = new PictureBox();
            pic_comm.Image = Pic_Ball_Rad;
            pic_comm.SizeMode = PictureBoxSizeMode.Zoom;
            return pic_comm;
        }

        public void refresh()
        {
            foreach (KeyValuePair<string, object[]> keyValue in dicTcpIpAgentInfo)
            {
                PictureBox pic_conn = keyValue.Value[0] as PictureBox;
                PictureBox pic_comm = keyValue.Value[1] as PictureBox;
                Label lbl_conn_time = keyValue.Value[2] as Label;
                Label lbl_disConn_time = keyValue.Value[3] as Label;
                Label lbl_disConnTimes = keyValue.Value[4] as Label;
                Label lbl_lostPackets = keyValue.Value[5] as Label;
                CheckBox ck_check_box = keyValue.Value[6] as CheckBox;
                bool IsListening = false;
                bool IsCommunication = false;
                bool IsConnections = false;
                DateTime connTime;
                TimeSpan accConnTime;
                DateTime disConnTime;
                TimeSpan accDisConnTime;
                int disconnTimes = 0;
                int lostPackets = 0;
                //Equipment eq = form.BCApp.SCApplication.getEQObjCacheManager().getEquipmentByEQPTID(keyValue.Key);
                AVEHICLE eq = form.BCApp.SCApplication.getEQObjCacheManager().getVehicletByVHID(keyValue.Key);
                eq.getAgentInfo(form.BCApp.SCApplication.getBCFApplication(),
                    out IsListening, out IsCommunication, out IsConnections,
                    out connTime, out accConnTime,
                    out disConnTime, out accDisConnTime,
                    out disconnTimes,
                    out lostPackets);
                pic_comm.Image = IsCommunication ? Pic_Ball_Green : Pic_Ball_Rad;
                pic_conn.Image = IsConnections ? Pic_Ball_Green : Pic_Ball_Rad;
                string connInfo = string.Format("{0}\n({1})",
                                                accConnTime.ToString("d'd 'h'h 'm'm 's's'"),
                                                connTime.ToString(SCAppConstants.DateTimeFormat_11));
                lbl_conn_time.Text = connInfo;
                string disConnInfo = string.Format("{0}\n({1})",
                                                accDisConnTime.ToString("d'd 'h'h 'm'm 's's'"),
                                                disConnTime.ToString(SCAppConstants.DateTimeFormat_11));
                lbl_disConn_time.Text = disConnInfo;
                lbl_disConnTimes.Text = disconnTimes.ToString();
                lbl_lostPackets.Text = lostPackets.ToString();

                ck_check_box.Checked = IsListening;
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
