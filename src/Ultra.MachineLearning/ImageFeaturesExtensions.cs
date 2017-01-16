using System;
using System.Collections.Generic;
using System.Linq;
using Ultra.Contract.Model;
using OpenCvSharp;

namespace Ultra.MachineLearning
{
    public static class ImageFeaturesExtensions
    {
        public static ImageFeatures CalculateFeatures(this Mat mat, Contour contour, bool isOccupied)
        {
            var hsvColorStats = ImageProcessor.GetHSVColorStats(contour, mat);

            var features = new ImageFeatures
            {
                IsOccupied = isOccupied,
                // counted edges and Chromated pixels
                EdgePixels = ImageProcessor.CountEdgePixels(contour, mat),
                ChromatedPixels = ImageProcessor.CountChromatedPixels(contour, mat),
                MaskPixels = ImageProcessor.CountMaskArea(contour, mat),
                //hsv stats
                SaturationMean = hsvColorStats.Item1.Item1,
                SaturationStddev = hsvColorStats.Item1.Item2,
                ValueMean = hsvColorStats.Item2.Item1,
                ValueStddev = hsvColorStats.Item2.Item2
            };

            return features;
        }

        public static Mat ToPredictionMat(this ImageFeatures features)
        {
            var array = features.ToArray();
            return new Mat(1, array.Length, MatType.CV_32FC1, array);
        }

        public static Mat ToTrainingMat(this List<ImageFeatures> features)
        {
            var array = features.Select(ToArray).To2D();
            return new Mat(array.GetLength(0), array.GetLength(1), MatType.CV_32FC1, array);
        }

        public static Mat ToResponseMat(this List<ImageFeatures> features)
        {
            int[] responses = features.Select(f => Convert.ToInt32(f.IsOccupied)).ToArray();

            return new Mat(responses.Length, 1, MatType.CV_32SC1, responses);
        }

        private static float[] ToArray(this ImageFeatures features)
        {
            return new[]
            {
                features.ChromatedPixelsRatio,
                features.EdgePixelsRatio,
                features.SaturationMean,
                features.SaturationStddev,
                features.ValueMean,
                features.ValueStddev,
            };
        }

        static float[,] To2D(this IEnumerable<float[]> source)
        {
            try
            {
                List<float[]> list = source as List<float[]> ?? source.ToList();

                int firstDim = list.Count();
                int secondDim = list.GroupBy(row => row.Length).Single().Key; // throws InvalidOperationException if source is not rectangular

                var result = new float[firstDim, secondDim];
                for (int i = 0; i < firstDim; ++i)
                    for (int j = 0; j < secondDim; ++j)
                        result[i, j] = list[i][j];

                return result;
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("The given jagged array is not rectangular.");
            }
        }
    }
}