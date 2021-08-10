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
        public ShelfMaintenanceForm(BCMainForm _mainForm)
        {
            InitializeComponent();
            BCApp = _mainForm.BCApp;
            dgv_shelfData.AutoGenerateColumns = false;
            initialShelfData();
            UpdateShelfData();
            MainForm = _mainForm;
        }

        private async void initialShelfData()
        {
            var shelfs = await Task.Run(() => BCApp.SCApplication.ShelfDefBLL.LoadShelf());
            shelfDefs = shelfs;
            dgv_shelfData.DataSource = shelfDefs;
        }

        private async void UpdateShelfData()
        {
            try
            {
                var shelfs = await Task.Run(() => BCApp.SCApplication.ShelfDefBLL.LoadShelf());
                shelfs.ForEach(shelf => setNewData(shelf));
                dgv_shelfData.Refresh();
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
            string result = await Task.Run(() => BCApp.SCApplication.TransferService.Manual_ShelfEnable(shelf_id, true));
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
            var first_selected_row = selected_rows[0];
            string shelf_id = first_selected_row.Cells[CELL_INDEX_SHELFID].Value.ToString();
            string result = await Task.Run(() => BCApp.SCApplication.TransferService.Manual_ShelfEnable(shelf_id, false));
            UpdateShelfData();
            MessageBox.Show($"Shelf:{shelf_id} disable {result}");

        }

        private void ShelfMaintenanceForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainForm.removeForm(nameof(ShelfMaintenanceForm));
        }
    }
}
