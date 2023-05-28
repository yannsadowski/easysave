using EasySave.lib.Models;
using System.Configuration;
using System.Diagnostics;

namespace EasySave.lib.Services
{
    public class SaveWorkService
    {
        public SaveWorkManager saveWorkManager = SaveWorkManager.GetInstance();

        public bool RunningProcess { get; set; } = false;
        private static bool TokenOfAvailability { get; set; } = true;

        private object LockerLog = new object();

        private object LockerProgressState = new object();
        private Server.Server server = new Server.Server();
        
        public void LaunchSaveWork(SaveWorkModel model)
        {
            if (model.TypeSaveWork == 1)
            {
                Thread thread = new Thread(() =>
                {
                    CompleteCopyFiles(model);
                });
                //thread.Name = $"{thread}"+ $"{model.NameSaveWork}";
                thread.Start();
                //CompleteCopyFiles(model);
            }
            else
            {
                Thread thread = new Thread(() =>
                {
                    DifferentialCopyFiles(model);
                });
                //thread.Name = $"{thread}" + $"{model.NameSaveWork}";
                thread.Start();
                //DifferentialCopyFiles(model);
            }
        }

        public void PauseSaveWork(SaveWorkModel model)
        {
            model.PauseEvent.Reset();
        }

        public void ResumeSaveWork(SaveWorkModel model)
        {
            model.PauseEvent.Set();
        }

        public void StopSaveWork(SaveWorkModel model)
        {
            model.PauseEvent.Reset();
            model.StopEvent.Set();
        }

        private int CompleteCopyFiles(SaveWorkModel model)
        {
            CopyModel copyModel = new CopyModel()
            {
                SourcePath = model.SourcePathSaveWork,
                DestinationPath = model.DestinationPathSaveWork,
                TotalFilesToCopy = 0,
                TotalFilesSizeToCopy = 0,
                NbFilesLeft = 0,
                FilesSizeLeft = 0,
                Percentage = 0,
            };

            if (Directory.Exists(copyModel.SourcePath))
            {
                Directory.CreateDirectory(copyModel.DestinationPath);

                foreach (string dirPath in Directory.GetDirectories(copyModel.SourcePath, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dirPath.Replace(copyModel.SourcePath, copyModel.DestinationPath));
                }

                string[] AllFiles = Directory.GetFiles(copyModel.SourcePath, "*.*", SearchOption.AllDirectories);

                copyModel.TotalFilesToCopy = AllFiles.Length;
                copyModel.NbFilesLeft = copyModel.TotalFilesToCopy;

                foreach (string file in AllFiles)
                {
                    copyModel.TotalFilesSizeToCopy += new FileInfo(file).Length;
                }
                copyModel.FilesSizeLeft = copyModel.TotalFilesSizeToCopy;

                string[] SpecificExtensions = ConfigurationManager.AppSettings["FileToPrioritize"].Split(',');
                long FileSizeLimit = long.Parse(ConfigurationManager.AppSettings["FileSizeLimit"]);

                string[] PrioritizedFiles = AllFiles.Where(file => SpecificExtensions.Contains(Path.GetExtension(file))).ToArray();
                string[] OtherFiles = AllFiles.Except(PrioritizedFiles).ToArray();

                // Création des 4 listes
                string[] PrioritizedBigFiles = PrioritizedFiles.Where(file => new FileInfo(file).Length >= FileSizeLimit).ToArray();
                string[] PrioritizedSmallFiles = PrioritizedFiles.Except(PrioritizedBigFiles).ToArray();

                string[] OtherBigFiles = OtherFiles.Where(file => new FileInfo(file).Length >= FileSizeLimit).ToArray();
                string[] OtherSmallFiles = OtherFiles.Except(OtherBigFiles).ToArray();

                // Création des 4 index

                int indexA = 0; // PrioritizedBigFiles
                int indexB = 0; // PrioritizedSmallFiles
                int indexC = 0; // OtherBigFiles
                int indexD = 0; // OtherSmallFiles
                // Création de la boucle infernale

                for (int i = 0; i < AllFiles.Length; i++)
                {
                    if (model.StopEvent.WaitOne(0))
                    {
                        break;
                    }
                    model.PauseEvent.WaitOne();
                    model.PauseEvent.Set();
                    if (indexA != PrioritizedBigFiles.Length || indexB != PrioritizedSmallFiles.Length)
                    {
                        if (TokenOfAvailability && indexA != PrioritizedBigFiles.Length)
                        {
                            TokenOfAvailability = false;
                            CompleteFileCopy(PrioritizedBigFiles[indexA], model, copyModel);
                            indexA++;
                            TokenOfAvailability = true;
                            server.Send(model);
                        }
                        else
                        {
                            if (indexB < PrioritizedSmallFiles.Length)
                            {
                                CompleteFileCopy(PrioritizedSmallFiles[indexB], model, copyModel);
                                indexB++;
                                server.Send(model);
                            }
                        }
                    }
                    else
                    {
                        if (TokenOfAvailability && indexC != OtherBigFiles.Length)
                        {
                            TokenOfAvailability = false;
                            CompleteFileCopy(OtherBigFiles[indexC], model, copyModel);
                            indexC++;
                            TokenOfAvailability = true;
                            server.Send(model);
                        }
                        else
                        {
                            if (indexD < OtherSmallFiles.Length)
                            {
                                CompleteFileCopy(OtherSmallFiles[indexD], model, copyModel);
                                indexD++;
                                server.Send(model);
                            }
                        }
                    }
                    if ((indexA + indexB + indexC + indexD) != i)
                    {
                        i = indexA + indexB + indexC + indexD;
                    }
                }

                //CompleteCopyListOfFiles(PrioritizedFiles, model, copyModel);
                //CompleteCopyListOfFiles(OtherFiles, model, copyModel);
            }
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

            model.Percentage = 0;
            model.ProgressState = "Inactive";
            server.Send(model);
            ProgressStateService.ProgressStateFile();

            return 0;
        }

        private int DifferentialCopyFiles(SaveWorkModel model)
        {
            CopyModel copyModel = new CopyModel()
            {
                SourcePath = model.SourcePathSaveWork,
                DestinationPath = model.DestinationPathSaveWork,
                TotalFilesToCopy = 0,
                TotalFilesSizeToCopy = 0,
                NbFilesLeft = 0,
                FilesSizeLeft = 0,
                Percentage = 0,
            };

            if (Directory.Exists(copyModel.SourcePath))
            {
                Directory.CreateDirectory(copyModel.DestinationPath);

                foreach (string dirPath in Directory.GetDirectories(copyModel.SourcePath, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dirPath.Replace(copyModel.SourcePath, copyModel.DestinationPath));
                }

                string[] AllFiles = Directory.GetFiles(copyModel.SourcePath, "*.*", SearchOption.AllDirectories);
                string[] DestFiles = Directory.GetFiles(copyModel.DestinationPath, "*", SearchOption.AllDirectories);

                copyModel.TotalFilesToCopy = 1;
                

                foreach (string file in AllFiles)
                {
                    if(File.GetLastWriteTime(file) > File.GetLastWriteTime(file.Replace(copyModel.SourcePath, copyModel.DestinationPath)))
                    {
                        copyModel.TotalFilesToCopy++;
                        copyModel.TotalFilesSizeToCopy += new FileInfo(file).Length;
                    }
                }
                copyModel.NbFilesLeft = copyModel.TotalFilesToCopy;
                copyModel.FilesSizeLeft = copyModel.TotalFilesSizeToCopy;

                string[] SpecificExtensions = ConfigurationManager.AppSettings["FileToPrioritize"].Split(',');
                long FileSizeLimit = long.Parse(ConfigurationManager.AppSettings["FileSizeLimit"]);

                string[] PrioritizedFiles = AllFiles.Where(file => SpecificExtensions.Contains(Path.GetExtension(file))).ToArray();
                string[] OtherFiles = AllFiles.Except(PrioritizedFiles).ToArray();

                // Création des 4 listes
                string[] PrioritizedBigFiles = PrioritizedFiles.Where(file => new FileInfo(file).Length >= FileSizeLimit).ToArray();
                string[] PrioritizedSmallFiles = PrioritizedFiles.Except(PrioritizedBigFiles).ToArray();

                string[] OtherBigFiles = OtherFiles.Where(file => new FileInfo(file).Length >= FileSizeLimit).ToArray();
                string[] OtherSmallFiles = OtherFiles.Except(OtherBigFiles).ToArray();

                int indexA = 0; // PrioritizedBigFiles
                int indexB = 0; // PrioritizedSmallFiles
                int indexC = 0; // OtherBigFiles
                int indexD = 0; // OtherSmallFiles
                // Création de la boucle infernale

                for (int i = 0; i < AllFiles.Length; i++)
                {
                    if (model.StopEvent.WaitOne(0))
                    {
                        break;
                    }
                    model.PauseEvent.WaitOne();
                    model.PauseEvent.Set();
                    if (indexA != PrioritizedBigFiles.Length || indexB != PrioritizedSmallFiles.Length)
                    {
                        if (TokenOfAvailability && indexA != PrioritizedBigFiles.Length)
                        {
                            TokenOfAvailability = false;
                            DifferentialFileCopy(PrioritizedBigFiles[indexA], model, copyModel);
                            indexA++;
                            TokenOfAvailability = true;
                            server.Send(model);
                        }
                        else
                        {
                            if (indexB < PrioritizedSmallFiles.Length)
                            {
                                DifferentialFileCopy(PrioritizedSmallFiles[indexB], model, copyModel);
                                indexB++;
                                server.Send(model);
                            }
                        }
                    }
                    else
                    {
                        if (TokenOfAvailability && indexC != OtherBigFiles.Length)
                        {
                            TokenOfAvailability = false;
                            DifferentialFileCopy(OtherBigFiles[indexC], model, copyModel);
                            indexC++;
                            TokenOfAvailability = true;
                            server.Send(model);
                        }
                        else
                        {
                            if (indexD < OtherSmallFiles.Length)
                            {
                                DifferentialFileCopy(OtherSmallFiles[indexD], model, copyModel);
                                indexD++;
                                server.Send(model);
                            }
                        }
                    }
                    if ((indexA + indexB + indexC + indexD) != i)
                    {
                        i = indexA + indexB + indexC + indexD;
                    }
                }
                
            }

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

            model.Percentage = 0;
            model.ProgressState = "Inactive";
            server.Send(model);
            ProgressStateService.ProgressStateFile();
            return 0;
        }

        private void CompleteFileCopy(string file, SaveWorkModel model, CopyModel copyModel)
        {
            while (saveWorkManager.CheckRunningProcess(ConfigurationManager.AppSettings["RunningProcess"]) == true)
            {
                Debug.WriteLine("Logiciel Métier Actif");
                RunningProcess = true;
            }
            RunningProcess = false;

            int timeForCryp = 0;

            string[] ExtensionToCrypt = ConfigurationManager.AppSettings["fileToCryp"].Split(',');

            Stopwatch stopwatch = Stopwatch.StartNew();

            if (ExtensionToCrypt.Contains(Path.GetExtension(file)))
            {
                timeForCryp = cryptoSoft.cryptoSoftEasySave("-c", $"{file}", $"{file.Replace(copyModel.SourcePath, copyModel.DestinationPath)}");
            }
            else
            {
                File.Copy(file, file.Replace(copyModel.SourcePath, copyModel.DestinationPath), true);
            }

            stopwatch.Stop();
            double FileTransferTime = stopwatch.Elapsed.TotalSeconds;

            //pourcent = (((copyModel.TotalFilesToCopy - copyModel.NbFilesLeft) / copyModel.TotalFilesToCopy));

            copyModel.Percentage = (((float)copyModel.TotalFilesToCopy - (float)copyModel.NbFilesLeft) / (float)copyModel.TotalFilesToCopy) * 100;
            copyModel.NbFilesLeft--;
            copyModel.FilesSizeLeft -= new FileInfo(file).Length;

            LogModel logModel = new LogModel()
            {
                Name = Path.GetFileName(file),
                FileSource = file,
                FileTarget = Path.Combine(copyModel.DestinationPath, Path.GetFileName(file)),
                DestPath = copyModel.DestinationPath,
                TimeToCrypt = timeForCryp,
                FileSize = new FileInfo(file).Length,
                FileTransferTime = FileTransferTime,
                Time = DateTime.Now,
            };

            model.ProgressStateModel.Name = model.NameSaveWork;
            model.ProgressStateModel.Time = DateTime.Now;
            model.ProgressStateModel.ProgressState = "Active";
            model.ProgressStateModel.TotalFilesToCopy = copyModel.TotalFilesToCopy;
            model.ProgressStateModel.TotalFilesSizeToCopy = copyModel.TotalFilesSizeToCopy;
            model.ProgressStateModel.NbFilesLeft = copyModel.NbFilesLeft;
            model.ProgressStateModel.FilesSizeLeft = copyModel.FilesSizeLeft;
            model.ProgressStateModel.Percentage = copyModel.Percentage;
            model.ProgressStateModel.FilePath = file;
            model.ProgressStateModel.FileDestinationPath = Path.Combine(copyModel.DestinationPath, Path.GetFileName(file));

            model.Percentage = copyModel.Percentage;
            model.ProgressState = "Active";
            lock (LockerLog)
            {
                LogService.LogFiles(logModel);
            }
            lock (LockerProgressState)
            {
                ProgressStateService.ProgressStateFile();
            }
        }


        private void DifferentialFileCopy(string file, SaveWorkModel model, CopyModel copyModel)
        {
            model.PauseEvent.WaitOne();
            model.PauseEvent.Set();
            if (File.GetLastWriteTime(file) > File.GetLastWriteTime(file.Replace(copyModel.SourcePath, copyModel.DestinationPath)))
            {
                int timeForCryp = 0;

                string[] ExtensionToCrypt = ConfigurationManager.AppSettings["fileToCryp"].Split(',');

                Stopwatch stopwatch = Stopwatch.StartNew();

                if (ExtensionToCrypt.Contains(Path.GetExtension(file)))
                {
                    timeForCryp = cryptoSoft.cryptoSoftEasySave("-c", $"{file}", $"{file.Replace(copyModel.SourcePath, copyModel.DestinationPath)}");
                }
                else
                {
                    File.Copy(file, file.Replace(copyModel.SourcePath, copyModel.DestinationPath), true);
                }

                stopwatch.Stop();
                double FileTransferTime = stopwatch.Elapsed.TotalSeconds;
                copyModel.Percentage = (((float)copyModel.TotalFilesToCopy - (float)copyModel.NbFilesLeft) / (float)copyModel.TotalFilesToCopy) * 100;
                copyModel.NbFilesLeft--;
                copyModel.FilesSizeLeft -= new FileInfo(file).Length;

                LogModel logModel = new LogModel()
                {
                    Name = Path.GetFileName(file),
                    FileSource = file,
                    FileTarget = Path.Combine(copyModel.DestinationPath, Path.GetFileName(file)),
                    DestPath = copyModel.DestinationPath,
                    TimeToCrypt = timeForCryp,
                    FileSize = new FileInfo(file).Length,
                    FileTransferTime = FileTransferTime,
                    Time = DateTime.Now,
                };

                model.ProgressStateModel.Name = model.NameSaveWork;
                model.ProgressStateModel.Time = DateTime.Now;
                model.ProgressStateModel.ProgressState = "Active";
                model.ProgressStateModel.TotalFilesToCopy = copyModel.TotalFilesToCopy;
                model.ProgressStateModel.TotalFilesSizeToCopy = copyModel.TotalFilesSizeToCopy;
                model.ProgressStateModel.NbFilesLeft = copyModel.NbFilesLeft;
                model.ProgressStateModel.FilesSizeLeft = copyModel.FilesSizeLeft;
                model.ProgressStateModel.Percentage = copyModel.Percentage;
                model.ProgressStateModel.FilePath = file;
                model.ProgressStateModel.FileDestinationPath = Path.Combine(copyModel.DestinationPath, Path.GetFileName(file));

                model.Percentage = copyModel.Percentage;
                model.ProgressState = "Active";

                lock (LockerLog)
                {
                    LogService.LogFiles(logModel);
                }
                lock (LockerProgressState)
                {
                    ProgressStateService.ProgressStateFile();
                }
            }
        }
    }
}