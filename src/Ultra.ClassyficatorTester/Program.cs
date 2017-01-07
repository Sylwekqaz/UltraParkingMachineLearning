using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using ConsoleTables;
using Ultra.Contract.Model;
using Ultra.IO;

namespace Ultra.ClassyficatorTester
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (!Prompt.FolderPrompt(out var path))
                return;

            var reloadCache = Prompt.YesNo("Odświeżyć cache?");
            var observations = FeatureLoader.GetObservations(path, reloadCache,StatusBar.DrawTextProgressBar).ToList();
            Console.WriteLine($"Liczba obserwacji: {observations.Count}");

            CrossValidation(observations);

            NsubOneValidation(observations);

            
            Console.WriteLine("Naciśnij dowolny klawisz aby zamknąć");
            while (Console.KeyAvailable)
                Console.ReadKey(true); //discard stacked keyboard events
            Console.ReadKey(true);
            }

        private static void NsubOneValidation(List<ImageFeatures> observations)
        {
            Console.WriteLine("Walidacja N-1");
            var cmatrix = ClassificationValidator.LeaveOneOutValidation(observations);
            cmatrix.PrintToConsole();
        }

        private static void CrossValidation(List<ImageFeatures> observations)
        {
            int iterations = 1000;
            double splitPercent = 0.7;
            Console.WriteLine($"Crossvalidacja {iterations} iteracji , podział zbioru {splitPercent*100}%-{100 - splitPercent * 100}%");
            var confusionMatrix = ClassificationValidator.CrossValidation(observations, iterations, splitPercent);
            confusionMatrix.PrintToConsole();
        }

        private static void PrintToConsole(this ConfusionMatrix cm)
        {
            new ConsoleTable("Predicted\\Actual", "True", "False")
                .AddRow("True",cm.TruePositive,cm.FalsePositive)
                .AddRow("False",cm.FalseNegative,cm.TrueNegative)
                .Write(Format.Alternative);

            // Statistics
            Console.WriteLine($"Sensitivity TPR: {cm.TruePositiveRatio}");
            Console.WriteLine($"Sensitivity TNR: {cm.TrueNegativeRatio}");
            Console.WriteLine($"Accuracy ACC: {cm.Accuracy}");
            Console.WriteLine();
            Console.WriteLine();

        }
    }
}