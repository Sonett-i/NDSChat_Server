﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServer.Client;

namespace TCPServer.ServerData
{
	public class ServerUsers
	{
		List<ClientSocket> connectedClients = new List<ClientSocket>();

		public ClientSocket GetUser(string username)
		{
			foreach (ClientSocket client in connectedClients)
			{
				if (client.user.GetName() == username)
				{
					return client;
				}
			}
			return null;
		}

		public void AddUser(ClientSocket socket)
		{
			if (!connectedClients.Contains(socket))
			{
				connectedClients.Add(socket);
			}
		}

		public void RemoveUser(ClientSocket socket)
		{
			if (connectedClients.Contains(socket))
				connectedClients.Remove(socket);
		}

		public List<ClientSocket> GetUsers()
		{
			return connectedClients;
		}

		public bool UserExists(string user)
		{
			foreach (ClientSocket client in connectedClients)
			{
				if (client.user.GetName() == user)
				{
					return true;
				}
			}

			return false;
		}
	}
}
