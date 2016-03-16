using Lidgren.Network;
using PlumeAPI.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeAPI.Networking {
	public class Client {
		public string Name;
		public NetConnection Connection;
		public Client(string name, NetConnection connection) { 
			this.Name = name;
			this.Connection = connection;
		}

		public virtual void Message(MessageHandler handler) { }
	}
}
