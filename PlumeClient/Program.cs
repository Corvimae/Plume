using System;

namespace PlumeClient {
#if WINDOWS || LINUX
	/// <summary>
	/// The main class.
	/// </summary>
	public static class Program {
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			var game = new Core();
			game.Run();
		}
	}
#endif
}
