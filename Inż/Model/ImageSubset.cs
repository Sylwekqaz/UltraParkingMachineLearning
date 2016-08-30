using System;
using System.Drawing;
using System.Windows.Media;
using OpenCvSharp;


namespace In¿.Model
{
    public class ImageSubset
    {
        public Mat Org { get; set; } = new Mat();
        public Mat Gray { get; set; } = new Mat();
        public Mat Edges { get; set; } = new Mat();
        public LineSegmentPoint[] HoughSpace { get; set; }
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