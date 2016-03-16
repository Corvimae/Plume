using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlumeAPI.Utilities;
using System.Text.RegularExpressions;
using PlumeAPI.Entities;
using PlumeAPI.Attributes;

namespace PlumeAPI.Modularization {
	public class Module {

		internal DirectoryInfo Directory;

		public ModuleDefinition Definition;

		public Assembly DLL;

		PlumeModule BaseLogic;

		Dictionary<string, Type> TypeRegistry = new Dictionary<string, Type>();

		public Module(string name) {
			Directory = new DirectoryInfo(ModuleController.ModuleDirectory + "/" + name);
			LoadDefinition();
		}

		public void BuildModule() {
			DLL = Assembly.LoadFile(Directory.FullName + "/" + Definition.ModuleInfo.Name + ".dll");
			foreach(Type type in DLL.GetTypes()) {
				string typeName = String.Join(".", type.FullName.Split('.').Skip(1));
				TypeRegistry.Add(typeName, type);

				TypeServices.TryInvokeStaticTypeMethod("SetModuleData", type, new object[] { this, typeName });

				if(type.FullName == Definition.StartupClass) {
					BaseLogic = GetInstance(type.Name);
				}

				foreach(MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public).
					Where(x => x.GetCustomAttributes().Any(attr => attr.GetType().Name == "RunOnLoadAttribute"))) {
					RunOnLoadAttribute attribute = (RunOnLoadAttribute) method.GetCustomAttribute(typeof(RunOnLoadAttribute));
					if(!attribute.Exceptions.Any(x => x == type.Name)) {
						TypeServices.TryInvokeStaticTypeMethod(method.Name, type, new object[] { });
					}
				}
			}
			TryInvokeStartupMethod("AfterLoad");
		}																 

		public dynamic TryInvokeStartupMethod(string methodName, params object[] arguments) {
			if(BaseLogic != null) {
				MethodInfo methodInfo = BaseLogic.GetType().GetMethod(methodName);
				if(methodInfo != null) {
					return methodInfo.Invoke(BaseLogic, arguments);
				}
			}
			return null;
		}

		public dynamic GetInstance(string typeName, params object[] arguments) {
			if(TypeRegistry.ContainsKey(typeName)) {
				return Activator.CreateInstance(TypeRegistry[typeName], arguments);
			} else {
				throw new EntityTypeNotFoundException(typeName);
			}
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
