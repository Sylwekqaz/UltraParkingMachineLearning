using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        static readonly ReadOnlyCollection<string> Extensions = Array.AsReadOnly(new[] { ".png", ".jpg", ".jpeg", ".bmp" });

        private static string GetJsonLocation(string path) => Path.ChangeExtension(path, ".json");
        private static string GetCsvPath(string directoryPath) => Path.Combine(directoryPath, "cachedFeatures.csv");

        public static List<string> GetPhotos(string directoryPath)
        {
            return Directory.EnumerateFiles(directoryPath)
                .Where(path => Extensions
                    .Any(ext => ext.Equals(Path.GetExtension(path), StringComparison.InvariantCultureIgnoreCase)))
                .Select(Path.GetFullPath)
                .ToList();
        }

        public static List<ParkingSlot> LoadSlots(string path)
        {
            return File.Exists(GetJsonLocation(path))
                ? JsonConvert.DeserializeObject<List<ParkingSlot>>(File.ReadAllText(GetJsonLocation(path)))
                : new List<ParkingSlot>();
        }

        public static void SaveSlots(string path, IEnumerable<ParkingSlot> slots)
        {
            var json = JsonConvert.SerializeObject(slots);
            File.WriteAllText(GetJsonLocation(path), json);
        }

        public static List<ImageFeatures> GetObservations(string directoryPath,bool reloadChache = false,Action<int, int> reportProgres = null)
        {
            if (!reloadChache && LoadCacheObservations(directoryPath,out var observations))
            {
                return observations;
            }
            observations = new List<ImageFeatures>();
            var paths = GetPhotos(directoryPath);
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
            SaveCacheObservations(directoryPath,observations);
            return observations;
        }

        public static bool LoadCacheObservations(string directoryPath,out List<ImageFeatures> observations)
        {
            var csvPath = GetCsvPath(directoryPath);
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

        public static void SaveCacheObservations(string directoryPath,IEnumerable<ImageFeatures> observations)
        {
            using (var csv = new CsvWriter(new StreamWriter(GetCsvPath(directoryPath), append: false)))
            {
                csv.Configuration.Delimiter = ";";
                csv.Configuration.HasHeaderRecord = true;

                csv.WriteRecords(observations);
            }
        }
    }
}