using CoreEngine.Attributes;
using CoreEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEngine.Modularization {
	public class PlumeModule : CoreObject {
		public virtual void AfterLoad() { }
		public virtual void Draw() { }
		public virtual void Update() { }

	}
}
