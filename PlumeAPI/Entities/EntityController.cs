using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Entities {
	public static class EntityController {
		public static Dictionary<int, string> EntityIds = new Dictionary<int, string>();
		static int NextHighestId = 0;

		public static int GetNextHighestId() {
			return NextHighestId++;
		}
		
		public static int GetEntityIdByName(string name) {
			return EntityIds.FirstOrDefault(item => item.Value == name).Key;
		}
	}
}
