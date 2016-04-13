using PlumeAPI.Entities;
using PlumeAPI.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Networking.Builtin {
	public class SendClientEntityStateMessageHandler : MessageHandler {
		BaseEntity entity;
		public SendClientEntityStateMessageHandler(BaseEntity entity) {
			this.entity = entity;
		}

		public override OutgoingMessage PackageMessage(OutgoingMessage message) {
			message.Write(entity.Id);
			foreach(EntityPropertyData property in entity.GetClientControlledValues()) {
				Action<object, OutgoingMessage> writer = EntitySnapshot.GetMessageWriter(property.Info.PropertyType);
				writer.Invoke(property.Get(), message);
			}
			return message;
		}

		public override void Handle(IncomingMessage message) {
			BaseEntity entity = ScopeController.GetEntityById(message.ReadInt32());
			foreach(EntityPropertyData property in entity.GetClientControlledValues()) {
				property.Set(EntitySnapshot.ReadType(property.Info.PropertyType, message));
			}			
		}
	}
}
