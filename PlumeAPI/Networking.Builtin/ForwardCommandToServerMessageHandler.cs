using PlumeAPI.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Networking.Builtin {
	public class ForwardCommandToServerMessageHandler : MessageHandler {
		string[] Arguments;

		public ForwardCommandToServerMessageHandler(string[] arguments) {
			Arguments = arguments;
		}
		public override OutgoingMessage PackageMessage(OutgoingMessage message) {
			foreach(string argument in Arguments) {
				message.Write(argument);
			}
			return message;
		}

		public override void Handle(IncomingMessage message) {
			string name = message.ReadString();
			List<string> arguments = new List<string>();
			while(message.Position < message.LengthBits) {
				arguments.Add(message.ReadString());
			}
			CommandController.Handle(name, arguments.ToArray(), ServerMessageDispatch.GetSender(message));
		}
	}
}
