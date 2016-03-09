using CoreEngine.Entities.Interfaces;
using CoreEngine.Modularization;
using CoreEngine.Scripting;
using CoreEngine.Utilities;
using IronRuby.Builtins;
using IronRuby.Runtime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CoreEngine.Entities {
	public class BaseEntity : IDrawableEntity, IUpdatableEntity {

		public EntityData Metadata;

		protected Vector2 Position;
		protected Vector2 DrawDimensions;


		public BaseEntity() {}


		public void Initialize(EntityData metadata) {
			Metadata = metadata;
			Create();
		}

		protected Texture2D GetTexture(string key) {
			try {
				return Metadata.TextureRegistry[key];
			} catch(KeyNotFoundException) {
				Debug.WriteLine("Item \"" + key + "\" not found in texture registry for " + GetReferencer());
				return null;
			}
		}

		protected virtual void Register() { }
		protected virtual void Create() { }

		protected Texture2D RegisterTexture(string name, string fileName) {
			try {
				if(!Metadata.TextureRegistry.ContainsKey(name)) {
					FileStream fileStream = new FileStream(Metadata.SourceDirectory.FullName + "\\" + fileName, FileMode.Open);
					Texture2D texture = Texture2D.FromStream(GameServices.GetService<GraphicsDevice>(), fileStream);
					Metadata.TextureRegistry.Add(name, texture);
					fileStream.Close();
					return texture;
				}
			} catch(Exception e) when(e is FileNotFoundException || e is DirectoryNotFoundException) {
				Debug.WriteLine(GetReferencer() + "was unable to load the texture " + Metadata.SourceDirectory.FullName + "\\" + fileName + " (Are you sure you're using relative paths?)");
			}
			return null;

		}

		public string GetReferencer() {
			return Metadata.GetReferencer();
		}

		public bool HasPropertyEnabled(string setting) {
			return Metadata.EntityProperties[setting];
		}

		protected void SetEntityProperties(Hash properties) {
			SetEntityProperties(RubyConverter.ConvertHashToDictionary(properties));
		}

		protected void SetEntityProperties(Dictionary<string, dynamic> properties) {
			foreach(KeyValuePair<string, dynamic> property in properties) {
				if(Metadata.EntityProperties.ContainsKey(property.Key)) {
					Metadata.EntityProperties[property.Key] = (bool)property.Value;
				} else {
					throw new InvalidEntityPropertyException(property.Key, GetReferencer());
				}
			}
		}

		public int GetDrawLayer() {
			return Metadata.DrawLayer;
		}
		public Vector2 GetPosition() {
			return Position;
		}
		public Vector2 GetDimensions() {
			return DrawDimensions;
		}

		public void SetPosition(int x, int y) {
			Position = new Vector2(x, y);
		}

		public void SetDimensions(int width, int height) {
			DrawDimensions = new Vector2(width, height);
		}

		public void RegisterAs(string identifier) {
			EntityController.RegisterEntity(identifier, this);
		}

		public Dictionary<int, dynamic> GetDrawActionRegistry() {
			return Metadata.DrawActionRegistry;
		}
		protected void DrawOnLayer(int level, RubySymbol method) {
			if(!Metadata.DrawActionRegistry.ContainsKey(level)) {
				Metadata.DrawActionRegistry.Add(level, method);
			}
		}
		protected void DrawOnLayer(int level, string method) {
			if(!Metadata.DrawActionRegistry.ContainsKey(level)) {
				Metadata.DrawActionRegistry.Add(level, method);
			}
		}

		protected void UnregisterDrawOnLayer(int level) {
			if(Metadata.DrawActionRegistry.ContainsKey(level)) {
				Metadata.DrawActionRegistry.Remove(level);
			}
		}

		public Rectangle GetDrawBoundry() {
			return new Rectangle((int) Position.X, (int)Position.Y, (int)DrawDimensions.X, (int)DrawDimensions.Y);
		}

		public virtual void Draw() { }

		public virtual void Update() { }

	}

	class InvalidEntityPropertyException : Exception {
		string Key;
		string Referencer;
		public InvalidEntityPropertyException(string key, string Referencer) {
			this.Key = key;
			this.Referencer = Referencer;
		}

		public override string ToString() {
			return Referencer + " attemped to set invalid entity property " + Key + ".";
		}
	}
}
