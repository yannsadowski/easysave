using EasySave.lib.Models;
using System.Configuration;
using System.Text.Json;

namespace EasySave.lib.Services
{
    public class ProgressStateService
    {
        public static List<ProgressStateModel> ProgressStates = new List<ProgressStateModel>();

        private static string CurrentName { get; set; } = "";

        private static int CurrentIndex { get; set; }

        public static void ProgressStateFile()
        {
            Serializer();
        }

        public static void AddNewSaveWorkProgressState(ProgressStateModel model)
        {
            ProgressStates.Add(model);

            Serializer();
        }

        public static void RemoveSaveWork(ProgressStateModel model)
        {
            ProgressStates.Remove(model);

            Serializer();
        }

        private static void Serializer()
        {
            string DirectoryPath = ConfigurationManager.AppSettings["ProgressStatePath"];
            string ProgressStatePath = Path.Combine(DirectoryPath, "ProgressState.json");

            Directory.CreateDirectory(DirectoryPath);

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            string JsonProgressState = JsonSerializer.Serialize(ProgressStates, options);

            try
            {
                File.WriteAllText(ProgressStatePath, JsonProgressState + Environment.NewLine);
            }
            catch { }
        }
    }
}