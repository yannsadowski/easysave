using System.Configuration;
using System.Diagnostics;

namespace EasySave.lib.Services
{
    public static class cryptoSoft
    {
        public static int cryptoSoftEasySave(string type, string fichier_source, string fichier_destination)
        {
            string filePath = ConfigurationManager.AppSettings["CryptKeyPath"]; // spécifiez le chemin d'accès complet à votre fichier key.txt
            string key = File.ReadAllText(filePath);
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = $@"{ConfigurationManager.AppSettings["pathToCryptoSoft"]}\CryptoSoftEasySave.exe",
                Arguments = $"{type} \"{fichier_source}\" \"{fichier_destination}\" {key}",
            };

            Process process = new Process();
            info.CreateNoWindow = true;
            process.StartInfo = info;
            process.Start();
            process.WaitForExit();
            var test = process.ExitCode;

            return (process.ExitCode);
        }
    }
}