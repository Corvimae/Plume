using System;
using System.Collections.Generic;
using PlumeAPI.Utilities;

namespace PlumeAPI.Events {
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

