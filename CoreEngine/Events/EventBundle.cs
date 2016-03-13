using System;
using System.Collections.Generic;
using IronRuby.Builtins;
using CoreEngine.Utilities;

namespace CoreEngine.Events {
	public class EventBundle {
		public Hash Content;

		public EventBundle(Hash hash) {
			Content = hash;
		}

		public EventBundle(Dictionary<object, object> items) {
			Content = new Hash(items);
		}

		public Dictionary<string, dynamic> ToDictionary() {
			return RubyConverter.ConvertHashToDictionary(Content);
		}
	}
}

