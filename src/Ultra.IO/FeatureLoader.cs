using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
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
                .Select(Path.GetFullPath)
                .ToList();
        }

        public static List<ParkingSlot> LoadSlots(string path)
        {
            var jsonPath = Path.ChangeExtension(path, ".json");
            return File.Exists(jsonPath)
                ? JsonConvert.DeserializeObject<List<ParkingSlot>>(File.ReadAllText(jsonPath))
                : new List<ParkingSlot>();
        }

        public static void SaveSlots(string path, IEnumerable<ParkingSlot> slots)
        {
            var jsonPath = Path.ChangeExtension(path, ".json");
            var json = JsonConvert.SerializeObject(slots);
            File.WriteAllText(jsonPath, json);
        }

        public static List<ImageFeatures> GetObservations(bool reloadChache = false,Action<int, int> reportProgres = null)
        {
            if (!reloadChache && LoadCacheObservations(out var observations))
            {
                return observations;
            }
            observations = new List<ImageFeatures>();
            var paths = GetPhotos();
            var progress = 0;
            reportProgres?.Invoke(progress, paths.Count);
            foreach (var path in paths)
            {
                using (var image = new Mat(path))
                {
                    foreach (var slot in LoadSlots(path))
                    {
                        var calculateFeatures = image.CalculateFeatures(slot.Contour, slot.IsOccupied);
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        observations.Add(calculateFeatures);
                    }
                    reportProgres?.Invoke(++progress, paths.Count);
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            SaveCacheObservations(observations);
            return observations;
        }

        public static bool LoadCacheObservations(out List<ImageFeatures> observations)
        {
            string directoryPath = @"..\..\..\..\DataSet\PhoneCamera";
            var csvPath = Path.Combine(directoryPath, "cachedFeatures.csv");
            if (!File.Exists(csvPath))
            {
                observations = null;
                return false;
            }
            try
            {
                using (var csv = new CsvReader(new StreamReader(csvPath)))
                {
                    csv.Configuration.Delimiter = ";";
                    csv.Configuration.HasHeaderRecord = true;
                    csv.Configuration.WillThrowOnMissingField = true;

                    observations = csv.GetRecords<ImageFeatures>().ToList();
                    return true;
                }
            }
            catch (CsvHelperException)
            {
                observations = null;
                return false;
            }
        }

        public static void SaveCacheObservations(IEnumerable<ImageFeatures> observations)
        {
            string directoryPath = @"..\..\..\..\DataSet\PhoneCamera";
            var csvPath = Path.Combine(directoryPath, "cachedFeatures.csv");
            using (var csv = new CsvWriter(new StreamWriter(csvPath, append: false)))
            {
                csv.Configuration.Delimiter = ";";
                csv.Configuration.HasHeaderRecord = true;

                csv.WriteRecords(observations);
            }
        }
    }
}