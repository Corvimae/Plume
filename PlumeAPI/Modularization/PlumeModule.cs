using PlumeAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Modularization {
	public class PlumeModule {
		public Module Module { get; set; }

		public virtual void Register() {}

		public virtual void Update() {}

		public virtual void AfterLoad() {	}

		public virtual void Draw() { }
	}
}
