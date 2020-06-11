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
    public partial class VehicleDataSettingForm : Form
    {
        BCMainForm mainForm = null;
        public VehicleDataSettingForm(string a, string b)
        {
            InitializeComponent();
            //mainForm = _mainForm;
            List<ObjectRelay.SectionObjToShow> sectionObjTos = new List<ObjectRelay.SectionObjToShow>();
            //dataGridView1.DataSource = sectionObjTos;

            //comboBox1.DataSource = Enum.GetValues(typeof(ObjectRelay.E_RAIL_DIR));
            //comboBox1.ValueMember = "Value";
            //comboBox1.DisplayMember = "Name";
        }

        private void SectionDataEditForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainForm.removeForm(typeof(VehicleDataSettingForm).Name);

        }

        private void lab_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
