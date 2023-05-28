using EasySave.lib.Models;
using System.Net.Sockets;
using System.Text.Json;
using EasySave.lib.Services;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace EasySave.lib.Services.Server
{
    public class ClientManager
    {
        public SaveWorkManager saveWorkManager = SaveWorkManager.GetInstance();
        public event EventHandler? Disconnected;

        public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

        public Socket Socket { get; set; }

        private const int HEADERSIZE = 20; 

        public ClientManager(Socket socket)
        {
            Socket = socket;
        }

        public void Start()
        {
            new Thread(Listen).Start();
            //Send("Test send");
            //saveWorkManager.SaveWorkInitializing();
            foreach (SaveWorkModel item in saveWorkManager.ArrayOfSaveWork)
            {
                Send(item);
            }
            //Send(saveWorkManager.ArrayOfSaveWork[0]);
        }

        public void Send(string message)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
            Socket.Send(buffer);
        }

        public void Send(SaveWorkModel saveWorkModels)
        {

            byte[] serializedData = Serialize(ClassReformater(saveWorkModels));
            serializedData = Encoding.ASCII.GetBytes(serializedData.Length.ToString().PadLeft(HEADERSIZE, '<')).Concat(serializedData).ToArray();

            Socket.Send(serializedData);
        }

        private string[] ClassReformater(SaveWorkModel saveWorkModels)
        {
            string[] ModelAttributs = new string[]
            {
                saveWorkModels.NameSaveWork,
                saveWorkModels.TypeSaveWork.ToString(),
                saveWorkModels.SourcePathSaveWork,
                saveWorkModels.DestinationPathSaveWork,
                saveWorkModels.ProgressState,
                saveWorkModels.Percentage.ToString(),
               
            };
            return ModelAttributs;
        }

        private byte[] Serialize(string[] ModelAttributs)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                formatter.Serialize(memoryStream, ModelAttributs);
                return memoryStream.ToArray();
            }
        }

        private void Listen()
        {
            try
            {
                byte[] buffer = new byte[1024];
                while (true)
                {
                    int length = Socket.Receive(buffer);
                    string message = System.Text.Encoding.UTF8.GetString(buffer, 0, length);
                    Console.WriteLine(message);

                    MessageReceived?.Invoke(this, new MessageReceivedEventArgs() { Message = message });
                }
            }
            catch
            {
                Disconnected?.Invoke(this, EventArgs.Empty);
                Socket.Dispose();
            }
        }
    }

    public class MessageReceivedEventArgs : EventArgs
    {
        public string Message { get; set; } = "";
    }
}