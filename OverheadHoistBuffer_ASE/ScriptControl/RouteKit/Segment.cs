using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteKit
{
    sealed class Segment
    {
        public string SegmentCode { get; set; }
        public string SegmentType { get; set; }
        public string Status { get; set; }
        public string[] Address { get; set; }
        public double Distance { get; set; }

        public Segment(string segmentCode, string segmentType, Nullable<double> distance, string[] address)
        {
            SegmentCode = segmentCode;
            SegmentType = segmentType;
            Address = address;
            if (distance.HasValue)
                Distance = distance.Value;
        }


        List<Segment> UpstreamSegmentList = new List<Segment>();
        List<Segment> DownstreamSegmentList = new List<Segment>();
        public List<Segment> UpstreamSegments
        {
            get
            {
                return UpstreamSegmentList;
            }
        }

        public List<Segment> DownstreamSegments
        {
            get
            {
                return DownstreamSegmentList;
            }
        }

        public void isUpstreamOf(Segment seg)
        {
            DownstreamSegmentList.Add(seg);
        }

        public void isDownstreamOf(Segment seg)
        {
            UpstreamSegmentList.Add(seg);
        }

        public override string ToString()
        {
            return SegmentCode;
        }
    }
}
