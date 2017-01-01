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
            var reloadCache = Prompt.YesNo("Odświerzyć cache?");
            var observations = FeatureLoader.GetObservations(@"..\..\..\..\DataSet\PhoneCamera", reloadCache,StatusBar.DrawTextProgressBar).ToList();
            Console.WriteLine($"Liczba obserwacji: {observations.Count}");

            var confusionMatrix = ClassificationValidator.CrossValidation(observations, iterations: 1000, splitPercent: 0.7);
            Console.WriteLine(confusionMatrix);

            Console.ReadKey();
        }

        
    }
}