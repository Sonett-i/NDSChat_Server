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
			COMMAND_USERINFO,
			COMMAND_WHO,
			COMMAND_ABOUT,
			COMMAND_WHISPER,
			COMMAND_SET_SECLVL,
			COMMAND_USERNAME,
			COMMAND_COMMANDS,
			COMMAND_INVALID
		}

		public static char commandChar = '!';

		public static Dictionary<Command, string> commandStrings = new Dictionary<Command, string>()
		{
			[Command.COMMAND_USERINFO] = "userinfo",
			[Command.COMMAND_WHO] = "who",
			[Command.COMMAND_ABOUT] = "about",
			[Command.COMMAND_WHISPER] = "whisper",
			[Command.COMMAND_SET_SECLVL] = "promote",
			[Command.COMMAND_USERNAME] = "username",
		};

		public delegate Message CommandDelegate(Message message, string[] args);
		// https://stackoverflow.com/questions/21924359/pass-a-dictionary-from-one-function-to-another-function-and-print-it
		public static Dictionary<Command, CommandDelegate> commands = new Dictionary<Command, CommandDelegate>()
		{
			[Command.COMMAND_WHO] = (message, args) => Who(message),
			[Command.COMMAND_USERNAME] = (message, args) => Username(message, args),
			[Command.COMMAND_USERINFO] = (message, args) => Placeholder(message),
			[Command.COMMAND_ABOUT] = (message, args) => Placeholder(message),
			[Command.COMMAND_WHISPER] = (message, args) => Placeholder(message),
			[Command.COMMAND_SET_SECLVL] = (message, args) => Placeholder(message),
		};

		// iterates through commandStrings dictionary and returns enum if match.
		public static Command CommandFromString(string input)
		{
			foreach (KeyValuePair<Command, string> entry in commandStrings)
			{
				if (entry.Value == input)
				{
					return entry.Key;
				}
			}
			return Command.COMMAND_INVALID;
		}

		// splits string using space delimeters and returns string array.
		public static string[] GetCommandArgs(string input)
		{
			input = input.Replace(commandChar.ToString(), "");
			return input.Split(' ');
		}

		// handles commands lol
		public static Message HandleCommand(Message message)
		{
			string[] args = GetCommandArgs(message.content);
			Command command = CommandFromString(args[0]);

			message.messageType = Message.MessageType.MESSAGE_TYPE_COMMAND;

			if (command != Command.COMMAND_INVALID)
			{
				commands[command].Invoke(message, args);
			}

			return message;
		}

		public static Message Placeholder(Message message)
		{
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

		public static Message Username(Message message, string[] args)
		{
			string newName = args[1];
			if (!Server.connectedClients.UserExists(newName))
			{
				string originalName = message.clientSocket.user.GetName();

				message.clientSocket.user.SetName(args[1]);
				message.messageType = Message.MessageType.MESSAGE_TYPE_ANNOUNCEMENT;
				message.content = $"{originalName} changed their name to: {message.clientSocket.user.GetName()}";
			}
			else
			{
				message.content = $"Error: username taken.";
			}

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
