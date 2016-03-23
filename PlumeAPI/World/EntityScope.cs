using Lidgren.Network;
using PlumeAPI.Entities;
using PlumeAPI.Modularization;
using PlumeAPI.Networking;
using PlumeAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.World {
	public class EntityScope {
		public string Name;
		public Dictionary<int, BaseEntity> EntitiesInScope = new Dictionary<int, BaseEntity>();

		public EntityScope(string name) {
			this.Name = name;
			ScopeController.RegisterScope(name, this);
		}

		public void RegisterEntity(BaseEntity entity) {
			ScopeController.RegisterEntity(Name, entity);
		}

		public List<BaseEntity> GetEntities() {
			return EntitiesInScope.Values.ToList<BaseEntity>();
		}

		public void PackageForMessage(NetOutgoingMessage message) {
			message.Write(Name);
			foreach(BaseEntity entity in GetEntities()) {
				entity.PackageForInitialTransfer(message);
			}
		}
		public void UnpackageFromMessage(ref NetIncomingMessage message) {
			while(message.Position < message.LengthBits) {
				string referencer = message.ReadString();
				Type entityType = ModuleController.GetEntityTypeByReferencer(referencer);
				object[] entityArguments = TypeServices.InvokeStaticTypeMethod("UnpackageFromInitialTransfer", entityType, message);
				BaseEntity entity = ScopeController.UpdateOrCreateWithId((int) entityArguments[0], this, referencer, entityArguments.Skip(1).ToArray());
			}
		}
	}
}
