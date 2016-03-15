using System;
using System.Collections.Generic;
using System.Linq;
using CoreEngine.Utilities;
using System.Diagnostics;
using CoreEngine.Entities;

namespace CoreEngine.Modularization {
	static class ModuleController {
		#if WINDOWS
				public static string ModuleDirectory = AppDomain.CurrentDomain.BaseDirectory + "../../../../Modules";
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

		public static BaseEntity CreateEntityByReferencer(string referenceString, params object[] arguments) {
			EntityReferencer referencer = ModuleController.ConvertStringToEntityReferencer(referenceString);
			if(ModuleRegistry.ContainsKey(referencer.Module)) {
				BaseEntity entity = ModuleRegistry[referencer.Module].GetInstance(referencer.Name, arguments);
				EntityController.EntityInstances.Add(entity);
				return entity;
			} else {
				throw new ModuleNotRegisteredException(referencer.Module);
			}
		}

		public static EntityReferencer ConvertStringToEntityReferencer(string referencer) {
			string[] referencerComponents = referencer.Split(new string[] { "." }, StringSplitOptions.None);
			if(referencerComponents.Length >= 2) {
				return new EntityReferencer(referencerComponents[0], String.Join(".", referencerComponents.Skip(1)));
			} else {
				throw new InvalidReferencerException(referencer);
			}
		}
	} 

	public struct EntityReferencer {
		public string Module;
		public string Name;
		public EntityReferencer(string module, string name) {
			this.Module = module;
			this.Name = name;
		}

		public string GetReferencer() {
			return Module + "::" + Name;
		}
	}
	public class InvalidEntityTypeException : Exception {
	}
	public class DuplicateEntityDefinitionException : Exception {
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
