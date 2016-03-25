using PlumeAPI.Entities;
using PlumeAPI.Modularization;
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
				entity.Id = GetNextHighestId();

				EntityScope entityScope = ScopeRegistry[scope];
				entityScope.EntitiesInScope.Add(entity.Id, entity);
				entity.Scope = entityScope;

				Entities.Add(entity.Id, entity);
				return entity.Id;
			}
			throw new InvalidScopeException(scope);
		}

		public static BaseEntity UpdateOrCreateWithId(int id, EntityScope scope, string referencer, params object[] arguments) {
			BaseEntity entity;
			if(Entities.ContainsKey(id)) {
				entity = Entities[id];
				entity.UpdateFromMessage(arguments);
			} else {
				entity = ModuleController.CreateEntityByReferencer(referencer, arguments);
				entity.Id = id;
				RegisterEntity(scope.Name, entity);
			}
			return entity;
		}
		public static void UpdateFromId(int id, params object[] arguments) {
			if(Entities.ContainsKey(id)) {
				BaseEntity entity = Entities[id];
				entity.UpdateFromMessage(arguments);
			} else {
				Console.WriteLine("Entity with id " + id + " missing.");
			}
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
