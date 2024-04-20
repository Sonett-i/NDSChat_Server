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
		IPAddress address = null;
		UserGroup.SecLevel secLevel;

		public User(string username)
		{
			this.username = username;
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

	}
}
