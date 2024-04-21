using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServer.Logging;
using TCPServer.Client;
using TCPServer.Messaging;
using TCPServer.Logging;

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
			COMMAND_PROMOTE,
			COMMAND_DEMOTE,
			COMMAND_USERNAME,
			COMMAND_COMMANDS,
			COMMAND_KICK,
			COMMAND_INVALID
		}

		public static char commandChar = '!';

		public static Dictionary<Command, string> commandStrings = new Dictionary<Command, string>()
		{
			[Command.COMMAND_USERINFO] = "userinfo",
			[Command.COMMAND_WHO] = "who",
			[Command.COMMAND_ABOUT] = "about",
			[Command.COMMAND_WHISPER] = "whisper",
			[Command.COMMAND_PROMOTE] = "promote",
			[Command.COMMAND_DEMOTE] = "demote",
			[Command.COMMAND_USERNAME] = "username",
			[Command.COMMAND_KICK] = "kick",
			[Command.COMMAND_COMMANDS] = "commands",
		};

		public static Dictionary<Command, string> commandDetails = new Dictionary<Command, string>()
		{
			[Command.COMMAND_USERINFO] = "Prints out users information, IP, SecLvl, username.",
			[Command.COMMAND_WHO] = "Returns a list of connected users.",
			[Command.COMMAND_ABOUT] = "Prints information about this chat application.",
			[Command.COMMAND_WHISPER] = "Message another client directly.",
			[Command.COMMAND_PROMOTE] = "Promote a user's security rank.",
			[Command.COMMAND_DEMOTE] = "Demote a user's security rank.",
			[Command.COMMAND_USERNAME] = "Change your username.",
			[Command.COMMAND_KICK] = "Kick another user.",
			[Command.COMMAND_COMMANDS] = "Lists all available commands",
		};

		public delegate Message CommandDelegate(Message message, string[] args);
		// https://stackoverflow.com/questions/21924359/pass-a-dictionary-from-one-function-to-another-function-and-print-it
		public static Dictionary<Command, CommandDelegate> commands = new Dictionary<Command, CommandDelegate>()
		{
			[Command.COMMAND_WHO] = (message, args) => Who(message),
			[Command.COMMAND_USERNAME] = (message, args) => Username(message, args),
			[Command.COMMAND_USERINFO] = (message, args) => UserInfo(message, args),
			[Command.COMMAND_ABOUT] = (message, args) => About(message),
			[Command.COMMAND_WHISPER] = (message, args) => Whisper(message, args),
			[Command.COMMAND_PROMOTE] = (message, args) => Promote(message, args),
			[Command.COMMAND_DEMOTE] = (message, args) => Demote(message, args),
			[Command.COMMAND_COMMANDS] = (message, args) => ListCommands(message),
			[Command.COMMAND_KICK] = (message, args) => Kick(message, args),
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

				Log.Event($"[{message.sender}] used the {commandStrings[command]} command.", Log.LogType.LOG_COMMAND);
			}
			else
			{
				message.content = "Error: Invalid Command";
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

				Message.CommandMessage(message.clientSocket, "!" + message.clientSocket.user.UserInfo());
				Message.CommandMessage(message.clientSocket, $"Successfully changed name to: {newName}");
			}
			else
			{
				message.content = $"Error: username taken.";
			}

			return message;
		}

		public static Message ListCommands(Message message)
		{
			string output = "Commands: ";

			foreach (KeyValuePair<Command, string> kvp in commandDetails)
			{
				output += "\n!" + commandStrings[kvp.Key] + ": " + kvp.Value;
			}

			message.content = output;

			return message;
		}

		public static Message Whisper(Message message, string[] args)
		{
			if (args.Length < 2)
			{
				return message;
			}

			string target = args[1];

			ClientSocket targetClient = Server.connectedClients.GetUser(target);

			if (targetClient != null)
			{
				string messageStr = "";

				for (int i = 2; i < args.Length; i++)
				{
					messageStr += args[i] + " ";
				}

				message.content = messageStr;

				message.messageType = Message.MessageType.MESSAGE_TYPE_WHISPER;

				message.Send(targetClient);
			}

			return message;
		}

		public static Message About(Message message)
		{
			message.content = $"[NDSChat v.0.1]: a TCP chat application developed by Sam Catcheside, circa 2024 as part of the NDS203 subject at Torrens University.";
			return message;
		}

		public static Message Promote(Message message, string[] args)
		{
			if (args.Length == 2)
			{
				ClientSocket target = Server.connectedClients.GetUser(args[1]);
				message.messageType = Message.MessageType.MESSAGE_TYPE_COMMAND;

				if (target == null)
				{
					message.content = $"Error: target does not exist.";
					return message;
				}

				if (Server.connectedClients.Promote(target))
				{
					message.messageType = Message.MessageType.MESSAGE_TYPE_ANNOUNCEMENT;
					message.content = $"{target.user.GetName()} promoted to {UserGroup.rankNames[target.user.secLevel]}.";
				}
				else
				{
					message.content = $"Error: {target.user.GetName()} is already at the maximum rank.";
				}
			}

			return message;
		}

		public static Message Demote(Message message, string[] args)
		{
			if (args.Length == 2)
			{
				ClientSocket target = Server.connectedClients.GetUser(args[1]);
				message.messageType = Message.MessageType.MESSAGE_TYPE_COMMAND;

				if (target == null)
				{
					message.content = $"Error: target does not exist.";
					return message;
				}

				if (Server.connectedClients.Demote(target))
				{
					message.messageType = Message.MessageType.MESSAGE_TYPE_ANNOUNCEMENT;
					message.content = $"{target.user.GetName()} demoted to {UserGroup.rankNames[target.user.secLevel]}.";
				}
				else
				{
					message.content = $"Error: {target.user.GetName()} is already at the lowest rank.";
				}
			}

			return message;
		}

		public static Message Kick(Message message, string[] args)
		{
			if (message.clientSocket.user.secLevel >= UserGroup.SecLevel.SEC_LVL_MODERATOR)
			{
				if (args.Length == 2)
				{
					string target = args[1];
					ClientSocket client = Server.connectedClients.GetUser(target);

					if (client != null)
					{
						if (client.user.secLevel == UserGroup.SecLevel.SEC_LVL_MODERATOR)
						{
							Server.connectedClients.Demote(client);
							message.messageType = Message.MessageType.MESSAGE_TYPE_ANNOUNCEMENT;
							message.content = $"{client.user.GetName()} demoted to {UserGroup.rankNames[client.user.secLevel]}.";
						}
						else
						{
							Message kickMessage = new Message() { clientSocket = message.clientSocket, messageType = Message.MessageType.MESSAGE_TYPE_COMMAND };
							kickMessage.content = "You have been kicked from the server.";
							kickMessage.Send(client);
							client.user.isActive = false;
							client.socket.Shutdown(System.Net.Sockets.SocketShutdown.Both);

							message.content = $"Kicked {target}";
						}
					}
					else
					{
						message.content = $"Error: user is not connected.";
						return message;
					}
				}
				else
				{
					message.content = $"Error: invalid syntax. usage: !kick $user";
					return message;
				}
				
			}
			else
			{
				message.content = $"Error: you do not have permission to use this command.";
			}
			return message;
		}

		public static Message UserInfo(Message message, string[] args)
		{
			string target = "";
			if (args.Length == 1)
			{
				target = message.sender;
			}
			else if (args.Length == 2)
			{
				target = args[1];
			}

			ClientSocket client = Server.connectedClients.GetUser(target);

			if (client != null)
			{
				message.content = "User info for: " + target + "\n" + client.user.UserInfo();
			}
			else
			{
				message.content = $"Error: user does not exist.";
			}

			return message;
		}
	}
}
