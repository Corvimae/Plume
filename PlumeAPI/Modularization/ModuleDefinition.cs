using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlumeAPI.Modularization {
	public class ModuleDefinition {

		[JsonProperty(PropertyName = "module")]
		public ModuleMetadata ModuleInfo;

		[JsonProperty(PropertyName = "startup")]
		public string StartupClass;

		[JsonProperty(PropertyName = "requires")]
		public Dictionary<string, string> Dependencies;

		public string GetFullName() {
			if(ModuleInfo.FullName != null) return ModuleInfo.FullName;
			return ModuleInfo.Name;
		}
	}
	public struct ModuleMetadata {
		public string Name;
		public string Version;
		public string Author;
		public string FullName;
	}
}
