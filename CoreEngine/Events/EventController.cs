using System;
using System.Collections.Generic;
using System.Linq;
using CoreEngine.Modularization;
using System.Diagnostics;

namespace CoreEngine.Events {
	public static class EventController {
		public static Dictionary<EventDefinition, SortedDictionary<int, List<Action<EventData>>>> EventRegistry 
		= new Dictionary<EventDefinition, SortedDictionary<int, List<Action<EventData>>>>();

		static EventController() {
			RegisterEvent("click");
		}
	
		public static EventDefinition RegisterEvent(string name) {
			if(!EventRegistry.Any(x => x.Key.Name == name)) {
				EventDefinition definition = new EventDefinition(name);
				EventRegistry.Add(definition, new SortedDictionary<int , List<Action<EventData>>>(Comparer<int>.Create((x, y) => y.CompareTo(x))));
				Debug.WriteLine("Event " + name + " registered.");
				return definition;
			} else {
				Debug.WriteLine("Event " + name + " already exists, returning reference to its definition.");
				return EventRegistry.First(x => x.Key.Name == name).Key;
			}
		}

		public static void CallOnEvent(string name, int priority, Action<EventData> method) {
			if(EventRegistry.Keys.Any(x => x.Name == name)) {
				KeyValuePair<EventDefinition, SortedDictionary<int, List<Action<EventData>>>> registryItem = EventRegistry.First(x => x.Key.Name == name);
				if(!registryItem.Value.Keys.Contains(priority)) {
					registryItem.Value[priority] = new List<Action<EventData>>();
				}
				registryItem.Value[priority].Add(method);
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

		public static void Fire(string name, EventData eventData) {
			if(EventRegistry.Any(x => x.Key.Name == name)) {	
				KeyValuePair<EventDefinition, SortedDictionary<int, List<Action<EventData>>>> registryItem = EventRegistry.First(x => x.Key.Name == name);
				foreach(KeyValuePair<int, List<Action<EventData>>> level in registryItem.Value) {
					foreach(Action<EventData> request in level.Value) {
						request.Invoke(eventData);
						if(!eventData.ContinuePropagating) return;
					}
				}
			} else {
				throw new InvalidEventException(name);
			}
		}

		public static void Fire(string name, Dictionary<string, object> hash) {
			Fire(name, new EventData(hash));
		}

		public static void Fire(string name) {
			Fire(name, new EventData());
		}
	}

	class InvalidEventException : Exception {
		public string Name;

		public InvalidEventException(string name) {
			Name = name;
		}
	}
}

