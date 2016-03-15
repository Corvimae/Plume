using System;
using System.Collections.Generic;
using CoreEngine.Utilities;

namespace CoreEngine.Events {
	public class EventData : Dictionary<string, object> {
		public bool ContinuePropagating = true;

		public EventData() : base() { }
		public EventData(Dictionary<string, object> items) : base(items) {
		}


		public void EndPropagation() {
			ContinuePropagating = false;
		}
	}
}

