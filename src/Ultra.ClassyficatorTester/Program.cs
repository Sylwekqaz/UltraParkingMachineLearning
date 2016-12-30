using System;
using System.Linq;
using System.Windows.Controls;
using Ultra.IO;

namespace Ultra.ClassyficatorTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var observations = FeatureLoader.GetObservations(StatusBar.DrawTextProgressBar).ToList();
            Console.WriteLine();
            Console.WriteLine($"Liczba obserwacji: {observations.Count}");

            var confusionMatrix = ClassificationValidator.CrossValidation(observations, iterations: 1000, splitPercent: 0.7);
            Console.WriteLine(confusionMatrix);

            Console.ReadKey();
        }

        
    }
}