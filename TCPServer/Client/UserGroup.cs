using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer.Client
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

		public static Dictionary<SecLevel, string> rankNames = new Dictionary<SecLevel, string>()
		{
			[SecLevel.SEC_LVL_GUEST] = "Guest",
			[SecLevel.SEC_LVL_MEMBER] = "Member",
			[SecLevel.SEC_LVL_MODERATOR] = "Moderator",
			[SecLevel.SEC_LVL_ADMIN] = "Administrator",
		};
	}
}

/*  Author: Sam Catcheside, A00115110
 *  Date: 21/04/2024
 */