using Libs.Terminal;

namespace TCPServer
{
	/*	TCPServer:
	 *	
	 *		Front end wrapper for TCPServer.
	 *		Runs async, and handles async task killing.
	 */

	class Program
	{
		static Server server;

		// CancelToken required for stopping async Tasks.
		static CancellationTokenSource cts = new CancellationTokenSource();

		static async Task Initialize(CancellationToken cancellationToken)
		{
			// Subscribe to ctrl+c keypress event;
			Console.CancelKeyPress += OnCancelKeypress;

			try
			{
				server = Server.Instance(6666);
				if (server == null)
					throw new Exception("No server instance");

				await server.SetupAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				if (!(ex is TaskCanceledException))
				{
					Terminal.Print(ex);
				}
			}
		}

		static async Task Main(string[] args)
		{
			cts = new CancellationTokenSource();
			CancellationToken ct = cts.Token;

			await Initialize(ct);
		}

		// Ctrl + C
		static void OnCancelKeypress(object sender, ConsoleCancelEventArgs e)
		{
			Terminal.Print("Exiting...");

			e.Cancel = true;

			server.Shutdown();
			cts.Cancel();

			return;
		}
	}
}