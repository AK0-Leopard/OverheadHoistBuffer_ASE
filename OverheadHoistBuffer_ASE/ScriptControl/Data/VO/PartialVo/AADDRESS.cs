using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using com.mirle.ibg3k0.sc.ProtocolFormat.OHTMessage;
using com.mirle.ibg3k0.sc.BLL;
using System.Collections;
using Newtonsoft.Json;

namespace com.mirle.ibg3k0.sc
{
    public partial class AADDRESS
    {
        private const int BIT_INDEX_CONTROL = 1;
        private const int BIT_INDEX_PORT = 2;
        private const int BIT_INDEX_SEGMENT = 5;

        public Boolean[] AddressTypeFlags { get; set; }
        public string[] SegmentIDs { get; set; }

        public event EventHandler<string> VehicleRelease;

        public void initialAddressType()
        {
            string s_type = ADR_ID.Substring(0, 2);
            int.TryParse(s_type, out int type);
            BitArray b = new BitArray(new int[] { type });
            AddressTypeFlags = new bool[b.Count];
            b.CopyTo(AddressTypeFlags, 0);
        }
        public void initialSegmentID(SectionBLL sectionBLL)
        {
            var sections = sectionBLL.cache.GetSectionsByFromAddress(ADR_ID);
            SegmentIDs = sections.Select(sec => sec.SEG_NUM).Distinct().ToArray();
            if (SegmentIDs.Length == 0)
            {
                throw new Exception($"Adr id:{ADR_ID},no setting on section or segment");
            }
        }

        [JsonIgnore]
        public bool IsPort
        { get { return AddressTypeFlags[BIT_INDEX_PORT]; } }
        [JsonIgnore]
        public bool IsControl
        { get { return AddressTypeFlags[BIT_INDEX_CONTROL]; } }
        [JsonIgnore]
        public bool IsSegment
        {
            get
            {
                return AddressTypeFlags[BIT_INDEX_SEGMENT];
            }
        }
    }
}
