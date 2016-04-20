using System;
using System.Collections.Generic;
using System.Linq;
using PlumeAPI.Modularization;
using System.Diagnostics;
using PlumeAPI.Entities;
using PlumeAPI.World;

namespace PlumeAPI.Events {
	public static class EventController {
		public static Dictionary<string, SortedDictionary<int, List<EventRegistration>>> EventRegistry 
		= new Dictionary<string, SortedDictionary<int, List<EventRegistration>>>();

		static EventController() {
			RegisterEvent("update");
			RegisterEvent("draw");
			RegisterEvent("click");
		}
	
		public static void RegisterEvent(string name) {
			if(!EventRegistry.Any(x => x.Key == name)) {
				EventRegistry.Add(name, new SortedDictionary<int , List<EventRegistration>>(Comparer<int>.Create((x, y) => y.CompareTo(x))));
				Console.WriteLine("Event " + name + " registered.");
			} else {
				Console.WriteLine("Event " + name + " already exists.");
			}
		}

		public static EventRegistration CallOnEvent(string name, int priority, Action<EventData> method, EntityScope scope) {
			if(EventRegistry.Keys.Any(x => x == name)) {
				KeyValuePair<string, SortedDictionary<int, List<EventRegistration>>> registryItem = EventRegistry.First(x => x.Key == name);
				if(!registryItem.Value.Keys.Contains(priority)) {
					registryItem.Value[priority] = new List<EventRegistration>();
				}
				EventRegistration registration = new EventRegistration(method, scope);
				registryItem.Value[priority].Add(registration);
				return registration;
			} else {
				throw new InvalidEventException(name);
			}
		}

		public static EventRegistration CallOnEvent(string name, int priority, Action<EventData> method) {
			return CallOnEvent(name, priority, method, null);
		}

		public static void UnregisterEvent(string name) {
			if(EventRegistry.Any(x => x.Key == name)) {
				EventRegistry.Remove(EventRegistry.First(x => x.Key == name).Key);
			} else {
				throw new InvalidEventException(name);
			}
		}

		public static void Fire(string name, EventData eventData, EntityScope scope) {
			if(EventRegistry.Any(x => x.Key == name)) {	
				KeyValuePair<string, SortedDictionary<int, List<EventRegistration>>> registryItem = EventRegistry.First(x => x.Key == name);
				for(int i = 0; i < registryItem.Value.Count(); i++) {
					List<EventRegistration> allCallbacks = registryItem.Value.ElementAt(i).Value;
					if(scope != null) {
						allCallbacks = allCallbacks.Where(x => x.Scope == scope).ToList();
					}
					EventRegistration[] callbacks = allCallbacks.ToArray();
					foreach(EventRegistration request in callbacks) {
						request.Callback.Invoke(eventData);
						if(!eventData.ContinuePropagating) return;
					}
				}
			} else {
				throw new InvalidEventException(name);
			}
		}

		public static void Fire(string name, EventData eventData) {
			Fire(name, eventData, null);
		}

		public static void Fire(string name, Dictionary<string, object> hash) {
			Fire(name, new EventData(hash));
		}

		public static void Fire(string name, Dictionary<string, object> hash, EntityScope scope) {
			Fire(name, new EventData(hash), scope);
		}


		public static void Fire(string name) {
			Fire(name, new EventData());
		}
		public static void Fire(string name, EntityScope scope) {
			Fire(name, new EventData(), scope);
		}
	}

	public class EventRegistration {
		public Action<EventData> Callback;
		public EntityScope Scope; 
		public EventRegistration(Action<EventData> callback, EntityScope scope) {
			Callback = callback;
			Scope = scope;
		}
	}

	class InvalidEventException : Exception {
		public string Name;

		public InvalidEventException(string name) {
			Name = name;
		}
	}
}

