using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CoreEngine.Utilities {
	static class TypeReflector {
		public static dynamic InvokeStaticTypeMethod(string method, Type type, Object[] arguments) {
			return type.InvokeMember(method, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, arguments);
		}

		public static dynamic InvokeMethod(object instance, string method, object[] arguments) {
			try {
				return instance.GetType().InvokeMember(
					method, 
					BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public, 
					null, 
					instance, 
					arguments
				);
			} catch (TargetInvocationException e) {
				System.Diagnostics.Debug.WriteLine(e.InnerException.ToString());
				return null;
			}
		}
	}
}
