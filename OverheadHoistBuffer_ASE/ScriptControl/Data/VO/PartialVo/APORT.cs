using com.mirle.ibg3k0.bcf.App;
using com.mirle.ibg3k0.bcf.Common;
using com.mirle.ibg3k0.bcf.Data.ValueDefMapAction;
using com.mirle.ibg3k0.bcf.Data.VO;
using com.mirle.ibg3k0.sc.App;
using com.mirle.ibg3k0.sc.Data.SECS;
using com.mirle.ibg3k0.sc.Data.VO;
using com.mirle.ibg3k0.sc.Data.VO.Interface;
using com.mirle.ibg3k0.sc.ObjectRelay;
using com.mirle.ibg3k0.stc.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.sc
{
    public partial class APORT : BaseUnitObject, ICassetteLoader
    {
        public APORT()
        {
            eqptObjectCate = SCAppConstants.EQPT_OBJECT_CATE_PORT;
        }

        private CassetteLoader cassetteLoader = new CassetteLoader();
        public virtual CassetteLoader CassetteLoader
        {
            get { return cassetteLoader; }
        }
        public virtual void loadCassette(ACASSETTE cassette)
        {
            cassetteLoader.loadCassette(cassette);
        }
        public virtual void unloadCassette()
        {
            cassetteLoader.unloadCassette();
        }
        /// <summary>
        /// 開始執行初始化動作
        /// </summary>
        /// <param name="runLevel">腳本內容初始化執行層級，0~9 以0為最高優先</param>
        public override void doShareMemoryInit(BCFAppConstants.RUN_LEVEL runLevel)
        {
            foreach (IValueDefMapAction action in valueDefMapActionDic.Values)
            {
                action.doShareMemoryInit(runLevel);
            }
        }

    }

}
