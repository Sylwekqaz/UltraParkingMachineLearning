using System.Collections.Generic;
using System.Linq;
using Inż.Model;
using OpenCvSharp;

namespace Inż.utils
{
    internal static partial class Gu
    {
        public static Mat GetMask(IEnumerable<Contour> pts, int[] sizes, Scalar color, Scalar background)
        {
            var ptss = pts.Where(c => c.Pts.Count > 0).Select(c => c.Pts.Select(p => (Point) p)).ToArray();
            var ret = new Mat(sizes, MatType.CV_8UC3, background);

            Cv2.FillPoly(ret, ptss, color);
            return ret;
        }

        public static Mat GetMask(IEnumerable<Contour> pts, int[] sizes, Scalar color)
        {
            return GetMask(pts, sizes, color, Scalar.Black);
        }

        public static Mat GetMask(Contour contour, int[] sizes, Scalar color)
        {
            return GetMask(new[] {contour}, sizes, color);
        }

       

        public static Mat GetMask(Contour contour, int[] sizes, Scalar color, Scalar background)
        {
            return GetMask(new[] {contour}, sizes, color, background);
        }
    }
}