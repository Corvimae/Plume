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
	}
}
