using PlumeAPI.Entities;
using PlumeAPI.Modularization;
using PlumeAPI.Utilities;
using PlumeAPI.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Networking.Builtin {
	class SyncEntityToClientMessageHandler : MessageHandler {
		BaseEntity Entity;

		public SyncEntityToClientMessageHandler(BaseEntity entity) {
			Entity = entity;
		}

		public override OutgoingMessage PackageMessage(OutgoingMessage message) {
			message.Write(Entity.Scope.Name);
			Entity.PackageForInitialTransfer(message);
			return message;
		}

		public override void Handle(IncomingMessage message) {
			string scopeName = message.ReadString();
			int typeId = message.ReadInt32();
			BaseEntity newEntity = EntityController.CreateNewEntityFromServerData(typeId, message.ReadInt32(), ScopeController.GetScope(scopeName), message);
			Console.WriteLine("Entity created of type " + newEntity.Name + ", id: " + newEntity.Id);
		}
	}
}
