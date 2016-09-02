using System;
using System.Drawing;
using System.Windows.Media;
using OpenCvSharp;


namespace In¿.Model
{
    public class ImageSubset
    {
        public Mat Org { get; set; } = new Mat();
        public Mat Edges { get; set; } = new Mat();
        public Mat Mask { get; set; }

        public Mat this[int value]
        {
            get {
                switch (value)
                {
                    case 0:
                        return Org;
                    case 1:
                        return Edges;

                    default:
                        throw new IndexOutOfRangeException(nameof(value));
                }
            }
        }
    }
}