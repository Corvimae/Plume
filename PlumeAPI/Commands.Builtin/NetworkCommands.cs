using PlumeAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Commands.Builtin {
	class NetworkCommands {
		public static void Register() {
			CommandController.RegisterCommand(
				"lerp_delay",
				"lerp_delay [ms] - Sets the delay in milliseconds for entity interpolation. Higher values increase likeliness for smooth motion, but increases the delay before " +
				"the client views the world.",
				(arguments) => {
					int delay;
					if(Int32.TryParse(arguments[0], out delay)) {
						Configuration.InterpolationDelay = delay;
					} else {
						CommandController.ArgumentError(0, arguments[0], "integer");
					}
				},
				CommandController.ServerNoop
			);
		}
	}
}
