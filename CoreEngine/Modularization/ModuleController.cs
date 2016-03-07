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

namespace CoreEngine.Modularization {
	static class ModuleControl {
		public static string ModuleDirectory = AppDomain.CurrentDomain.BaseDirectory + "../../../";

		public static Dictionary<string, Dictionary<string, CoreScript>> EntityRegistry = new Dictionary<string, Dictionary<string, CoreScript>>();

		public static Dictionary<string, Module> ModuleRegistry = new Dictionary<string, Module>();

		static ModuleControl() {
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
				return script.GetInstance(arguments);
			} catch (MissingMethodException e) {
				Debug.WriteLine("Attempted to make entity of type " + referencer, " but a method was missing: " + e.Message);
				return null;
			}
		}

		public static void RegisterEntity(FileInfo file) {
			try {
				CoreScript entity = new CoreScript(file);
				entity.Compile();

				string entityName = entity.ClassReference.Name;

				string entityType = entity.ClassReference.SuperClass.Name;
				entityType = entityType.Split(new string[] { "::" }, StringSplitOptions.None).Last();

				entity.SetStaticMember("SourceDirectory", entity.SourceFile.Directory);		
				entity.SetStaticMember("EntityType", entityType);
				entity.SetStaticMember("Name", entityName);

				if(entityType == "BaseEntity") {
					if(!EntityRegistry.ContainsKey(entityName)) {
						EntityRegistry.Add(entityName, new Dictionary<string, CoreScript>());
						Debug.WriteLine(entityName + " added as new Entity Type.");
					} else {
						throw new DuplicateEntityTypeDefinitionException();
					}
				} else {
					if(EntityRegistry.ContainsKey(entityType)) {
						EntityRegistry[entityType].Add(entityName, entity);
					} else {
						throw new InvalidEntityTypeException();
					}
				}
			} catch (MemberAccessException e) {
				Debug.WriteLine("Unable to compile " + file.Name + ": " + e.Message);
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
