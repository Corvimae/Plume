using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using PlumeAPI.Entities;

namespace PlumeAPI.Networking.Builtin {
	class SyncEntityIdsMessageHandler : MessageHandler {
		public override OutgoingMessage PackageMessage(OutgoingMessage message) {
			foreach(KeyValuePair<int, BaseEntity> pair in EntityController.EntityPrototypes) {
				Console.WriteLine(pair.Key + "," + pair.Value.Name);
				message.Write(pair.Key);
				message.Write(pair.Value.Name);
			}
			return message;
		}

		public override void Handle(IncomingMessage message) {
			Dictionary<int, BaseEntity> newRegistry = new Dictionary<int, BaseEntity>();
			while(message.Position < message.LengthBits) {
				newRegistry.Add(message.ReadInt32(), EntityController.GetEntityPrototypeByName(message.ReadString()));
			}
			EntityController.EntityPrototypes = newRegistry;
		}
	}
}
