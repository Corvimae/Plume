using PlumeAPI.Networking;
using PlumeAPI.Utilities;
using PlumeAPI.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Entities {
	public static class EntityController {
		public static Dictionary<int, BaseEntity> EntityPrototypes = new Dictionary<int, BaseEntity>();

		public static SortedSet<ClientEntitySnapshot> Snapshots = new SortedSet<ClientEntitySnapshot>();
		public static ClientEntitySnapshot SnapshotBeforeMoment = null;
		public static ClientEntitySnapshot SnapshotAfterMoment = null;
		public static long InterpolationPoint = GameServices.TimeElapsed();

		static int NextHighestId = 0;

		public static long Tick = 0;
		public static long LastProcessedTick = 0;

		public static int GetNextHighestId() {
			return NextHighestId++;
		}
		
		public static BaseEntity CreateNewEntity(string name) {
			return EntityPrototypes.Values.FirstOrDefault(item => item.Name == name).Clone();
		}

		public static BaseEntity CreateNewEntity(string name, EntityScope scope) {
			BaseEntity entity = CreateNewEntity(name);
			entity.RegisterToScope(scope);
			return entity;
		}

		public static BaseEntity CreateNewEntityFromServerData(int typeId, int id, EntityScope scope, IncomingMessage message) {
			BaseEntity newEntity = EntityController.CreateNewEntity(typeId);
			newEntity.Id = id;
			newEntity.UnpackageFromInitialTransfer(message);
			ScopeController.RegisterEntity(scope, newEntity, id);
			return newEntity;
		}
		public static BaseEntity CreateNewEntity(int id) {
			return EntityPrototypes[id].Clone();
		}

		public static int GetEntityPrototypeIdByName(string name) {
			return EntityPrototypes.FirstOrDefault(item => item.Value.Name == name).Key;
		}

		public static BaseEntity GetEntityPrototypeByName(string name) {
			return EntityPrototypes.Values.FirstOrDefault(item => item.Name == name);
		}

		public static void RegisterPrototype(BaseEntity prototype) {
			EntityPrototypes.Add(GetNextHighestId(), prototype);
		}

		public static void SetSnapshotsForMoment() {
			if(Snapshots.Count() >= 2) {
				InterpolationPoint = GameServices.TimeElapsed() - ClientConfiguration.InterpolationDelay;
				ClientEntitySnapshot snapshot = Snapshots.Last();
				int i = Snapshots.Count() - 2;
				while(snapshot.Received >= InterpolationPoint && i >= 0) {
					snapshot = Snapshots.ElementAt(i--);
				}
				if(i > 0 && i + 2 < Snapshots.Count()) {
					SnapshotBeforeMoment = snapshot;
					SnapshotAfterMoment = Snapshots.ElementAt(i + 2);
					Snapshots.RemoveWhere(x => x.Received < SnapshotBeforeMoment.Received);
				}
			}
		}
	}
}
