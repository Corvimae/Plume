using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using PlumeAPI.World;
using PlumeAPI.Entities;
using System.Reflection;
using PlumeAPI.Attributes;

namespace PlumeAPI.Networking.Builtin {
	public class SendScopeSnapshotMessageHandler : MessageHandler {
		ScopeSnapshot Snapshot;
		static DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		public SendScopeSnapshotMessageHandler(ScopeSnapshot snapshot) {
			Snapshot = snapshot;
		}
		public override OutgoingMessage PackageMessage(OutgoingMessage message) {
			message.Write(Snapshot.Tick);
			Snapshot.PackageForMessage(message);
			return message;
		}

		public override void Handle(IncomingMessage message) {
			Dictionary<int, object> PropertyValues = new Dictionary<int, object>();
			//Console.WriteLine("Snapshot update of size " + message.LengthBytes);
			long ticks = message.ReadInt64();
			DateTime received = DateTime.UtcNow;

			ClientEntitySnapshot snapshot = new ClientEntitySnapshot(ticks, received);

			while(message.Position < message.LengthBits) {
				//Get the next ID
				PropertyValues.Clear();
				int id = message.ReadInt32();
				BaseEntity entity = ScopeController.GetEntityById(id);
				//If the entity is null, it simply means the message to register it client-side hasn't arrived yet.
				//This screws up the rest of the snapshot, so we abort early
				if(entity != null) {
					PropertyInfo[] properties = entity.GetSyncableProperties().ToArray();
					short count = message.ReadByte();
					for(int i = 0; i < count; i++) {
						byte position = message.ReadByte();
						//Find the property in that position
						PropertyInfo property = properties[position];
						object value = EntitySnapshot.ReadType(property.PropertyType, message);
						snapshot.SetProperty(id, property, value);
					}
				} else {
					return;
				}
			}

			EntityController.Snapshots.Add(snapshot);

		}
	}
}
