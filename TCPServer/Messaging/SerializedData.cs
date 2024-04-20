using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer.Messaging
{
	static class SerializedData
	{
		public static char field = (char)29; // ASCII field separator
		public static char record = '\0'; // ASCII record separator

		/* Message Structure:
		 * 
		 *	string sender\29
		 *	int messageType\29
		 *	string message\29
		 *  \30
		 */

		public static string Serialize(Message message)
		{
			string serialized = $"{message.sender}{field}"
				+ $"{(int)message.messageType}{field}"
				+ $"{message.content}{record}";

			return serialized;
		}

		public static string Sanitize(string message)
		{
			string output = message.Replace(record.ToString(), "");
			return output;
		}

		public static Message Decode(string data)
		{
			//data = data.Replace(record, '\0');
			Message message = new Message();

			string[] received = data.Split(field);

			if (received.Length == 3)
			{
				message.sender = received[0];
				message.messageType = (Message.MessageType)int.Parse(received[1]);
				message.content = Sanitize(received[2]);
			}

			return message;
		}
	}
}
