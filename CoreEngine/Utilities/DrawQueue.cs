using CoreEngine.Entities;
using CoreEngine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreEngine.World;

namespace CoreEngine.Utilities {
	class DrawQueue {
		//By placing base .draw operations in a new queue, we can speed up the draw process considerably.
		private SortedDictionary<int, DrawLayer> Operations = new SortedDictionary<int, DrawLayer>();

		public DrawQueue() { }

		public DrawQueue(IComparer<int> comparer) {
			this.Operations = new SortedDictionary<int, DrawLayer>(comparer);

		}

		public void AddOperation(int layer, DynamicDelegate del, BaseEntity instance) {
			if(!Operations.ContainsKey(layer)) {
				Operations.Add(layer, new DrawLayer());
			}
			Operations[layer].DrawOperations.Add(new DrawQueueOperation(instance, del));
		}


		public void AddBaseDrawOperation(int layer, BaseEntity instance) {
			if(!Operations.ContainsKey(layer)) {
				Operations.Add(layer, new DrawLayer());
			}
			Operations[layer].BaseDrawEntities.Add(instance);
		}

		public SortedDictionary<int, DrawLayer> ProcessAndReturnDrawQueue() {
			Operations.Clear();
			foreach(BaseEntity entity in EntityController.GetAllEntities()) {
				if(entity.HasPropertyEnabled("draw") && Camera.IsOnCamera(entity.GetDrawBoundry())) {
					Dictionary<int, DynamicDelegate> drawActions = entity.GetDrawActionRegistry();
					AddBaseDrawOperation(entity.GetDrawLayer(), entity);
					foreach(KeyValuePair<int, DynamicDelegate> entry in drawActions) {
						AddOperation(entry.Key, entry.Value, entity);
					}
				}
			}
			return Operations;
		}

	}

	struct DrawQueueOperation {
		public BaseEntity Entity;
		public DynamicDelegate Delegate;
		
		public DrawQueueOperation(BaseEntity entity, DynamicDelegate del) {
			this.Entity = entity;
			this.Delegate = del;
		}
	}

	class DrawLayer {
		public List<BaseEntity> BaseDrawEntities;
		public List<DrawQueueOperation> DrawOperations;

		public DrawLayer() {
			this.BaseDrawEntities = new List<BaseEntity>();
			this.DrawOperations = new List<DrawQueueOperation>();
		}
	}
}
