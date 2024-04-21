using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace TCPServer.Client
{
	public class User
	{
		int id;
		string username = "guest";
		public string remoteAddress = "";
		public UserGroup.SecLevel secLevel;
		public bool isActive = true;

		public User(string username, string IP)
		{
			this.username = username;
			this.remoteAddress = IP;
			SetSecurityLevel();
		}

		public string GetName()
		{
			return username;
		}

		public void SetName(string newName)
		{
			this.username = newName;
		}

		public int GetID()
		{
			return id;
		}

		public void SetID(int id)
		{
			this.id = id;
		}

		private void SetSecurityLevel()
		{
			if (this.username == "")
			{
				this.secLevel = UserGroup.SecLevel.SEC_LVL_GUEST;
			}
			else if (this.username != "")
			{
				this.secLevel = UserGroup.SecLevel.SEC_LVL_GUEST;
			}
			else
			{
				// to-do server sets sec level.
			}
		}

		public string UserInfo()
		{
			string output = $"Username: {username}\nIP: {remoteAddress}\nrank: {UserGroup.rankNames[secLevel]}";

			return output;
		}

	}
}
