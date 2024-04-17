using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace TCPServer.UserData
{
	public class User
	{
		string username = "null";
		IPAddress address = null;
		UserGroup.SecLevel secLevel;

		public User(string username)
		{
			this.username = username;
			SetSecurityLevel();
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
