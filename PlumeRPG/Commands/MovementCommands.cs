using PlumeAPI.Networking;
using PlumeRPG.Entities;
using PlumeAPI.Networking.Builtin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Commands.Builtin {
	class MovementCommands {
		public static void Register() {
			CommandController.RegisterCommand(
				"+north",
				"Sends a request to the server to begin moving the client's player entity north.",
				CommandController.ForwardCommandToServer,
				(sender, arguments) => {
					((Player)sender["PlayerEntity"]).MotionState |= ActorMotion.North;
				}
			);
			CommandController.RegisterCommand(
				"-north",
				"Sends a request to the server to stop moving the client's player entity north.",
				CommandController.ForwardCommandToServer,
				(sender, arguments) => {
					((Player)sender["PlayerEntity"]).MotionState &= ~ActorMotion.North;
				}
			);

			CommandController.RegisterCommand(
				"+south",
				"Sends a request to the server to begin moving the client's player entity south.",
				CommandController.ForwardCommandToServer,
				(sender, arguments) => {
					((Player)sender["PlayerEntity"]).MotionState |= ActorMotion.South;
				}
			);
			CommandController.RegisterCommand(
				"-south",
				"Sends a request to the server to stop moving the client's player entity south.",
				CommandController.ForwardCommandToServer,
				(sender, arguments) => {
					((Player)sender["PlayerEntity"]).MotionState &= ~ActorMotion.South;
				}
			);

			CommandController.RegisterCommand(
				"+east",
				"Sends a request to the server to begin moving the client's player entity east.",
				CommandController.ForwardCommandToServer,
				(sender, arguments) => {
					((Player)sender["PlayerEntity"]).MotionState |= ActorMotion.East;
				}
			);
			CommandController.RegisterCommand(
				"-east",
				"Sends a request to the server to stop moving the client's player entity east.",
				CommandController.ForwardCommandToServer,
				(sender, arguments) => {
					((Player)sender["PlayerEntity"]).MotionState &= ~ActorMotion.East;
				}
			);

			CommandController.RegisterCommand(
				"+west",
				"Sends a request to the server to begin moving the client's player entity west.",
				CommandController.ForwardCommandToServer,
				(sender, arguments) => {
					((Player)sender["PlayerEntity"]).MotionState |= ActorMotion.West;
				}
			);
			CommandController.RegisterCommand(
				"-west",
				"Sends a request to the server to stop moving the client's player entity west.",
				CommandController.ForwardCommandToServer,
				(sender, arguments) => {
					((Player)sender["PlayerEntity"]).MotionState &= ~ActorMotion.West;
				}
			);
		}
	}
}
