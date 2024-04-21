using System;
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

		// To-do for ass3: make mod and admin check happen with database transaction.

		List<string> admins = new List<string>() { "Sam" };
		List<string> moderators = new List<string>() { "Lisa" };

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

		public void UpdateUser(ClientSocket socket)
		{
			if (admins.Contains(socket.user.GetName()))
			{
				socket.user.secLevel = UserGroup.SecLevel.SEC_LVL_ADMIN;
			}

			if (moderators.Contains(socket.user.GetName()))
			{
				socket.user.secLevel = UserGroup.SecLevel.SEC_LVL_MODERATOR;
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

		public bool Promote(ClientSocket client)
		{
			if ((int)(client.user.secLevel + 1) < Enum.GetNames(typeof(UserGroup.SecLevel)).Length)
			{
				client.user.secLevel++;
				return true;

			}
			return false;
		}

		public bool Demote(ClientSocket client)
		{
			if ((int)(client.user.secLevel - 1) >= 0)
			{
				client.user.secLevel--;
				return true;

			}
			return false;
		}

		public bool KickUser(ClientSocket sender, ClientSocket target)
		{
			if (sender.user.secLevel >= UserGroup.SecLevel.SEC_LVL_MODERATOR)
			{

				return true;
			}

			return false;
		}

		public string GetMods()
		{
			string output = "";

			foreach (ClientSocket client in connectedClients)
			{
				if (client.user.secLevel >= UserGroup.SecLevel.SEC_LVL_MODERATOR)
				{
					output += $"\n{client.user.GetName()} [{UserGroup.rankNames[client.user.secLevel]}]";
				}
			}

			return output;
		}
	}
}

/*  Author: Sam Catcheside, A00115110
 *  Date: 21/04/2024
 */