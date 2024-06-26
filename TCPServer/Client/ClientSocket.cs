﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TCPServer.Client
{
	public class ClientSocket
	{
		// Encoding - do not alter
		

		public Socket? socket;
		public const int BUFFER_SIZE = 2048;
		public byte[] buffer = new byte[BUFFER_SIZE];
		public User? user;

		public IPAddress? IP;


		public EndPoint GetIP()
		{
			return socket.RemoteEndPoint;
		}
	}
}

/*  Author: Sam Catcheside, A00115110
 *  Date: 21/04/2024
 */