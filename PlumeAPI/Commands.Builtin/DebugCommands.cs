using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Commands.Builtin {
	static class DebugCommands {
		public static void Register() {
			CommandController.RegisterCommand(
				"plume_test",
				"Test",
				(arguments) => {
					Console.WriteLine("Test!");
				}
			);
		}
	}
}
