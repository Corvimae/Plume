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
using PlumeAPI.World;
using PlumeAPI.Entities.Components;
using Newtonsoft.Json.Linq;

namespace PlumeAPI.Entities {
	public class BaseEntity {
		public int Id { get; set; }
		public string Name { get; set; }
		public EntityScope Scope { get; set; }
		public bool Prototypal { get; set; } = false;
		protected List<EntityComponent> Components = new List<EntityComponent>();
		internal JObject Definition {get;set;}
		public BaseEntity() {}

		public void RegisterToScope(EntityScope scope) {
			ScopeController.RegisterEntity(scope, this);
			foreach(EventTriggeredComponent component in GetDerivativeComponents<EventTriggeredComponent>()) {
				component.Registration.Scope = scope;
			}
		}

		public void AddComponent(EntityComponent component) {
			Components.Add(component);
			component.Entity = this;
		}

		public void RemoveComponent(string name) {
			Components.RemoveAll(x => x.GetType().Name == name);
		}

		public T GetDerivativeComponent<T>() where T : EntityComponent {
			return (T)Components.FirstOrDefault(x => x.GetType() == typeof(T) || x.GetType().IsSubclassOf(typeof(T)));
		}

		public T[] GetDerivativeComponents<T>() where T : EntityComponent {
			return Components.Where(x => x.GetType() == typeof(T) || x.GetType().IsSubclassOf(typeof(T))).Select(x => (T) x).ToArray<T>();
		}

		public T GetComponent<T>() where T : EntityComponent {
			return (T)Components.FirstOrDefault(x => x.GetType() == typeof(T));
		}

		public T[] GetComponents<T>() where T : EntityComponent {
			return Components.Where(x => x.GetType() == typeof(T)).Select(x => (T) x).ToArray<T>();
		}

		public EntityComponent[] GetComponents() {
			return Components.ToArray();
		}

		public bool TryGetComponent<T>(ref T component) where T : EntityComponent {
			component = GetComponent<T>();
			return component != default(T);
		}
		public bool TryGetDerivativeComponent<T>(ref T component) where T : EntityComponent {
			component = GetDerivativeComponent<T>();
			return component != default(T);
		}

		public bool HasComponent(string name) {
			return Components.Any(x => x.GetType().Name == name);
		}

		public bool HasDerivativeComponent<T>() where T : EntityComponent {
			return Components.Any(x => x.GetType() == typeof(T) || x.GetType().IsSubclassOf(typeof(T)));
		}

		public void SendMessage<T>(string methodName, params object[] arguments) where T : EntityComponent {
			foreach(T component in GetDerivativeComponents<T>()) {
				typeof(T).GetMethod(methodName)?.Invoke(component, arguments);
			}
		}

		public BaseEntity Clone() {
			BaseEntity entity = new BaseEntity();
			entity.Name = Name;
			entity.Prototypal = false;
			entity.Definition = (JObject) Definition.DeepClone();
			entity.Components = new List<EntityComponent>();
			// Load each component
			foreach(EntityComponent component in Components) {
				entity.Components.Add(component.Clone(entity));
			}

			//Run setup for each component
			foreach(EntityComponent component in entity.Components) {
				component.Setup();
			}
			return entity;
		}
	}
}
