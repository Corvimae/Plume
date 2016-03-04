using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEngine.Modularization {
	class ModuleDefinition {

		[JsonProperty(PropertyName = "module")]
		public ModuleMetadata ModuleInfo;
	}
	struct ModuleMetadata {
		public string Name;
		public string Version;
	}
}
