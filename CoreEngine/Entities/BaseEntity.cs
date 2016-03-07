using CoreEngine.Scripting;
using CoreEngine.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace CoreEngine.Entities {
	public class BaseEntity {
		protected static Dictionary<string, Texture2D> TextureRegistry = new Dictionary<string, Texture2D>();

		protected static DirectoryInfo SourceDirectory;
		public static string EntityType = "__";
		public static string Name = "__";

		public string TestName = "ayylmao";

		private static bool HasRegistered = false;

		public BaseEntity() {
			if(!HasRegistered) {
				Debug.WriteLine("Registering");
				Register();
				HasRegistered = true;
			}
			Create();
		}


		protected Texture2D GetTexture(string key) {
			try {
				return TextureRegistry[key];
			} catch (KeyNotFoundException) {
				Debug.WriteLine("Item \"" + key + "\" not found in texture registry for " + GetReferencer());
				return null;
			}
		}

		protected virtual void Register() { }
		protected virtual void Create() { }

		protected Texture2D RegisterTexture(string name, string fileName) {
			try {
				FileStream fileStream = new FileStream(SourceDirectory.FullName + "\\" + fileName, FileMode.Open);
				Texture2D texture = Texture2D.FromStream(GameServices.GetService<GraphicsDevice>(), fileStream);
				TextureRegistry.Add(name, texture);
				fileStream.Close();
				return texture;
			} catch(Exception e) when(e is FileNotFoundException || e is DirectoryNotFoundException) {
				Debug.WriteLine(GetReferencer() + "was unable to load the texture " + SourceDirectory.FullName + "\\" + fileName + " (Are you sure you're using relative paths?)");
				return null;
			}
		}

		public string GetReferencer() {
			return EntityType + "." + Name;
		}
	}
}
