using EasySave.lib.Models;
using System.Diagnostics;

namespace EasySave.lib.Services.Server
{
    public class Server
    {
        private static readonly List<ClientManager> clients = new(16);

        public void Start()
        {
            Listener listener = new(55263);
            listener.ClientConnected += Listener_ClientConnected;
            listener.Start();
        }

        public void Send(SaveWorkModel model)
        {
            foreach (ClientManager client in clients) {
                client.Send(model);

            }
        }

        private void Listener_ClientConnected(object? sender, SocketEventArgs e)
        {
            if (e.Socket == null)
                return;

            ClientManager client = new(e.Socket);
            client.MessageReceived += Client_MessageReceived;
            client.Disconnected += Client_Disconnected;
            clients.Add(client);

            client.Start();
        }

        private void Client_Disconnected(object? sender, EventArgs e)
        {
            if (!(sender is ClientManager client))
                return;

            clients.Remove(client);
            client.Disconnected -= Client_Disconnected;
            client.MessageReceived -= Client_MessageReceived;
            client.Socket.Dispose();

            Debug.WriteLine("Client décédé");
        }

        private void Client_MessageReceived(object? sender, MessageReceivedEventArgs e)
        {
            foreach (var client in clients)
                if (sender != client)
                    client.Send(e.Message);
        }
    }
}