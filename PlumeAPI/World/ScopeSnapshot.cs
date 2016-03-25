using Lidgren.Network;
using PlumeAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.World {
	public class ScopeSnapshot {
		List<EntitySnapshot> EntitySnapshots = new List<EntitySnapshot>();

		public ScopeSnapshot(EntityScope scope) {
			foreach(BaseEntity entity in scope.GetEntities()) {
				EntitySnapshots.Add(new EntitySnapshot(entity));
			}
		}

		public EntitySnapshot GetSnapshotForEntity(int id) {
			return EntitySnapshots.FirstOrDefault(x => x.Id == id);
		}
		
		public void PackageForMessage(NetOutgoingMessage message) {
			foreach(EntitySnapshot entitySnapshot in EntitySnapshots) {
				entitySnapshot.AddToMessage(message);
			}
		}
	}


}
