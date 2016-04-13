using PlumeAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Entities {
	public static class EntityController {
		public static Dictionary<int, string> EntityIds = new Dictionary<int, string>();

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
		
		public static int GetEntityIdByName(string name) {
			return EntityIds.FirstOrDefault(item => item.Value == name).Key;
		}

		public static void AddEntityType(string fullName) {
			EntityIds.Add(GetNextHighestId(), fullName);
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
					Console.WriteLine((InterpolationPoint - SnapshotBeforeMoment.Received) + "," + (SnapshotAfterMoment.Received - SnapshotBeforeMoment.Received));
					Snapshots.RemoveWhere(x => x.Received < SnapshotBeforeMoment.Received);
				}
			}
		}
	}
}
