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
				"lerp_delay [ms] - Sets the delay in milliseconds for entity interpolation. Higher values increase likeliness for smooth motion, but increases the delay between " +
				"client and world state.",
				(arguments) => {
					int delay;
					if(Int32.TryParse(arguments[0], out delay)) {
						ClientConfiguration.InterpolationDelay = delay;
					} else {
						CommandController.ArgumentError(0, arguments[0], "integer");
					}
				},
				CommandController.ServerNoop
			);

			CommandController.RegisterCommand(
				"client_smoothing",
				"client_smoothing [bool] - Enables smoothing when client-side prediction does not line up with the server.",
				(arguments) => {
					bool enable;
					if(arguments[0].ParseToBool(out enable)) {
						ClientConfiguration.ClientPredictionSmoothing = enable;
					} else {
						CommandController.ArgumentError(0, arguments[0], "boolean");
					}
				},
				CommandController.ServerNoop
			);
		}
	}
}
