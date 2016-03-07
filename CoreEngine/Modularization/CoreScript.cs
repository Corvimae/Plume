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
	class CoreScript {
		protected ScriptEngine Engine;
		protected string RawScript;

		private dynamic Scope;

		public FileInfo SourceFile;
		public RubyClass ClassReference;

		public CoreScript(FileInfo file) {
			this.RawScript = File.ReadAllText(file.FullName);
			this.SourceFile = file;
			Engine = Ruby.CreateEngine();
			Engine.Runtime.LoadAssembly(typeof(BaseEntity).Assembly);
		}

		public void Compile() {
			Scope = Engine.Execute(RawScript);
			KeyValuePair<string, dynamic> classDefinition = Engine.Runtime.Globals.GetItems().First();
			ClassReference = (RubyClass)classDefinition.Value;
			Debug.WriteLine(ClassReference.Name + " registered.");
		}

		public BaseEntity GetInstance(params object[] arguments) {
			dynamic instanceVariable;
			var instanceVaraibleResult = Engine.Runtime.Globals.TryGetVariable(ClassReference.Name, out instanceVariable);
			if(!instanceVaraibleResult && instanceVariable == null) {
				throw new InvalidOperationException("Unable to find " + ClassReference.Name);
			}
			return (BaseEntity) Engine.Operations.CreateInstance(instanceVariable, arguments);
		}

		public void SetStaticMember(string name, object value) {
			Engine.Operations.SetMember(ClassReference, name, value);
		}
		/*	dynamic instanceVariable;
			var instanceVaraibleResult = Engine.Runtime.Globals.TryGetVariable(ClassReference.Name, out instanceVariable);
			if(!instanceVaraibleResult && instanceVariable == null) {
				throw new InvalidOperationException("Unable to find " + ClassReference.Name);
			}

			dynamic instance = Engine.Operations.CreateInstance(instanceVariable, arguments);

			return new CoreScriptInstance(instance, Engine);*/



	}
}
