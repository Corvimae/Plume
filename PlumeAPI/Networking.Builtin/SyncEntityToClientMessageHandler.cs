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
			string referencer = EntityController.EntityIds[typeId];
			Type entityType = ModuleController.GetEntityTypeByReferencer(referencer);
			object[] entityArguments = TypeServices.InvokeStaticTypeMethod("UnpackageFromInitialTransfer", entityType, message);
			EntityScope scope = ScopeController.GetScope(scopeName);
			BaseEntity entity = ScopeController.UpdateOrCreateWithId((int)entityArguments[0], scope, referencer, entityArguments.Skip(1).ToArray());
			Console.WriteLine("Entity created of type " + entityType.FullName);
		}
	}
}
