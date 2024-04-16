using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer
{
	public class ServerConfig
	{
		int port;


		public static bool ValidPort(int port)
		{
			return (port > 0 && port < 65535) ? true : false;
		}
	}
}
