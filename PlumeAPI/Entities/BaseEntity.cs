﻿using PlumeAPI.Entities.Interfaces;
using PlumeAPI.Events;
using PlumeAPI.Modularization;
using PlumeAPI.Graphics;
using PlumeAPI.Utilities;
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
using Lidgren.Network;

namespace PlumeAPI.Entities {
	public class BaseEntity : CoreObject, IDrawableEntity {

		protected Vector2 Position;
		protected Vector2 DrawDimensions;

		protected string Identifier;

		public Dictionary<int, Action> DrawActionRegistry = new Dictionary<int, Action>();

		public static Dictionary<string, bool> EntityProperties = new Dictionary<string, bool> {
			{ "draw", false },
			{ "update", false },
			{ "click", false }
		};

		public int DrawLayer = 0;

		public BaseEntity() { }

		protected virtual void Create(params object[] arguments) { }

		public virtual void Destroy() { }

		public bool HasPropertyEnabled(string setting) {
			return EntityProperties[setting];
		}

		protected void SetEntityProperties(Dictionary<string, bool> properties) {
			foreach(KeyValuePair<string, bool> property in properties) {
				if(EntityProperties.ContainsKey(property.Key)) {
					EntityProperties[property.Key] = (bool)property.Value;
				} else {
					throw new InvalidEntityPropertyException(property.Key, GetName());
				}
			}
		}

		protected void SetEntityProperty(string property, bool value) {
			if(EntityProperties.ContainsKey(property)) {
				EntityProperties[property] = (bool)value;
			} else {
				throw new InvalidEntityPropertyException(property, GetName());
			}
		}

		public int GetDrawLayer() {
			return DrawLayer;
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

		public Dictionary<int, Action> GetDrawActionRegistry() {
			return DrawActionRegistry;
		}

		protected void DrawOnLayer(int level, string method) {
			if(!DrawActionRegistry.ContainsKey(level)) {
				DrawActionRegistry.Add(level, (Action)GetDelegate(method));
			}
		}

		protected void UnregisterDrawOnLayer(int level) {
			if(DrawActionRegistry.ContainsKey(level)) {
				DrawActionRegistry.Remove(level);
			}
		}

		public Rectangle GetDrawBoundry() {
			return new Rectangle((int)Position.X, (int)Position.Y, (int)DrawDimensions.X, (int)DrawDimensions.Y);
		}

		public virtual void Draw() { }

		public virtual void OnClick(EventData data) { }
		
		public virtual void PackageForInitialTransfer(NetOutgoingMessage message) {
			message.Write(GetName());
			message.Write(Id);
		}

		public virtual void PackageForUpdate(NetOutgoingMessage message) {
			message.Write(Id);
		}

		public virtual void UpdateFromMessage(object[] arguments) {
		}

		public static object[] UnpackageFromUpdate(NetIncomingMessage message) {
			return new object[] { message.ReadInt32() };
		}

		public static object[] UnpackageFromInitialTransfer(NetIncomingMessage message) {
			return new object[] { message.ReadInt32() };
		}
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