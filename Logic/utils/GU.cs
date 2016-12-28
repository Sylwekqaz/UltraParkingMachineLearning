using System;
using System.Linq;
using Logic.Model;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace Logic.utils
{
    public static partial class Gu
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

        public static int CountEdgePixels(Contour contour, Mat src)
        {
            var rect = GetContourRect(contour, src.Height,src.Width);

            var mask = GetMask(contour, src.GetSizes(), color: Scalar.White, background: Scalar.Black)
                .Clone(rect)
                .CvtColor(ColorConversionCodes.BGR2GRAY)
                .Threshold(200,255, ThresholdTypes.Binary);

            var satMat = src
                .Clone(rect)
                .DetectEdges()
                .BitwiseAnd(mask)
                .Threshold(100, 255, ThresholdTypes.Binary);

            var white = Cv2.CountNonZero(satMat);

            return white;
        }


        public static int CountSaturationPixels(Contour contour, Mat src)
        {
            var rect = GetContourRect(contour,src.Height,src.Width);

            var mask = GetMask(contour, src.GetSizes(), color: Scalar.White, background: Scalar.Black)
                .Clone(rect)
                .CvtColor(ColorConversionCodes.BGR2GRAY)
                .Threshold(200, 255, ThresholdTypes.Binary);

            var satMat = src
                .Clone(rect)
                .CvtColor(ColorConversionCodes.BGR2HSV)
                .ScaleSaturationWithValue() // returns only saturation layer
                .BitwiseAnd(mask)
                .Threshold(100, 255, ThresholdTypes.Binary);


            var white = Cv2.CountNonZero(satMat);

            return white;
        }

        /// <summary>
        /// Returns EX and DX (Mean and Standard Derivation) for saturation and value layers in HSV color model with values from 0.0 to 1.0 (not 0-255)
        /// </summary>
        /// <param name="contour"></param>
        /// <param name="src"></param>
        /// <returns></returns>
        public static ((float mean, float stddev) saturation, (float mean, float stddev) value) GetHSVColorStats(
            Contour contour, Mat src)
        {
            var rect = GetContourRect(contour, src.Height, src.Width);

            var mask = GetMask(contour, src.GetSizes(), color: Scalar.White, background: Scalar.Black)
                .Clone(rect)
                .CvtColor(ColorConversionCodes.BGR2GRAY);

            var layers = src.Clone(rect)
                .CvtColor(ColorConversionCodes.BGR2HSV)
                .Split();

            (float mean, float stddev) LocalMeanStdDev(Mat area)
            {
                Cv2.MeanStdDev(area, out var scalarMean, out var scalarStddev, mask);
                var mean = (float) (scalarMean[0] / 255);
                var stddev = (float) (scalarStddev[0] / 255);
                return (mean, stddev);
            }

            return (LocalMeanStdDev(layers[1])/*saturation layer*/, LocalMeanStdDev(layers[2])/* value layer*/);
        }

        public static int CountMaskArea(Contour contour, Mat src)
        {
            var rect = GetContourRect(contour, src.Height, src.Width);

            var mask = GetMask(contour, src.GetSizes(), color: Scalar.White, background: Scalar.Black)
                .Clone(rect)
                .CvtColor(ColorConversionCodes.BGR2GRAY)
                .Threshold(200, 255, ThresholdTypes.Binary);

            var white = Cv2.CountNonZero(mask);
            return white;
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