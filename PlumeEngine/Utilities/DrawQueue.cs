using PlumeAPI.Entities;
using PlumeAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlumeAPI.World;

namespace PlumeEngine.Utilities {
	class DrawQueue {
		//By placing base .draw operations in a new queue, we can speed up the draw process considerably.
		private SortedDictionary<int, List<Action>> Operations = new SortedDictionary<int, List<Action>>();

		public DrawQueue() { }

		public DrawQueue(IComparer<int> comparer) {
			this.Operations = new SortedDictionary<int, List<Action>>(comparer);
		}

		public void AddOperation(int layer, Action action) {
			if(!Operations.ContainsKey(layer)) {
				Operations.Add(layer, new List<Action>());
			}
			Operations[layer].Add(action);
		}


		public SortedDictionary<int, List<Action>> ProcessAndReturnDrawQueue() {
			Operations.Clear();
			foreach(BaseEntity entity in EntityController.GetAllEntities()) {
				if(entity.HasPropertyEnabled("draw") && Camera.IsOnCamera(entity.GetDrawBoundry())) {
					AddOperation(entity.DrawLayer, (Action) entity.GetDelegate("Draw"));
					foreach(KeyValuePair<int, Action> entry in entity.GetDrawActionRegistry()) {
						AddOperation(entry.Key, entry.Value);
					}
				}
			}
			return Operations;
		}

	}
}
