using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CoreEngine.Utilities;

namespace CoreEngine.Modularization {
	class Module {

		DirectoryInfo directory;
		public ModuleDefinition Definition;
		public Module(string directory) {
			this.directory = new DirectoryInfo(ModuleControl.ModuleDirectory + "/" + directory);
			if(LoadDefinition()) {
				LoadModuleContents();
			}
		}
		
		private bool LoadModuleContents() {
			foreach(DirectoryInfo subDirectory in directory.GetDirectories()) {
				//Determine what type of object this is
				Type moduleType = ModuleControl.MatchDirectoryToObjectType(subDirectory.Name);
				if(moduleType != null) {
					//Find all subdirectories in this directory, and process them each.
					foreach (DirectoryInfo entityFolder in subDirectory.GetDirectories()) {
						ReferenceData typeData = (ReferenceData) TypeReflector.InvokeStaticTypeMethod("DeserializeMetadata", moduleType, new Object[] {
							File.ReadAllText(entityFolder.FullName + "/" + entityFolder.Name + ".json"), moduleType
						});
						typeData.Module = this;
						typeData.Process();
						TypeReflector.InvokeStaticTypeMethod("RegisterReference", moduleType, new object[] { typeData });
						Log("Registered type " + typeData.GetReferencer());
					}
				} else {
					LogError("Unknown type repository \"" + subDirectory.Name + "\"");
					return false;
				}
			}
			return true;
		}

		private bool LoadDefinition() {
			try {
				this.Definition = JsonConvert.DeserializeObject<ModuleDefinition>(File.ReadAllText(directory.FullName + "/module.json"));
				Log("initialized.");
				return true;
			} catch (DirectoryNotFoundException) {
				Debug.WriteLine("Module " + directory + " could not be found.");
			} catch (FileNotFoundException) {
				Debug.WriteLine("module.json missing for module " + directory);
			} catch(JsonSerializationException) {
				Debug.WriteLine("Failed to serialize module " + directory);
			}
			return false;
		}


		private void LogError(object error) {
			Debug.WriteLine("Error in module " + Definition.ModuleInfo.Name + ": " + error.ToString());
		}

		private void Log(object message) {
			Debug.WriteLine("Module " + Definition.ModuleInfo.Name + ": " + message.ToString());
		}
	}

}
