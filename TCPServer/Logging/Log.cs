using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libs.Terminal;

namespace TCPServer.Logging
{
	class Log
	{
		DateTime timestamp;
		string message = string.Empty;

		public Log(string message)
		{
			timestamp = DateTime.UtcNow;
			this.message = message;
		}

		public static void Event(string message)
		{
			Log log = new Log(message);
			Terminal.Print(log.ToString());
		}

		public override string ToString()
		{
			return $"{timestamp}: {this.message}";
		}
	}
}
