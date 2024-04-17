using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer.Messaging
{
	public static class Commands
	{
		public enum Command
		{
			COMMAND_USER,
			COMMAND_WHO,
			COMMAND_ABOUT,
			COMMAND_WHISPER,
			COMMAND_SET_SECLVL
		}

		public static Dictionary<Command, Action> commands = new Dictionary<Command, Action>()
		{
			[Command.COMMAND_USER] = Console.WriteLine,
			[Command.COMMAND_WHO] = Console.WriteLine,
			[Command.COMMAND_ABOUT] = Console.WriteLine,
			[Command.COMMAND_WHISPER] = Console.WriteLine,
			[Command.COMMAND_SET_SECLVL] = Console.WriteLine,
		};
	}
}
