using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Classifiers;
using Logic.Model;
using Logic.utils;
using Newtonsoft.Json;
using OpenCvSharp;

namespace ClassyficatorTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var observations = GetObservations().ToList();

            double tp = 0;
            double tn = 0;
            double fp = 0;
            double fn = 0;
            for (int i = 0; i < 20; i++)
            {
                var tuple = observations.Shuffle().Split(0.7);
                var result = ValidateClassyficator(tuple.Item1, tuple.Item2);
                tp += result.Item1;
                tn += result.Item2;
                fp += result.Item3;
                fn += result.Item4;
            }

            //print tp
            Console.WriteLine($"True Positive: {tp}");
            Console.WriteLine($"True Negative: {tn}");
            Console.WriteLine($"False Positive: {fp}");
            Console.WriteLine($"False Negative: {fn}");
            Console.WriteLine();
            // print ratio
            Console.WriteLine($"Sensitivity TPR: {tp/(tp+fn)}");
            Console.WriteLine($"Sensitivity TNR: {tn/(tn+fp)}");
            Console.WriteLine($"Accuracy ACC: {(tp+tn)/(tp+tn+fp+fn)}");


            Console.ReadKey();
        }

        private static Tuple<double, double, double, double> ValidateClassyficator(List<ImageFeatures> train, List<ImageFeatures> validation)
        {
            var smvClassifier = SMVClassifier.Create(train);

            double tp = 0, tn = 0, fp = 0, fn = 0;
            foreach (var validationObservation in validation)
            {
                var predict = smvClassifier.Predict(validationObservation);
                if (predict == 1)
                    if (validationObservation.IsOccupied)
                        tp++;
                    else
                        fp++;
                else if (validationObservation.IsOccupied)
                    fn++;
                else
                    tn++;
            }
            return new Tuple<double,double,double,double>(tp,tn,fp,fn);
        }

        private static IEnumerable<ImageFeatures> GetObservations()
        {
            string directoryPath = @"..\..\..\..\DataSet\PhoneCamera";

            string[] extensions = {".png", ".jpg", ".jpeg", ".bmp"};
            var files = Directory.EnumerateFiles(directoryPath)
                .Where(path => extensions
                    .Any(ext => ext.Equals(Path.GetExtension(path), StringComparison.InvariantCultureIgnoreCase)));

            foreach (var path in files)
            {
                var jsonPath = Path.ChangeExtension(path, ".json");
                var slots = JsonConvert.DeserializeObject<List<ParkingSlot>>(File.ReadAllText(jsonPath));
                using (var image = new Mat(path))
                {
                    foreach (var slot in slots)
                    {
                        var calculateFeatures = image.CalculateFeatures(slot.Contour, slot.IsOccupied);
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        yield return calculateFeatures;
                    }
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}