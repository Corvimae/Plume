using PlumeAPI.Networking;
using PlumeRPG.Entities;
using PlumeAPI.Networking.Builtin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlumeRPG;

namespace PlumeAPI.Commands.Builtin {
	class MovementCommands {
		public static void Register() {
			CommandController.RegisterCommand(
				"+north",
				"Sends a request to the server to begin moving the client's player entity north.",
				(arguments) => {
					Main.ActivePlayer.MotionState |= ActorMotion.North;
				},
				CommandController.ServerNoop

			);
			CommandController.RegisterCommand(
				"-north",
				"Sends a request to the server to stop moving the client's player entity north.",
				(arguments) => {
					Main.ActivePlayer.MotionState &= ~ActorMotion.North;
				},
				CommandController.ServerNoop
			);

			CommandController.RegisterCommand(
				"+south",
				"Sends a request to the server to begin moving the client's player entity south.",
				(arguments) => {
					Main.ActivePlayer.MotionState |= ActorMotion.South;
				},
				CommandController.ServerNoop
			);
			CommandController.RegisterCommand(
				"-south",
				"Sends a request to the server to stop moving the client's player entity south.",
				(arguments) => {
					Main.ActivePlayer.MotionState &= ~ActorMotion.South;
				},
				CommandController.ServerNoop
			);

			CommandController.RegisterCommand(
				"+east",
				"Sends a request to the server to begin moving the client's player entity east.",
				(arguments) => {
					Main.ActivePlayer.MotionState |= ActorMotion.East;
				},
				CommandController.ServerNoop
			);
			CommandController.RegisterCommand(
				"-east",
				"Sends a request to the server to stop moving the client's player entity east.",
				(arguments) => {
					Main.ActivePlayer.MotionState &= ~ActorMotion.East;
				},
				CommandController.ServerNoop
			);

			CommandController.RegisterCommand(
				"+west",
				"Sends a request to the server to begin moving the client's player entity west.",
				(arguments) => {
					Main.ActivePlayer.MotionState |= ActorMotion.West;
				},
				CommandController.ServerNoop
			);
			CommandController.RegisterCommand(
				"-west",
				"Sends a request to the server to stop moving the client's player entity west.",
				(arguments) => {
					Main.ActivePlayer.MotionState &= ~ActorMotion.West;
				},
				CommandController.ServerNoop
			);
		}
	}
}
