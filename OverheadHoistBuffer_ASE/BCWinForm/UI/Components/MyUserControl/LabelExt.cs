using com.mirle.ibg3k0.bc.winform.i18n;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.mirle.ibg3k0.bc.winform.UI.Components.MyUserControl
{
    class LabelExt : Label
    {
        [Browsable(true)]
        public string DisplayName { get; set; } = "Non";
        //public string DisplayName { get; set; } = "N";



        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Point x = this.Location;
            //Location = new Point(3, 44);
            this.Text = MsgHelper.getMsg(DisplayName);
        }

        //private string DisplayValue()
        //{
        //    return MsgHelper.getMsg(this.DisplayName, this.DisplayName);
        //}
    }
}
