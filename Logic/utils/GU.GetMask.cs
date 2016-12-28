using System.Collections.Generic;
using System.Linq;
using Logic.Model;
using OpenCvSharp;

namespace Logic.utils
{
    public static partial class Gu
    {
        public static Mat GetMask(IEnumerable<Contour> pts, Size size, Scalar color, Scalar background)
        {
            var ptss = pts.Where(c => c.Count > 0).Select(c => c.Select(p => (Point) p)).ToArray();
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