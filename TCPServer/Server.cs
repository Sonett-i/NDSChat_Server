﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Libs.Terminal;
using TCPServer.Logging;
using TCPServer.Client;
using TCPServer.ServerData;
using TCPServer.Messaging;

namespace TCPServer
{
	public class Server
    {
        public int port;
        private bool running = true;
        private Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // Want this accessible by other sections of code.
        //public static List<ClientSocket> clientSockets = new List<ClientSocket>();

        public static ServerUsers connectedClients = new ServerUsers();

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

            Log.Event($"Server bound to: {IPAddress.Any}:{port}...", Log.LogType.LOG_EVENT);
            Log.Event($"Listening...", Log.LogType.LOG_EVENT);

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
            User newUser = new User("guest", joiningSocket.RemoteEndPoint.ToString());
            ClientSocket newClientSocket = new ClientSocket { socket = joiningSocket, user = newUser };
            
            connectedClients.AddUser(newClientSocket);

            newClientSocket.socket.BeginReceive(newClientSocket.buffer, 0, ClientSocket.BUFFER_SIZE, SocketFlags.None, ReceiveCallback, newClientSocket);
            Log.Event($"{newClientSocket.user.remoteAddress} connected.", Log.LogType.LOG_EVENT);
        }

        // Async Callback
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
                Log.Event($"{currentClientSocket.user.remoteAddress} disconnected", Log.LogType.LOG_EVENT);
                currentClientSocket.socket.Close();
                connectedClients.RemoveUser(currentClientSocket);
                return;
            }

            byte[] buffer = new byte[received];
            Array.Copy(currentClientSocket.buffer, buffer, received);

            Message message = Message.Receive(buffer, currentClientSocket);

            if (message != null)
            {
                if (message.messageType == Message.MessageType.MESSAGE_TYPE_DEFAULT || message.messageType == Message.MessageType.MESSAGE_TYPE_ANNOUNCEMENT)
                {
                    Log.Event(message.Format(), Log.LogType.LOG_MESSAGE);
                    SendToAll(message, currentClientSocket);
                }
                else
                {
                    message.Send();
                }
            }

            currentClientSocket.socket.BeginReceive(currentClientSocket.buffer, 0, ClientSocket.BUFFER_SIZE, SocketFlags.None, ReceiveCallback, currentClientSocket);
        }

        // Send to all
        public void SendToAll(Message message, ClientSocket sender)
        {
            if (sender.user.isActive)
			{
                foreach (ClientSocket client in connectedClients.GetUsers())
                {
                    if (client == sender && message.messageType != Message.MessageType.MESSAGE_TYPE_ANNOUNCEMENT)
                        continue;

                    message.Send(client);
                }
            }
        }

        // Shutdown procedure
        public void Shutdown()
		{
            Terminal.Print("Server shutting down...");
            running = false;
		}
    }
}

/*  Author: Sam Catcheside, A00115110
 *  Date: 21/04/2024
 */