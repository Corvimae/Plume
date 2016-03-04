using CoreEngine.Lua;
using CoreEngine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CoreEngine.Modularization {
	class ReferenceData {
		public string Name;
		public Type ReferenceType;
		public string TypeDirectory;
		public Module Module;

		private string texturePath;
		public Texture2D Texture;

		[JsonProperty(PropertyName = "scripts")]
		public string[] ScriptFiles;
		private Script CompiledScript;

		public ReferenceData(string name, Type type) {
			this.Name = name;
			this.ReferenceType = type;
		}

		public void Process() {
			TypeDirectory = ModuleControl.MatchObjectTypeToDirectory(ReferenceType);
			GraphicsDevice graphics = GameServices.GetService<GraphicsDevice>();
			if(texturePath == null) {
				texturePath = Name + ".png";
			}
			Texture = LoadTexture(graphics, texturePath);

			LoadScripts();
		}

		private Texture2D LoadTexture(GraphicsDevice graphics, string path) {
			try {
				FileStream fileStream = new FileStream(GetContainingPath() + texturePath, FileMode.Open);
				return Texture2D.FromStream(graphics, fileStream);
			} catch (Exception e) when (e is FileNotFoundException || e is DirectoryNotFoundException) {
				Debug.WriteLine("Type " + GetReferencer() + " was unable to load the texture " + path + "(" + (GetContainingPath() + texturePath) + ")");
				return null;
			}
		}

		private void LoadScripts() {
			if(ScriptFiles == null) {
				if (File.Exists(GetContainingPath() + Name + ".lua")) {
					ScriptFiles = new string[] { Name + ".lua" };
				}
			}
			string combinedScript = "";
			foreach(string scriptFile in ScriptFiles) {
				Debug.WriteLine("Reference " + GetReferencer() + " imported script file " + scriptFile + ".");
				combinedScript += File.ReadAllText(GetContainingPath() + scriptFile) + "\n";
			}

			try {
				CompiledScript = new Script();
				CompiledScript.DoString(combinedScript);
				foreach(Type type in UserData.GetRegisteredTypes().Where(t => t.Namespace == "CoreEngine.Lua")) {
					CompiledScript.Globals[type.Name] = type;
				}
			} catch (SyntaxErrorException e) {
				Debug.WriteLine("Lua scripts for " + GetReferencer() + " failed to compile.");
			}
		}

		public DynValue AttemptLuaFunctionCall(string functionName, params object[] arguments) {
			try {
				if (CompiledScript.Globals[functionName] != null) {
					return CompiledScript.Call(CompiledScript.Globals[functionName], arguments);
				}
			} catch (ScriptRuntimeException e) {
				Debug.WriteLine("Failed Lua script in reference " + GetReferencer() + ": " + e.ToString());
			}
			return null;

		}

		private string GetContainingPath() {
			return ModuleControl.ModuleDirectory + Module.Definition.ModuleInfo.Name + "/" + TypeDirectory + "/" + Name + "/";
		}

		public string GetReferencer() {
			return ReferenceType.Name + "." + Name;
		}
	}
}
