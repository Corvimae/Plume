using PlumeAPI.Entities;
using PlumeAPI.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Modularization {
	public class PlumeModule {

		public Module Module { get; set; }
		public virtual void Register() {
			if(ModuleController.Environment == PlumeEnvironment.Client) {
				RegisterClient();
			} else {
				RegisterServer();
			}
		}
		public virtual void RegisterClient() { }
		public virtual void RegisterServer() { }

		public virtual void Update() {
			if(ModuleController.Environment == PlumeEnvironment.Client) {
				UpdateClient();
			} else {
				UpdateServer();
			}
		}
		public virtual void UpdateClient() { }
		public virtual void UpdateServer() { }

		public virtual void AfterLoad() {
			if(ModuleController.Environment == PlumeEnvironment.Client) {
				AfterLoadClient();
			} else {
				AfterLoadServer();
			}
		}
		public virtual void AfterLoadClient() { }

		public virtual void AfterLoadServer() { }
		public virtual void Draw() { }

		public virtual void UserConnected(Client user) { }
		public virtual void UserDisconnected(Client user) { }
		public virtual void UserFullyLoaded(Client user) { }

	}
}
