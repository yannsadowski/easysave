using EasySave.lib.Models;
using System.Configuration;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

namespace EasySave.lib.Services
{
    public static class LogService
    {
        public static void LogFiles(LogModel model)
        {
            if (ConfigurationManager.AppSettings["typeLogXML"] == "false")
            {
                string DirectoryPath = ConfigurationManager.AppSettings["LogPath"];
                string LogPath = Path.Combine(DirectoryPath, $"{DateTime.Now.ToString("dd_MM_yyyy")}_log.json");


                Directory.CreateDirectory(DirectoryPath);

                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string JsonLog = JsonSerializer.Serialize(model, options);

                try
                {
                    File.AppendAllText(LogPath, JsonLog + Environment.NewLine);
                }
                catch { }
            }
            else
            {
                string DirectoryPath = ConfigurationManager.AppSettings["LogPath"];
                string LogPath = Path.Combine(DirectoryPath, $"{DateTime.Now.ToString("dd_MM_yyyy")}_log.xml");

                Directory.CreateDirectory(DirectoryPath);

                XmlSerializer serializer = new XmlSerializer(typeof(LogModel));

                // Ouvrez un flux d'écriture vers le fichier XML
                using (FileStream stream = new FileStream(LogPath, FileMode.Append))
                {
                    // Sérialisez l'objet en XML et écrivez-le dans le flux
                    serializer.Serialize(stream, model);
                }
            }
        }
    }
}