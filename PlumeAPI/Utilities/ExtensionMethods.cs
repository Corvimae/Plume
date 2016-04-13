using PlumeAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Utilities {
	static class ExtensionMethods {
		public static bool ChangeKey<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey oldKey, TKey newKey) {
			TValue value;
			if(!dict.TryGetValue(oldKey, out value))
				return false;

			dict.Remove(oldKey);  // do not change order
			dict[newKey] = value;  // or dict.Add(newKey, value) depending on ur comfort
			return true;
		}

		public static T[] TakeAfter<T>(this IEnumerable<T> array, Func<T, bool> lambda) {
			bool found = false;
			List<T> items = new List<T>();
			for(int i = 0; i < array.Count(); i++) {
				T item = array.ElementAt(i);
				if(!found) {
					if(lambda(item)) {
						found = true;
						items.Add(item);
					}
				} else {
					items.Add(item);
				}
			}
			return items.ToArray();
		}
		public static T[] KeepAfter<T>(this SortedSet<T> array, Func<T, bool> lambda) {
			bool found = false;
			List<T> items = new List<T>();
			for(int i = 0; i < array.Count(); i++) {
				T item = array.ElementAt(i);
				if(!found) {
					if(lambda(item)) {
						found = true;
						items.Add(item);
					} else {
						array.Remove(item);
					}
				} else {
					items.Add(item);
				}
			}
			return items.ToArray();
		}


		public static bool ParseToBool(this string str, out bool value) {
			switch(str) {
				case "true":
				case "True":
				case "1":
					value = true;
					return true;
				case "false":
				case "False":
				case "0":
					value = false;
					return true;
				default:
					value = false;
					return false;
			}
		}

	}
}
