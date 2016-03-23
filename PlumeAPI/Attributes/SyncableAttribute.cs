using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Attributes {
	[AttributeUsage(AttributeTargets.Parameter)]
	class SyncableAttribute : Attribute {
		public SyncableAttribute() { }
	}
}
