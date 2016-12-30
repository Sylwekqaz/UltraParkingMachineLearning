using System.Collections.Generic;
using Logic.Classifiers;
using Logic.Model;

namespace ClassyficatorTester
{
    public static class ClasyficationValidator
    {
        public static (double TruePositive, double TrueNegative, double FalsePositive, double FalseNegative)
            Validate(List<ImageFeatures> train, List<ImageFeatures> validation)
        {
            var smvClassifier = SMVClassifier.Create(train);

            double tp = 0, tn = 0, fp = 0, fn = 0;
            foreach (var validationObservation in validation)
            {
                var predict = smvClassifier.Predict(validationObservation);
                if (predict)
                    if (validationObservation.IsOccupied)
                        tp++;
                    else
                        fp++;
                else if (validationObservation.IsOccupied)
                    fn++;
                else
                    tn++;
            }
            return (tp, tn, fp, fn);
        }

        public static (double TruePositive, double TrueNegative, double FalsePositive, double FalseNegative)
            CrossValidation(List<ImageFeatures> observations,int iterations, double splitPercent)
        {
            double tp = 0;
            double tn = 0;
            double fp = 0;
            double fn = 0;

            for (int i = 0; i < iterations; i++)
            {
                var tuple = observations.Shuffle().Split(splitPercent);
                var result = Validate(tuple.Item1, tuple.Item2);
                tp += result.Item1;
                tn += result.Item2;
                fp += result.Item3;
                fn += result.Item4;
            }
            return (tp, tn, fp, fn);
        }
    }
}