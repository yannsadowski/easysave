using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.lib.Services.Server
{
    public class Listener
    {
        public event EventHandler<SocketEventArgs>? ClientConnected;

        public int Port { get; set; }

        public Listener(int port)
        {
            Port = port;
        }

        public void Start()
        {
            Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, Port));
            socket.Listen(16);

            // Boucle pour récupérer tous les clients
            new Thread(_ =>
            {
                while (true)
                {
                    Socket client = socket.Accept();
                    ClientConnected?.Invoke(this, new SocketEventArgs() { Socket = client });
                }

                socket.Dispose();
            }).Start();
        }
    }

    public class SocketEventArgs : EventArgs
    {
        public Socket? Socket { get; set; }
    }
}
