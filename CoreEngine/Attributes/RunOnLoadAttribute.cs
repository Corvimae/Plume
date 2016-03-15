using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEngine.Attributes {
	public class RunOnLoadAttribute : Attribute {
		public string[] Exceptions;
		
		public RunOnLoadAttribute() {
			Exceptions = new string[] { };
		}
		public RunOnLoadAttribute(string[] exceptions) {
			Exceptions = exceptions;
		}
	}
}
