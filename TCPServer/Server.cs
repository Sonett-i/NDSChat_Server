using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Libs.Terminal;
using TCPServer.structs;
using TCPServer.Logging;

namespace TCPServer
{
	public class Server
    {
        public int port;
        private bool running = true;
        private Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private List<ClientSocket> clientSockets = new List<ClientSocket>();

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

            Log.Event($"Listening...");
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
            ClientSocket newClientSocket = new ClientSocket { socket = joiningSocket };
            clientSockets.Add(newClientSocket);
            joiningSocket.BeginReceive(newClientSocket.buffer, 0, ClientSocket.BUFFER_SIZE, SocketFlags.None, ReceiveCallback, newClientSocket);
            Log.Event("Client connected");
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            ClientSocket currentClientSocket = (ClientSocket)AR.AsyncState;
            int received = ClientSocket.BUFFER_SIZE;

            try
            {
                received = currentClientSocket.socket.EndReceive(AR);
            }
            catch (SocketException)
            {
                Log.Event("Client disconnected");
                currentClientSocket.socket.Close();
                clientSockets.Remove(currentClientSocket);
                return;
            }

            byte[] buffer = new byte[received];
            Array.Copy(currentClientSocket.buffer, buffer, received);

            string text = ClientSocket.encoding.GetString(buffer);

            Terminal.Print(text);
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
