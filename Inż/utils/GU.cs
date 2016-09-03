using System.Linq;
using Inż.Model;
using OpenCvSharp;

namespace Inż.utils
{
    internal static class Gu
    {
        public static Mat GetMask(Contour[] pts, int[] sizes)
        {
            var ptss = pts.Where(c => c.Pts.Count > 0).Select(c => c.Pts.Select(p => (Point) p)).ToArray();
            var ret = new Mat(sizes, MatType.CV_8UC3, new Scalar(0, 0, 0, 0));

            Cv2.FillPoly(ret, ptss, new Scalar(150, 150, 150, 150));
            return ret;
        }

        public static int[] GetSizes(this Mat mat)
        {
            return new[] {mat.Size(0), mat.Size(1)};
        }

        public static Mat AddLayers(params Mat[] mats)
        {
            if (mats.Length == 1)
            {
                return mats[0];
            }

            var ret = new Mat();
            Mat layers = AddLayers(mats.Skip(1).ToArray());
            Cv2.Add(mats.First(), layers, ret);
            return ret;
        }

        public static Mat Canny(Mat src)
        {
            var temp = new Mat();
            var ret = new Mat();
            Cv2.Canny(src, temp, 120, 150);
            Cv2.CvtColor(temp, ret, ColorConversionCodes.GRAY2BGR);
            return ret;
        }
    }
}