using PlumeAPI.Modularization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Networking.Builtin {
	public class SendModuleRequirementsMessageHandler : MessageHandler {
		public override OutgoingMessage PackageMessage(OutgoingMessage message) {
			foreach(Module module in ModuleController.GetModules()) {
				message.Write(module.Definition.ModuleInfo.Name);
				message.Write(module.Definition.ModuleInfo.Version);
				message.Write(module.Definition.ModuleInfo.Author);
			}
			return message;
		}

		public override void Handle(IncomingMessage message) {
			while(message.Position < message.LengthBits) {
				string name = message.ReadString();
				string version = message.ReadString();
				string author = message.ReadString();
				if(!ModuleController.IsModuleRegistered(name, version, author)) {
					ClientMessageDispatch.Disconnect("Missing dependency: " + author + "/" + name + " (" + version + ")");
					Console.WriteLine("Missing required module: " + author + "/" + name + " (" + version + ")");
				}
			}
		}
	}
}
