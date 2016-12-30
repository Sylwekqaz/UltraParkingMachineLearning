using System.Collections.Generic;
using System.Text;
using Logic.Classifiers;
using Logic.Model;

namespace ClassyficatorTester
{
    public static class ClasyficationValidator
    {
        public static ConfusionMatrix
            Validate(List<ImageFeatures> train, List<ImageFeatures> validation)
        {
            var smvClassifier = SMVClassifier.Create(train);

            var confusionMatrix = new ConfusionMatrix();
            foreach (var validationObservation in validation)
            {
                var predict = smvClassifier.Predict(validationObservation);
                confusionMatrix.AddVote(actual: validationObservation.IsOccupied, predicted: predict);
            }
            return confusionMatrix;
        }

        public static ConfusionMatrix
            CrossValidation(List<ImageFeatures> observations, int iterations, double splitPercent)
        {
            var summaryConfusionMatrix = new ConfusionMatrix();
            for (int i = 0; i < iterations; i++)
            {
                var tuple = observations.Shuffle().Split(splitPercent);
                var iterationConfusionMatrix = Validate(tuple.Item1, tuple.Item2);
                summaryConfusionMatrix += iterationConfusionMatrix;
            }
            return summaryConfusionMatrix;
        }
    }

    public struct ConfusionMatrix
    {
        public int TruePositive { get; set; }
        public int TrueNegative { get; set; }
        public int FalsePositive { get; set; }
        public int FalseNegative { get; set; }


        public double TruePositiveRatio => (double) TruePositive / (TruePositive + FalseNegative);
        public double TrueNegativeRatio => (double) TrueNegative / (TrueNegative + FalsePositive);

        public double Accuracy
            => ((double) TruePositive + TrueNegative) / (TruePositive + TrueNegative + FalsePositive + FalseNegative);

        public void AddVote(bool actual, bool predicted)
        {
            switch (predicted)
            {
                case true when actual == true:
                    TruePositive++;
                    break;
                case true when actual == false:
                    FalsePositive++;
                    break;
                case false when actual == true:
                    FalseNegative++;
                    break;
                case false when actual == false:
                    TrueNegative++;
                    break;
            }
        }

        public static ConfusionMatrix operator +(ConfusionMatrix left, ConfusionMatrix right)
        {
            return new ConfusionMatrix()
            {
                TruePositive = left.TruePositive + right.TruePositive,
                TrueNegative = left.TrueNegative + right.TrueNegative,
                FalsePositive = left.FalsePositive + right.FalsePositive,
                FalseNegative = left.FalseNegative + right.FalseNegative,
            };
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            //Print Matrix
            builder.AppendLine($"True Positive: {TruePositive}");
            builder.AppendLine($"True Negative: {TrueNegative}");
            builder.AppendLine($"False Positive: {FalsePositive}");
            builder.AppendLine($"False Negative: {FalseNegative}");
            builder.AppendLine();
            // Statistics
            builder.AppendLine($"Sensitivity TPR: {TruePositiveRatio}");
            builder.AppendLine($"Sensitivity TNR: {TrueNegativeRatio}");
            builder.AppendLine($"Accuracy ACC: {Accuracy}");
            return builder.ToString();
        }
    }
}