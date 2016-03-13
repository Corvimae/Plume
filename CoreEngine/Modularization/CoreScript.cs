using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronRuby;
using System.IO;
using Microsoft.Scripting.Hosting;
using IronRuby.Runtime;
using IronRuby.Builtins;
using Microsoft.Scripting;
using System.Diagnostics;
using CoreEngine.Modularization;
using CoreEngine.Entities;
using CoreEngine.Scripting;

namespace CoreEngine.Modularization {
	public class CoreScript {
		public Module Module;
		public ScriptEngine Engine;
		protected string RawScript;

		private dynamic Scope;

		public FileInfo SourceFile;
		public RubyClass ClassReference;

		public CoreScript(FileInfo file, Module module) {
			this.Module = module;
			this.RawScript = File.ReadAllText(file.FullName);
			this.SourceFile = file;
			Engine = Ruby.CreateEngine();
			Engine.SetSearchPaths(new string[] { ModuleController.ModuleDirectory });
			Engine.Runtime.LoadAssembly(typeof(BaseEntity).Assembly);
		}

		public void Compile() {
			Scope = Engine.Execute(RawScript);
			ClassReference = (RubyClass) Engine.Runtime.Globals.GetItems().First(t => t.Key == Path.GetFileNameWithoutExtension(SourceFile.Name)).Value;
			Debug.WriteLine(ClassReference.Name + " registered.");
		}

		public T GetInstance<T>(params object[] arguments) {
			dynamic instanceVariable;
			var instanceVaraibleResult = Engine.Runtime.Globals.TryGetVariable(ClassReference.Name, out instanceVariable);
			if(!instanceVaraibleResult && instanceVariable == null) {
				throw new InvalidOperationException("Unable to find " + ClassReference.Name);
			}
			T entity = (T) Engine.Operations.CreateInstance(instanceVariable, arguments);
			return entity;
		}

		public void SetStaticMember(string name, object value) {
			Engine.Operations.SetMember(ClassReference, name, value);
		}

		public dynamic GetMember(object instance, string name) {
			return Engine.Operations.GetMember(instance, name);
		}

		public dynamic InvokeMethod(object instance, string name, params object[] arguments) {
 			return Engine.Operations.InvokeMember(instance, name, new object[] { });
		}

		public dynamic InvokeMethod(string name, params object[] arguments) {
			RubyObject instance = GetInstance<RubyObject>(new object[] { });
			return Engine.Operations.InvokeMember(instance, name, new object[] { });
		}

		public dynamic TryInvokeMethod(object instance, string name, params object[] arguments) {
			if(Engine.Operations.GetMember(instance, name) != null) {
				return InvokeMethod(instance, name, arguments);
			}
			return null;
		}
	}
}
