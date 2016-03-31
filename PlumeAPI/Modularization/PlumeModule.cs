using PlumeAPI.Entities;
using PlumeAPI.Entities.Interfaces;
using PlumeAPI.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Modularization {
	public class PlumeModule : CoreObject {
		public virtual void AfterLoad() { }
		public virtual void Draw() { }

		public virtual void UserConnected(Client user) { }
		public virtual void UserDisconnected(Client user) { }

		public virtual void UserFullyLoaded(Client user) { }

	}
}
