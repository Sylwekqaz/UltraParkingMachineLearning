using System;
using System.Linq;
using System.Windows.Controls;
using Ultra.IO;

namespace Ultra.ClassyficatorTester
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (!Prompt.FolderPrompt(out var path))
                return;

            var reloadCache = Prompt.YesNo("Odświeżyć cache?");
            var observations = FeatureLoader.GetObservations(path, reloadCache,StatusBar.DrawTextProgressBar).ToList();
            Console.WriteLine($"Liczba obserwacji: {observations.Count}");

            var confusionMatrix = ClassificationValidator.CrossValidation(observations, iterations: 1000, splitPercent: 0.7);
            Console.WriteLine(confusionMatrix);

            Console.ReadKey();
        }

        
    }
}