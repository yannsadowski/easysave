using EasySave.DistentClient.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace EasySave.DistentClient.Services
{
    internal class Listener
    {
        public int Port { get; set; }
        private const int HEADERSIZE = 20;
        public EventHandler<msgEventArgs> msgRecevie;
        public Object Locker;

        public Listener(int port)
        {
            Port = port;
        }

        public void Start()
        {
            Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), Port));
            Debug.WriteLine("Chat connecté au serveur");

            new Thread(Listen).Start(socket);
        }

        public void Listen(object? obj)
        {
            Socket? socket = obj as Socket;
            if (socket == null)
                return;

            byte[] headerBuffer = new byte[HEADERSIZE];

            while (true)
            {
                int bytesRead = socket.Receive(headerBuffer, 0, HEADERSIZE, SocketFlags.None);

                int dataSize = Convert.ToInt32(Encoding.ASCII.GetString(headerBuffer).Trim('<'));

                byte[] dataBuffer = new byte[dataSize];
                bytesRead = socket.Receive(dataBuffer, 0, dataSize, SocketFlags.None);

                if (bytesRead != dataSize)
                {
                    throw new Exception("La taille des données n'a pas été lue correctement");
                }
                string[] ModelAttributs;

                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    memoryStream.Write(dataBuffer, 0, dataBuffer.Length);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    ModelAttributs = formatter.Deserialize(memoryStream) as string[];
                }

                msgRecevie?.Invoke(this, new msgEventArgs() { msg = ModelAttributs });
            }
        }

        //private SaveWorkModel Deserialize(byte[] serializedData)
        //{
        //    BinaryFormatter formatter = new BinaryFormatter();
        //    using (MemoryStream memoryStream = new MemoryStream(serializedData))
        //    {
        //        object deserializedObject = formatter.Deserialize(memoryStream);

        //        if (deserializedObject is SaveWorkModel)
        //        {
        //            return (SaveWorkModel)deserializedObject;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //}

        public class msgEventArgs : EventArgs
        {
            public string[]? msg { get; set; }
        }
    }
}