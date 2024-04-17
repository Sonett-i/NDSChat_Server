using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Libs.Terminal;
using TCPServer.Logging;
using TCPServer.Data;
using TCPServer.Client;
using TCPServer.ServerData;

namespace TCPServer
{
	public class Server
    {
        public int port;
        private bool running = true;
        private Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private List<ClientSocket> clientSockets = new List<ClientSocket>();

        ServerUsers connectedClients = new ServerUsers();

        public Server(int port)
        {
            this.port = port;
        }

        public static Server Instance(int port)
        {
            return ServerConfig.ValidPort(port) ? new Server(port) : null;
        }

        public async Task SetupAsync(CancellationToken cancellationToken)
        {
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            serverSocket.Listen(0);

            Log.Event($"Server bound to: {IPAddress.Any}:{port}...");
            Log.Event($"Listening...");
            while (!cancellationToken.IsCancellationRequested)
            {
                Socket joiningSocket = await AcceptAsync(cancellationToken);
                HandleNewClient(joiningSocket);
            }
        }

        private Task<Socket> AcceptAsync(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<Socket>();

            cancellationToken.Register(() =>
            {
                tcs.TrySetCanceled();
            });

            serverSocket.BeginAccept(asyncResult =>
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    tcs.TrySetCanceled();
                    return;
                }

                try
                {
                    Socket socket = serverSocket.EndAccept(asyncResult);
                    tcs.TrySetResult(socket);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }, null);

            return tcs.Task;
        }

        private void HandleNewClient(Socket joiningSocket)
        {
            User newUser = new User("");
            ClientSocket newClientSocket = new ClientSocket { socket = joiningSocket, user = newUser };
            
            connectedClients.AddUser(newClientSocket);

            newClientSocket.socket.BeginReceive(newClientSocket.buffer, 0, ClientSocket.BUFFER_SIZE, SocketFlags.None, ReceiveCallback, newClientSocket);
            Log.Event($"{newClientSocket.socket.RemoteEndPoint.ToString()} connected.");
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            ClientSocket? currentClientSocket = (ClientSocket)AR.AsyncState;
            int received = ClientSocket.BUFFER_SIZE;

            try
            {
                received = currentClientSocket.socket.EndReceive(AR);
            }
            catch (SocketException)
            {
                Log.Event($"{currentClientSocket.socket.RemoteEndPoint.ToString()} disconnected");
                currentClientSocket.socket.Close();
                clientSockets.Remove(currentClientSocket);
                return;
            }

            byte[] buffer = new byte[received];
            Array.Copy(currentClientSocket.buffer, buffer, received);

            string text = ClientSocket.encoding.GetString(buffer);

            Terminal.Print(text);

            currentClientSocket.socket.BeginReceive(currentClientSocket.buffer, 0, ClientSocket.BUFFER_SIZE, SocketFlags.None, ReceiveCallback, currentClientSocket);

            SendToAll(text, currentClientSocket);
        }

        public void SendToAll(string message, ClientSocket sender)
        {
            foreach (ClientSocket client in clientSockets)
            {
                if (client == sender)
                    continue;

                byte[] data = Encoding.UTF8.GetBytes(message);
                client.socket.Send(data);
            }
        }


        public void Shutdown()
		{
            Terminal.Print("Server shutting down...");
            running = false;
		}
    }
}
