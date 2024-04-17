using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServer.Client;

namespace TCPServer.ServerData
{
	internal class ServerUsers
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
