using System;
using CoreEngine.Utilities;
using IronRuby.Builtins;
using CoreEngine.Modularization;

namespace CoreEngine.Events {
	public struct EventRequest {
		public DynamicDelegate Delegate;
		public object Instance;

		public EventRequest(RubySymbol method, CoreScript script, object instance) {
			Delegate = DynamicDelegate.CreateDelegateForRubyMethod(method, instance, script);
			Instance = instance;
		}

		public EventRequest(string method, object instance) {
			Delegate = DynamicDelegate.CreateDelegateForCSharpMethod(method, instance);
			Instance = instance;
		}
	}
}

