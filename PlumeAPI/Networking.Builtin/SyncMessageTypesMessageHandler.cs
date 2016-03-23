using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlumeAPI.Utilities;
using Lidgren.Network;

namespace PlumeAPI.Networking.Builtin {
	public class SyncMessageTypesMessageHandler : MessageHandler {
		public SyncMessageTypesMessageHandler() {
			Name = "SyncMessageTypes";
		}

		public override NetOutgoingMessage PackageMessage(NetOutgoingMessage message) {
			foreach(KeyValuePair<string, int> pair in MessageController.MessageTypeIds) {
				message.Write(pair.Key);
				message.Write(pair.Value);
			}
			return message;
		}

		public override void Handle(NetIncomingMessage message) {
			//Clear the ids registered on the client side to prevent collisions
			MessageController.MessageTypes.Keys.ToList().ForEach(k => {
				MessageController.MessageTypes.ChangeKey(k, -1 * k);
			});
			while(message.Position < message.LengthBits) {
				string name = message.ReadString();
				int id = message.ReadInt32();
				int oldId = MessageController.MessageTypeIds[name];
				MessageController.MessageTypeIds[name] = id;
				MessageController.MessageTypes.ChangeKey(oldId * -1, id);
			}
		}
	}
}
