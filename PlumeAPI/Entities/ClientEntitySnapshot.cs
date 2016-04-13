using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Entities {
	public class ClientEntitySnapshot : IComparable<ClientEntitySnapshot> {
		public long Tick { get; set; }
		public long Received { get; set; }

		public Dictionary<int, Dictionary<PropertyInfo, object>> Properties = new Dictionary<int, Dictionary<PropertyInfo, object>>();
		public ClientEntitySnapshot(long tick, long received) {
			Tick = tick;
			Received = received;
		}

		public void SetProperty(int id, PropertyInfo property, object value) {
			if(!Properties.ContainsKey(id)) {
				Properties.Add(id, new Dictionary<PropertyInfo, object>());
			}
			Properties[id].Add(property, value);
		}

		public int CompareTo(ClientEntitySnapshot obj) {
			return (int) (Received - obj.Received);
		}
	}
}
