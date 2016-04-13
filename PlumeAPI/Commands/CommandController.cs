using PlumeAPI.Commands.Builtin;
using PlumeAPI.Modularization;
using PlumeAPI.Networking;
using PlumeAPI.Networking.Builtin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Commands {
	public static class CommandController {
		static Dictionary<string, PlumeCommand> CommandRegistry = new Dictionary<string, PlumeCommand>();

		public static Action<string[]> ClientNoop = (x) => { };
		public static Action<string[]> Forward = (x) => ClientMessageDispatch.Send(new ForwardCommandToServerMessageHandler(x));

		public static Action<Client, string[]> ServerNoop = (x, y) => { };

		static CommandController() {
			BuiltinCommandsRegistrator.Register();
		}
		public static void RegisterCommand(string name, string description, Action<string[]> client, Action<Client, string[]> server) {
			CommandRegistry.Add(name, new PlumeCommand(client, server, description));
		}

		public static void Handle(string name, string[] arguments, Client source) {
			if(CommandRegistry.ContainsKey(name)) {
				if(ModuleController.Environment == PlumeEnvironment.Client) {
					if(CommandRegistry[name].ClientMethod == Forward) {
						CommandRegistry[name].ClientMethod.Invoke(CreateForwardActionArguments(name, arguments));
					} else {
						CommandRegistry[name].ClientMethod.Invoke(arguments);
					}
				} else {
					CommandRegistry[name].ServerMethod.Invoke(source, arguments);
				}
			} else {
				Console.WriteLine(name + ": command not found.");
			}
		}

		public static void ParseCommand(string command) {
			string[] parts = command.Split(' ');
			Handle(parts[0], parts.Skip(1).ToArray(), null);
		}

		public static string[] CreateForwardActionArguments(string name, string[] arguments) {
			string[] fullArguments = new string[arguments.Length + 1];
			fullArguments[0] = name;
			Array.Copy(arguments, 0, fullArguments, 1, arguments.Length);
			return fullArguments;
		}

		public static void ForwardCommandToServer(string name, string[] arguments) {
			ClientMessageDispatch.Send(new ForwardCommandToServerMessageHandler(CreateForwardActionArguments(name, arguments)));
		}

		public static void ArgumentError(int index, string argument, string type) {
			Console.WriteLine("Could not parse command: argument (" + index + ") requires type " + type + " (gave: " + argument + ").");
		}
	}

	public struct PlumeCommand {
		public Action<string[]> ClientMethod;
		public Action<Client, string[]> ServerMethod;
		string Description;

		public PlumeCommand(Action<string[]> client, Action<Client, string[]> server, string description) {
			ClientMethod = client;
			ServerMethod = server;
			Description = description;
		}
	}
}
