using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using PlumeAPI.Entities;

namespace PlumeAPI.Networking.Builtin {
	class SyncEntityIdsMessageHandler : MessageHandler {
		public override NetOutgoingMessage PackageMessage(NetOutgoingMessage message) {
			foreach(KeyValuePair<int, string> pair in EntityController.EntityIds) {
				message.Write(pair.Key);
				message.Write(pair.Value);
			}
			return message;
		}

		public override void Handle(NetIncomingMessage message) {
			EntityController.EntityIds.Clear();
			while(message.Position < message.LengthBits) {
				EntityController.EntityIds.Add(message.ReadInt32(), message.ReadString());
			}
		}
	}
}
