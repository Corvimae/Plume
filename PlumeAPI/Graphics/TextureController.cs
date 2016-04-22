using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlumeAPI.Entities;
using PlumeAPI.Modularization;
using PlumeAPI.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Graphics {
	public class TextureController {
		protected static Dictionary<string, Texture2D> TextureRegistry = new Dictionary<string, Texture2D>();
		protected static Dictionary<string, Animation> AnimationRegistry = new Dictionary<string, Animation>();

		public static Texture2D RegisterTexture(string name, string fileName, Module module) {
			string path = module.Directory.FullName + "/Assets/" + fileName;
			try {
				if(!TextureRegistry.ContainsKey(name)) {
					FileStream fileStream = new FileStream(path, FileMode.Open);
					Texture2D texture = Texture2D.FromStream(GameServices.GetService<GraphicsDevice>(), fileStream);
					TextureRegistry.Add(name, texture);
					fileStream.Close();
					return texture;
				} else {
					return TextureRegistry[name];
				}
			} catch(Exception e) when(e is FileNotFoundException || e is DirectoryNotFoundException) {
				Console.WriteLine("Unable to load the texture " + path + "(Are you sure you're using relative paths?)");
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
					Console.WriteLine("Unable to register animation " + name + ".");
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
				Console.WriteLine("Item \"" + key + "\" not found in texture registry");
				return null;
			}
		}

		public static Animation GetAnimationInstance(string key) {
			try {
				return AnimationRegistry[key].Clone();
			} catch(KeyNotFoundException) {
				Console.WriteLine("Item \"" + key + "\" not found in animation registry");
				return null;
			}
		}
	}
}
