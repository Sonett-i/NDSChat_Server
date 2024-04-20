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
		public enum LogType
		{
			LOG_ERROR,
			LOG_EVENT,
			LOG_MESSAGE,
			LOG_COMMAND
		}

		DateTime timestamp;
		string message = string.Empty;
		LogType logType = LogType.LOG_ERROR;

		public Log(string message, LogType type)
		{
			timestamp = DateTime.UtcNow;
			this.message = message;
			this.logType = type;
		}

		public static void Event(string message, LogType type)
		{
			Log log = new Log(message, type);
			Terminal.Print(log.ToString());
		}

		public override string ToString()
		{
			return $"{timestamp}: {this.message}";
		}
	}
}
