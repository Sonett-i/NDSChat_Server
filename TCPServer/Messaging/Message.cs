using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPServer.Data;
using TCPServer.UserData;

namespace TCPServer.Messaging
{
	internal class Message
	{
		public enum MessageType
		{
			MESSAGE_TYPE_DEFAULT,
			MESSAGE_TYPE_ANNOUNCEMENT,
			MESSAGE_TYPE_WHISPER
		}

		public static void Receive(string message, ClientSocket client)
		{

		}
	}
}
