using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer.Messaging
{
	public class Events
	{
		public enum EventType
		{
			EVENT_JOINED,
			EVENT_DISCONNECTED,
			EVENT_NAME_CHANGED
		}

		public static Dictionary<EventType, string> EventStrings = new Dictionary<EventType, string>() 
		{
			[EventType.EVENT_JOINED] = "JOINED",
			[EventType.EVENT_DISCONNECTED] = "DISCONNECTED",
			[EventType.EVENT_NAME_CHANGED] = "NAME CHANGED",
		};

		public static void HandleEvent(Message message)
		{
			if (message.content == "JOINED")
			{
				message.clientSocket.user.SetName(message.sender);
				Server.connectedClients.AddUser(message.clientSocket);
			}
		}
	}
}
