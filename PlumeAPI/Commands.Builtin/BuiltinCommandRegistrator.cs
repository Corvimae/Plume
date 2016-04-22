using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Commands.Builtin {
	static class BuiltinCommandsRegistrator {
		public static void Register() {
			DebugCommands.Register();
		}
	}
}
