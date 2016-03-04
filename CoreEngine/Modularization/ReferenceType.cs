using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json;

namespace CoreEngine.Modularization {
	class ReferenceType {
		private static Dictionary<string, ReferenceData> RegisteredReferences = new Dictionary<string, ReferenceData>();
		public static void RegisterReference(ReferenceData data) {
			RegisteredReferences.Add(data.Name, data);
		}

		public static ReferenceData FindReferenceData(string name) {
			KeyValuePair<string, ReferenceData> type = RegisteredReferences.FirstOrDefault(t => t.Key == name);
			if(type.Key != null) {
				return type.Value;
			}
			throw new InvalidReferencerException(name);
		}


		public static ReferenceData DeserializeMetadata(string rawJson, Type referenceType) {
			ReferenceData data = JsonConvert.DeserializeObject<ReferenceData>(rawJson);
			data.ReferenceType = referenceType;
			return data;
		}
	}
}
