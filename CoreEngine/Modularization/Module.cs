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
	public class Module {

		DirectoryInfo Directory;

		public ModuleDefinition Definition;
		public Module(string directory) {
			this.Directory = new DirectoryInfo(ModuleControl.ModuleDirectory + "/" + directory);
			if(LoadDefinition()) {
				LoadModuleContents(this.Directory);
			}
		}
		
		private void LoadModuleContents(DirectoryInfo directory) {
			IEnumerable<FileInfo> rubyFiles = directory.GetFiles().Where(t => t.Extension == ".rb");
			foreach(FileInfo script in rubyFiles) {
				ModuleControl.RegisterEntity(script);
			}
			foreach(DirectoryInfo subDirectory in directory.GetDirectories()) {
				LoadModuleContents(subDirectory);
			}
		}

		private bool LoadDefinition() {
			try {
				this.Definition = JsonConvert.DeserializeObject<ModuleDefinition>(File.ReadAllText(Directory.FullName + "/module.json"));
				Log("Module definition file loaded successfully.");
				return true;
			} catch (DirectoryNotFoundException) {
				Debug.WriteLine("Module " + Directory + " could not be found.");
			} catch (FileNotFoundException) {
				Debug.WriteLine("module.json missing for module " + Directory);
			} catch(JsonSerializationException) {
				Debug.WriteLine("Failed to serialize module " + Directory);
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
