using com.mirle.ibg3k0.bc.winform.Common;
using com.mirle.ibg3k0.bc.winform.i18n;
using com.mirle.ibg3k0.bc.winform.Properties;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.sc;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.bc.winform.ObjectRelay
{
    public class SectionObjToShow
    {
        ASECTION section = null;
        public SectionObjToShow(ASECTION myDatabaseObject)
        {
            this.section = myDatabaseObject;
        }
        [LocalizedDisplayNameAttribute("SEC_ID")]
        public string SEC_ID
        {
            get { return section.SEC_ID; }
        }
        [LocalizedDisplayNameAttribute("Dirve Dir.")]
        public int DIRC_DRIV
        {
            get { return (int)section.DIRC_DRIV; }
        }
        [LocalizedDisplayNameAttribute("Guide Dir.")]
        public int DIRC_GUID
        {
            get { return (int)section.DIRC_GUID; }
        }
    }

    public enum E_RAIL_DIR : int
    {
        [Display(Name = "E_RAIL_DIR_F", ResourceType = typeof(Resources))]
        F = 1,
        [Display(Name = "E_RAIL_DIR_R", ResourceType = typeof(Resources))]
        R = 2
    }

}
