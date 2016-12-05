using System.Collections.Generic;
using System.Linq;
using Inż.Model;
using OpenCvSharp;

namespace Inż.utils
{
    public static class ImageFeaturesExtensions
    {
        public static ImageFeatures CalculateFeatures(this Mat mat, Contour contour, bool? isOccupied=null)
        {
            return new ImageFeatures()
            {
                EdgePixels = Gu.CountEdgePixels(contour, mat),
                SaturatedPixels = Gu.CountSaturationPixels(contour, mat),
                MaskPixels = Gu.CountMaskArea(contour, mat),
                IsOccupied = isOccupied
            };
        }

        public static Mat ToPredictionMat(this ImageFeatures features)
        {
            float[] sample =
            {
                features.SaturatedPixelsRatio,
                features.EdgePixelsRatio,
            };
            return new Mat(1, 2, MatType.CV_32FC1, sample);
        }

        public static Mat ToTrainingMat(this List<ImageFeatures> features)
        {
            var count = features.Count;

            float[,] sample = new float[count, 3];
            for (int i = 0; i < count; i++)
            {
                sample[i, 0] = features[i].SaturatedPixels;
                sample[i, 1] = features[i].EdgePixels;
            }

            return new Mat(count, 2, MatType.CV_32FC1, sample);
        }

        public static Mat ToResponseMat(this List<ImageFeatures> features)
        {
            int[] responses = features.Select(f => f.IsOccupied.Value ? 1 : 2).ToArray();

            return new Mat(responses.Length, 1, MatType.CV_32SC1, responses);
        }


    }
}