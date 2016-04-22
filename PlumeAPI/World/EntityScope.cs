using Lidgren.Network;
using Microsoft.Xna.Framework;
using PlumeAPI.Entities;
using PlumeAPI.Events;
using PlumeAPI.Modularization;
using PlumeAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.World {
	public class EntityScope {
		public string Name;
		public Dictionary<int, BaseEntity> EntitiesInScope = new Dictionary<int, BaseEntity>();
		public Rectangle Boundry = new Rectangle(-999999999, -999999999, 999999999*2, 999999999*2);
		public EntityScope(string name) {
			this.Name = name;
			ScopeController.RegisterScope(name, this);
		}

		public void RegisterEntity(BaseEntity entity) {
			ScopeController.RegisterEntity(Name, entity);
		}

		public List<BaseEntity> GetEntities() {
			return EntitiesInScope.Values.ToList<BaseEntity>();
		}
		

		public Rectangle GetBoundry() {
			return Boundry;
		}

		public void Update() {
			EventController.Fire("update", this);
		}
	}
}
