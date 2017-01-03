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
            var smvClassifier = SMVClassifier.Create(train);

            var confusionMatrix = new ConfusionMatrix();
            foreach (var validationObservation in validation)
            {
                var predict = smvClassifier.Predict(validationObservation);
                confusionMatrix.AddVote(actual: validationObservation.IsOccupied, predicted: predict);
            }
            return confusionMatrix;
        }

        public static ConfusionMatrix CrossValidation(List<ImageFeatures> observations, int iterations,
            double splitPercent)
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

        public static ConfusionMatrix NSubOneValidation(List<ImageFeatures> observations)
        {
            var confumaMatrix = new ConfusionMatrix();
            for (var i = 0; i < observations.Count; i++)
            {
                var validation = observations.Where((features, i1) => i1==i).ToList();
                var train = observations.Where((features, i1) => i1 != i).ToList();
                confumaMatrix += Validate(train, validation);
            }

            return confumaMatrix;
        }

    }
}