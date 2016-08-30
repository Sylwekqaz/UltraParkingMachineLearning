using System;
using System.Windows.Media;
using Emgu.CV;
using Emgu.CV.Structure;

namespace In¿.Model
{
    public class ImageSubset
    {
        public Mat Org { get; set; } = new Mat();
        public Mat Gray { get; set; } = new Mat();
        public Mat Edges { get; set; } = new Mat();
        public LineSegment2D[] HoughSpaceP { get; set; }
        public Mat HoughP { get; set; } = new Mat();

        public Mat this[int value]
        {
            get {
                switch (value)
                {
                    case 0:
                        return Org;
                    case 1:
                        return Gray;
                    case 2:
                        return Edges;
                    case 3:
                        return HoughP;

                    default:
                        throw new IndexOutOfRangeException(nameof(value));
                }
            }
        }
    }
}