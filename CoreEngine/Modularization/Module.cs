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

		public CoreObject StartupInstance;

		Dictionary<string, CoreScript> Scripts = new Dictionary<string, CoreScript>();
		Dictionary<string, EntityRegistryRecord> EntityRegistry = new Dictionary<string, EntityRegistryRecord>();

		public Module(string directory) {
			Directory = new DirectoryInfo(ModuleController.ModuleDirectory + "/" + directory);
			LoadDefinition();
		}

		public void BuildModule() {
			LoadModuleClasses();
			TryInvokeStartupMethod("after_load", new object[] { });
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
					CoreScript startupScript = new CoreScript(script, this);
					startupScript.Compile();
					Scripts.Add(Path.GetFileNameWithoutExtension(script.Name), startupScript);
					StartupInstance = startupScript.GetInstance<CoreObject>(new object[] { });
					StartupInstance.SetReferenceScript(startupScript);
					StartupInstance.Metadata = new CoreObjectData();
					StartupInstance.Metadata.Module = this;
					startupScript.TryInvokeMethod(StartupInstance, "before_load", new object[] { });
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
				try {
					CoreScript entity = new CoreScript(file.Item, this);
					entity.Compile();
					Scripts.Add(Path.GetFileNameWithoutExtension(file.Item.Name), entity);

					string entityName = entity.ClassReference.Name;

					RubyClass entityType = entity.ClassReference.SuperClass;
					string entityTypeName = entityType.Name.Split(new string[] { "::" }, StringSplitOptions.None).Last();

					bool hasRegistered = false;

					if(entityTypeName != "BaseEntity") {
						string superTypeName;
						string baseParentTypeName = entityType.Name.Split(new string[] { "::" }, StringSplitOptions.None).Last();
						while(entityTypeName != "BaseClass" && entityType != null && !hasRegistered) {
							entityTypeName = entityType.Name.Split(new string[] { "::" }, StringSplitOptions.None).Last();
							superTypeName = entityType.SuperClass != null ? entityType.SuperClass.Name.Split(new string[] { "::" }, StringSplitOptions.None).Last() : null;
							if(superTypeName == "BaseEntity") {
								if(!EntityRegistry.ContainsKey(entityTypeName + "." + entityName)) {
									EntityData data = null;
									if(EntityRegistry.ContainsKey(entityTypeName + "." + baseParentTypeName)) {
										data = EntityRegistry[entityTypeName + "." + baseParentTypeName].Data.CreateImpartialClone();
									} else {
										EntityData recoveredData = ModuleController.FindEntityData(entityTypeName, baseParentTypeName);
										if(recoveredData != null) {
											data = recoveredData;
										} else {
											data = new EntityData();
										}
									}
									data.Name = entityName;
									data.EntityType = entityTypeName;
									data.Module = this;
									CoreScript script = ModuleController.FindEntityRecordByReferencer(data.GetReferencer());
									BaseEntity instance = script.GetInstance<BaseEntity>(new object[] { });
									instance.Metadata = data;
									instance.SetReferenceScript(entity);
									EntityRegistry.Add(entityType + "." + entityName, new EntityRegistryRecord(entity, data));

									script.InvokeMethod(instance, "register", new object[] { });
									hasRegistered = true;
									break;
								} else {
									throw new DuplicateEntityDefinitionException();
								}
							}
							entityType = entityType.SuperClass;
						}
					}
					if(!hasRegistered) {
						throw new InvalidEntityTypeException();
					}
				} catch(MemberAccessException e) {
					Debug.WriteLine("Unable to compile " + file.Item.Name + ": " + e.Message);
					Debug.WriteLine(e.ToString());
				} catch(Exception e) {
					Debug.WriteLine("An unexpected error occured while attempting to register the CoreScript entity " + file.Item.Name);
					Debug.WriteLine(e.ToString());
				}
			}
		}

		public CoreScript GetEntityRecord(string entityType, string entityName) {
			if(EntityRegistry.ContainsKey(entityType + "." + entityName)) {
					return EntityRegistry[entityType + "." + entityName].Script;
			} else {
				throw new EntityNotFoundException(entityType);
			}
		}

		public EntityData GetEntityData(string entityType, string entityName) {
			if(EntityRegistry.ContainsKey(entityType + "." + entityName)) {
				return EntityRegistry[entityType + "." + entityName].Data;
			} else {
				return null;
			}
		}

		public BaseEntity CreateEntityInstance(string entityType, string entityName, params object[] arguments) {
			try {
				CoreScript script = GetEntityRecord(entityType, entityName);
				BaseEntity instance = script.GetInstance<BaseEntity>();
				instance.Initialize(EntityRegistry[entityType + "." + entityName].Data, arguments);
				EntityController.RegisterEntityInstance(instance);
				return instance;
			} catch(MissingMethodException e) {
				Debug.WriteLine("Attempted to make entity of type " + entityType + "." + entityName + ", but a method was missing: " + e.Message);
				return null;
			}
		}


		public dynamic TryInvokeStartupMethod(string method, params object[] arguments) {
			if(Definition.StartupFile != null) {
				return Scripts[Path.GetFileNameWithoutExtension(Definition.StartupFile)].TryInvokeMethod(StartupInstance, method, arguments);
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

	struct EntityRegistryRecord {
		public CoreScript Script;
		public EntityData Data;

		public EntityRegistryRecord(CoreScript script, EntityData data) {
			this.Script = script;
			this.Data = data;
		}
	}

}
