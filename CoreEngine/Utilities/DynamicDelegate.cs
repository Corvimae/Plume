using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreEngine.Utilities {
	public class DynamicDelegate {

		public dynamic Delegate;
		public bool IsCSharp;
		public int Arity;

		public DynamicDelegate(dynamic del, bool isCSharp, int arity) {
			Delegate = del;
			IsCSharp = isCSharp;
			Arity = arity;
		}
	}
}
