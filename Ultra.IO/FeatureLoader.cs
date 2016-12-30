using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using OpenCvSharp;
using Ultra.Contract.Model;
using Ultra.MachineLearning;

namespace Ultra.IO
{
    public static class FeatureLoader
    {
        public static List<string> GetPhotos()
        {
            string directoryPath = @"..\..\..\..\DataSet\PhoneCamera";
            string[] extensions = {".png", ".jpg", ".jpeg", ".bmp"};

            return Directory.EnumerateFiles(directoryPath)
                .Where(path => extensions
                    .Any(ext => ext.Equals(Path.GetExtension(path), StringComparison.InvariantCultureIgnoreCase)))
                .ToList();
        }

        public static List<ParkingSlot> GetSlots(string path)
        {
            var jsonPath = Path.ChangeExtension(path, ".json");
            return JsonConvert.DeserializeObject<List<ParkingSlot>>(File.ReadAllText(jsonPath));
        }

        public static IEnumerable<ImageFeatures> GetObservations(Action<int, int> reportProgres = null)
        {
            var paths = GetPhotos();
            var progress = 0;
            reportProgres?.Invoke(progress, paths.Count);
            foreach (var path in paths)
            {
                using (var image = new Mat(path))
                {
                    foreach (var slot in GetSlots(path))
                    {
                        var calculateFeatures = image.CalculateFeatures(slot.Contour, slot.IsOccupied);
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        yield return calculateFeatures;
                    }
                    reportProgres?.Invoke(++progress, paths.Count);
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}