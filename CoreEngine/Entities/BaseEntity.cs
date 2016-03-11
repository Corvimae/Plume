﻿using CoreEngine.Entities.Interfaces;
using CoreEngine.Modularization;
using CoreEngine.Scripting;
using CoreEngine.Utilities;
using IronRuby.Builtins;
using IronRuby.Runtime;
using IronRuby.Runtime.Calls;
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

		protected string Identifier;

		public BaseEntity() {}


		public void Initialize(EntityData metadata, params object[] arguments) {
			Metadata = metadata;
			Create(arguments);
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
		protected virtual void Create(params object[] arguments) { }

		protected virtual void Destroy() {
			if(Identifier != null) {
				EntityController.UnregisterEntity(Identifier);
			}
		}

		protected Texture2D RegisterTexture(string name, string fileName) {
			try {
				if(!Metadata.TextureRegistry.ContainsKey(name)) {
					FileStream fileStream = new FileStream(Metadata.SourceDirectory.FullName + "/" + fileName, FileMode.Open);
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

		public void SetPosition(float x, float y) {
			Position = new Vector2(x, y);
		}

		public void SetDrawDimensions(int width, int height) {
			DrawDimensions = new Vector2(width, height);
		}

		public void RegisterAs(string identifier) {
			EntityController.UnregisterEntity(Identifier);
			Identifier = identifier;
			EntityController.RegisterEntity(identifier, this);
		}


		public Dictionary<int, DynamicDelegate> GetDrawActionRegistry() {
			return Metadata.DrawActionRegistry;
		}
		protected void DrawOnLayer(int level, RubySymbol method) {
			if(!Metadata.DrawActionRegistry.ContainsKey(level)) {
				Metadata.DrawActionRegistry.Add(level, CreateDynamicDelegateForRubyMethod(method));
			}
		}

		protected void DrawOnLayer(int level, string method) {
			if(!Metadata.DrawActionRegistry.ContainsKey(level)) {
				Metadata.DrawActionRegistry.Add(level, CreateDynamicDelegateForCSharpMethod(method));
			}
		}

		protected void UnregisterDrawOnLayer(int level) {
			if(Metadata.DrawActionRegistry.ContainsKey(level)) {
				Metadata.DrawActionRegistry.Remove(level);
			}
		}

		private DynamicDelegate CreateDynamicDelegateForRubyMethod(RubySymbol method) {
			CoreScript script = ModuleController.FindEntityRecordByReferencer(GetReferencer());
			RubyMethodInfo info = script.Engine.Operations.GetMember(this, (string)method.String).Info;
			var rawDelegate = info.GetType().InvokeMember(
					"GetDelegate",
					BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic,
					null,
					info,
					new object[] { }
				);
			return new DynamicDelegate(rawDelegate, false, info.GetArity());
		}

		private DynamicDelegate CreateDynamicDelegateForCSharpMethod(string method) {
			return new DynamicDelegate(Delegate.CreateDelegate(this.GetType(), this, method), true, this.GetType().GetMethod(method).GetParameters().Length);
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
