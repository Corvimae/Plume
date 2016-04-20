using PlumeAPI.Entities;
using PlumeAPI.Modularization;
using PlumeAPI.Networking;
using PlumeAPI.Networking.Builtin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.World {
	public class ScopeController {
		public static Dictionary<string, EntityScope> ScopeRegistry = new Dictionary<string, EntityScope>();
		public static Dictionary<int, BaseEntity> Entities = new Dictionary<int, BaseEntity>();
		private static int NextHighestId = 0;

		public static void RegisterScope(string name, EntityScope scope) {
			ScopeRegistry.Add(name, scope);
		}

		public static void UnregisterScope(string name) {
			if(ScopeRegistry.ContainsKey(name)) {
				ScopeRegistry.Remove(name);
			}
		}

		public static List<EntityScope> GetAllScopes() {
			return ScopeRegistry.Values.ToList();
		}

		public static BaseEntity GetEntityById(int id) {
			if(Entities.ContainsKey(id)) {
				return Entities[id];
			} else {
				Console.WriteLine("Entity with id " + id + " missing.");
				return null;
			}
		}

		public static int RegisterEntity(string scope, BaseEntity entity) {
			if(ScopeRegistry.ContainsKey(scope)) {
				EntityScope entityScope = ScopeRegistry[scope];
				return RegisterEntity(entityScope, entity);
			}
			throw new InvalidScopeException(scope);
		}

		public static int RegisterEntity(EntityScope entityScope, BaseEntity entity) {
			return RegisterEntity(entityScope, entity, GetNextHighestId());
		}

		public static int RegisterEntity(EntityScope entityScope, BaseEntity entity, int id) {
			entity.Id = id;
			entityScope.EntitiesInScope.Add(entity.Id, entity);
			entity.Scope = entityScope;

			Entities.Add(entity.Id, entity);

			if(ModuleController.Environment == PlumeEnvironment.Server) {
				entityScope.Broadcast(new SyncEntityToClientMessageHandler(entity));
			}
			return entity.Id;
		}

		public static EntityScope GetScope(string name) {
			if(ScopeRegistry.ContainsKey(name)) {
				return ScopeRegistry[name];
			}
			throw new InvalidScopeException(name);
		}

		public static int GetNextHighestId() {
			return NextHighestId++;
		}

		public static List<BaseEntity> GetAllEntities() {
			return Entities.Values.ToList<BaseEntity>();
		}
	}

	class InvalidScopeException : Exception {
		string Name;
		public InvalidScopeException(string name) {
			this.Name = name;
		}
	}
}
