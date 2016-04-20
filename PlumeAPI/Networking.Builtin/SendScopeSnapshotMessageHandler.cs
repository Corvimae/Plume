using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using PlumeAPI.World;
using PlumeAPI.Entities;
using System.Reflection;
using PlumeAPI.Utilities;
using PlumeAPI.Entities.Components;

namespace PlumeAPI.Networking.Builtin {
	public class SendScopeSnapshotMessageHandler : MessageHandler {
		ScopeSnapshot Snapshot;
		Client Recipient;
		public SendScopeSnapshotMessageHandler(ScopeSnapshot snapshot, Client recipient) {
			Snapshot = snapshot;
			Recipient = recipient;
		}
		public override OutgoingMessage PackageMessage(OutgoingMessage message) {
			message.Write(Recipient.LastProcessedTick);
			Snapshot.PackageForMessage(message);
			return message;
		}

		public override void Handle(IncomingMessage message) {
			Dictionary<int, object> PropertyValues = new Dictionary<int, object>();
			//Console.WriteLine("Snapshot update of size " + message.LengthBytes);
			long ticks = message.ReadInt64();
			long received = GameServices.TimeElapsed();

			ClientEntitySnapshot snapshot = new ClientEntitySnapshot(ticks, received);

			while(message.Position < message.LengthBits) {
				//Get the next ID
				PropertyValues.Clear();
				int id = message.ReadInt32();
				BaseEntity entity = ScopeController.GetEntityById(id);
				Console.WriteLine(id);
				//If the entity is null, it simply means the message to register it client-side hasn't arrived yet.
				//This screws up the rest of the snapshot, so we abort early
				if(entity != null) {
					NetworkedPropertyData[] properties = entity.GetComponent<NetworkedComponent>().GetSyncableProperties();
					//Set the last updated tick
					short count = message.ReadByte();
					for(int i = 0; i < count; i++) {
						byte position = message.ReadByte();
						//Find the property in that position
						NetworkedPropertyData property = properties[position];
						object value = EntitySnapshot.ReadType(property.Referencer.Info.PropertyType, message);
						snapshot.SetProperty(id, property.Referencer, value);
						Console.WriteLine(property.Referencer.Info.Name + " set to " + value);
					}
				} else {
					return;
				}
			}

			EntityController.Snapshots.Add(snapshot);

		}
	}
}
