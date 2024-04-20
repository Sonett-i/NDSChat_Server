using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServer.Client;
using System.Net.Sockets;

namespace TCPServer.Messaging
{
	public class Message
	{
		public static Encoding encoding = Encoding.UTF8;
		public enum MessageType
		{
			MESSAGE_TYPE_DEFAULT,
			MESSAGE_TYPE_ANNOUNCEMENT,
			MESSAGE_TYPE_WHISPER,
			MESSAGE_TYPE_COMMAND,
		}

		public static Dictionary<MessageType, string> messageTypes = new Dictionary<MessageType, string>()
		{
			[MessageType.MESSAGE_TYPE_DEFAULT] = "DEFAULT",
			[MessageType.MESSAGE_TYPE_ANNOUNCEMENT] = "ANNOUNCEMENT",
			[MessageType.MESSAGE_TYPE_WHISPER] = "WHISPER",
			[MessageType.MESSAGE_TYPE_COMMAND] = "COMMAND"
		};

		public MessageType messageType;
		public ClientSocket? clientSocket = null;
		public string content = string.Empty;
		public string sender;

		public byte[] Encode()
		{
			return encoding.GetBytes(content);
		}

		public byte[] Encode(string message)
		{
			return encoding.GetBytes(message);
		}

		public static Message? Receive(byte[] received, ClientSocket client)
		{
			Message message = SerializedData.Decode(Message.encoding.GetString(received));
			message.clientSocket = client;
			if (message.messageType == MessageType.MESSAGE_TYPE_COMMAND)
			{
				Commands.HandleCommand(message);
			}

			if (message.messageType == MessageType.MESSAGE_TYPE_ANNOUNCEMENT)
			{
				Events.HandleEvent(message);
			}

			return message;
		}

		public void Send()
		{
			string serialized = SerializedData.Serialize(this);

			byte[] data = Encode(serialized);

			clientSocket.socket.Send(data, 0, data.Length, SocketFlags.None);
		}

		public void Send(ClientSocket client)
		{
			string serialized = SerializedData.Serialize(this);

			byte[] data = Encode(serialized);

			client.socket.Send(data, 0, data.Length, SocketFlags.None);
		}

		public string Format()
		{
			return $"[{this.clientSocket.user.GetName()}][{Message.messageTypes[this.messageType]}]: {this.content}";
		}
	}
}
