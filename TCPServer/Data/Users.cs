﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServer.UserData;

namespace TCPServer.Data
{
	internal class Users
	{
		List<ClientSocket> connectedClients = new List<ClientSocket>();

		public User GetUser(int ID)
		{
			return null;
		}
		public User GetUser(string username)
		{
			return GetUser(0);
		}

		public void AddUser(ClientSocket socket)
		{
			if (!connectedClients.Contains(socket))
				connectedClients.Add(socket);
		}
	}
}
