using PlumeAPI.Commands.Builtin;
using PlumeAPI.Modularization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Commands {
	public static class CommandController {
		static Dictionary<string, PlumeCommand> CommandRegistry = new Dictionary<string, PlumeCommand>();

		public static Action<string[]> Noop = (x) => { };


		static CommandController() {
			BuiltinCommandsRegistrator.Register();
		}
		public static void RegisterCommand(string name, string description, Action<string[]> client) {
			CommandRegistry.Add(name, new PlumeCommand(client, description));
		}

		public static void Handle(string name, string[] arguments) {
			if(CommandRegistry.ContainsKey(name)) {
				CommandRegistry[name].ClientMethod.Invoke(arguments);
			} else {
				Console.WriteLine(name + ": command not found.");
			}
		}

		public static void ParseCommand(string command) {
			string[] parts = command.Split(' ');
			Handle(parts[0], parts.Skip(1).ToArray());
		}


		public static void ArgumentError(int index, string argument, string type) {
			Console.WriteLine("Could not parse command: argument (" + index + ") requires type " + type + " (gave: " + argument + ").");
		}
	}

	public struct PlumeCommand {
		public Action<string[]> ClientMethod;
		string Description;

		public PlumeCommand(Action<string[]> client, string description) {
			ClientMethod = client;
			Description = description;
		}
	}
}
