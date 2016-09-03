using System;
using System.Linq;
using Inż.Model;
using OpenCvSharp;

namespace Inż.utils
{
    internal static class Gu
    {
        public static Mat GetMask(Contour[] pts, int[] sizes, Scalar color)
        {
            return GetMask(pts, sizes, color, Scalar.Black);
        }

        public static Mat GetMask(Contour[] pts, int[] sizes, Scalar color, Scalar background)
        {
            var ptss = pts.Where(c => c.Pts.Count > 0).Select(c => c.Pts.Select(p => (Point) p)).ToArray();
            var ret = new Mat(sizes, MatType.CV_8UC3, background);

            Cv2.FillPoly(ret, ptss, color);
            return ret;
        }

        public static Mat GetMask(Contour contour, int[] sizes, Scalar color)
        {
            return GetMask(new[] { contour }, sizes, color);
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
            return src.CvtColor(ColorConversionCodes.BGR2GRAY)
                .MedianBlur(5)
                .Canny(40,50)
                //.AdaptiveThreshold(255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.BinaryInv, 51, 2)
                .CvtColor(ColorConversionCodes.GRAY2BGR);
        }

        public static bool EdgeTreshold(Contour contour,Mat srcEdges)
        {
            var m = GetMask(contour, srcEdges.GetSizes(), Scalar.White,Scalar.Black).CvtColor(ColorConversionCodes.BGR2GRAY);
            var e = srcEdges.CvtColor(ColorConversionCodes.BGR2GRAY);
            var mask = new MatOfByte(m).GetIndexer();
            var edge = new MatOfByte(e).GetIndexer();
            double all = 0;
            double white = 0;



            for (int y = 0; y < m.Height; y++)
            {
                for (int x = 0; x < m.Width; x++)
                {
                    if (mask[y,x]==255)
                    {
                        all++;
                        if (edge[y,x]==255)
                        {
                            white++;
                        }
                    }
                }
            }

            return white/all >0.1;
        }

        public static Mat GetMask(Contour contour, int[] sizes, Scalar color, Scalar background)
        {
            return GetMask(new[] {contour}, sizes, color, background);
        }
    }
}