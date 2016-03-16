using PlumeAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.World {
	public class EntityScope {
		public string Name;
		private static int NextHighestId = 0;
		public Dictionary<int, BaseEntity> EntitiesInScope = new Dictionary<int, BaseEntity>();

		public EntityScope(string name) {
			this.Name = name;
		}

		public int RegisterEntity(BaseEntity entity) {
			EntitiesInScope.Add(NextHighestId++, entity);
			return NextHighestId - 1;
		}

		public List<BaseEntity> GetEntities() {
			return EntitiesInScope.Values.ToList<BaseEntity>();
		}
	}
}
