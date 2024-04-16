﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace TCPServer
{
	public class ClientSocket
	{
		// Encoding - do not alter
		public static Encoding encoding = Encoding.UTF8;

		public Socket socket;
		public const int BUFFER_SIZE = 2048;
		public byte[] buffer = new byte[BUFFER_SIZE];

	}
}
