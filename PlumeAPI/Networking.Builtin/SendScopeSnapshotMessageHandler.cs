using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using PlumeAPI.World;
using PlumeAPI.Entities;
using System.Reflection;

namespace PlumeAPI.Networking.Builtin {
	public class SendScopeSnapshotMessageHandler : MessageHandler {
		ScopeSnapshot Snapshot;
		public SendScopeSnapshotMessageHandler(ScopeSnapshot snapshot) {
			Snapshot = snapshot;
		}
		public override NetOutgoingMessage PackageMessage(NetOutgoingMessage message) {
			Snapshot.PackageForMessage(message);
			return message;
		}

		public override void Handle(NetIncomingMessage message) {
			while(message.Position < message.LengthBits) {
				//Get the next ID
				int id = message.ReadInt32();
				BaseEntity entity = ScopeController.GetEntityById(id);
				IEnumerable<PropertyInfo> properties = entity.GetSyncableProperties();
				foreach(PropertyInfo property in properties) {
					property.SetValue(entity, EntitySnapshot.ReadType(property.PropertyType, message));
				}
			}
		}
	}
}
