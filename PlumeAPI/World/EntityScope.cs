using Lidgren.Network;
using PlumeAPI.Entities;
using PlumeAPI.Modularization;
using PlumeAPI.Networking;
using PlumeAPI.Networking.Builtin;
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
		public ScopeSnapshot PreviousSnapshot { get; set; }
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

		public IEnumerable<Client> GetClients() {
			return ServerMessageDispatch.Clients.Where(x => x.Scope == this);
		}
		
		public void Broadcast(MessageHandler handler) {
			ServerMessageDispatch.SendToScope(handler, this);
		}


		public void Update() {
			foreach(BaseEntity entity in EntitiesInScope.Values.ToArray()) {
				if(entity.HasPropertyEnabled("update")) entity.Update();
			}

			//Create a new snapshot if anyone's here.
			if(GetClients().Count() > 0) {
				ScopeSnapshot snapshot = new ScopeSnapshot(this);
				foreach(Client client in GetClients()) {
					ServerMessageDispatch.Send(new SendScopeSnapshotMessageHandler(snapshot, client), client);
				}
				PreviousSnapshot = snapshot;
			}
			//Set the previous snapshot to the new one
		}

		public void PackageForMessage(OutgoingMessage message) {
			message.Write(Name);
			foreach(BaseEntity entity in GetEntities()) {
				entity.PackageForInitialTransfer(message);
			}
		}
		public void UnpackageFromMessage(ref IncomingMessage message) {
			while(message.Position < message.LengthBits) {
				int typeId = message.ReadInt32();
				string referencer = EntityController.EntityIds[typeId];
				Type entityType = ModuleController.GetEntityTypeByReferencer(referencer);
				object[] entityArguments = TypeServices.InvokeStaticTypeMethod("UnpackageFromInitialTransfer", entityType, message);
				BaseEntity entity = ScopeController.UpdateOrCreateWithId((int) entityArguments[0], this, referencer, entityArguments.Skip(1).ToArray());
			}
		}
	}
}
