using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CoreEngine.Entities {
	public class EntityController {
		public static List<BaseEntity> EntityInstances = new List<BaseEntity>();
		public static Dictionary<string, BaseEntity> EntityRegistry = new Dictionary<string, BaseEntity>();

		public static void RegisterEntityInstance(BaseEntity entity) {
			EntityInstances.Add(entity);
		}

		public static List<BaseEntity> GetAllEntities() {
			return EntityInstances;
		}

		public static void RegisterEntity(string name, BaseEntity entity) {
			if(!EntityRegistry.ContainsKey(name)) {
				EntityRegistry.Add(name, entity);
			} else {
				throw new EntityAlreadyRegisteredException(name);
			}
		}

		public static void UnregisterEntity(string name) {
			if(EntityRegistry.ContainsKey(name)) {
				EntityRegistry.Remove(name);
			}
		}
		
		public static BaseEntity GetEntity(string name) {
			if(EntityRegistry.ContainsKey(name)) {
				return EntityRegistry[name];
			} else {
				throw new EntityNotRegisteredException(name);
			} 

		}
	}
	class EntityNotRegisteredException : Exception {
		string Name;

		public EntityNotRegisteredException(string name) {
			this.Name = name;
		}
	}
	class EntityAlreadyRegisteredException : Exception {
		string Name;

		public EntityAlreadyRegisteredException(string name) {
			this.Name = name;
		}
	}
}
