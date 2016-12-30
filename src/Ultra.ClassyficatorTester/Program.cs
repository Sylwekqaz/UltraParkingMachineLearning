﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ultra.Contract.Model;
using Newtonsoft.Json;
using OpenCvSharp;
using Ultra.MachineLearning;
using static ClassyficatorTester.StatusBar;
using static Ultra.ClassyficatorTester.ClassificationValidator;

namespace ClassyficatorTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var observations = GetObservations().ToList();
            Console.WriteLine($"Liczba obserwacji: {observations.Count}");

            var confusionMatrix = CrossValidation(observations, iterations: 1000, splitPercent: 0.7);
            Console.WriteLine(confusionMatrix);

            Console.ReadKey();
        }

        private static IEnumerable<ImageFeatures> GetObservations()
        {
            Console.WriteLine("Przetwarzanie danych testowych");
            string directoryPath = @"..\..\..\..\DataSet\PhoneCamera";

            string[] extensions = {".png", ".jpg", ".jpeg", ".bmp"};
            var files = Directory.EnumerateFiles(directoryPath)
                .Where(path => extensions
                    .Any(ext => ext.Equals(Path.GetExtension(path), StringComparison.InvariantCultureIgnoreCase)))
                .ToList();

            DrawTextProgressBar(0, files.Count);
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
                DrawTextProgressBar(files.IndexOf(path) + 1, files.Count);
            }
            Console.WriteLine();
        }
    }
}