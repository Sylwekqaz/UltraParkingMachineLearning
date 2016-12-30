using System.Collections.Generic;
using System.Linq;
using Ultra.Contract.Model;
using OpenCvSharp;

namespace Ultra.MachineLearning
{
    public static partial class ImageProcessor
    {
        public static Mat GetMask(IEnumerable<Contour> pts, Size size, Scalar color, Scalar background)
        {
            var ptss = pts.Where(c => c.Count > 0).Select(c => c.Select(p => new Point(p.X,p.Y))).ToArray();
            var ret = new Mat(size, MatType.CV_8UC3, background);

            Cv2.FillPoly(ret, ptss, color);
            return ret;
        }

        public static Mat GetMask(IEnumerable<Contour> pts, Size size, Scalar color)
        {
            return GetMask(pts, size, color, Scalar.Black);
        }

        public static Mat GetMask(Contour contour, Size size, Scalar color)
        {
            return GetMask(new[] {contour}, size, color);
        }

       

        public static Mat GetMask(Contour contour, Size size, Scalar color, Scalar background)
        {
            return GetMask(new[] {contour}, size, color, background);
        }
    }
}