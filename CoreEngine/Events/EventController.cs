using System;
using System.Collections.Generic;
using System.Linq;
using IronRuby.Builtins;
using CoreEngine.Modularization;
using System.Diagnostics;

namespace CoreEngine.Events {
	public static class EventController {
		public static Dictionary<EventDefinition, SortedDictionary<int, List<EventRequest>>> EventRegistry 
		= new Dictionary<EventDefinition, SortedDictionary<int, List<EventRequest>>>();

		static EventController() {
			RegisterEvent("click");
		}
	
		public static EventDefinition RegisterEvent(string name) {
			if(!EventRegistry.Any(x => x.Key.Name == name)) {
				EventDefinition definition = new EventDefinition(name);
				EventRegistry.Add(definition, new SortedDictionary<int , List<EventRequest>>(Comparer<int>.Create((x, y) => y.CompareTo(x))));
				Debug.WriteLine("Event " + name + " registered.");
				return definition;
			} else {
				Debug.WriteLine("Event " + name + " already exists, returning reference to its definition.");
				return EventRegistry.First(x => x.Key.Name == name).Key;
			}
		}

		public static void CallOnEvent(string name, RubySymbol method, CoreScript script, int priority, object instance) {
			if(EventRegistry.Keys.Any(x => x.Name == name)) {
				KeyValuePair<EventDefinition, SortedDictionary<int, List<EventRequest>>> registryItem = EventRegistry.First(x => x.Key.Name == name);
				if(!registryItem.Value.Keys.Contains(priority)) {
					registryItem.Value[priority] = new List<EventRequest>();
				}
				registryItem.Value[priority].Add(new EventRequest(method, script, instance));
			} else {
				throw new InvalidEventException(name);
			}
		}

		public static void UnregisterEvent(string name) {
			if(EventRegistry.Any(x => x.Key.Name == name)) {
				EventRegistry.Remove(EventRegistry.First(x => x.Key.Name == name).Key);
			} else {
				throw new InvalidEventException(name);
			}
		}

		public static EventBundle Fire(string name, EventBundle bundle) {
			if(EventRegistry.Any(x => x.Key.Name == name)) {	
				KeyValuePair<EventDefinition, SortedDictionary<int, List<EventRequest>>> registryItem = EventRegistry.First(x => x.Key.Name == name);
				foreach(KeyValuePair<int, List<EventRequest>> level in registryItem.Value) {
					foreach(EventRequest request in level.Value) {
						if(request.Delegate.IsCSharp) {
							bundle = request.Delegate.Delegate.Invoke(bundle);
						} else {
							bundle = request.Delegate.Delegate.Invoke(request.Instance, null, bundle);
						}
					}
				}
			} else {
				throw new InvalidEventException(name);
			}
			return bundle;
		}

		public static void Fire(string name, Hash hash) {
			Fire(name, new EventBundle(hash));
		}
	}

	class InvalidEventException : Exception {
		public string Name;

		public InvalidEventException(string name) {
			Name = name;
		}
	}
}

