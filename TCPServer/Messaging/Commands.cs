using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServer.Logging;
using TCPServer.Client;
using TCPServer.Messaging;

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

		public static string CommandFromString(string input)
		{
			return input.Replace("!", "").Replace("\0", "");
		}
		public static Message HandleCommand(Message message)
		{
			string commandString = CommandFromString(message.content);
			message.messageType = Message.MessageType.MESSAGE_TYPE_COMMAND;

			if (commandString == "who")
			{
				message = Who(message);
			}

			return message;
		}

		public static Message Who(Message message)
		{
			List<ClientSocket> clients = Server.connectedClients.GetUsers();

			string output = "Connected clients: " + clients.Count;

			foreach (ClientSocket client in clients)
			{
				output += "\n" + client.user.GetName();
			}
			
			message.content = output;
			return message;
		}

		public static void Whisper()
		{

		}
		public static void About()
		{

		}
	}
}
