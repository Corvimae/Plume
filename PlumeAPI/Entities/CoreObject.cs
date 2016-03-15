using System;
using PlumeAPI.Modularization;
using PlumeAPI.Events;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Linq;
using PlumeAPI.Utilities;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Reflection;
using System.Linq.Expressions;

namespace PlumeAPI.Entities {
	public class CoreObject {
		protected static Dictionary<string, Texture2D> TextureRegistry = new Dictionary<string, Texture2D>();
		protected static Dictionary<string, Animation> AnimationRegistry = new Dictionary<string, Animation>();

		protected static Modularization.Module Module;

		public static string Name;

		public static void Register() { }

		public static Texture2D RegisterTexture(string name, string fileName) {
			try {
				if(!TextureRegistry.ContainsKey(name)) {
					FileStream fileStream = new FileStream(Module.Directory.FullName + "/Assets/" + fileName, FileMode.Open);
					Texture2D texture = Texture2D.FromStream(GameServices.GetService<GraphicsDevice>(), fileStream);
					TextureRegistry.Add(name, texture);
					fileStream.Close();
					return texture;
				} else {
					return TextureRegistry[name];
				}
			} catch(Exception e) when(e is FileNotFoundException || e is DirectoryNotFoundException) {
				Debug.WriteLine(GetReferencer() + " was unable to load the texture " + Module.Directory.FullName + 
												"/" + fileName +	" (Are you sure you're using relative paths?)");
			}
			return null;
		}

		public static Animation RegisterAnimation(string name, string textureName, int width, int height, int frameDuration, int totalFrames) {
			if(!AnimationRegistry.ContainsKey(name)) {
				Texture2D texture = GetTexture(textureName);
				if(texture != null) {
					Animation animation = new Animation(texture, new Vector2(width, height), frameDuration, totalFrames);
					AnimationRegistry.Add(name, animation);
					return animation;
				} else {
					Debug.WriteLine("Unable to register animation " + name + ".");
					return null;
				}
			} else {
				return AnimationRegistry[name];
			}
		}

		public static Texture2D GetTexture(string key) {
			try {
				return TextureRegistry[key];
			} catch(KeyNotFoundException) {
				Debug.WriteLine("Item \"" + key + "\" not found in texture registry for " + Name);
				return null;
			}
		}

		public static Animation GetAnimationInstance(string key) {
			try {
				return AnimationRegistry[key].Clone();
			} catch(KeyNotFoundException) {
				Debug.WriteLine("Item \"" + key + "\" not found in animation registry for " + Name);
				return null;
			}
		}

		public static void SetModuleData(Modularization.Module module, string name) {
			Module = module;
			Name = name;
			Debug.WriteLine("Name set to " + Name);
		}

		public Delegate GetDelegate(string method) {
			MethodInfo methodInfo = GetType().GetMethod(method);
			return methodInfo.CreateDelegate(
				Expression.GetDelegateType(
					(from parameter in methodInfo.GetParameters() select parameter.ParameterType).
				Concat(new[] { methodInfo.ReturnType }).
				ToArray()
				),
				this
			);
		}

		public static string GetReferencer() {
			return Module.Definition.ModuleInfo.Name + "." + Name;
		}

		public void CallOnEvent(string eventName, int priority, string method) {
			try {
				EventController.CallOnEvent(eventName, priority, (Action<EventData>)GetDelegate(method));
			} catch (InvalidCastException) {
				Debug.WriteLine(method + " does not contain the correct parameters to be used as an event callback. void func(EventData)");
			}
		}
	}
}

