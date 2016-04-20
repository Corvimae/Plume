using Lidgren.Network;
using PlumeAPI.Entities;
using PlumeAPI.Entities.Components;
using PlumeAPI.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.World {
	public class ScopeSnapshot {
		Dictionary<int, EntitySnapshot> EntitySnapshots = new Dictionary<int, EntitySnapshot>();
		public long Tick;

		public ScopeSnapshot(EntityScope scope) {
			Tick = ServerMessageDispatch.GetTick();
			foreach(BaseEntity entity in scope.GetEntities()) {
				NetworkedComponent component = null;
				if(entity.TryGetDerivativeComponent(ref component)) {
					if(component.GetSyncableProperties().Length > 0) {
						EntitySnapshots.Add(entity.Id, new EntitySnapshot(entity));
					}
				}
			}
		}

		public EntitySnapshot GetSnapshotForEntity(int id) {
			EntitySnapshot value;
			EntitySnapshots.TryGetValue(id, out value);
			return value;
		}
		
		public void PackageForMessage(OutgoingMessage message) {
			foreach(EntitySnapshot entitySnapshot in EntitySnapshots.Values) {
				entitySnapshot.AddToMessage(message);
			}
		}
	}


}
