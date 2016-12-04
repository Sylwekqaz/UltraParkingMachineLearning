using System;
using System.Diagnostics;
using System.Linq;
using Inż.Model;
using OpenCvSharp;

namespace Inż.utils
{
    internal static partial class Gu
    {
        public static Mat AddLayers(params Mat[] mats)
        {
            if (mats.Length == 1)
            {
                return mats[0];
            }

            var ret = new Mat();
            var layers = AddLayers(mats.Skip(1).ToArray());
            Cv2.Add(mats.First(), layers, ret);
            return ret;
        }

        public static Mat DetectEdges(this Mat src)
        {
            return src.CvtColor(ColorConversionCodes.BGR2GRAY)
                .Canny(40, 50);
        }

        public static double EdgeTreshold(Contour contour, Mat src)
        {
            var rect = GetContourRect(contour, src.Height,src.Width);

            var mask = GetMask(contour, src.GetSizes(), color: Scalar.White, background: Scalar.Black)
                .Clone(rect)
                .CvtColor(ColorConversionCodes.BGR2GRAY)
                .Threshold(200, 1, ThresholdTypes.Binary);

            var satMat = src
                .Clone(rect)
                .DetectEdges()
                .Mul(mask)
                .ToMat()
                .Threshold(100, 255, ThresholdTypes.Binary);


            var all = Cv2.CountNonZero(mask);
            var white = Cv2.CountNonZero(satMat);

            var ratio = (double) white/all;
            return ratio;
        }


        public static double SaturationTreshold(Contour contour, Mat src)
        {
            var rect = GetContourRect(contour,src.Height,src.Width);

            var mask = GetMask(contour, src.GetSizes(), color: Scalar.White, background: Scalar.Black)
                .Clone(rect)
                .CvtColor(ColorConversionCodes.BGR2GRAY)
                .Threshold(200, 1, ThresholdTypes.Binary);

            var satMat = src
                .Clone(rect)
                .CvtColor(ColorConversionCodes.BGR2HSV)
                .ScaleSaturationWithValue() // returns only saturation layer
                .Mul(mask)
                .ToMat()
                .Threshold(100, 255, ThresholdTypes.Binary);


            var all = Cv2.CountNonZero(mask);
            var white = Cv2.CountNonZero(satMat);

            var ratio = (double) white/all;
            return ratio;
        }

        public static Mat FastNlMeansDenoisingColored(this Mat src, float h = 3F, float hColor = 3F,
            int templateWindowSize = 7, int searchWindowSize = 21)
        {
            var dst = new Mat();
            Cv2.FastNlMeansDenoisingColored(src, dst, h, hColor, templateWindowSize, searchWindowSize);
            return dst;
        }

        /// <summary>
        /// </summary>
        /// <param name="src"></param>
        /// <returns>Only Saturate Layer</returns>
        public static Mat ScaleSaturationWithValue(this Mat src)
        {
            var mats = src.Split();
            return mats[1].Mul(mats[2], 1.0/255);
        }

        private static Rect GetContourRect(Contour contour, int height, int width)
        {
            var minX = (int) Math.Floor(contour.Pts.Min(p => p.X));
            var minY = (int) Math.Floor(contour.Pts.Min(p => p.Y));
            var maxX = (int) Math.Ceiling(contour.Pts.Max(p => p.X));
            var maxY = (int) Math.Ceiling(contour.Pts.Max(p => p.Y));

            var valueBetwen = new Func<int, int, int, int>((value, min, max) =>
                value < min
                    ? min
                    : value > max ? max : value);

            minX = valueBetwen(minX, 0, width - 1);
            maxX = valueBetwen(maxX, 1, width);
            minY = valueBetwen(minY, 0, height - 1);
            maxY = valueBetwen(maxY, 1, height);

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }
    }
}