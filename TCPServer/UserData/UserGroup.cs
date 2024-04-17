using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer.UserData
{
	public static class UserGroup
	{
		public enum SecLevel
		{
			SEC_LVL_GUEST,
			SEC_LVL_MEMBER,
			SEC_LVL_MODERATOR,
			SEC_LVL_ADMIN
		}
	}
}
