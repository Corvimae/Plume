using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronRuby.Builtins;
using CoreEngine.Modularization;
using IronRuby.Runtime.Calls;
using System.Reflection;

namespace CoreEngine.Utilities {
	public class DynamicDelegate {

		public dynamic Delegate;
		public bool IsCSharp;
		public int Arity;

		public DynamicDelegate(dynamic del, bool isCSharp, int arity) {
			Delegate = del;
			IsCSharp = isCSharp;
			Arity = arity;
		}

		public static DynamicDelegate CreateDelegateForRubyMethod(RubySymbol method, object instance, CoreScript reference) {
			RubyMethodInfo info = reference.Engine.Operations.GetMember(instance, (string)method.String).Info;
			var rawDelegate = info.GetType().InvokeMember(
				"GetDelegate",
				BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic,
				null,
				info,
				new object[] { }
			);
			return new DynamicDelegate(rawDelegate, false, info.GetArity());
		}

		public static DynamicDelegate CreateDelegateForCSharpMethod(string method, object instance) {
			System.Delegate del = System.Delegate.CreateDelegate(instance.GetType(), instance, method);
			return new DynamicDelegate(
				System.Delegate.CreateDelegate(instance.GetType(), instance, method), 
				true, 
				instance.GetType().GetMethod(method).GetParameters().Length
			);

		}
	}
}
