using Lidgren.Network;
using PlumeAPI.Entities;
using PlumeAPI.Modularization;
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
		Dictionary<string, object> _clientStorage = new Dictionary<string, object>();
		public long LastProcessedTick { get; set; }

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
			Console.WriteLine("Sending " + handler.GetType().FullName + " message to " + Name);
			ServerMessageDispatch.Send(handler, this);
		}

		public void SendInitialConnectionData() {
			//TODO: Load from save
			Message(new SyncEntityIdsMessageHandler());
			SetAndSendScope(ScopeController.GetScope("MyMap"));
			ModuleController.InvokeStartupMethod("UserFullyLoaded", this);
		}

		public object this[string key] {
			get {
				if(_clientStorage.ContainsKey(key)) {
					return _clientStorage[key];
				} else {
					return null;
				}
			}
			set {
				if(!_clientStorage.ContainsKey(key)) {
					_clientStorage.Add(key, value);
				} else {
					_clientStorage[key] = value;
				}
			}
		}
	}
}
