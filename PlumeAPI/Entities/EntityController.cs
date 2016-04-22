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

		static int NextHighestId = 0;

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
	}
}
