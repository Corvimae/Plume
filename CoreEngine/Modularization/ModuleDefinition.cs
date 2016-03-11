using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEngine.Modularization {
	public class ModuleDefinition {

		[JsonProperty(PropertyName = "module")]
		public ModuleMetadata ModuleInfo;

		[JsonProperty(PropertyName = "startup")]
		public string StartupFile;

		[JsonProperty(PropertyName = "requires")]
		public Dictionary<string, string> Dependencies;
	}
	public struct ModuleMetadata {
		public string Name;
		public string Version;
		public string Author;
	}
}
