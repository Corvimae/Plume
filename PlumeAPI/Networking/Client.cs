using Lidgren.Network;
using PlumeAPI.Networking;
using PlumeAPI.Networking.Builtin;
using PlumeAPI.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Networking {
	public class Client {
		public string Name;
		public NetConnection Connection;
		public EntityScope Scope;
		public Client(string name, NetConnection connection) { 
			this.Name = name;
			this.Connection = connection;
		}

		public void SetScope(EntityScope scope) {
			this.Scope = scope;
		}

		public void SetAndSendScope(EntityScope scope) {
			SetScope(scope);
			Message(new SendScopeMessageHandler(Scope));
		}

		public void Message(MessageHandler handler) {
			Console.WriteLine("Sending " + handler.Name + "Message to " + Name);
			ServerMessageDispatch.Send(handler, this);
		}

		public void SendInitialConnectionData() {
			//TODO: Load from save
			SetAndSendScope(ScopeController.GetScope("MyMap"));
		}
	}
}
