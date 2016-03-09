using CoreEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEngine.World {
	static class DrawQueue {
		static SortedDictionary<int, List<DrawOperation>> DrawOperations = new SortedDictionary<int, List<DrawOperation>>();
		
		private static void AddDrawAction(int layer, dynamic identifier, BaseEntity instance) {
			if(!DrawOperations.ContainsKey(layer)) {
				DrawOperations.Add(layer, new List<DrawOperation>());
			}
			DrawOperations[layer].Add(new DrawOperation(instance, identifier));
		}
		public static SortedDictionary<int, List<DrawOperation>> ProcessAndReturnDrawQueue() {
			DrawOperations.Clear();
			foreach(BaseEntity entity in EntityController.GetAllEntities()) {
				if(entity.HasPropertyEnabled("draw") && Camera.IsOnCamera(entity.GetDrawBoundry())) {
					Dictionary<int, dynamic> drawActions = entity.GetDrawActionRegistry();
					AddDrawAction(entity.GetDrawLayer(), "Draw", entity);
					foreach(KeyValuePair<int, dynamic> entry in drawActions) {
						AddDrawAction(entry.Key, entry.Value, entity);
					}
				}
			}
			return DrawOperations;
		}
	}
	struct DrawOperation {
		public BaseEntity Entity;
		public dynamic Identifier;
		public DrawOperation(BaseEntity entity, dynamic identifier) {
			this.Entity = entity;
			this.Identifier = identifier;
		}
	}
}
