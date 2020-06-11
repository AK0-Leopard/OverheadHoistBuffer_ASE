using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteKit
{
    sealed class Section
    {
        public string SectionCode { get; set; }
        public double Distance { get; set; }
        public string FromAdr { get; set; }
        public string ToAdr { get; set; }
        public string Status { get; set; }
        public string Segment { get; set; }
        List<Section> UpstreamSectionList = new List<Section>();
        List<Section> DownstreamSectionList = new List<Section>();

        public Section(string sectionCode, string fromAdr, string toAdr, Nullable<double> teachingDistance, string segment)
        {
            SectionCode = sectionCode;
            if (teachingDistance.HasValue)
                Distance = teachingDistance.Value;
            FromAdr = fromAdr;
            ToAdr = toAdr;
            Segment = segment;
        }

        public List<Section> UpstreamSectionLists
        {
            get
            {
                return UpstreamSectionList;
            }
        }

        public List<Section> DownstreamSectionLists
        {
            get
            {
                return DownstreamSectionList;
            }
        }

        public void isUpstreamOf(Section sec)
        {
            UpstreamSectionList.Add(sec);
        }

        public void isDownstreamOf(Section sec)
        {
            DownstreamSectionList.Add(sec);
        }
    }
}
