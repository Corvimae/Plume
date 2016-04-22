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
			Directory = new DirectoryInfo(Path.Combine(ModuleController.ModuleDirectory, name));
			LoadDefinition();
		}

		public void BuildModule() {
			DLL = Assembly.LoadFile(Path.Combine(Directory.FullName, Definition.ModuleInfo.Name + ".dll"));
			foreach(Type type in DLL.GetTypes()) {
				if(type.FullName == Definition.StartupClass) {
					BaseLogic = (PlumeModule)Activator.CreateInstance(type);
					BaseLogic.Module = this;
					BaseLogic.Register();
				}
			}
		}

		public void LoadEntitiesFromFile(string filePath) {
			JContainer entityDataContainer = JsonConvert.DeserializeObject<JContainer>(File.ReadAllText(Path.Combine(Directory.FullName, filePath)));
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

			if(entityData["extends"] != null) {
				newEntity = EntityController.GetEntityPrototypeByName((string)entityData["extends"]).Clone();
				newEntity.Definition.Merge(entityData, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Merge });
			} else {
				newEntity.Definition = entityData;
			}

			entityData = newEntity.Definition;

			if(entityData["name"] != null) {
				newEntity.Name = (string)entityData["name"];
				newEntity.Prototypal = true;
				if(entityData["components"] != null) {
					JObject componentList = (JObject)entityData["components"];
					foreach(JProperty component in componentList.Properties()) {
						EntityComponent newComponent = BuildComponent(component.Name, (JObject)component.Value, newEntity);
						if(newEntity.HasComponent(newComponent.GetType().Name)) {
							newEntity.RemoveComponent(newComponent.GetType().Name);
						}
						newEntity.AddComponent(newComponent);
					}
					EntityController.RegisterPrototype(newEntity);
				}
			} else {
				throw new InvalidEntityException("Entities must have a name.");
			}
		}

		private EntityComponent BuildComponent(string name, JObject componentData, BaseEntity entity) {
			ComponentDefinition properties = new ComponentDefinition() { { "entity", entity } };
			foreach(JProperty property in componentData.Properties()) {
				properties.Add(property.Name, ConvertComponentItem(property.Value));
			}

			string componentName = name.Contains("Component") ? name : name + "Component";
			Type componentType = null;
			foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				Type type = assembly.GetTypes().FirstOrDefault(x => x.Name == componentName);
				if(type != null) componentType = type;
			}

			if(componentType != null) {
				return (EntityComponent)Activator.CreateInstance(componentType, MapToParameters(componentType.GetConstructors()[0], properties));
			} else {
				throw new InvalidEntityException(entity.Name + " contains component \"" + componentName + "\", which was not found in any installed modules.");
			}
		}

		private object ConvertComponentItem(JToken item) {
			switch(item.Type) {
				case JTokenType.Object:
					ComponentDefinition result = new ComponentDefinition();
					foreach(JProperty property in ((JObject) item).Properties()) {
						result.Add(property.Name, ConvertComponentItem(property.Value));
					}
					return result;
				case JTokenType.Array:
					List<object> subArray = new List<object>();
					foreach(JToken arrayItem in ((JArray)item).Values()) {
						subArray.Add(ConvertComponentItem(arrayItem));
					}
					return subArray.ToArray();
				case JTokenType.Boolean:
					return (bool)item;
				case JTokenType.Float:
					return (float)item;
				case JTokenType.Integer:
					return (int)item;
				case JTokenType.String:
					return (string)item;
				default:
					return item;
			}
		}
		private object[] MapToParameters(MethodBase method, IDictionary<string, object> parameters) {
			string[] parameterNames = method.GetParameters().Select(p => p.Name).ToArray();
			Type[] parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
			object[] parametersOut = new object[parameterNames.Length];
			for(int i = 0; i < parameterNames.Length; i++) {
				parametersOut[i] = Type.Missing;
			}

			foreach(KeyValuePair<string, object> pair in parameters) {
				int index = Array.IndexOf(parameterNames, pair.Key);
				parametersOut[index] = pair.Value;
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

	public class ComponentDefinition : Dictionary<string, object> { }

	public class InvalidEntityException : Exception {
		string _message;
		public InvalidEntityException(string message) {
			_message = message;
		}

		public override string ToString() {
			return "Invalid entity: " + _message;
		}
	}


}
