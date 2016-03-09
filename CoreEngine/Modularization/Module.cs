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
using IronRuby.Builtins;

namespace CoreEngine.Modularization {
	public class Module {

		DirectoryInfo Directory;
		public string[] FolderScope;

		public ModuleDefinition Definition;

		CoreScript Startup;
		public RubyObject StartupInstance;

		public Module(string directory) {
			Directory = new DirectoryInfo(ModuleController.ModuleDirectory + "/" + directory);
			FolderScope = Directory.GetDirectories("*.*", SearchOption.AllDirectories).Select(x => x.FullName).ToArray();
			if(LoadDefinition()) {
				LoadModuleContents(this.Directory);
			}
		}
		
		private void LoadModuleContents(DirectoryInfo directory) {
			IEnumerable<FileInfo> rubyFiles = directory.GetFiles().Where(t => t.Extension == ".rb");
			foreach(FileInfo script in rubyFiles) {
				if(script.Name == Definition.StartupFile) {
					Startup = new CoreScript(script, this);
					Startup.Compile();
					StartupInstance = Startup.GetInstance<RubyObject>(new object[] { });
					TryInvokeStartupMethod("before_load", new object[] { });
				} else {
					ModuleController.RegisterEntity(this, script);
				}
			}
			TryInvokeStartupMethod("after_load", new object[] { });

			foreach(DirectoryInfo subDirectory in directory.GetDirectories()) {
				LoadModuleContents(subDirectory);
			}
		}

		public dynamic InvokeStartupMethod(string method, params object[] arguments) {
			if(Startup != null) {
				return Startup.InvokeMethod(StartupInstance, method, arguments);
			}
			return null;
		}

		public dynamic TryInvokeStartupMethod(string method, params object[] arguments) {
			if(Startup != null) {
				return Startup.TryInvokeMethod(StartupInstance, method, arguments);
			}
			return null;
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
