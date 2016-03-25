using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Attributes {
	[AttributeUsage(AttributeTargets.Property)]
	public class SyncableAttribute : Attribute {
		public SyncableAttribute() { }
	}
}
