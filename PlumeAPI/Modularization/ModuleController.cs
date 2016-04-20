using System;
using System.Collections.Generic;
using System.Linq;
using PlumeAPI.Utilities;
using System.Diagnostics;
using PlumeAPI.Entities;

namespace PlumeAPI.Modularization {
	public static class ModuleController {
		#if WINDOWS
				public static string ModuleDirectory = AppDomain.CurrentDomain.BaseDirectory + "../../../../Modules";
		#else
				public static string ModuleDirectory = AppDomain.CurrentDomain.BaseDirectory + "../../../../Modules";
		#endif

		public static PlumeEnvironment Environment = PlumeEnvironment.Client;

		public static Dictionary<string, Module> ModuleRegistry = new Dictionary<string, Module>();

		private static DependencyGraph<Module> dependencies;

		public static void SetEnvironment(PlumeEnvironment environment) {
			Environment = environment;
			if(Environment == PlumeEnvironment.Server) {
				ModuleDirectory = AppDomain.CurrentDomain.BaseDirectory + "../../../Modules";
			}
		}
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

		public static bool IsModuleRegistered(string name, string version, string author) {
			return GetModules().Any(x => x.Definition.ModuleInfo.Name == name && x.Definition.ModuleInfo.Version == version && x.Definition.ModuleInfo.Author == author);
		}
		public static void ImportModules() {
			List<DependencyNode<Module>> processOrder = dependencies.GetProcessingOrder();
			foreach(DependencyNode<Module> module in processOrder) {
				Console.WriteLine("Importing script data for " + module.Item.Definition.ModuleInfo.Name);
				module.Item.BuildModule();
			}
			InvokeStartupMethod("AfterLoad");

		}

		public static IEnumerable<Module> GetModules() {
			return ModuleRegistry.Values;
		}

		public static void InvokeStartupMethod(string method, params object[] arguments) {
			foreach(Module module in ModuleController.ModuleRegistry.Values) {
				module.TryInvokeStartupMethod(method, arguments);
			}
		}
	} 

	public enum PlumeEnvironment {
		Client,
		Server
	}


	public class ModuleNotRegisteredException : Exception {
		public string ModuleName;
		public ModuleNotRegisteredException(string name) {
			this.ModuleName = name;
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
