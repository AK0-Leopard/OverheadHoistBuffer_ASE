using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.mirle.ibg3k0.bc.winform.UI
{
    public partial class ShelfMaintenanceForm : Form
    {
        public BCMainForm MainForm { get; }
        public App.BCApplication BCApp;
        List<sc.ShelfDef> shelfDefs = null;
        List<sc.ShelfDef> showShelfDefs = null;
        List<sc.ZoneDef> zoneDefs = null;
        public ShelfMaintenanceForm(BCMainForm _mainForm)
        {
            InitializeComponent();
            BCApp = _mainForm.BCApp;
            dgv_shelfData.AutoGenerateColumns = false;
            initialShelfData();
            UpdateShelfData();
            MainForm = _mainForm;
            if (sc.Common.SCUtility.isMatche(BCApp.LoginUserID, App.BCAppConstants.ADMIN_USER_NAME))
            {
                table_HightLvlSet.Enabled = true;
            }
        }
        private void initialComboBox()
        {
            List<string> zone_ids = new List<string>();
            zone_ids.Add("");
            List<string> current_zone_ids = shelfDefs.Select(s => s.ZoneID).Distinct().OrderBy(s => s).ToList();
            zone_ids.AddRange(current_zone_ids);
            cmb_zoneID.DataSource = zone_ids;
        }

        private async void initialShelfData()
        {
            var shelfs = await Task.Run(() => BCApp.SCApplication.ShelfDefBLL.LoadShelf());
            var zones = await Task.Run(() => BCApp.SCApplication.ZoneDefBLL.loadZoneData());
            zoneDefs = zones;
            shelfDefs = shelfs.OrderBy(s => s.ZoneID).ThenBy(s => s.SeqNo).ToList();
            showShelfDefs = shelfDefs;
            dgv_shelfData.DataSource = showShelfDefs;

            initialComboBox();
        }

        private async void UpdateShelfData()
        {
            try
            {
                var shelfs = await Task.Run(() => BCApp.SCApplication.ShelfDefBLL.LoadShelf());
                shelfs.ForEach(shelf => setNewData(shelf));

                dgv_shelfData.DataSource = showShelfDefs;
                dgv_shelfData.Refresh();
                refreshZoneShelfInfo();
            }
            catch (Exception ex)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(ex, "Exception");
            }
        }

        void setNewData(sc.ShelfDef shelf)
        {
            if (shelf == null) return;
            var source_shelf_data = shelfDefs.Where(s => sc.Common.SCUtility.isMatche(s.ShelfID, shelf.ShelfID)).FirstOrDefault();
            if (source_shelf_data == null) return;
            source_shelf_data.put(shelf);
        }

        const int CELL_INDEX_SHELFID = 0;


        private async void btn_enable_Click(object sender, EventArgs e)
        {
            var selected_rows = dgv_shelfData.SelectedRows;
            if (selected_rows.Count <= 0)
            {
                MessageBox.Show("Please select want to disable shelf.");
                return;
            }
            var first_selected_row = selected_rows[0];
            string shelf_id = first_selected_row.Cells[CELL_INDEX_SHELFID].Value.ToString();
            string result = await Task.Run(() => BCApp.SCApplication.TransferService.Manual_ShelfEnable(shelf_id, true, ""));
            UpdateShelfData();
            MessageBox.Show($"Shelf:{shelf_id} enable {result}");
        }

        private async void btn_disable_Click(object sender, EventArgs e)
        {
            var selected_rows = dgv_shelfData.SelectedRows;
            if (selected_rows.Count <= 0)
            {
                MessageBox.Show("Please select want to enable shelf.");
                return;
            }
            string reason = txt_reason.Text;
            if (sc.Common.SCUtility.isEmpty(reason))
            {
                MessageBox.Show("Please enter the reason for disable shelf.");
                return;
            }
            var first_selected_row = selected_rows[0];
            string shelf_id = first_selected_row.Cells[CELL_INDEX_SHELFID].Value.ToString();
            string result = await Task.Run(() => BCApp.SCApplication.TransferService.Manual_ShelfEnable(shelf_id, false, reason));
            UpdateShelfData();
            MessageBox.Show($"Shelf:{shelf_id} disable {result}");

        }

        private void ShelfMaintenanceForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainForm.removeForm(nameof(ShelfMaintenanceForm));
        }

        private void cmb_zoneID_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected_zone_id = cmb_zoneID.Text;
            if (sc.Common.SCUtility.isEmpty(selected_zone_id))
            {
                table_HightLvlSet.Visible = false;
            }
            else
            {
                table_HightLvlSet.Visible = true;
            }
            refreshDataGridView();
            refreshZoneShelfInfo();
        }

        private void refreshZoneShelfInfo()
        {
            if (showShelfDefs == null || showShelfDefs.Count == 0)
            {
                lbl_totalShelfCount.Text = "";
                lbl_DisableCount.Text = "";
                lbl_hightLvl.Text = "";
            }
            else
            {
                string selected_zone_id = cmb_zoneID.Text;
                int total_count = showShelfDefs.Count;
                int disable_count = showShelfDefs.Where(s => s.Enable != "Y").Count();
                var zone = zoneDefs.Where(z => sc.Common.SCUtility.isMatche(z.ZoneID, selected_zone_id)).FirstOrDefault();
                int hight_lvl_count = 0;
                if (sc.Common.SCUtility.isEmpty(selected_zone_id))
                {
                    var total_reserve_count = zoneDefs.Sum(z => z.HighWaterMark);
                    if (total_reserve_count.HasValue)
                    {
                        hight_lvl_count = (int)total_reserve_count.Value;
                    }
                }
                else
                {
                    if (zone != null && zone.HighWaterMark.HasValue)
                    {
                        hight_lvl_count = (int)zone.HighWaterMark.Value;
                        num_hightLvl.Value = hight_lvl_count;
                    }
                }
                lbl_totalShelfCount.Text = total_count.ToString();
                lbl_DisableCount.Text = disable_count.ToString();
                lbl_hightLvl.Text = hight_lvl_count.ToString();
            }
        }

        private void refreshDataGridView()
        {
            string selected_zone_id = cmb_zoneID.Text;
            if (sc.Common.SCUtility.isEmpty(selected_zone_id))
            {
                showShelfDefs = shelfDefs;
            }
            else
            {
                var shelfs_temp = shelfDefs.Where(s => sc.Common.SCUtility.isMatche(s.ZoneID, selected_zone_id)).ToList();
                showShelfDefs = shelfs_temp;
            }
            dgv_shelfData.DataSource = showShelfDefs;
            dgv_shelfData.Refresh();
        }

        const int SHELF_ENABLE_DISABLE_CLOUMN_INDEX_ENABLE = 1;
        private void dgv_shelfData_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (dgv_shelfData.Rows.Count <= e.RowIndex) return;
            if (e.RowIndex < 0) return;
            var enable_status = dgv_shelfData.Rows[e.RowIndex].Cells[SHELF_ENABLE_DISABLE_CLOUMN_INDEX_ENABLE].Value;
            if (!(enable_status is string))
                return;
            string enable = enable_status as string;
            DataGridViewRow row = dgv_shelfData.Rows[e.RowIndex];
            if (sc.Common.SCUtility.isMatche(enable, sc.App.SCAppConstants.YES_FLAG))
            {
                row.DefaultCellStyle.BackColor = Color.White;
                row.DefaultCellStyle.ForeColor = Color.Black;
            }
            else
            {
                row.DefaultCellStyle.BackColor = Color.Yellow;
                row.DefaultCellStyle.ForeColor = Color.Red;
                if (row.Selected)
                {
                    row.DefaultCellStyle.SelectionBackColor = Color.SkyBlue;
                    row.DefaultCellStyle.SelectionForeColor = Color.Red;
                }
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                btn_setHightLvl.Enabled = false;
                bool is_success = false;
                string selected_zone_id = cmb_zoneID.Text;
                int hight_lvl = (int)num_hightLvl.Value;
                await Task.Run(() => is_success = BCApp.SCApplication.ZoneDefBLL.updateHighWater(selected_zone_id, hight_lvl));
                if (is_success)
                {
                    var zone = zoneDefs.Where(z => sc.Common.SCUtility.isMatche(z.ZoneID, selected_zone_id)).FirstOrDefault();
                    zone.HighWaterMark = hight_lvl;

                    refreshZoneShelfInfo();
                    MessageBox.Show("Set complete");
                }
                else
                {
                    MessageBox.Show("Set fail");
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                btn_setHightLvl.Enabled = true;
            }

        }
    }
}
