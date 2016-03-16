using Lidgren.Network;
using PlumeAPI.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlumeServer.Networking {
	class PlumeServerClient : Client {
		public PlumeServerClient(string name, NetConnection connection) : base(name, connection) { }
		public override void Message(MessageHandler handler) {
			Console.WriteLine("Sending " + handler.Name + "Message to " + Name);
			MessageDispatch.Send(handler, this);
		}
	}
}
