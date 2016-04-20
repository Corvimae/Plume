using PlumeAPI.Entities.Components;
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

		public Dictionary<int, Dictionary<NetworkedPropertyReferencer, object>> Properties = new Dictionary<int, Dictionary<NetworkedPropertyReferencer, object>>();
		public ClientEntitySnapshot(long tick, long received) {
			Tick = tick;
			Received = received;
		}

		public void SetProperty(int id, NetworkedPropertyReferencer property, object value) {
			if(!Properties.ContainsKey(id)) {
				Properties.Add(id, new Dictionary<NetworkedPropertyReferencer, object>());
			}
			if(Properties[id].ContainsKey(property)) {
				Properties[id][property] = value;
			} else {
				Properties[id].Add(property, value);
			}
		}

		public int CompareTo(ClientEntitySnapshot obj) {
			return (int) (Received - obj.Received);
		}
	}
}
