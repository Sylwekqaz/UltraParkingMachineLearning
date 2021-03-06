﻿using System;
using System.Linq;
using Ultra.Contract.Model;
using OpenCvSharp;

namespace Ultra.MachineLearning
{
    public static partial class ImageProcessor
    {
        public static Mat DetectEdges(this Mat src, double sigma = 0.33)
        {
            var graySrc = src.CvtColor(ColorConversionCodes.BGR2GRAY);
            Cv2.MeanStdDev(graySrc, out var meanScalar, out var stddevScalar);
            var mean = meanScalar[0];
            var lower = (int) Math.Max(0, (1.0 - sigma) * mean);
            var upper = (int) Math.Min(255, (1.0 + sigma) * mean);
            return graySrc.Canny(lower, upper);
        }

        public static int CountEdgePixels(Contour contour, Mat src)
        {
            var rect = GetContourRect(contour, src.Height,src.Width);

            var mask = GetMask(contour, src.Size(), color: Scalar.White, background: Scalar.Black)
                .Clone(rect)
                .CvtColor(ColorConversionCodes.BGR2GRAY);

            return src
                .Clone(rect)
                .DetectEdges()
                .BitwiseAnd(mask)
                .Threshold(100, 255, ThresholdTypes.Binary)
                .CountNonZero();
        }


        public static int CountChromatedPixels(Contour contour, Mat src)
        {
            var rect = GetContourRect(contour, src.Height, src.Width);

            var mask = GetMask(contour, src.Size(), color: Scalar.White, background: Scalar.Black)
                .Clone(rect)
                .CvtColor(ColorConversionCodes.BGR2GRAY);

            return src
                .Clone(rect)
                .CvtColor(ColorConversionCodes.BGR2HSV)
                .GetChromaLayer()
                .BitwiseAnd(mask)
                .Threshold(100, 255, ThresholdTypes.Binary)
                .CountNonZero();
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

            var mask = GetMask(contour, src.Size(), color: Scalar.White, background: Scalar.Black)
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

            return GetMask(contour, src.Size(), color: Scalar.White, background: Scalar.Black)
                .Clone(rect)
                .CvtColor(ColorConversionCodes.BGR2GRAY)
                .Threshold(200, 255, ThresholdTypes.Binary)
                .CountNonZero();
        }

        public static Mat GetChromaLayer(this Mat src)
        {
            var mats = src.Split();
            return mats[1].Mul(mats[2], 1.0/255);
        }

        private static Rect GetContourRect(Contour contour, int height, int width)
        {
            var minX = (int) Math.Floor(contour.Min(p => p.X));
            var minY = (int) Math.Floor(contour.Min(p => p.Y));
            var maxX = (int) Math.Ceiling(contour.Max(p => p.X));
            var maxY = (int) Math.Ceiling(contour.Max(p => p.Y));

            int ValueBetwen(int value, int min, int max)
            {
                return value < min
                    ? min
                    : value > max
                        ? max
                        : value;
            }

            minX = ValueBetwen(minX, 0, width - 1);
            maxX = ValueBetwen(maxX, 1, width);
            minY = ValueBetwen(minY, 0, height - 1);
            maxY = ValueBetwen(maxY, 1, height);

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }
    }
}