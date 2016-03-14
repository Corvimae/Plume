using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreEngine.Utilities;
using CoreEngine.Scripting;
using System.Diagnostics;
using System.IO;
using CoreEngine.Entities;
using System.Reflection;
using IronRuby.Builtins;

namespace CoreEngine.Modularization {
	static class ModuleController {
		#if WINDOWS
				public static string ModuleDirectory = AppDomain.CurrentDomain.BaseDirectory + "../../../Content/Modules";
		#else
				public static string ModuleDirectory = AppDomain.CurrentDomain.BaseDirectory + "../../../../../Content/Modules";
		#endif

		public static Dictionary<string, Module> ModuleRegistry = new Dictionary<string, Module>();

		private static DependencyGraph<Module> dependencies;

		public static void RegisterModule(string moduleName) {
			Module module = new Module(moduleName);
			ModuleRegistry.Add(moduleName, module);
		}

		public static void ResolveDependencies() {
			dependencies = new DependencyGraph<Module>();
			Func<Module, Module, bool> moduleEquality =
				(a, b) => a.Definition.ModuleInfo.Name == b.Definition.ModuleInfo.Name && a.Definition.ModuleInfo.Author == b.Definition.ModuleInfo.Author;
			foreach(Module module in ModuleRegistry.Values) {
				DependencyNode<Module> node = dependencies.AddNode(module, moduleEquality);
				if(module.Definition.Dependencies != null) {
					foreach(KeyValuePair<string, string> dependency in module.Definition.Dependencies) {
						string[] dependencyIdentifier = dependency.Key.Split('/');
						//Find a module that matches version, author, and name
						Module matchingModule = ModuleRegistry.Values.FirstOrDefault(a =>
							a.Definition.ModuleInfo.Author == dependencyIdentifier[0] &&
							a.Definition.ModuleInfo.Name == dependencyIdentifier[1] &&
							a.Definition.ModuleInfo.Version == dependency.Value
						);
						if(matchingModule != null) {
							DependencyNode<Module> parentNode = dependencies.AddNode(matchingModule, moduleEquality);
							dependencies.AddDependency(parentNode, node);
						} else {
							throw new MissingDependencyModuleException(dependency.Key, dependency.Value);
						}
					}
				}
			}
		}

		public static void ImportModules() {
			List<DependencyNode<Module>> processOrder = dependencies.GetProcessingOrder();
			foreach(DependencyNode<Module> module in processOrder) {
				Debug.WriteLine("Importing script data for " + module.Item.Definition.ModuleInfo.Name);
				module.Item.BuildModule();
			}
		}

		public static CoreScript FindEntityRecordByReferencer(string referenceString) {
			EntityReferencer referencer = ModuleController.ConvertStringToEntityReferencer(referenceString);
			if(ModuleRegistry.ContainsKey(referencer.Module)) {
				return ModuleRegistry[referencer.Module].GetEntityRecord(referencer.Type, referencer.Name);
			} else {
				throw new ModuleNotRegisteredException(referencer.Module);
			}
		}

		public static BaseEntity CreateEntityByReferencer(string referenceString, params object[] arguments) {
			EntityReferencer referencer = ModuleController.ConvertStringToEntityReferencer(referenceString);
			if(ModuleRegistry.ContainsKey(referencer.Module)) {
				return ModuleRegistry[referencer.Module].CreateEntityInstance(referencer.Type, referencer.Name, arguments);
			} else {
				throw new ModuleNotRegisteredException(referencer.Module);
			}
		}

		public static EntityReferencer ConvertStringToEntityReferencer(string referencer) {
			string[] referencerComponents = referencer.Split('.');
			if(referencerComponents.Length == 3) {
				return new EntityReferencer(referencerComponents[0], referencerComponents[1], referencerComponents[2]);
			} else {
				throw new InvalidReferencerException(referencer);
			}
		}
	}

	public struct EntityReferencer {
		public string Module;
		public string Type;
		public string Name;
		public EntityReferencer(string module, string type, string name) {
			this.Module = module;
			this.Type = type;
			this.Name = name;
		}

		public string GetReferencer() {
			return Module + "." + Type + "." + Name;
		}
	}
	public class InvalidEntityTypeException : Exception {
	}
	public class DuplicateEntityTypeDefinitionException : Exception {
	}
	public class ModuleNotRegisteredException : Exception {
		public string ModuleName;
		public ModuleNotRegisteredException(string name) {
			this.ModuleName = name;
		}
	}
	public class EntityNotFoundException : Exception {
		public string EntityName;
		public EntityNotFoundException(string name) {
			this.EntityName = name;
		}
	}
	public class EntityTypeNotFoundException : Exception {
		public string EntityTypeName;
		public EntityTypeNotFoundException(string name) {
			this.EntityTypeName = name;
		}
	}
	public class InvalidReferencerException : Exception {
		public string Referencer;
		public InvalidReferencerException(string referencer) {
			this.Referencer = referencer;
		}
	}
	public class MissingDependencyModuleException : Exception {
		public string Identifier;
		public string Version;
		public MissingDependencyModuleException(string identifier, string version) {
			Identifier = identifier;
			Version = version;
		}
	}
}
