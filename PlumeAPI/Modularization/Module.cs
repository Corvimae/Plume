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
using PlumeAPI.Entities.Components;

namespace PlumeAPI.Modularization {
	public class Module {

		internal DirectoryInfo Directory;

		public ModuleDefinition Definition;

		public Assembly DLL;

		PlumeModule BaseLogic;

		public Module(string name) {
			Directory = new DirectoryInfo(ModuleController.ModuleDirectory + "/" + name);
			LoadDefinition();
		}

		public void BuildModule() {
			DLL = Assembly.LoadFile(Directory.FullName + "/" + Definition.ModuleInfo.Name + ".dll");
			foreach(Type type in DLL.GetTypes()) {
				if(type.FullName == Definition.StartupClass) {
					BaseLogic = (PlumeModule) Activator.CreateInstance(type);
					BaseLogic.Module = this;
					BaseLogic.Register();
				}
			}
		}

		public void LoadEntitiesFromFile(string filePath) {
			JContainer entityDataContainer = JsonConvert.DeserializeObject<JContainer>(File.ReadAllText(Directory.FullName + "/" + filePath));
			if(entityDataContainer is JObject) { 
				JObject entityData = (JObject)entityDataContainer;
				BuildEntity(entityData, filePath);
			} else if(entityDataContainer is JArray) {
				JArray entityDataArray = (JArray)entityDataContainer;
				foreach(JObject entityData in entityDataArray.Children<JObject>()) {
					BuildEntity(entityData, filePath);
				}
			} else {
				Console.WriteLine("Entities in file " + filePath + " could not be parsed.");
			}
		}

		public void LoadEntitiesFromDirectory(string directoryPath) {
			DirectoryInfo directory = new DirectoryInfo(Directory.FullName + "/" + directoryPath);
			foreach(FileInfo file in directory.GetFiles("*.json")) {
				LoadEntitiesFromFile(file.FullName);
			}
		}

		private void BuildEntity(JObject entityData, string filename) {
			BaseEntity newEntity = new BaseEntity();
			if(entityData["name"] != null) {
				newEntity.Name = (string) entityData["name"];
				newEntity.Prototypal = true;
				if(entityData["components"] != null) {
					JArray components = (JArray)entityData["components"];
					foreach(JObject component in components) {
						if(component["component"] != null) {
							Dictionary<string, object> properties = new Dictionary<string, object>();
							properties.Add("entity", newEntity);
							foreach(JProperty property in component.Properties()) {
								if(property.Name != "component") {
									properties.Add(property.Name, property.Value);
								}
							}
							string componentName = component["component"] + "Component";
							Type componentType = null;
							foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
								Type type = assembly.GetTypes().FirstOrDefault(x=> x.Name == componentName);
								if(type != null) componentType = type;
							}
							if(componentType != null) {
								EntityComponent newComponent = (EntityComponent)Activator.CreateInstance(componentType, MapJTokensToParameters(componentType.GetConstructors()[0], properties));
								newEntity.AddComponent(newComponent);
							} else {
								throw new InvalidEntityException(filename, entityData["name"] + " contains component \"" + componentName + "\", which was not found in any installed modules.");
							}
						} else {
							throw new InvalidEntityException(filename, entityData["name"] + " contains an unnamed component, which could not be handled.");
						}
					}
					EntityController.RegisterPrototype(newEntity);
				}
			} else {
				throw new InvalidEntityException(filename, "Entities must have a name.");
			}
		}
		private object[] MapJTokensToParameters(MethodBase method, IDictionary<string, object> parameters) {
			string[] parameterNames = method.GetParameters().Select(p => p.Name).ToArray();
			Type[] parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
			object[] parametersOut = new object[parameterNames.Length];
			for(int i = 0; i < parameterNames.Length; i++) {
				parametersOut[i] = Type.Missing;
			}

			foreach(KeyValuePair<string, object> pair in parameters) {
				int index = Array.IndexOf(parameterNames, pair.Key);
				if(pair.Value is JToken) {
					parametersOut[index] = ((JToken)pair.Value).ToObject(parameterTypes[index]);
				} else {
					parametersOut[index] = pair.Value;
				}
			}

			return parametersOut;
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


		private bool LoadDefinition() {
			try {
				this.Definition = JsonConvert.DeserializeObject<ModuleDefinition>(File.ReadAllText(Directory.FullName + "/module.json"));
				Log("Module definition file loaded successfully.");
				return true;
			} catch(DirectoryNotFoundException) {
				Console.WriteLine("Module " + Directory + " could not be found.");
			} catch(FileNotFoundException) {
				Console.WriteLine("module.json missing for module " + Directory);
			} catch(JsonSerializationException) {
				Console.WriteLine("Failed to serialize module " + Directory);
			}
			return false;
		}

		private void LogError(object error) {
			Console.WriteLine("Error in module " + Definition.ModuleInfo.Name + ": " + error.ToString());
		}

		private void Log(object message) {
			Console.WriteLine("Module " + Definition.ModuleInfo.Name + ": " + message.ToString());
		}
	}

	public class InvalidEntityException : Exception {
		string _filename;
		string _message;
		public InvalidEntityException(string filename, string message) {
			_filename = filename;
			_message = message;
		}

		public override string ToString() {
			return "Invalid entity in " + _filename + ": " + _message;
		}
	}


}
