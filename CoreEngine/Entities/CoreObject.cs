using System;
using CoreEngine.Modularization;
using IronRuby.Builtins;
using CoreEngine.Events;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using CoreEngine.Utilities;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CoreEngine.Entities {
	public class CoreObject {
		protected CoreScript ReferenceScript;
		public CoreObjectData Metadata;

		internal void SetReferenceScript(CoreScript reference) {
			ReferenceScript = reference;
		}

		protected Texture2D RegisterTexture(string name, string fileName) {
			try {
				if(!Metadata.TextureRegistry.ContainsKey(name)) {
					FileStream fileStream = new FileStream(ReferenceScript.SourceFile.Directory.FullName + "/" + fileName, FileMode.Open);
					Texture2D texture = Texture2D.FromStream(GameServices.GetService<GraphicsDevice>(), fileStream);
					Metadata.TextureRegistry.Add(name, texture);
					fileStream.Close();
					return texture;
				} else {
					return Metadata.TextureRegistry[name];
				}
			} catch(Exception e) when(e is FileNotFoundException || e is DirectoryNotFoundException) {
				Debug.WriteLine(ReferenceScript.SourceFile.Name + " was unable to load the texture " + ReferenceScript.SourceFile.Directory.FullName + 
												"/" + fileName +	" (Are you sure you're using relative paths?)");
			}
			return null;
		}

		protected Animation RegisterAnimation(string name, string textureName, int width, int height, int frameDuration, int totalFrames) {
			if(!Metadata.AnimationRegistry.ContainsKey(name)) {
				Texture2D texture = GetTexture(textureName);
				if(texture != null) {
					Animation animation = new Animation(texture, new Vector2(width, height), frameDuration, totalFrames);
					Metadata.AnimationRegistry.Add(name, animation);
					return animation;
				} else {
					Debug.WriteLine("Unable to register animation " + name + ".");
					return null;
				}
			} else {
				return Metadata.AnimationRegistry[name];
			}
		}

		protected Texture2D GetTexture(string key) {
			try {
				return Metadata.TextureRegistry[key];
			} catch(KeyNotFoundException) {
				Debug.WriteLine("Item \"" + key + "\" not found in texture registry for " + ReferenceScript.SourceFile.Name);
				return null;
			}
		}

		protected Animation GetAnimationInstance(string key) {
			try {
				return Metadata.AnimationRegistry[key].Clone();
			} catch(KeyNotFoundException) {
				Debug.WriteLine("Item \"" + key + "\" not found in animation registry for " + ReferenceScript.SourceFile.Name);
				return null;
			}
		}

		public void CallOnEvent(string eventName, int priority, RubySymbol method) {
			EventController.CallOnEvent(eventName, method, ReferenceScript, priority, this);
		}
	}
}

