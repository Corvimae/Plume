using CoreEngine.Entities;
using CoreEngine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEngine.World {
	static class DrawQueue {
		static SortedDictionary<int, List<DrawOperation>> DrawOperations = new SortedDictionary<int, List<DrawOperation>>();
		
		private static void AddDrawAction(int layer, DynamicDelegate del, BaseEntity instance) {
			if(!DrawOperations.ContainsKey(layer)) {
				DrawOperations.Add(layer, new List<DrawOperation>());
			}
			DrawOperations[layer].Add(new DrawOperation(instance, del));
		}

		public static SortedDictionary<int, List<DrawOperation>> ProcessAndReturnDrawQueue() {
			DrawOperations.Clear();
			foreach(BaseEntity entity in EntityController.GetAllEntities()) {
				if(entity.HasPropertyEnabled("draw") && Camera.IsOnCamera(entity.GetDrawBoundry())) {
					Dictionary<int, DynamicDelegate> drawActions = entity.GetDrawActionRegistry();
					AddDrawAction(entity.GetDrawLayer(), new DynamicDelegate(Delegate.CreateDelegate(typeof(Action), entity, "Draw"), true, 0), entity);
					foreach(KeyValuePair<int, DynamicDelegate> entry in drawActions) {
						AddDrawAction(entry.Key, entry.Value, entity);
					}
				}
			}
			return DrawOperations;
		}
	}
	struct DrawOperation {
		public BaseEntity Entity;
		public DynamicDelegate Delegate;
		public DrawOperation(BaseEntity entity, DynamicDelegate del) {
			this.Entity = entity;
			this.Delegate = del;
		}
	}
}
