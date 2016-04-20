using PlumeAPI.Entities;
using PlumeAPI.Entities.Components;
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
			foreach(NetworkedPropertyData property in entity.GetDerivativeComponent<NetworkedComponent>().GetClientControlledProperties()) {
				Action<object, OutgoingMessage> writer = EntitySnapshot.GetMessageWriter(property.Referencer.Info.PropertyType);
				writer.Invoke(property.Referencer.Info.GetValue(property.Referencer.Component), message);
			}
			return message;
		}

		public override void Handle(IncomingMessage message) {
			BaseEntity entity = ScopeController.GetEntityById(message.ReadInt32());
			if(entity != null && entity.GetDerivativeComponent<NetworkedComponent>() != null) {
				foreach(NetworkedPropertyData property in entity.GetDerivativeComponent<NetworkedComponent>().GetClientControlledProperties()) {
					property.Referencer.Info.SetValue(property.Referencer.Component, EntitySnapshot.ReadType(property.Referencer.Info.PropertyType, message));
				}
			}
		}
	}
}
