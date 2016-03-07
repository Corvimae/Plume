using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEngine.Modularization {
	public class ModuleDefinition {

		[JsonProperty(PropertyName = "module")]
		public ModuleMetadata ModuleInfo;
	}
	public struct ModuleMetadata {
		public string Name;
		public string Version;
		public string Author;
	}
}
