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
using PlumeAPI.Entities.Interfaces;

namespace PlumeAPI.Entities {
	public class CoreObject : IUpdatableEntity, IRegisterableEntity {

		public int Id;
		protected static Modularization.Module Module;
		public virtual void Register() {
			if(ModuleController.Environment == PlumeEnvironment.Client) {
				RegisterClient();
			} else {
				RegisterServer();
			}
		}
		public virtual void RegisterClient() { }
		public virtual void RegisterServer() { }

		public virtual void Update() {
			if(ModuleController.Environment == PlumeEnvironment.Client) {
				UpdateClient();
			} else {
				UpdateServer();
			}
		}
		public virtual void UpdateClient() { }
		public virtual void UpdateServer() { }

		public static void SetModuleData(Modularization.Module module) {
			Module = module;
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

		public string GetName() {
			return GetType().FullName;
		}
		public virtual string GetLocalName() {
			return GetType().Name;
		}

		public Modularization.Module GetModule() {
			return Module;
		}

		public void CallOnEvent(string eventName, int priority, string method) {
			try {
				EventController.CallOnEvent(eventName, priority, (Action<EventData>)GetDelegate(method));
			} catch (InvalidCastException) {
				Console.WriteLine(method + " does not contain the correct parameters to be used as an event callback. void func(EventData)");
			}
		}
	}
}

