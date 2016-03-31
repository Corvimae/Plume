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
		public static DateTime InterpolationPoint = DateTime.UtcNow;

		static int NextHighestId = 0;

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
				InterpolationPoint = DateTime.UtcNow.AddMilliseconds(-1 * Configuration.InterpolationDelay);
				ClientEntitySnapshot snapshot = Snapshots.Last();
				int i = Snapshots.Count() - 2;
				while(snapshot.Received > InterpolationPoint && i >= 0) {
					snapshot = Snapshots.ElementAt(i--);
				}
				if(i > 0) {
					SnapshotBeforeMoment = Snapshots.ElementAt(i);
					SnapshotAfterMoment = Snapshots.ElementAt(i + 1);
					Snapshots.RemoveWhere(x => x.Received < SnapshotBeforeMoment.Received);
				}
			}
		}
	}
}
