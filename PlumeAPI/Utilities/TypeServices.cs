using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PlumeAPI.Utilities {
	static class TypeServices {
		public static dynamic InvokeStaticTypeMethod(string method, string typeName, params object[] arguments) {
			Type type = Type.GetType(typeName);
			return type.InvokeMember(method, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, arguments);
		}
		public static dynamic InvokeStaticTypeMethod(string method, Type type, params object[] arguments) {
			return type.InvokeMember(method, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, arguments);
		}

		public static dynamic TryInvokeStaticTypeMethod(string method, Type type, params object[] arguments) {
			if(type.GetMethod(method, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static) != null) {
				return type.InvokeMember(method, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, arguments);
			}
			return null;
		}

		public static object InvokeWithNamedParameters(this MethodBase method, object obj, IDictionary<string, object> parameters) {
			return method.Invoke(obj, MapParameters(method, parameters));
		}

		public static object[] MapParameters(MethodBase method, IDictionary<string, object> parameters) {
			string[] parameterNames = method.GetParameters().Select(p => p.Name).ToArray();
			object[] parametersOut = new object[parameterNames.Length];
			for(int i = 0; i < parameterNames.Length; i++) {
				parametersOut[i] = Type.Missing;
			}

			foreach(KeyValuePair<string, object> pair in parameters) {
				parametersOut[Array.IndexOf(parameterNames, pair.Key)] = pair.Value;
			}

			return parametersOut;
		}

		public static dynamic InvokeMethod(object instance, string method, params object[] arguments) {
			try {
				return instance.GetType().InvokeMember(
					method,
					BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public,
					null,
					instance,
					arguments
				);
			} catch(TargetInvocationException e) {
				Console.WriteLine(e.InnerException.ToString());
				return null;
			}
		}
	}
}
