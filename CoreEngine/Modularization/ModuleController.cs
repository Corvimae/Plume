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
		public static string ModuleDirectory = AppDomain.CurrentDomain.BaseDirectory + "../../../";

		public static Dictionary<string, Dictionary<string, CoreScript>> EntityRegistry = new Dictionary<string, Dictionary<string, CoreScript>>();
		public static Dictionary<string, EntityData> EntityDataRegistry = new Dictionary<string, EntityData>();

		public static Dictionary<string, Module> ModuleRegistry = new Dictionary<string, Module>();


		static ModuleController() {
			RegisterBuiltinTypes();
		}

		public static void RegisterModule(string moduleName) {
			Module module = new Module(moduleName);
			ModuleRegistry.Add(moduleName, module);

		}

		public static CoreScript FindEntityRecordByReferencer(string referencer) {
			string[] referencerComponents = referencer.Split('.');
			if(referencerComponents.Length == 2) {
				string entityType = referencerComponents[0];
				string entityName = referencerComponents[1];
				if(EntityRegistry.ContainsKey(entityType)) {
					if(EntityRegistry[entityType].ContainsKey(entityName)) {
						return EntityRegistry[entityType][entityName];
					} else {
						throw new EntityTypeNotFoundException(entityName);
					}
				} else {
					throw new EntityTypeNotFoundException(entityType);
				}
			} else {
				throw new InvalidReferencerException(referencer);
			}
		}

		public static BaseEntity CreateEntityByReferencer(string referencer, params object[] arguments) {
			try {
				CoreScript script = FindEntityRecordByReferencer(referencer);
				BaseEntity instance = script.GetInstance<BaseEntity>(arguments);
				instance.Initialize(EntityDataRegistry[referencer]);
				EntityController.RegisterEntityInstance(instance);
				return instance;
			} catch (MissingMethodException e) {
				Debug.WriteLine("Attempted to make entity of type " + referencer, " but a method was missing: " + e.Message);
				return null;
			}
		}

		public static void RegisterEntity(Module module, FileInfo file) {
			try {
				CoreScript entity = new CoreScript(file, module);
				entity.Compile();

				string entityName = entity.ClassReference.Name;

				RubyClass entityType = entity.ClassReference.SuperClass;
				string entityTypeName = entityType.Name.Split(new string[] { "::" }, StringSplitOptions.None).Last();

				bool hasRegistered = false;

				if(entityTypeName == "BaseEntity") {
					if(!EntityRegistry.ContainsKey(entityName)) {
						EntityRegistry.Add(entityName, new Dictionary<string, CoreScript>());
						Debug.WriteLine(entityName + " added as new Entity Type.");
						hasRegistered = true;
					} else {
						throw new DuplicateEntityTypeDefinitionException();
					}
				} else {
					string superTypeName;
					string baseParentTypeName = entityType.Name.Split(new string[] { "::" }, StringSplitOptions.None).Last();
					while(entityTypeName != "BaseClass" && entityType != null && !hasRegistered) {
						entityTypeName = entityType.Name.Split(new string[] { "::" }, StringSplitOptions.None).Last();
						superTypeName = entityType.SuperClass.Name.Split(new string[] { "::" }, StringSplitOptions.None).Last();
						if(superTypeName == "BaseEntity") {
							if(EntityRegistry.ContainsKey(entityTypeName)) {
								EntityRegistry[entityTypeName].Add(entityName, entity);
								EntityData data = null;
								if(EntityDataRegistry.ContainsKey(entityTypeName + "." + baseParentTypeName)) {
									data = EntityDataRegistry[entityTypeName + "." + baseParentTypeName].CreateImpartialClone();
								} else {
									data = new EntityData();
								}
								data.Name = entityName;
								data.EntityType = entityTypeName;
								data.SourceDirectory = entity.SourceFile.Directory;
								EntityDataRegistry.Add(data.GetReferencer(), data);
								CoreScript script = ModuleController.FindEntityRecordByReferencer(data.GetReferencer());
								BaseEntity instance = script.GetInstance<BaseEntity>(new object[] { });
								instance.Metadata = data;
								script.InvokeMethod(instance, "register", new object[] { });
								hasRegistered = true;
							} else {
								throw new InvalidEntityTypeException();
							}
						}
						entityType = entityType.SuperClass;
					}
				}
				if(!hasRegistered) {
					throw new InvalidEntityTypeException();
				}
			} catch (MemberAccessException e) {
				Debug.WriteLine("Unable to compile " + file.Name + ": " + e.Message);
				Debug.WriteLine(e.ToString());
			} catch(Exception e) {
				Debug.WriteLine("An unexpected error occured while attempting to register the CoreScript entity " + file.Name);
				Debug.WriteLine(e.ToString());
			}
		}

		private static void RegisterBuiltinTypes() {
			foreach(Type type in Assembly.GetAssembly(typeof(BaseEntity)).GetTypes().Where(t => t.IsSubclassOf(typeof(BaseEntity)))) {
				EntityRegistry.Add(type.Name, new Dictionary<string, CoreScript>());
			}
		}
	}
	public class InvalidEntityTypeException : Exception {
	}
	public class DuplicateEntityTypeDefinitionException : Exception {
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
}
