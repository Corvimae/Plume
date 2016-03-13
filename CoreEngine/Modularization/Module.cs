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
using System.Text.RegularExpressions;
using CoreEngine.Entities;

namespace CoreEngine.Modularization {
	public class Module {

		DirectoryInfo Directory;

		public ModuleDefinition Definition;

		CoreScript Startup;
		public CoreObject StartupInstance;

		public Module(string directory) {
			Directory = new DirectoryInfo(ModuleController.ModuleDirectory + "/" + directory);
			LoadDefinition();
		}

		public void BuildModule() {
			LoadModuleClasses();
			Startup.TryInvokeMethod(StartupInstance, "after_load", new object[] { });
		}

		private void LoadModuleClasses() {
			DependencyGraph<FileInfo> dependencyGraph = new DependencyGraph<FileInfo>();
			IEnumerable<FileInfo> rubyFiles = System.IO.Directory.GetFiles(this.Directory.FullName, "*.rb", SearchOption.AllDirectories).Select(t => new FileInfo(t));

			//Find lines of syntax class [a] | OR | class [a] < [b]
			Regex classExtractor = new Regex(@"class ([A-Za-z]+)(?: < ([A-Za-z]+))?", RegexOptions.Multiline);
			Func<FileInfo, FileInfo, bool> FileInfoEqualityChecker = delegate (FileInfo existing, FileInfo file) {
				return existing.FullName == file.FullName;
			};

			foreach(FileInfo script in rubyFiles) {
				if(script.Name == Definition.StartupFile) {
					Startup = new CoreScript(script, this);
					Startup.Compile();
					StartupInstance = Startup.GetInstance<CoreObject>(new object[] { });
					StartupInstance.SetReferenceScript(Startup);
					StartupInstance.Metadata = new CoreObjectData();
					Startup.TryInvokeMethod(StartupInstance, "before_load", new object[] { });
				} else {
					//Get the extended class for this script
					DependencyNode<FileInfo> classNode = dependencyGraph.AddNode(script, FileInfoEqualityChecker);
					string rawScript = File.ReadAllText(script.FullName);
					Match classMatch = classExtractor.Match(rawScript);
					if(classMatch != null && classMatch.Groups.Count == 3) {
						string baseClass = classMatch.Groups[2].Value;
						FileInfo dependency = rubyFiles.FirstOrDefault(t => {
							return Path.GetFileNameWithoutExtension(t.Name) == baseClass;
						});
						//If the dependency is not found, it must belong to an outside source.
						if(dependency != null) {
							DependencyNode<FileInfo> baseNode = dependencyGraph.AddNode(dependency, FileInfoEqualityChecker);
							dependencyGraph.AddDependency(baseNode, classNode);
						}
					}
				}
			}

			List<DependencyNode<FileInfo>> processOrder = dependencyGraph.GetProcessingOrder();
			foreach(DependencyNode<FileInfo> file in processOrder) {
				ModuleController.RegisterEntity(this, file.Item);
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
			} catch(DirectoryNotFoundException) {
				Debug.WriteLine("Module " + Directory + " could not be found.");
			} catch(FileNotFoundException) {
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
