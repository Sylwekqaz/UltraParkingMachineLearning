using System.Collections.Generic;
using System.Linq;
using Ultra.Contract.Model;
using Ultra.MachineLearning.Classifiers;

namespace Ultra.ClassyficatorTester
{
    public static class ClassificationValidator
    {
        public static ConfusionMatrix Validate(List<ImageFeatures> train, List<ImageFeatures> validation)
        {
            var svmClassifier = SVMClassifier.Create(train);

            var confusionMatrix = new ConfusionMatrix();
            foreach (var validationObservation in validation)
            {
                var predict = svmClassifier.Predict(validationObservation);
                confusionMatrix.AddVote(actual: validationObservation.IsOccupied, predicted: predict);
            }
            return confusionMatrix;
        }

        public static ConfusionMatrix CrossValidation(List<ImageFeatures> observations, int iterations,
            double splitRatio)
        {
            var summaryConfusionMatrix = new ConfusionMatrix();
            for (int i = 0; i < iterations; i++)
            {
                var tuple = observations.Shuffle().Split(splitRatio);
                var iterationConfusionMatrix = Validate(tuple.Item1, tuple.Item2);
                summaryConfusionMatrix += iterationConfusionMatrix;
            }
            return summaryConfusionMatrix;
        }

        public static ConfusionMatrix LeaveOneOutValidation(List<ImageFeatures> observations)
        {
            var confumaMatrix = new ConfusionMatrix();
            for (var i = 0; i < observations.Count; i++)
            {
                var validation = new List<ImageFeatures> {observations[i]};
                var train = observations.WithoutElementAt(i);
                confumaMatrix += Validate(train, validation);
            }

            return confumaMatrix;
        }
    }
}