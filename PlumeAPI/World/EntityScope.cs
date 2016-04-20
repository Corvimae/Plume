using Lidgren.Network;
using Microsoft.Xna.Framework;
using PlumeAPI.Entities;
using PlumeAPI.Events;
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

		public Rectangle Boundry = new Rectangle(-999999999, -999999999, 999999999*2, 999999999*2);
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

		public Rectangle GetBoundry() {
			return Boundry;
		}

		public void Update() {
			EventController.Fire("update", this);
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
			message.Write(Boundry.X);
			message.Write(Boundry.Y);
			message.Write(Boundry.Width);
			message.Write(Boundry.Height);
			Console.WriteLine(GetEntities().Count);
			foreach(BaseEntity entity in GetEntities()) {
				entity.PackageForInitialTransfer(message);
			}
		}
		public void UnpackageFromMessage(ref IncomingMessage message) {
			Boundry = new Rectangle(message.ReadInt32(), message.ReadInt32(), message.ReadInt32(), message.ReadInt32());
			while(message.Position < message.LengthBits) {
				int typeId = message.ReadInt32();
				BaseEntity newEntity = EntityController.CreateNewEntityFromServerData(typeId, message.ReadInt32(), this, message);
			}
		}
	}
}
