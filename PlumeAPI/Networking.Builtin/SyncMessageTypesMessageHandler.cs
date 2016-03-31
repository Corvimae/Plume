using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlumeAPI.Utilities;
using Lidgren.Network;

namespace PlumeAPI.Networking.Builtin {
	public class SyncMessageTypesMessageHandler : MessageHandler {
		public override OutgoingMessage PackageMessage(OutgoingMessage message) {
			foreach(KeyValuePair<string, int> pair in MessageController.MessageTypeIds) {
				message.Write(pair.Key);
				message.Write(pair.Value);
			}
			return message;
		}

		public override void Handle(IncomingMessage message) {
			//Clear the ids registered on the client side to prevent collisions
			MessageController.MessageTypes.Keys.ToList().ForEach(k => {
				MessageController.MessageTypes.ChangeKey(k, -1 * k);
			});
			while(message.Position < message.LengthBits) {
				try {
					string name = message.ReadString();
					int id = message.ReadInt32();
					int oldId = MessageController.MessageTypeIds[name];
					MessageController.MessageTypeIds[name] = id;
					MessageController.MessageTypes.ChangeKey(oldId * -1, id);
				} catch(KeyNotFoundException) {
					Console.WriteLine("A message type key could not be found... is a dependency missing?");
				}
			}
		}
	}
}
