using EasySave.lib.Models;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Text.Json;
using EasySave.lib.Services.Server;
using System.ComponentModel;

namespace EasySave.lib.Services
{
    public class SaveWorkManager
    {
        public static int index;
        public List<SaveWorkModel> ArrayOfSaveWork { get; set; } = new List<SaveWorkModel>();
        private Server.Server server = new Server.Server();
        private static SaveWorkManager instance = new();

        public static SaveWorkManager GetInstance()
        {
            return instance;
        }

        public void AddNewSaveWork(SaveWorkModel model)
        {
            ArrayOfSaveWork.Add(model);

            model.ProgressStateModel.Name = model.NameSaveWork;
            model.ProgressStateModel.Time = DateTime.Now;
            model.ProgressStateModel.ProgressState = "Inactive";
            model.ProgressStateModel.TotalFilesToCopy = 0;
            model.ProgressStateModel.TotalFilesSizeToCopy = 0;
            model.ProgressStateModel.NbFilesLeft = 0;
            model.ProgressStateModel.FilesSizeLeft = 0;
            model.ProgressStateModel.Percentage = 0;
            model.ProgressStateModel.FilePath = "";
            model.ProgressStateModel.FileDestinationPath = "";

            ProgressStateService.AddNewSaveWorkProgressState(model.ProgressStateModel);

            string DirectoryPath = ConfigurationManager.AppSettings["SaveWorkPath"];
            string path = Path.Combine(DirectoryPath, $"{model.NameSaveWork}.json");

            Directory.CreateDirectory(DirectoryPath);

            if (!File.Exists(path))
            {
                string jsonString = JsonSerializer.Serialize(model);
                File.WriteAllText(path, jsonString);
            }
            server.Send(model);
        }

        public void UpdateSaveWork(SaveWorkModel model, SaveWorkModel oldModel)
        {
            RemoveSaveWorkWPF(oldModel);
            AddNewSaveWork(model);
        }

        public void SaveWorkInitializing()
        {
            string SaveWorkPath = ConfigurationManager.AppSettings["SaveWorkPath"];

            Directory.CreateDirectory(SaveWorkPath);

            string[] Files = Directory.GetFiles(SaveWorkPath, "*.json");
            int FileCount = Files.Length;

            for (int i = 0; i < FileCount; i++)
            {
                string Json = File.ReadAllText(Path.Combine(SaveWorkPath, Files[i]));
                SaveWorkModel saveWorkJSON = JsonSerializer.Deserialize<SaveWorkModel>(Json)!;

                SaveWorkModel model = new SaveWorkModel()
                {
                    NameSaveWork = saveWorkJSON.NameSaveWork,
                    TypeSaveWork = saveWorkJSON.TypeSaveWork,
                    SourcePathSaveWork = saveWorkJSON.SourcePathSaveWork,
                    DestinationPathSaveWork = saveWorkJSON.DestinationPathSaveWork
                }; // crée savework à partir de attributs

                AddNewSaveWork(model);
            }
        }

        public void RemoveSaveWorkWPF(SaveWorkModel model)
        {
            string path = Path.Combine(ConfigurationManager.AppSettings["SaveWorkPath"], $"{model.NameSaveWork}.json");
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);

                    string NameOfSaveWork = model.NameSaveWork;
                    for (int i = 0; i < ArrayOfSaveWork.Count; i++)
                    {
                        if (ArrayOfSaveWork[i].NameSaveWork == model.NameSaveWork)
                        {
                            index = i;
                        }
                    }
                    ArrayOfSaveWork.RemoveAt(index);
                }


            }
            catch (Exception ex)
            {

            }
        }

        public void ExecuteSaveWorkWPF(SaveWorkModel model)
        {
            SaveWorkService service = new SaveWorkService();
            service.LaunchSaveWork(model);
        }

        private char[] IllegalChars = Path.GetInvalidPathChars();

        public int TestNameSaveWork(string SaveWorkName)
        {
            if (SaveWorkName != "")
            {
                foreach (char c in IllegalChars)
                {
                    if (SaveWorkName.Contains(c))
                        return 1;
                }
                foreach (var index in ArrayOfSaveWork)
                {
                    if (index.NameSaveWork == SaveWorkName)
                    {
                        return 1;
                    }
                }
                return 0;
            }
            else
            {
                return 1;
            }
        }

        public int TestTypeSaveWork(string SaveWorkTypeToConvert)
        {
            int SaveWorkType;

            if (int.TryParse(SaveWorkTypeToConvert, out SaveWorkType))
            {
                if (SaveWorkType > 0 && SaveWorkType <= 2)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return 1;
            }
        }

        public int TestSourcePathSaveWork(string SaveWorkSourcePath)
        {
            if (Directory.Exists(SaveWorkSourcePath))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        public int TestDestinationPathSaveWork(string SaveWorkDestinationPath)
        {
            if (SaveWorkDestinationPath != "")
            {
                foreach (char c in IllegalChars)
                {
                    if (SaveWorkDestinationPath.Contains(c))
                        return 1;
                }
                return 0;
            }
            else
                return 1;
        }

        public bool CheckRunningProcess(string ProcessName)
        {
            bool ProcessRunning = false;
            var process = Process.GetProcessesByName(ProcessName);
            if (process.Length > 0)
            {
                ProcessRunning = true;
            }
            return ProcessRunning;
        }
    }
}