using IronRuby.Builtins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CoreEngine.Utilities {
	static class RubyConverter {
		public static Dictionary<string, dynamic> ConvertHashToDictionary(Hash rubyHash) {
			Dictionary<string, dynamic> dictionary = new Dictionary<string, dynamic>();
			foreach(KeyValuePair<object, object> value in rubyHash) {
				if(value.Key.GetType().Name == "RubySymbol") {
					dictionary.Add(value.Key.ToString(), value.Value);
				} else {
					dictionary.Add((string) value.Key, value.Value);
				}
			}
			return dictionary;
		}
	}
}
