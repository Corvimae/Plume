using CoreEngine.Entities.Interfaces;
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
	public class BaseEntity : CoreObject, IDrawableEntity, IUpdatableEntity {
		
		protected Vector2 Position;
		protected Vector2 DrawDimensions;

		protected string Identifier;

		public BaseEntity() {}

		public void Initialize(EntityData metadata, params object[] arguments) {
			Metadata = metadata;
			Create(arguments);
		}

		protected virtual void Register() { }
		protected virtual void Create(params object[] arguments) { }

		public virtual void Destroy() {
			if(Identifier != null) {
				EntityController.UnregisterEntity(Identifier);
			}
		}

		public EntityData GetMetadata() {
			return (EntityData) Metadata;
		}

		public string GetReferencer() {
			return GetMetadata().GetReferencer();
		}

		public bool HasPropertyEnabled(string setting) {
			return GetMetadata().EntityProperties[setting];
		}

		protected void SetEntityProperties(Hash properties) {
			SetEntityProperties(RubyConverter.ConvertHashToDictionary(properties));
		}

		protected void SetEntityProperties(Dictionary<string, dynamic> properties) {
			foreach(KeyValuePair<string, dynamic> property in properties) {
				if(GetMetadata().EntityProperties.ContainsKey(property.Key)) {
					GetMetadata().EntityProperties[property.Key] = (bool)property.Value;
				} else {
					throw new InvalidEntityPropertyException(property.Key, GetReferencer());
				}
			}
		}

		public int GetDrawLayer() {
			return GetMetadata().DrawLayer;
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
			return GetMetadata().DrawActionRegistry;
		}
		protected void DrawOnLayer(int level, RubySymbol method) {
			if(!GetMetadata().DrawActionRegistry.ContainsKey(level)) {
				GetMetadata().DrawActionRegistry.Add(
					level, 
					DynamicDelegate.CreateDelegateForRubyMethod(method, this, ReferenceScript)
				);
			}
		}

		protected void DrawOnLayer(int level, string method) {
			if(!GetMetadata().DrawActionRegistry.ContainsKey(level)) {
				GetMetadata().DrawActionRegistry.Add(level, DynamicDelegate.CreateDelegateForCSharpMethod(method, this));
			}
		}

		protected void UnregisterDrawOnLayer(int level) {
			if(GetMetadata().DrawActionRegistry.ContainsKey(level)) {
				GetMetadata().DrawActionRegistry.Remove(level);
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
