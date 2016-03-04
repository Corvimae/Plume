using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreEngine.Utilities;

namespace CoreEngine.Modularization {
	static class ModuleControl {
		public static string ModuleDirectory = AppDomain.CurrentDomain.BaseDirectory + "../../../";

		public static Dictionary<string, Type> ExtendableTypes = new Dictionary<string, Type> {
			{ "tiles", Type.GetType("CoreEngine.World.MapTile") }
		};

		public static Type MatchDirectoryToObjectType(string DirectoryName) {
			KeyValuePair<string, Type> foundType = ExtendableTypes.FirstOrDefault(t => t.Key == DirectoryName);
			if(foundType.Key != null) {
				return foundType.Value;
			}
			return null;
		}
		public static string MatchObjectTypeToDirectory(Type type) {
			KeyValuePair<string, Type> foundType = ExtendableTypes.FirstOrDefault(t => t.Value == type);
			if (foundType.Value != null) {
				return foundType.Key;
			}
			return null;
		}

		public static ReferenceData FindDataByReferencer(string referencer) {
			string[] referencerComponents = referencer.Split('.');
			if (referencerComponents.Length == 2) {
				string className = referencerComponents[0];
				string typeName = referencerComponents[1];
				System.Diagnostics.Debug.WriteLine(ExtendableTypes.FirstOrDefault(t => t.Key == "tiles").Value.Name);
				Type type = (Type) ExtendableTypes.Values.FirstOrDefault(t => t.Name == className);
				if (type != null) {
					return TypeReflector.InvokeStaticTypeMethod("FindReferenceData", type, new object[] { typeName });
				} else {
					throw new ReferenceTypeNotFoundException(className);
				}
			} else {
				throw new InvalidReferencerException(referencer);
			}
		}
	}

	public class ReferenceTypeNotFoundException : Exception {
		public string ModularTypeName;
		public ReferenceTypeNotFoundException(string name) {
			this.ModularTypeName = name;
		}
	}
	public class InvalidReferencerException : Exception {
		public string Referencer;
		public InvalidReferencerException(string referencer) {
			this.Referencer = referencer;
		}
	}
}
