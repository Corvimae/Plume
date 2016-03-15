using PlumeAPI.Attributes;
using PlumeAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Modularization {
	public class PlumeModule : CoreObject {
		public virtual void AfterLoad() { }
		public virtual void Draw() { }
		public virtual void Update() { }

	}
}
